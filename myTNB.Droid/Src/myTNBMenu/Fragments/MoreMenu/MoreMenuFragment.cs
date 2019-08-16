using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.FindUs.Activity;
using myTNB_Android.Src.MyAccount.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.NotificationSettings.Activity;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;

namespace myTNB_Android.Src.myTNBMenu.Fragments.MoreMenu
{
    public class MoreMenuFragment : BaseFragment, MoreFragmentContract.IView
    {

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        /// TITLES
        [BindView(Resource.Id.txt_more_fragment_settings_title)]
        TextView txt_more_fragment_settings_title;

        [BindView(Resource.Id.txt_more_fragment_help_support_title)]
        TextView txt_more_fragment_help_support_title;


        [BindView(Resource.Id.txt_more_fragment_share_title)]
        TextView txt_more_fragment_share_title;


        /// CONTENT
        [BindView(Resource.Id.txt_more_fragment_settings_notifications)]
        TextView txt_more_fragment_settings_notifications;

        [BindView(Resource.Id.txt_more_fragment_settings_my_account)]
        TextView txt_more_fragment_settings_my_account;


        [BindView(Resource.Id.txt_more_fragment_help_support_find_us)]
        TextView txt_more_fragment_help_support_find_us;


        [BindView(Resource.Id.txt_more_fragment_help_support_understand_bill)]
        TextView txt_more_fragment_help_support_understand_bill;


        [BindView(Resource.Id.txt_more_fragment_help_support_faq)]
        TextView txt_more_fragment_help_support_faq;


        [BindView(Resource.Id.txt_more_fragment_help_support_TC)]
        TextView txt_more_fragment_help_support_TC;


        [BindView(Resource.Id.txt_more_fragment_help_support_call_us)]
        TextView txt_more_fragment_help_support_call_us;

        [BindView(Resource.Id.txt_more_fragment_help_support_call_us_1)]
        TextView txt_more_fragment_help_support_call_us_1;


        [BindView(Resource.Id.txt_more_fragment_share_share_this_app)]
        TextView txt_more_fragment_share_share_this_app;


        [BindView(Resource.Id.txt_more_fragment_share_rate_this_app)]
        TextView txt_more_fragment_share_rate_this_app;

        [BindView(Resource.Id.txt_app_version)]
        TextView txt_app_version;


        MoreFragmentContract.IUserActionsListener userActionsListener;
        MoreFragmentPresenter mPresenter;

        MaterialDialog notificationsProgressDialog;

        private LoadingOverlay loadingOverlay;

        private bool mobileNoUpdated = false;

