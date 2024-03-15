using Refit;

namespace myTNB.AndroidApp.Src.Base.Models
{
    public class BaseRequest
    {

        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        public BaseRequest(string apiKeyID)
        {
            this.apiKeyID = apiKeyID;
        }
    }
}