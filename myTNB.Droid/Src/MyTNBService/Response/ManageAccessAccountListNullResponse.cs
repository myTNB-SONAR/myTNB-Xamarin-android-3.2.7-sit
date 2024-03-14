using System.Collections.Generic;
using myTNB.Android.Src.ManageUser.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class ManageAccessAccountListNullResponse : BaseResponse<CustomerAcc>
    {
        public CustomerAcc GetData()
        {
            return Response.Data;
        }      
    }
}
