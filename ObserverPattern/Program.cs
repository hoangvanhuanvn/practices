using System;
using System.Threading;
using ObserverPattern.Observer;

namespace ObserverPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            IStateBank transformer = new StateBank();

            var client1 = new ClientBank();
            client1.Subscribe(transformer);

            var client2 = new ClientBank();
            client2.Subscribe(transformer);

            var client3 = new ClientBank();
            client3.Subscribe(transformer);

            int counter = 0;
            while (counter < 1000)
            {
                transformer.Notify();

                Thread.Sleep(2000);
                counter += 1;
            }

            Console.ReadKey();
        }
    }
}
