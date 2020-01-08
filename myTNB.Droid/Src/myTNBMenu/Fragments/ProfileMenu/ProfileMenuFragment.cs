using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
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
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.FindUs.Activity;
using myTNB_Android.Src.LogoutEnd.Activity;
using myTNB_Android.Src.ManageCards.Activity;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.MyAccount.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.NotificationSettings.Activity;
using myTNB_Android.Src.Profile.Activity;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.UpdatePassword.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ProfileMenu
{
    public class ProfileMenuFragment : BaseFragmentCustom, ProfileMenuContract.IView
	{
        [BindView(Resource.Id.profileMenuRootContent)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.profileMenuItemsContent)]
        LinearLayout profileMenuItemsContent;

        [BindView(Resource.Id.appVersion)]
        TextView appVersion;

        [BindView(Resource.Id.btnLogout)]
        Button btnLogout;

        ProfileMenuPresenter mPresenter;
        private LoadingOverlay loadingOverlay;
        private ProfileMenuItemContentComponent fullName, referenceNumber, email, mobileNumber, password, cards, electricityAccount;
        private bool mobileNoUpdated = false;
        MaterialDialog logoutProgressDialog;

        const string PAGE_ID = "Profile";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ProfileMenuPresenter(this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                }
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

                Bundle extras = Arguments;
                if (extras != null && extras.ContainsKey(Constants.FORCE_UPDATE_PHONE_NO))
                {
                    mobileNoUpdated = extras.GetBoolean(Constants.FORCE_UPDATE_PHONE_NO);
                    if (mobileNoUpdated)
                    {
                        ShowManageAccount();
                    }
                }

                logoutProgressDialog = new MaterialDialog.Builder(this.Activity)
                    .Title(GetLabelByLanguage("logout"))
                    .Content(GetLabelByLanguage("logoutMessage"))
                    .PositiveText(GetLabelCommonByLanguage("ok"))
                    .NeutralText(GetLabelCommonByLanguage("cancel"))
                    .OnPositive((dialog, which) => mPresenter.OnLogout(this.DeviceId()))
                    .OnNeutral((dialog, which) => dialog.Dismiss())
                    .Build();

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
                PopulateActiveAccountDetails();
                mPresenter.Start();
                bool hasUpdatedMobile = MyTNBAccountManagement.GetInstance().IsUpdatedMobile();
                if (hasUpdatedMobile)
                {
                    UserEntity userEntity = UserEntity.GetActive();
                    ShowMobileUpdateSuccess(userEntity.MobileNo);
                    MyTNBAccountManagement.GetInstance().SetIsUpdatedMobile(false);
                }

                try
                {
                    ((DashboardHomeActivity)Activity).SetToolBarTitle(GetLabelByLanguage("title"));
                }
                catch (System.Exception ex)
                {
                    Utility.LoggingNonFatalError(ex);
                }
            }
            catch (System.Exception e)
            {
                Log.Debug("Package Manager", e.StackTrace);
                appVersion.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();

            try
            {
                if (SMRPopUpUtils.GetSSMRMeterReadingRefreshNeeded())
                {
                    SMRPopUpUtils.SetSSMRMeterReadingRefreshNeeded(false);
                    ((DashboardHomeActivity)this.Activity).OnResetSSMRMeterReadingTutorial();
                    ((DashboardHomeActivity)this.Activity).OnResetPromotionRewards();
                }

                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Profile");
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                ((DashboardHomeActivity)Activity).ReloadProfileMenu();

                if (requestCode == Constants.UPDATE_MOBILE_NO_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        UserEntity userEntity = UserEntity.GetActive();
                        ShowMobileUpdateSuccess(userEntity.MobileNo);
                        MyTNBAccountManagement.GetInstance().SetIsUpdatedMobile(true);
                    }
                }
                else if (requestCode == Constants.UPDATE_PASSWORD_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        ShowPasswordUpdateSuccess();
                    }
                }
                else if (requestCode == Constants.MANAGE_CARDS_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        CreditCardData creditCard = JsonConvert.DeserializeObject<CreditCardData>(data.Extras.GetString(Constants.REMOVED_CREDIT_CARD));
                        mPresenter.UpdateCardList(creditCard);
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
            Context context = Activity.ApplicationContext;

            ProfileMenuItemComponent myTNBAccountItem = new ProfileMenuItemComponent(context);

            List<View> myTNBAccountItems = new List<View>();

            fullName = new ProfileMenuItemContentComponent(context);
            fullName.SetTitle(GetLabelCommonByLanguage("fullname").ToUpper());
            fullName.SetValue("");
            myTNBAccountItems.Add(fullName);

            referenceNumber = new ProfileMenuItemContentComponent(context);
            referenceNumber.SetTitle(GetLabelCommonByLanguage("idNumber").ToUpper());
            referenceNumber.SetValue("");
            referenceNumber.SetItemActionVisibility(false);
            myTNBAccountItems.Add(referenceNumber);

            email = new ProfileMenuItemContentComponent(context);
            email.SetTitle(GetLabelCommonByLanguage("email").ToUpper());
            email.SetValue("");
            email.SetItemActionVisibility(false);
            myTNBAccountItems.Add(email);

            mobileNumber = new ProfileMenuItemContentComponent(context);
            mobileNumber.SetTitle(GetLabelCommonByLanguage("mobileNumber").ToUpper());
            mobileNumber.SetValue("");
            mobileNumber.SetItemActionVisibility(true);
            mobileNumber.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            mobileNumber.SetItemActionCall(UpdateMobileNumber);
            myTNBAccountItems.Add(mobileNumber);

            password = new ProfileMenuItemContentComponent(context);
            password.SetTitle(GetLabelCommonByLanguage("password").ToUpper());
            password.SetValue("");
            password.SetItemActionVisibility(true);
            password.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            password.SetItemActionCall(UpdatePassword);
            myTNBAccountItems.Add(password);

            cards = new ProfileMenuItemContentComponent(context);
            cards.SetTitle(GetLabelCommonByLanguage("cards").ToUpper());
            cards.SetValue("");
            cards.SetItemActionVisibility(true);
            cards.SetItemActionTitle(GetLabelCommonByLanguage("manage"));
            cards.SetItemActionCall(ManageCards);
            myTNBAccountItems.Add(cards);

            electricityAccount = new ProfileMenuItemContentComponent(context);
            electricityAccount.SetTitle(GetLabelCommonByLanguage("electricityAccounts").ToUpper());
            List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
            electricityAccount.SetValue(customerAccountList.Count().ToString());
            electricityAccount.SetItemActionVisibility(true);
            electricityAccount.SetItemActionTitle(GetLabelCommonByLanguage("manage"));
            electricityAccount.SetItemActionCall(ShowManageAccount);
            myTNBAccountItems.Add(electricityAccount);
            if (customerAccountList.Count > 0)
            {
                electricityAccount.EnableActionCall(true);
            }
            else
            {
                electricityAccount.EnableActionCall(false);
            }

            myTNBAccountItem.AddComponentView(myTNBAccountItems);
            return myTNBAccountItem;
        }

        private ProfileMenuItemComponent GetSettingsItems()
        {
            Context context = Activity.ApplicationContext;

            ProfileMenuItemComponent settingItem = new ProfileMenuItemComponent(context);

            List<View> settingItems = new List<View>();

            ProfileMenuItemSingleContentComponent notification = new ProfileMenuItemSingleContentComponent(context);
            notification.SetTitle(GetLabelByLanguage("notifications"));
            notification.SetItemActionCall(ShowNotificationSetting);
            settingItems.Add(notification);

            ProfileMenuItemSingleContentComponent language = new ProfileMenuItemSingleContentComponent(context);
            language.SetTitle(GetLabelByLanguage("setAppLanguage"));
            language.SetItemActionCall(ShowAppLanguageSetting);
            settingItems.Add(language);

            settingItem.AddComponentView(settingItems);
            return settingItem;
        }

        private ProfileMenuItemComponent GetHelpSupportItems()
        {
            Context context = Activity.ApplicationContext;

            ProfileMenuItemComponent helpSupportItem = new ProfileMenuItemComponent(context);

            List<View> helpSupportItems = new List<View>();

            ProfileMenuItemSingleContentComponent findUs = new ProfileMenuItemSingleContentComponent(context);
            findUs.SetTitle(GetLabelByLanguage("findUs"));
            findUs.SetItemActionCall(ShowFindUs);
            helpSupportItems.Add(findUs);

            ProfileMenuItemSingleContentComponent billInquiry = new ProfileMenuItemSingleContentComponent(context);
            if (WeblinkEntity.HasRecord("TNBCLE"))
            {
                WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLE");
                if (entity != null && !string.IsNullOrEmpty(entity.Title))
                {
                    billInquiry.SetTitle(entity.Title);
                }
                else
                {
                    billInquiry.SetTitle(GetLabelByLanguage("callUsBilling"));
                }
            }
            else
            {
                billInquiry.SetTitle(GetLabelByLanguage("callUsBilling"));
            }
            billInquiry.SetItemActionCall(ShowCallUsBilling);
            helpSupportItems.Add(billInquiry);

            ProfileMenuItemSingleContentComponent outage = new ProfileMenuItemSingleContentComponent(context);
            if (WeblinkEntity.HasRecord("TNBCLO"))
            {
                WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLO");
                if (entity != null && !string.IsNullOrEmpty(entity.Title))
                {
                    outage.SetTitle(entity.Title);
                }
                else
                {
                    outage.SetTitle(GetLabelByLanguage("callUsOutagesAndBreakdown"));
                }
            }
            else
            {
                outage.SetTitle(GetLabelByLanguage("callUsOutagesAndBreakdown"));
            }
            outage.SetItemActionCall(ShowCallUsOutage);
            helpSupportItems.Add(outage);

            ProfileMenuItemSingleContentComponent faq = new ProfileMenuItemSingleContentComponent(context);
            faq.SetTitle(GetLabelByLanguage("faq"));
            faq.SetItemActionCall(ShowFAQ);
            helpSupportItems.Add(faq);

            ProfileMenuItemSingleContentComponent TnC = new ProfileMenuItemSingleContentComponent(context);
            TnC.SetTitle(GetLabelByLanguage("tnc"));
            TnC.SetItemActionCall(ShowTnC);
            helpSupportItems.Add(TnC);

            helpSupportItem.AddComponentView(helpSupportItems);
            return helpSupportItem;
        }

        private ProfileMenuItemComponent GetShareItems()
        {
            Context context = Activity.ApplicationContext;

            ProfileMenuItemComponent shareItem = new ProfileMenuItemComponent(context);

            List<View> shareItems = new List<View>();

            ProfileMenuItemSingleContentComponent share = new ProfileMenuItemSingleContentComponent(context);
            share.SetTitle(GetLabelByLanguage("shareDescription"));
            share.SetItemActionCall(ShowShareApp);
            shareItems.Add(share);

            ProfileMenuItemSingleContentComponent rate = new ProfileMenuItemSingleContentComponent(context);
            rate.SetTitle(GetLabelByLanguage("rate"));
            rate.SetItemActionCall(ShowRateApp);
            shareItems.Add(rate);

            shareItem.AddComponentView(shareItems);
            return shareItem;
        }

        public void ShowUserData(UserEntity user, int numOfCards)
        {
            cards.SetValue(string.Format("{0}", numOfCards));
        }

        private void PopulateActiveAccountDetails()
        {
            UserEntity user = UserEntity.GetActive();
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
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void UpdateMobileNumber()
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    Intent updateMobileNo = new Intent(this.Activity, typeof(UpdateMobileActivity));
                    StartActivityForResult(updateMobileNo, Constants.UPDATE_MOBILE_NO_REQUEST);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void UpdatePassword()
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    Intent updateMobileNo = new Intent(this.Activity, typeof(UpdatePasswordActivity));
                    StartActivityForResult(updateMobileNo, Constants.UPDATE_PASSWORD_REQUEST);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void ManageCards()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                mPresenter.OnManageCards();
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

        private void ShowCallUsOutage()
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

        private void ShowFAQ()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(new Intent(this.Activity, typeof(FAQListActivity)));
            }
        }

        private void ShowTnC()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(new Intent(this.Activity, typeof(TermsAndConditionActivity)));
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
                StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
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
            try
            {
                StartActivity(new Intent(this.Activity, typeof(NotificationSettingsActivity)));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowManageCards(List<CreditCardData> cardList)
        {
            try
            {
                Intent manageCard = new Intent(this.Activity, typeof(ManageCardsActivity));
                manageCard.PutExtra(Constants.CREDIT_CARD_LIST, JsonConvert.SerializeObject(cardList));
                StartActivityForResult(manageCard, Constants.MANAGE_CARDS_REQUEST);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowManageAccount()
        {
            Intent nextIntent = new Intent(this.Activity, typeof(MyAccountActivity));
            nextIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
            StartActivityForResult(nextIntent, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
        }

        public void ShowLogout()
        {
            try
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.Activity.ApplicationContext);
                Intent logout = new Intent(this.Activity, typeof(LogoutEndActivity));
                StartActivity(logout);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                .SetAction(GetLabelCommonByLanguage("close"), delegate
                {

                    mCancelledExceptionSnackBar.Dismiss();
                }
                );
                View v = mCancelledExceptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
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
                .SetAction(GetLabelCommonByLanguage("close"), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();

                }
                );
                View v = mApiExcecptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
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
                .SetAction(GetLabelCommonByLanguage("close"), delegate
                {

                    mUknownExceptionSnackBar.Dismiss();

                }
                );
                View v = mUknownExceptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mUknownExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowLogoutErrorMessage(string message)
        {
            try
            {
                Snackbar logoutErrorSnackbar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("ok"),
                             (view) =>
                             {

                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                            View v = logoutErrorSnackbar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        logoutErrorSnackbar.Show();
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

        [OnClick(Resource.Id.btnLogout)]
        void OnClickLogout(object sender, EventArgs eventArgs)
        {
            try
            {
                if (IsActive())
                {
                    logoutProgressDialog.Show();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void EnableManageCards()
        {
            try
            {
                cards.EnableActionCall(true);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableManageCards()
        {
            try
            {
                cards.EnableActionCall(false);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowMobileUpdateSuccess(string newPhone)
        {
            try
            {
                mobileNumber.SetValue(newPhone);
                Snackbar updatePhoneSnackBar = Snackbar.Make(rootView, GetLabelByLanguage("mobileNumberVerified"), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updatePhoneSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePhoneSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowPasswordUpdateSuccess()
        {
            try
            {
                Snackbar updatePassWordBar = Snackbar.Make(rootView, GetString(Resource.String.my_account_successful_update_password), Snackbar.LengthIndefinite)
                            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updatePassWordBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePassWordBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRemovedCardSuccess(CreditCardData creditCard, int numOfCards)
        {
            try
            {
                string lastDigits = creditCard.LastDigits.Substring(creditCard.LastDigits.Length - 4);
                cards.SetValue(string.Format("{0}", numOfCards));
                Snackbar removeCardSnackbar = Snackbar.Make(rootView, GetString(Resource.String.manage_cards_card_remove_successfully_wildcard, lastDigits), Snackbar.LengthIndefinite)
                           .SetAction(Utility.GetLocalizedCommonLabel("close"),
                            (view) =>
                            {
                                // EMPTY WILL CLOSE SNACKBAR
                            }
                           );
                           View v = removeCardSnackbar.View;
                       TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                       tv.SetMaxLines(5);
                       removeCardSnackbar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
