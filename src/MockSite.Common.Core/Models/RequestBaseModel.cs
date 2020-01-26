#region
using System.Collections.Generic;

#endregion

namespace MockSite.Common.Core.Models
{
    public class RequestBaseModel<T>
    {
        public IReadOnlyDictionary<string, string> Carrier
        {
            get { return _carrier ?? (_carrier = new Dictionary<string, string>()); }
        }

        public T Data { get; }
        
        private IReadOnlyDictionary<string, string> _carrier;

        public RequestBaseModel(T data, Dictionary<string, string> carrier)
        {
            Data = data;
            _carrier = carrier;

        }

    }
}