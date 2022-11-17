using Microsoft.Practices.ObjectBuilder;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Report.Views
{
    public partial class frmPurchaseReport : POCBasePage, IfrmpurchaseReportView
    {
        private frmPurchaseReportPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
            }
            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public frmPurchaseReportPresenter Presenter
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
                return "{11DD2697-6CF6-416C-98D6-848916B5393D}";
            }
        }
        private void ViewPurchaseReport()
        {

            var path = Server.MapPath("PurchaseReport.rdlc");
            var datasource = _presenter.GetPurchaseReport(txtDateFrom.Text, txtDateTo.Text);
            ReportDataSource s = new ReportDataSource("PurchaseReportDataSet", datasource.Tables[0]);
            rvPurchase.ProcessingMode = ProcessingMode.Local;
            rvPurchase.LocalReport.DataSources.Clear();
            rvPurchase.LocalReport.DataSources.Add(s);
            rvPurchase.LocalReport.ReportPath = path;
            var DateFrom = txtDateFrom.Text != "" ? txtDateFrom.Text : " ";
            var DateTo = txtDateTo.Text != "" ? txtDateTo.Text : " ";
            var param4 = new ReportParameter("DateFrom", DateFrom);
            var param5 = new ReportParameter("DateTo", DateTo);
            var parameters = new List<ReportParameter>
            {
                param4,
                param5
            };
            rvPurchase.LocalReport.SetParameters(parameters);
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            ViewPurchaseReport();
        }
    }
}

