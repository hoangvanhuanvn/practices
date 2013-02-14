using ExceptionHandlingBlock.Biz;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace ExceptionHandlingBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 10;
            int y = 2;
            int result1 = BusinessLogic.Devide(x, y);

            y = 0;
            int result2 = BusinessLogic.Devide(x, y);
        }
    }
}
