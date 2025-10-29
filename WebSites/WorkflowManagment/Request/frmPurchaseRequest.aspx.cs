using AjaxControlToolkit;
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
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public partial class frmPurchaseRequest : POCBasePage, IPurchaseRequestView
    {
        private PurchaseRequestPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private PurchaseRequest _purchaserequest;
        private int _leaverequestId = 0;
        private int _totalprice = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //CheckApprovalSettings();
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                BindSearchPurchaseRequestGrid();
                BindPurchaseRequestDetails();
                BindInitialValues();
            }
            this._presenter.OnViewLoaded();
        }
        [CreateNew]
        public PurchaseRequestPresenter Presenter
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
            if (_presenter.GetApprovalSetting(RequestType.Purchase_Request.ToString().Replace('_', ' '), 0) == null)
            {
                pnlWarning.Visible = true;
            }
        }
        private void BindInitialValues()
        {
            AppUser CurrentUser = _presenter.CurrentUser();
            txtRequester.Text = CurrentUser.FirstName + " " + CurrentUser.LastName;

            if (_presenter.CurrentPurchaseRequest.Id <= 0)
            {
                AutoNumber();
                txtRequestDate.Text = DateTime.Today.Date.ToShortDateString();

            }
        }
        private string AutoNumber()
        {
            return "PR-" + _presenter.CurrentUser().Id.ToString() + "-" + (_presenter.GetLastPurchaseRequestId() + 1).ToString();
        }
        private void BindPurchaseRequest()
        {

            if (_presenter.CurrentPurchaseRequest.Id > 0)
            {
                // txtRequestNo.Text = _presenter.CurrentPurchaseRequest.RequestNo;
                txtRequestDate.Text = _presenter.CurrentPurchaseRequest.RequestedDate.ToShortDateString();
                txtComment.Text = _presenter.CurrentPurchaseRequest.Comment.ToString();
                ddlPayMethods.Text = _presenter.CurrentPurchaseRequest.PaymentMethod;
                txtDeliverto.Text = _presenter.CurrentPurchaseRequest.DeliverTo.ToString();
                txtdeliveryDate.Text = _presenter.CurrentPurchaseRequest.Requireddateofdelivery.ToShortDateString();
                txtSuggestedSupplier.Text = _presenter.CurrentPurchaseRequest.SuggestedSupplier.ToString();
                txtSpecialNeed.Text = _presenter.CurrentPurchaseRequest.SpecialNeed.ToString();
                chkBudgeted.Checked = _presenter.CurrentPurchaseRequest.Budgeted;
                txtTotal.Text = _presenter.CurrentPurchaseRequest.TotalPrice.ToString();

            }
        }
        private void SavePurchaseRequest()
        {
            AppUser CurrentUser = _presenter.CurrentUser();
            try
            {
                _presenter.CurrentPurchaseRequest.Requester = CurrentUser.Id;
                _presenter.CurrentPurchaseRequest.RequestedDate = Convert.ToDateTime(txtRequestDate.Text);
                _presenter.CurrentPurchaseRequest.RequestNo = AutoNumber();
                _presenter.CurrentPurchaseRequest.DeliverTo = txtDeliverto.Text;
                _presenter.CurrentPurchaseRequest.Comment = txtComment.Text;
                _presenter.CurrentPurchaseRequest.PaymentMethod = ddlPayMethods.Text;
                _presenter.CurrentPurchaseRequest.SuggestedSupplier = txtSuggestedSupplier.Text;
                _presenter.CurrentPurchaseRequest.SpecialNeed = txtSpecialNeed.Text;
                _presenter.CurrentPurchaseRequest.Requireddateofdelivery = Convert.ToDateTime(txtdeliveryDate.Text);
                _presenter.CurrentPurchaseRequest.Budgeted = chkBudgeted.Checked;
                //Determine total cost
                decimal cost = 0;
                if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Count > 0)
                {

                    foreach (PurchaseRequestDetail detail in _presenter.CurrentPurchaseRequest.PurchaseRequestDetails)
                    {
                        cost = cost + detail.EstimatedCost;
                    }
                }
                _presenter.CurrentPurchaseRequest.TotalPrice = cost;
                //Determine total cost end
                SavePurchaseRequestStatus();
                GetCurrentApprover();
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Violation of UNIQUE KEY"))
                    {
                        Master.ShowMessage(new AppMessage("Please Click Request button Again,There is a duplicate Number", RMessageType.Error));
                        //AutoNumber();
                    }
                }

            }

        }
        private void SavePurchaseRequestStatus()
        {
            if (_presenter.CurrentPurchaseRequest.Id <= 0)
            {
                if (_presenter.GetApprovalSettingforPurchaseProcess(RequestType.Purchase_Request.ToString().Replace('_', ' '), 0) != null)
                {
                    int i = 1;
                    foreach (ApprovalLevel AL in _presenter.GetApprovalSettingforPurchaseProcess(RequestType.Purchase_Request.ToString().Replace('_', ' '), 0).ApprovalLevels)
                    {
                        PurchaseRequestStatus PRS = new PurchaseRequestStatus();
                        PRS.PurchaseRequest = _presenter.CurrentPurchaseRequest;
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
                        else if (AL.EmployeePosition.PositionName == "Program Manager")
                        {
                            if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[0].Project.Id != 0)
                            {
                                PRS.Approver = _presenter.GetProject(_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[0].Project.Id).AppUser.Id;
                            }

                        }
                        else
                        {
                            PRS.Approver = _presenter.Approver(AL.EmployeePosition.Id).Id;
                        }
                        PRS.WorkflowLevel = i;
                        i++;
                        _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses.Add(PRS);

                    }
                }
                else { pnlWarning.Visible = true; }
            }
        }
        private void GetCurrentApprover()
        {
            foreach (PurchaseRequestStatus PRS in _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses)
            {
                if (PRS.ApprovalStatus == null)
                {
                    SendEmail(PRS);
                    _presenter.CurrentPurchaseRequest.CurrentApprover = PRS.Approver;
                    _presenter.CurrentPurchaseRequest.CurrentLevel = PRS.WorkflowLevel;
                    _presenter.CurrentPurchaseRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    break;

                }
            }
        }
        private void SendEmail(PurchaseRequestStatus PRS)
        {


            if (_presenter.GetSuperviser(PRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetSuperviser(PRS.Approver).Email, "Purchase Request", _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName + "' Request for Item procurment No. '" + _presenter.CurrentPurchaseRequest.RequestNo + "'");
            }
            else
            {

                EmailSender.Send(_presenter.GetSuperviser(_presenter.GetAssignedJobbycurrentuser(PRS.Approver).AssignedTo).Email, "Purchase Request", _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName + "' Request for Item procurment No. '" + _presenter.CurrentPurchaseRequest.RequestNo + "'");
            }


        }
        public PurchaseRequest PurchaseRequest
        {
            get
            {
                return _purchaserequest;
            }
            set
            {
                _purchaserequest = value;
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
        public string GetPaymentMethod
        {
            get { return ddlPayMethods.Text; }
        }
        public int PurchaseRequestId
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
        private void BindProject(ComboBox cbProject)
        {
            cbProject.DataSource = _presenter.GetProjects();
            cbProject.DataValueField = "Id";
            cbProject.DataTextField = "ProjectCode";
            cbProject.DataBind();

            cbProject.Items.Insert(0, new ListItem("---Select Project---", "0"));
            cbProject.SelectedIndex = 0;

        }
        private void BindGrant(ComboBox cbGrant, int projectId)
        {
            cbGrant.DataSource = _presenter.GetGrantbyprojectId(projectId);
            cbGrant.DataValueField = "Id";
            cbGrant.DataTextField = "GrantCode";
            cbGrant.DataBind();

            cbGrant.Items.Insert(0, new ListItem("---Select Grant---", "0"));
            cbGrant.SelectedIndex = 0;

        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmPurchaseRequest.aspx");
        }
        protected void grvPurchaseRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _leaverequestId = Convert.ToInt32(grvPurchaseRequestList.SelectedDataKey[0]);
            _presenter.OnViewLoaded();
            BindPurchaseRequest();
            BindPurchaseRequestDetails();
        }
        protected void grvPurchaseRequestList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            _presenter.DeletePurchaseRequest(_presenter.GetPurchaseRequestById(Convert.ToInt32(grvPurchaseRequestList.DataKeys[e.RowIndex].Value)));

            btnFind_Click(sender, e);
            Master.ShowMessage(new AppMessage("Purchase Request Successfully Deleted", RMessageType.Info));

        }
        protected void grvPurchaseRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }
        protected void grvPurchaseRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvPurchaseRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindSearchPurchaseRequestGrid();
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
        }
        private void BindSearchPurchaseRequestGrid()
        {
            grvPurchaseRequestList.DataSource = _presenter.ListPurchaseRequests(txtRequestNosearch.Text, txtRequestDatesearch.Text);
            grvPurchaseRequestList.DataBind();
        }
        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            _presenter.CancelPage();
        }

        #region PurchaseRequestDetail
        private void BindPurchaseRequestDetails()
        {
            dgPurchaseRequestDetail.DataSource = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails;
            dgPurchaseRequestDetail.DataBind();
        }
        protected void dgPurchaseRequestDetail_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgPurchaseRequestDetail.EditItemIndex = -1;
            BindPurchaseRequestDetails();
        }
        protected void dgPurchaseRequestDetail_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgPurchaseRequestDetail.DataKeys[e.Item.ItemIndex];
            int PRDId = (int)dgPurchaseRequestDetail.DataKeys[e.Item.ItemIndex];
            PurchaseRequestDetail prd;

            if (PRDId > 0)
                prd = _presenter.CurrentPurchaseRequest.GetPurchaseRequestDetail(PRDId);
            else
                prd = (PurchaseRequestDetail)_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.ItemIndex];
            try
            {
                if (PRDId > 0)
                {
                    _presenter.CurrentPurchaseRequest.RemovePurchaseRequestDetail(id);
                    _presenter.DeletePurchaseRequestDetail(_presenter.GetPurchaseRequestDetail(id));
                    _presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice - prd.EstimatedCost;
                    txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
                    _presenter.SaveOrUpdatePurchaseRequest(_presenter.CurrentPurchaseRequest);
                }
                else
                {
                    _presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Remove(prd);
                    _presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice - prd.EstimatedCost;
                    txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
                }
                BindPurchaseRequestDetails();

                Master.ShowMessage(new AppMessage("Purchase Request Detail was Removed Successfully", RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Purchase Request Detail. " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }


        }
        protected void dgPurchaseRequestDetail_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgPurchaseRequestDetail.EditItemIndex = e.Item.ItemIndex;

            BindPurchaseRequestDetails();
        }
        protected void dgPurchaseRequestDetail_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "AddNew")
            {
                try
                {
                    if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Count() < 1)
                    {
                        PurchaseRequestDetail Detail = new PurchaseRequestDetail();
                        DropDownList ddlFAccount = e.Item.FindControl("ddlFAccount") as DropDownList;
                        Detail.ItemAccount = _presenter.GetItemAccount(int.Parse(ddlFAccount.SelectedValue));
                        TextBox txtFAccountCode = e.Item.FindControl("txtFAccountCode") as TextBox;
                        Detail.AccountCode = txtFAccountCode.Text;
                        TextBox txtFItem = e.Item.FindControl("txtFItem") as TextBox;
                        Detail.Item = txtFItem.Text;
                        TextBox txtFQty = e.Item.FindControl("txtFQty") as TextBox;
                        Detail.Qty = Convert.ToInt32(txtFQty.Text);

                        TextBox txtFPriceperunit = e.Item.FindControl("txtFPriceperunit") as TextBox;
                        Detail.Priceperunit = Convert.ToDecimal(txtFPriceperunit.Text);
                        Detail.EstimatedCost = Convert.ToInt32(txtFQty.Text) * Convert.ToDecimal(txtFPriceperunit.Text);
                        //Determine total cost
                        decimal cost = 0;
                        if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Count > 0)
                        {

                            foreach (PurchaseRequestDetail detail in _presenter.CurrentPurchaseRequest.PurchaseRequestDetails)
                            {
                                cost = cost + detail.EstimatedCost;
                            }
                        }
                        _presenter.CurrentPurchaseRequest.TotalPrice = cost;
                        //Determine total cost end
                        _presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice + Detail.EstimatedCost;
                        txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
                        ComboBox CbProject = e.Item.FindControl("CbProject") as ComboBox;
                        Detail.Project = _presenter.GetProject(int.Parse(CbProject.SelectedValue));
                        ComboBox CbGrant = e.Item.FindControl("CbGrant") as ComboBox;
                        Detail.Grant = _presenter.GetGrant(int.Parse(CbGrant.SelectedValue));
                        Detail.PurchaseRequest = _presenter.CurrentPurchaseRequest;
                        _presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Add(Detail);
                        Master.ShowMessage(new AppMessage("Purchase Request Detail added successfully.", RMessageType.Info));
                        dgPurchaseRequestDetail.EditItemIndex = -1;
                        BindPurchaseRequestDetails();
                    }
                    else
                    {
                        Master.ShowMessage(new AppMessage("You can only request ONE Item per request!", RMessageType.Error));
                    }

                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to Add Purchase Request Detail. " + ex.Message, RMessageType.Error));
                    ExceptionUtility.LogException(ex, ex.Source);
                    ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
                }
            }
        }
        protected void dgPurchaseRequestDetail_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                DropDownList ddlFItemAccount = e.Item.FindControl("ddlFAccount") as DropDownList;
                BindAccount(ddlFItemAccount);
                ComboBox CbProject = e.Item.FindControl("CbProject") as ComboBox;
                BindProject(CbProject);
                ComboBox CbGrant = e.Item.FindControl("CbGrant") as ComboBox;
                BindGrant(CbGrant, Convert.ToInt32(CbProject.SelectedValue));
            }
            else
            {
                if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails != null)
                {
                    DropDownList ddlItemAccount = e.Item.FindControl("ddlAccount") as DropDownList;
                    if (ddlItemAccount != null)
                    {
                        BindAccount(ddlItemAccount);
                        if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].ItemAccount.Id != null)
                        {
                            ListItem liI = ddlItemAccount.Items.FindByValue(_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].ItemAccount.Id.ToString());
                            if (liI != null)
                                liI.Selected = true;
                        }
                    }

                    ComboBox cbEdtProject = e.Item.FindControl("CbEdtProject") as ComboBox;
                    if (cbEdtProject != null)
                    {
                        BindProject(cbEdtProject);
                        int projectId = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].Project.Id;
                        if (projectId != 0)
                        {
                            cbEdtProject.SelectedValue = projectId.ToString();
                        }
                    }
                    ComboBox cbEdtGrant = e.Item.FindControl("cbEdtGrant") as ComboBox;
                    if (cbEdtGrant != null)
                    {
                        BindGrant(cbEdtGrant, Convert.ToInt32(cbEdtProject.SelectedValue));
                        int grantId = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].Grant.Id;
                        if (grantId != 0)
                        {
                            cbEdtGrant.SelectedValue = grantId.ToString();
                        }
                    }
                }
            }
        }
        protected void dgPurchaseRequestDetail_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgPurchaseRequestDetail.DataKeys[e.Item.ItemIndex];
            PurchaseRequestDetail Detail;
            if (id > 0)
                Detail = _presenter.CurrentPurchaseRequest.GetPurchaseRequestDetail(id);
            else
                Detail = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.ItemIndex];

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

                TextBox txtPriceperunit = e.Item.FindControl("txtPriceperunit") as TextBox;
                Detail.Priceperunit = Convert.ToDecimal(txtPriceperunit.Text);

                //TextBox txtEstimatedCost = e.Item.FindControl("txtEstimatedCost") as TextBox;
                Detail.EstimatedCost = Convert.ToInt32(txtQty.Text) * Convert.ToDecimal(txtPriceperunit.Text);
                //Determine total cost
                decimal cost = 0;
                if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Count > 0)
                {
                    foreach (PurchaseRequestDetail detail in _presenter.CurrentPurchaseRequest.PurchaseRequestDetails)
                    {
                        cost = cost + detail.EstimatedCost;
                    }
                }
                _presenter.CurrentPurchaseRequest.TotalPrice = cost;
                //Determine total cost end
                //_presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice + Detail.EstimatedCost;
                txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
                ComboBox CbEdtProject = e.Item.FindControl("CbEdtProject") as ComboBox;
                Detail.Project = _presenter.GetProject(int.Parse(CbEdtProject.SelectedValue));
                ComboBox CbEdtGrant = e.Item.FindControl("CbEdtGrant") as ComboBox;
                Detail.Grant = _presenter.GetGrant(int.Parse(CbEdtGrant.SelectedValue));
                Detail.PurchaseRequest = _presenter.CurrentPurchaseRequest;
                Master.ShowMessage(new AppMessage("Purchase Request Detail  Updated successfully.", RMessageType.Info));
                dgPurchaseRequestDetail.EditItemIndex = -1;
                BindPurchaseRequestDetails();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to Update Purchase Request Detail. " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        #endregion
        protected void CbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbProject = sender as ComboBox;
            if (cbProject == null) return;

            // DataGridItem that contains this ComboBox
            var item = cbProject.NamingContainer as DataGridItem;
            if (item == null) return;

            // find the other combobox (grant) in the same row
            var cbGrant = item.FindControl("CbGrant") as ComboBox;
            if (cbGrant == null) return;

            // use selected project value to bind grant combobox
            int projectId;
            if (int.TryParse(cbProject.SelectedValue, out projectId))
            {
                BindGrant(cbGrant, projectId);
            }
        }
        protected void CbEdtProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbEdtProject = sender as ComboBox;
            if (cbEdtProject == null) return;

            // DataGridItem that contains this ComboBox
            var item = cbEdtProject.NamingContainer as DataGridItem;
            if (item == null) return;

            // find the other combobox (grant) in the same row
            var cbEdtGrant = item.FindControl("cbEdtGrant") as ComboBox;
            if (cbEdtGrant == null) return;

            // use selected project value to bind grant combobox
            int projectId;
            if (int.TryParse(cbEdtProject.SelectedValue, out projectId))
            {
                BindGrant(cbEdtGrant, projectId);
            }
        }
        protected void btnRequest_Click(object sender, EventArgs e)
        {
            SavePurchaseRequest();
            if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Count != 0)
            {
                if (_presenter.CurrentPurchaseRequest.PurchaseRequestStatuses.Count != 0)
                {
                    _presenter.SaveOrUpdatePurchaseRequest(_presenter.CurrentPurchaseRequest);
                    //ClearForm();
                    BindSearchPurchaseRequestGrid();
                    Master.ShowMessage(new AppMessage("Successfully did a Purchase Request, Reference No - <b>'" + _presenter.CurrentPurchaseRequest.RequestNo + "'</b> ", RMessageType.Info));
                    Log.Info(_presenter.CurrentUser().FullName + " has requested for a Purchase of Total Price " + _presenter.CurrentPurchaseRequest.TotalPrice);
                    btnRequest.Visible = false;
                }
                else
                {
                    Master.ShowMessage(new AppMessage("There was an error while constructing the Approval Process", RMessageType.Error));
                }
            }
            else
            {
                Master.ShowMessage(new AppMessage("You have to insert at least one purchase item detail", RMessageType.Error));
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentPurchaseRequest.CurrentStatus == null)
                {
                    _presenter.DeletePurchaseRequest(_presenter.CurrentPurchaseRequest);
                    Master.ShowMessage(new AppMessage("Purchase Request Deleted ", RMessageType.Info));
                    BindSearchPurchaseRequestGrid();
                }
                else
                    Master.ShowMessage(new AppMessage("Warning: Unable to Delete Purchase Request ", RMessageType.Error));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Warning: Unable to Delete Purchase Request " + ex.Message, RMessageType.Error));
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
        #region Attachments
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            UploadFile();
        }
        protected void DownloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }
        protected void DeleteFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            _presenter.CurrentPurchaseRequest.RemovePRAttachment(filePath);
            File.Delete(Server.MapPath(filePath));
            grvAttachments.DataSource = _presenter.CurrentPurchaseRequest.PRAttachments;
            grvAttachments.DataBind();
            //Response.Redirect(Request.Url.AbsoluteUri);


        }
        private void UploadFile()
        {
            string fileName = Path.GetFileName(fuReciept.PostedFile.FileName);

            if (fileName != String.Empty)
            {
                PRAttachment attachment = new PRAttachment();
                attachment.FilePath = "~/PRUploads/" + fileName;
                fuReciept.PostedFile.SaveAs(Server.MapPath("~/PRUploads/") + fileName);
                //Response.Redirect(Request.Url.AbsoluteUri);
                _presenter.CurrentPurchaseRequest.PRAttachments.Add(attachment);

                grvAttachments.DataSource = _presenter.CurrentPurchaseRequest.PRAttachments;
                grvAttachments.DataBind();

                Master.ShowMessage(new AppMessage("Successfully Uploaded the Attachment!", RMessageType.Info));

            }
            else
            {
                Master.ShowMessage(new AppMessage("Please select file ", RMessageType.Error));
            }
        }
        #endregion
    }
}