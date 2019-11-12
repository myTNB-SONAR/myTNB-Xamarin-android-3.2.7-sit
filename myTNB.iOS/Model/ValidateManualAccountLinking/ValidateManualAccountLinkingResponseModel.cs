namespace myTNB.Model
{
    public class ValidateManualAccountLinkingResponseModel
    {
        public ValidateManualAccountLinkingModel d { get; set; }
    }

    public class ValidateManualAccountLinkingModel : BaseModelV2
    {
        public ValidateManualAccountLinkingDataModel data { get; set; }
    }

    public class ValidateManualAccountLinkingDataModel
    {
        public string __type { set; get; }
        public string accNum { set; get; }
        public bool isLocal { set; get; }
        public string accountTypeId { set; get; }
        public string accountStAddress { set; get; }
        public string icNum { set; get; }
        public string isOwned { set; get; }
        public string accountNickName { set; get; }
        public string accountCategoryId { set; get; }
    }
}