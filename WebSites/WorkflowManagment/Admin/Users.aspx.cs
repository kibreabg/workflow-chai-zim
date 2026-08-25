using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.ObjectBuilder;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.CoreDomain.Users;
using System.Data;
using OfficeOpenXml;
using System.Linq;

namespace Chai.WorkflowManagment.Modules.Admin.Views
{
	public partial class Users : Microsoft.Practices.CompositeWeb.Web.UI.Page, IUsersView
	{
		private UsersPresenter _presenter;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				this._presenter.OnViewInitialized();
			}
			this._presenter.OnViewLoaded();
		}

		[CreateNew]
		public UsersPresenter Presenter
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

		public void BindUsers()
		{
			this.grvUser.DataSource = _presenter.SearchUser(txtUsername.Text.Trim(), txtFirstname.Text.Trim(), txtLastname.Text.Trim());
			this.grvUser.DataBind();
		}

		protected void btnFind_Click(object sender, EventArgs e)
		{
			BindUsers();
		}

		protected void btnNew_Click(object sender, EventArgs e)
		{
			_presenter.AddNewUser();
		}

		protected void btnExport_Click(object sender, EventArgs e)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Username");
			dt.Columns.Add("FirstName");
			dt.Columns.Add("LastName");
			dt.Columns.Add("Supervisor");
			dt.Columns.Add("Email");
			dt.Columns.Add("LastLogin");
			dt.Columns.Add("LastIp");
			dt.Columns.Add("Status");

			var users = _presenter.SearchUser(txtUsername.Text.Trim(), txtFirstname.Text.Trim(), txtLastname.Text.Trim()).ToList();
			foreach (var u in users)
			{
				string supervisorName = string.Empty;
				if (u.Superviser.HasValue && u.Superviser.Value > 0)
				{
					try
					{
						var sup = _presenter.Controller.GetUser(u.Superviser.Value);
						if (sup != null)
							supervisorName = sup.FullName;
					}
					catch
					{
						// ignore
					}
				}

				DataRow dr = dt.NewRow();
				dr["Username"] = u.UserName ?? string.Empty;
				dr["FirstName"] = u.FirstName ?? string.Empty;
				dr["LastName"] = u.LastName ?? string.Empty;
				dr["Supervisor"] = supervisorName;
				dr["Email"] = u.Email ?? string.Empty;
				dr["LastLogin"] = u.LastLogin.HasValue ? u.LastLogin.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
				dr["LastIp"] = u.LastIp ?? string.Empty;
				dr["Status"] = u.IsActive ? "Active" : "Inactive";
				dt.Rows.Add(dr);
			}

			using (ExcelPackage pck = new ExcelPackage())
			{
				var ws = pck.Workbook.Worksheets.Add("Users");
				ws.Cells["A1"].LoadFromDataTable(dt, true);

				Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				Response.AddHeader("content-disposition", "attachment; filename=Users.xlsx");
				Response.BinaryWrite(pck.GetAsByteArray());
				Response.Flush();
				Response.End();
			}
		}

		protected void grvUser_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType != DataControlRowType.DataRow)
				return;

			AppUser user = e.Row.DataItem as AppUser;
			if (user != null)
			{
				HyperLink hpl = e.Row.FindControl("hplEdit") as HyperLink;
				string url = string.Format("~/Admin/UserEdit.aspx?{0}=0&{1}={2}", AppConstants.TABID, AppConstants.USERID, user.Id);
				hpl.NavigateUrl = this.ResolveUrl(url);

				Label lblSupervisor = e.Row.FindControl("lblSupervisor") as Label;
				if (lblSupervisor != null)
				{
					if (user.Superviser.HasValue && user.Superviser.Value > 0)
					{
						try
						{
							var supervisor = _presenter.Controller.GetUser(user.Superviser.Value);
							if (supervisor != null)
								lblSupervisor.Text = supervisor.FullName;
						}
						catch
						{
							// ignore lookup errors and leave label empty
						}
					}
				}
			}
		}

		protected void grvUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			grvUser.PageIndex = e.NewPageIndex;
			BindUsers();
		}

	}
}
