
using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Iid;
using Firebase.Messaging;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.Firebase.Services
{
    //[Service]
    //[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    internal class FirebaseTokenService : FirebaseMessagingService
    {
        readonly string TAG = typeof(FirebaseTokenService).Name;

        public override void OnNewToken(string refreshToken)
        {
            base.OnNewToken(refreshToken);
            Log.Debug(TAG, "Refreshed token :----------------> new FirebaseMessagingService API " + refreshToken);

            if (!string.IsNullOrEmpty(refreshToken))
            {
                FirebaseTokenEntity.RemoveLatest();
                FirebaseTokenEntity.InsertOrReplace(refreshToken, true);
            }

        }
    }
}