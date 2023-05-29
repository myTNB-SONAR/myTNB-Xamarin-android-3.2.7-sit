using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
	public class PostDeleteCOTDraftResponse : BaseResponse<PostDeleteCOTDraftModel>
    {
		
	}
    public class PostDeleteCOTDraftModel
    {
        [JsonProperty("result")]
        public bool Result { set; get; }
    }
}

