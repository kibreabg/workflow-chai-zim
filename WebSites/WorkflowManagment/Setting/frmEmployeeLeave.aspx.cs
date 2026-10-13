using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.ObjectBuilder;
using Newtonsoft.Json;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public partial class frmEmployeeLeave : POCBasePage, IEmployeeLeave
    {
        private EmployeeLeavePresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                BindEmployeeList();
            }

            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public EmployeeLeavePresenter Presenter
        {
            get { return this._presenter; }
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
            get { return "{973D4E9B-2721-461A-9519-8B4AF2E75B5A}"; }
        }

        public void BindEmployeeList()
        {
            foreach (AppUser appuser in _presenter.GetEmployeeList())
            {
                TreeNode root = new TreeNode(appuser.FullName, appuser.Id.ToString());
                root.SelectAction = TreeNodeSelectAction.Select;
                TrvEmployeeList.Nodes.Add(root);
            }
        }

        private void ShowButtons()
        {
            if (
                _presenter
                    .GetEmployeeLeaves(Convert.ToInt32(TrvEmployeeList.SelectedNode.Value))
                    .Count > 0
            )
            {
                btnAddContract.Enabled = false;
                btnRenewContract.Enabled = true;
                btnTerminateContract.Enabled = true;
            }
            else
            {
                btnAddContract.Enabled = true;
                btnRenewContract.Enabled = false;
                btnTerminateContract.Enabled = false;
            }
        }

        public void BindEmployeeLeaveSetting()
        {
            dgEmployeeSetting.DataSource = _presenter.GetEmployeeLeaves(
                Convert.ToInt32(TrvEmployeeList.SelectedNode.Value)
            );
            dgEmployeeSetting.DataBind();
        }

        protected void TrvEmployeeList_SelectedNodeChanged(object sender, EventArgs e)
        {
            lblSelectedEmp.Text = TrvEmployeeList.SelectedNode.Text;
            BindEmployeeLeaveSetting();
            ShowButtons();
        }

        protected void dgEmployeeSetting_CancelCommand(
            object source,
            DataGridCommandEventArgs e
        ) { }

        protected void dgEmployeeSetting_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgEmployeeSetting.EditItemIndex = e.Item.ItemIndex;
            BindEmployeeLeaveSetting();
        }

        protected void dgEmployeeSetting_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (
                e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem
            )
            {
                EmployeeLeave employeeLeave = e.Item.DataItem as EmployeeLeave;
                LinkButton lnkRecalculate = e.Item.FindControl("lnkRecalculate") as LinkButton;
                if (lnkRecalculate != null)
                {
                    lnkRecalculate.Visible = employeeLeave != null && employeeLeave.Status;
                }

                // Format the LastCalculatedDate
                if (e.Item.Cells[6].Text != "Status") // Check if it's the LastCalculated column
                {
                    try
                    {
                        DateTime lastCalcDate = Convert.ToDateTime(e.Item.Cells[6].Text);
                        e.Item.Cells[6].Text = lastCalcDate.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        // If parsing fails, leave as is
                    }
                }
            }
        }

        protected void dgEmployeeSetting_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "Recalculate")
            {
                int id = (int)dgEmployeeSetting.DataKeys[e.Item.ItemIndex];
                EmployeeLeave EmployeeLeave = _presenter.GetEmployeeLeave(id);

                try
                {
                    UpdateEmployeeLeaveBalance(EmployeeLeave);
                    Master.ShowMessage(
                        new AppMessage(
                            "Employee leave balance recalculated successfully. ",
                            Chai.WorkflowManagment.Enums.RMessageType.Info
                        )
                    );
                    dgEmployeeSetting.EditItemIndex = -1;
                    BindEmployeeLeaveSetting();
                }
                catch (Exception ex)
                {
                    Master.ShowMessage(
                        new AppMessage(
                            "Error: Unable to recalculate leave balance. " + ex.Message,
                            Chai.WorkflowManagment.Enums.RMessageType.Error
                        )
                    );
                }
            }
        }

        protected void dgEmployeeSetting_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgEmployeeSetting.DataKeys[e.Item.ItemIndex];
            Chai.WorkflowManagment.CoreDomain.Setting.EmployeeLeave EmployeeLeave =
                _presenter.GetEmployeeLeave(id);
            try
            {
                TextBox txtStartDate = e.Item.FindControl("txtStartDate") as TextBox;
                EmployeeLeave.StartDate = Convert.ToDateTime(txtStartDate.Text);
                TextBox txtEndDate = e.Item.FindControl("txtEndDate") as TextBox;
                EmployeeLeave.EndDate = Convert.ToDateTime(txtEndDate.Text);
                TextBox txtRate = e.Item.FindControl("txtRate") as TextBox;
                EmployeeLeave.Rate = Convert.ToDecimal(txtRate.Text);
                TextBox txtLeaveTaken = e.Item.FindControl("txtLeaveTaken") as TextBox;
                EmployeeLeave.LeaveTaken =
                    txtLeaveTaken.Text != "" ? Convert.ToDecimal(txtLeaveTaken.Text) : 0;
                TextBox txtBeginingBalance = e.Item.FindControl("txtBeginingBalance") as TextBox;
                EmployeeLeave.BeginingBalance =
                    txtBeginingBalance.Text != "" ? Convert.ToDecimal(txtBeginingBalance.Text) : 0;
                _presenter.SaveOrUpdateEmployeeleave(EmployeeLeave);
                Master.ShowMessage(
                    new AppMessage(
                        "Emloyee Leave setting Successfully Updated. ",
                        Chai.WorkflowManagment.Enums.RMessageType.Info
                    )
                );
                dgEmployeeSetting.EditItemIndex = -1;
                BindEmployeeLeaveSetting();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(
                    new AppMessage(
                        "Error: Unable to Update Employee Leave Setting. " + ex.Message,
                        Chai.WorkflowManagment.Enums.RMessageType.Error
                    )
                );
            }
        }

        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            pnlAddEmployeeLeave.Visible = false;
            ClearAddControls();
        }

        private void AddContract()
        {
            try
            {
                int selectedUserId = Convert.ToInt32(TrvEmployeeList.SelectedNode.Value);
                EmployeeLeave existingActive = _presenter.GetActiveEmployeeLeave(
                    selectedUserId,
                    true
                );
                if (existingActive != null)
                {
                    Master.ShowMessage(
                        new AppMessage(
                            "Error: This employee already has an active contract. Please renew or terminate the existing contract before adding a new one.",
                            Chai.WorkflowManagment.Enums.RMessageType.Error
                        )
                    );
                    return;
                }
                EmployeeLeave EL = new EmployeeLeave();
                EL.StartDate = Convert.ToDateTime(txtStartDate.Text);
                EL.EndDate = Convert.ToDateTime(txtEndDate.Text);
                EL.Rate = Convert.ToDecimal(txtRate.Text);
                EL.BeginingBalance =
                    txtBeginingBalance.Text != "" ? Convert.ToDecimal(txtBeginingBalance.Text) : 0;
                EL.LeaveTaken =
                    txtLeaveTaken.Text != "" ? Convert.ToDecimal(txtLeaveTaken.Text) : 0;
                EL.Status = true;
                EL.AppUser = _presenter.GetUser(selectedUserId);
                _presenter.SaveOrUpdateEmployeeleave(EL);
                ShowButtons();
                Master.ShowMessage(
                    new AppMessage(
                        "Employee Contract Created. ",
                        Chai.WorkflowManagment.Enums.RMessageType.Info
                    )
                );
            }
            catch (Exception ex)
            {
                Master.ShowMessage(
                    new AppMessage(
                        "Error: Unable to Create Employee Contract. " + ex.Message,
                        Chai.WorkflowManagment.Enums.RMessageType.Error
                    )
                );
            }
        }

        private void DisablePreviousContract()
        {
            EmployeeLeave empleave = _presenter.GetActiveEmployeeLeave(
                Convert.ToInt32(TrvEmployeeList.SelectedNode.Value),
                true
            );
            if (empleave != null)
            {
                empleave.Status = false;
                _presenter.SaveOrUpdateEmployeeleave(empleave);
            }
        }

        private void clearRenewalControls()
        {
            txtRStartDate.Text = "";
            txtREndDate.Text = "";
            txtRRate.Text = "";
            txtRBeginingBalance.Text = "";
            txtRLeaveTaken.Text = "";
        }

        private void ClearAddControls()
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtRate.Text = "";
            txtBeginingBalance.Text = "";
            txtLeaveTaken.Text = "";
        }

        private void RenewContract()
        {
            try
            {
                DisablePreviousContract();
                EmployeeLeave EL = new EmployeeLeave();
                EL.StartDate = Convert.ToDateTime(txtRStartDate.Text);
                EL.EndDate = Convert.ToDateTime(txtREndDate.Text);
                EL.Rate = Convert.ToDecimal(txtRRate.Text);
                EL.BeginingBalance =
                    txtRBeginingBalance.Text != ""
                        ? Convert.ToDecimal(txtRBeginingBalance.Text)
                        : 0;
                EL.Status = true;
                EL.AppUser = _presenter.GetUser(
                    Convert.ToInt32(TrvEmployeeList.SelectedNode.Value)
                );
                _presenter.SaveOrUpdateEmployeeleave(EL);
                ShowButtons();
                Master.ShowMessage(
                    new AppMessage(
                        "Employee Contract Renewed. ",
                        Chai.WorkflowManagment.Enums.RMessageType.Info
                    )
                );
            }
            catch (Exception ex)
            {
                Master.ShowMessage(
                    new AppMessage(
                        "Error: Unable to Renew Employee Contract. " + ex.Message,
                        Chai.WorkflowManagment.Enums.RMessageType.Error
                    )
                );
            }
        }

        private void TerminateContract() { }

        private decimal CalculateLeave(EmployeeLeave empleave)
        {
            decimal res = 0;
            if (empleave != null)
            {
                decimal workingdays = Convert.ToDecimal(
                    (DateTime.Today.Date - empleave.StartDate).TotalDays
                );
                decimal leavedays = (workingdays / 30) * empleave.Rate;
                res = empleave.BeginingBalance + leavedays - empleave.LeaveTaken;
            }
            if (res < 0)
                return 0;
            else
                return Math.Round(res);
        }

        private void UpdateEmployeeLeaveBalance(EmployeeLeave employeeLeave)
        {
            if (employeeLeave != null)
            {
                // Use the entity's own method to update balance
                employeeLeave.UpdateBalance(
                    DateTime.Today.Year,
                    (uid, y) => CalculateLeavesTakenForYear(uid, y)
                );

                // Save the updated entity
                _presenter.SaveOrUpdateEmployeeleave(employeeLeave);
            }
        }

        private decimal CalculateLeavesTakenForYear(int userId, int year)
        {
            // Query leave requests for current year
            // This would use the existing presenter methods
            return _presenter
                .GetAnnualLeaveRequestsByRequester(userId)
                .Where(lr => lr.RequestedDate.Year == year)
                .Sum(lr => lr.RequestedDays);
        }

        private void CalculateRemainingLeave(string caller)
        {
            EmployeeLeave EL = _presenter.GetActiveEmployeeLeave(
                Convert.ToInt32(TrvEmployeeList.SelectedNode.Value),
                true
            );
            if (EL != null)
            {
                if (caller != "Terminate")
                {
                    txtRBeginingBalance.Text = Convert.ToString(0); // Convert.t0CalculateLeave(EL).ToString();//Calculate leave
                }
                EL.Status = false;
            }
        }

        private void CalculateAvailableBalance(EmployeeLeave el)
        {
            if (el != null)
            {
                // Use the entity's own method to calculate available balance
                el.CalculateAvailableBalance(
                    DateTime.Today.Year,
                    (uid, y) => CalculateLeavesTakenForYear(uid, y)
                );
            }
        }

        protected void btnRenew_Click(object sender, EventArgs e)
        {
            RenewContract();
            BindEmployeeLeaveSetting();
            clearRenewalControls();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            AddContract();
            BindEmployeeLeaveSetting();
        }

        protected void btnRenewContract_Click(object sender, EventArgs e)
        {
            pnlRenewEmployeeLeave.Visible = true;
            CalculateRemainingLeave("Renew");
            BindEmployeeLeaveSetting();
        }

        protected void btnAddContract_Click(object sender, EventArgs e)
        {
            pnlAddEmployeeLeave.Visible = true;
        }

        protected void btnCancelRenew_Click(object sender, EventArgs e)
        {
            pnlRenewEmployeeLeave.Visible = false;
            clearRenewalControls();
        }

        protected void btnTerminateContract_Click(object sender, EventArgs e)
        {
            DisablePreviousContract();

            AppUser User = _presenter.GetUser(Convert.ToInt32(TrvEmployeeList.SelectedNode.Value));
            try
            {
                User.IsActive = false;
                User.UserName = User.UserName + "old";
                User.TerminationDate = DateTime.Today;
                var emppos = User.EmployeePosition;
                User.EmployeePosition = null;
                _presenter.SaveOrUpdateAppUser(User);
                Master.ShowMessage(
                    new AppMessage(
                        "Employee Contract Terminated. ",
                        Chai.WorkflowManagment.Enums.RMessageType.Info
                    )
                );
            }
            catch (Exception ex)
            {
                Master.ShowMessage(
                    new AppMessage(
                        "Error: Unable to Terminate Employee. " + ex.Message,
                        Chai.WorkflowManagment.Enums.RMessageType.Error
                    )
                );
            }
            _presenter.CancelPage();
        }
    }
}
