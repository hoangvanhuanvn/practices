using System;

namespace ImplicitOperator
{
    public class Person
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }

        private Person(Guid id)
        {
            Id = id;
        }

        public Person(string name)
        {
            Name = name;
        }

        public static implicit operator Person(Guid guid)
        {
            return new Person(guid);
        }
    }
}
