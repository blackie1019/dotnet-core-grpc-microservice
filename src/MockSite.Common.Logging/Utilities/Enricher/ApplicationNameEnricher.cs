#region

using Microsoft.Extensions.Configuration;
using Serilog.Core;
using Serilog.Events;

#endregion

namespace MockSite.Common.Logging.Utilities.LogProvider.Serilog.Enricher
{
    public class ApplicationNameEnricher : ILogEventEnricher
    {
        private const string ApplicationNameKey = "ApplicationName";
        private readonly IConfiguration _config;

        public ApplicationNameEnricher(IConfiguration config)
        {
            _config = config;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var applicationName = _config.GetSection(ApplicationNameKey).Value ?? "None";

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ApplicationName", applicationName));
        }

        internal string GetApplicationName()
        {
            return _config.GetSection(ApplicationNameKey).Value ?? "None";
        }
    }
}