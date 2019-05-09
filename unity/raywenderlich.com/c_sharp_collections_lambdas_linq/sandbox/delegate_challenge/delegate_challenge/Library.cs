using System;
using System.Collections.Generic;

namespace delegate_challenge
{
    public class Library
    {
        public delegate void BookAdded();
        public BookAdded ProcessBook;
        public IList<Book> Books { get; private set; }

        public Library()
        {
            Books = new List<Book>();
        }

        public void AddBook(Book book)
        {
            Books.Add(book);
            ProcessBook?.Invoke();
        }
    }
}
