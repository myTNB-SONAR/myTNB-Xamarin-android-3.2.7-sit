using System;
using myTNB.AndroidApp.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
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
