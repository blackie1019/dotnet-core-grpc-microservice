using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MockSite.Common.Logging.Utilities;

namespace MockSite.Common.Logging
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder UseMockSiteLogging(this IHostBuilder hostBuilder,
            string loggingSectionName)
        {
            return hostBuilder.ConfigureServices((hostContext, collection) =>
            {
                ConfigureLogging(hostContext.Configuration, collection,
                    loggingSectionName,
                    hostContext.HostingEnvironment.IsDevelopment());
            });
        }
        
        public static IWebHostBuilder UseMockSiteLogging(this IWebHostBuilder hostBuilder,
            string loggingSectionName)
        {
            return hostBuilder.ConfigureServices((hostContext, collection) =>
            {
                ConfigureLogging(hostContext.Configuration, collection,
                    loggingSectionName,
                    hostContext.HostingEnvironment.IsDevelopment());
            });
        }
        
        private static void ConfigureLogging(IConfiguration configuration, IServiceCollection collection,
            string loggingSectionName, bool isDev = false)
        {
            IConfiguration rootSection = configuration.GetSection("MockSite:Global");
            if (!rootSection.GetChildren().Any())
            {
                rootSection = configuration;
            }

            if (!rootSection.GetChildren().Any(x => x.Key.Equals("Serilog")))
            {
                throw new ArgumentException("The Serilog section doesn't exist or incorrect.");
            }

            var loggingSection = configuration.GetSection(isDev ? "Logging" : loggingSectionName);

            var loggerProvider = new MockSiteLoggerProvider(rootSection, loggingSection, isDev);
            collection.AddLogging(logging =>
            {
                logging.AddConfiguration(loggingSection);
                logging.AddProvider(loggerProvider);
            });
            collection.AddSingleton(loggerProvider);
        }
    }
}