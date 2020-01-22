#region

using Jaeger;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;

#endregion

namespace MockSite.DomainService.Utilities
{
    public static class TracingHelper
    {
        public static Tracer InitTracer(string serviceName, ILoggerFactory loggerFactory)
        {
            var samplerConfiguration = new Configuration.SamplerConfiguration(loggerFactory)
                .WithType(ConstSampler.Type)
                .WithParam(1);
            var reporterConfiguration = new Configuration.ReporterConfiguration(loggerFactory)
                .WithLogSpans(true);

            return (Tracer) new Configuration(serviceName, loggerFactory)
                .WithSampler(samplerConfiguration)
                .WithReporter(reporterConfiguration)
                .GetTracer();
        }
    }
}