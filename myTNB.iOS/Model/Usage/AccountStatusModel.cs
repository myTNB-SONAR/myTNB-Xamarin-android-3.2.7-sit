namespace myTNB.Model.Usage
{
    public class AccountStatusResponseModel
    {
        public AccountStatusResponseDataModel d { set; get; }
    }

    public class AccountStatusResponseDataModel : BaseModelV2
    {
        public AccountStatusDataModel data { set; get; }
    }

    public class AccountStatusDataModel
    {
        public string RateCategory { set; get; }
        public string TextRateCategory { set; get; }
        public string MeterReadingUnit { set; get; }
        public string SingleCharacterIndicator { set; get; }
        public string RateFit { set; get; }
        public string RateFitCD { set; get; }
        public string RateInstalCAP { set; get; }
        public string RateKwhRec { set; get; }
        public string DisconnectionStatus { set; get; }
        public string RateAccDaa { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
        public string AccountStatusModalTitle { set; get; }
        public string AccountStatusModalMessage { set; get; }
        public string AccountStatusModalBtnText { set; get; }
        public string AccountStatusMessage { set; get; }
    }
}