        public override int ResourceId()
        {
            return Resource.Layout.MoreMenuView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //mPresenter = new MoreFragmentPresenter(this);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mPresenter = new MoreFragmentPresenter(this);


            TextViewUtils.SetMuseoSans500Typeface(txt_more_fragment_help_support_title, txt_more_fragment_settings_title, txt_more_fragment_share_title);
            TextViewUtils.SetMuseoSans300Typeface(txt_more_fragment_settings_notifications,
                txt_more_fragment_settings_my_account,
                txt_more_fragment_help_support_find_us,
                txt_more_fragment_help_support_call_us,
                txt_more_fragment_help_support_call_us_1,
                txt_more_fragment_help_support_understand_bill,
                txt_more_fragment_help_support_faq,
                txt_more_fragment_help_support_TC,
                txt_more_fragment_share_share_this_app,
                txt_more_fragment_share_rate_this_app,
                txt_app_version);

            try
            {
                Context context = Activity.ApplicationContext;
                var name = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
                var code = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
                if (name != null)
                {
                    txt_app_version.Text = GetString(Resource.String.text_app_version) + " " + name;
                }
            }
            catch (System.Exception e)
            {
                Log.Debug("Package Manager", e.StackTrace);
                txt_app_version.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }

            Bundle extras = Arguments;
            if (extras != null && extras.ContainsKey(Constants.FORCE_UPDATE_PHONE_NO))
            {
                mobileNoUpdated = extras.GetBoolean(Constants.FORCE_UPDATE_PHONE_NO);
                if (mobileNoUpdated)
                {
                    this.userActionsListener.OnMyAccount();
                }
            }
        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                }
            }
            catch (ClassCastException e)
            {

            }
            base.OnAttach(context);
        }

        [OnClick(Resource.Id.btnLogout)]
        void OnLogout(object sender, EventArgs eventArgs)
        {
            if (Activity is DashboardHomeActivity)
            {
                var dashboard = Activity as DashboardHomeActivity;
                dashboard.Logout();
            }
        }

        [OnClick(Resource.Id.txt_more_fragment_settings_notifications)]
        void OnNotificationClick(object sender, EventArgs e)
        {
            Log.Debug(Tag, "On Click " + this.userActionsListener);
            this.userActionsListener.OnNotification(this.DeviceId());
        }
        [OnClick(Resource.Id.txt_more_fragment_settings_my_account)]
        void OnMyAccountClick(object sender, EventArgs e)
        {
            this.userActionsListener.OnMyAccount();
        }

        [OnClick(Resource.Id.txt_more_fragment_help_support_call_us)]
        void OnCallUs(object sender, EventArgs e)
        {
            this.userActionsListener.OnCallUs();
        }

        [OnClick(Resource.Id.txt_more_fragment_help_support_call_us_1)]
        void OnCallUs1(object sender, EventArgs e)
        {
            this.userActionsListener.OnCallUs1();
        }


        [OnClick(Resource.Id.txt_more_fragment_help_support_understand_bill)]
        void OnUnderstandBill(object sender, EventArgs e)
        {
            this.userActionsListener.OnUnderstandBill();
        }

        [OnClick(Resource.Id.txt_more_fragment_help_support_faq)]
        void OnFAQ(object sender, EventArgs e)
        {
            this.userActionsListener.OnFAQ();
        }

        [OnClick(Resource.Id.txt_more_fragment_help_support_TC)]
        void OnTC(object sender, EventArgs e)
        {
            this.userActionsListener.OnTermsAndConditions();
        }

        [OnClick(Resource.Id.txt_more_fragment_share_share_this_app)]
        void OnShareApp(object sender, EventArgs e)
        {
            this.userActionsListener.OnShareApp();
        }

        [OnClick(Resource.Id.txt_more_fragment_help_support_find_us)]
        void OnFindUs(object sender, EventArgs e)
        {
            this.userActionsListener.OnFindUs();
        }

        [OnClick(Resource.Id.txt_more_fragment_share_rate_this_app)]
        void OnRateApp(object sender, EventArgs e)
        {
            this.userActionsListener.OnRateUs();
        }

        public void ShowNotifications()
        {
            StartActivity(new Intent(this.Activity, typeof(NotificationSettingsActivity)));
        }

        public void ShowTermsAndConditions()
        {
            StartActivity(new Intent(this.Activity, typeof(TermsAndConditionActivity)));
        }

        public void SetPresenter(MoreFragmentContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsResumed;
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            try
            {
                if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
                {
                    mCancelledExceptionSnackBar.Dismiss();
                }

                mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.more_cancelled_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.more_menu_cancelled_exception_btn_close), delegate
                {

                    mCancelledExceptionSnackBar.Dismiss();
                }
                );
                mCancelledExceptionSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            try
            {
                if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
                {
                    mApiExcecptionSnackBar.Dismiss();
                }

                mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.more_api_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.more_menu_api_exception_btn_close), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();

                }
                );
                mApiExcecptionSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(System.Exception exception)
        {
            try
            {
                if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
                {
                    mUknownExceptionSnackBar.Dismiss();

                }

                mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.more_unknown_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.more_menu_unknown_exception_btn_close), delegate
                {

                    mUknownExceptionSnackBar.Dismiss();

                }
                );
                mUknownExceptionSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowNotificationsProgressDialog()
        {
            //if (notificationsProgressDialog != null && notificationsProgressDialog.IsShowing)
            //{
            //    notificationsProgressDialog.Dismiss();
            //}

            //notificationsProgressDialog = new MaterialDialog.Builder(Activity)
            //    .Title(GetString(Resource.String.more_fragment_progress_title))
            //    .Content(GetString(Resource.String.more_fragment_progress_content))
            //    .Progress(true, 0)
            //    .Cancelable(false)
            //    .Build();

            //notificationsProgressDialog.Show();'
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(Activity, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideNotificationsProgressDialog()
        {
            //if (notificationsProgressDialog != null && notificationsProgressDialog.IsShowing)
            //{
            //    notificationsProgressDialog.Dismiss();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowMyAccount()
        {
            Intent nextIntent = new Intent(this.Activity, typeof(MyAccountActivity));
            nextIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
            StartActivity(nextIntent);
        }


        public void ShowFindUs()
        {
            // TODO : IMPL FIND US
            StartActivity(new Intent(this.Activity, typeof(MapActivity)));
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

        public void ShowUnderstandYourBill(WeblinkEntity entity)
        {
            if (entity.OpenWith.Equals("APP"))
            {
                //Intent weblink = new Intent(this.Activity , typeof(WeblinkActivity));
                //weblink.PutExtra(Constants.SELECTED_WEBLINK, JsonConvert.SerializeObject(entity));
                //StartActivity(weblink);
            }
            else
            {
                var uri = Android.Net.Uri.Parse(entity.Url);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
        }

        public void ShowFAQ(WeblinkEntity entity)
        {
            //if (entity.OpenWith.Equals("APP"))
            //{
            //    Intent weblink = new Intent(this.Activity, typeof(WeblinkActivity));
            //    weblink.PutExtra(Constants.SELECTED_WEBLINK, JsonConvert.SerializeObject(entity));
            //    StartActivity(weblink);
            //}
            //else
            //{
            //    var uri = Android.Net.Uri.Parse(entity.Url);
            //    var intent = new Intent(Intent.ActionView, uri);
            //    StartActivity(intent);
            //}
            StartActivity(new Intent(this.Activity, typeof(FAQListActivity)));
        }

        public void ShowShareApp(WeblinkEntity entity)
        {
            Intent shareIntent = new Intent(Intent.ActionSend);
            shareIntent.SetType("text/plain");
            shareIntent.PutExtra(Intent.ExtraSubject, entity.Title);
            shareIntent.PutExtra(Intent.ExtraText, entity.Url);
            StartActivity(Intent.CreateChooser(shareIntent, GetString(Resource.String.more_fragment_share_via)));
        }

        public void ShowRateUs(WeblinkEntity entity)
        {


            try
            {
                string[] array = entity.Url.Split(new[] { "?id=" }, StringSplitOptions.None);

                if (array.Length > 1)
                {
                    string id = array[1];
                    var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=" + id));
                    // we need to add this, because the activity is in a new context.
                    // Otherwise the runtime will block the execution and throw an exception
                    intent.AddFlags(ActivityFlags.NewTask);

                    Application.Context.StartActivity(intent);
                }
                //else
                //{
                //    if (entity.OpenWith.Equals("APP"))
                //    {
                //        Intent weblink = new Intent(this.Activity, typeof(WeblinkActivity));
                //        weblink.PutExtra(Constants.SELECTED_WEBLINK, JsonConvert.SerializeObject(entity));
                //        StartActivity(weblink);
                //    }
                //    else
                //    {
                //        var uri = Android.Net.Uri.Parse(entity.Url);
                //        var intent = new Intent(Intent.ActionView, uri);
                //        StartActivity(intent);
                //    }
                //}
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                //if (entity.OpenWith.Equals("APP"))
                //{
                //    Intent weblink = new Intent(this.Activity, typeof(WeblinkActivity));
                //    weblink.PutExtra(Constants.SELECTED_WEBLINK, JsonConvert.SerializeObject(entity));
                //    StartActivity(weblink);
                //}
                //else
                //{
                var uri = Android.Net.Uri.Parse(entity.Url);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
                //}
            }
        }
    }
}