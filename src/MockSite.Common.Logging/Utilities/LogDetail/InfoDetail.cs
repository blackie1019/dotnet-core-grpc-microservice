using System.Collections.Generic;

namespace MockSite.Common.Logging.Utilities.LogDetail
{
    public class InfoDetail
    {
        public string Target { get; set; }
        public string Message { get; set; }
        
        public Dictionary<string, string> Parameter { get; set; }
        
        public string Remark { get; set; }
        
    }
}