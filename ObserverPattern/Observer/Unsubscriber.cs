using System;
using System.Collections.Generic;

namespace ObserverPattern.Observer
{
    public class Unsubscriber : IDisposable
    {
        private readonly IList<IObserver<RateEnvelope>> _observers;
        private readonly IObserver<RateEnvelope> _observer;

        public Unsubscriber(IList<IObserver<RateEnvelope>> observers, IObserver<RateEnvelope> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null)
            {
                _observers.Remove(_observer);
            }
        }
    }
}
