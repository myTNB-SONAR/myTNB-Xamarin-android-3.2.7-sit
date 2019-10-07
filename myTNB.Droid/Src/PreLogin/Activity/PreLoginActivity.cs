using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_PreLogin_Menu.Activity;
using myTNB_Android.Src.FindUs.Activity;
using myTNB_Android.Src.Login.Activity;
using myTNB_Android.Src.PreLogin.MVP;
using myTNB_Android.Src.RegistrationForm.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.PreLogin.Activity
{
    [Activity(Label = "@string/app_name"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.PreLogin")]
    public class PreLoginActivity : BaseAppCompatActivity, PreLoginContract.IView
    {

        private PreLoginPresenter mPresenter;
        private PreLoginContract.IUserActionsListener userActionsListener;

        [BindView(Resource.Id.txtWelcome)]
        TextView txtWelcome;

        [BindView(Resource.Id.txtManageAccount)]
        TextView txtManageAccount;

        [BindView(Resource.Id.btnRegister)]
        Button btnRegister;

        [BindView(Resource.Id.btnLogin)]
        Button btnLogin;

        [BindView(Resource.Id.txtLikeToday)]
        TextView txtLikeToday;

        [BindView(Resource.Id.txtPromotion)]
        TextView txtPromotion;

        [BindView(Resource.Id.txtFindUs)]
        TextView txtLocation;

        [BindView(Resource.Id.txtFeedback)]
        TextView txtFeedback;

        [BindView(Resource.Id.txtCallUs)]
        TextView txtCallUs;

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        [BindView(Resource.Id.imgPromotion)]
        ImageView imgPromotion;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new PreLoginPresenter(this);
                TextViewUtils.SetMuseoSans500Typeface(txtWelcome);

                TextViewUtils.SetMuseoSans300Typeface(txtManageAccount, txtLikeToday, txtLocation, txtFeedback, txtCallUs, txtPromotion);

                TextViewUtils.SetMuseoSans500Typeface(btnLogin, btnRegister);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
            /** Enable/Disable Sitecore **/
            //if (MyTNBApplication.siteCoreUpdated)
            //{
            //    GetDataFromSiteCore();
            //}
            //else
            //{
            //    ShowPreLoginPromotion(true);
            //}
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.PreLoginView;
        }

        public void SetPresenter(PreLoginContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowLogin()
        {
            // TODO : ADD START ACTIVITY LOGIN ACTIVITY
            StartActivity(typeof(LoginActivity));
        }

        public void ShowRegister()
        {
            // TODO : ADD START ACTIVITY REGISTER ACTIVITY
            StartActivity(typeof(RegistrationFormActivity));
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Pre Login");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        [OnClick(Resource.Id.btnLogin)]
        void OnLogin(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToLogin();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnRegister)]
        void OnRegister(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToRegister();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.txtFindUs)]
        void OnFindUs(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToFindUs();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.txtCallUs)]
        void OnCallUs(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToCallUs();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }



        [OnClick(Resource.Id.txtFeedback)]
        void OnFeedback(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToFeedback();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowPreLoginPromotion(bool success)
        {
            if (success)
            {
                try
                {
                    RunOnUiThread(() =>
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        PreLoginPromoEntity wtManager = new PreLoginPromoEntity();
                        List<PreLoginPromoEntity> items = wtManager.GetAllItems();
                        if (items != null)
                        {
                            if (items.Count == 0)
                            {
                                GetDataFromSiteCore();
                            }
                            else
                            {
                                foreach (PreLoginPromoModel obj in items)
                                {
                                    var imageBitmap = ImageUtils.GetImageBitmapFromUrl(obj.Image);
                                    imgPromotion.SetImageBitmap(imageBitmap);
                                    imgPromotion.Click += delegate
                                    {
                                        //Intent webIntent = new Intent(this, typeof(PromotionWebActivity));
                                        //webIntent.PutExtra(Constants.PROMOTIONS_LINK, obj.GeneralLinkUrl);
                                        //StartActivity(webIntent);
                                    };
                                }
                            }
                        }
                        else
                        {
                            imgPromotion.SetBackgroundResource(Resource.Drawable.promotion);
                        }
                    });
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    Utility.LoggingNonFatalError(e);
                }
            }

        }

        public void ShowFindUS()
        {
            StartActivity(new Intent(this, typeof(MapActivity)));
        }

        public void ShowCallUs(WeblinkEntity entity)
        {
            if (entity.OpenWith.Equals("PHONE"))
            {
                var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                var intent = new Intent(Intent.ActionDial, uri);
                StartActivity(intent);
            }
        }

        public void ShowFeedback()
        {
            var feedbackIntent = new Intent(this, typeof(FeedbackPreLoginMenuActivity));
            StartActivity(feedbackIntent);
        }

        public void GetDataFromSiteCore()
        {
            try
            {
                progressBar.Visibility = ViewStates.Visible;
                this.userActionsListener.OnGetPreLoginPromo();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}