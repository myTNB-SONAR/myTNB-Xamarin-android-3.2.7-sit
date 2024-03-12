using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB.Mobile.API.Services.Rating
{
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA"
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