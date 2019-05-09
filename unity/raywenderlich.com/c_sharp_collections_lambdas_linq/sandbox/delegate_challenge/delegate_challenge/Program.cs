using System;

namespace delegate_challenge
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var book = new Book()
            {
                Title = "The count of Monte Cristo"
            };
            var library = new Library();
            library.ProcessBook += book.PrintTitle;
            library.AddBook(book);
            library.ProcessBook -= book.PrintTitle;
        }
        //public static void Main(string[] args)
        //{
        //    var book = new Book()
        //    {
        //        Title = "The count of Monte Cristo"
        //    };
        //    var library = new Library();
        //    library.ProcessBook += book.PrintTitle;
        //    library.ProcessBook += delegate () // cant delete specifically this anonymous delegate, as it doesnt have a reference
        //    {
        //        do smth;
        //    };
        //    library.AddBook(book);
        //    library.ProcessBook -= book.PrintTitle;
        //}
        //public static void Main(string[] args)
        //{
        //    var book = new Book()
        //    {
        //        Title = "The count of Monte Cristo"
        //    };
        //    var library = new Library();
        //    library.ProcessBook += book.PrintTitle;
        //    Library.BookAdded myDelegate = delegate () // assigning delegate to a variable so it can be deleted later
        //    {
        //        dp smth;
        //    };
        //    library.AddBook(book);
        //    library.ProcessBook += myDelegate;
        //    library.ProcessBook -= book.PrintTitle;
        //    library.ProcessBook -= myDelegate;
        //}
    }
}
