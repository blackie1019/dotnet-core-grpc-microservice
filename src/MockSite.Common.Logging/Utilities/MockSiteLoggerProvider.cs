#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Destructurama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using MockSite.Common.Core.Utilities;
using Serilog.Core;

#endregion

namespace MockSite.Common.Logging.Utilities
{
    internal  sealed  class MockSiteLoggerProvider : ILoggerProvider
    {
        private bool _disposed;
        private readonly ConcurrentDictionary<string, MockSiteLogger> _loggers = new ConcurrentDictionary<string, MockSiteLogger>();
        private readonly Dictionary<string, string> _logLevels;

        public MockSiteLoggerProvider(IConfiguration configuration, IConfiguration loggingSection,
            bool consoleOutput = false)
        {
            var loggerCreator = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Destructure.JsonNetTypes();

            _logLevels = loggingSection.GetSection("LogLevel").GetChildren()
                .OrderByDescending(x => x.Key.Length)
                .ToDictionary(x => x.Key, x => x.Value);
            if (consoleOutput) loggerCreator.WriteTo.Console();

            Log.Logger = loggerCreator.CreateLogger();
        }

        ~MockSiteLoggerProvider()
        {
            Dispose(false);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, key =>
            {
                var enabledLogLevel = GetEnabledLogLevel(categoryName);
                return new MockSiteLogger(categoryName, enabledLogLevel);
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed) return;

            if (disposing)
            {
                (Log.Logger as Logger).Dispose();
                Log.Logger = null;
            }

            _disposed = true;
        }

        private LogLevel GetEnabledLogLevel(string categoryName)
        {
            var enabledLogLevel = _logLevels.FirstOrDefault(x => categoryName.StartsWith(x.Key)).Value;
            if (string.IsNullOrEmpty(enabledLogLevel))
            {
                enabledLogLevel = _logLevels.FirstOrDefault(x => x.Key.EqualsIgnoreCase("Default")).Value ?? "Trace";
            }

            return (LogLevel) Enum.Parse(typeof(LogLevel), enabledLogLevel, true);
        }

    }
}