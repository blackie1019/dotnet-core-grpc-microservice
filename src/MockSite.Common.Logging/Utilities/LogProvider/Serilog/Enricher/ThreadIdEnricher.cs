#region

using System.Threading;
using Serilog.Core;
using Serilog.Events;

#endregion

namespace MockSite.Common.Logging.Utilities.LogProvider.Serilog.Enricher
{
    public class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ThreadId", Thread.CurrentThread.ManagedThreadId.ToString()));
        }
    }
}