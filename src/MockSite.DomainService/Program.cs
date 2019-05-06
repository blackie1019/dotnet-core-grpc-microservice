using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Jaeger;
using Microsoft.Extensions.Logging;
using MockSite.Core.Services;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Common.Logging.Utilities;
using MockSite.Common.Logging.Utilities.LogDetail;
using MockSite.Common.Logging.Utilities.LogProvider.Serilog;
using MockSite.DomainService.Utilities;
using MockSite.Message;
using OpenTracing.Contrib.Grpc.Interceptors;
using Unity;

namespace MockSite.DomainService
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();

            var server = GenerateServerInstance();

            server.Start();

            WaitingForTerminateServerInstance(server);
        }

        private static void Initialize()
        {
            PrintLogAndConsole("Initialize Logger...");
            LoggerHelper.Instance.SetLogProvider(SeriLogProvider.Instance);
        }

        private static Server GenerateServerInstance()
        {
            PrintLogAndConsole("Initialize gRPC server ...");
            
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole();
            var serviceName = "MockSite.DomainService";
            Tracer tracer = TracingHelper.InitTracer(serviceName, loggerFactory);
            ServerTracingInterceptor tracingInterceptor = new ServerTracingInterceptor(tracer);

            var host = AppSettingsHelper.Instance.GetValueFromKey(HostNameConst.TestKey);
            var port = Convert.ToInt32(AppSettingsHelper.Instance.GetValueFromKey(PortConst.TestKey));

            var server = new Server
            {
                Ports =
                {
                    new ServerPort(host, port, ServerCredentials.Insecure)
                }
            };

            Console.WriteLine($"Greeter server listening on host:{host} and port:{port}");

            server.Services.Add(
                Message.UserService.BindService(
                    new UserServiceImpl(ContainerHelper.Instance.Container.Resolve<IUserService>())).Intercept(tracingInterceptor));
            return server;
        }

        #region - Private -

        private static void PrintLogAndConsole(string msg)
        {
            Console.WriteLine(msg);
            LoggerHelper.Instance.Info(new InfoDetail
            {
                Message = msg, Target = "Program"
            });
        }

        private static void WaitingForTerminateServerInstance(Server serverInstance)
        {
            PrintLogAndConsole("Server is up. Input any key to shutdown...");

            Console.ReadKey();

            serverInstance.ShutdownAsync().Wait();
        }

        #endregion
    }
}