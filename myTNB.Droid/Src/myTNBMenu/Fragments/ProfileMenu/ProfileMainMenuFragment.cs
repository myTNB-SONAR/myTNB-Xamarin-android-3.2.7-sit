using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;


using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CoordinatorLayout.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Java.Lang;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.AddAccountDisclaimer.Activity;
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
using myTNB_Android.Src.MyLearnMoreAboutTnb.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.NotificationAddAccount.Activity;
using myTNB_Android.Src.NotificationSettings.Activity;
using myTNB_Android.Src.Profile.Activity;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.UpdatePassword.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ProfileMenu
{
    [Activity(Label = "@string/dashboard_activity_title"
              , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.DashboardHome"
        , WindowSoftInputMode = SoftInput.AdjustNothing)]
    public class ProfileMainMenuFragment : BaseFragmentCustom, ProfileMenuContract.IView
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
        private ProfileMainMenuItemContentComponent fullName, referenceNumber, email, mobileNumber, password, cards, electricityAccount;
        private bool mobileNoUpdated = false;
        MyTNBAppToolTipBuilder logoutDialog;

        const string PAGE_ID = "Profile";

        private bool fromIDFlag = false;

        private int APP_LANGUAGE_REQUEST = 32766;

        public static AndroidX.Fragment.App.Fragment currentFragment;


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

                UserEntity user = UserEntity.GetActive();
                if (string.IsNullOrEmpty(user.IdentificationNo) || !user.IsActivated)
                {
                    fromIDFlag = true;
                }

                logoutDialog = MyTNBAppToolTipBuilder.Create(Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(GetLabelByLanguage("logout"))
                        .SetMessage(GetLabelByLanguage("logoutMessage"))
                        .SetCTALabel(GetLabelCommonByLanguage("ok"))
                        .SetCTAaction(() => { mPresenter.OnLogout(this.DeviceId()); })
                        .SetSecondaryCTALabel(GetLabelCommonByLanguage("cancel"))
                        .Build();

                ProfileDetailItemComponent myTNBAccountItem = GetMyTNBAccountItems();
                profileMenuItemsContent.AddView(myTNBAccountItem);

                ProfileMainMenuComponent settingsItem = GetSettingsItems();
                settingsItem.SetHeaderTitle(GetLabelByLanguage("settings"));
                settingsItem.HideHeaderTitle();
                profileMenuItemsContent.AddView(settingsItem);

                ProfileMenuItemComponent shareItem = GetShareItems();
                shareItem.SetHeaderTitle(GetLabelByLanguage("share"));
                shareItem.HideHeaderTitle();
                profileMenuItemsContent.AddView(shareItem);

                TextViewUtils.SetMuseoSans500Typeface(btnLogout);
                TextViewUtils.SetMuseoSans300Typeface(appVersion);

                appVersion.Text = Utility.GetAppVersionName(context);
                btnLogout.Text = GetLabelByLanguage("logout");
                PopulateActiveAccountDetails();
                mPresenter.Start();
                bool hasUpdatedMobile = MyTNBAccountManagement.GetInstance().IsUpdatedMobile();
                bool hasUpdatedPassword = MyTNBAccountManagement.GetInstance().IsPasswordUpdated();
                //bool hasUpdateLanguage = MyTNBAccountManagement.GetInstance().IsUpdateLanguage();
                if (hasUpdatedMobile)
                {
                    UserEntity userEntity = UserEntity.GetActive();
                    ShowMobileUpdateSuccess(userEntity.MobileNo);
                    MyTNBAccountManagement.GetInstance().SetIsUpdatedMobile(false);
                }

                /*if (hasUpdateLanguage)
                {
                    ShowLanguageUpdateSuccess();
                    MyTNBAccountManagement.GetInstance().SetIsUpdateLanguage(false);
                }*/
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
                ShowBackButton(false);
                ((DashboardHomeActivity)this.Activity).RemoveHeaderDropDown();
                ((DashboardHomeActivity)this.Activity).HideAccountName();
                ((DashboardHomeActivity)Activity).SetToolBarTitle(GetLabelByLanguage("title"));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (SMRPopUpUtils.GetSSMRMeterReadingRefreshNeeded())
                {
                    SMRPopUpUtils.SetSSMRMeterReadingRefreshNeeded(false);
                    ((DashboardHomeActivity)this.Activity).OnResetSSMRMeterReadingTutorial();
                    ((DashboardHomeActivity)this.Activity).OnResetPromotionRewards();
                    ((DashboardHomeActivity)this.Activity).OnResetEppTooltip();
                    ((DashboardHomeActivity)this.Activity).OnResetWhereIsMyAccNumber();
                    ((DashboardHomeActivity)this.Activity).OnResetBillDetailTooltip();
                }

                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Profile");
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetCurrentFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            currentFragment = fragment;
        }
        public void ShowBackButton(bool flag)
        {
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(flag);
            actionBar.SetDisplayShowHomeEnabled(flag);
        }


        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                ((DashboardHomeActivity)Activity).ReloadProfileMenu();

                if (requestCode == Constants.UPDATE_MOBILE_NO_REQUEST)
                {
                    if (resultCode == (int) Result.Ok)
                    {
                        UserEntity userEntity = UserEntity.GetActive();
                        ShowMobileUpdateSuccess(userEntity.MobileNo);
                        MyTNBAccountManagement.GetInstance().SetIsUpdatedMobile(true);
                    }
                }
                else if (requestCode == Constants.UPDATE_PASSWORD_REQUEST)
                {
                    if (resultCode == (int) Result.Ok)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsPasswordUpdated(true);
                    }
                }
                else if (requestCode == Constants.MANAGE_CARDS_REQUEST)
                {
                    if (resultCode == (int) Result.Ok)
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
            return Resource.Layout.ZProfileMenuFragmentLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        private ProfileDetailItemComponent GetMyTNBAccountItems()
        {
            Context context = Activity.ApplicationContext;

            ProfileDetailItemComponent myTNBAccountItem = new ProfileDetailItemComponent(context);

            List<View> myTNBAccountItems = new List<View>();

            fullName = new ProfileMainMenuItemContentComponent(context);
            fullName.SetTitle(GetLabelCommonByLanguage("titleProfile"));
            fullName.SetValue("");
            fullName.SetValue2("");
            fullName.SetFlagID(fromIDFlag);
            fullName.SetItemActionVisibility(true);
            fullName.SetItemActionCall(ShowProfile);
            myTNBAccountItems.Add(fullName);

            myTNBAccountItem.AddComponentView(myTNBAccountItems);
            return myTNBAccountItem;
        }

        private ProfileMainMenuComponent GetSettingsItems()
        {
            Context context = Activity.ApplicationContext;

            ProfileMainMenuComponent settingItem = new ProfileMainMenuComponent(context);

            List<View> settingItems = new List<View>();

            ProfileMainMenuItemSingleContentComponent payment = new ProfileMainMenuItemSingleContentComponent(context);
            payment.SetTitle(GetLabelCommonByLanguage("paymentMethod"));
            payment.SetIcon(1);
            payment.SetItemActionVisibility(true);
            payment.SetItemActionCall(ManageCards);
            settingItems.Add(payment);

            ProfileMainMenuItemSingleContentComponent electricityAcc = new ProfileMainMenuItemSingleContentComponent(context);
            electricityAcc.SetTitle(GetLabelCommonByLanguage("myElecttricityAcc"));
            electricityAcc.SetIcon(2);
            electricityAcc.SetItemActionVisibility(true);
            electricityAcc.SetItemActionCall(ShowManageAccount);
            settingItems.Add(electricityAcc);

            ProfileMainMenuItemSingleContentComponent AppSetting = new ProfileMainMenuItemSingleContentComponent(context);
            AppSetting.SetTitle(GetLabelCommonByLanguage("appSetting"));
            AppSetting.SetIcon(3);
            AppSetting.SetItemActionVisibility(true);
            AppSetting.SetItemActionCall(ShowNotificationSetting);
            settingItems.Add(AppSetting);

            ProfileMainMenuItemSingleContentComponent learnMoreTNB = new ProfileMainMenuItemSingleContentComponent(context);
            learnMoreTNB.SetTitle(GetLabelCommonByLanguage("learnMore"));
            learnMoreTNB.SetIcon(4);
            learnMoreTNB.SetItemActionVisibility(true);
            learnMoreTNB.SetItemActionCall(LearnMoreAboutTnb);
            settingItems.Add(learnMoreTNB);

            settingItem.AddComponentView(settingItems);
            return settingItem;
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
            
            try
            {
                fullName.SetValue(user.DisplayName);
                fullName.SetValue2(user.Email);

                string maskedNo = !string.IsNullOrEmpty(user?.IdentificationNo) ? user.IdentificationNo : "";

                if (!string.IsNullOrEmpty(maskedNo) && maskedNo.Count() > 4)
                {
                    string lastDigit = maskedNo.Substring(maskedNo.Length - 4);

                    maskedNo =  GetString(Resource.String.my_account_ic_no_mask) + " " + lastDigit;
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
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

        private void ShowNotificationSetting()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                mPresenter.OnNotification(this.DeviceId());
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

        public void ShowProfile()
        {
            try
            {
                Intent nextIntent = new Intent(this.Activity, typeof(MyProfileActivity));
                nextIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
                StartActivityForResult(nextIntent, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowNotifications()
        {
            try
            {
                MyTNBAccountManagement.GetInstance().SetIsUpdateLanguage(false);
                Intent nextIntent = new Intent(this.Activity, typeof(NotificationSettingsActivity));
                //nextIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
                StartActivity(nextIntent);
                //StartActivity(new Intent(this.Activity, typeof(NotificationSettingsActivity)));
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
            List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
            if (customerAccountList != null && customerAccountList.Count > 0)
            {
                Intent nextIntent = new Intent(this.Activity, typeof(MyAccountActivity));
                nextIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
                StartActivityForResult(nextIntent, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
            }
            else
            {
                Intent nextIntent = new Intent(this.Activity, typeof(LinkAccountActivity));
                nextIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
                StartActivityForResult(nextIntent, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
            }

        }

        private void LearnMoreAboutTnb()
        {
            Intent nextIntent = new Intent(this.Activity, typeof(MyLearnMoreActivity));
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
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
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
                    logoutDialog.Show();
                }
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

        private Snackbar mCCErrorSnakebar;
        public void ShowCCErrorSnakebar()
        {
            try
            {
                if (mCCErrorSnakebar != null && mCCErrorSnakebar.IsShown)
                {
                    mCCErrorSnakebar.Dismiss();
                }

                mCCErrorSnakebar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(GetLabelCommonByLanguage("ok"), delegate
                {

                    mCCErrorSnakebar.Dismiss();
                }
                );
                View v = mCCErrorSnakebar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(6);
                mCCErrorSnakebar.Show();
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
