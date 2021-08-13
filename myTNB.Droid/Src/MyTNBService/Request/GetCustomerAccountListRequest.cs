using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetCustomerAccountListRequest : BaseRequest
    {
        public DateTime LastSyncDateTime;
        public GetCustomerAccountListRequest(DateTime LastSyncDateTime)
        {
            this.LastSyncDateTime = LastSyncDateTime;
        }
    }
}
