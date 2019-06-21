using myTNB_Android.Src.UpdateNickname.Models;
using myTNB_Android.Src.UpdateNickname.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.UpdateNickname.Api
{
    public interface IUpdateAccountApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/UpdateLinkedAccountNickName")]
        Task<UpdateLinkedAccountNickNameResponse> UpdateLinkedAccountNickName([Body] UpdateLinkedAccountNickNameRequest request, CancellationToken cancellationToken);

    }
}