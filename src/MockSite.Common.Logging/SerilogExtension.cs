#region

using System;
using MockSite.Common.Logging.Enums;
using MockSite.Common.Logging.Utilities.LogDetail;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

#endregion

//using Serilog;

namespace MockSite.Common.Logging
{
    public static class SerilogExtension
    {
        public static void Debug(this ILogger logger, string message)
        {
            Log.Logger.Debug("[Debug]{@detail}", new DebugDetail {Message = message});
        }

        public static void Debug(this ILogger logger, DebugDetail detail)
        {
            Log.Logger.Debug("[Debug]{@detail}", detail);
        }

        public static void Info(this ILogger logger, string message)
        {
            Log.Logger.Information("[Info]{@detail}", new InfoDetail {Message = message});
        }

        public static void Info(this ILogger logger, InfoDetail detail)
        {
            Log.Logger.Information("[Info]{@detail}", detail);
        }

        #region Error

        public static void Error(this ILogger logger, string message)
        {
            Log.Logger.Error("[Exception]{@detail}", new ErrorDetail {Message = message});
        }

        public static void Error(this ILogger logger, ErrorDetail detail)
        {
            Log.Logger.Error("[Exception]{@detail}", detail);
        }

        public static void Error(this ILogger logger, Exception exception)
        {
            Log.Logger.Error("[Exception]{@detail}", new ErrorDetail {StackTrace = exception.ToString()});
        }

        public static void Error(this ILogger logger, string message, Exception exception)
        {
            Log.Logger.Error("[Exception]{@detail}",
                new ErrorDetail {Message = message, StackTrace = exception.ToString()});
        }

        #endregion

        #region Performance

        public static void Performance(this ILogger logger, PerformanceDetail detail)
        {
            if (detail.Duration > 0 && detail.Rank == DurationRank.Unknown)
            {
                if (detail.Duration > 0 && detail.Duration < 3 * 1000) // < 3 Seconds
                {
                    detail.Rank = DurationRank.Normal;
                }
                else if (detail.Duration >= 3000 && detail.Duration <= 10000) // 3~10 Seconds
                {
                    detail.Rank = DurationRank.Slow;
                }
                else if (detail.Duration > 10000 && detail.Duration <= 30000) // 10~30 Seconds
                {
                    detail.Rank = DurationRank.DamnSlow; // > 30 Seconds
                }
                else if (detail.Duration > 30000)
                {
                    detail.Rank = DurationRank.FxxxSlow;
                }
            }

            Log.Logger.Warning("[Perf]{@detail}", detail);
        }

        #endregion
    }
}