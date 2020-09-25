using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication;
using Refit;

namespace myTNB.Mobile.API.Services.ApplicationStatus
{
    //Todo: Add Release API Key
#if !Release
    [Headers(new string[] { "ApiKey: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA"
        , "Content-Type: application/json" })]
#else
    [Headers(new string[] { "ApiKey: "
        , "Content-Type: application/json" })]
#endif
    internal interface IApplicationStatusService
    {
        [Get("/{urlPrefix}/SearchApplicationType?lang={language}")]
        Task<SearchApplicationTypeResponse> SearchApplicationType([Header(Constants.Header_UserInfo)] string userInfo
            , CancellationToken cancelToken
            , string language
            , string urlPrefix = Constants.ApiUrlPath
            , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Get("/{urlPrefix}/ApplicationStatus?lang={language}&applicationType={applicationType}&searchType={searchType}&searchTerm={searchTerm}")]
        Task<HttpResponseMessage> GetApplicationStatus(string applicationType
            , string searchType
            , string searchTerm
            , [Header(Constants.Header_UserInfo)] string userInfo
            , CancellationToken cancelToken
            , string language
            , string urlPrefix = Constants.ApiUrlPath
            , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Post("/{urlPrefix}/SaveApplication")]
        Task<HttpResponseMessage> SaveApplication([Body] PostSaveApplicationRequest request
           , [Header(Constants.Header_UserInfo)] string userInfo
           , CancellationToken cancelToken
           , string urlPrefix = Constants.ApiUrlPath
           , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Get("/{urlPrefix}/AllApplications?lang={language}&Page={page}&Limit={limit}&SortBy={sortBy}&SortDirection={sortDirection}&ReferenceNo={referenceNo}&SrNo={srNo}&SearchApplicationType={searchApplicationType}&StatusId={statusId}&StatusDescription={statusDescription}&CreatedDateFrom={createdDateFrom}&CreatedDateTo={createdDateTo}")]
        Task<HttpResponseMessage> GetAllApplications(int page
           , int limit
           , string sortBy
           , string sortDirection
           , string referenceNo
           , string srNo
           , string searchApplicationType
           , string statusId
           , string statusDescription
           , string createdDateFrom
           , string createdDateTo
           , [Header(Constants.Header_UserInfo)] string userInfo
           , CancellationToken cancelToken
           , string language
           , string urlPrefix = Constants.ApiUrlPath
           , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Get("/{urlPrefix}/ApplicationDetail?lang={language}&applicationType={applicationType}&id={id}")]
        Task<HttpResponseMessage> GetApplicationDetail(string applicationType
            , string id
            , [Header(Constants.Header_UserInfo)] string userInfo
            , CancellationToken cancelToken
            , string language
            , string urlPrefix = Constants.ApiUrlPath
            , [Header(Constants.Header_SecureKey)] string secureKey = Constants.ApiKeyId);
    }
}