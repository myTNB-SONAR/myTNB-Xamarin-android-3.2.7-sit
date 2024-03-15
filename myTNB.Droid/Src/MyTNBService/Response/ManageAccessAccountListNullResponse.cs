using System.Collections.Generic;
using myTNB.AndroidApp.Src.ManageUser.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class ManageAccessAccountListNullResponse : BaseResponse<CustomerAcc>
    {
        public CustomerAcc GetData()
        {
            return Response.Data;
        }      
    }
}
