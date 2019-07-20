namespace myTNB.Model
{
    public class BaseModelV2
    {
        public string ErrorCode { set; get; }
        public string ErrorMessage { set; get; }
        public string DisplayMessage { set; get; }
        public string DisplayType { set; get; }
        public bool IsSuccess
        {
            get
            {
                return ErrorCode == "7002";
            }
        }
    }
}