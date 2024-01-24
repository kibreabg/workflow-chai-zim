namespace Chai.WorkflowManagment.CoreDomain.Setting
{
    public partial class Inventory : IEntity
    {
        public Inventory()
        {

        }
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }

    }
}
