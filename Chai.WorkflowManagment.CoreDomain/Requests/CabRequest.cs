using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    public partial class CabRequest : IEntity
    {
        public CabRequest()
        {
            this.CabRequestDetails = new List<CabRequestDetail>();
            this.CabRequestStatuses = new List<CabRequestStatus>();
        }
        public int Id { get; set; }
        public string CabNo { get; set; }
        public Nullable<DateTime> RequestDate { get; set; }
        public string VisitingTeam { get; set; }
        public string PurposeOfTravel { get; set; }
        public string Comments { get; set; }
        public decimal TotalCab { get; set; }
        public string CardNo { get; set; }
        public int CurrentApprover { get; set; }
        public int CurrentApproverPosition { get; set; }
        public int CurrentLevel { get; set; }
        public string CurrentStatus { get; set; }        
        public string ProgressStatus { get; set; }
        public string ExpenseLiquidationStatus { get; set; }
        public string ExportStatus { get; set; }
        public virtual Project Project { get; set; }
        public virtual Account Account { get; set; }
        public virtual Grant Grant { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual ExpenseLiquidationRequest ExpenseLiquidationRequest { get; set; }
        public virtual IList<CabRequestDetail> CabRequestDetails { get; set; }
        public virtual IList<CabRequestStatus> CabRequestStatuses { get; set; }

        #region CabRequestDetail
        public virtual CabRequestDetail GetCabRequestDetail(int Id)
        {
            foreach (CabRequestDetail TARD in CabRequestDetails)
            {
                if (TARD.Id == Id)
                    return TARD;
            }
            return null;
        }

        public virtual IList<CabRequestDetail> GetCabRequestDetailsByTARId(int tarId)
        {
            IList<CabRequestDetail> TARDs = new List<CabRequestDetail>();
            foreach (CabRequestDetail TARD in CabRequestDetails)
            {
                if (TARD.CabRequest.Id == tarId)
                    TARDs.Add(TARD);
            }
            return TARDs;
        }
        public virtual void RemoveCabRequestDetail(int Id)
        {
            foreach (CabRequestDetail TARD in CabRequestDetails)
            {
                if (TARD.Id == Id)
                    CabRequestDetails.Remove(TARD);
                break;
            }
        }
        #endregion

    }
}
