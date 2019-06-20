using Refit;

namespace myTNB_Android.Src.FindUs.Request
{
    public class GetLocationTypesRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }
    }
}