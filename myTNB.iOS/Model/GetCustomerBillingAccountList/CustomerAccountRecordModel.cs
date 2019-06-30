using System.Collections.Generic;
using myTNB.DataManager;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class CustomerAccountRecordModel
    {
        string _accountStAddress = string.Empty;
        string _accNum = string.Empty;
        string _accDesc = string.Empty;
        string _ownerName = string.Empty;
        string _isOwned = string.Empty;
        string _accountCategoryId = string.Empty;
        string _smartMeterCode = string.Empty;

        public string __type { set; get; }
        public string accNum
        {
            set
            {
                _accNum = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _accNum;
            }
        }
        public string userAccountID { set; get; }
        public string accDesc
        {
            set
            {
                _accDesc = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _accDesc;
            }
        }
        public string icNum { set; get; }
        public double amCurrentChg { set; get; }
        public string isRegistered { set; get; }
        public string isPaid { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
        public string isOwned
        {
            set
            {
                _isOwned = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _isOwned.ToLower();
            }
        }
        public bool isLocal { set; get; }
        public string accountTypeId { set; get; }
        public string accountStAddress
        {
            set
            {
                _accountStAddress = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _accountStAddress;
            }
        }
        public string accountNickName
        {
            set
            {
                string nickName = ServiceCall.ValidateResponseItem(value);
                if (!string.IsNullOrEmpty(nickName) && !string.IsNullOrWhiteSpace(nickName))
                {
                    _accDesc = nickName;
                }
            }
            get
            {
                return _accDesc;
            }
        }

        public string ownerName
        {
            set
            {
                _ownerName = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _ownerName;
            }
        }
        public string accountCategoryId
        {
            set
            {
                _accountCategoryId = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _accountCategoryId;
            }
        }

        public string accountOwnerName
        {
            set
            {
                _ownerName = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _ownerName;
            }
        }

        public string smartMeterCode
        {
            set
            {
                _smartMeterCode = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _smartMeterCode;
            }
        }

        [JsonIgnore]
        public bool IsNormalMeter
        {
            get
            {
                var res = true;

                if (!string.IsNullOrEmpty(smartMeterCode))
                {
                    res = string.Compare(smartMeterCode, "0") == 0;
                }
                return res;
            }
        }

        [JsonIgnore]
        public bool IsREAccount
        {
            get
            {
                return accountCategoryId != null && accountCategoryId.Equals("2");
            }
        }

        [JsonIgnore]
        /// <summary>
        /// IsOwned property as boolean
        /// </summary>
        public bool IsOwnedAccount
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(isOwned))
                {
                    res = string.Compare(isOwned, "true") == 0;
                }
                return res;
            }
        }

        //Multi Payment Properties
        public double Amount { set; get; }
        public double AmountDue { set; get; }
        public bool IsAccountSelected { set; get; }
        public List<ItemisedBilling> ItemizedBillings { set; get; }
        public double OpenChargesTotal { set; get; }
        public bool IsItemisedBilling
        {
            get
            {
                return OpenChargesTotal > 0;
            }
        }
    }
}