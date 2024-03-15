using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class Data
    {
        public string ClaimId { get; set; }
    }
    public class D
    {
        public Data data { get; set; }
        public string status { get; set; }
        public string isError { get; set; }
        public string message { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string DisplayMessage { get; set; }
        public string DisplayType { get; set; }
        public string DisplayTitle { get; set; }
        public string RefreshTitle { get; set; }
        public string RefreshMessage { get; set; }
        public string RefreshBtnText { get; set; }
        public bool IsPayEnabled { get; set; }
    }

    public class GetOvervoltageClaimDetailModel
    {
        public D d { get; set; }
    }
}
