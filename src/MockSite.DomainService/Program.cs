using System;
using Grpc.Core;
using Infrastructure;
using Infrastructure.Helpers;
using MockSite.Core.Repositories;
using MockSite.Core.Services;
using System.Threading;

namespace MockSite.DomainService
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = AppSettingsHelper.Instance.GetValueFromKey(MessageConst.TestHostNameKey);
            var port = Convert.ToInt32(AppSettingsHelper.Instance.GetValueFromKey(MessageConst.TestPortKey));

            var server = new Server
            {
                Ports =
                {
                    new ServerPort(host, port, ServerCredentials.Insecure)
                }
            };

            server.Services.Add(
                Message.UserService.BindService(
                    new UserServiceImpl(
                        new UserService(
                            new UserRepository()))));

            Console.WriteLine($"Greeter server listening on host:{host} and port:{port}");

            server.Start();

            Console.WriteLine("Press any key to stop the server...");
            // Console.ReadKey();
            Thread.Sleep(Timeout.Infinite);

            server.ShutdownAsync().Wait();
        }
    }
}