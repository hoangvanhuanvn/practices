using System;
using System.Collections.Generic;

namespace ObserverPattern.Observer
{
    public class StateBank : IStateBank
    {
        private readonly Random _randomizer = new Random();
        private readonly IList<IObserver<RateEnvelope>> _observers;

        public StateBank()
        {
            _observers = new List<IObserver<RateEnvelope>>();
        }

        public IDisposable Subscribe(IObserver<RateEnvelope> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                foreach (var envelope in GetUpdatedEnvelopes())
                {
                    observer.OnNext(envelope);
                }
                observer.OnCompleted();
            }
        }

        private IEnumerable<RateEnvelope> GetUpdatedEnvelopes()
        {
            return new List<RateEnvelope>()
                {
                    new RateEnvelope(Currency.JPY, _randomizer.Next(800, 900), "A"),
                    new RateEnvelope(Currency.EUR, _randomizer.Next(1500, 1600), "A"),
                    new RateEnvelope(Currency.USD, _randomizer.Next(1000, 1100), "B"),
                };
        }

        public void Dispose()
        {
            foreach (var observer in _observers)
            {
            }
        }
    }
}
