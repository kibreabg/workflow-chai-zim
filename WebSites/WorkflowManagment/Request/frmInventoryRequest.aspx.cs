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
    public partial class frmInventoryRequest : POCBasePage, IInventoryRequestView
    {
        private InventoryRequestPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private InventoryRequest _Inventoryrequest;
        private int _leaverequestId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                CheckApprovalSettings();
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                BindSearchInventoryRequestGrid();
                BindInventoryRequestDetails();
                BindInitialValues();
            }
            this._presenter.OnViewLoaded();
        }
        [CreateNew]
        public InventoryRequestPresenter Presenter
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
            if (_presenter.GetApprovalSetting(RequestType.Inventory_Request.ToString().Replace('_', ' '), 0) == null)
            {
                pnlWarning.Visible = true;
            }
        }
        private void BindInitialValues()
        {
            AppUser CurrentUser = _presenter.CurrentUser();
            txtRequester.Text = CurrentUser.FirstName + " " + CurrentUser.LastName;

            if (_presenter.CurrentInventoryRequest.Id <= 0)
            {
                AutoNumber();
                txtRequestDate.Text = DateTime.Today.Date.ToShortDateString();

            }
        }
        private string AutoNumber()
        {
            return "IR-" + _presenter.CurrentUser().Id.ToString() + "-" + (_presenter.GetLastInventoryRequestId() + 1).ToString();
        }
        private void BindInventoryRequest()
        {

            if (_presenter.CurrentInventoryRequest.Id > 0)
            {
                txtRequestDate.Text = _presenter.CurrentInventoryRequest.RequestedDate.ToShortDateString();
                txtDeliveryDate.Text = _presenter.CurrentInventoryRequest.RequiredDateOfDelivery.ToShortDateString();
                txtDeliverto.Text = _presenter.CurrentInventoryRequest.DeliverTo;
                txtSpecialNeed.Text = _presenter.CurrentInventoryRequest.SpecialNeed;
                txtPurposeOfRequest.Text = _presenter.CurrentInventoryRequest.PurposeOfRequest;
            }
        }
        private void SaveInventoryRequest()
        {
            AppUser CurrentUser = _presenter.CurrentUser();
            try
            {
                _presenter.CurrentInventoryRequest.Requester = CurrentUser.Id;
                _presenter.CurrentInventoryRequest.RequestedDate = Convert.ToDateTime(txtRequestDate.Text);
                _presenter.CurrentInventoryRequest.RequiredDateOfDelivery = Convert.ToDateTime(txtDeliveryDate.Text);
                _presenter.CurrentInventoryRequest.RequestNo = AutoNumber();
                _presenter.CurrentInventoryRequest.DeliverTo = txtDeliverto.Text;
                _presenter.CurrentInventoryRequest.SpecialNeed = txtSpecialNeed.Text;
                _presenter.CurrentInventoryRequest.PurposeOfRequest = txtPurposeOfRequest.Text;
                SaveInventoryRequestStatus();
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
        private void SaveInventoryRequestStatus()
        {
            if (_presenter.CurrentInventoryRequest.Id <= 0)
            {
                if (_presenter.GetApprovalSetting(RequestType.Inventory_Request.ToString().Replace('_', ' '), 0) != null)
                {
                    int i = 1;
                    foreach (ApprovalLevel AL in _presenter.GetApprovalSetting(RequestType.Inventory_Request.ToString().Replace('_', ' '), 0).ApprovalLevels)
                    {
                        InventoryRequestStatus PRS = new InventoryRequestStatus();
                        PRS.InventoryRequest = _presenter.CurrentInventoryRequest;
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
                        _presenter.CurrentInventoryRequest.InventoryRequestStatuses.Add(PRS);

                    }
                }
                else { pnlWarning.Visible = true; }
            }
        }
        private void GetCurrentApprover()
        {
            foreach (InventoryRequestStatus PRS in _presenter.CurrentInventoryRequest.InventoryRequestStatuses)
            {
                if (PRS.ApprovalStatus == null)
                {
                    SendEmail(PRS);
                    _presenter.CurrentInventoryRequest.CurrentApprover = PRS.Approver;
                    _presenter.CurrentInventoryRequest.CurrentLevel = PRS.WorkflowLevel;
                    _presenter.CurrentInventoryRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    break;

                }
            }
        }
        private void SendEmail(InventoryRequestStatus PRS)
        {


            if (_presenter.GetSuperviser(PRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetSuperviser(PRS.Approver).Email, "Inventory Request", _presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName + "' Request for Item procurment No. '" + _presenter.CurrentInventoryRequest.RequestNo + "'");
            }
            else
            {

                EmailSender.Send(_presenter.GetSuperviser(_presenter.GetAssignedJobbycurrentuser(PRS.Approver).AssignedTo).Email, "Inventory Request", _presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName + "' Request for Item procurment No. '" + _presenter.CurrentInventoryRequest.RequestNo + "'");
            }


        }
        public InventoryRequest InventoryRequest
        {
            get
            {
                return _Inventoryrequest;
            }
            set
            {
                _Inventoryrequest = value;
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
        public int InventoryRequestId
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
        private void BindInventoryItem(DropDownList ddlItemName)
        {
            ddlItemName.DataSource = _presenter.GetInventories();
            ddlItemName.DataBind();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmInventoryRequest.aspx");
        }
        protected void grvInventoryRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _leaverequestId = Convert.ToInt32(grvInventoryRequestList.SelectedDataKey[0]);
            _presenter.OnViewLoaded();
            BindInventoryRequest();
            BindInventoryRequestDetails();
        }
        protected void grvInventoryRequestList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            _presenter.DeleteInventoryRequest(_presenter.GetInventoryRequestById(Convert.ToInt32(grvInventoryRequestList.DataKeys[e.RowIndex].Value)));

            btnFind_Click(sender, e);
            Master.ShowMessage(new AppMessage("Inventory Request Successfully Deleted", RMessageType.Info));

        }
        protected void grvInventoryRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Add Implementation
            }
        }
        protected void grvInventoryRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvInventoryRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindSearchInventoryRequestGrid();
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
        }
        private void BindSearchInventoryRequestGrid()
        {
            grvInventoryRequestList.DataSource = _presenter.ListInventoryRequests(txtRequestNosearch.Text, txtRequestDatesearch.Text);
            grvInventoryRequestList.DataBind();
        }
        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            _presenter.CancelPage();
        }

        #region InventoryRequestDetail
        private void BindInventoryRequestDetails()
        {
            dgInventoryRequestDetail.DataSource = _presenter.CurrentInventoryRequest.InventoryRequestDetails;
            dgInventoryRequestDetail.DataBind();
        }
        protected void dgInventoryRequestDetail_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgInventoryRequestDetail.EditItemIndex = -1;
            BindInventoryRequestDetails();
        }
        protected void dgInventoryRequestDetail_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgInventoryRequestDetail.DataKeys[e.Item.ItemIndex];
            int PRDId = (int)dgInventoryRequestDetail.DataKeys[e.Item.ItemIndex];
            InventoryRequestDetail prd;

            if (PRDId > 0)
                prd = _presenter.CurrentInventoryRequest.GetInventoryRequestDetail(PRDId);
            else
                prd = _presenter.CurrentInventoryRequest.InventoryRequestDetails[e.Item.ItemIndex];
            try
            {
                if (PRDId > 0)
                {
                    _presenter.CurrentInventoryRequest.RemoveInventoryRequestDetail(id);
                    _presenter.DeleteInventoryRequestDetail(_presenter.GetInventoryRequestDetail(id));
                    _presenter.SaveOrUpdateInventoryRequest(_presenter.CurrentInventoryRequest);
                }
                else
                {
                    _presenter.CurrentInventoryRequest.InventoryRequestDetails.Remove(prd);
                }
                BindInventoryRequestDetails();

                Master.ShowMessage(new AppMessage("Inventory Request Detail was Removed Successfully", RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Inventory Request Detail. " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void dgInventoryRequestDetail_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgInventoryRequestDetail.EditItemIndex = e.Item.ItemIndex;

            BindInventoryRequestDetails();
        }
        protected void dgInventoryRequestDetail_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "AddNew")
            {
                try
                {
                    if (!_presenter.CurrentInventoryRequest.InventoryRequestDetails.Any())
                    {
                        InventoryRequestDetail Detail = new InventoryRequestDetail();
                        DropDownList ddlFItemName = e.Item.FindControl("ddlFItemName") as DropDownList;
                        Detail.Inventory = _presenter.GetInventory(int.Parse(ddlFItemName.SelectedValue));
                        TextBox txtFCategory = e.Item.FindControl("txtFCategory") as TextBox;
                        Detail.Category = txtFCategory.Text;
                        TextBox txtFUnit = e.Item.FindControl("txtFUnit") as TextBox;
                        Detail.Unit = txtFUnit.Text;
                        TextBox txtFQty = e.Item.FindControl("txtFQty") as TextBox;
                        Detail.Qty = Convert.ToInt32(txtFQty.Text);
                        Detail.InventoryRequest = _presenter.CurrentInventoryRequest;
                        _presenter.CurrentInventoryRequest.InventoryRequestDetails.Add(Detail);
                        Master.ShowMessage(new AppMessage("Inventory Request Detail added successfully.", RMessageType.Info));
                        dgInventoryRequestDetail.EditItemIndex = -1;
                        BindInventoryRequestDetails();
                    }
                    else
                    {
                        Master.ShowMessage(new AppMessage("You can only request ONE Item per request!", RMessageType.Error));
                    }

                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to Add Inventory Request Detail. " + ex.Message, RMessageType.Error));
                    ExceptionUtility.LogException(ex, ex.Source);
                    ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
                }
            }
        }
        protected void dgInventoryRequestDetail_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                DropDownList ddlFItemName = e.Item.FindControl("ddlFItemName") as DropDownList;
                BindInventoryItem(ddlFItemName);
            }
            else
            {
                if (_presenter.CurrentInventoryRequest.InventoryRequestDetails != null)
                {
                    DropDownList ddlItemName = e.Item.FindControl("ddlItemName") as DropDownList;
                    if (ddlItemName != null)
                    {
                        BindInventoryItem(ddlItemName);
                        if (_presenter.CurrentInventoryRequest.InventoryRequestDetails[e.Item.DataSetIndex].Inventory != null)
                        {
                            ListItem liI = ddlItemName.Items.FindByValue(_presenter.CurrentInventoryRequest.InventoryRequestDetails[e.Item.DataSetIndex].Inventory.Id.ToString());
                            if (liI != null)
                                liI.Selected = true;
                        }

                    }
                }
            }
        }
        protected void dgInventoryRequestDetail_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgInventoryRequestDetail.DataKeys[e.Item.ItemIndex];
            InventoryRequestDetail Detail;
            if (id > 0)
                Detail = _presenter.CurrentInventoryRequest.GetInventoryRequestDetail(id);
            else
                Detail = _presenter.CurrentInventoryRequest.InventoryRequestDetails[e.Item.ItemIndex];

            try
            {
                DropDownList ddlItemName = e.Item.FindControl("ddlItemName") as DropDownList;
                Detail.Inventory = _presenter.GetInventory(int.Parse(ddlItemName.SelectedValue));
                TextBox txtCategory = e.Item.FindControl("txtCategory") as TextBox;
                Detail.Category = txtCategory.Text;
                TextBox txtUnit = e.Item.FindControl("txtUnit") as TextBox;
                Detail.Unit = txtUnit.Text;
                TextBox txtQty = e.Item.FindControl("txtQty") as TextBox;
                Detail.Qty = Convert.ToInt32(txtQty.Text);
                Detail.InventoryRequest = _presenter.CurrentInventoryRequest;
                Master.ShowMessage(new AppMessage("Inventory Request Detail Updated successfully.", RMessageType.Info));
                dgInventoryRequestDetail.EditItemIndex = -1;
                BindInventoryRequestDetails();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to Update Inventory Request Detail. " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        #endregion
        protected void btnRequest_Click(object sender, EventArgs e)
        {
            SaveInventoryRequest();
            if (_presenter.CurrentInventoryRequest.InventoryRequestDetails.Any())
            {
                if (_presenter.CurrentInventoryRequest.InventoryRequestStatuses.Any())
                {
                    _presenter.SaveOrUpdateInventoryRequest(_presenter.CurrentInventoryRequest);
                    BindSearchInventoryRequestGrid();
                    Master.ShowMessage(new AppMessage("Successfully did a Inventory Request, Reference No - <b>'" + _presenter.CurrentInventoryRequest.RequestNo + "'</b> ", RMessageType.Info));
                    btnRequest.Visible = false;
                }
                else
                {
                    Master.ShowMessage(new AppMessage("There was an error while constructing the Approval Process", RMessageType.Error));
                }
            }
            else
            {
                Master.ShowMessage(new AppMessage("You have to insert at least one Inventory item detail", RMessageType.Error));
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentInventoryRequest.CurrentStatus == null)
                {
                    _presenter.DeleteInventoryRequest(_presenter.CurrentInventoryRequest);
                    Master.ShowMessage(new AppMessage("Inventory Request Deleted ", RMessageType.Info));
                    BindSearchInventoryRequestGrid();
                }
                else
                    Master.ShowMessage(new AppMessage("Warning: Unable to Delete Inventory Request ", RMessageType.Error));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Warning: Unable to Delete Inventory Request " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void ddlFItemName_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            TextBox txtFCategory = ddl.FindControl("txtFCategory") as TextBox;
            txtFCategory.Text = _presenter.GetInventory(Convert.ToInt32(ddl.SelectedValue)).Category;
            TextBox txtFUnit = ddl.FindControl("txtFUnit") as TextBox;
            txtFUnit.Text = _presenter.GetInventory(Convert.ToInt32(ddl.SelectedValue)).Unit;
        }

    }
}