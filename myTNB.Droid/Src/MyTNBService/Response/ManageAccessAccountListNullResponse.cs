using System.Collections.Generic;
using myTNB_Android.Src.ManageUser.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class ManageAccessAccountListNullResponse : BaseResponse<CustomerAcc>
    {
        public CustomerAcc GetData()
        {
            return Response.Data;
        }      
    }
}
