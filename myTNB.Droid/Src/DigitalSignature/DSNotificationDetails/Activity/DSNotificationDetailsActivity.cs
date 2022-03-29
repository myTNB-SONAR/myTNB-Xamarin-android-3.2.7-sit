using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.Activity;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.Fragment;
using myTNB_Android.Src.DigitalSignature.DSNotificationDetails.MVP;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.DigitalSignature.DSNotificationDetails.Activity
{
    [Activity(Label = "DS Notification Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class DSNotificationDetailsActivity : BaseActivityCustom, DSNotificationDetailsContract.IView
    {
        [BindView(Resource.Id.dsNotifDetailTitle)]
        readonly TextView dsNotifDetailTitle;

        [BindView(Resource.Id.dsNotifDetailBtnVerifyNow)]
        readonly Button dsNotifDetailBtnVerifyNow;

        [BindView(Resource.Id.identityVerificationListContainer)]
        readonly LinearLayout identityVerificationListContainer;

        [BindView(Resource.Id.rootView)]
        ViewGroup rootView;

        NotificationDetails.Models.NotificationDetails notificationDetails;
        internal static myTNB.Mobile.NotificationOpenDirectDetails Notification;
        int position;
        DSNotificationDetailsPresenter mPresenter;
        public bool pushFromDashboard = false;
        private const string PAGE_ID = "DSNotificationDetails";

        AlertDialog removeDialog;

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DSNotificationDetailsView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.NotificationDetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_delete_notification:
                    removeDialog = new AlertDialog.Builder(this)
                        .SetTitle(Utility.GetLocalizedLabel("PushNotificationList", "deleteTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("PushNotificationList", "deleteMessage"))
                        .SetNegativeButton(Utility.GetLocalizedCommonLabel("cancel"),
                        delegate
                        {
                            removeDialog.Dismiss();
                        })
                        .SetPositiveButton(Utility.GetLocalizedCommonLabel("ok"),
                        delegate
                        {
                            mPresenter.DeleteNotificationDetail(notificationDetails);
                        })
                        .Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            result.PutExtra(Constants.ACTION_IS_READ, true);
            SetResult(Result.Ok, result);
            base.OnBackPressed();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                mPresenter = new DSNotificationDetailsPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
                base.OnCreate(savedInstanceState);

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM))
                    {
                        notificationDetails = DeSerialze<NotificationDetails.Models.NotificationDetails>(extras.GetString(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM));
                    }
                }

                SetUpViews();
                RenderContent();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);

            if (dsNotifDetailTitle != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(dsNotifDetailTitle);
                TextViewUtils.SetTextSize16(dsNotifDetailTitle);
                dsNotifDetailTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.DS_NOTIF_DETAILS, LanguageConstants.DSNotificationDetails.TITLE);
            }

            if (dsNotifDetailBtnVerifyNow != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(dsNotifDetailBtnVerifyNow);
                TextViewUtils.SetTextSize16(dsNotifDetailBtnVerifyNow);
                dsNotifDetailBtnVerifyNow.Text = Utility.GetLocalizedLabel(LanguageConstants.DS_NOTIF_DETAILS, LanguageConstants.DSNotificationDetails.VERIFY_NOW);
            }
        }

        public void RenderContent()
        {
            try
            {
                Dictionary<string, List<SelectorModel>> notificationDetailsDictionary = LanguageManager.Instance.GetSelectorsByPage<SelectorModel>("PushNotificationDetails");
                if (notificationDetailsDictionary != null
                && notificationDetailsDictionary.Count > 0
                && notificationDetailsDictionary.ContainsKey("dsDescriptionList"))
                {
                    List<SelectorModel> notificationContentList = notificationDetailsDictionary["dsDescriptionList"];
                    if (notificationContentList.Count > 0)
                    {
                        for (int j = 0; j < notificationContentList.Count; j++)
                        {
                            string title = notificationContentList[j].Value;
                            string desc = notificationContentList[j].Description;
                            DSIdentityVerificationListItemComponent itemListComponent = new DSIdentityVerificationListItemComponent(this);
                            var resIcon = j switch
                            {
                                0 => Resource.Drawable.Icon_Notification_Details_1,
                                1 => Resource.Drawable.Icon_Notification_Details_2,
                                2 => Resource.Drawable.Icon_Notification_Details_3,
                                _ => Resource.Drawable.Icon_Notification_Details_1,
                            };
                            itemListComponent.SetItemTitleText(title);
                            itemListComponent.SetItemDescText(desc);
                            itemListComponent.SetItemIcon(resIcon);
                            identityVerificationListContainer.AddView(itemListComponent);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                identityVerificationListContainer.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowLoadingScreen()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideLoadingScreen()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ReturnToDashboard()
        {
            Finish();
        }

        public void ShowNotificationListAsDeleted()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            result.PutExtra(Constants.ACTION_IS_DELETE, true);
            SetResult(Result.Ok, result);
            Finish();
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();

        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();

        }

        [OnClick(Resource.Id.dsNotifDetailBtnVerifyNow)]
        void VerifyOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                NavigateToIdentityVerification();
            }
        }

        private void NavigateToIdentityVerification()
        {
            Intent nbrDiscoverMoreIntent = new Intent(this, typeof(DSIdentityVerificationActivity));
            StartActivity(nbrDiscoverMoreIntent);
        }
    }
}