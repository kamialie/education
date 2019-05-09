using System;
namespace events
{
    public class Book
    {
        public string Title { get; set; }

        public void PrintTitle()
        {
            Console.WriteLine(Title);
        }
    }
}
