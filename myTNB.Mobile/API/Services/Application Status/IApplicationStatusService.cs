using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.ApplicationStatus;
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
        [Get("/{urlPrefix}/SearchApplicationType")]
        Task<SearchApplicationTypeResponse> SearchApplicationType([Header(Constants.H_UserInfo)] string userInfo
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath
            , [Header(Constants.H_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Get("/{urlPrefix}/ApplicationStatus?applicationType={applicationType}&searchType={searchType}&searchTerm={searchTerm}")]
        Task<GetApplicationStatusResponse> GetApplicationStatus(string applicationType
            , string searchType
            , string searchTerm
            , [Header(Constants.H_UserInfo)] string userInfo
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath
            , [Header(Constants.H_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Post("/{urlPrefix}/SaveApplication")]
        Task<HttpResponseMessage> SaveApplication([Body] SaveApplicationRequest request
           , [Header(Constants.H_UserInfo)] string userInfo
           , CancellationToken cancelToken
           , string urlPrefix = Constants.ApiUrlPath
           , [Header(Constants.H_SecureKey)] string secureKey = Constants.ApiKeyId);

        [Get("/{urlPrefix}/AllApplications?Page={page}&Limit={limit}&SortBy={sortBy}&SortDirection={sortDirection}&ReferenceNo={referenceNo}&SrNo={srNo}&SearchApplicationType={searchApplicationType}&StatusId={statusId}&StatusDescription={statusDescription}&CreatedDateFrom={createdDateFrom}&CreatedDateTo={createdDateTo}")]
        Task<AllApplicationsResponse> GetAllApplications(int page
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
           , [Header(Constants.H_UserInfo)] string userInfo
           , CancellationToken cancelToken
           , string urlPrefix = Constants.ApiUrlPath
           , [Header(Constants.H_SecureKey)] string secureKey = Constants.ApiKeyId);
    }
}