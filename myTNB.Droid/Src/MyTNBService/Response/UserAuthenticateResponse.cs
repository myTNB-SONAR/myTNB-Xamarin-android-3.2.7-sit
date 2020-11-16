using System;
using myTNB_Android.Src.Login.Models;
using myTNB_Android.Src.XEmailRegistrationForm.Models;


namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserAuthenticateResponse : BaseResponse<User>
    {
        public User GetData()
        {
            return Response.Data;
        }

    }

    public class UserAuthenticateResponseEmail : BaseResponse<UserAll>
    {

        public UserAll GetDataAll()
        {
            return Response.Data;
        }
    }

    public class UserAuthenticateResponseID : BaseResponse<UserAll>
    {

        public UserAll GetDataAll()
        {
            return Response.Data;
        }
    }

    public class UserAuthenticateResponseName : BaseResponse<UserAll>
    {

        public UserAll GetDataAll()
        {
            return Response.Data;
        }
    }
}
