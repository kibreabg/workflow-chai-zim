using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using log4net;
using log4net.Config;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public partial class frmStationaryRequest : POCBasePage, IStationaryRequestView
    {
        private StationaryRequestPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private StationaryRequest _Stationaryrequest;
        private int _leaverequestId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //CheckApprovalSettings();
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                BindSearchStationaryRequestGrid();
                BindStationaryRequestDetails();
                BindInitialValues();
            }
            this._presenter.OnViewLoaded();
        }
        [CreateNew]
        public StationaryRequestPresenter Presenter
        {
            get
            {
                return this._presenter;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this._presenter = value;
                this._presenter.View = this;
            }
        }
        public override string PageID
        {
            get
            {
                return "{334AAED8-456F-44AC-A203-FC4CE87FC3CD}";
            }
        }
        private void CheckApprovalSettings()
        {
            if (_presenter.GetApprovalSetting(RequestType.Stationary_Request.ToString().Replace('_', ' '), 0) == null)
            {
                pnlWarning.Visible = true;
            }
        }
        private void BindInitialValues()
        {
            AppUser CurrentUser = _presenter.CurrentUser();
            txtRequester.Text = CurrentUser.FirstName + " " + CurrentUser.LastName;

            if (_presenter.CurrentStationaryRequest.Id <= 0)
            {
                AutoNumber();
                txtRequestDate.Text = DateTime.Today.Date.ToShortDateString();

            }
        }
        private string AutoNumber()
        {
            return "SR-" + _presenter.CurrentUser().Id.ToString() + "-" + (_presenter.GetLastStationaryRequestId() + 1).ToString();
        }
        private void BindStationaryRequest()
        {

            if (_presenter.CurrentStationaryRequest.Id > 0)
            {
                txtRequestDate.Text = _presenter.CurrentStationaryRequest.RequestedDate.ToShortDateString();
                txtDeliveryDate.Text = _presenter.CurrentStationaryRequest.RequiredDateOfDelivery.ToShortDateString();
                txtDeliverto.Text = _presenter.CurrentStationaryRequest.DeliverTo;
                txtSpecialNeed.Text = _presenter.CurrentStationaryRequest.SpecialNeed;
                txtPurposeOfRequest.Text = _presenter.CurrentStationaryRequest.PurposeOfRequest;
            }
        }
        private void SaveStationaryRequest()
        {
            AppUser CurrentUser = _presenter.CurrentUser();
            try
            {
                _presenter.CurrentStationaryRequest.Requester = CurrentUser.Id;
                _presenter.CurrentStationaryRequest.RequestedDate = Convert.ToDateTime(txtRequestDate.Text);
                _presenter.CurrentStationaryRequest.RequiredDateOfDelivery = Convert.ToDateTime(txtDeliveryDate.Text);
                _presenter.CurrentStationaryRequest.RequestNo = AutoNumber();
                _presenter.CurrentStationaryRequest.DeliverTo = txtDeliverto.Text;
                _presenter.CurrentStationaryRequest.SpecialNeed = txtSpecialNeed.Text;
                _presenter.CurrentStationaryRequest.PurposeOfRequest = txtPurposeOfRequest.Text;
                SaveStationaryRequestStatus();
                GetCurrentApprover();
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
                if (ex.InnerException != null && ex.InnerException.InnerException.Message.Contains("Violation of UNIQUE KEY"))
                {
                    Master.ShowMessage(new AppMessage("Please Click Request button Again, There is a duplicate Number", RMessageType.Error));
                }

            }

        }
        private void SaveStationaryRequestStatus()
        {
            if (_presenter.CurrentStationaryRequest.Id <= 0)
            {
                if (_presenter.GetApprovalSetting(RequestType.Stationary_Request.ToString().Replace('_', ' '), 0) != null)
                {
                    int i = 1;
                    foreach (ApprovalLevel AL in _presenter.GetApprovalSetting(RequestType.Stationary_Request.ToString().Replace('_', ' '), 0).ApprovalLevels)
                    {
                        StationaryRequestStatus PRS = new StationaryRequestStatus();
                        PRS.StationaryRequest = _presenter.CurrentStationaryRequest;
                        if (AL.EmployeePosition.PositionName == "Superviser/Line Manager")
                        {
                            if (_presenter.CurrentUser().Superviser.Value != 0)
                            {
                                PRS.Approver = _presenter.CurrentUser().Superviser.Value;
                            }
                            else
                            {
                                PRS.ApprovalStatus = ApprovalStatus.Approved.ToString();
                                PRS.ApprovalDate = DateTime.Today.Date;
                            }
                        }
                        else
                        {
                            PRS.Approver = _presenter.Approver(AL.EmployeePosition.Id).Id;
                        }
                        PRS.WorkflowLevel = i;
                        i++;
                        _presenter.CurrentStationaryRequest.StationaryRequestStatuses.Add(PRS);

                    }
                }
                else { pnlWarning.Visible = true; }
            }
        }
        private void GetCurrentApprover()
        {
            foreach (StationaryRequestStatus PRS in _presenter.CurrentStationaryRequest.StationaryRequestStatuses)
            {
                if (PRS.ApprovalStatus == null)
                {
                    SendEmail(PRS);
                    _presenter.CurrentStationaryRequest.CurrentApprover = PRS.Approver;
                    _presenter.CurrentStationaryRequest.CurrentLevel = PRS.WorkflowLevel;
                    _presenter.CurrentStationaryRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    break;

                }
            }
        }
        private void SendEmail(StationaryRequestStatus PRS)
        {


            if (_presenter.GetSuperviser(PRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetSuperviser(PRS.Approver).Email, "Stationary Request", _presenter.GetUser(_presenter.CurrentStationaryRequest.Requester).FullName + "' Request for Item procurment No. '" + _presenter.CurrentStationaryRequest.RequestNo + "'");
            }
            else
            {

                EmailSender.Send(_presenter.GetSuperviser(_presenter.GetAssignedJobbycurrentuser(PRS.Approver).AssignedTo).Email, "Stationary Request", _presenter.GetUser(_presenter.CurrentStationaryRequest.Requester).FullName + "' Request for Item procurment No. '" + _presenter.CurrentStationaryRequest.RequestNo + "'");
            }


        }
        public StationaryRequest StationaryRequest
        {
            get
            {
                return _Stationaryrequest;
            }
            set
            {
                _Stationaryrequest = value;
            }
        }
        public string RequestNo
        {
            get { return txtRequestNosearch.Text; }
        }
        public string RequestDate
        {
            get { return txtRequestDatesearch.Text; }
        }
        public int StationaryRequestId
        {
            get
            {
                if (_leaverequestId != 0)
                {
                    return _leaverequestId;
                }
                else
                {
                    return 0;
                }
            }
        }
        private void BindAccount(DropDownList ddlItemAccount)
        {
            ddlItemAccount.DataSource = _presenter.GetItemAccounts();
            ddlItemAccount.DataBind();

        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmStationaryRequest.aspx");
        }
        protected void grvStationaryRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _leaverequestId = Convert.ToInt32(grvStationaryRequestList.SelectedDataKey[0]);
            _presenter.OnViewLoaded();
            BindStationaryRequest();
            BindStationaryRequestDetails();
        }
        protected void grvStationaryRequestList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            _presenter.DeleteStationaryRequest(_presenter.GetStationaryRequestById(Convert.ToInt32(grvStationaryRequestList.DataKeys[e.RowIndex].Value)));

            btnFind_Click(sender, e);
            Master.ShowMessage(new AppMessage("Stationary Request Successfully Deleted", RMessageType.Info));

        }
        protected void grvStationaryRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //LeaveRequest leaverequest = e.Row.DataItem as LeaveRequest;
            //if (leaverequest != null)
            //{
            //    if (leaverequest.GetLeaveRequestStatusworkflowLevel(1).ApprovalStatus != null)
            //    {
            //        e.Row.Cells[5].Enabled = false;
            //        e.Row.Cells[6].Enabled = false;
            //    }

            //}
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //LinkButton db = (LinkButton)e.Row.Cells[5].Controls[0];
                //db.OnClientClick = "return confirm('Are you sure you want to delete this Recieve?');";
            }
        }
        protected void grvStationaryRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvStationaryRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindSearchStationaryRequestGrid();
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
        }
        private void BindSearchStationaryRequestGrid()
        {
            grvStationaryRequestList.DataSource = _presenter.ListStationaryRequests(txtRequestNosearch.Text, txtRequestDatesearch.Text);
            grvStationaryRequestList.DataBind();
        }
        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            _presenter.CancelPage();
        }

        #region StationaryRequestDetail
        private void BindStationaryRequestDetails()
        {
            dgStationaryRequestDetail.DataSource = _presenter.CurrentStationaryRequest.StationaryRequestDetails;
            dgStationaryRequestDetail.DataBind();
        }
        protected void dgStationaryRequestDetail_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgStationaryRequestDetail.EditItemIndex = -1;
            BindStationaryRequestDetails();
        }
        protected void dgStationaryRequestDetail_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgStationaryRequestDetail.DataKeys[e.Item.ItemIndex];
            int PRDId = (int)dgStationaryRequestDetail.DataKeys[e.Item.ItemIndex];
            StationaryRequestDetail prd;

            if (PRDId > 0)
                prd = _presenter.CurrentStationaryRequest.GetStationaryRequestDetail(PRDId);
            else
                prd = _presenter.CurrentStationaryRequest.StationaryRequestDetails[e.Item.ItemIndex];
            try
            {
                if (PRDId > 0)
                {
                    _presenter.CurrentStationaryRequest.RemoveStationaryRequestDetail(id);
                    _presenter.DeleteStationaryRequestDetail(_presenter.GetStationaryRequestDetail(id));
                    _presenter.SaveOrUpdateStationaryRequest(_presenter.CurrentStationaryRequest);
                }
                else
                {
                    _presenter.CurrentStationaryRequest.StationaryRequestDetails.Remove(prd);
                }
                BindStationaryRequestDetails();

                Master.ShowMessage(new AppMessage("Stationary Request Detail was Removed Successfully", RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Stationary Request Detail. " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void dgStationaryRequestDetail_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgStationaryRequestDetail.EditItemIndex = e.Item.ItemIndex;

            BindStationaryRequestDetails();
        }
        protected void dgStationaryRequestDetail_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "AddNew")
            {
                try
                {
                    if (!_presenter.CurrentStationaryRequest.StationaryRequestDetails.Any())
                    {
                        StationaryRequestDetail Detail = new StationaryRequestDetail();
                        DropDownList ddlFAccount = e.Item.FindControl("ddlFAccount") as DropDownList;
                        Detail.ItemAccount = _presenter.GetItemAccount(int.Parse(ddlFAccount.SelectedValue));
                        TextBox txtFAccountCode = e.Item.FindControl("txtFAccountCode") as TextBox;
                        Detail.AccountCode = txtFAccountCode.Text;
                        TextBox txtFItem = e.Item.FindControl("txtFItem") as TextBox;
                        Detail.Item = txtFItem.Text;
                        TextBox txtFQty = e.Item.FindControl("txtFQty") as TextBox;
                        Detail.Qty = Convert.ToInt32(txtFQty.Text);
                        Detail.StationaryRequest = _presenter.CurrentStationaryRequest;
                        _presenter.CurrentStationaryRequest.StationaryRequestDetails.Add(Detail);
                        Master.ShowMessage(new AppMessage("Stationary Request Detail added successfully.", RMessageType.Info));
                        dgStationaryRequestDetail.EditItemIndex = -1;
                        BindStationaryRequestDetails();
                    }
                    else
                    {
                        Master.ShowMessage(new AppMessage("You can only request ONE Item per request!", RMessageType.Error));
                    }

                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to Add Stationary Request Detail. " + ex.Message, RMessageType.Error));
                    ExceptionUtility.LogException(ex, ex.Source);
                    ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
                }
            }
        }
        protected void dgStationaryRequestDetail_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                DropDownList ddlFItemAccount = e.Item.FindControl("ddlFAccount") as DropDownList;
                BindAccount(ddlFItemAccount);
            }
            else
            {
                if (_presenter.CurrentStationaryRequest.StationaryRequestDetails != null)
                {
                    DropDownList ddlItemAccount = e.Item.FindControl("ddlAccount") as DropDownList;
                    if (ddlItemAccount != null)
                    {
                        BindAccount(ddlItemAccount);
                        if (_presenter.CurrentStationaryRequest.StationaryRequestDetails[e.Item.DataSetIndex].ItemAccount != null)
                        {
                            ListItem liI = ddlItemAccount.Items.FindByValue(_presenter.CurrentStationaryRequest.StationaryRequestDetails[e.Item.DataSetIndex].ItemAccount.Id.ToString());
                            if (liI != null)
                                liI.Selected = true;
                        }

                    }
                }
            }
        }
        protected void dgStationaryRequestDetail_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgStationaryRequestDetail.DataKeys[e.Item.ItemIndex];
            StationaryRequestDetail Detail;
            if (id > 0)
                Detail = _presenter.CurrentStationaryRequest.GetStationaryRequestDetail(id);
            else
                Detail = _presenter.CurrentStationaryRequest.StationaryRequestDetails[e.Item.ItemIndex];

            try
            {
                DropDownList ddlAccount = e.Item.FindControl("ddlAccount") as DropDownList;
                Detail.ItemAccount = _presenter.GetItemAccount(int.Parse(ddlAccount.SelectedValue));
                TextBox txtAccountCode = e.Item.FindControl("txtAccountCode") as TextBox;
                Detail.AccountCode = txtAccountCode.Text;
                TextBox txtItem = e.Item.FindControl("txtItem") as TextBox;
                Detail.Item = txtItem.Text;
                TextBox txtQty = e.Item.FindControl("txtQty") as TextBox;
                Detail.Qty = Convert.ToInt32(txtQty.Text);
                Detail.StationaryRequest = _presenter.CurrentStationaryRequest;
                Master.ShowMessage(new AppMessage("Stationary Request Detail Updated successfully.", RMessageType.Info));
                dgStationaryRequestDetail.EditItemIndex = -1;
                BindStationaryRequestDetails();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to Update Stationary Request Detail. " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }




        }
        #endregion
        protected void btnRequest_Click(object sender, EventArgs e)
        {
            SaveStationaryRequest();
            if (_presenter.CurrentStationaryRequest.StationaryRequestDetails.Any())
            {
                if (_presenter.CurrentStationaryRequest.StationaryRequestStatuses.Any())
                {
                    _presenter.SaveOrUpdateStationaryRequest(_presenter.CurrentStationaryRequest);
                    BindSearchStationaryRequestGrid();
                    Master.ShowMessage(new AppMessage("Successfully did a Stationary Request, Reference No - <b>'" + _presenter.CurrentStationaryRequest.RequestNo + "'</b> ", RMessageType.Info));
                    btnRequest.Visible = false;
                }
                else
                {
                    Master.ShowMessage(new AppMessage("There was an error while constructing the Approval Process", RMessageType.Error));
                }
            }
            else
            {
                Master.ShowMessage(new AppMessage("You have to insert at least one Stationary item detail", RMessageType.Error));
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentStationaryRequest.CurrentStatus == null)
                {
                    _presenter.DeleteStationaryRequest(_presenter.CurrentStationaryRequest);
                    Master.ShowMessage(new AppMessage("Stationary Request Deleted ", RMessageType.Info));
                    BindSearchStationaryRequestGrid();
                }
                else
                    Master.ShowMessage(new AppMessage("Warning: Unable to Delete Stationary Request ", RMessageType.Error));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Warning: Unable to Delete Stationary Request " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void ddlFAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            TextBox txtAccountCode = ddl.FindControl("txtFAccountCode") as TextBox;
            txtAccountCode.Text = _presenter.GetItemAccount(Convert.ToInt32(ddl.SelectedValue)).AccountCode;
        }

    }
}