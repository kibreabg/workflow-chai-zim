using Microsoft.Practices.ObjectBuilder;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Report.Views
{
    public partial class frmVehicleReport : POCBasePage, IfrmVehicleReportView
    {
        private frmVehicleReportPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
            }
            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public frmVehicleReportPresenter Presenter
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
                return "{A91F2C96-C8E9-493B-8D47-033940D3BA93}}";
            }
        }
        private void ViewVehicleReport()
        {

            var path = Server.MapPath("VehicleReport.rdlc");
            var datasource = _presenter.GetVehicleReport(txtDateFrom.Text, txtDateTo.Text);
            ReportDataSource s = new ReportDataSource("VehicleReportDataSet", datasource.Tables[0]);
            rvVehicleReport.ProcessingMode = ProcessingMode.Local;
            rvVehicleReport.LocalReport.DataSources.Clear();
            rvVehicleReport.LocalReport.DataSources.Add(s);
            rvVehicleReport.LocalReport.ReportPath = path;
            var DateFrom = txtDateFrom.Text != "" ? txtDateFrom.Text : " ";
            var DateTo = txtDateTo.Text != "" ? txtDateTo.Text : " ";
            var param4 = new ReportParameter("DateFrom", DateFrom);
            var param5 = new ReportParameter("DateTo", DateTo);
            var parameters = new List<ReportParameter>();

            parameters.Add(param4);
            parameters.Add(param5);
            rvVehicleReport.LocalReport.SetParameters(parameters);


        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            pnlVehicleReport.Visible = true;
            ViewVehicleReport();
        }

    }
}

