#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class ErrorDetail : InfoDetail
    {
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public string StackTrace { get; set; }
    }
}