using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MockSite.Common.Logging.Utilities.LogDetail;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog.Context;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MockSite.Common.Logging.Utilities
{
    internal class MockSiteLogger : ILogger
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private static readonly Dictionary<Type, Action<object>> _logObjActionMap =
            new Dictionary<Type, Action<object>>
            {
                {typeof(DebugDetail), detail => Debug(detail as DebugDetail)},
                {typeof(InfoDetail), detail => Information(detail as InfoDetail)},
                {typeof(ErrorDetail), detail => Error(detail as ErrorDetail)},
                {typeof(PerformanceDetail), detail => Performance(detail as PerformanceDetail)}
            };

        private static readonly Dictionary<LogLevel, Action<string, Exception>> _logActionMap =
            new Dictionary<LogLevel, Action<string, Exception>>
            {
                {LogLevel.Debug, (msg, ex) => { Debug(new DebugDetail {Message = msg}); }},
                {LogLevel.Information, (msg, ex) => { Information(new InfoDetail {Message = msg}); }},
                {LogLevel.Error, (msg, ex) => { Error(new ErrorDetail {Message = msg, StackTrace = ex?.ToString()}); }}
            };

        private readonly string _sender;
        private readonly LogLevel _enabledLogLevel;

        public MockSiteLogger(string sender, LogLevel enabledLogLevel)
        {
            _sender = sender;
            _enabledLogLevel = enabledLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);
            var formattedLogValues = state as IReadOnlyList<KeyValuePair<string, object>>;

            using (LogContext.PushProperty("Sender", _sender))
            {
                if (formattedLogValues?.Any() != null && formattedLogValues[0].Value is BaseDetail)
                {
                    Log(formattedLogValues[0].Value);
                }
                else
                {
                    Log(logLevel, message, exception);
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _enabledLogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private static void Log(object detail)
        {
            var type = detail.GetType();

            if (_logObjActionMap.ContainsKey(type))
            {
                _logObjActionMap[type](detail);
            }
            else
            {
                Error(new ErrorDetail
                    {Message = $"Unknown log level {JsonConvert.SerializeObject(detail, _jsonSerializerSettings)}"});
            }
        }

        private static void Log(LogLevel logLevel, string message, Exception exception)
        {
            if (string.IsNullOrEmpty(message) && exception == null) return;


            if (_logActionMap.ContainsKey(logLevel))
            {
                _logActionMap[logLevel](message, exception);
            }
            else
            {
                Error(new ErrorDetail {Message = $"Unknown log level {logLevel.ToString()}.\n{message}"});
            }
        }

        private static void Debug(DebugDetail detail)
        {
            Serilog.Log.Logger.Debug(StandardizeMessage(detail));
        }

        private static void Information(InfoDetail detail)
        {
            Serilog.Log.Logger.Information(StandardizeMessage(detail));
        }

        private static void Error(ErrorDetail detail)
        {
            Serilog.Log.Logger.Error(StandardizeMessage(detail));
        }

        private static void Performance(PerformanceDetail detail)
        {
            Serilog.Log.Logger.Debug(StandardizeMessage(detail, "[Perf]"));
        }

        private static string StandardizeMessage(BaseDetail detail, string prefix = "")
        {
            var message = $"{prefix}{detail.Message}";

            if (!string.IsNullOrWhiteSpace(detail.Arguments))
                message += $"\narguments:{detail.Arguments}";

            if (!string.IsNullOrWhiteSpace(detail.StackTrace))
                message += $"\nstackTrace:{detail.StackTrace}";

            if (detail.Duration > 0L)
                message += $"\nduration:{detail.Duration}";

            return message;
        }
    }
}