#region

using System.Collections.Generic;
using MockSite.Common.Core.Enums;

#endregion

namespace MockSite.Common.Core.Models
{
    public class ResponseBaseModel<T>
    {
        public Dictionary<string, string> Carrier { get; set; }

        public ResponseCode Code { get; private set; }

        public T Data { get; private set; }

        public string Msg { get; private set; }

        public ResponseBaseModel()
        {
            Code = ResponseCode.GeneralError;
        }

        public void SetCode(ResponseCode code, string msg = null)
        {
            Code = code;
            Msg = msg ?? code.ToString();
        }

        public void SetData(T data)
        {
            Data = data;
            SetCode(ResponseCode.Success);
        }

        public void SetErrorMsg(string msg, ResponseCode code = ResponseCode.GeneralError)
        {
            SetCode(code, msg);
        }
    }
}