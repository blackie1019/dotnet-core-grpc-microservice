using MockSite.Common.Logging.Utilities.LogDetail;

namespace MockSite.Common.Logging.Utilities
{
    public interface ILoggerService
    {
        void Debug(DebugDetail detail);

        void Info(InfoDetail detail);

        void Error(ErrorDetail detail);

        void Performance(PerformanceDetail detail);
    }
}