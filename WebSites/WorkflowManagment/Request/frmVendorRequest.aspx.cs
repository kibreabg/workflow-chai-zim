using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using log4net;
using log4net.Config;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.IO;
using System.Web.UI.WebControls;
using Chai.WorkflowManagment.CoreDomain.Setting;
using System.Collections.Generic;
using System.Web.UI;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public partial class frmVendorRequest : POCBasePage, IVendorRequestView
    {
        private SupplierPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                BindVendorRequests();

            }
            txtRequestDate.Text = DateTime.Today.Date.ToShortDateString();
            this._presenter.OnViewLoaded();

        }
        [CreateNew]
        public SupplierPresenter Presenter
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
                return "{B3667CAD-17ED-4E34-82DD-A02E177052AE}";
            }
        }
        #region FieldGetters
        public int GetSupplierId
        {
            get
            {
                if (grvVendorRequestList.SelectedDataKey != null)
                {
                    return Convert.ToInt32(grvVendorRequestList.SelectedDataKey.Value);
                }
                else
                {
                    return 0;
                }
            }
        }
        public string GetRequestNo
        {
            get { return AutoNumber(); }
        }
        public DateTime GetRequestDate
        {
            get { return Convert.ToDateTime(txtRequestDate.Text); }
        }
        public string GetVendorLegalEntityName
        {
            get { return txtVendorLegalName.Text; }
        }
        public string GetTradeName
        {
            get { return txtTradeName.Text; }
        }
        public string GetCertificate
        {
            get { return txtCertificate.Text; }
        }
        public string GetTaxNo
        {
            get { return txtTaxNo.Text; }
        }
        public string GetBusinessAddressLine1
        {
            get { return txtAddressLine1.Text; }
        }
        public string GetBusinessAddressLine2
        {
            get { return txtAddressLine2.Text; }
        }
        public string GetBusinessCity
        {
            get { return txtCity.Text; }
        }
        public string GetBusinessPostalCode
        {
            get { return txtBusinessPostalCode.Text; }
        }
        public string GetBusinessStateProvince
        {
            get { return txtStateProvince.Text; }
        }
        public string GetBusinessCountry
        {
            get { return txtCountry.Text; }
        }
        public string GetPostalAddressLine1
        {
            get { return txtPostalAddress1.Text; }
        }
        public string GetPostalAddressLine2
        {
            get { return txtPostalAddress2.Text; }
        }
        public string GetPostalCity
        {
            get { return txtPostalCity.Text; }
        }
        public string GetPostalPostalCode
        {
            get { return txtPostalPostalCode.Text; }
        }
        public string GetPostalStateProvince
        {
            get { return txtPostalState.Text; }
        }
        public string GetPostalCountry
        {
            get { return txtPostalCountry.Text; }
        }
        public string GetContactName
        {
            get { return txtContactName.Text; }
        }
        public string GetEmail
        {
            get { return txtEmailAddress.Text; }
        }
        public string GetWebsite
        {
            get { return txtWebsite.Text; }
        }
        public string GetPosition
        {
            get { return txtPosition.Text; }
        }
        public string GetPhoneNumber
        {
            get { return txtPhoneNumber.Text; }
        }
        public string GetCellNumber
        {
            get { return txtCellNumber.Text; }
        }
        public string GetFaxNumber
        {
            get { return txtFaxNumber.Text; }
        }
        public string GetTechnicalCapacity
        {
            get { return txtTechnicalCapacity.Text; }
        }
        public string GetTradeRef1
        {
            get { return txtTradeRef1.Text; }
        }
        public string GetTradeRef2
        {
            get { return txtTradeRef2.Text; }
        }
        #endregion
        private string AutoNumber()
        {
            return "VRN-" + _presenter.CurrentUser().Id.ToString() + "-" + (_presenter.GetLastSupplierId() + 1).ToString();
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
            _presenter.CurrentSupplier.RemoveVendorAttachment(filePath);
            File.Delete(Server.MapPath(filePath));
            grvAttachments.DataSource = _presenter.CurrentSupplier.VendorAttachments;
            grvAttachments.DataBind();
        }
        private void UploadFile()
        {
            string fileName = Path.GetFileName(fuReciept.PostedFile.FileName);

            if (fileName != String.Empty)
            {
                VendorAttachment attachment = new VendorAttachment();
                attachment.FilePath = "~/VAUploads/" + fileName;
                fuReciept.PostedFile.SaveAs(Server.MapPath("~/VAUploads/") + fileName);
                _presenter.CurrentSupplier.VendorAttachments.Add(attachment);

                grvAttachments.DataSource = _presenter.CurrentSupplier.VendorAttachments;
                grvAttachments.DataBind();
            }
            else
            {
                Master.ShowMessage(new AppMessage("Please select file ", RMessageType.Error));
            }
        }
        #endregion
        private void BindVendorRequests()
        {
            grvVendorRequestList.DataSource = _presenter.ListSuppliers(txtSrchRequestNo.Text, txtSrchRequestDate.Text);
            grvVendorRequestList.DataBind();
        }
        private void BindCashPaymentRequestFields()
        {
            _presenter.OnViewLoaded();
            if (_presenter.CurrentSupplier != null)
            {
                txtVendorLegalName.Text = _presenter.CurrentSupplier.VendorLegalEntityName;
                txtTradeName.Text = _presenter.CurrentSupplier.TradeName;
                txtCertificate.Text = _presenter.CurrentSupplier.Certificate;
                txtTaxNo.Text = _presenter.CurrentSupplier.TaxNo;
                txtAddressLine1.Text = _presenter.CurrentSupplier.BusinessAddressLine1;
                txtAddressLine2.Text = _presenter.CurrentSupplier.BusinessAddressLine2;
                txtCity.Text = _presenter.CurrentSupplier.BusinessCity;
                txtBusinessPostalCode.Text = _presenter.CurrentSupplier.BusinessPostalCode;
                txtStateProvince.Text = _presenter.CurrentSupplier.BusinessStateProvince;
                txtCountry.Text = _presenter.CurrentSupplier.BusinessCountry;
                txtPostalAddress1.Text = _presenter.CurrentSupplier.PostalAddressLine1;
                txtPostalAddress2.Text = _presenter.CurrentSupplier.PostalAddressLine2;
                txtPostalCity.Text = _presenter.CurrentSupplier.PostalCity;
                txtPostalPostalCode.Text = _presenter.CurrentSupplier.PostalPostalCode;
                txtPostalState.Text = _presenter.CurrentSupplier.PostalStateProvince;
                txtPostalCountry.Text = _presenter.CurrentSupplier.PostalCountry;
                txtContactName.Text = _presenter.CurrentSupplier.ContactName;
                txtWebsite.Text = _presenter.CurrentSupplier.Website;
                txtEmailAddress.Text = _presenter.CurrentSupplier.Email;
                txtPosition.Text = _presenter.CurrentSupplier.Position;
                txtPhoneNumber.Text = _presenter.CurrentSupplier.PhoneNumber;
                txtCellNumber.Text = _presenter.CurrentSupplier.CellNumber;
                txtFaxNumber.Text = _presenter.CurrentSupplier.FaxNumber;
                txtTechnicalCapacity.Text = _presenter.CurrentSupplier.TechnicalCapacity;
                txtTradeRef1.Text = _presenter.CurrentSupplier.TradeRef1;
                txtTradeRef2.Text = _presenter.CurrentSupplier.TradeRef2;
                BindVendorRequests();
            }
        }
        protected void grvVendorRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["VendorRequest"] = true;
            //ClearForm();
            BindCashPaymentRequestFields();
            grvAttachments.DataSource = _presenter.CurrentSupplier.VendorAttachments;
            grvAttachments.DataBind();
            if (_presenter.CurrentSupplier.CurrentStatus != null)
            {
                btnSave.Visible = false;
            }
            else
            {
                btnSave.Visible = true;
            }
        }
        protected void grvVendorRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvVendorRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
        }
        protected void grvVendorRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.DataItem != null)
            {
                Supplier supplier = e.Row.DataItem as Supplier;
                if (supplier.CurrentStatus == "Rejected")
                {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentSupplier.VendorAttachments.Count != 0)
                {
                    _presenter.SaveOrUpdateVendorRequest();
                    //BindVendorRequests();
                    Master.ShowMessage(new AppMessage("Successfully did a Vendor Request, Reference No - <b>'" + _presenter.CurrentSupplier.RequestNo + "'</b>", RMessageType.Info));
                    btnSave.Visible = false;
                }
                else
                {
                    Master.ShowMessage(new AppMessage("Please Attach Certificate", RMessageType.Error));
                }

            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage(ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindVendorRequests();
            //pnlSearch_ModalPopupExtender.Show();
            ScriptManager.RegisterStartupScript(this, GetType(), "showSearch", "showSearch();", true);
        }
    }
}