#region

using System.Collections.Generic;

#endregion

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class InfoDetail : BaseDetail
    {
        public string Target { get; set; }

        public IDictionary<string, string> Parameter { get; set; }

        public string Remark { get; set; }
    }
}