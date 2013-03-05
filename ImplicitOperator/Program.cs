using System;

namespace ImplicitOperator
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid personId = Guid.NewGuid();
            Person person = personId;

            Console.WriteLine("Name: " + person.Name);
            Console.ReadLine();
        }
    }
}
