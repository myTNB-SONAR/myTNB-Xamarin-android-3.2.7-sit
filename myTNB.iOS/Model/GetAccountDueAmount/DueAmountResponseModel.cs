namespace myTNB.Model
{
    public class DueAmountResponseModel
    {
        public DueAmountModel d { set; get; }
    }

    public class DueAmountModel : BaseModelV2
    {
        public DueAmountData data { set; get; }
        public bool IsPayEnabled { set; get; }
    }

    public class DueAmountData
    {
        public DueAmountDataModel AccountAmountDue { set; get; }
    }
}