using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;

namespace MockSite.Common.Core.Utilities
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder ConfigureConsulConfig(this IHostBuilder builder, ILogger logger = null, bool initConsul = false)
        {
            return builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                        true)
                    .Build();
                var consulIp = configuration.GetSection(ConsulConfigConst.ConsulIp).Value;
                var consulPort = configuration.GetSection(ConsulConfigConst.ConsulPort).Value;
                var consulModule = configuration.GetSection(ConsulConfigConst.ConsulModule).Value;

                if(initConsul) InitialConsulConfig(consulIp, consulPort, consulModule);
                var consulProvider =
                    ConsulConfigProvider.LoadConsulConfig($"http://{consulIp}:{consulPort}/v1/kv/",
                        consulModule.Split(','));

                config.AddJsonFile(consulProvider, "none.json", true, false)
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                        true)
                    .AddEnvironmentVariables();

            });
        }
        
        private static void InitialConsulConfig(string consulIp, string consulPort, string consulModule)
        {

            var client = new HttpClient {BaseAddress = new Uri($"http://{consulIp}:{consulPort}/v1/kv/")};
            var initConfigJson = File.ReadAllText(ConsulConfigConst.ConsulInitDataPath);

            // 將 config 內容打進 consul
            HttpContent contentPost = new StringContent(initConfigJson, Encoding.UTF8, "application/json");

            client.PutAsync(consulModule, contentPost)
                .ConfigureAwait(false).GetAwaiter().GetResult();

        }
    }
}