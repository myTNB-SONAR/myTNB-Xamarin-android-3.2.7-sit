using myTNB_Android.Src.Login.Models;
using myTNB_Android.Src.Login.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Login.Api
{
    public interface IAuthenticateUser
    {

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/IsUserAuthenticate")]
        Task<UserResponse> DoLogin([Body] UserAuthenticationRequest userRequest, CancellationToken cancellationToken);

    }
}