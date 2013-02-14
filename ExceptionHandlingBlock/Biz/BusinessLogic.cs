namespace ExceptionHandlingBlock.Biz
{
    public class BusinessLogic
    {
        public static int Devide(int x, int y)
        {
            if (y != 0)
            {
                return x / y;
            }

            throw new InvalidDataException();
        }
    }
}
