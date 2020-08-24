using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public class GetAccountsChargesResponseModel
    {
        public GetAccountsChargesModel d { set; get; }
    }

    public class GetAccountsChargesModel : BaseModelV2
    {
        public AccountChargesDataModel data { set; get; }
        public bool IsPayEnabled { set; get; }
    }

    public class AccountChargesDataModel
    {
        public List<AccountChargesModel> AccountCharges { set; get; } = new List<AccountChargesModel>();
        public List<PopupModel> MandatoryChargesPopUpDetails { set; get; } = new List<PopupModel>();
    }

    public class AccountChargesModel
    {
        public string ContractAccount { set; get; } = string.Empty;
        public double CurrentCharges { set; get; } = 0;
        public double OutstandingCharges { set; get; } = 0;
        public double AmountDue { set; get; } = 0;
        public string DueDate { set; get; } = string.Empty;
        public string BillDate { set; get; } = string.Empty;
        public double IncrementREDueDateByDays { set; get; } = 0;
        public MandatoryChargesModel MandatoryCharges { set; get; } = new MandatoryChargesModel();
        public bool ShowEppToolTip { set; get; } //Created by Syahmi ICS 05052020
    }

    public class MandatoryChargesModel
    {
        public double TotalAmount { set; get; } = 0;
        public List<ChargesModel> Charges { set; get; } = new List<ChargesModel>();
    }

    public class ChargesModel
    {
        public string Key { set; get; } = string.Empty;
        public string Title { set; get; } = string.Empty;
        public double Amount { set; get; } = 0;
    }
}