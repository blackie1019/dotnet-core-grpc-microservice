#region

using System;
using Microsoft.Extensions.Logging;
using MockSite.Common.Logging.Utilities.LogDetail;
using ILogger = Microsoft.Extensions.Logging.ILogger;

#endregion

//using Serilog;

namespace MockSite.Common.Logging
{
    public static class SerilogExtension
    {
        public static void Debug(this ILogger logger, string message)
        {
            logger.LogDebug("[Debug]{@detail}", new DebugDetail {Message = message});
        }

        public static void Debug(this ILogger logger, DebugDetail detail)
        {
            logger.LogDebug("[Debug]{@detail}", detail);
        }

        public static void Info(this ILogger logger, string message)
        {
            logger.LogInformation("[Info]{@detail}", new InfoDetail {Message = message});
        }

        public static void Info(this ILogger logger, InfoDetail detail)
        {
            logger.LogInformation("[Info]{@detail}", detail);
        }

        #region Performance

        public static void Performance(this ILogger logger, PerformanceDetail detail)
        {
            logger.LogWarning("[Perf]{@detail}", detail);
        }

        public static void Warning(this ILogger logger, PerformanceDetail detail)
        {
            logger.LogWarning("[Perf]{@detail}", detail);
        }

        #endregion

        #region Error

        public static void Error(this ILogger logger, string message)
        {
            logger.LogError("[Exception]{@detail}", new ErrorDetail {Message = message});
        }

        public static void Error(this ILogger logger, ErrorDetail detail)
        {
            logger.LogError("[Exception]{@detail}", detail);
        }

        public static void Error(this ILogger logger, Exception exception)
        {
            logger.LogError("[Exception]{@detail}", new ErrorDetail {StackTrace = exception.ToString()});
        }

        public static void Error(this ILogger logger, string message, Exception exception)
        {
            logger.LogError("[Exception]{@detail}",
                new ErrorDetail {Message = message, StackTrace = exception.ToString()});
        }

        #endregion
    }
}