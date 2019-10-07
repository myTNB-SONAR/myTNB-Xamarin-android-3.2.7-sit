namespace myTNB.Model
{
    public class DueAmountResponseModel
    {
        public DueAmountModel d { set; get; }
    }

    public class DueAmountModel : BaseModelRefresh
    {
        public DueAmountData data { set; get; }
    }

    public class DueAmountData
    {
        public DueAmountDataModel AccountAmountDue { set; get; }
    }
}