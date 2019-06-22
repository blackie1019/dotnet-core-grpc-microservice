#region

using System.Collections.Generic;

#endregion

namespace MockSite.Common.Core.Models
{
    public class RequestBaseModel<T>
    {
        public Dictionary<string, string> Carrier { get; set; }

        public T Data { get; set; }
    }
}