using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AppLaunch.Api
{
    public interface IWeblinksApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetWebLinks")]
        Task<WeblinkResponse> GetWebLinks([Body] WeblinkRequest request, CancellationToken token);

    }
}