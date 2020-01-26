using System.Diagnostics.CodeAnalysis;

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class BaseDetail
    {
        public string Arguments { get; set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public string Message { get; set; }
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public string StackTrace { get; set; }
        public long Duration { get; set; }
    }
}