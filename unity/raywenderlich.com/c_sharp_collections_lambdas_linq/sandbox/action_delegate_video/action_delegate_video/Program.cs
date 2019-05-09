using System;

namespace action_delegate_video
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var book = new Book()
            {
                Title = "Kidnapped"
            };
            Action<Book> processBook = delegate (Book aBook)
            {
                Console.WriteLine(aBook.Title);
            };
            processBook(book);
        }
    }
}
