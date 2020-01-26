#region

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MockSite.Common.Logging.Utilities.LogDetail;
using MockSite.Common.Logging;

#endregion

namespace MockSite.DomainService.Utilities
{
    public static class MethodTimeLogger
    {
        private static ILogger _logger;

        public static void Log(MethodBase methodBase, long milliseconds, string message)
        {
            try
            {
                var detail = new PerformanceDetail
                {
                    Target = $"{methodBase.DeclaringType.Name}/{methodBase.Name}", Duration = milliseconds
                };
                if (!string.IsNullOrWhiteSpace(message)) detail.Arguments = message;

                _logger.Performance( detail);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        
        public static void SetLogger(ILoggerProvider loggerProvider)
        {
            _logger = _logger ?? loggerProvider.CreateLogger(nameof(MethodTimeLogger));
        }

    }
}