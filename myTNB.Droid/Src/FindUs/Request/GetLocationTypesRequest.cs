using Refit;

namespace myTNB.Android.Src.FindUs.Request
{
    public class GetLocationTypesRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }
    }
}