using System;

namespace ObserverPattern.Observer
{
    public class RateEnvelope
    {
        public Guid Id { get; private set; }
        public double Weight { get; private set; }
        public string Description { get; private set; }
        public Currency Currency { get; private set; }
        public DateTime IssuedDate { get; private set; }

        public RateEnvelope(Currency currency, double weight)
            : this(currency, weight, string.Empty)
        {
        }

        public RateEnvelope(Currency currency, double weight, string description)
        {
            Id = Guid.NewGuid();
            IssuedDate = DateTime.Now;
            Currency = currency;
            Weight = weight;
            Description = description;
        }
    }
}
