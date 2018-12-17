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
using Firebase.Iid;
using Android.Util;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.Firebase.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    internal class FirebaseTokenService : FirebaseInstanceIdService
    {
        readonly string TAG = typeof(FirebaseTokenService).Name;

        public override void OnTokenRefresh()
        {
            base.OnTokenRefresh();
            var refreshToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshToken);

            FirebaseTokenEntity.RemoveLatest();
            FirebaseTokenEntity.InsertOrReplace(refreshToken , true);

        }
    }
}