using System;
using myTNB_Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserNotificationDeleteResponse : BaseResponse<UserNotificationDeleteResponse.APIResponse>
    {
		public APIResponse GetData()
        {
            return Response.Data;
        }

		public class APIResponse
		{
			
		}
	}
}
