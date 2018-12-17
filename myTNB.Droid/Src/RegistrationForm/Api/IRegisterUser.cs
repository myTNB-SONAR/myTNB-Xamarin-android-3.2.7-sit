using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using myTNB_Android.Src.RegistrationForm.Models;
using myTNB_Android.Src.RegistrationForm.Requests;
using System.Threading.Tasks;
using System.Threading;

namespace myTNB_Android.Src.RegistrationForm.Api
{
    public interface IRegisterUser
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/CreateNewUserWithToken")]
        Task<UserRegistrationResponse> RegisterUser([Body] UserRegistrationRequest request , CancellationToken token);
    }
}