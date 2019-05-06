using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace MockSite.Common.Logging.Utilities.LogProvider.Serilog.Enricher
{
    public class ApplicationNameEnricher: ILogEventEnricher
    {
        private readonly string _applicationNameKey = "AppSettings:ApplicationName";
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var applicationName = ApplicationNameHelper.Instance.GetValueFromKey(_applicationNameKey) ?? "None";
            
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ApplicationName", applicationName));
        }
    }
}