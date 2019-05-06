using System.Collections.Generic;
using MockSite.Common.Core.Enums;

namespace MockSite.Common.Core.Models
{
    public class ResponseBaseModel<T>
    {
        public ResponseBaseModel()
        {
            _code = ResponseCode.GeneralError;
        }

        public Dictionary<string, string> Carrier { get; set; }

        public T Data => _data;

        public string Msg => _msg;

        public ResponseCode Code => _code;

        private T _data { get; set; }

        private string _msg { get; set; }

        private ResponseCode _code { get; set; }

        public void SetData(T data)
        {
            _data = data;
            SetCode(ResponseCode.Success);
        }

        public void SetErrorMsg(string msg, ResponseCode code = ResponseCode.GeneralError)
        {
            SetCode(code, msg);
        }

        public void SetCode(ResponseCode code, string msg = null)
        {
            _code = code;

            _msg = msg ?? code.ToString();
        }
    }
}