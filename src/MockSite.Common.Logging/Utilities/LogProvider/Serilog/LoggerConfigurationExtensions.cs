using MockSite.Common.Logging.Utilities.LogProvider.Serilog.Enricher;
using Serilog;
using Serilog.Configuration;

namespace MockSite.Common.Logging.Utilities.LogProvider.Serilog
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithThreadId(this LoggerEnrichmentConfiguration enrich)
        {
            return enrich.With(new ThreadIdEnricher());
        }
        
        public static LoggerConfiguration WithApplicationName(this LoggerEnrichmentConfiguration enrich)
        {
            return enrich.With(new ApplicationNameEnricher());
        }
    }
}