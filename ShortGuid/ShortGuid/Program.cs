using System;
using System.Globalization;

namespace ShortGuid
{
    class Program
    {
        static void Main(string[] args)
        {
            var guid = Guid.NewGuid();
            var hashCode = TransformToInteger(guid);

            Console.WriteLine("Guid/Hashcode: {0} / {1}", guid.ToString().ToUpper(), hashCode);
            Console.WriteLine("GUID Length / HashCode length: {0} / {1}", guid.ToString().Length,
                hashCode.ToString(CultureInfo.InvariantCulture).Length);

            Console.ReadLine();
        }

        static string TransformToInteger(Guid guid)
        {
            var hashCode = guid.ToString().ToUpperInvariant().GetHashCode();
            var prefix = (hashCode > 0) ? 1 : 0;
            var computedHashCode = (hashCode > 0) ? hashCode : hashCode * -1;
            return string.Format("{0}{1}", prefix, computedHashCode);
        }
    }
}
