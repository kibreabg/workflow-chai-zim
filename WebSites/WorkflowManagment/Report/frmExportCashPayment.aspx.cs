﻿using System;
using Microsoft.Practices.ObjectBuilder;
using System.Collections.Generic;
using System.Data;
using OfficeOpenXml;
using Chai.WorkflowManagment.CoreDomain.Requests;

namespace Chai.WorkflowManagment.Modules.Report.Views
{
    public partial class frmExportCashPayment : POCBasePage, IExportView
    {
        private ExportPresenter _presenter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                grvCashPaymentRequestList.DataSource = _presenter.ExportCashPayment(txtDateFrom.Text, txtDateTo.Text, ddlExportType.SelectedValue);
                grvCashPaymentRequestList.DataBind();
            }
            this._presenter.OnViewLoaded();

        }

        [CreateNew]
        public ExportPresenter Presenter
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
                return "{74031D41-C86A-4777-AEB4-624BEB8A27FF}";
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            grvCashPaymentRequestList.DataSource = _presenter.ExportCashPayment(txtDateFrom.Text, txtDateTo.Text, ddlExportType.SelectedValue);
            grvCashPaymentRequestList.DataBind();
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt1 = new DataTable();
            try
            {
                dt1 = _presenter.ExportCashPayment(txtDateFrom.Text, txtDateTo.Text, ddlExportType.SelectedValue).Tables[0];

                // mySqlDataAdapter.Fill(dt1);

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Cash Payment");
                    ws.Cells["A1"].LoadFromDataTable(dt1, true);
                    //Write it back to the client
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=Cash Payment Data.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                    Response.Flush();
                    UpdateExportStatus(dt1);
                    Response.End();
                }

            }
            catch (Exception ex)
            {
                UpdateExportStatus(dt1);
            }
        }
        private void UpdateExportStatus(DataTable dt)
        {
            foreach (DataRow DR in dt.Rows)
            {
                CashPaymentRequest Request = _presenter.GetCashPaymentRequestRequest(DR.Field<string>("RefNumber"));
                Request.ExportStatus = "Exported";
                _presenter.UpdateCashPaymentRequestExportStatus(Request);
            }
        }

    }
}

