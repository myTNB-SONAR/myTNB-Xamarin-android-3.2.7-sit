using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.FindUs.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.NotificationSettings.Activity;
using myTNB_Android.Src.Profile.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ProfileMenu
{
    public class ProfileMenuFragment : BaseFragmentCustom, ProfileMenuContract.IView
	{
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.profileMenuItemsContent)]
        LinearLayout profileMenuItemsContent;

        [BindView(Resource.Id.appVersion)]
        TextView appVersion;

        [BindView(Resource.Id.btnLogout)]
        Button btnLogout;

        ProfileMenuPresenter mPresenter;
        private LoadingOverlay loadingOverlay;
        private ProfileMenuItemContentComponent fullName, referenceNumber, email, mobileNumber, password, cards, electricityAccount;

        const string PAGE_ID = "Profile";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ProfileMenuPresenter(this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
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
                    ((DashboardHomeActivity)Activity).SetToolBarTitle(GetLabelByLanguage("title"));
                }
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Profile");
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            base.OnAttach(context);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            try
            {
                Context context = Activity.ApplicationContext;

                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);

                ProfileMenuItemComponent myTNBAccountItem = GetMyTNBAccountItems();
                myTNBAccountItem.SetHeaderTitle(GetLabelByLanguage("myTNBAccount"));
                profileMenuItemsContent.AddView(myTNBAccountItem);

                ProfileMenuItemComponent settingsItem = GetSettingsItems();
                settingsItem.SetHeaderTitle(GetLabelByLanguage("settings"));
                profileMenuItemsContent.AddView(settingsItem);

                ProfileMenuItemComponent helpSupportItem = GetHelpSupportItems();
                helpSupportItem.SetHeaderTitle(GetLabelByLanguage("helpAndSupport"));
                profileMenuItemsContent.AddView(helpSupportItem);

                ProfileMenuItemComponent shareItem = GetShareItems();
                shareItem.SetHeaderTitle(GetLabelByLanguage("share"));
                profileMenuItemsContent.AddView(shareItem);

                TextViewUtils.SetMuseoSans500Typeface(btnLogout);
                TextViewUtils.SetMuseoSans300Typeface(appVersion);

                appVersion.Text = Utility.GetAppVersionName(context);
                btnLogout.Text = GetLabelByLanguage("logout");

                mPresenter.Start();
            }
            catch (System.Exception e)
            {
                Log.Debug("Package Manager", e.StackTrace);
                //txt_app_version.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            ((DashboardHomeActivity)Activity).ReloadProfileMenu();
        }

        public override int ResourceId()
        {
            return Resource.Layout.ProfileMenuFragmentLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        private ProfileMenuItemComponent GetMyTNBAccountItems()
        {
            ProfileMenuItemComponent myTNBAccountItem = new ProfileMenuItemComponent(Context);

            List<View> myTNBAccountItems = new List<View>();

            fullName = new ProfileMenuItemContentComponent(Context);
            fullName.SetTitle(GetLabelCommonByLanguage("fullname").ToUpper());
            fullName.SetValue("");
            myTNBAccountItems.Add(fullName);

            referenceNumber = new ProfileMenuItemContentComponent(Context);
            referenceNumber.SetTitle(GetLabelCommonByLanguage("idNumber").ToUpper());
            referenceNumber.SetValue("");
            referenceNumber.SetItemActionVisibility(false);
            myTNBAccountItems.Add(referenceNumber);

            email = new ProfileMenuItemContentComponent(Context);
            email.SetTitle(GetLabelCommonByLanguage("email").ToUpper());
            email.SetValue("");
            email.SetItemActionVisibility(false);
            myTNBAccountItems.Add(email);

            mobileNumber = new ProfileMenuItemContentComponent(Context);
            mobileNumber.SetTitle(GetLabelCommonByLanguage("mobileNumber").ToUpper());
            mobileNumber.SetValue("");
            mobileNumber.SetItemActionVisibility(true);
            mobileNumber.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            myTNBAccountItems.Add(mobileNumber);

            password = new ProfileMenuItemContentComponent(Context);
            password.SetTitle(GetLabelCommonByLanguage("password").ToUpper());
            password.SetValue("");
            password.SetItemActionVisibility(true);
            password.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            myTNBAccountItems.Add(password);

            cards = new ProfileMenuItemContentComponent(Context);
            cards.SetTitle(GetLabelCommonByLanguage("cards").ToUpper());
            cards.SetValue("");
            cards.SetItemActionVisibility(true);
            cards.SetItemActionTitle(GetLabelCommonByLanguage("manage"));
            myTNBAccountItems.Add(cards);

            electricityAccount = new ProfileMenuItemContentComponent(Context);
            electricityAccount.SetTitle(GetLabelCommonByLanguage("electricityAccounts").ToUpper());
            electricityAccount.SetValue("3");
            electricityAccount.SetItemActionVisibility(true);
            electricityAccount.SetItemActionTitle(GetLabelCommonByLanguage("manage"));
            myTNBAccountItems.Add(electricityAccount);

            myTNBAccountItem.AddComponentView(myTNBAccountItems);
            return myTNBAccountItem;
        }

        private ProfileMenuItemComponent GetSettingsItems()
        {
            ProfileMenuItemComponent settingItem = new ProfileMenuItemComponent(Context);

            List<View> settingItems = new List<View>();

            ProfileMenuItemSingleContentComponent notification = new ProfileMenuItemSingleContentComponent(Context);
            notification.SetTitle(GetLabelByLanguage("notifications"));
            notification.SetItemActionCall(ShowNotificationSetting);
            settingItems.Add(notification);

            ProfileMenuItemSingleContentComponent language = new ProfileMenuItemSingleContentComponent(Context);
            language.SetTitle(GetLabelByLanguage("setAppLanguage"));
            language.SetItemActionCall(ShowAppLanguageSetting);
            settingItems.Add(language);

            settingItem.AddComponentView(settingItems);
            return settingItem;
        }

        private ProfileMenuItemComponent GetHelpSupportItems()
        {
            ProfileMenuItemComponent helpSupportItem = new ProfileMenuItemComponent(Context);

            List<View> helpSupportItems = new List<View>();

            ProfileMenuItemSingleContentComponent findUs = new ProfileMenuItemSingleContentComponent(Context);
            findUs.SetTitle(GetLabelByLanguage("findUs"));
            findUs.SetItemActionCall(ShowFindUs);
            helpSupportItems.Add(findUs);

            ProfileMenuItemSingleContentComponent billInquiry = new ProfileMenuItemSingleContentComponent(Context);
            billInquiry.SetTitle(GetLabelByLanguage("callUsBilling"));
            billInquiry.SetItemActionCall(ShowCallUsBilling);
            helpSupportItems.Add(billInquiry);

            ProfileMenuItemSingleContentComponent outage = new ProfileMenuItemSingleContentComponent(Context);
            outage.SetTitle(GetLabelByLanguage("callUsOutagesAndBreakdown"));
            outage.SetItemActionCall(ShowCallUsOutage);
            helpSupportItems.Add(outage);

            ProfileMenuItemSingleContentComponent faq = new ProfileMenuItemSingleContentComponent(Context);
            faq.SetTitle(GetLabelByLanguage("faq"));
            faq.SetItemActionCall(ShowFAQ);
            helpSupportItems.Add(faq);

            helpSupportItem.AddComponentView(helpSupportItems);
            return helpSupportItem;
        }

        private ProfileMenuItemComponent GetShareItems()
        {
            ProfileMenuItemComponent shareItem = new ProfileMenuItemComponent(Context);

            List<View> shareItems = new List<View>();

            ProfileMenuItemSingleContentComponent share = new ProfileMenuItemSingleContentComponent(Context);
            share.SetTitle(GetLabelByLanguage("shareDescription"));
            share.SetItemActionCall(ShowShareApp);
            shareItems.Add(share);

            ProfileMenuItemSingleContentComponent rate = new ProfileMenuItemSingleContentComponent(Context);
            rate.SetTitle(GetLabelByLanguage("rate"));
            rate.SetItemActionCall(ShowRateApp);
            shareItems.Add(rate);

            shareItem.AddComponentView(shareItems);
            return shareItem;
        }

        public void ShowUserData(UserEntity user, int numOfCards)
        {
            fullName.SetValue(user.DisplayName);

            try
            {

                if (user.IdentificationNo.Count() >= 4)
                {
                    string lastDigit = user.IdentificationNo.Substring(user.IdentificationNo.Length - 4);

                    referenceNumber.SetValue(GetString(Resource.String.my_account_ic_no_mask) + " " + lastDigit);
                }
                else
                {
                    referenceNumber.SetValue(GetString(Resource.String.my_account_ic_no_mask));
                }

                email.SetValue(user.Email);
                mobileNumber.SetValue(user.MobileNo);
                password.SetValue(GetString(Resource.String.my_account_dummy_password));
                cards.SetValue(string.Format("{0}", numOfCards));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowAppLanguageSetting()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent nextIntent = new Intent(this.Activity, typeof(AppLanguageActivity));
                StartActivityForResult(nextIntent, 1234908);
            }
        }

        private void ShowNotificationSetting()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                mPresenter.OnNotification(this.DeviceId());
            }
        }

        private void ShowFindUs()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(new Intent(this.Activity, typeof(MapActivity)));
            }
        }

        private void ShowCallUsBilling()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (WeblinkEntity.HasRecord("TNBCLO"))
                {
                    WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLO");
                    if (entity.OpenWith.Equals("PHONE"))
                    {
                        var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                        var intent = new Intent(Intent.ActionDial, uri);
                        StartActivity(intent);
                    }
                }
            }
        }

        private void ShowCallUsOutage()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (WeblinkEntity.HasRecord("TNBCLE"))
                {
                    WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLE");
                    if (entity.OpenWith.Equals("PHONE"))
                    {
                        var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                        var intent = new Intent(Intent.ActionDial, uri);
                        StartActivity(intent);
                    }
                }
            }
        }

        private void ShowFAQ()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(new Intent(this.Activity, typeof(FAQListActivity)));
            }
        }

        private void ShowShareApp()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                WeblinkEntity entity = WeblinkEntity.GetByCode("DROID");
                Intent shareIntent = new Intent(Intent.ActionSend);
                shareIntent.SetType("text/plain");
                shareIntent.PutExtra(Intent.ExtraSubject, entity.Title);
                shareIntent.PutExtra(Intent.ExtraText, entity.Url);
                StartActivity(Intent.CreateChooser(shareIntent, GetString(Resource.String.more_fragment_share_via)));
            }
        }

        private void ShowRateApp()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (WeblinkEntity.HasRecord("DROID"))
                {
                    WeblinkEntity entity = WeblinkEntity.GetByCode("DROID");
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
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                        var uri = Android.Net.Uri.Parse(entity.Url);
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                    }
                }
            }
        }

        public void ShowNotifications()
        {
            StartActivity(new Intent(this.Activity, typeof(NotificationSettingsActivity)));
        }

        public void ShowNotificationsProgressDialog()
        {
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

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            try
            {
                if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
                {
                    mCancelledExceptionSnackBar.Dismiss();
                }

                mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.more_menu_cancelled_exception_btn_close), delegate
                {

                    mCancelledExceptionSnackBar.Dismiss();
                }
                );
                mCancelledExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
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

                mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.more_menu_api_exception_btn_close), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();

                }
                );
                mApiExcecptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
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

                mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.more_menu_unknown_exception_btn_close), delegate
                {

                    mUknownExceptionSnackBar.Dismiss();

                }
                );
                mUknownExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(ProfileMenuContract.IUserActionsListener userActionListener)
        {
            //No Impl
        }
    }
}
