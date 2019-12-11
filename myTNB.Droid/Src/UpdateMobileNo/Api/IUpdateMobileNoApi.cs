using myTNB_Android.Src.UpdateMobileNo.Models;
using myTNB_Android.Src.UpdateMobileNo.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.UpdateMobileNo.Api
{
    public interface IUpdateMobileNoApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/UpdatePhoneNumber")]
        Task<UpdateMobileResponse> UpdatePhoneNumber([Body] UpdateMobileRequest request, CancellationToken cancellationToken);
    }
}