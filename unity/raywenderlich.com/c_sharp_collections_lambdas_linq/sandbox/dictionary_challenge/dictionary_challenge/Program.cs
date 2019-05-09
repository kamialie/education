using System;
using System.Collections.Generic;

namespace dictionary_challenge
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var dictionary = new Dictionary<int, string>()
            {
                [2019] = "Avengers",
                [2008] = "Avatar"
            };
            int recent = 3000;
            foreach (var key in dictionary.Keys)
            {
                if (key < recent)
                    recent = key;
            }
            Console.WriteLine($"most recent movie is {dictionary[recent]} in {recent}");
        }
    }
}
