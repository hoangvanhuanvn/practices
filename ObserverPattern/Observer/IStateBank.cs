using System;

namespace ObserverPattern.Observer
{
    public interface IStateBank : IObservable<RateEnvelope>, IDisposable
    {
        void Notify();
    }
}
