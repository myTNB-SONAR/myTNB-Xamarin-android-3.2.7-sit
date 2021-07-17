namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
    }
    public enum DBRTypeEnum
    {
        EBill,
        Email,
        Paper,
        WhatsApp,
        None
    }
}