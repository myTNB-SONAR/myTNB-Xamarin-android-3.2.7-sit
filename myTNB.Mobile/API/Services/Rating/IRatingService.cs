using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB.Mobile.API.Models.Rating.PostSubmitRating;
using Refit;

namespace myTNB.Mobile.API.Services.Rating
{
#if RELEASE
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiIwRTBDRDFFOS04MDZDLTREMkEtQUM0NC1BQ0FGRjZCNDdCMDgiLCJuYmYiOjE2MTIzNTI0MjIsImV4cCI6MTYxMjM1NjAyMiwiaWF0IjoxNjEyMzUyNDIyLCJpc3MiOiJteVROQiBBUEkiLCJhdWQiOiJteVROQiBBUEkgQXVkaWVuY2UifQ.eWIvm3kznjBFt84Q79wlylYUTCnCt4L1sjTCI2QjbJMaS_EfSQ96F1ilbYamSmMLYdcNCFz2NCyfWLZJ4ThJyg"
        , "Content-Type: application/json" })]
#else
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA"
        , "Content-Type: application/json" })]
#endif
    internal interface IRatingService
    {
        [Get("/{urlPrefix}/CustomerRatingMaster?categoryID={categoryID}")]
        Task<GetCustomerRatingMasterResponse> GetCustomerRatingMaster([Header(Constants.Header_UserInfo)] string userInfo
            , string categoryID
            , CancellationToken cancelToken
            , string language
            , [Header(Constants.Header_Lang)] string lang
            , string urlPrefix = Constants.ApiUrlPath
            , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Post("/{urlPrefix}/SubmitRating")]
        Task<HttpResponseMessage> SubmitRating([Body] PostSubmitRatingRequest request
           , [Header(Constants.Header_UserInfo)] string userInfo
           , CancellationToken cancelToken
           , [Header(Constants.Header_Lang)] string lang
           , string urlPrefix = Constants.ApiUrlPath
           , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);
    }
}