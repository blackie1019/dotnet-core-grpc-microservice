#region

using System.Collections.Generic;
using MockSite.Common.Core.Enums;

#endregion

namespace MockSite.Common.Core.Models
{
    public class ResponseBaseModel<T>
    {
        public IReadOnlyDictionary<string, string> Carrier
        {
            get { return _carrier ?? (_carrier = new Dictionary<string, string>()); }
        }

        private IReadOnlyDictionary<string, string> _carrier;

        public ResponseCode Code { get; }

        public T Data { get; }

        public string Message { get; }
        
        public ResponseBaseModel(ResponseCode responseCode, T data,string message = "", Dictionary<string,string> carrier = null)
        {
            Code = responseCode;
            Data = data;
            Message = message;
            _carrier = carrier;
        }
    }
}