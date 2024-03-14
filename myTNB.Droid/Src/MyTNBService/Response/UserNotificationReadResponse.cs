using System;
using myTNB.Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB.Android.Src.MyTNBService.Response
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
