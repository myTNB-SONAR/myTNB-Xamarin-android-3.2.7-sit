using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB.Mobile.API.Services.Payment
{
    internal interface IPaymentService
    {
        [Headers(new string[] { "Content-Type: application/json" })]
        [Post("/{urlPrefix}/ApplicationPayment")]
        Task<HttpResponseMessage> ApplicationPayment([Body] EncryptedRequest request
            , CancellationToken cancellationToken
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath);

        [Headers(new string[] { "Content-Type: application/json" })]
        [Post("/{urlPrefix}/GetApplicationsPaidDetails")]
        Task<HttpResponseMessage> GetApplicationsPaidDetails([Body] EncryptedRequest request
            , CancellationToken cancellationToken
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath);

        [Headers(new string[] { "Content-Type: application/pdf" })]
        [Get("/{urlPrefix}/GetTaxInvoiceForApplicationPayment?srNumber={srNumber}")]
        Task<HttpResponseMessage> GetTaxInvoice(string srNumber
          , [Header(MobileConstants.Header_UserInfo)] string userInfo
          , CancellationToken cancellationToken
          , string language
          , [Header(MobileConstants.Header_Lang)] string lang
          , string urlPrefix = MobileConstants.ApiUrlPath
          , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);
    }
}