using System;
using System.Collections.Generic;

namespace list_challenge
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var myList = new List<string>()
            {
                "Ray", "Sam", "Chris"
            };
            for (int i = 0; i < myList.Count; i++)
            {
                Console.WriteLine(myList[i]);
            }
        }
    }
}
