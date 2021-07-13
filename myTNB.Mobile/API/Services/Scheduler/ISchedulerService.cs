using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Scheduler.GetAvailableAppointment;
using myTNB.Mobile.API.Models.Scheduler.PostSetAppointment;
using Refit;

namespace myTNB.Mobile.API.Services.Scheduler
{
#if RELEASE
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiIwRTBDRDFFOS04MDZDLTREMkEtQUM0NC1BQ0FGRjZCNDdCMDgiLCJuYmYiOjE2MTIzNTI0MjIsImV4cCI6MTYxMjM1NjAyMiwiaWF0IjoxNjEyMzUyNDIyLCJpc3MiOiJteVROQiBBUEkiLCJhdWQiOiJteVROQiBBUEkgQXVkaWVuY2UifQ.eWIvm3kznjBFt84Q79wlylYUTCnCt4L1sjTCI2QjbJMaS_EfSQ96F1ilbYamSmMLYdcNCFz2NCyfWLZJ4ThJyg"
        , "Content-Type: application/json" })]
#else
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA"
        , "Content-Type: application/json" })]
#endif
    internal interface ISchedulerService
    {
        [Get("/{urlPrefix}/AvailableAppointment?businessArea={businessArea}")]
        Task<GetAvailableAppointmentResponse> GetAvailableAppointment([Header(MobileConstants.Header_UserInfo)] string userInfo
            , string businessArea
            , CancellationToken cancelToken
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath
            , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Post("/{urlPrefix}/SetAppointment")]
        Task<HttpResponseMessage> SetAppointment([Body] PostSetAppointmentRequest request
           , [Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancelToken
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);
    }
}