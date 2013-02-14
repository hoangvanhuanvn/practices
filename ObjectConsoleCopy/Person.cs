using System;

namespace ObjectConsoleCopy
{
    public class Person
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public Guid Guid { get; private set; }

        public Person(string name)
        {
            Name = name;
            Guid = Guid.NewGuid();
        }

        public Person ShallowCopy()
        {
            return (Person)MemberwiseClone();
        }

        public Person DeepCopy()
        {
            var p = ShallowCopy();
            if (Address != null)
            {
                p.Address = Address.DeepCopy();
            }

            return p;
        }
    }
}
