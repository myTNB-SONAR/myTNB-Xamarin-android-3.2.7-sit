﻿using myTNB.DataManager;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class BaseModel
    {
        string _isError = string.Empty;
        string _status = string.Empty;

        public string __type { set; get; }
        public string status
        {
            set
            {
                _status = ServiceCall.ValidateResponseItem(value);
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
                _isError = ServiceCall.ValidateResponseItem(value);
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

        public string RefreshTitle { set; get; }
        public string RefreshMessage { set; get; }
        public string RefreshBtnText { set; get; }
    }
}