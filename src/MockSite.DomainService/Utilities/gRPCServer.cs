using System.Collections.Generic;
using Grpc.Core;

namespace MockSite.DomainService.Utilities
{
    public class gRPCServer
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        private readonly Grpc.Core.Server serverInstance;

        public gRPCServer(string host, int port, params ServerServiceDefinition[] serverServices)
        {
            Host = host;
            Port = port;
            serverInstance = new Grpc.Core.Server(new List<ChannelOption>
                {
                    new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                    new ChannelOption("grpc.http2.max_pings_without_data", 0)
                }
            )
            {
                Ports =
                {
                    new ServerPort(Host, Port, ServerCredentials.Insecure)
                }
            };
            foreach (var serverService in serverServices)
            {
                serverInstance.Services.Add(serverService);
            }

            serverInstance.Start();
        }
    }
}