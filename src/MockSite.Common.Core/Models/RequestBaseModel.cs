using System.Collections.Generic;

namespace MockSite.Common.Core.Models
{
    public class RequestBaseModel<T>
    {
        public RequestBaseModel()
        {

        }

        public Dictionary<string, string> Carrier { get; set; }

        public T Data { get; set; }
    }
}