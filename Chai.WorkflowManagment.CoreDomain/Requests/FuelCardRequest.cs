using Chai.WorkflowManagment.CoreDomain.Approval;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    public partial class FuelCardRequest : IEntity
    {

        public FuelCardRequest()
        {
            this.FuelCardRequestStatuses = new List<FuelCardRequestStatus>();
            this.FuelCardRequestDetails = new List<FuelCardRequestDetail>();
            this.FCRAttachments = new List<FCRAttachment>();
            //this.PurchaseOrders = new List<PurchaseOrder>();
            //  this.BidAnalysises = new BidAnalysis();
        }
        
        public int Id { get; set; }
        public string RequestNo { get; set; }
        public int Requester { get; set; }
        public Nullable<DateTime> RequestedDate { get; set; }
        public string CardHolderName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    
        public decimal TotalReimbursement { get; set; }
      
        public int CurrentApprover { get; set; }
        public int CurrentLevel { get; set; }
        public string CurrentStatus { get; set; }
        public string ProgressStatus { get; set; }
      
        public virtual IList<FuelCardRequestStatus> FuelCardRequestStatuses { get; set; }
        public virtual IList<FuelCardRequestDetail> FuelCardRequestDetails { get; set; }
        public virtual IList<FCRAttachment> FCRAttachments { get; set; }
        // public virtual PurchaseOrder PurchaseOrders { get; set; }
        #region FuelCardRequestStatus
        public virtual FuelCardRequestStatus GetFuelCardRequestStatus(int Id)
        {

            foreach (FuelCardRequestStatus PRS in FuelCardRequestStatuses)
            {
                if (PRS.Id == Id)
                    return PRS;

            }
            return null;
        }
        public virtual FuelCardRequestStatus GetFuelCardRequestStatusworkflowLevel(int workflowLevel)
        {

            foreach (FuelCardRequestStatus FCRS in FuelCardRequestStatuses)
            {
                if (FCRS.WorkflowLevel == workflowLevel)
                    return FCRS;

            }
            return null;
        }
        public virtual IList<FuelCardRequestStatus> GetFuelCardRequestStatusByRequestId(int RequestId)
        {
            IList<FuelCardRequestStatus> FCRS = new List<FuelCardRequestStatus>();
            foreach (FuelCardRequestStatus AR in FuelCardRequestStatuses)
            {
                if (AR.FuelCardRequest.Id == RequestId)
                    FCRS.Add(AR);

            }
            return FCRS;
        }
        public virtual void RemoveFuelCardRequestStatus(int Id)
        {

            foreach (FuelCardRequestStatus PRS in FuelCardRequestStatuses)
            {
                if (PRS.Id == Id)
                    FuelCardRequestStatuses.Remove(PRS);
                break;
            }

        }

        #endregion
        #region FuelCardRequestDetail
        public virtual FuelCardRequestDetail GetFuelCardRequestDetail(int Id)
        {

            foreach (FuelCardRequestDetail PRS in FuelCardRequestDetails)
            {
                if (PRS.Id == Id)
                    return PRS;

            }
            return null;
        }
        public virtual IList<FuelCardRequestDetail> GetFuelCardRequestDetailByFuelCardId(int FuelId)
        {
            IList<FuelCardRequestDetail> FCRS = new List<FuelCardRequestDetail>();
            foreach (FuelCardRequestDetail AR in FuelCardRequestDetails)
            {
                if (AR.FuelCardRequest.Id == FuelId)
                    FCRS.Add(AR);

            }
            return FCRS;
        }
        public virtual void RemoveFCRSDetail(int Id)
        {

            foreach (FuelCardRequestDetail FCRS in FuelCardRequestDetails)
            {
                if (FCRS.Id == Id)
                    FuelCardRequestDetails.Remove(FCRS);
                break;
            }

        }
        #endregion
        
        
        #region FCRAttachment

        public virtual void RemoveFCRAttachment(string FilePath)
        {
            foreach (FCRAttachment cpa in FCRAttachments)
            {
                if (cpa.FilePath == FilePath)
                {
                    FCRAttachments.Remove(cpa);
                    break;
                }
            }
        }
        #endregion

    }
}
