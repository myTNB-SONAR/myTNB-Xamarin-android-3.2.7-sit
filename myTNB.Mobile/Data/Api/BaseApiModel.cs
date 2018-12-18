using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.Model
{
    public class BaseApiModel<T>
    {
        string _isError = string.Empty;
        string _status = string.Empty;

        public string __type { set; get; }
        public string status
        {
            set
            {
                _status = ApiHelper.ValidateResponseItem(value);
            }
            get
            {
                return _status.ToLower();
            }
        }
        public string isError
        {
            set
            {
                _isError = ApiHelper.ValidateResponseItem(value);
            }
            get
            {
                return _isError.ToLower();
            }
        }

        public string message { set; get; }
        public string StatusCode { set; get; }

        [JsonIgnore]
        public bool didSucceed
        {
            get
            {
                if (!string.IsNullOrEmpty(isError))
                {
                    return !bool.Parse(isError);
                }
                return false;
            }
        }

        public T data { set; get; }
    }
}
