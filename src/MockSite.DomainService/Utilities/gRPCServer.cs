#region

using System.Collections.Generic;
using Grpc.Core;

#endregion

namespace MockSite.DomainService.Utilities
{
    public class GrpcServer
    {
        public GrpcServer(string host, int port, params ServerServiceDefinition[] serverServices)
        {
            var serverInstance = new Server(new List<ChannelOption>
                {
                    new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                    new ChannelOption("grpc.http2.max_pings_without_data", 0)
                }
            )
            {
                Ports =
                {
                    new ServerPort(host, port, ServerCredentials.Insecure)
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