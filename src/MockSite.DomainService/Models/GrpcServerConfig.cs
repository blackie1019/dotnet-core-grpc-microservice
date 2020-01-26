using Grpc.Core;

namespace MockSite.DomainService.Models
{
    public class GrpcServerConfig
    {
        public GrpcServerConfig(string host, int port, params ServerServiceDefinition[] serviceDefinitions)
        {
            Host = host;
            Port = port;
            ServiceDefinitions = serviceDefinitions;
        }
        
        public string Host { get; private set; }

        public int Port { get;private set; }

        public ServerServiceDefinition[] ServiceDefinitions { get; private set; }
    }
}