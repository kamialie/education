using System;
namespace delegates_video_example
{
    public class Operator
    {
        public delegate void Operation(int x, int y);
        public Operation Operations;

        public void PerformOperators(int x, int y)
        {
            if (Operations != null)
            {
                Operations(x, y);
            }
        }
    }
}
