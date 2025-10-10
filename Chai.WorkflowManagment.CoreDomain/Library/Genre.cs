namespace Chai.WorkflowManagment.CoreDomain.Library
{
    public partial class Genre : IEntity
    {
        public Genre()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
