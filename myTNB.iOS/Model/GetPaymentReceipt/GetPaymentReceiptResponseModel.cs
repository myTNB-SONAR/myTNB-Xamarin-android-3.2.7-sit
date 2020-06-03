using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public class GetPaymentReceiptResponseModel
    {
        public GetPaymentReceiptDataModel d { set; get; } = new GetPaymentReceiptDataModel();
    }
    public class GetPaymentReceiptDataModel : BaseModelV2
    {
        public ReceiptDataModel data { set; get; } = new ReceiptDataModel();
    }

    public class ReceiptDataModel
    {
        private string _referenceNum = string.Empty;
        private string _customerName = string.Empty;
        private string _customerEmail = string.Empty;
        private string _customerPhone = string.Empty;
        private string _payMethod = string.Empty;
        private string _payTransID = string.Empty;
        private string _paySellerNum = string.Empty;
        private string _payTransStatus = string.Empty;
        private string _payTransDate = string.Empty;
        private string _merchantID = string.Empty;
        private string _payAmt = string.Empty;

        public string referenceNum
        {
            set
            {
                if (value.IsValid())
                {
                    _referenceNum = value;
                }
            }
            get { return _referenceNum; }
        }
        public List<MultiPayDataModel> accMultiPay { set; get; } = new List<MultiPayDataModel>();
        public string customerName
        {
            set
            {
                if (value.IsValid())
                {
                    _customerName = value;
                }
            }
            get { return _customerName; }
        }
        public string customerEmail
        {
            set
            {
                if (value.IsValid())
                {
                    _customerEmail = value;
                }
            }
            get { return _customerEmail; }
        }
        public string customerPhone
        {
            set
            {
                if (value.IsValid())
                {
                    _customerPhone = value;
                }
            }
            get { return _customerPhone; }
        }
        public string payMethod
        {
            set
            {
                if (value.IsValid())
                {
                    _payMethod = value;
                }
            }
            get { return _payMethod; }
        }
        public string payTransID
        {
            set
            {
                if (value.IsValid())
                {
                    _payTransID = value;
                }
            }
            get { return _payTransID; }
        }
        public string paySellerNum
        {
            set
            {
                if (value.IsValid())
                {
                    _paySellerNum = value;
                }
            }
            get { return _paySellerNum; }
        }
        public string payTransStatus
        {
            set
            {
                if (value.IsValid())
                {
                    _payTransStatus = value;
                }
            }
            get { return _payTransStatus; }
        }
        public string payTransDate
        {
            set
            {
                if (value.IsValid())
                {
                    _payTransDate = value;
                }
            }
            get { return _payTransDate; }
        }
        public string merchantID
        {
            set
            {
                if (value.IsValid())
                {
                    _merchantID = value;
                }
            }
            get { return _merchantID; }
        }
        public string payAmt
        {
            set
            {
                if (value.IsValid())
                {
                    _payAmt = value;
                }
            }
            get { return _payAmt; }
        }
    }

    public class MultiPayDataModel
    {
        private string _accountOwnerName = string.Empty;
        private string _accountNum = string.Empty;
        private string _itmAmt = string.Empty;

        public string AccountOwnerName
        {
            set
            {
                if (value.IsValid())
                {
                    _accountOwnerName = value;
                }
            }
            get { return _accountOwnerName; }
        }
        public string accountNum
        {
            set
            {
                if (value.IsValid())
                {
                    _accountNum = value;
                }
            }
            get { return _accountNum; }
        }
        public string itmAmt
        {
            set
            {
                if (value.IsValid())
                {
                    _itmAmt = value;
                }
            }
            get { return _itmAmt; }
        }
    }
}