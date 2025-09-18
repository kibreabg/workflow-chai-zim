using Microsoft.Practices.ObjectBuilder;
using System;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public partial class BookList : POCBasePage, IBookListView
    {
        private BookListPresenter _presenter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                //BindProgram();
                GrvBookList.DataSource = _presenter.ListBooks(txtAuthor.Text);
                GrvBookList.DataBind();

            }
            this._presenter.OnViewLoaded();

        }
        [CreateNew]
        public BookListPresenter Presenter
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
                return "{30b6d941-bf37-4b4e-bfb7-00e9d105f99c}";
            }
        }
        public void BindProgram()
        {
            //ddlSrchSrchProgram.DataSource = _presenter.GetPrograms();
            //ddlSrchSrchProgram.DataBind();
        }
        protected void GrvBookList_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            //if (_presenter.ListBooks(txtSrchEmpNo.Text, txtSrchSrchFullName.Text, int.Parse(ddlSrchSrchProgram.SelectedValue), ddlEmpStatus.SelectedValue) != null)
            //{
            //    if (e.Row.RowType == DataControlRowType.DataRow)
            //    {
            //        Book emp = e.Row.DataItem as Book;
            //        e.Row.Cells[2].Text = emp.GetBookProgram();
            //        e.Row.Cells[3].Text = emp.GetBookPosition();
            //        e.Row.Cells[4].Text = emp.AppUser.HiredDate.ToString();
            //        e.Row.Cells[5].Text = emp.AppUser.IsActive == true ? "Active" : "In-Active";
            //        e.Row.Cells[5].ForeColor = emp.AppUser.IsActive == true ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Red;
            //    }

            //}
        }
        protected void GrvBookList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvBookList.PageIndex = e.NewPageIndex;
            GrvBookList.DataSource = _presenter.ListBooks(txtAuthor.Text);
            GrvBookList.DataBind();
        }
        protected void BtnFind_Click(object sender, EventArgs e)
        {
            GrvBookList.DataSource = _presenter.ListBooks(txtAuthor.Text);
            GrvBookList.DataBind();
        }

    }
}

