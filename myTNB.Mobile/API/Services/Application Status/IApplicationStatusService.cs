using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Mobile.API.Services.ApplicationStatus
{
    [Headers("ApiKey:eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA")]
    public interface IApplicationStatusService
    {
        [Post("/{urlPrefix}/GetApplicationStatusMeta")]
        Task<GetApplicationStatusMetadataResponse> GetApplicationStatusMetaData([Body] GetApplicationStatusMetadataRequest request
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Post("/{urlPrefix}/GetApplicationStatusDetails")]
        Task<GetApplicationStatusDetailsResponse> GetApplicationStatusDetails([Body] GetApplicationStatusDetailsRequest request
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Post("/{urlPrefix}/GetSavedProjectStatus")]
        Task<GetSavedProjectStatusResponse> GetSavedProjectStatus([Body] GetSavedProjectStatusRequest request
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Post("/{urlPrefix}/GetSavedApplicationStatus")]
        Task<GetSavedApplicationStatusResponse> GetSavedApplicationStatus([Body] GetSavedApplicationStatusRequest request
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Post("/{urlPrefix}/SearchApplicationStatus")]
        Task<SearchApplicationStatusResponse> SearchApplicationStatus([Body] SearchApplicationStatusRequest request
            , CancellationToken cancelToken
            , string urlPrefix = Constants.ApiUrlPath);

        [Post("/{urlPrefix}/PatchUserApplicationStatus")]
        Task<PatchUserApplicationStatusResponse> PatchUserApplicationStatus([Body] PatchUserApplicationStatusRequest request
           , CancellationToken cancelToken
           , string urlPrefix = Constants.ApiUrlPath);

        //Todo: Temporary url and need to change after asmx is done
        [Get("/v6/mytnbappws.asmx/SearchApplicationType")]
        Task<SearchApplicationTypeResponse> SearchApplicationType(//[Body] SearchApplicationTypeRequest request
             [Header("RoleId")] string roleID
            , [Header("UserId")] string userID
            , [Header("UserName")] string userName
            , CancellationToken cancelToken);
    }
}