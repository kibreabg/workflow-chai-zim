using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    public partial class InventoryRequest : IEntity
    {
        public InventoryRequest()
        {
            this.InventoryRequestStatuses = new List<InventoryRequestStatus>();
            this.InventoryRequestDetails = new List<InventoryRequestDetail>();
        }
        public int Id { get; set; }
        public string RequestNo { get; set; }
        public int Requester { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime RequiredDateOfDelivery { get; set; }
        public string DeliverTo { get; set; }
        public string SpecialNeed { get; set; }
        public string PurposeOfRequest { get; set; }
        public int CurrentApprover { get; set; }
        public int CurrentLevel { get; set; }
        public string CurrentStatus { get; set; }
        public string ProgressStatus { get; set; }
        public virtual IList<InventoryRequestStatus> InventoryRequestStatuses { get; set; }
        public virtual IList<InventoryRequestDetail> InventoryRequestDetails { get; set; }

        #region InventoryRequestStatus
        public virtual InventoryRequestStatus GetInventoryRequestStatus(int Id)
        {
            foreach (InventoryRequestStatus srs in InventoryRequestStatuses)
            {
                if (srs.Id == Id)
                    return srs;
            }
            return null;
        }
        public virtual InventoryRequestStatus GetInventoryRequestStatusWorkflowLevel(int workflowLevel)
        {
            foreach (InventoryRequestStatus srs in InventoryRequestStatuses)
            {
                if (srs.WorkflowLevel == workflowLevel)
                    return srs;
            }
            return null;
        }
        public virtual IList<InventoryRequestStatus> GetInventoryRequestStatusByRequestId(int RequestId)
        {
            IList<InventoryRequestStatus> lsrs = new List<InventoryRequestStatus>();
            foreach (InventoryRequestStatus srs in InventoryRequestStatuses)
            {
                if (srs.InventoryRequest.Id == RequestId)
                    lsrs.Add(srs);
            }
            return lsrs;
        }
        public virtual void RemoveInventoryRequestStatus(int Id)
        {
            foreach (InventoryRequestStatus srs in InventoryRequestStatuses)
            {
                if (srs.Id == Id)
                    InventoryRequestStatuses.Remove(srs);
                break;
            }
        }

        #endregion
        #region InventoryRequestDetail
        public virtual InventoryRequestDetail GetInventoryRequestDetail(int Id)
        {
            foreach (InventoryRequestDetail srs in InventoryRequestDetails)
            {
                if (srs.Id == Id)
                    return srs;
            }
            return null;
        }
        public virtual void RemoveInventoryRequestDetail(int Id)
        {
            foreach (InventoryRequestDetail srs in InventoryRequestDetails)
            {
                if (srs.Id == Id)
                    InventoryRequestDetails.Remove(srs);
                break;
            }
        }
        #endregion

    }
}
