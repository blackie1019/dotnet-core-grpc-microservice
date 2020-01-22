#region

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Common.Logging;
using MockSite.DomainService.Utilities;

#endregion

namespace MockSite.DomainService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appHost = Host.CreateDefaultBuilder(args)
                    .ConfigureHostConfiguration(configHost => { configHost.AddEnvironmentVariables("ASPNETCORE_"); })
                    .ConfigureConsulConfig(null,true)
                    .UseMockSiteLogging("Logging")
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>()
                            .ConfigureKestrel((context, kestrelOptions) =>
                            {
                                var port = context.Configuration.GetValue<int>(PortConst.TestKey);
                                kestrelOptions.ListenAnyIP(port,
                                    listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                            });
                    }).Build();

            var logger = appHost.Services.GetRequiredService<ILogger<Program>>();
            MethodTimeLogger.SetLogger(appHost.Services.GetRequiredService<ILoggerProvider>());

            try
            {
                appHost.Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }
    }
}