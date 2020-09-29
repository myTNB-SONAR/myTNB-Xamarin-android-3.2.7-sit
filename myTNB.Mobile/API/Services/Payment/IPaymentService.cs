using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Payment.ApplicationPayment;
using Refit;

namespace myTNB.Mobile.API.Services.Payment
{
    [Headers(new string[] { "Content-Type: application/json" })]
    internal interface IPaymentService
    {
        [Post("/{urlPrefix}/ApplicationPayment")]
        Task<HttpResponseMessage> ApplicationPayment([Body] PostApplicationPaymentRequest request
           , CancellationToken cancelToken
           , string urlPrefix = Constants.ApiUrlPath);
    }
}