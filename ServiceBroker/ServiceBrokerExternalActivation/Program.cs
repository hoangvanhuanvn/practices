using System;
using System.Diagnostics;
using System.IO;

namespace ServiceBrokerExternalActivation
{
    class Program
    {
        static void Main(string[] args)
        {
            string sSource;
            string sLog;
            string sEvent;

            sSource = "dotNET Sample App";
            sLog = "Application";
            sEvent = "Sample Event";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog.WriteEntry(sSource, sEvent);
            EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Warning, 234);

            File.WriteAllText(@"D:\HuanHV\practices\ServiceBroker\ServiceBrokerExternalActivation\bin\Debug\info.txt", "Application starts at " + DateTime.Now);
        }
    }
}
