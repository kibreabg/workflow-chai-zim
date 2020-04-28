using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Modules.Approval.Views;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using log4net;
using log4net.Config;
using Microsoft.Practices.ObjectBuilder;
using Chai.WorkflowManagment.CoreDomain.Users;
using System.IO;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class frmVendorApproval : POCBasePage, IVendorApprovalView
    {
        private VendorApprovalPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                PopProgressStatus();
            }
            this._presenter.OnViewLoaded();
            BindSearchSupplierGrid();
            if (_presenter.CurrentSupplier.Id != 0)
            {
                PrintTransaction();
            }

        }
        [CreateNew]
        public VendorApprovalPresenter Presenter
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
                return "{DBC4CB73-69E5-42F3-84F8-09F8EA69B06D}";
            }
        }
        #region Field Getters
        public int GetSupplierId
        {
            get
            {
                if (grvSupplierList.SelectedDataKey != null)
                {
                    return Convert.ToInt32(grvSupplierList.SelectedDataKey.Value);
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion
        private void PopApprovalStatus()
        {
            ddlApprovalStatus.Items.Clear();
            ddlApprovalStatus.Items.Add(new ListItem("Select Status", "0"));
            string[] s = Enum.GetNames(typeof(ApprovalStatus));

            for (int i = 0; i < s.Length; i++)
            {
                if (GetWillStatus().Substring(0, 3) == s[i].Substring(0, 3))
                {
                    ddlApprovalStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));
                }

            }
            ddlApprovalStatus.Items.Add(new ListItem(ApprovalStatus.Rejected.ToString().Replace('_', ' '), ApprovalStatus.Rejected.ToString().Replace('_', ' ')));

        }
        private string GetWillStatus()
        {
            ApprovalSetting AS = _presenter.GetApprovalSettingforProcess(RequestType.Vendor_Request.ToString().Replace('_', ' ').ToString(), 0);
            string will = "";
            foreach (ApprovalLevel AL in AS.ApprovalLevels)
            {
                if (AL.EmployeePosition.PositionName == "Superviser/Line Manager" || AL.EmployeePosition.PositionName == "Program Manager" && _presenter.CurrentSupplier.CurrentLevel == 1)
                {
                    will = "Approve";
                    break;

                }
                /*else if (_presenter.GetUser(_presenter.CurrentSupplier.CurrentApprover).EmployeePosition.PositionName == AL.EmployeePosition.PositionName)
                {
                    will = AL.Will;
                }*/
                else
                {
                    try
                    {
                        if (_presenter.GetUser(_presenter.CurrentSupplier.CurrentApprover).EmployeePosition.PositionName == AL.EmployeePosition.PositionName)
                        {
                            will = AL.Will;
                        }
                    }
                    catch
                    {
                        if (_presenter.CurrentSupplier.CurrentApproverPosition == AL.EmployeePosition.Id)
                        {
                            will = AL.Will;
                        }
                    }
                }

            }
            return will;
        }
        private void PopProgressStatus()
        {
            string[] s = Enum.GetNames(typeof(ProgressStatus));

            for (int i = 0; i < s.Length; i++)
            {
                ddlSrchProgressStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));
                ddlSrchProgressStatus.DataBind();
            }
        }
        private void BindSupplierTypes()
        {
            ddlSupplierType.DataSource = _presenter.GetSupplierTypes();
            ddlSupplierType.DataValueField = "Id";
            ddlSupplierType.DataTextField = "SupplierTypeName";
            ddlSupplierType.DataBind();
        }
        private void BindSearchSupplierGrid()
        {
            grvSupplierList.DataSource = _presenter.ListSuppliers(txtSrchRequestNo.Text, txtSrchRequestDate.Text, ddlSrchProgressStatus.SelectedValue);
            grvSupplierList.DataBind();
        }
        private void BindVendorRequestStatus()
        {
            foreach (VendorRequestStatus VRS in _presenter.CurrentSupplier.VendorRequestStatuses)
            {
                if (VRS.WorkflowLevel == _presenter.CurrentSupplier.CurrentLevel && _presenter.CurrentSupplier.ProgressStatus != ProgressStatus.Completed.ToString())
                {
                    btnApprove.Enabled = true;
                }
                if (VRS.WorkflowLevel == _presenter.CurrentSupplier.VendorRequestStatuses.Count && VRS.ApprovalStatus != null)
                {
                    btnPrint.Enabled = true;
                }
                else
                    btnPrint.Enabled = false;
            }
        }
        private void ShowPrint()
        {
            if (_presenter.CurrentSupplier.CurrentLevel == _presenter.CurrentSupplier.VendorRequestStatuses.Count && _presenter.CurrentSupplier.ProgressStatus == ProgressStatus.Completed.ToString())
            {
                btnPrint.Enabled = true;
                SendEmailToRequester();
            }
        }
        private void SendEmail(VendorRequestStatus VRS)
        {
            if (VRS.Approver != 0)
            {
                if (_presenter.GetUser(VRS.Approver).IsAssignedJob != true)
                {
                    EmailSender.Send(_presenter.GetSuperviser(VRS.Approver).Email, "Vendor Approval", (_presenter.CurrentSupplier.AppUser.FullName).ToUpper() + " Requests for Vendor with Request No. - " + (_presenter.CurrentSupplier.RequestNo).ToUpper());
                }
                else
                {
                    EmailSender.Send(_presenter.GetSuperviser(_presenter.GetAssignedJobbycurrentuser(VRS.Approver).AssignedTo).Email, "Vendor Approval", (_presenter.CurrentSupplier.AppUser.FullName).ToUpper() + " Requests for Vendor with Request No. - " + (_presenter.CurrentSupplier.RequestNo).ToUpper());
                }
            }
            else
            {
                foreach (AppUser Payer in _presenter.GetAppUsersByEmployeePosition(VRS.ApproverPosition))
                {
                    if (Payer.IsAssignedJob != true)
                    {
                        EmailSender.Send(Payer.Email, "Vendor Approval", (_presenter.CurrentSupplier.AppUser.FullName).ToUpper() + " Requests for Vendor with Request No. - " + (_presenter.CurrentSupplier.RequestNo).ToUpper());
                    }
                    else
                    {
                        EmailSender.Send(_presenter.GetUser(_presenter.GetAssignedJobbycurrentuser(Payer.Id).AssignedTo).Email, "Vendor Approval", (_presenter.CurrentSupplier.AppUser.FullName).ToUpper() + " Requests for Vendor with Request No. - " + (_presenter.CurrentSupplier.RequestNo).ToUpper());
                    }
                }
            }
        }
        private void SendEmailRejected(VendorRequestStatus VRS)
        {
            EmailSender.Send(_presenter.GetUser(_presenter.CurrentSupplier.AppUser.Id).Email, "Vendor Request Rejection", "Your Vendor Request with Request No. - '" + (_presenter.CurrentSupplier.RequestNo).ToUpper() + "' made by " + (_presenter.GetUser(_presenter.CurrentSupplier.AppUser.Id).FullName).ToUpper() + " was Rejected by " + _presenter.CurrentUser().FullName + " for this reason - '" + (VRS.RejectedReason).ToUpper() + "'");

            if (VRS.WorkflowLevel > 1)
            {
                for (int i = 0; i + 1 < VRS.WorkflowLevel; i++)
                {
                    EmailSender.Send(_presenter.GetUser(_presenter.CurrentSupplier.VendorRequestStatuses[i].Approver).Email, "Vendor Request Rejection", "Vendor Request with Request No. - '" + (_presenter.CurrentSupplier.RequestNo).ToUpper() + "' made by " + (_presenter.GetUser(_presenter.CurrentSupplier.AppUser.Id).FullName).ToUpper() + " was Rejected by " + _presenter.CurrentUser().FullName + " for this reason - '" + (VRS.RejectedReason).ToUpper() + "'");
                }
            }
        }
        private void SendEmailToRequester()
        {
            if (_presenter.CurrentSupplier.CurrentStatus != ApprovalStatus.Rejected.ToString())
                EmailSender.Send(_presenter.GetUser(_presenter.CurrentSupplier.AppUser.Id).Email, "Vendor Completion", "Your Vendor Request with Request No. - '" + (_presenter.CurrentSupplier.RequestNo).ToUpper() + "' was Completed. Your funds are being processed.");
        }
        private void GetNextApprover()
        {
            foreach (VendorRequestStatus VRS in _presenter.CurrentSupplier.VendorRequestStatuses)
            {
                if (VRS.ApprovalStatus == null)
                {
                    if (VRS.Approver == 0)
                    {
                        //This is to handle multiple Finance Officers responding to this request
                        //SendEmailToFinanceOfficers;
                        _presenter.CurrentSupplier.CurrentApproverPosition = VRS.ApproverPosition;
                    }
                    SendEmail(VRS);
                    _presenter.CurrentSupplier.CurrentApprover = VRS.Approver;
                    _presenter.CurrentSupplier.CurrentLevel = VRS.WorkflowLevel;
                    _presenter.CurrentSupplier.CurrentStatus = VRS.ApprovalStatus;
                    _presenter.CurrentSupplier.ProgressStatus = ProgressStatus.InProgress.ToString();
                    break;
                }
            }
        }
        private void PrintTransaction()
        {
            lblRequestNo.Text = _presenter.CurrentSupplier.RequestNo;
            lblRequestDate.Text = _presenter.CurrentSupplier.RequestDate.ToString();
            lblVendorLegalName.Text = _presenter.CurrentSupplier.VendorLegalEntityName;
            lblBusinessAdd1.Text = _presenter.CurrentSupplier.BusinessAddressLine1;
            lblCellNo.Text = _presenter.CurrentSupplier.CellNumber;
            lblContactName.Text = _presenter.CurrentSupplier.ContactName;
            lblEmail.Text = _presenter.CurrentSupplier.Email;
            lblFaxNo.Text = _presenter.CurrentSupplier.FaxNumber;
            lblPhoneNo.Text = _presenter.CurrentSupplier.PhoneNumber;
            lblTaxNo.Text = _presenter.CurrentSupplier.TaxNo;
            lblTradeRef.Text = _presenter.CurrentSupplier.TradeRef1;
            lblWebsite.Text = _presenter.CurrentSupplier.Website;

            grvStatuses.DataSource = _presenter.CurrentSupplier.VendorRequestStatuses;
            grvStatuses.DataBind();

        }
        private void SaveVendorRequestStatus()
        {
            foreach (VendorRequestStatus VRS in _presenter.CurrentSupplier.VendorRequestStatuses)
            {
                if ((VRS.Approver == _presenter.CurrentUser().Id || (VRS.ApproverPosition == _presenter.CurrentUser().EmployeePosition.Id) || _presenter.CurrentUser().Id == (_presenter.GetAssignedJobbycurrentuser(VRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(VRS.Approver).AssignedTo : 0)) && VRS.WorkflowLevel == _presenter.CurrentSupplier.CurrentLevel)
                {
                    VRS.ApprovalStatus = ddlApprovalStatus.SelectedValue;
                    VRS.Date = Convert.ToDateTime(DateTime.Today.ToShortDateString());
                    VRS.AssignedBy = _presenter.GetAssignedJobbycurrentuser(VRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(VRS.Approver).AppUser.FullName : "";
                    VRS.RejectedReason = txtRejectedReason.Text;
                    if (VRS.ApprovalStatus != ApprovalStatus.Rejected.ToString())
                    {
                        if (_presenter.CurrentSupplier.CurrentLevel == _presenter.CurrentSupplier.VendorRequestStatuses.Count)
                        {
                            _presenter.CurrentSupplier.ProgressStatus = ProgressStatus.Completed.ToString();
                            _presenter.CurrentSupplier.Status = "Active";
                            VRS.Approver = _presenter.CurrentUser().Id;                            
                        }
                        _presenter.CurrentSupplier.CurrentStatus = VRS.ApprovalStatus;
                        GetNextApprover();
                        Log.Info(_presenter.GetUser(VRS.Approver).FullName + " has " + VRS.ApprovalStatus + " Vendor Request made by " + _presenter.CurrentSupplier.AppUser.FullName);
                    }
                    else
                    {
                        _presenter.CurrentSupplier.ProgressStatus = ProgressStatus.Completed.ToString();
                        _presenter.CurrentSupplier.CurrentStatus = VRS.ApprovalStatus;
                        VRS.Approver = _presenter.CurrentUser().Id;
                        SendEmailRejected(VRS);
                        Log.Info(_presenter.GetUser(VRS.Approver).FullName + " has " + VRS.ApprovalStatus + " Vendor Request made by " + _presenter.CurrentSupplier.AppUser.FullName);
                    }
                    break;
                }

            }
        }
        protected void grvSupplierList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Page")
            {
                if (e.CommandName == "ViewItem")
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    int reqId = Convert.ToInt32(grvSupplierList.DataKeys[rowIndex].Value);
                    Session["CurrentSupplier"] = _presenter.GetSupplier(reqId);
                    _presenter.CurrentSupplier = (Supplier)Session["CurrentSupplier"];
                    grvAttachments.DataSource = _presenter.CurrentSupplier.VendorAttachments;
                    grvAttachments.DataBind();
                    //_presenter.OnViewLoaded();
                    pnlDetail_ModalPopupExtender.Show();
                }
            }
        }
        protected void grvSupplierList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Button btnStatus = e.Row.FindControl("btnStatus") as Button;
            Supplier SuppStatus = e.Row.DataItem as Supplier;
            if (SuppStatus != null)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (SuppStatus.ProgressStatus == ProgressStatus.InProgress.ToString())
                    {
                        btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFF6C");

                    }
                    else if (SuppStatus.ProgressStatus == ProgressStatus.Completed.ToString())
                    {
                        btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF7251");

                    }
                }
            }
        }
        protected void grvSupplierList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.OnViewLoaded();
            PopApprovalStatus();
            BindVendorRequestStatus();
            BindSupplierTypes();
            if (_presenter.CurrentUser().EmployeePosition.PositionName == "Admin Officer")
            {
                pnlSupplierType.Visible = true;
                rfvSupplierType.Enabled = true;
            }
            txtRejectedReason.Visible = false;
            rfvRejectedReason.Enabled = false;
            pnlApproval_ModalPopupExtender.Show();
        }
        protected void grvSupplierList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvSupplierList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
        }
        protected void grvStatuses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (_presenter.CurrentSupplier.VendorRequestStatuses != null)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (_presenter.CurrentSupplier.VendorRequestStatuses[e.Row.RowIndex].Approver != 0)
                        e.Row.Cells[1].Text = _presenter.GetUser(_presenter.CurrentSupplier.VendorRequestStatuses[e.Row.RowIndex].Approver).FullName;
                }
            }
        }
        protected void downloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
            pnlDetail_ModalPopupExtender.Show();
        }
        protected void deleteFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            _presenter.CurrentSupplier.RemoveVendorAttachment(filePath);
            File.Delete(Server.MapPath(filePath));
            grvAttachments.DataSource = _presenter.CurrentSupplier.VendorAttachments;
            grvAttachments.DataBind();
            pnlDetail_ModalPopupExtender.Show();
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindSearchSupplierGrid();
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentSupplier.ProgressStatus != ProgressStatus.Completed.ToString())
                {
                    SaveVendorRequestStatus();
                    if (_presenter.CurrentUser().EmployeePosition.PositionName == "Admin Officer")
                    {
                        _presenter.CurrentSupplier.SupplierType = _presenter.GetSupplierType(Convert.ToInt32(ddlSupplierType.SelectedValue));
                    }
                    _presenter.SaveOrUpdateSupplier(_presenter.CurrentSupplier);
                    ShowPrint();
                    if (ddlApprovalStatus.SelectedValue != "Rejected")
                        Master.ShowMessage(new AppMessage("Vendor Approval Processed", RMessageType.Info));
                    else
                        Master.ShowMessage(new AppMessage("Vendor Approval Rejected", RMessageType.Info));
                    btnApprove.Enabled = false;
                    BindSearchSupplierGrid();
                    pnlApproval_ModalPopupExtender.Show();
                }
                PrintTransaction();
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            pnlApproval_ModalPopupExtender.Hide();
        }
        protected void btnCancelPopup2_Click(object sender, EventArgs e)
        {
            pnlDetail.Visible = false;
        }
        protected void ddlApprovalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApprovalStatus.SelectedValue == "Rejected")
            {
                lblRejectedReason.Visible = true;
                txtRejectedReason.Visible = true;
                rfvRejectedReason.Enabled = true;
            }
            pnlApproval_ModalPopupExtender.Show();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Default.aspx");
        }
    }
}