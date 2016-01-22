using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Modules.Approval.Views;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using log4net;
using log4net.Config;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class frmExpenseLiquidationApproval : POCBasePage, IExpenseLiquidationApprovalView
    {
        private ExpenseLiquidationApprovalPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private int liqID = 0;
        decimal _totalUnitPrice = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                PopProgressStatus();
            }
            this._presenter.OnViewLoaded();
            BindSearchExpenseLiquidationRequestGrid();
            if (_presenter.CurrentExpenseLiquidationRequest != null)
            {
                if (_presenter.CurrentExpenseLiquidationRequest.ProgressStatus == ProgressStatus.Completed.ToString())
                {
                    PrintTransaction();
                }
            }
            

        }
        [CreateNew]
        public ExpenseLiquidationApprovalPresenter Presenter
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
                return "{136836C4-1353-4DEF-A912-BF65AA84497C}";
            }
        }
        private void ShowControls()
        {
            if (_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses.Count == _presenter.CurrentExpenseLiquidationRequest.CurrentLevel && (_presenter.CurrentUser().EmployeePosition.PositionName == "Finance Officer" || _presenter.GetUser(_presenter.CurrentExpenseLiquidationRequest.CurrentApprover).IsAssignedJob == true))
            {
                lblReimbersmentType.Visible = true;
                ddlType.Visible = true;
                lblNumber.Visible = true;
                txtNumber.Visible = true;
                iReimbersmentType.Visible = true;
            }

        }
        #region Field Getters
        public int GetExpenseLiquidationRequestId
        {
            get
            {
                if (grvExpenseLiquidationRequestList.SelectedDataKey != null)
                {
                    return Convert.ToInt32(grvExpenseLiquidationRequestList.SelectedDataKey.Value);
                }
                else if (Convert.ToInt32(Session["ReqID"]) != 0)
                {
                    return Convert.ToInt32(Session["ReqID"]);
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion
        private void PopProgressStatus()
        {
            string[] s = Enum.GetNames(typeof(ProgressStatus));

            for (int i = 0; i < s.Length; i++)
            {
                ddlSrchProgressStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));
                ddlSrchProgressStatus.DataBind();
            }
        }
        private void PopApprovalStatus()
        {
            ddlApprovalStatus.Items.Clear();
            ddlApprovalStatus.Items.Add(new ListItem("Select Status", "0"));
            string[] s = Enum.GetNames(typeof(ApprovalStatus));

            for (int i = 0; i < s.Length; i++)
            {
                if (GetWillStatus().Substring(0, 2) == s[i].Substring(0, 2))
                {
                    ddlApprovalStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));
                }

            }
            ddlApprovalStatus.Items.Add(new ListItem(ApprovalStatus.Rejected.ToString().Replace('_', ' '), ApprovalStatus.Rejected.ToString().Replace('_', ' ')));

        }
        private string GetWillStatus()
        {
            ApprovalSetting AS = _presenter.GetApprovalSettingforProcess(RequestType.ExpenseLiquidation_Request.ToString().Replace('_', ' ').ToString(), 0);
            string will = "";
            foreach (ApprovalLevel AL in AS.ApprovalLevels)
            {
                if (AL.EmployeePosition.PositionName == "Superviser/Line Manager" || AL.EmployeePosition.PositionName == "Program Manager" && _presenter.CurrentExpenseLiquidationRequest.CurrentLevel == 1)
                {
                    will = "Approve";
                    break;
                }
                else if (_presenter.GetUser(_presenter.CurrentExpenseLiquidationRequest.CurrentApprover).EmployeePosition.PositionName == AL.EmployeePosition.PositionName)
                {
                    will = AL.Will;
                }

            }
            return will;
        }
        private void BindSearchExpenseLiquidationRequestGrid()
        {
            grvExpenseLiquidationRequestList.DataSource = _presenter.ListExpenseLiquidationRequests(txtSrchExpenseType.Text, txtSrchRequestDate.Text, ddlSrchProgressStatus.SelectedValue);
            grvExpenseLiquidationRequestList.DataBind();
        }
        private void BindExpenseLiquidationRequestStatus()
        {
            // ExpenseLiquidationApprovalPresenter _presenterm = new   ExpenseLiquidationApprovalPresenter;
            foreach (ExpenseLiquidationRequestStatus ELRS in _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses)
            {
                //if (ELRS.WorkflowLevel == _presenter.CurrentExpenseLiquidationRequest.CurrentLevel && _presenter.CurrentExpenseLiquidationRequest.ProgressStatus != ProgressStatus.Completed.ToString())
                //{
                //    btnApprove.Enabled = true;
                //}
                //else
                //    btnApprove.Enabled = false;
                if (ELRS.ApprovalStatus != null)
                {
                    btnPrint.Enabled = true;
                    btnExport.Enabled = true;
                }
            }
        }
        private void ShowPrint()
        {
            btnPrint.Enabled = true;
            btnExport.Enabled = true;
        }
        private void SendEmail(ExpenseLiquidationRequestStatus ELRS)
        {
            if (_presenter.GetUser(ELRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetUser(ELRS.Approver).Email, "Expense Liquidation Request", _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.AppUser.FullName + "' Request for Expense Liquidation  for Travel Advance No. '" + _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.TravelAdvanceNo + "'");
            }
            else
            {
                EmailSender.Send(_presenter.GetUser(_presenter.GetAssignedJobbycurrentuser(ELRS.Approver).AssignedTo).Email, "Expense Liquidation Request", _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.AppUser.FullName + "' Request for Expense Liquidation for Travel Advance No. '" + _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.TravelAdvanceNo + "'");
            }
            EmailSender.Send(_presenter.GetUser(ELRS.Approver).Email, "Expense Liquidation Request", "Request for Expense Liquidation");
        }
        private void SendEmailRejected(ExpenseLiquidationRequestStatus ELRS)
        {
            EmailSender.Send(_presenter.GetUser(_presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.AppUser.Id).Email, "Expense Liquidation Request", "'" + "' Your Liquidation Request for Travel Advance No. '" + _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.TravelAdvanceNo + "' was Rejected for this reason '" + ELRS.RejectedReason + "'");

            if (ELRS.WorkflowLevel > 1)
            {
                for (int i = 0; i + 1 < ELRS.WorkflowLevel; i++)
                {
                    EmailSender.Send(_presenter.GetUser(_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses[i].Approver).Email, "Expense Liquidation Request Rejection", "'" + "' Expense Liquidation Request made by " + _presenter.GetUser(_presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.AppUser.Id).FullName + " for Travel Advance No. '" + _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.TravelAdvanceNo + "' was Rejected for this reason - '" + ELRS.RejectedReason + "'");
                }
            }
        }
        private void SaveExpenseLiquidationRequestStatus()
        {
            foreach (ExpenseLiquidationRequestStatus ELRS in _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses)
            {
                if ((ELRS.Approver == _presenter.CurrentUser().Id || _presenter.CurrentUser().Id == (_presenter.GetAssignedJobbycurrentuser(ELRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(ELRS.Approver).AssignedTo : 0)) && ELRS.WorkflowLevel == _presenter.CurrentExpenseLiquidationRequest.CurrentLevel)
                {
                    ELRS.ApprovalStatus = ddlApprovalStatus.SelectedValue;
                    ELRS.RejectedReason = txtRejectedReason.Text;
                    ELRS.Date = Convert.ToDateTime(DateTime.Today.ToShortDateString());
                    ELRS.AssignedBy = _presenter.GetAssignedJobbycurrentuser(ELRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(ELRS.Approver).AppUser.FullName : "";
                    if (ELRS.ApprovalStatus != ApprovalStatus.Rejected.ToString())
                    {
                        _presenter.CurrentExpenseLiquidationRequest.ProgressStatus = ProgressStatus.Completed.ToString();
                        _presenter.CurrentExpenseLiquidationRequest.ExpenseReimbersmentType = ddlType.SelectedValue;
                        _presenter.CurrentExpenseLiquidationRequest.ReimbersmentNo = txtNumber.Text;
                        _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.ExpenseLiquidationStatus = "Finished";
                        GetNextApprover();
                        ELRS.Approver = _presenter.CurrentUser().Id;
                        Log.Info(_presenter.GetUser(ELRS.Approver).FullName + " has " + ELRS.ApprovalStatus + " Expense Liquidation Request made by " + _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.AppUser.FullName);
                    }
                    else
                    {
                         ELRS.Approver = _presenter.CurrentUser().Id;
                        _presenter.CurrentExpenseLiquidationRequest.CurrentStatus = ApprovalStatus.Rejected.ToString();
                       // _presenter.CurrentExpenseLiquidationRequest.CurrentLevel = 0;
                        SendEmailRejected(ELRS);
                        Log.Info(_presenter.GetUser(ELRS.Approver).FullName + " has " + ELRS.ApprovalStatus + " Expense Liquidation Request made by " + _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.AppUser.FullName);
                    }
                    break;
                }
              
            }
        }
        private void GetNextApprover()
        {
            foreach (ExpenseLiquidationRequestStatus ELRS in _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses)
            {
                if (ELRS.ApprovalStatus == null)
                {
                    SendEmail(ELRS);
                    _presenter.CurrentExpenseLiquidationRequest.CurrentApprover = ELRS.Approver;
                    _presenter.CurrentExpenseLiquidationRequest.CurrentLevel = ELRS.WorkflowLevel;
                    _presenter.CurrentExpenseLiquidationRequest.CurrentStatus = ELRS.ApprovalStatus;
                    _presenter.CurrentExpenseLiquidationRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    break;
                }
            }
        }
        protected void grvExpenseLiquidationRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //grvExpenseLiquidationRequestList.SelectedDataKey.Value
            _presenter.OnViewLoaded();
            PopApprovalStatus();
            btnApprove.Enabled = true;
            ShowControls();
            grvAttachments.DataSource = _presenter.CurrentExpenseLiquidationRequest.ELRAttachments;
            grvAttachments.DataBind();
            BindExpenseLiquidationRequestStatus();
            pnlApproval_ModalPopupExtender.Show();

        }
        protected void grvExpenseLiquidationRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Button btnStatus = e.Row.FindControl("btnStatus") as Button;
            ExpenseLiquidationRequest CSR = e.Row.DataItem as ExpenseLiquidationRequest;
            if (CSR != null)
            {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                    if (CSR.ProgressStatus == ProgressStatus.InProgress.ToString())
                    {
                        btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFF6C");

                    }
                    else if (CSR.ProgressStatus == ProgressStatus.Completed.ToString())
                    {
                        btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF7251");

                    }
                }
            }
        }
        protected void grvExpenseLiquidationRequestList_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            liqID = (int)grvExpenseLiquidationRequestList.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
            Session["ReqID"] = liqID;
                _presenter.CurrentExpenseLiquidationRequest = _presenter.GetExpenseLiquidationRequest(liqID);
            if (e.CommandName == "ViewItem")
            {                
                //_presenter.OnViewLoaded();
                dgLiquidationRequestDetail.DataSource = _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails;
                dgLiquidationRequestDetail.DataBind();
                pnlDetail_ModalPopupExtender.Show();
            }
        }
        protected void grvExpenseLiquidationRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvExpenseLiquidationRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
        }
        protected void grvStatuses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses != null)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[1].Text = _presenter.GetUser(_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses[e.Row.RowIndex].Approver).FullName;
                }
            }
        }
        protected void DownloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }
        protected void grvDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblVariance = (Label)e.Row.FindControl("lblVariance");
                decimal qty = Convert.ToDecimal(lblVariance.Text);
                _totalUnitPrice = _totalUnitPrice + qty;
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lblTotalqty = (Label)e.Row.FindControl("lblTotalqty");
                lblTotalqty.Text = _totalUnitPrice.ToString();
            }
        }
        
        protected void ddlApprovalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApprovalStatus.SelectedValue == "Rejected")
            {
                lblRejectedReason.Visible = true;
                txtRejectedReason.Visible = true;
            }
            else
            {
                lblRejectedReason.Visible = false;
                txtRejectedReason.Visible = false;
            }
            pnlApproval_ModalPopupExtender.Show();
        }
        protected void ddlEdtAccountDescription_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            TextBox txtAccountCode = ddl.FindControl("txtEdtAccountCode") as TextBox;
            txtAccountCode.Text = _presenter.GetItemAccount(Convert.ToInt32(ddl.SelectedValue)).AccountCode;
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindSearchExpenseLiquidationRequestGrid();
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentExpenseLiquidationRequest.ProgressStatus != ProgressStatus.Completed.ToString())
                {
                    SaveExpenseLiquidationRequestStatus();
                    _presenter.SaveOrUpdateExpenseLiquidationRequest(_presenter.CurrentExpenseLiquidationRequest);
                    ShowPrint();
                    if (ddlApprovalStatus.SelectedValue != "Rejected")
                    {
                        Master.ShowMessage(new AppMessage("Expense Liquidation Approval Processed", Chai.WorkflowManagment.Enums.RMessageType.Info));
                    }
                    else
                    {
                        Master.ShowMessage(new AppMessage("Expense Liquidation Approval Rejected", Chai.WorkflowManagment.Enums.RMessageType.Info));
                    }
                    btnApprove.Enabled = false;
                    BindSearchExpenseLiquidationRequestGrid();
                    pnlApproval_ModalPopupExtender.Show();
                    PrintTransaction();
                }

            }
            catch (Exception ex)
            {

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
        private void PrintTransaction()
        {
            //pnlApproval_ModalPopupExtender.Hide();
            TravelAdvanceRequest taRequest = _presenter.GetTravelAdvanceRequest(_presenter.CurrentExpenseLiquidationRequest.Id);
            lblRequestNoResult.Text = taRequest.TravelAdvanceNo;
            lblRequestedDateResult.Text = _presenter.CurrentExpenseLiquidationRequest.RequestDate.Value.ToShortDateString();
            lblRequesterResult.Text = taRequest.AppUser.FullName;
            //lblExpenseTypeResult.Text = _presenter.CurrentExpenseLiquidationRequest.ExpenseType.ToString();
            lblPurposeofAdvanceResult.Text = _presenter.CurrentExpenseLiquidationRequest.Comment.ToString();
            lblApprovalStatusResult.Text = _presenter.CurrentExpenseLiquidationRequest.ProgressStatus.ToString();
            //lblRetirmentTypeResult.Text = _presenter.CurrentExpenseLiquidationRequest.ExpenseReimbersmentType;
            lblRetirmenNoResult.Text = _presenter.CurrentExpenseLiquidationRequest.ReimbersmentNo;
            grvDetails.DataSource = _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails;
            grvDetails.DataBind();

            grvStatuses.DataSource = _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses;
            grvStatuses.DataBind();
        }
        public string GetMimeTypeByFileName(string sFileName)
        {
            string sMime = "application/octet-stream";

            string sExtension = System.IO.Path.GetExtension(sFileName);
            if (!string.IsNullOrEmpty(sExtension))
            {
                sExtension = sExtension.Replace(".", "");
                sExtension = sExtension.ToLower();

                if (sExtension == "xls" || sExtension == "xlsx")
                {
                    sMime = "application/ms-excel";
                }
                else if (sExtension == "doc" || sExtension == "docx")
                {
                    sMime = "application/msword";
                }
                else if (sExtension == "ppt" || sExtension == "pptx")
                {
                    sMime = "application/ms-powerpoint";
                }
                else if (sExtension == "rtf")
                {
                    sMime = "application/rtf";
                }
                else if (sExtension == "zip")
                {
                    sMime = "application/zip";
                }
                else if (sExtension == "mp3")
                {
                    sMime = "audio/mpeg";
                }
                else if (sExtension == "bmp")
                {
                    sMime = "image/bmp";
                }
                else if (sExtension == "gif")
                {
                    sMime = "image/gif";
                }
                else if (sExtension == "jpg" || sExtension == "jpeg")
                {
                    sMime = "image/jpeg";
                }
                else if (sExtension == "png")
                {
                    sMime = "image/png";
                }
                else if (sExtension == "tiff" || sExtension == "tif")
                {
                    sMime = "image/tiff";
                }
                else if (sExtension == "txt")
                {
                    sMime = "text/plain";
                }
            }

            return sMime;
        }
         
        private void BindProject(DropDownList ddlProject)
        {
            ddlProject.DataSource = _presenter.GetProjectList();
            ddlProject.DataValueField = "Id";
            ddlProject.DataTextField = "ProjectCode";
            ddlProject.DataBind();
        }
        private void BindAccountDescription(DropDownList ddlAccountDescription)
        {
            ddlAccountDescription.DataSource = _presenter.GetItemAccountList();
            ddlAccountDescription.DataValueField = "Id";
            ddlAccountDescription.DataTextField = "AccountName";
            ddlAccountDescription.DataBind();
        }
        protected void dgLiquidationRequestDetail_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails != null)
            {
                DropDownList ddlProject = e.Item.FindControl("ddlEdtProject") as DropDownList;
                if (ddlProject != null)
                {
                    BindProject(ddlProject);
                    if (_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails[e.Item.DataSetIndex].Project.Id != 0)
                    {
                        ListItem liI = ddlProject.Items.FindByValue(_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails[e.Item.DataSetIndex].Project.Id.ToString());
                        if (liI != null)
                            liI.Selected = true;
                    }
                }
                DropDownList ddlAccountDescription = e.Item.FindControl("ddlEdtAccountDescription") as DropDownList;
                if (ddlAccountDescription != null)
                {
                    BindAccountDescription(ddlAccountDescription);
                    if (_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails[e.Item.DataSetIndex].ItemAccount.Id != 0)
                    {
                        ListItem liI = ddlAccountDescription.Items.FindByValue(_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails[e.Item.DataSetIndex].ItemAccount.Id.ToString());
                        if (liI != null)
                            liI.Selected = true;
                    }
                }
            }
        }
        protected void dgLiquidationRequestDetail_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgLiquidationRequestDetail.EditItemIndex = e.Item.ItemIndex;
            dgLiquidationRequestDetail.DataSource = _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails;
            dgLiquidationRequestDetail.DataBind();
            pnlDetail_ModalPopupExtender.Show();
        }
        protected void dgLiquidationRequestDetail_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            int CPRDId = (int)dgLiquidationRequestDetail.DataKeys[e.Item.ItemIndex];
            ExpenseLiquidationRequestDetail cprd;

            if (CPRDId > 0)
                cprd = _presenter.CurrentExpenseLiquidationRequest.GetExpenseLiquidationRequestDetail(CPRDId);
            else
                cprd = (ExpenseLiquidationRequestDetail)_presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails[e.Item.ItemIndex];

            try
            {
                cprd.ExpenseLiquidationRequest = _presenter.CurrentExpenseLiquidationRequest;
                DropDownList ddlAccountDescription = e.Item.FindControl("ddlEdtAccountDescription") as DropDownList;
                cprd.ItemAccount = _presenter.GetItemAccount(Convert.ToInt32(ddlAccountDescription.SelectedValue));
                DropDownList ddlProject = e.Item.FindControl("ddlEdtProject") as DropDownList;
                cprd.Project = _presenter.GetProject(Convert.ToInt32(ddlProject.SelectedValue));
                              
                dgLiquidationRequestDetail.EditItemIndex = -1;
                dgLiquidationRequestDetail.DataSource = _presenter.CurrentExpenseLiquidationRequest.ExpenseLiquidationRequestDetails;
                dgLiquidationRequestDetail.DataBind();
                pnlDetail_ModalPopupExtender.Show();
                Master.ShowMessage(new AppMessage("Expense Liquidation Detail Successfully Updated", Chai.WorkflowManagment.Enums.RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to Update Expense Liquidation Detail. " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt1 = new DataTable();

                dt1 = _presenter.ExportTravelAdvance(_presenter.CurrentExpenseLiquidationRequest.Id).Tables[0];

                // mySqlDataAdapter.Fill(dt1);

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Journal Entry - '"+_presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.TravelAdvanceNo+"'");


                    ws.Cells["A1"].LoadFromDataTable(dt1, true);


                    //Write it back to the client
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=Journal Entry - '" + _presenter.CurrentExpenseLiquidationRequest.TravelAdvanceRequest.TravelAdvanceNo + "'.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {

            }
            pnlApproval_ModalPopupExtender.Show();
        }
}
}


