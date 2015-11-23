using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.CoreDomain.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Approval
{
    [Table("BidAnalisises")]
    public partial class BidAnalysis : IEntity
    {

        public BidAnalysis()
        {
            this.Bidders = new List<Bidder>();
            this.BAAttachments = new List<BAAttachment>();
        }

        public int Id { get; set; }
        [Required]
        
        public virtual PurchaseRequest PurchaseRequest { get; set; }

        public DateTime AnalyzedDate { get; set; }
        public string Neededfor { get; set; }

        public string SpecialNeed { get; set; }
          // [ForeignKey("SupplierSelected_Id")]
        public virtual Supplier Supplier { get; set; }

        public string ReasonforSelection { get; set; }
        public int SelectedBy { get; set; }
        public string Status { get; set; }
        public virtual IList<Bidder> Bidders { get; set; }
        public virtual IList<BAAttachment> BAAttachments { get; set; }
        #region Bidder
        public virtual Bidder GetBidder(int Id)
        {

            foreach (Bidder bidder in Bidders)
            {
                if (bidder.Id == Id)
                    return bidder;

            }
            return null;
        }
        public virtual Bidder GetBidderbyRank()
        {

            foreach (Bidder bidder in Bidders)
            {
                if (bidder.Rank == 1)
                    return bidder;

            }
            return null;
        }
        public virtual IList<Bidder> GetBidderByBidAnalysisId(int AnalisisId)
        {
            IList<Bidder> Bidders = new List<Bidder>();
            foreach (Bidder bidder in Bidders)
            {
                if (bidder.BidAnalisis.Id == AnalisisId)
                    Bidders.Add(bidder);

            }
            return Bidders;
        }
        public virtual void RemoveBidder(int Id)
        {

            foreach (Bidder bidder in Bidders)
            {
                if (bidder.Id == Id)
                    Bidders.Remove(bidder);
                break;
            }

        }
        public IList<BidderItemDetail> GetAllBidderItemDetails()
        {
            IList<BidderItemDetail> details = new List<BidderItemDetail>();
            foreach (Bidder b in Bidders)
            {
                details.Add(b.BidderItemDetails[0]);
            }
            return details;
        }
        #endregion
        #region CPAttachment

        public virtual void RemoveBAAttachment(string FilePath)
        {
            foreach (BAAttachment cpa in BAAttachments)
            {
                if (cpa.FilePath == FilePath)
                {
                    BAAttachments.Remove(cpa);
                    break;
                }
            }
        }
        #endregion
    }
}
