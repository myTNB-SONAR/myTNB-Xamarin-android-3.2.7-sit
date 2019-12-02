using System;
using myTNB_Android.Src.MyTNBService.Model;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class CustomerAccountListResponse : BaseResponse<CustomerAccountData>
    {
        public CustomerAccountData GetData()
        {
            return Response.Data;
        }
    }
}
