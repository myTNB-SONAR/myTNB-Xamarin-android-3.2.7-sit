using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
	public class PostDeleteCOADraftResponse : BaseResponse<PostDeleteCOADraftModel>
    {

    }
    public class PostDeleteCOADraftModel
    {
        [JsonProperty("result")]
        public bool Result { set; get; }
    }
}

