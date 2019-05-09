using System;
using System.Collections.Generic;

namespace events
{
    public class Library
    {
        public delegate void BookAdded();
        public event BookAdded OnBookAdded;
        public IList<Book> Books { get; private set; }

        public Library()
        {
            Books = new List<Book>();
        }

        public void AddBook(Book book)
        {
            Books.Add(book);
            if (OnBookAdded != null)
            {
                OnBookAdded();
            }
        }
    }
}
