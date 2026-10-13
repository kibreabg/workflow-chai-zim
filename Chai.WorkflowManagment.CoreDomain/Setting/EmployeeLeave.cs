using Chai.WorkflowManagment.CoreDomain.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Setting
{
    [Table("EmployeeLeaves")]
    public partial class EmployeeLeave : IEntity
    {
        public EmployeeLeave()
        {
            Status = true;
            LeaveTaken = 0;
            AvailableBalance = 0;
            LastCalculatedDate = DateTime.Today;
        }
       
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual AppUser AppUser { get; set; }
        public decimal LeaveTaken { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public DateTime LastCalculatedDate { get; set; }
        public bool Status { get; set; }
        public decimal Rate { get; set; }
        
        // Helper methods moved to entity for better encapsulation
        public void UpdateBalance(int currentYear, Func<int, int, decimal> getLeavesTakenForYear)
        {
            if (this != null)
            {
                // Calculate leaves taken for current year
                decimal leavesTaken = getLeavesTakenForYear(this.AppUser.Id, currentYear);
                
                // Calculate working days from StartDate to today
                decimal workingdays = Convert.ToDecimal((DateTime.Today.Date - this.StartDate).TotalDays);
                
                // Calculate leavedays
                decimal leavedays = (workingdays / 30) * this.Rate;
                
                // Calculate available balance
                this.AvailableBalance = this.BeginingBalance + leavedays - leavesTaken;
                if (this.AvailableBalance < 0)
                    this.AvailableBalance = 0;
                
                // Update stored values
                this.LeaveTaken = leavesTaken;
                this.LastCalculatedDate = DateTime.Today;
            }
        }
        
        public decimal CalculateAvailableBalance(int currentYear, Func<int, int, decimal> getLeavesTakenForYear)
        {
            if (this == null)
                return 0;
                
            // Calculate leaves taken for current year
            decimal leavesTaken = getLeavesTakenForYear(this.AppUser.Id, currentYear);
            
            // Calculate working days from StartDate to today
            decimal workingdays = Convert.ToDecimal((DateTime.Today.Date - this.StartDate).TotalDays);
            
            // Calculate leavedays
            decimal leavedays = (workingdays / 30) * this.Rate;
            
            // Calculate available balance
            decimal availableBalance = this.BeginingBalance + leavedays - leavesTaken;
            if (availableBalance < 0)
                availableBalance = 0;
                
            return availableBalance;
        }
        
        public void RecalculateLeaveBalance(int currentYear, Func<int, int, decimal> getLeavesTakenForYear)
        {
            UpdateBalance(currentYear, getLeavesTakenForYear);
        }
    }

}
