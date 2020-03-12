using System;
using myTNB_Android.Src.Login.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserAuthenticateResponse : BaseResponse<User>
    {
        public User GetData()
        {
            return Response.Data;
        }
    }
}
