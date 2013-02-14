using System;

namespace ObjectConsoleCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            // For details about deep copy/shallow copy, let's see on http://msdn.microsoft.com/en-us/library/system.object.memberwiseclone.aspx

            var p1 = new Person("Huan")
                {
                    Address = new Address()
                    {
                        Number = 10
                    },
                };

            var p2 = p1.DeepCopy();
            Console.WriteLine("DeepCopy");
            Console.WriteLine("p1 == p2: {0}, ReferenceEquals: {1}", p1 == p2, ReferenceEquals(p1, p2));
            Console.WriteLine("p1.Name == p2.Name: {0}, ReferenceEquals: {1}", p1.Name == p2.Name, ReferenceEquals(p1.Name, p2.Name));
            Console.WriteLine("p1.Guid == p2.Guid: {0}", p1.Guid == p2.Guid);
            Console.WriteLine("p1.Address == p2.Address:  {0}, ReferenceEquals: {1}", p1.Address == p2.Address, ReferenceEquals(p1.Address, p2.Address));

            p2 = p1.ShallowCopy();
            Console.WriteLine();
            Console.WriteLine("ShallowCopy");
            Console.WriteLine("p1 == p2: {0}, ReferenceEquals: {1}", p1 == p2, ReferenceEquals(p1, p2));
            Console.WriteLine("p1.Name == p2.Name: {0}, ReferenceEquals: {1}", p1.Name == p2.Name, ReferenceEquals(p1.Name, p2.Name));
            Console.WriteLine("p1.Guid == p2.Guid: {0}", p1.Guid == p2.Guid);
            Console.WriteLine("p1.Address == p2.Address:  {0}, ReferenceEquals: {1}", p1.Address == p2.Address, ReferenceEquals(p1.Address, p2.Address));

            Console.ReadKey();
        }
    }
}
