using Refit;

namespace myTNB.AndroidApp.Src.FindUs.Request
{
    public class GetLocationTypesRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }
    }
}