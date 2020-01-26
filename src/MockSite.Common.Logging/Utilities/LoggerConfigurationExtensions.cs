#region

using Microsoft.Extensions.Configuration;
using MockSite.Common.Logging.Utilities.LogProvider.Serilog.Enricher;
using Serilog;
using Serilog.Configuration;

#endregion

namespace MockSite.Common.Logging.Utilities
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithThreadId(this LoggerEnrichmentConfiguration enrich)
        {
            return enrich.With(new ThreadIdEnricher());
        }

        public static LoggerConfiguration WithApplicationName(this LoggerEnrichmentConfiguration enrich,
            IConfiguration config)
        {
            return enrich.With(new ApplicationNameEnricher(config));
        }
    }
}