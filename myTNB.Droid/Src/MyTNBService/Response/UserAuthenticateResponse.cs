using System;
using myTNB.AndroidApp.Src.Login.Models;
using myTNB.AndroidApp.Src.XEmailRegistrationForm.Models;


namespace myTNB.AndroidApp.Src.MyTNBService.Response
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
