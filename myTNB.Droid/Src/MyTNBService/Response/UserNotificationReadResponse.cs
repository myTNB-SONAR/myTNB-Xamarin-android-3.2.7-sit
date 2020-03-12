using System;
using myTNB_Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
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
