#region

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Common.Logging;
using MockSite.Common.Logging.Utilities;
using MockSite.Core.Interfaces;
using MockSite.Core.Repositories;
using MockSite.DomainService.Utilities;
using MockSite.Message;
using OpenTracing.Contrib.Grpc.Interceptors;
using Serilog;
using Serilog.AspNetCore;

#endregion

namespace MockSite.DomainService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            InitialConsulConfig();

            var builder = new HostBuilder()
                    .ConfigureHostConfiguration(configHost => { configHost.AddEnvironmentVariables("ASPNETCORE_"); }
                    )
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddEnvironmentVariables();
                        config.AddJsonFile("appsettings.json", true);
                        config.AddJsonFile(
                            $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                            true
                        );

                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        }

                        var appConfig = config.Build();
                        var consulIp = appConfig[ConsulConfigConst.ConsulIp];
                        var consulPort = appConfig[ConsulConfigConst.ConsulPort];
                        var consulModule = appConfig[ConsulConfigConst.ConsulModule];

                        // 從 consul kv store 取得 config
                        var configProvider = ConfigHelper.GetConfig(
                            $"http://{consulIp}:{consulPort}/v1/kv/",
                            consulModule.Split(','));

                        // 將 config 塞入 application 中
                        IConfiguration configFromConsul = new ConfigurationBuilder()
                            .AddJsonFile(configProvider, "none.json", false, false)
                            .Build();
                        config.AddConfiguration(configFromConsul);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        var configInstance = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("serilogsettings.json")
                            .Build();

                        var loggerCreator = new LoggerConfiguration()
                            .ReadFrom.Configuration(configInstance);
                        if (hostContext.HostingEnvironment.IsDevelopment())
                        {
                            loggerCreator.WriteTo.Console();
                        }

                        Log.Logger = loggerCreator
                            .CreateLogger();

                        services.AddOptions();
                        services.AddSingleton<ILoggerFactory>(a => new SerilogLoggerFactory(Log.Logger));
                        services.AddSingleton<UserService.UserServiceBase, UserServiceImpl>();
                        services.AddSingleton<IUserService, MockSite.Core.Services.UserService>();
                        services.AddSingleton<IRepository, UserRepository>();
                        services.AddSingleton<IMongoRepository, MongoUserRepository>();
                        services.AddSingleton<IRedisRepository, RedisUserRepository>();
                        services.AddSingleton<Common.Data.Utilities.RedisConnectHelper>();

                        const string serviceName = "MockSite.DomainService";
                        var serviceProvider = services.BuildServiceProvider();
                        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                        var tracer = TracingHelper.InitTracer(serviceName, loggerFactory);
                        var tracingInterceptor = new ServerTracingInterceptor(tracer);
                        var host = hostContext.Configuration.GetSection(HostNameConst.TestKey).Value;
                        var port = Convert.ToInt32(hostContext.Configuration.GetSection(PortConst.TestKey).Value);

                        logger.Info($"Initialize gRPC Server on  host:{host} and port:{port} ...");

                        services.AddSingleton(
                            new GrpcServer(host, port,
                                UserService.BindService(
                                        serviceProvider.GetRequiredService<UserService.UserServiceBase>())
                                    .Intercept(tracingInterceptor)
                            ));

                        logger.Info($"gRPC Server ready to listening on {host}:{port}");
                    })
                ;

            await builder.RunConsoleAsync();
        }

        private static void InitialConsulConfig()
        {
            var consulIp = AppSettingsHelper.Instance.GetValueFromKey(ConsulConfigConst.ConsulIp);
            var consulPort = AppSettingsHelper.Instance.GetValueFromKey(ConsulConfigConst.ConsulPort);
            var consulModule = AppSettingsHelper.Instance.GetValueFromKey(ConsulConfigConst.ConsulModule);
            var client = new HttpClient {BaseAddress = new Uri($"http://{consulIp}:{consulPort}/v1/kv/")};
            var initConfigJson = File.ReadAllText(ConsulConfigConst.ConsulInitDataPath);

            // 將 config 內容打進 consul
            HttpContent contentPost = new StringContent(initConfigJson, Encoding.UTF8, "application/json");

            var result = client.PutAsync(consulModule, contentPost)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            if (result.IsSuccessStatusCode)
            {
                LoggerHelper.Instance.Info(result.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
            else
            {
                LoggerHelper.Instance.Error(result.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        }
    }
}