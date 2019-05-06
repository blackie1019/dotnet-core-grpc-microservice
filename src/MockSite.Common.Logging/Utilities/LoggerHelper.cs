using System;
using MockSite.Common.Logging.Enums;
using MockSite.Common.Logging.Utilities.LogDetail;

namespace MockSite.Common.Logging.Utilities
{
    public sealed class LoggerHelper
    {
        private static readonly Lazy<LoggerHelper> Lazy =
            new Lazy<LoggerHelper>(() => new LoggerHelper());

        public static LoggerHelper Instance => Lazy.Value;

        private ILoggerService _service;

        private LoggerHelper()
        {

        }

        public void SetLogProvider(ILoggerService service)
        {
            _service = service;
        }

        #region Debug

        public void Debug(string message)
        {
            if (_service != null)
            {
                var detail = GetDebugDetail(message);
                _service.Debug(detail);
            }
        }

        #endregion

        #region Info

        public void Info(string message)
        {
            if (_service != null)
            {
                var detail = GetInfoDetail(message);
                _service.Info(detail);
            }
        }

        public void Info(InfoDetail detail)
        {
            if (_service != null)
            {
                _service.Info(detail);
            }
        }

        #endregion

        #region Error

        public void Error(string message)
        {
            if (_service != null)
            {
                var detail = GetExceptionDetail(message);
                _service.Error(detail);
            }
        }

        public void Error(ErrorDetail detail)
        {
            if (_service != null)
            {
                _service.Error(detail);
            }
        }

        public void Error(Exception exception)
        {
            if (_service != null)
            {
                var detail = GetExceptionDetail(exception);
                _service.Error(detail);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (_service != null)
            {
                var detail = GetExceptionDetail(message, exception);
                _service.Error(detail);
            }
        }

        #endregion

        #region Performance

        public void Performance(PerformanceDetail detail)
        {
            if (_service != null)
            {
                if (detail.Duration > 0 && detail.Rank == DurationRank.Unknow)
                {
                    if (detail.Duration > 0 && detail.Duration < 3*1000) // < 3 Seconds
                    {
                        detail.Rank = DurationRank.Normal;
                    }
                    else if (detail.Duration >= 3000 && detail.Duration <= 10000) // 3~10 Seconds
                    {
                        detail.Rank = DurationRank.Slow;
                    }else if (detail.Duration > 10000 && detail.Duration <= 30000) // 10~30 Seconds
                    {
                        detail.Rank = DurationRank.DamnSlow; // > 30 Seconds
                    }else if (detail.Duration > 30000)
                    {
                        detail.Rank = DurationRank.FXXXSlow;
                    }
                }
                _service.Performance(detail);
            }
        }

        #endregion

        public DebugDetail GetDebugDetail(string message)
        {
            return new DebugDetail {Message = message};
        }

        public InfoDetail GetInfoDetail(string message)
        {
            return new InfoDetail {Message = message};
        }

        public ErrorDetail GetExceptionDetail(string message)
        {
            return new ErrorDetail {Message = message};
        }

        public ErrorDetail GetExceptionDetail(Exception exception)
        {
            return new ErrorDetail {StackTrace = exception.ToString()};
        }

        public ErrorDetail GetExceptionDetail(string message, Exception exception)
        {
            return new ErrorDetail {Message = message, StackTrace = exception.ToString()};
        }

        public PerformanceDetail GetPerformanceDetail(string message)
        {
            return new PerformanceDetail {Message = message};
        }

        public PerformanceDetail GetPerformanceDetail()
        {
            return new PerformanceDetail();
        }
    }
}