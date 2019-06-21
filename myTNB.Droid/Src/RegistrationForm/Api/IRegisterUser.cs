using myTNB_Android.Src.RegistrationForm.Models;
using myTNB_Android.Src.RegistrationForm.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.RegistrationForm.Api
{
    public interface IRegisterUser
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/CreateNewUserWithToken")]
        Task<UserRegistrationResponse> RegisterUser([Body] UserRegistrationRequest request, CancellationToken token);
    }
}