#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSite.Common.Logging;
using MockSite.DomainService.Models;

#endregion

namespace MockSite.DomainService.Utilities
{
    public class GrpcServer : BackgroundService
    {
        private readonly Server _serverInstance;
        private readonly ILogger<GrpcServer> _logger;

        public GrpcServer(GrpcServerConfig serverConfig, ILogger<GrpcServer> logger)
        {
            _serverInstance = new Server(new List<ChannelOption>
                {
                    new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                    new ChannelOption("grpc.http2.max_pings_without_data", 0),
                    new ChannelOption("grpc.max_receive_message_length", -1),
                    new ChannelOption("grpc.max_send_message_length", -1)
                }
            )
            {
                Ports =
                {
                    new ServerPort(serverConfig.Host, serverConfig.Port, ServerCredentials.Insecure)
                }
            };
            foreach (var serverService in serverConfig.ServiceDefinitions)
            {
                _serverInstance.Services.Add(serverService);
            }

            _logger = logger;
            
            _logger.Info($"Initialize gRPC Server on  host:{serverConfig.Host} and port:{serverConfig.Port} ...");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() => { 
                try
                {
                    _serverInstance.Start();
                
                    _logger.Info($"gRPC Server ready to listening on {_serverInstance.Ports.Select(x => x.Host).FirstOrDefault()}:{_serverInstance.Ports.Select(x => x.Port).FirstOrDefault()}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                
            }, stoppingToken);
        }
    }
}