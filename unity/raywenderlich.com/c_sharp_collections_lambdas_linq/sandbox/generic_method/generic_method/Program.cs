using System;

namespace generic_method
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(MainClass.ConvertItem<int>(30.5));
        }

        public static Type ConvertItem<Type>(object item)
        {
            return (Type) Convert.ChangeType(item, typeof(Type));
        }
    }
}
