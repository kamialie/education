using System;

namespace delegates_video_example
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var myOperator = new Operator();
            var multiplyOperation = new MultiplyOperation();
            var addOperation = new AdditiveOperation();

            myOperator.Operations += multiplyOperation.MultiplyNumbers;
            myOperator.Operations += addOperation.AddNumbers; // add delegate

            myOperator.PerformOperators(10, 20);

            myOperator.Operations -= addOperation.AddNumbers; // delete delegate

            myOperator.PerformOperators(10, 20);
        }
    }

    class MultiplyOperation
    {
        public void MultiplyNumbers(int x, int y)
        {
            Console.WriteLine(x * y);
        }
    }

    class AdditiveOperation
    {
        public void AddNumbers(int x, int y)
        {
            Console.WriteLine(x + y);
        }
    }
}
