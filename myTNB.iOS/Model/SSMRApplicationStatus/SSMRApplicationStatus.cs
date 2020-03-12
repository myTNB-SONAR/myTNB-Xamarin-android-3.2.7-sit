namespace myTNB.Model
{
    public class SSMRApplicationStatusResponseModel
    {
        public SSMRApplicationStatusDataModel d { set; get; }
    }

    public class SSMRApplicationStatusDataModel : BaseModelV2
    {
        public SSMRApplicationStatusModel data { set; get; }
    }

    public class SSMRApplicationStatusModel
    {
        public string Status { set; get; }
        public string ServiceReqNo { set; get; }
        public string ApplicationID { set; get; }
        public string AppliedOn { set; get; }
    }
}