using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    public partial class StationaryRequest : IEntity
    {
        public StationaryRequest()
        {
            this.StationaryRequestStatuses = new List<StationaryRequestStatus>();
            this.StationaryRequestDetails = new List<StationaryRequestDetail>();
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
        public virtual IList<StationaryRequestStatus> StationaryRequestStatuses { get; set; }
        public virtual IList<StationaryRequestDetail> StationaryRequestDetails { get; set; }

        #region StationaryRequestStatus
        public virtual StationaryRequestStatus GetStationaryRequestStatus(int Id)
        {
            foreach (StationaryRequestStatus srs in StationaryRequestStatuses)
            {
                if (srs.Id == Id)
                    return srs;
            }
            return null;
        }
        public virtual StationaryRequestStatus GetStationaryRequestStatusWorkflowLevel(int workflowLevel)
        {
            foreach (StationaryRequestStatus srs in StationaryRequestStatuses)
            {
                if (srs.WorkflowLevel == workflowLevel)
                    return srs;
            }
            return null;
        }
        public virtual IList<StationaryRequestStatus> GetStationaryRequestStatusByRequestId(int RequestId)
        {
            IList<StationaryRequestStatus> lsrs = new List<StationaryRequestStatus>();
            foreach (StationaryRequestStatus srs in StationaryRequestStatuses)
            {
                if (srs.StationaryRequest.Id == RequestId)
                    lsrs.Add(srs);
            }
            return lsrs;
        }
        public virtual void RemoveStationaryRequestStatus(int Id)
        {
            foreach (StationaryRequestStatus srs in StationaryRequestStatuses)
            {
                if (srs.Id == Id)
                    StationaryRequestStatuses.Remove(srs);
                break;
            }
        }

        #endregion
        #region StationaryRequestDetail
        public virtual StationaryRequestDetail GetStationaryRequestDetail(int Id)
        {
            foreach (StationaryRequestDetail srs in StationaryRequestDetails)
            {
                if (srs.Id == Id)
                    return srs;
            }
            return null;
        }
        public virtual void RemoveStationaryRequestDetail(int Id)
        {
            foreach (StationaryRequestDetail srs in StationaryRequestDetails)
            {
                if (srs.Id == Id)
                    StationaryRequestDetails.Remove(srs);
                break;
            }
        }
        #endregion

    }
}
