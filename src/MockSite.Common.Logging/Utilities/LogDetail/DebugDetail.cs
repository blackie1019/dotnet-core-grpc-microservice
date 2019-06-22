#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class DebugDetail : BaseDetail
    {
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public string Message { get; set; }
    }
}