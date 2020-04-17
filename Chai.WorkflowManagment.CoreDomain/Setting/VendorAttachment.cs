
namespace Chai.WorkflowManagment.CoreDomain.Setting
{
    public partial class VendorAttachment : IEntity
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
