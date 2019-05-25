using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core.Interceptors;
using Jaeger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Common.Logging.Utilities;
using MockSite.Core.Repositories;
using MockSite.Core.Services;
using MockSite.DomainService.Utilities;
using MockSite.Message;
using OpenTracing.Contrib.Grpc.Interceptors;
using Serilog;
using Serilog.AspNetCore;
using UserService = MockSite.Message.UserService;

namespace MockSite.DomainService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            InitialConsulConfig();

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                    
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }

                    var appconfig=config.Build();
                    

                    var consulIp = appconfig[ConsulConfigConst.ConsulIp];
                    var consulPort = appconfig[ConsulConfigConst.ConsulPort];
                    var consulModule = appconfig[ConsulConfigConst.ConsulModule];
                    
                    // 從 consul kv store 取得 config
                    var configProvider = ConfigHelper.GetConfig(
                        $"http://{consulIp}:{consulPort}/v1/kv/",
                        consulModule.Split(','));
                    // 將 config 塞入 application 中
                    IConfiguration configfromconsul = new ConfigurationBuilder()
                        .AddJsonFile(configProvider, "none.json", false, false)
                        .Build();
                    config.AddConfiguration(configfromconsul);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configInstance = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("serilogsettings.json")
                        .Build();
                    
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configInstance)
                        .CreateLogger();
                    
                    services.AddOptions();
                    services.AddSingleton<ILoggerFactory>(a => new SerilogLoggerFactory(Log.Logger, false));

                    ILoggerFactory loggerFactory = new LoggerFactory().AddConsole();
                    var serviceName = "MockSite.DomainService";
                    Tracer tracer = TracingHelper.InitTracer(serviceName, loggerFactory);
                    ServerTracingInterceptor tracingInterceptor = new ServerTracingInterceptor(tracer);


                    var host = hostContext.Configuration.GetSection(HostNameConst.TestKey).Value;
                    var port = Convert.ToInt32(hostContext.Configuration.GetSection(PortConst.TestKey).Value);

                    services.AddSingleton<UserService.UserServiceBase, UserServiceImpl>();
                    services.AddSingleton<IUserService, MockSite.Core.Services.UserService>();
                    services.AddSingleton<IRepository, MockSite.Core.Repositories.UserRepository>();


                    var Services = services.BuildServiceProvider();

                    services.AddSingleton(
                        new gRPCServer(host, port,
                            Message.UserService.BindService(Services.GetRequiredService<UserService.UserServiceBase>())
                                .Intercept(tracingInterceptor)
                        ));
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

            //將 config 內容打進 consul
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