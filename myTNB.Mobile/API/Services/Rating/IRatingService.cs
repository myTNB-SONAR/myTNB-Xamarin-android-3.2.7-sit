using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB.Mobile.API.Services.Rating
{
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiIwRTBDRDFFOS04MDZDLTREMkEtQUM0NC1BQ0FGRjZCNDdCMDgiLCJuYmYiOjE2MTIzNTI0MjIsImV4cCI6MTYxMjM1NjAyMiwiaWF0IjoxNjEyMzUyNDIyLCJpc3MiOiJteVROQiBBUEkiLCJhdWQiOiJteVROQiBBUEkgQXVkaWVuY2UifQ.eWIvm3kznjBFt84Q79wlylYUTCnCt4L1sjTCI2QjbJMaS_EfSQ96F1ilbYamSmMLYdcNCFz2NCyfWLZJ4ThJyg"
        , "Content-Type: application/json" })]
    internal interface IRatingService
    {
        [Get("/{urlPrefix}/CustomerRatingMaster?categoryID={categoryID}")]
        Task<GetCustomerRatingMasterResponse> GetCustomerRatingMaster([Header(MobileConstants.Header_UserInfo)] string userInfo
            , string categoryID
            , CancellationToken cancellationToken
            , string language
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath
            , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Post("/{urlPrefix}/SubmitRating")]
        Task<HttpResponseMessage> SubmitRating([Body] EncryptedRequest request
           , [Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancellationToken
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);
    }
}