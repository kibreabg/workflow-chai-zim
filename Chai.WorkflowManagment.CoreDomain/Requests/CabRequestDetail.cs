using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    public partial class CabRequestDetail : IEntity
    {
        public CabRequestDetail()
        {
            this.CabCosts = new List<CabCost>();
        }
        public int Id { get; set; }
        public string CityFrom { get; set; }
        public string CityTo { get; set; }
        public string HotelBooked { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ModeOfTravel { get; set; }
        public int DriverId { get; set; }
        public decimal AirFare { get; set; }
        public virtual CabRequest CabRequest { get; set; }
        public virtual IList<CabCost> CabCosts { get; set; }

        #region CabCost
        public virtual CabCost GetCabCost(int tacId)
        {
            foreach (CabCost TAC in CabCosts)
            {
                if (TAC.Id == tacId)
                    return TAC;
            }
            return null;
        }

        public virtual IList<CabCost> GetCabCostsByTARId(int tarId)
        {
            IList<CabCost> TACs = new List<CabCost>();
            foreach (CabCost TAC in CabCosts)
            {
                if (TAC.CabRequestDetail.Id == tarId)
                    TACs.Add(TAC);
            }
            return TACs;
        }
        public virtual void RemoveCabCost(int Id)
        {
            foreach (CabCost TAC in CabCosts)
            {
                if (TAC.Id == Id)
                    CabCosts.Remove(TAC);
                break;
            }
        }
        #endregion

    }
}
