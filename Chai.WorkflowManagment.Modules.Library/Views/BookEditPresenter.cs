using Chai.WorkflowManagment.CoreDomain.Library;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public class BookEditPresenter : Presenter<IBookEditView>
    {
        private readonly LibraryController _libraryController;
        private Book _book;

        public BookEditPresenter([CreateNew] LibraryController libraryController)
        {
            _libraryController = libraryController;
        }

        public override void OnViewLoaded()
        {

        }

        public override void OnViewInitialized()
        {

        }

        public Book CurrentBook
        {
            get
            {
                if (_book == null)
                {
                    int id = View.GetBookId;
                    if (id > 0)
                        _book = _libraryController.GetBook(id);
                    else
                        _book = new Book();
                }
                return _book;
            }
        }
        public AppUser CurrentUser()
        {
            return _libraryController.GetCurrentUser();
        }
        public static IList<Author> GetAuthors()
        {
            return LibraryController.GetAuthors();
        }
        public Author GetAuthor(int authorId)
        {
            return _libraryController.GetAuthor(authorId);
        }
        public static IList<Genre> GetGenres()
        {
            return LibraryController.GetGenres();
        }
        public Genre GetGenre(int genreId)
        {
            return _libraryController.GetGenre(genreId);
        }
        public void SaveOrUpdateBook()
        {
            Book book = CurrentBook;

            book.Title = View.GetTitle;
            book.ISBN = View.GetISBN;
            book.PublishedYear = View.GetPublishedYear;
            book.CopiesAvailable = View.GetCopiesAvailable;
            book.Author = _libraryController.GetAuthor(View.Author.Id);
            book.Genre = _libraryController.GetGenre(View.Genre.Id);

            _libraryController.SaveOrUpdateBook(book);
        }

        public void DeleteBook()
        {
            if (CurrentBook.Id > 0)
                _libraryController.DeleteEntity<Book>(CurrentBook);
        }

        public void RedirectToBooks()
        {
            _libraryController.Navigate(String.Format("~/Library/Books.aspx?{0}=0", AppConstants.TABID));
        }
        public void RedirectPage(string url)
        {
            _libraryController.Navigate(url);
        }

    }
}







