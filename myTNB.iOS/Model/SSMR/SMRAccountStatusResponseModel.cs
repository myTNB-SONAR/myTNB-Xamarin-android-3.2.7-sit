using System.Collections.Generic;
using myTNB.DataManager;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class SMRAccountStatusResponseModel
    {
        public SMRAccountStatusDataModel d { set; get; }
    }

    public class SMRAccountStatusDataModel : BaseModelV2
    {
        public List<SMRAccountStatusModel> data { set; get; }
    }

    public class SMRAccountStatusModel
    {
        string _sTaggedSMR = string.Empty;
        string _isPeriodOpen = string.Empty;
        public string ContractAccount { set; get; }
        public string IsTaggedSMR
        {
            set
            {
                _sTaggedSMR = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _sTaggedSMR.ToLower();
            }
        }
        public string IsPeriodOpen
        {
            set
            {
                _isPeriodOpen = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _isPeriodOpen.ToLower();
            }
        }
        [JsonIgnore]
        public bool isTaggedSMR
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(IsTaggedSMR))
                {
                    res = string.Compare(IsTaggedSMR, "true") == 0;
                }
                return res;
            }
        }
        [JsonIgnore]
        public bool isPeriodOpen
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(IsPeriodOpen))
                {
                    res = string.Compare(IsPeriodOpen, "true") == 0;
                }
                return res;
            }
        }
    }
}
