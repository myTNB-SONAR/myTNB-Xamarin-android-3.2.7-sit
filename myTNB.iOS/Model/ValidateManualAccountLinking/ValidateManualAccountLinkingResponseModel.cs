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
        public string __type { set; get; } = string.Empty;
        public string accNum { set; get; } = string.Empty;
        public bool isLocal { set; get; }
        public string accountTypeId { set; get; } = string.Empty;
        public string accountStAddress { set; get; } = string.Empty;
        public string icNum { set; get; } = string.Empty;
        public string isOwned { set; get; } = string.Empty;
        public string accountNickName { set; get; } = string.Empty;
        public string accountCategoryId { set; get; } = string.Empty;
    }
}