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
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.PreLogin.Activity;
using Android.Preferences;

namespace myTNB_Android.Src.LogoutEnd.Activity
{
    [Activity(Label = "@string/logout_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Logout")]
    public class LogoutEndActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.btnBackToHome)]
        Button btnBackToHome;

        private ISharedPreferences mSharedPref;

        public override int ResourceId()
        {
            return Resource.Layout.LogoutEndView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
            TextViewUtils.SetMuseoSans500Typeface(btnBackToHome , txtTitleInfo);
            // Create your application here
            mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            UserSessions.SavePhoneVerified(mSharedPref, false);
        }

        [OnClick(Resource.Id.btnBackToHome)]
        void OnBackToHome(object sender , EventArgs eventArgs)
        {
            Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
            PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(PreLoginIntent);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
            PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(PreLoginIntent);
        }
    }
}