using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using System.Reflection;
using log4net.Config;

using LumenWorks.Framework.IO.Csv;
using System.Globalization;
using ClosedXML.Excel;
using System.Data;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public partial class frmFuelCardRequest : POCBasePage, IFuelCardRequestView
    {
        private FuelCardRequestPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private FuelCardRequest _fuelcardrequest;
        private int _leaverequestId = 0;
        private int _totalprice = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //CheckApprovalSettings();
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
               // BindSearchPurchaseRequestGrid();
               // BindPurchaseRequestDetails();
                BindInitialValues();
            }
            this._presenter.OnViewLoaded();



        }

        [CreateNew]
        public FuelCardRequestPresenter Presenter
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

            if (_presenter.CurrentFuelCardRequest.Id <= 0)
            {
                AutoNumber();
                txtRequestDate.Text = DateTime.Today.Date.ToShortDateString();

            }
        }
        private string AutoNumber()
        {
            //    return "PR-" + _presenter.CurrentUser().Id.ToString() + "-" + (_presenter.GetLastFuelCardRequestId() + 1).ToString();
            return null;
        }
        private void BindFuelCardRequest()
        {

            //if (_presenter.CurrentPurchaseRequest.Id > 0)
            //{
            //    // txtRequestNo.Text = _presenter.CurrentPurchaseRequest.RequestNo;
            //    txtRequestDate.Text = _presenter.CurrentPurchaseRequest.RequestedDate.ToShortDateString();
            //    txtComment.Text = _presenter.CurrentPurchaseRequest.Comment.ToString();
                
            //    ddlPayMethods.Text = _presenter.CurrentPurchaseRequest.PaymentMethod;
            //    txtDeliverto.Text = _presenter.CurrentPurchaseRequest.DeliverTo.ToString();
            //    txtdeliveryDate.Text = _presenter.CurrentPurchaseRequest.Requireddateofdelivery.ToShortDateString();
            //    txtSuggestedSupplier.Text = _presenter.CurrentPurchaseRequest.SuggestedSupplier.ToString();
            //    txtSpecialNeed.Text = _presenter.CurrentPurchaseRequest.SpecialNeed.ToString();
            //    chkBudgeted.Checked = _presenter.CurrentPurchaseRequest.Budgeted;
            //    txtTotal.Text = _presenter.CurrentPurchaseRequest.TotalPrice.ToString();

            //}
        }
        private void SaveFuelCardRequest()
        {
            AppUser CurrentUser = _presenter.CurrentUser();
            try
            {
                _presenter.CurrentFuelCardRequest.Requester = CurrentUser.Id;
                _presenter.CurrentFuelCardRequest.RequestedDate = Convert.ToDateTime(txtRequestDate.Text);
                _presenter.CurrentFuelCardRequest.RequestNo = AutoNumber();
              
                //Determine total cost
                decimal cost = 0;
                
                   
                    foreach (FuelCardRequestDetail detail in _presenter.CurrentFuelCardRequest.FuelCardRequestDetails)
                    {
                        cost = Convert.ToDecimal(cost + detail.Amount);
                    }
                
                _presenter.CurrentFuelCardRequest.TotalReimbursement = cost;
                //Determine total cost end
             //   SaveFuelCardRequestStatus();
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
                        Master.ShowMessage(new AppMessage("Please Click Request button Again,There is a duplicate Number", Chai.WorkflowManagment.Enums.RMessageType.Error));
                        //AutoNumber();
                    }
                }

            }

        }
        private void SaveFuelCardRequestStatus()
        {
            if (_presenter.CurrentFuelCardRequest.Id <= 0)
            {
                if (_presenter.GetApprovalSettingforFuelCardProcess(RequestType.FuelCard_Request.ToString().Replace('_', ' '), 0) != null)
                {
                    int i = 1;
                    foreach (ApprovalLevel AL in _presenter.GetApprovalSettingforFuelCardProcess(RequestType.FuelCard_Request.ToString().Replace('_', ' '), 0).ApprovalLevels)
                    {
                        FuelCardRequestStatus FCRS = new FuelCardRequestStatus();
                        FCRS.FuelCardRequest = _presenter.CurrentFuelCardRequest;
                        if (AL.EmployeePosition.PositionName == "Superviser/Line Manager")
                        {
                            if (_presenter.CurrentUser().Superviser.Value != 0)
                            {
                                FCRS.Approver = _presenter.CurrentUser().Superviser.Value;
                            }
                            else
                            {
                                FCRS.ApprovalStatus = ApprovalStatus.Approved.ToString();
                                FCRS.ApprovalDate = DateTime.Today.Date;
                            }
                        }
                        else if (AL.EmployeePosition.PositionName == "Program Manager")
                        {
                            if (_presenter.CurrentFuelCardRequest.FuelCardRequestDetails[0].Project.Id != 0)
                            {
                                FCRS.Approver = _presenter.GetProject(_presenter.CurrentFuelCardRequest.FuelCardRequestDetails[0].Project.Id).AppUser.Id;
                            }

                        }
                        else
                        {
                            FCRS.Approver = _presenter.Approver(AL.EmployeePosition.Id).Id;
                        }
                        FCRS.WorkflowLevel = i;
                        i++;
                        _presenter.CurrentFuelCardRequest.FuelCardRequestStatuses.Add(FCRS);

                    }
                }
                else { pnlWarning.Visible = true; }
            }
        }
        private void GetCurrentApprover()
        {
            foreach (FuelCardRequestStatus FCRS in _presenter.CurrentFuelCardRequest.FuelCardRequestStatuses)
            {
                if (FCRS.ApprovalStatus == null)
                {
                    SendEmail(FCRS);
                    _presenter.CurrentFuelCardRequest.CurrentApprover = FCRS.Approver;
                    _presenter.CurrentFuelCardRequest.CurrentLevel = FCRS.WorkflowLevel;
                    _presenter.CurrentFuelCardRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    break;

                }
            }
        }
        private void SendEmail(FuelCardRequestStatus FCRS)
        {


            if (_presenter.GetSuperviser(FCRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetSuperviser(FCRS.Approver).Email, "Fuel Card Request", _presenter.GetUser(_presenter.CurrentFuelCardRequest.Requester).FullName + "' Request for Fuel Card Request No. '" + _presenter.CurrentFuelCardRequest.RequestNo + "'");
            }
            else
            {

                EmailSender.Send(_presenter.GetSuperviser(_presenter.GetAssignedJobbycurrentuser(FCRS.Approver).AssignedTo).Email, "Fuel Card Request", _presenter.GetUser(_presenter.CurrentFuelCardRequest.Requester).FullName + "' Request for Fuel Card Request No. '" + _presenter.CurrentFuelCardRequest.RequestNo + "'");
            }


        }
        public FuelCardRequest FuelCardRequest
        {
            get
            {
                return _fuelcardrequest;
            }
            set
            {
                _fuelcardrequest = value;
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
       
        public int FuelCarddRequestId
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

       
        public int Month
        {
            get
            {
                return Convert.ToInt32(txtMonth.Text);
            }
        }

        public int Year
        {
            get
            {
               return Convert.ToInt32(txtYear.Text);
            }
        }

      

        private void BindAccount(DropDownList ddlItemAccount)
        {
            ddlItemAccount.DataSource = _presenter.GetItemAccounts();
            ddlItemAccount.DataBind();

        }
        private void BindProject(DropDownList ddlProject)
        {
            ddlProject.DataSource = _presenter.GetProjects();
            ddlProject.DataBind();

        }
        private void BindGrant(DropDownList ddlGrant, int projectId)
        {
            ddlGrant.Items.Clear();
            ListItem lst = new ListItem();
            lst.Text = "Select Grant";
            lst.Value = "";
            ddlGrant.Items.Add(lst);
            ddlGrant.DataSource = _presenter.GetGrantbyprojectId(projectId);
            ddlGrant.DataBind();

        }
        private void ClearForm()
        {
            //txtRequestNo.Text = "";
            txtRequestDate.Text = "";
            txtCardHolder.Text = "";
            txtMonth.Text = "";
            txtYear.Text = "";
            txtTotal.Text = "";


        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmFuelCardRequest.aspx");
        }
        protected void grvPurchaseRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //////// Session["ApprovalLevel"] = true;
            //////// ClearForm();
            ////////BindLeaveRequest();
            //////_leaverequestId = Convert.ToInt32(grvPurchaseRequestList.SelectedDataKey[0]);
            //////_presenter.OnViewLoaded();
            //////BindPurchaseRequest();
            //////BindPurchaseRequestDetails();
        }
        protected void grvPurchaseRequestList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //////_presenter.DeletePurchaseRequest(_presenter.GetPurchaseRequestById(Convert.ToInt32(grvPurchaseRequestList.DataKeys[e.RowIndex].Value)));

            //////btnFind_Click(sender, e);
            //////Master.ShowMessage(new AppMessage("Purchase Request Successfully Deleted", Chai.WorkflowManagment.Enums.RMessageType.Info));

        }
        protected void grvPurchaseRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void grvPurchaseRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvPurchaseRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
            //pnlSearch_ModalPopupExtender.Show();
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindSearchPurchaseRequestGrid();
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
            //pnlSearch_ModalPopupExtender.Show();
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

        #region FuelCardRequestDetails
        private void BindFuelCardRequestDetails()
        {
            grvResult.DataSource = _presenter.CurrentFuelCardRequest.FuelCardRequestDetails;
            grvResult.DataBind();
        }

       
         
        
    protected void dgPurchaseRequestDetail_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            ////////this.dgPurchaseRequestDetail.EditItemIndex = -1;
            ////////BindFuelCardRequestDetails();
        }
        protected void dgPurchaseRequestDetail_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            ////////int id = (int)dgPurchaseRequestDetail.DataKeys[e.Item.ItemIndex];
            ////////int PRDId = (int)dgPurchaseRequestDetail.DataKeys[e.Item.ItemIndex];
            ////////PurchaseRequestDetail prd;

            ////////if (PRDId > 0)
            ////////    prd = _presenter.CurrentPurchaseRequest.GetPurchaseRequestDetail(PRDId);
            ////////else
            ////////    prd = (PurchaseRequestDetail)_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.ItemIndex];
            ////////try
            ////////{
            ////////    if (PRDId > 0)
            ////////    {
            ////////        _presenter.CurrentPurchaseRequest.RemovePurchaseRequestDetail(id);
            ////////        _presenter.DeletePurchaseRequestDetail(_presenter.GetPurchaseRequestDetail(id));
            ////////        _presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice - prd.EstimatedCost;
            ////////        txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
            ////////        _presenter.SaveOrUpdateLeavePurchase(_presenter.CurrentPurchaseRequest);
            ////////    }
            ////////    else
            ////////    {
            ////////        _presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Remove(prd);
            ////////        _presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice - prd.EstimatedCost;
            ////////        txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
            ////////    }
            ////////    BindPurchaseRequestDetails();

            ////////    Master.ShowMessage(new AppMessage("Purchase Request Detail was Removed Successfully", RMessageType.Info));
            ////////}
            ////////catch (Exception ex)
            ////////{
            ////////    Master.ShowMessage(new AppMessage("Error: Unable to delete Purchase Request Detail. " + ex.Message, RMessageType.Error));
            ////////    ExceptionUtility.LogException(ex, ex.Source);
            ////////    ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            ////////}


        }
        protected void dgPurchaseRequestDetail_EditCommand(object source, DataGridCommandEventArgs e)
        {
            ////////this.dgPurchaseRequestDetail.EditItemIndex = e.Item.ItemIndex;

            ////////BindPurchaseRequestDetails();
        }
        protected void dgPurchaseRequestDetail_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            ////////if (e.CommandName == "AddNew")
            ////////{
            ////////    try
            ////////    {
            ////////        PurchaseRequestDetail Detail = new PurchaseRequestDetail();
            ////////        DropDownList ddlFAccount = e.Item.FindControl("ddlFAccount") as DropDownList;
            ////////        Detail.ItemAccount = _presenter.GetItemAccount(int.Parse(ddlFAccount.SelectedValue));
            ////////        TextBox txtFAccountCode = e.Item.FindControl("txtFAccountCode") as TextBox;
            ////////        Detail.AccountCode = txtFAccountCode.Text;
            ////////        TextBox txtFItem = e.Item.FindControl("txtFItem") as TextBox;
            ////////        Detail.Item = txtFItem.Text;
            ////////        TextBox txtFQty = e.Item.FindControl("txtFQty") as TextBox;
            ////////        Detail.Qty = Convert.ToInt32(txtFQty.Text);

            ////////        TextBox txtFPriceperunit = e.Item.FindControl("txtFPriceperunit") as TextBox;
            ////////        Detail.Priceperunit = Convert.ToDecimal(txtFPriceperunit.Text);
            ////////        Detail.EstimatedCost = Convert.ToInt32(txtFQty.Text) * Convert.ToDecimal(txtFPriceperunit.Text);
            ////////        //Determine total cost
            ////////        decimal cost = 0;
            ////////        if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Count > 0)
            ////////        {

            ////////            foreach (PurchaseRequestDetail detail in _presenter.CurrentPurchaseRequest.PurchaseRequestDetails)
            ////////            {
            ////////                cost = cost + detail.EstimatedCost;
            ////////            }
            ////////        }
            ////////        _presenter.CurrentPurchaseRequest.TotalPrice = cost;
            ////////        //Determine total cost end
            ////////        _presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice + Detail.EstimatedCost;
            ////////        txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
            ////////        DropDownList ddlFProject = e.Item.FindControl("ddlFProject") as DropDownList;
            ////////        Detail.Project = _presenter.GetProject(int.Parse(ddlFProject.SelectedValue));
            ////////        DropDownList ddlFGrant = e.Item.FindControl("ddlFGrant") as DropDownList;
            ////////        Detail.Grant = _presenter.GetGrant(int.Parse(ddlFGrant.SelectedValue));
            ////////        Detail.PurchaseRequest = _presenter.CurrentPurchaseRequest;
            ////////        _presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Add(Detail);
            ////////        Master.ShowMessage(new AppMessage("Purchase Request Detail added successfully.", RMessageType.Info));
            ////////        dgPurchaseRequestDetail.EditItemIndex = -1;
            ////////        BindPurchaseRequestDetails();
            ////////    }
            ////////    catch (Exception ex)
            ////////    {
            ////////        Master.ShowMessage(new AppMessage("Error: Unable to Add Purchase Request Detail. " + ex.Message, RMessageType.Error));
            ////////        ExceptionUtility.LogException(ex, ex.Source);
            ////////        ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            ////////    }
            ////////}
        }
        protected void dgPurchaseRequestDetail_ItemDataBound(object sender, DataGridItemEventArgs e)
        {


            ////////if (e.Item.ItemType == ListItemType.Footer)
            ////////{
            ////////    DropDownList ddlFItemAccount = e.Item.FindControl("ddlFAccount") as DropDownList;
            ////////    BindAccount(ddlFItemAccount);
            ////////    DropDownList ddlFProject = e.Item.FindControl("ddlFProject") as DropDownList;
            ////////    BindProject(ddlFProject);
            ////////    DropDownList ddlFGrant = e.Item.FindControl("ddlFGrant") as DropDownList;
            ////////    BindGrant(ddlFGrant, Convert.ToInt32(ddlFProject.SelectedValue));


            ////////}
            ////////else
            ////////{


            ////////    if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails != null)
            ////////    {

            ////////        DropDownList ddlItemAccount = e.Item.FindControl("ddlAccount") as DropDownList;
            ////////        if (ddlItemAccount != null)
            ////////        {
            ////////            BindAccount(ddlItemAccount);
            ////////            if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].ItemAccount.Id != null)
            ////////            {
            ////////                ListItem liI = ddlItemAccount.Items.FindByValue(_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].ItemAccount.Id.ToString());
            ////////                if (liI != null)
            ////////                    liI.Selected = true;
            ////////            }

            ////////        }


            ////////        DropDownList ddlProject = e.Item.FindControl("ddlProject") as DropDownList;

            ////////        if (ddlProject != null)
            ////////        {
            ////////            BindProject(ddlProject);

            ////////            if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].Project != null)
            ////////            {
            ////////                ListItem li = ddlProject.Items.FindByValue(_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].Project.Id.ToString());
            ////////                if (li != null)
            ////////                    li.Selected = true;
            ////////            }
            ////////        }
            ////////        DropDownList ddlGrant = e.Item.FindControl("ddlGrant") as DropDownList;
            ////////        if (ddlGrant != null)
            ////////        {
            ////////            BindGrant(ddlGrant, Convert.ToInt32(ddlProject.SelectedValue));
            ////////            if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].Grant.Id != null)
            ////////            {
            ////////                ListItem liI = ddlGrant.Items.FindByValue(_presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.DataSetIndex].Grant.Id.ToString());
            ////////                if (liI != null)
            ////////                    liI.Selected = true;
            ////////            }

            ////////        }
            ////////    }

            ////////}


        }
        protected void dgPurchaseRequestDetail_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            ////////int id = (int)dgPurchaseRequestDetail.DataKeys[e.Item.ItemIndex];
            ////////PurchaseRequestDetail Detail;
            ////////if (id > 0)
            ////////    Detail = _presenter.CurrentPurchaseRequest.GetPurchaseRequestDetail(id);
            ////////else
            ////////    Detail = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails[e.Item.ItemIndex];

            ////////try
            ////////{
            ////////    DropDownList ddlAccount = e.Item.FindControl("ddlAccount") as DropDownList;
            ////////    Detail.ItemAccount = _presenter.GetItemAccount(int.Parse(ddlAccount.SelectedValue));
            ////////    TextBox txtAccountCode = e.Item.FindControl("txtAccountCode") as TextBox;
            ////////    Detail.AccountCode = txtAccountCode.Text;
            ////////    TextBox txtItem = e.Item.FindControl("txtItem") as TextBox;
            ////////    Detail.Item = txtItem.Text;
            ////////    TextBox txtQty = e.Item.FindControl("txtQty") as TextBox;
            ////////    Detail.Qty = Convert.ToInt32(txtQty.Text);

            ////////    TextBox txtPriceperunit = e.Item.FindControl("txtPriceperunit") as TextBox;
            ////////    Detail.Priceperunit = Convert.ToDecimal(txtPriceperunit.Text);

            ////////    //TextBox txtEstimatedCost = e.Item.FindControl("txtEstimatedCost") as TextBox;
            ////////    Detail.EstimatedCost = Convert.ToInt32(txtQty.Text) * Convert.ToDecimal(txtPriceperunit.Text);
            ////////    //Determine total cost
            ////////    decimal cost = 0;
            ////////    if (_presenter.CurrentPurchaseRequest.PurchaseRequestDetails.Count > 0)
            ////////    {

            ////////        foreach (PurchaseRequestDetail detail in _presenter.CurrentPurchaseRequest.PurchaseRequestDetails)
            ////////        {
            ////////            cost = cost + detail.EstimatedCost;
            ////////        }
            ////////    }
            ////////    _presenter.CurrentPurchaseRequest.TotalPrice = cost;
            ////////    //Determine total cost end
            ////////    //_presenter.CurrentPurchaseRequest.TotalPrice = _presenter.CurrentPurchaseRequest.TotalPrice + Detail.EstimatedCost;
            ////////    txtTotal.Text = (_presenter.CurrentPurchaseRequest.TotalPrice).ToString();
            ////////    DropDownList ddlProject = e.Item.FindControl("ddlProject") as DropDownList;
            ////////    Detail.Project = _presenter.GetProject(int.Parse(ddlProject.SelectedValue));
            ////////    DropDownList ddlGrant = e.Item.FindControl("ddlGrant") as DropDownList;
            ////////    Detail.Grant = _presenter.GetGrant(int.Parse(ddlGrant.SelectedValue));
            ////////    Detail.PurchaseRequest = _presenter.CurrentPurchaseRequest;
            ////////    Master.ShowMessage(new AppMessage("Purchase Request Detail  Updated successfully.", RMessageType.Info));
            ////////    dgPurchaseRequestDetail.EditItemIndex = -1;
            ////////    BindPurchaseRequestDetails();
            ////////}
            ////////catch (Exception ex)
            ////////{
            ////////    Master.ShowMessage(new AppMessage("Error: Unable to Update Purchase Request Detail. " + ex.Message, RMessageType.Error));
            ////////    ExceptionUtility.LogException(ex, ex.Source);
            ////////    ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            ////////}




        }
        #endregion
        protected void ddlFProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            DropDownList ddlFGrant = ddl.FindControl("ddlFGrant") as DropDownList;
            BindGrant(ddlFGrant, Convert.ToInt32(ddl.SelectedValue));
        }
        protected void ddlGrant_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            DropDownList ddlGrant = ddl.FindControl("ddlGrant") as DropDownList;
            BindGrant(ddlGrant, Convert.ToInt32(ddl.SelectedValue));
        }
        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            DropDownList ddlGrant = ddl.FindControl("ddlGrant") as DropDownList;
            BindGrant(ddlGrant, Convert.ToInt32(ddl.SelectedValue));
        }
        protected void btnRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentFuelCardRequest.FuelCardRequestDetails.Count != 0)
                {
                    //if (_presenter.CurrentFuelCardRequest.FuelCardRequestStatuses.Count != 0)
                    //{
                    _presenter.SaveOrUpdateFuelCardRequest();
                    //ClearForm();
                    BindSearchPurchaseRequestGrid();
                    Master.ShowMessage(new AppMessage("Successfully did a Fuel Card Request, Reference No - <b>'" + _presenter.CurrentFuelCardRequest.RequestNo + "'</b> ", Chai.WorkflowManagment.Enums.RMessageType.Info));
                    Log.Info(_presenter.CurrentUser().FullName + " has requested for a Fuel Card of Total Price " + _presenter.CurrentFuelCardRequest.TotalReimbursement);
                    btnRequest.Visible = false;
                    //}
                    //else
                    //{
                    //    Master.ShowMessage(new AppMessage("There is an error constracting Approval Process", Chai.WorkflowManagment.Enums.RMessageType.Error));

                    //}

                }
                else
                {
                    Master.ShowMessage(new AppMessage("You have to insert at least one purchase item detail", Chai.WorkflowManagment.Enums.RMessageType.Error));
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentFuelCardRequest.CurrentStatus == null)
                {
                    _presenter.DeleteFuelCardRequest(_presenter.CurrentFuelCardRequest);
                    Master.ShowMessage(new AppMessage("Fuel Card Request Deleted ", RMessageType.Info));
                    BindSearchPurchaseRequestGrid();
                }
                else
                    Master.ShowMessage(new AppMessage("Warning: Unable to Delete Fuel Card Request ", RMessageType.Error));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Warning: Unable to Delete Fuel Card Request " + ex.Message, RMessageType.Error));
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
            _presenter.CurrentFuelCardRequest.RemoveFCRAttachment(filePath);
            File.Delete(Server.MapPath(filePath));
            grvAttachments.DataSource = _presenter.CurrentFuelCardRequest.FCRAttachments;
            grvAttachments.DataBind();
            //Response.Redirect(Request.Url.AbsoluteUri);


        }
        private void UploadFile()
        {
            string fileName = Path.GetFileName(fuReciept.PostedFile.FileName);

            if (fileName != String.Empty)
            {



                FCRAttachment attachment = new FCRAttachment();
                attachment.FilePath = "~/FCRUploads/" + fileName;
                fuReciept.PostedFile.SaveAs(Server.MapPath("~/FCRUploads/") + fileName);
                //Response.Redirect(Request.Url.AbsoluteUri);
                _presenter.CurrentFuelCardRequest.FCRAttachments.Add(attachment);

                grvAttachments.DataSource = _presenter.CurrentFuelCardRequest.FCRAttachments;
                grvAttachments.DataBind();


            }
            else
            {
                Master.ShowMessage(new AppMessage("Please select file ", Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
        #endregion

        protected void btnImport_Click(object sender, EventArgs e)
        {
            using (XLWorkbook workbook = new XLWorkbook(FileUpload1.PostedFile.InputStream))
            {

                IXLWorksheet sheet = workbook.Worksheet(1);
                DataTable dt = new DataTable();
                bool firstRow = true;

                foreach (IXLRow row in sheet.Rows())
                {

                    if (firstRow)
                    {
                        var row1 = sheet.Row(5);
                        foreach (IXLCell cell in row1.Cells())
                        {

                            dt.Columns.Add(cell.Value.ToString());

                        }

                        firstRow = false;

                        List<FuelCardRequestDetail> list = new List<FuelCardRequestDetail>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            FuelCardRequestDetail fclist = new FuelCardRequestDetail();
                            if (dt.Rows[i]["CustomerNumber"].ToString() != "" && dt.Rows[i]["Date"].ToString() != "" && dt.Rows[i]["CardNumber"].ToString() != "")
                            {
                                fclist.CustomerNumber = dt.Rows[i]["CustomerNumber"].ToString();

                                fclist.Date = dt.Rows[i]["Date"].ToString();

                                fclist.CardNumber = dt.Rows[i]["CardNumber"].ToString();

                                fclist.CardName = dt.Rows[i]["CardName"].ToString();

                                fclist.ReceiptNumber = dt.Rows[i]["ReceiptNumber"].ToString();

                                fclist.PastMileage = dt.Rows[i]["PastMileage"].ToString();

                                fclist.CurrentMileage = dt.Rows[i]["CurrentMileage"].ToString();

                                fclist.UnitPrice = dt.Rows[i]["UnitPrice"].ToString();

                                fclist.Quantity = dt.Rows[i]["Quantity"].ToString();

                                fclist.Amount = dt.Rows[i]["Amount"].ToString();

                                fclist.Balance = dt.Rows[i]["Balance"].ToString();

                                fclist.Location = dt.Rows[i]["Location"].ToString();

                                fclist.BusinessPurposeofTrip = dt.Rows[i]["BusinessPurposeofTrip"].ToString();
                                list.Add(fclist);

                                grvResult.DataSource = list;
                                grvResult.DataBind();
                            }




                        }
                       
                    }
                    else


                    {

                        dt.Rows.Add();


                        if (dt.Rows.Count > 2)
                        {

                            int j = 0;
                            foreach (IXLCell cell in row.Cells())
                            {

                                dt.Rows[dt.Rows.Count - 1][j] = cell.Value.ToString();
                                j++;


                            }
                            List<FuelCardRequestDetail> list = new List<FuelCardRequestDetail>();

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                FuelCardRequestDetail fclist = new FuelCardRequestDetail();
                                if (dt.Rows[i]["CustomerNumber"].ToString() != "" && dt.Rows[i]["Date"].ToString() != "" && dt.Rows[i]["CardNumber"].ToString() != "")
                                {
                                    fclist.CustomerNumber = dt.Rows[i]["CustomerNumber"].ToString();

                                    fclist.Date = dt.Rows[i]["Date"].ToString();

                                    fclist.CardNumber = dt.Rows[i]["CardNumber"].ToString();

                                    fclist.CardName = dt.Rows[i]["CardName"].ToString();

                                    fclist.ReceiptNumber = dt.Rows[i]["ReceiptNumber"].ToString();

                                    fclist.PastMileage = dt.Rows[i]["PastMileage"].ToString();

                                    fclist.CurrentMileage = dt.Rows[i]["CurrentMileage"].ToString();

                                    fclist.UnitPrice = dt.Rows[i]["UnitPrice"].ToString();

                                    fclist.Quantity = dt.Rows[i]["Quantity"].ToString();

                                    fclist.Amount = dt.Rows[i]["Amount"].ToString();

                                    fclist.Balance = dt.Rows[i]["Balance"].ToString();

                                    fclist.Location = dt.Rows[i]["Location"].ToString();

                                    fclist.BusinessPurposeofTrip = dt.Rows[i]["BusinessPurposeofTrip"].ToString();
                                    list.Add(fclist);

                                    grvResult.DataSource = list;
                                    grvResult.DataBind();
                                }




                            }
                            foreach (GridViewRow item in grvResult.Rows)
                            {
                                int x = grvResult.Rows.Count;
                                if (item.RowType == DataControlRowType.DataRow)
                                {
                                    foreach (GridViewRow r in grvResult.Rows)
                                    {

                                        FuelCardRequestDetail detail = new FuelCardRequestDetail();

                                        detail.CustomerNumber = r.Cells[0].Text.ToString();
                                        detail.Date = r.Cells[1].Text.ToString();
                                        detail.CardNumber = r.Cells[2].Text.ToString();
                                        detail.CardName = r.Cells[3].Text.ToString();
                                        detail.ReceiptNumber = r.Cells[4].Text.ToString();
                                        detail.PastMileage = r.Cells[5].Text.ToString();
                                        detail.CurrentMileage = r.Cells[6].Text.ToString();
                                        detail.UnitPrice = r.Cells[7].Text.ToString();
                                        detail.Quantity = r.Cells[8].Text.ToString();
                                        detail.Amount = r.Cells[9].Text.ToString();
                                        detail.Balance = r.Cells[10].Text.ToString();
                                        detail.Location = r.Cells[11].Text.ToString();
                                        detail.BusinessPurposeofTrip = r.Cells[12].Text.ToString();


                                        _presenter.CurrentFuelCardRequest.FuelCardRequestDetails.Add(detail);



                                    }


                                }
                            }
                        }




                    }


                    
                        
                        /*   if (fclist.CardName !=null && fclist.CardNumber !=null && fclist.ReceiptNumber !=null && fclist.CustomerNumber!=null)
                           {
                               foreach (GridViewRow item in grvResult.Rows)
                               {

                                   // check row is datarow
                                   if (item.RowType == DataControlRowType.DataRow)
                                   {


                                       FuelCardRequestDetail detail = new FuelCardRequestDetail();

                                       detail.CustomerNumber = fclist.CustomerNumber;
                                       detail.Date = fclist.Date;
                                       detail.CardNumber = fclist.CardNumber;
                                       detail.CardName = fclist.CardName;
                                       detail.ReceiptNumber = fclist.ReceiptNumber;
                                       detail.PastMileage = fclist.PastMileage;

                                       detail.CurrentMileage = fclist.CurrentMileage;
                                       detail.UnitPrice = fclist.UnitPrice;
                                       detail.Quantity = fclist.Quantity;
                                       detail.Amount = fclist.Amount;
                                       detail.Balance = fclist.Balance;
                                       detail.Location = fclist.Location;
                                       detail.BusinessPurposeofTrip = fclist.BusinessPurposeofTrip;

                                       if (detail.CardName != null && detail.CardNumber != null && detail.ReceiptNumber != null && detail.CustomerNumber != null && detail.Amount != null && detail.Balance != null)
                                       {
                                           _presenter.CurrentFuelCardRequest.FuelCardRequestDetails.Add(detail);
                                       }



                                   }

                               }


                           }*/









                    }




                }

            }

        }
    }
