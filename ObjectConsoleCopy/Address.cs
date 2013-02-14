namespace ObjectConsoleCopy
{
    public class Address
    {
        public uint Number { get; set; }
        public string Street { get; set; }

        public Address DeepCopy()
        {
            return (Address)MemberwiseClone();
        }
    }
}
