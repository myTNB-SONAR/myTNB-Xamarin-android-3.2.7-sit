using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.ApplicationStatus.PostRemoveApplication;
using myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication;
using Refit;

namespace myTNB.Mobile.API.Services.ApplicationStatus
{
#if RELEASE
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiIwRTBDRDFFOS04MDZDLTREMkEtQUM0NC1BQ0FGRjZCNDdCMDgiLCJuYmYiOjE2MTIzNTI0MjIsImV4cCI6MTYxMjM1NjAyMiwiaWF0IjoxNjEyMzUyNDIyLCJpc3MiOiJteVROQiBBUEkiLCJhdWQiOiJteVROQiBBUEkgQXVkaWVuY2UifQ.eWIvm3kznjBFt84Q79wlylYUTCnCt4L1sjTCI2QjbJMaS_EfSQ96F1ilbYamSmMLYdcNCFz2NCyfWLZJ4ThJyg"
        , "Content-Type: application/json" })]
#else
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA"
        , "Content-Type: application/json" })]
#endif
    internal interface IApplicationStatusService
    {
        [Get("/{urlPrefix}/SearchApplicationType?lang={language}")]
        Task<SearchApplicationTypeResponse> SearchApplicationType([Header(MobileConstants.Header_UserInfo)] string userInfo
            , CancellationToken cancellationToken
            , string language
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath
            , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Get("/{urlPrefix}/SearchApplicationType?lang={language}")]
        Task<SearchApplicationTypeResponse> SearchApplicationType(CancellationToken cancellationToken
            , string language
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath
            , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Get("/{urlPrefix}/ApplicationStatus?lang={language}&applicationType={applicationType}&searchType={searchType}&searchTerm={searchTerm}")]
        Task<HttpResponseMessage> GetApplicationStatus(string applicationType
            , string searchType
            , string searchTerm
            , [Header(MobileConstants.Header_UserInfo)] string userInfo
            , CancellationToken cancellationToken
            , string language
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath
            , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Get("/{urlPrefix}/ApplicationStatus?lang={language}&applicationType={applicationType}&searchType={searchType}&searchTerm={searchTerm}")]
        Task<HttpResponseMessage> GetApplicationStatus(string applicationType
           , string searchType
           , string searchTerm
           , CancellationToken cancellationToken
           , string language
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Post("/{urlPrefix}/SaveApplication")]
        Task<HttpResponseMessage> SaveApplication([Body] PostSaveApplicationRequest request
           , [Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancellationToken
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Get("/{urlPrefix}/AllApplications?lang={language}&Page={page}&Limit={limit}&SortBy={sortBy}&SortDirection={sortDirection}&ReferenceNo={referenceNo}&SrNo={srNo}&ApplicationType={applicationType}&StatusId={statusId}&StatusDescription={statusDescription}&CreatedDateFrom={createdDateFrom}&CreatedDateTo={createdDateTo}")]
        Task<HttpResponseMessage> GetAllApplications(int page
           , int limit
           , string sortBy
           , string sortDirection
           , string referenceNo
           , string srNo
           , string applicationType
           , string statusId
           , string statusDescription
           , string createdDateFrom
           , string createdDateTo
           , [Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancellationToken
           , string language
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Get("/{urlPrefix}/ApplicationDetailV2?lang={language}&applicationType={applicationType}&searchTerm={searchTerm}&system={system}")]
        Task<HttpResponseMessage> GetApplicationDetail(string applicationType
            , string searchTerm
            , string system
            , bool isDSEligible
            , [Header(MobileConstants.Header_UserInfo)] string userInfo
            , CancellationToken cancellationToken
            , string language
            , [Header(MobileConstants.Header_Lang)] string lang
            , string urlPrefix = MobileConstants.ApiUrlPath
            , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Post("/{urlPrefix}/RemoveApplication")]
        Task<HttpResponseMessage> RemoveApplication([Body] PostRemoveApplicationRequest request
           , [Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancellationToken
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Get("/{urlPrefix}/SearchApplicationByCA?CANumber={accountNumber}")]
        Task<HttpResponseMessage> GetApplicationsByCA(string accountNumber
           , [Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancellationToken
           , string language
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);

        [Get("/{urlPrefix}/SyncSrApplication")]
        Task<HttpResponseMessage> SyncSRApplication([Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancellationToken
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);
    }
}