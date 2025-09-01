namespace Chai.WorkflowManagment.CoreDomain.Library
{
    public partial class Book : IEntity
    {
        public Book()
        {

        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public int CopiesAvailable { get; set; }
        public virtual Author Author { get; set; }
        public virtual Genre Genre { get; set; }


    }
}
