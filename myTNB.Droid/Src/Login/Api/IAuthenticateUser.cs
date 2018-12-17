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
using myTNB_Android.Src.Login.Models;
using System.Threading.Tasks;
using myTNB_Android.Src.Login.Requests;
using System.Threading;

namespace myTNB_Android.Src.Login.Api
{
    public interface IAuthenticateUser
    {

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/IsUserAuthenticate")]
        Task<UserResponse> DoLogin([Body] UserAuthenticationRequest userRequest, CancellationToken cancellationToken);

    }
}