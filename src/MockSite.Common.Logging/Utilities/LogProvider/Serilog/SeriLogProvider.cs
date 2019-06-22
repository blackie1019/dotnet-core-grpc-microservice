#region

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MockSite.Common.Logging.Utilities.LogDetail;
using Serilog;

#endregion

namespace MockSite.Common.Logging.Utilities.LogProvider.Serilog
{
    public class SerilogProvider : ILoggerService
    {
        private const string DefaultLoadFile = "serilogsettings.json";

        private readonly ILogger _log;

        private static readonly Lazy<SerilogProvider> _lazy =
            new Lazy<SerilogProvider>(() => new SerilogProvider());

        public static SerilogProvider Instance => _lazy.Value;

        private SerilogProvider()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultLoadFile);
            if (File.Exists(filePath))
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(DefaultLoadFile)
                    .Build();

                _log = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
            }
            else
            {
                throw new FileNotFoundException(
                    "initialize Serilog Provider fail, Can not find serilogsettings.json");
            }
        }

        public void Debug(DebugDetail detail)
        {
            _log.Debug("[Debug]{@detail}", detail);
        }

        public void Info(InfoDetail detail)
        {
            _log.Information("[Info]{@detail}", detail);
        }

        public void Error(ErrorDetail detail)
        {
            _log.Error("[Exception]{@detail}", detail);
        }

        public void Performance(PerformanceDetail detail)
        {
            _log.Warning("[Perf]{@detail}", detail);
        }
    }
}