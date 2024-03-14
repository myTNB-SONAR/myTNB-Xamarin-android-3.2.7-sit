using System;
using myTNB.Android.Src.Login.Models;
using myTNB.Android.Src.XEmailRegistrationForm.Models;


namespace myTNB.Android.Src.MyTNBService.Response
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
