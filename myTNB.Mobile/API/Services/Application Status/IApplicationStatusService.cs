using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Mobile.API.Services.ApplicationStatus
{
    [Headers(new string[] { "ApiKey:eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA"
        , "SecureKey: 9515F2FA-C267-42C9-8087-FABA77CB84DF"})]
    public interface IApplicationStatusService
    {
        [Get("/{urlPrefix}/SearchApplicationType")]
        Task<SearchApplicationTypeResponse> SearchApplicationType([Header(Constants.H_RoleID)] string roleID
            , [Header(Constants.H_UserID)] string userID
            , [Header(Constants.H_UserName)] string userName
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Get("/{urlPrefix}/GetApplicationStatus?applicationType={applicationType}&searchType={searchType}&searchTerm={searchTerm}")]
        Task<GetApplicationStatusResponse> GetApplicationStatus(string applicationType
            , string searchType
            , string searchTerm
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);
    }
}