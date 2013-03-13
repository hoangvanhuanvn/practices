using System;
using System.Globalization;
using System.IO;

namespace ProcessingApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateNewLogFile();

            Broker broker = new Broker();

            while (true)
            {
                string msg;
                string msgType;
                Guid dialogHandle;
                Guid serviceInstance;

                broker.tran = broker.cnn.BeginTransaction();
                broker.Receive("TargetQueue", out msgType, out msg, out serviceInstance, out dialogHandle);

                if (msg == null)
                {
                    broker.tran.Commit();
                    break;
                }

                switch (msgType)
                {
                    case "http://traceone.com/ExternalActivation/RequestMessage":
                        {
                            broker.Send(dialogHandle, "<Response>This is the response message from the external activated C# program...</Response>");
                            break;
                        }
                    case "http://schemas.microsoft.com/SQL/ServiceBroker/EndDialog":
                        {
                            broker.EndDialog(dialogHandle);
                            break;
                        }
                    case "http://schemas.microsoft.com/SQL/ServiceBroker/Error":
                        {
                            broker.EndDialog(dialogHandle);
                            break;
                        }
                }

                broker.tran.Commit();
            }

            Console.WriteLine("External activated application succeeds successfully and terminates now...");
            System.Threading.Thread.Sleep(500);
        }

        private static void CreateNewLogFile()
        {
            const string dirName = @"C:\temp\Log";
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            var fileName = string.Format("ExternalActivator_{0}.txt", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            fileName = fileName.Replace(":", "-").Replace(" ", "-").Replace("/", "-");

            var path = Path.Combine(dirName, fileName);
            File.WriteAllText(path, "Started at " + DateTime.Now);
        }
    }
}