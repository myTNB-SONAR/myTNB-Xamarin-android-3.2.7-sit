using System;
using myTNB.AndroidApp.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class UserNotificationReadResponse : BaseResponse<UserNotificationReadResponse.APIResponse>
    {
		public APIResponse GetData()
        {
            return Response.Data;
        }

		public class APIResponse
		{
            //No Impl
		}
	}
}
