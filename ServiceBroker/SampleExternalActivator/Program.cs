using System;
using System.Globalization;
using System.IO;

namespace SampleExternalActivator
{
    class Program
    {
        static void Main(string[] args)
        {
            const string dirName = @"C:\temp\Log";
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            var path = Path.Combine(dirName, string.Format("ExternalActivator_{0}.txt", DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(" ", "-")));
            File.WriteAllText(path, "Started at " + DateTime.Now);
        }
    }
}
