using System;

namespace ObserverPattern.Observer
{
    public class ClientBank : IObserver<RateEnvelope>
    {
        private IDisposable _unsubscriber;

        public virtual void Subscribe(IObservable<RateEnvelope> provider)
        {
            _unsubscriber = provider.Subscribe(this);
        }

        public virtual void Unsubscriber()
        {
            _unsubscriber.Dispose();
        }

        public void OnCompleted()
        {
            Console.WriteLine("-----------------------------------------");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("OnError");
        }

        public void OnNext(RateEnvelope value)
        {
            var output = string.Format("{0,5} {1,10}", value.Currency, value.Weight);
            Console.WriteLine(output);
        }
    }
}
