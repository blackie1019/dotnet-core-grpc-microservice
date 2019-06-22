#region

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class InfoDetail : BaseDetail
    {
        public string Target { get; set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public string Message { get; set; }

        public IDictionary<string, string> Parameter { get; set; }

        public string Remark { get; set; }
    }
}