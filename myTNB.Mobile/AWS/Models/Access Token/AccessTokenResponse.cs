using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.AccessToken
{
    public class AccessTokenResponse : BaseResponse<AccessTokenModel>
    {

    }

    public class AccessTokenModel
    {
        [JsonProperty("accessToken")]
        public string AccessToken { set; get; }
    }
}
