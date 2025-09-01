namespace Chai.WorkflowManagment.CoreDomain.Library
{
    public partial class Author : IEntity
    {
        public Author()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
    }
}