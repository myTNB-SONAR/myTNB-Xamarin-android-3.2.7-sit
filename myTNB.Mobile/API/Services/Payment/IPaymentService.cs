using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Payment.PostApplicationPayment;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using Refit;

namespace myTNB.Mobile.API.Services.Payment
{
    internal interface IPaymentService
    {
        [Headers(new string[] { "Content-Type: application/json" })]
        [Post("/{urlPrefix}/ApplicationPayment")]
        Task<HttpResponseMessage> ApplicationPayment([Body] PostApplicationPaymentRequest request
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Headers(new string[] { "Content-Type: application/json" })]
        [Post("/{urlPrefix}/GetApplicationsPaidDetails")]
        Task<HttpResponseMessage> GetApplicationsPaidDetails([Body] PostApplicationsPaidDetailsRequest request
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Headers(new string[] { "Content-Type: application/pdf" })]
        [Get("/{urlPrefix}/GetTaxInvoiceForApplicationPayment?srNumber={srNumber}")]
        Task<HttpResponseMessage> GetTaxInvoice(string srNumber
          , [Header(Constants.Header_UserInfo)] string userInfo
          , CancellationToken cancelToken
          , string language
          , string urlPrefix = Constants.ApiUrlPath
          , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);
    }
}