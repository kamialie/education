using System;

namespace events
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var book = new Book()
            {
                Title = "FUCKING BOOK"
            };
            var library = new Library();
            Library.BookAdded bookAdded = delegate ()
            {
                Console.WriteLine("A book was added");
            };
            library.OnBookAdded += bookAdded;
            library.AddBook(book);
        }
    }
}
