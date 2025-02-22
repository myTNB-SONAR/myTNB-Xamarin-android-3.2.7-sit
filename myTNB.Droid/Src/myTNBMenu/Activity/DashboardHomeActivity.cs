﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.BottomNavigation;
using myTNB.AndroidApp.Src.AppLaunch.Activity;
using myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Bills.AccountStatement;
using myTNB.AndroidApp.Src.Bills.NewBillRedesign;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.EnergyBudget.Activity;
using myTNB.AndroidApp.Src.FloatingButtonMarketing.Activity;
using myTNB.AndroidApp.Src.ManageSupplyAccount.Activity;
using myTNB.AndroidApp.Src.MyAccount.Activity;
using myTNB.AndroidApp.Src.MyHome;
using myTNB.AndroidApp.Src.MyHome.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Async;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.ProfileMenu;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.myTNBMenu.MVP;
using myTNB.AndroidApp.Src.PreLogin.Activity;
using myTNB.AndroidApp.Src.RewardDetail.MVP;
using myTNB.AndroidApp.Src.SelectSupplyAccount.Activity;
using myTNB.AndroidApp.Src.ServiceDistruption.Activity;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.SummaryDashBoard.SummaryListener;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.Utils.Deeplink;
using myTNB.AndroidApp.Src.Utils.Notification;
using myTNB.AndroidApp.Src.WhatsNewDetail.MVP;
using myTNB.AndroidApp.Src.WhatsNewDialog;
using myTNB.Mobile;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Models.DBR;
using myTNB.Mobile.SessionCache;
using myTNB.SitecoreCMS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Android.Views.View;
using MarketingPopUp = myTNB.Mobile.Constants.MarketingPopup;

namespace myTNB.AndroidApp.Src.myTNBMenu.Activity
{
    [Activity(Label = "@string/dashboard_activity_title"
        , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.DashboardHome"
        , WindowSoftInputMode = SoftInput.AdjustPan)]
    public class DashboardHomeActivity : BaseToolbarAppCompatActivity, DashboardHomeContract.IView, ISummaryFragmentToDashBoardActivtyListener
    {
        internal readonly string TAG = typeof(DashboardHomeActivity).Name;

        public readonly static int PAYMENT_RESULT_CODE = 5451;
        public static DashboardHomeActivity dashboardHomeActivity;

        private DashboardHomeContract.IUserActionsListener userActionsListener;
        public DashboardHomePresenter mPresenter;

        //private bool urlSchemaCalled = false;
        //private string urlSchemaData = "";
        //private string urlSchemaPath = "";

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.mainView)]
        LinearLayout mainView;

        [BindView(Resource.Id.content_layout)]
        FrameLayout contentLayout;

        [BindView(Resource.Id.txt_account_name)]
        TextView txtAccountName;

        [BindView(Resource.Id.floating_button_img)]
        ImageView floatingButtonImg;

        [BindView(Resource.Id.floating_button_x)]
        ImageView floatingButtonHide;

        [BindView(Resource.Id.floating_button_layout)]
        RelativeLayout floatingButtonLayout;

        [BindView(Resource.Id.hide_button_layout)]
        LinearLayout hideButtonLayout;

        [BindView(Resource.Id.bottom_navigation)]
        public BottomNavigationView bottomNavigationView;

        AccountData SelectedAccountData;

        FloatingButtonModel FBitem;

        private bool alreadyStarted = false;

        bool mobileNoUpdated = false;

        private bool isBackButtonVisible = false;

        private bool isFromNotification = false;

        private string savedSSMRMeterReadingTimeStamp = "0000000";

        private string savedSSMRMeterReadingThreePhaseTimeStamp = "0000000";

        private string savedSSMRMeterReadingNoOCRTimeStamp = "0000000";

        private string savedSSMRMeterReadingThreePhaseNoOCRTimeStamp = "0000000";

        public static AndroidX.Fragment.App.Fragment currentFragment;

        public static bool GO_TO_INNER_DASHBOARD = false;

        private bool isSetToolbarClick = false;

        public bool IsRootTutorialShown = false;

        private static bool isFirstInitiate = false;

        private static bool isWhatNewDialogOnHold = false;

        private static bool isFromHomeMenu = false;

        private static bool isFromLogin = false;

        private string savedTimeStamp = "0000000";

        private string savedFloatingButtonTimeStamp = "0000000";

        private bool isFloatingButtonSiteCoreDone = false;

        internal static readonly int SELECT_SM_ACCOUNT_REQUEST_CODE = 8809;

        internal static readonly int SELECT_SD_POPUP_REQUEST_CODE = 8820;

        internal static readonly int SELECT_SM_POPUP_REQUEST_CODE = 8810;

        private IMenu ManageSupplyAccountMenu;

        ISharedPreferences mPref;

        //PostBREligibilityIndicatorsResponse billRenderingTenantResponse;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DashboardHomeView;
        }

        public void SetPresenter(DashboardHomeContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowPreLogin()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(PreLoginIntent);
            }
        }

        public int BottomNavigationViewHeight()
        {
            try
            {
                return bottomNavigationView.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return 0;
        }

        public override bool ShowBackArrowIndicator()
        {
            return isBackButtonVisible;
        }

        public void SetCurrentFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            currentFragment = fragment;
        }

        private void SetBottomNavigationLabels()
        {
            try
            {
                RelativeSizeSpan relativeSizeSpan = TextViewUtils.IsLargeFonts
                    ? new RelativeSizeSpan(1.5f)
                    : new RelativeSizeSpan(1f);

                IMenu bottomMenu = bottomNavigationView.Menu;
                IMenuItem item;

                item = bottomMenu.FindItem(Resource.Id.menu_dashboard);


                SpannableString spanStringhome = new SpannableString(Utility.GetLocalizedLabel("Tabbar", "home"));

                spanStringhome.SetSpan(relativeSizeSpan, 0, spanStringhome.Length(), SpanTypes.ExclusiveExclusive);
                item.SetTitle(spanStringhome);

                item = bottomMenu.FindItem(Resource.Id.menu_bill);


                SpannableString spanStringbill = new SpannableString(Utility.GetLocalizedLabel("Tabbar", "bill"));

                spanStringbill.SetSpan(relativeSizeSpan, 0, spanStringbill.Length(), SpanTypes.ExclusiveExclusive);
                item.SetTitle(spanStringbill);

                item = bottomMenu.FindItem(Resource.Id.menu_promotion);


                SpannableString spanStringpromotion = new SpannableString(Utility.GetLocalizedLabel("Tabbar", "promotion"));

                spanStringpromotion.SetSpan(relativeSizeSpan, 0, spanStringpromotion.Length(), SpanTypes.ExclusiveExclusive);
                item.SetTitle(spanStringpromotion);

                item = bottomMenu.FindItem(Resource.Id.menu_reward);
                if (item != null)
                {
                    item.SetTitle(Utility.GetLocalizedLabel("Tabbar", "rewards"));

                    SpannableString spanStringprofile = new SpannableString(Utility.GetLocalizedLabel("Tabbar", "rewards"));

                    spanStringprofile.SetSpan(relativeSizeSpan, 0, spanStringprofile.Length(), SpanTypes.ExclusiveExclusive);
                    item.SetTitle(spanStringprofile);
                }

                item = bottomMenu.FindItem(Resource.Id.menu_more);


                SpannableString spanString = new SpannableString(Utility.GetLocalizedLabel("Tabbar", "profile"));
                int end = spanString.Length();
                spanString.SetSpan(relativeSizeSpan, 0, end, SpanTypes.ExclusiveExclusive);
                item.SetTitle(spanString);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardHomeLarge
                : Resource.Style.Theme_DashboardHome);
            dashboardHomeActivity = this;
            base.SetToolBarTitle(GetString(Resource.String.dashboard_activity_title));
            mPresenter = new DashboardHomePresenter(this, this, PreferenceManager.GetDefaultSharedPreferences(this));
            TextViewUtils.SetMuseoSans500Typeface(txtAccountName);
            mPref = PreferenceManager.GetDefaultSharedPreferences(this);

            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
            if (IsRewardsDisabled)
            {
                if (bottomNavigationView.Menu.FindItem(Resource.Id.menu_reward) != null)
                {
                    bottomNavigationView.Menu.RemoveItem(Resource.Id.menu_reward);
                }
            }
            CheckStatusEligibleEB();
            CheckStatusEligibleSD();
            IsRootTutorialShown = false;
            SetBottomNavigationLabels();
            bottomNavigationView.SetShiftMode(false, false);
            bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
            bottomNavigationView.ItemIconTintList = null;

            bottomNavigationView.ItemSelected += BottomNavigationView_NavigationItemSelected;

            RewardsMenuUtils.OnSetRewardLoading(false);

            WhatsNewMenuUtils.OnSetWhatsNewLoading(false);

            Bundle extras = Intent?.Extras;

            if (extras != null && (extras.ContainsKey("FROM_NOTIFICATION") ||
                extras.ContainsKey("FROM_MANAGE_BILL_DELIVERY") ||
                extras.ContainsKey(AccountStatementConstants.FROM_ACCOUNT_STATEMENT)))
            {
                if (extras.ContainsKey("MENU"))
                {
                    if (extras.GetString("MENU") == "BillMenu")
                    {
                        AccountData selectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString("DATA"));
                        alreadyStarted = true;
                        this.mPresenter.UpdateTrackDashboardMenu(Resource.Id.menu_bill);
                        ShowBillMenu(selectedAccountData);
                    }
                }
                else
                {
                    mPresenter.OnAccountSelectDashBoard();
                    isFromNotification = true;
                    alreadyStarted = true;
                }
            }

            // if (extras != null && extras.ContainsKey("urlSchemaData"))
            // {
            //     urlSchemaCalled = true;
            //     urlSchemaData = extras.GetString("urlSchemaData");
            //     if (extras != null && extras.ContainsKey("urlSchemaPath"))
            //     {
            //         urlSchemaPath = extras.GetString("urlSchemaPath");
            //     }
            // }
            this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Click += DashboardHomeActivity_Click;

            if (extras != null && extras.ContainsKey("FromDashBoard"))
            {
                isFromLogin = true;
            }

            try
            {
                if (!alreadyStarted)
                {
                    this.userActionsListener.Start();
                    OnSetupSSMRMeterReadingTutorial();
                    this.mPresenter.OnGetEPPTooltipContentDetail();
                    this.mPresenter.OnGetBillTooltipContent();
                    alreadyStarted = true;
                    //PopulateIdentificationDetails();
                }
            }
            catch (System.Exception e)
            {
                Intent LaunchViewIntent = new Intent(this, typeof(LaunchViewActivity));
                LaunchViewActivity.MAKE_INITIAL_CALL = true;
                LaunchViewIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(LaunchViewIntent);
                Utility.LoggingNonFatalError(e);
            }
            TextViewUtils.SetTextSize18(txtAccountName);

            if (ApplicationStatusSearchDetailCache.Instance.ShouldSave)
            {
                RouteToApplicationLanding();
            }

            //GetBillTenantRenderingAsync();

            try
            {
                new SyncSRApplicationAPI(this).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, string.Empty);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Sync SR Error: " + e.Message);
            }
        }

        public async void OnSetupFloatingButton()
        {
            try
            {
                if (FloatingButtonUtils.GetFloatingButton() != null)
                {
                    SetCustomFloatingButtonImage(FloatingButtonUtils.GetFloatingButton());
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        PopulateFloatingButton(FloatingButtonUtils.GetFloatingButton());
                    };
                    h.PostDelayed(myAction, 3000);

                }
                else
                {
                    if (!isFloatingButtonSiteCoreDone)
                    {
                        this.userActionsListener.GetSavedFloatingButtonTimeStamp();
                    }
                }

            }
            catch (Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }

        }

        //private async void GetBillTenantRenderingAsync()
        //{
        //    try
        //    {
        //        if (!AccessTokenCache.Instance.HasTokenSaved(this))
        //        {
        //            string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
        //            AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
        //        }
        //        List<string> dBRCAs = DBRUtility.Instance.GetCAList();
        //        billRenderingTenantResponse = await DBRManager.Instance.PostBREligibilityIndicators(dBRCAs, UserEntity.GetActive().UserID, AccessTokenCache.Instance.GetAccessToken(this));

        //        HideProgressDialog();

        //        //Nullity Check
        //        //    if (billRenderingTenantResponse == null
        //        //       && billRenderingTenantResponse.StatusDetail == null
        //        //       && !billRenderingTenantResponse.StatusDetail.IsSuccess
        //        //       && billRenderingTenantResponse.Content == null
        //        //      )
        //        //    {


        //        //        string title = billRenderingTenantResponse != null && billRenderingTenantResponse.StatusDetail != null && billRenderingTenantResponse.StatusDetail.Title.IsValid()
        //        //            ? billRenderingTenantResponse?.StatusDetail?.Title
        //        //            : Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE);

        //        //        string message = billRenderingTenantResponse != null && billRenderingTenantResponse.StatusDetail != null && billRenderingTenantResponse.StatusDetail.Message.IsValid()
        //        //           ? billRenderingTenantResponse?.StatusDetail?.Message
        //        //           : Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG);

        //        //        string cta = billRenderingTenantResponse != null && billRenderingTenantResponse.StatusDetail != null && billRenderingTenantResponse.StatusDetail.PrimaryCTATitle.IsValid()
        //        //           ? billRenderingTenantResponse?.StatusDetail?.PrimaryCTATitle
        //        //           : Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.OK);

        //        //        this.RunOnUiThread(() =>
        //        //        {
        //        //            MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
        //        //                .SetTitle(title ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE))
        //        //                .SetMessage(message ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG))
        //        //                .SetCTALabel(cta ?? Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.OK))
        //        //                .Build();
        //        //            errorPopup.Show();
        //        //        });
        //        //    }
        //    }
        //    catch (System.Exception e)
        //    {
        //        Utility.LoggingNonFatalError(e);
        //    }
        //}

        public void RouteToApplicationLanding(string toastMessage = "")
        {
            RunOnUiThread(() =>
            {
                ShowProgressDialog();
            });

            _ = OnSearchApplication(toastMessage);
        }

        private async Task OnSearchApplication(string toastMessage = "")
        {
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                searchApplicationTypeResponse = await myTNB.Mobile.ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);

                RunOnUiThread(() =>
                {
                    if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                    {
                        SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                    }
                    HideProgressDialog();
                });
            }

            RunOnUiThread(() =>
            {
                HideProgressDialog();

                if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    AllApplicationsCache.Instance.Clear();
                    AllApplicationsCache.Instance.Reset();
                    Intent applicationLandingIntent = new Intent(this, typeof(ApplicationStatusLandingActivity));
                    applicationLandingIntent.PutExtra(MyHomeConstants.CANCEL_TOAST_MESSAGE, toastMessage);
                    StartActivityForResult(applicationLandingIntent, Constants.APPLICATION_STATUS_LANDING_FROM_DASHBOARD_REQUEST_CODE);
                }
                else
                {
                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                         .SetTitle(searchApplicationTypeResponse.StatusDetail.Title)
                         .SetMessage(searchApplicationTypeResponse.StatusDetail.Message)
                         .SetCTALabel(searchApplicationTypeResponse.StatusDetail.PrimaryCTATitle)
                         .Build();
                    errorPopup.Show();
                }
            });
        }

        public void ShowBackButton(bool flag)
        {
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(flag);
            this.SupportActionBar.SetDisplayShowHomeEnabled(flag);
        }

        public void OnResetSSMRMeterReadingTutorial()
        {
            OnSetupSSMRMeterReadingTutorial();
        }

        public void OnResetPromotionRewards()
        {
            this.mPresenter.OnResetRewardPromotionThread();
        }

        public void OnResetEppTooltip()
        {
            this.mPresenter.OnGetEPPTooltipContentDetail();
        }

        public void OnResetBillDetailTooltip()
        {
            this.mPresenter.OnGetBillTooltipContent();
        }

        public void OnResetWhereIsMyAccNumber()
        {
            this.mPresenter.OnWhereIsMyAccNumberContentDetail();
        }

        private void OnSetupSSMRMeterReadingTutorial()
        {
            savedSSMRMeterReadingTimeStamp = "0000000";

            savedSSMRMeterReadingThreePhaseTimeStamp = "0000000";

            savedSSMRMeterReadingNoOCRTimeStamp = "0000000";

            savedSSMRMeterReadingThreePhaseNoOCRTimeStamp = "0000000";

            this.mPresenter.GetSmartMeterReadingWalkthroughtTimeStamp();

            this.mPresenter.GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();

            this.mPresenter.GetSmartMeterReadingWalkthroughtNoOCRTimeStamp();

            this.mPresenter.GetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();
        }

        public override void OnBackPressed()
        {
            try
            {
                if (currentFragment.GetType() == typeof(DashboardChartFragment) ||
                    currentFragment.GetType() == typeof(FeedbackMenuFragment))
                {
                    if (isFromNotification)
                    {
                        this.Finish();
                    }
                    else
                    {
                        ShowHomeDashBoard();
                    }
                }
                else
                {
                    this.Finish();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void BottomNavigationView_NavigationItemSelected(object sender, BottomNavigationView.ItemSelectedEventArgs e)
        {
            this.userActionsListener.OnMenuSelect(e.Item.ItemId);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            HideFloatingButton();
            //if (DashboardHomeActivity.GO_TO_INNER_DASHBOARD)
            //{

            //}
            //else
            //{
            if (currentFragment.GetType() == typeof(HomeMenuFragment) ||
                currentFragment.GetType() == typeof(ProfileMainMenuFragment) ||
                currentFragment.GetType() == typeof(RewardMenuFragment) ||
                currentFragment.GetType() == typeof(ItemisedBillingMenuFragment) ||
                currentFragment.GetType() == typeof(FeedbackMenuFragment) ||
                currentFragment.GetType() == typeof(WhatsNewMenuFragment))
            {
                //CustomerBillingAccount selected = new CustomerBillingAccount();
                //selected = CustomerBillingAccount.GetSelected();
                //DashboardHomeActivity.GO_TO_INNER_DASHBOARD = false;
                MenuInflater.Inflate(Resource.Menu.ManageSupplyAccountToolbarMenu, menu);
                ManageSupplyAccountMenu = menu;
                ManageSupplyAccountMenu.FindItem(Resource.Id.icon_log_activity_unread).SetIcon(GetDrawable(Resource.Drawable.manage_account)).SetVisible(false);
            }
            else
            {
                CustomerBillingAccount selected = new CustomerBillingAccount();
                selected = CustomerBillingAccount.GetSelected();
                DashboardHomeActivity.GO_TO_INNER_DASHBOARD = false;
                MenuInflater.Inflate(Resource.Menu.ManageSupplyAccountToolbarMenu, menu);
                ManageSupplyAccountMenu = menu;
                ManageSupplyAccountMenu.FindItem(Resource.Id.icon_log_activity_unread).SetIcon(GetDrawable(Resource.Drawable.manage_account)).SetVisible(true);
                Handler h = new Handler();
                Action myAction = () =>
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    if (!UserSessions.HasManageAccessIconTutorialShown(this.mPref))
                    {
                        OnManageAccessIconTutorialDialog(selected.isOwned, selected.AccountTypeId);

                    }
                };
                h.PostDelayed(myAction, 50);
            }

            //}

            return base.OnCreateOptionsMenu(menu);
        }

        public void OnManageAccessIconTutorialDialog(bool flag, string accountTypeId)
        {
            Handler h = new Handler();
            Action myAction = () =>
            {
                NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.mPresenter.OnGeneraNewAppTutorialList(flag, accountTypeId));
            };
            h.PostDelayed(myAction, 100);
        }

        public int GetViewBillButtonHeight()
        {

            int height = ManageSupplyAccountMenu.FindItem(Resource.Id.icon_log_activity_unread).Icon.IntrinsicHeight;
            return height;
        }

        public int GetViewBillButtonWidth()
        {
            int width = ManageSupplyAccountMenu.FindItem(Resource.Id.icon_log_activity_unread).Icon.IntrinsicWidth;
            return width;
        }

        public int GetTopHeight()
        {
            int i = 0;

            try
            {
                Rect offsetViewBounds = new Rect();
                //returns the visible bounds
                toolbar.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent

                rootView.OffsetDescendantRectToMyCoords(toolbar, offsetViewBounds);

                i = offsetViewBounds.Top + (int)DPUtils.ConvertDPToPx(14f);

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            string mAccountNumber = null;
            switch (item.ItemId)
            {
                case Resource.Id.icon_log_activity_unread:
                    CustomerBillingAccount selected = new CustomerBillingAccount();
                    selected = CustomerBillingAccount.GetSelected();
                    UsageHistoryEntity storedEntity = new UsageHistoryEntity();
                    storedEntity = UsageHistoryEntity.GetItemByAccountNo(selected.AccNum);
                    ShowManageSupplyAccount(AccountData.Copy(selected, false));
                    break;

            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (this.mPresenter != null)
            {
                this.mPresenter.OnValidateData();

            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        [OnClick(Resource.Id.txt_account_name)]
        void OnSelectSupplyAccount(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.SelectSupplyAccount();
        }

        [OnClick(Resource.Id.floating_button_x)]
        void HideFloatingIcon(object sender, EventArgs eventArgs)
        {
            floatingButtonImg.Visibility = ViewStates.Gone;
            floatingButtonHide.Visibility = ViewStates.Gone;
        }

        [OnClick(Resource.Id.floating_button_img)]
        void OnSelectFloatingIcon(object sender, EventArgs eventArgs)
        {

            // FBitem = FloatingButtonUtils.GetFloatingButton();
            FloatingButtonEntity wtManager = new FloatingButtonEntity();
            List<FloatingButtonEntity> floatingButtonList = wtManager.GetAllItems();

            //tncWebView = FindViewById<WebView>(Resource.Id.tncWebView);

            if (floatingButtonList[0].Title == Module.WEB.ToString())
            {
                if (!string.IsNullOrEmpty(floatingButtonList[0].Description))
                {

                    string url = string.Empty;
                    string title = string.Empty;

                    var splitedString = floatingButtonList[0].Description.Split(';');

                    url = splitedString[0] ?? string.Empty;
                    title = splitedString[1] ?? string.Empty;

                    if ((!string.IsNullOrEmpty(url)) && (!string.IsNullOrEmpty(title)))
                    {
                        try
                        {
                            DynatraceHelper.OnTrack(DynatraceConstants.FloatingIcon.FloatingModule.WEB);
                            Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                            webIntent.PutExtra(Constants.IN_APP_LINK, url);
                            webIntent.PutExtra(Constants.IN_APP_TITLE, title);
                            this.StartActivity(webIntent);
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }

                    }
                }

            }
            else if (floatingButtonList[0].Title == Module.DBR.ToString())
            {
                if (!string.IsNullOrEmpty(floatingButtonList[0].Description))
                {
                    CustomerBillingAccount dbrAccount = GetEligibleDBRAccount();
                    try
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.FloatingIcon.FloatingModule.DBR);
                        Intent intent = new Intent(this, typeof(FloatingButtonMarketingActivity));
                        //intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(billRenderingTenantResponse));
                        intent.PutExtra("accountNumber", dbrAccount.AccNum);
                        StartActivity(intent);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }
            else if (floatingButtonList[0].Title == Module.BR.ToString())
            {
                if (!string.IsNullOrEmpty(floatingButtonList[0].Description))
                {

                    try
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.FloatingIcon.FloatingModule.BR);
                        Intent supplyAccount = new Intent(this, typeof(NBRDiscoverMoreActivity));
                        StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

            }
            else if (floatingButtonList[0].Title == Module.SD.ToString())
            {
                if (!string.IsNullOrEmpty(floatingButtonList[0].Description))
                {

                    try
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.FloatingIcon.FloatingModule.SD);
                        Intent SDDiscoverCom = new Intent(this, typeof(ServiceDisruptionActivity));
                        SDDiscoverCom.PutExtra("fromDashboard", true);
                        StartActivityForResult(SDDiscoverCom, SELECT_SD_POPUP_REQUEST_CODE);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

            }
            else if (floatingButtonList[0].Title == Module.EB.ToString())
            {
                if (!string.IsNullOrEmpty(floatingButtonList[0].Description))
                {

                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        try
                        {
                            if (currentFragment.GetType() == typeof(HomeMenuFragment))
                            {
                                DynatraceHelper.OnTrack(DynatraceConstants.FloatingIcon.FloatingModule.EB);
                                //Intent EBPopupPage = new Intent(this, typeof(EBPopupScreenActivity));
                                //StartActivityForResult(EBPopupPage, SELECT_SM_POPUP_REQUEST_CODE);
                                HomeMenuFragment fragment = (HomeMenuFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                                fragment.EBPopupActivity();
                            }
                        }
                        catch (System.Exception err)
                        {
                            Utility.LoggingNonFatalError(err);
                        }
                    }
                }

            }
            else if (floatingButtonList[0].Title == Module.TNG.ToString())
            {
                if (!string.IsNullOrEmpty(floatingButtonList[0].Description))
                {

                    try
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.FloatingIcon.FloatingModule.TNG);
                        Intent MarketingActivity = new Intent(this, typeof(FloatingButtonMarketingActivity));
                        StartActivity(MarketingActivity);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

            }
        }

        public CustomerBillingAccount GetEligibleDBRAccount()
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
            List<string> dBRCAs = DBRUtility.Instance.GetCAList();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            CustomerBillingAccount account = new CustomerBillingAccount();
            if (dBRCAs.Count > 0)
            {
                var dbrSelected = dBRCAs.Where(x => x == customerAccount.AccNum).FirstOrDefault();
                if (dbrSelected != string.Empty)
                {
                    account = allAccountList.Where(x => x.AccNum == dbrSelected).FirstOrDefault();
                }
                if (account == null)
                {
                    foreach (var dbrca in dBRCAs)
                    {
                        account = allAccountList.Where(x => x.AccNum == dbrca).FirstOrDefault();
                        if (account != null)
                        {
                            break;
                        }
                    }
                }
            }
            return account;
        }

        public void ShowManageSupplyAccount(AccountData accountData)
        {
            try
            {
                Intent manageAccount = new Intent(this, typeof(ManageSupplyAccountActivityEdit));
                manageAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                //manageAccount.PutExtra(Constants.SELECTED_ACCOUNT_POSITION);
                StartActivityForResult(manageAccount, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowSelectSupplyAccount()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent supplyAccount = new Intent(this, typeof(SelectSupplyAccountActivity));
                StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);

        }

        public void PopulateIdentificationDetails()
        {
            UserEntity user = UserEntity.GetActive();
            //isWhatNewDialogOnHold = false;
            if (UserSessions.HasHomeTutorialShown(this.mPref))
            {
                try
                {
                    if (user.IdentificationNo.Equals(""))
                    {
                        //with check box
                        //Utility.ShowIdentificationUpdateProfileDialog(this, () =>
                        //{
                        //    ShowIdentificationUpdate();
                        //},
                        //() =>
                        //{
                        //    UserSessions.UpdateIdDialog(this.mPref);
                        //},
                        //() =>
                        //{
                        //    this.mPref.Edit().Remove("DialogIDUpdated").Apply();
                        //}
                        //);

                        Utility.ShowIdentificationUpdateProfileDialog(this, () =>
                        {
                            ShowIdentificationUpdate();
                        }
                        );

                        UserSessions.SetUpdateIdPopUp(this.mPref);
                    }
                    else if (UserSessions.MyHomeDashboardTutorialHasShown(this.mPref) && MyHomeUtility.Instance.IsAccountEligible)
                    {
                        bool myHomeMarketingPopUpHasShown = UserSessions.MyHomeMarketingPopUpHasShown(this.mPref);

                        if (!myHomeMarketingPopUpHasShown &&
                            MyHomeUtility.Instance.IsMarketingPopupEnabled &&
                            MyHomeUtility.Instance.IsAccountEligible)
                        {
                            ShowMyHomeMarketingPopUp();
                            UserSessions.SetShownMyHomeMarketingPopUp(this.mPref);
                        }
                        else
                        {
                            LogicCheckForDBRMarketingPopUp();
                        }
                    }
                    else
                    {
                        LogicCheckForDBRMarketingPopUp();
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

            }
            else
            {
                MyTNBAccountManagement.GetInstance().OnHoldWhatNew(true);
                UserSessions.SetUpdateIdDialog(this.mPref);
            }
        }

        private void LogicCheckForDBRMarketingPopUp()
        {
            UserEntity user = UserEntity.GetActive();
            int loginCount = UserLoginCountEntity.GetLoginCount(user.Email);
            bool dbrPopUpHasShown = UserSessions.GetDBRPopUpFlag(this.mPref);

            //GetBillTenantRenderingAsync();

            int countCA = 0;
            bool flagOwner = false;
            List<string> dBRCAs = DBRUtility.Instance.GetCAList();
            List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
            CustomerBillingAccount tenantOwnerInfo = new CustomerBillingAccount();
            List<PostBREligibilityIndicatorsModel> tenantList = TenantDBRCache.Instance.IsTenantDBREligible();


            if (tenantList != null)
            {
                foreach (CustomerBillingAccount item in accounts)
                {
                    if (item.AccountHasOwner == true)
                    {
                        flagOwner = true;
                    }
                }

                for (int j = 0; j < accounts.Count; j++)
                {
                    for (int i = 0; i < tenantList.Count; i++)
                    {
                        if (flagOwner
                            && tenantList[i].CaNo == accounts[j].AccNum
                            && !tenantList[i].IsOwnerOverRule
                            && !tenantList[i].IsOwnerAlreadyOptIn
                            && !tenantList[i].IsTenantAlreadyOptIn)
                        {
                            countCA++;
                        }
                    }

                }

            }

            if (!dbrPopUpHasShown && loginCount == 1 &&
                DBRUtility.Instance.ShouldShowHomeCard &&
                DBRUtility.Instance.IsAccountEligible &&
                CustomerBillingAccount.HasOwnerCA())
            {
                ShowMarketingTooltip();
                UserSessions.SaveDBRPopUpFlag(this.mPref, true);
            }
            else
            {
                if (!dbrPopUpHasShown
                    && loginCount == 1
                    && DBRUtility.Instance.ShouldShowHomeCard
                    && DBRUtility.Instance.IsAccountEligible
                    && countCA > 0)
                {
                    ShowMarketingTooltip();
                    UserSessions.SaveDBRPopUpFlag(this.mPref, true);
                }
                else
                {
                    this.userActionsListener.OnCheckDraftForResume(PreferenceManager.GetDefaultSharedPreferences(this));
                }
            }
        }


        public void PopulateFloatingButton(FloatingButtonModel content)
        {

            if (!string.IsNullOrEmpty(content.Title))
            {
                if (content.Title == Module.DBR.ToString())
                {
                    try
                    {
                        int countCA = 0;
                        bool flagOwner = false;
                        List<string> dBRCAs = DBRUtility.Instance.GetCAList();
                        List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                        CustomerBillingAccount tenantOwnerInfo = new CustomerBillingAccount();
                        List<PostBREligibilityIndicatorsModel> tenantList = TenantDBRCache.Instance.IsTenantDBREligible();

                        if (tenantList != null)
                        {
                            foreach (CustomerBillingAccount item in accounts)
                            {
                                if (item.AccountHasOwner == true)
                                {
                                    flagOwner = true;
                                }
                            }

                            for (int j = 0; j < accounts.Count; j++)
                            {
                                for (int i = 0; i < tenantList.Count; i++)
                                {
                                    if (flagOwner
                                        && tenantList[i].CaNo == accounts[j].AccNum
                                        && !tenantList[i].IsOwnerOverRule
                                        && !tenantList[i].IsOwnerAlreadyOptIn
                                        && !tenantList[i].IsTenantAlreadyOptIn)
                                    {
                                        countCA++;
                                    }
                                }

                            }

                        }

                        if (DBRUtility.Instance.IsAccountEligible &&
                            CustomerBillingAccount.HasOwnerCA())
                        {
                            ShowFloatingButton();
                        }
                        else if (DBRUtility.Instance.IsAccountEligible
                                && countCA > 0)
                        {
                            ShowFloatingButton();
                        }
                        else
                        {
                            HideFloatingButton();
                        }

                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                }
                else if (content.Title == Module.TNG.ToString())
                {
                    if (TNGUtility.Instance.IsAccountEligible)
                    {
                        ShowFloatingButton();
                    }
                    else
                    {
                        HideFloatingButton();
                    }

                }
                else if (content.Title == Module.EB.ToString())
                {
                    if (UserSessions.GetEnergyBudgetList().Count > 0
                        && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                        && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                    {
                        ShowFloatingButton();
                    }
                    else
                    {
                        HideFloatingButton();
                    }
                }
                else if (content.Title == Module.BR.ToString())
                {
                    if (BillRedesignUtility.Instance.IsAccountEligible)
                    {
                        ShowFloatingButton();
                    }
                    else
                    {
                        HideFloatingButton();
                    }
                }
                else if (content.Title == Module.SD.ToString())
                {
                    if (MyTNBAccountManagement.GetInstance().IsSDUserVerify())
                    {
                        ShowFloatingButton();
                    }
                    else
                    {
                        HideFloatingButton();
                    }
                }
                else if (content.Title == Module.WEB.ToString())
                {
                    ShowFloatingButton();
                }
                else
                {
                    HideFloatingButton();
                }

            }
            else
            {
                HideFloatingButton();
            }

        }

        public void ShowMarketingTooltip()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
                    .SetHeaderImage(Resource.Drawable.popup_non_targeted_digital_bill)
                    .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.DASHBOARD_HOME, LanguageConstants.DashboardHome.DBR_REMINDER_POPUP_TITLE))
                    .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.DASHBOARD_HOME, LanguageConstants.DashboardHome.DBR_REMINDER_POPUP_MESSAGE))
                    .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.DASHBOARD_HOME, LanguageConstants.DashboardHome.DBR_REMINDER_POPUP_START_NOW))
                    .SetCTAaction(() => ShowManageBill())
                    .SetSecondaryCTALabel(Utility.GetLocalizedLabel(LanguageConstants.DASHBOARD_HOME, LanguageConstants.DashboardHome.GOT_IT))
                    .SetSecondaryCTATextSize(12)
                    .SetSecondaryCTAaction(() =>
                    {
                        this.SetIsClicked(false);
                        DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Home.Reminder_Popup_GotIt);
                        this.userActionsListener.OnCheckDraftForResume(PreferenceManager.GetDefaultSharedPreferences(this));
                    })
                    .Build();
                marketingTooltip.Show();
            }
        }

        public void ShowMyHomeMarketingPopUp()
        {
            MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
                    .SetHeaderImage(Resource.Drawable.Banner_MyHome_Marketing)
                    .SetTitle(Utility.GetLocalizedLabel("MarketingPopup", MarketingPopUp.MyHome.I18N_Title))
                    .SetMessage(Utility.GetLocalizedLabel("MarketingPopup", MarketingPopUp.MyHome.I18N_Message))
                    .SetCTALabel(Utility.GetLocalizedLabel("MarketingPopup", MarketingPopUp.MyHome.I18N_CTA))
                    .SetCTAaction(() =>
                    {
                        this.SetIsClicked(false);
                        LogicCheckForDBRMarketingPopUp();
                    })
                    .Build();
            marketingTooltip.Show();
        }

        public void ShowManageBill()
        {
            try
            {
                DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Home.Reminder_Popup_Viewmore);
                this.GetBillRendering();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowIdentificationUpdate()
        {
            Intent updateICNo = new Intent(this, typeof(MyProfileActivity));
            //updateICNo.PutExtra("fromDashboard", true);
            StartActivityForResult(updateICNo, Constants.UPDATE_IC_REQUEST);
        }

        public void ShowBillMenu(AccountData selectedAccount, bool isIneligiblePopUpActive = false)
        {
            HideFloatingButton();

            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetChecked(true);
            txtAccountName.Visibility = ViewStates.Gone;
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            currentFragment = ItemisedBillingMenuFragment.NewInstance(selectedAccount, isIneligiblePopUpActive);
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, currentFragment)
                .CommitAllowingStateLoss();
        }

        public void SetToolbarTitle(int stringResourceId)
        {
            try
            {
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = GetString(stringResourceId);
                RemoveHeaderDropDown();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RemoveHeaderDropDown()
        {
            try
            {
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).SetPadding(0, 0, 0, 0);
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).CompoundDrawablePadding = 0;
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                isSetToolbarClick = false;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetAccountToolbarTitle(string accountName)
        {
            try
            {
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = accountName;
                int padding = (int)DPUtils.ConvertDPToPx(3f);
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).SetPadding(padding, padding, padding, padding);
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).CompoundDrawablePadding = padding;
                List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
                bool enableDropDown = accountList.Count > 0 ? true : false;
                if (enableDropDown)
                {
                    Drawable dropdown = ContextCompat.GetDrawable(this, Resource.Drawable.ic_spinner_dropdown);
                    Drawable transparentDropDown = ContextCompat.GetDrawable(this, Resource.Drawable.ic_action_dropdown);
                    transparentDropDown.Alpha = 0;
                    this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).SetCompoundDrawablesWithIntrinsicBounds(transparentDropDown, null, dropdown, null);
                }
                else
                {
                    this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                }
                isSetToolbarClick = true;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void DashboardHomeActivity_Click(object sender, EventArgs e)
        {
            if (isSetToolbarClick)
            {
                this.userActionsListener.SelectSupplyAccount();
            }
        }

        public void EnableDropDown(bool enable)
        {
            if (enable)
            {
                Drawable dropdown = ContextCompat.GetDrawable(this, Resource.Drawable.ic_spinner_dropdown);
                Drawable transparentDropDown = ContextCompat.GetDrawable(this, Resource.Drawable.ic_action_dropdown);
                transparentDropDown.Alpha = 0;
                txtAccountName.SetCompoundDrawablesWithIntrinsicBounds(transparentDropDown, null, dropdown, null);
            }
            else
            {
                txtAccountName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);

            }
        }

        internal void OnTapRefresh()
        {
            this.userActionsListener.OnTapToRefresh();
        }

        public void SetAccountName(string accountName)
        {
            txtAccountName.Text = accountName;
        }

        public void ShowProgressDialog()
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

        public void HideProgressDialog()
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

        public void OnCheckDeeplink()
        {
            if (DeeplinkUtil.Instance.TargetScreen != Deeplink.ScreenEnum.None)
            {
                this.DeeplinkValidation();
            }
        }

        public void OnCheckNotification()
        {
            bool hasNotification = UserSessions.HasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
            string loggedInEmail = UserEntity.GetActive() != null ? UserEntity.GetActive().Email : string.Empty;
            bool isLoggedInEmail = loggedInEmail.Equals(UserSessions.GetUserEmailNotification(PreferenceManager.GetDefaultSharedPreferences(this)), StringComparison.OrdinalIgnoreCase);
            if (hasNotification &&
                isLoggedInEmail &&
                NotificationUtil.Instance.IsDirectPush)
            {
                this.NotificationValidation();
            }
            else
            {
                NotificationUtil.Instance.ClearData();
            }
            UserSessions.RemoveNotificationSession(PreferenceManager.GetDefaultSharedPreferences(this));
        }

        public void ShowFeedbackMenu()
        {
            HideFloatingButton();
            ShowBackButton(false);
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            currentFragment = new FeedbackMenuFragment();
            SupportFragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, currentFragment)
                     .CommitAllowingStateLoss();
        }

        public void ShowWhatsNewMenu()
        {
            HideFloatingButton();
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            currentFragment = new WhatsNewMenuFragment();
            SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_layout, currentFragment)
                        .CommitAllowingStateLoss();

        }

        public void ShowRewardsMenu()
        {
            HideFloatingButton();
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            currentFragment = new RewardMenuFragment();
            SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_layout, currentFragment)
                        .CommitAllowingStateLoss();

        }

        public void ShowMoreMenu()
        {
            HideFloatingButton();
            //ProfileMenuFragment profileMenuFragment = new ProfileMenuFragment();
            ProfileMainMenuFragment profileMenuFragment = new ProfileMainMenuFragment();

            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            if (mobileNoUpdated)
            {
                Bundle extras = new Bundle();
                extras.PutBoolean(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
                profileMenuFragment.Arguments = extras;
                mobileNoUpdated = false;
            }
            currentFragment = profileMenuFragment;
            SupportFragmentManager.BeginTransaction()
                     .Replace(Resource.Id.content_layout, currentFragment)
                     .CommitAllowingStateLoss();
        }

        public void Logout()
        {
            this.userActionsListener.Logout();
        }

        public void HideFloatingButton()
        {
            floatingButtonLayout.Visibility = ViewStates.Gone;
            hideButtonLayout.Visibility = ViewStates.Gone;
        }

        public void ShowFloatingButton()
        {
            floatingButtonLayout.Visibility = ViewStates.Visible;
            hideButtonLayout.Visibility = ViewStates.Visible;
        }

        public void HideAccountName()
        {
            txtAccountName.Visibility = ViewStates.Gone;
        }

        public void ShowAccountName()
        {
            txtAccountName.Visibility = ViewStates.Visible;
        }

        public void OnFinish()
        {
            this.Finish();
        }

        public override bool TelephonyPermissionRequired()
        {
            return false;
        }

        public override void Ready()
        {
            base.Ready();
        }

        public void EnableBillMenu()
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetEnabled(true);
        }

        public void DisableBillMenu()
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetEnabled(false);
        }

        public void ShowREAccount(Boolean enable)
        {
            Drawable leafIcon = ContextCompat.GetDrawable(this, Resource.Drawable.ic_display_RE_Dashboard);
            leafIcon.Alpha = 255;
            if (enable)
            {
                Drawable dropdown = ContextCompat.GetDrawable(this, Resource.Drawable.ic_spinner_dropdown);

                txtAccountName.SetCompoundDrawablesWithIntrinsicBounds(leafIcon, null, dropdown, null);
                txtAccountName.CompoundDrawablePadding = 10;
            }
            else
            {
                txtAccountName.SetCompoundDrawablesWithIntrinsicBounds(leafIcon, null, null, null);
                txtAccountName.CompoundDrawablePadding = 10;
            }
        }

        public void ShowUnreadWhatsNew(bool flag)
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    if (UserSessions.HasWhatNewShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        int count = WhatsNewEntity.Count();
                        if (count > 0)
                        {
                            SetReadUnReadNewBottomView(flag, true, count, promotionMenuItem);
                        }
                        else
                        {
                            HideUnreadWhatsNew(flag);
                        }
                    }
                    else
                    {
                        SetNewWhatsNewBottomView(flag, Utility.GetLocalizedCommonLabel("new"), promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void ShowUnreadWhatsNew()
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    if (UserSessions.HasWhatNewShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        int count = WhatsNewEntity.Count();
                        if (count > 0)
                        {
                            SetReadUnReadNewBottomView(promotionMenuItem.IsChecked, true, count, promotionMenuItem);
                        }
                        else
                        {
                            HideUnreadWhatsNew(promotionMenuItem.IsChecked);
                        }
                    }
                    else
                    {
                        SetNewWhatsNewBottomView(promotionMenuItem.IsChecked, Utility.GetLocalizedCommonLabel("new"), promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void HideUnreadWhatsNew(bool flag)
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    if (UserSessions.HasWhatNewShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        SetReadUnReadNewBottomView(flag, false, 0, promotionMenuItem);
                    }
                    else
                    {
                        SetNewWhatsNewBottomView(flag, Utility.GetLocalizedCommonLabel("new"), promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void HideUnreadWhatsNew()
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    if (UserSessions.HasWhatNewShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        SetReadUnReadNewBottomView(promotionMenuItem.IsChecked, false, 0, promotionMenuItem);
                    }
                    else
                    {
                        SetNewWhatsNewBottomView(promotionMenuItem.IsChecked, Utility.GetLocalizedCommonLabel("new"), promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void HideUnverifiedProfile(bool keypress, bool isfromHome)
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem profileMenuItem = bottomMenu.FindItem(Resource.Id.menu_more);
                if (profileMenuItem != null)
                {
                    SetVerifiedMenuMoreBottomView(keypress, isfromHome, 0, profileMenuItem);
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void ShowUnverifiedProfile(bool keypress, bool isfromHome)
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem profileMenuItem = bottomMenu.FindItem(Resource.Id.menu_more);
                if (profileMenuItem != null)
                {
                    SetUnverifiedMenuMoreBottomView(keypress, isfromHome, 0, profileMenuItem);
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void SetUnverifiedMenuMoreBottomView(bool keypress, bool isfromHome, int count, IMenuItem profileMenuItem)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayoutProfile, null, false);
                        ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                        RelativeLayout.LayoutParams bottomImgParam = bottomImg.LayoutParameters as RelativeLayout.LayoutParams;
                        if (keypress)
                        {

                            if (isfromHome)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.profile_unverified);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_more_toggled);
                            }
                        }
                        else
                        {
                            if (isfromHome)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.profile_unverified);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_more_toggled);
                            }
                        }
                        int specWidth = MeasureSpec.MakeMeasureSpec(0 /* any */, MeasureSpecMode.Unspecified);
                        v.Measure(specWidth, specWidth);
                        Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                        Canvas c = new Canvas(b);
                        v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f));
                        v.Draw(c);

                        var bitmapDrawable = new BitmapDrawable(b);
                        profileMenuItem.SetIcon(bitmapDrawable);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void SetVerifiedMenuMoreBottomView(bool flag, bool Indicator, int count, IMenuItem profileMenuItem)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayoutProfile, null, false);
                        ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                        RelativeLayout.LayoutParams bottomImgParam = bottomImg.LayoutParameters as RelativeLayout.LayoutParams;
                        if (Indicator)
                        {

                            if (!flag)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_more);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_more_toggled);
                            }
                        }
                        else
                        {
                            if (!flag)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_more);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_more_toggled);
                            }
                        }
                        int specWidth = MeasureSpec.MakeMeasureSpec(0 /* any */, MeasureSpecMode.Unspecified);
                        v.Measure(specWidth, specWidth);
                        Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                        Canvas c = new Canvas(b);
                        v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f));
                        v.Draw(c);

                        var bitmapDrawable = new BitmapDrawable(b);
                        profileMenuItem.SetIcon(bitmapDrawable);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void SetReadUnReadNewBottomView(bool flag, bool isGotRead, int count, IMenuItem promotionMenuItem)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayout, null, false);
                        LinearLayout newLabel = v.FindViewById<LinearLayout>(Resource.Id.newLabel);
                        TextView txtNewLabel = v.FindViewById<TextView>(Resource.Id.txtNewLabel);
                        ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                        if (isGotRead && count > 0)
                        {
                            newLabel.Visibility = ViewStates.Visible;
                            newLabel.SetBackgroundResource(Resource.Drawable.bottom_indication_bg);
                            TextViewUtils.SetMuseoSans500Typeface(txtNewLabel);
                            RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                            RelativeLayout.LayoutParams bottomImgParam = bottomImg.LayoutParameters as RelativeLayout.LayoutParams;
                            newLabelParam.TopMargin = 10;
                            newLabelParam.Height = (int)DPUtils.ConvertDPToPx(16f);
                            bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(10f);
                            txtNewLabel.SetTextSize(Android.Util.ComplexUnitType.Dip, 10f);
                            txtNewLabel.Text = count.ToString();
                            TextViewUtils.SetTextSize8(txtNewLabel);
                            txtNewLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.white)));
                            newLabelParam.LeftMargin = (int)DPUtils.ConvertDPToPx(-3f);
                            if (count > 0 && count <= 9)
                            {
                                newLabelParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            }
                            else
                            {
                                bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(14f);
                                if (count > 99)
                                {
                                    txtNewLabel.Text = "99+";
                                }
                                newLabelParam.Width = (int)DPUtils.ConvertDPToPx(22f);
                            }

                            if (!flag)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo_toggled);
                            }
                            newLabel.SetPadding(0, 0, 0, 0);
                        }
                        else
                        {
                            newLabel.Visibility = ViewStates.Gone;
                            if (!flag)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo_toggled);
                            }
                        }
                        int specWidth = MeasureSpec.MakeMeasureSpec(0 /* any */, MeasureSpecMode.Unspecified);
                        v.Measure(specWidth, specWidth);
                        Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                        Canvas c = new Canvas(b);
                        v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f));
                        v.Draw(c);

                        var bitmapDrawable = new BitmapDrawable(b);
                        promotionMenuItem.SetIcon(bitmapDrawable);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void SetNewWhatsNewBottomView(bool flag, string word, IMenuItem promotionMenuItem)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayout, null, false);
                        LinearLayout newLabel = v.FindViewById<LinearLayout>(Resource.Id.newLabel);
                        TextView txtNewLabel = v.FindViewById<TextView>(Resource.Id.txtNewLabel);
                        ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                        newLabel.Visibility = ViewStates.Visible;
                        newLabel.SetBackgroundResource(Resource.Drawable.new_label);
                        RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                        RelativeLayout.LayoutParams bottomImgParam = bottomImg.LayoutParameters as RelativeLayout.LayoutParams;
                        newLabelParam.LeftMargin = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts
                            && LanguageUtil.GetAppLanguage().ToUpper() != Constants.DEFAULT_LANG ? -40 : -25f);
                        newLabelParam.Height = (int)DPUtils.ConvertDPToPx(14f);
                        newLabelParam.TopMargin = 10;
                        newLabelParam.Width = ViewGroup.LayoutParams.WrapContent;
                        newLabel.SetPadding((int)DPUtils.ConvertDPToPx(6f), 0, (int)DPUtils.ConvertDPToPx(6f), 0);
                        if (LanguageUtil.GetAppLanguage().ToUpper() == Constants.DEFAULT_LANG)
                        {
                            bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(10f);
                        }
                        else
                        {
                            bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(20f);
                        }

                        txtNewLabel.SetTextSize(Android.Util.ComplexUnitType.Dip, 8f);
                        txtNewLabel.Text = word;
                        TextViewUtils.SetTextSize8(txtNewLabel);
                        txtNewLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                        TextViewUtils.SetMuseoSans500Typeface(txtNewLabel);
                        if (!flag)
                        {
                            bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo);
                        }
                        else
                        {
                            bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo_toggled);
                        }
                        int specWidth = MeasureSpec.MakeMeasureSpec(0 /* any */, MeasureSpecMode.Unspecified);
                        v.Measure(specWidth, specWidth);
                        if (LanguageUtil.GetAppLanguage().ToUpper() == Constants.DEFAULT_LANG)
                        {
                            Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(50f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                            Canvas c = new Canvas(b);
                            v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(50f), (int)DPUtils.ConvertDPToPx(28f));
                            v.Draw(c);

                            var bitmapDrawable = new BitmapDrawable(b);
                            promotionMenuItem.SetIcon(bitmapDrawable);
                        }
                        else
                        {
                            Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(70f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                            Canvas c = new Canvas(b);
                            v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(70f), (int)DPUtils.ConvertDPToPx(28f));
                            v.Draw(c);

                            var bitmapDrawable = new BitmapDrawable(b);
                            promotionMenuItem.SetIcon(bitmapDrawable);
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void SetNewRewardsBottomView(bool flag, string word, IMenuItem rewardMenuItem)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayout, null, false);
                        LinearLayout newLabel = v.FindViewById<LinearLayout>(Resource.Id.newLabel);
                        TextView txtNewLabel = v.FindViewById<TextView>(Resource.Id.txtNewLabel);
                        ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                        newLabel.Visibility = ViewStates.Visible;
                        newLabel.SetBackgroundResource(Resource.Drawable.new_label);
                        RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                        RelativeLayout.LayoutParams bottomImgParam = bottomImg.LayoutParameters as RelativeLayout.LayoutParams;
                        newLabelParam.LeftMargin = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts
                            && LanguageUtil.GetAppLanguage().ToUpper() != Constants.DEFAULT_LANG ? -40 : -25f);
                        newLabelParam.Height = (int)DPUtils.ConvertDPToPx(14f);
                        newLabelParam.TopMargin = 10;
                        newLabelParam.Width = ViewGroup.LayoutParams.WrapContent;
                        newLabel.SetPadding((int)DPUtils.ConvertDPToPx(4f), 0, (int)DPUtils.ConvertDPToPx(4f), 0);
                        if (LanguageUtil.GetAppLanguage().ToUpper() == Constants.DEFAULT_LANG)
                        {
                            bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(10f);
                        }
                        else
                        {
                            bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(20f);
                        }

                        txtNewLabel.SetTextSize(Android.Util.ComplexUnitType.Dip, 8f);
                        txtNewLabel.Text = word;
                        TextViewUtils.SetTextSize8(txtNewLabel);
                        txtNewLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                        TextViewUtils.SetMuseoSans500Typeface(txtNewLabel);
                        if (!flag)
                        {
                            bottomImg.SetImageResource(Resource.Drawable.ic_menu_reward);
                        }
                        else
                        {
                            bottomImg.SetImageResource(Resource.Drawable.ic_menu_reward_toggled);
                        }
                        int specWidth = MeasureSpec.MakeMeasureSpec(0 /* any */, MeasureSpecMode.Unspecified);
                        v.Measure(specWidth, specWidth);
                        if (LanguageUtil.GetAppLanguage().ToUpper() == Constants.DEFAULT_LANG)
                        {
                            Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(50f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                            Canvas c = new Canvas(b);
                            v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(50f), (int)DPUtils.ConvertDPToPx(28f));
                            v.Draw(c);

                            var bitmapDrawable = new BitmapDrawable(b);
                            rewardMenuItem.SetIcon(bitmapDrawable);
                        }
                        else
                        {
                            Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(70f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                            Canvas c = new Canvas(b);
                            v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(70f), (int)DPUtils.ConvertDPToPx(28f));
                            v.Draw(c);

                            var bitmapDrawable = new BitmapDrawable(b);
                            rewardMenuItem.SetIcon(bitmapDrawable);
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void SetReadUnReadRewardNewBottomView(bool flag, bool isGotRead, int count, IMenuItem rewardsMenuItem)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayout, null, false);
                        LinearLayout newLabel = v.FindViewById<LinearLayout>(Resource.Id.newLabel);
                        TextView txtNewLabel = v.FindViewById<TextView>(Resource.Id.txtNewLabel);
                        ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                        if (isGotRead && count > 0)
                        {
                            newLabel.Visibility = ViewStates.Visible;
                            newLabel.SetBackgroundResource(Resource.Drawable.bottom_indication_bg);
                            TextViewUtils.SetMuseoSans500Typeface(txtNewLabel);
                            RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                            RelativeLayout.LayoutParams bottomImgParam = bottomImg.LayoutParameters as RelativeLayout.LayoutParams;
                            newLabelParam.TopMargin = 10;
                            newLabelParam.Height = (int)DPUtils.ConvertDPToPx(16f);
                            bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(10f);
                            txtNewLabel.SetTextSize(Android.Util.ComplexUnitType.Dip, 10f);
                            txtNewLabel.Text = count.ToString();
                            TextViewUtils.SetTextSize8(txtNewLabel);
                            txtNewLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.white)));
                            newLabelParam.LeftMargin = (int)DPUtils.ConvertDPToPx(-3f);
                            if (count > 0 && count <= 9)
                            {
                                newLabelParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            }
                            else
                            {
                                bottomImgParam.LeftMargin = (int)DPUtils.ConvertDPToPx(14f);
                                if (count > 99)
                                {
                                    txtNewLabel.Text = "99+";
                                }
                                newLabelParam.Width = (int)DPUtils.ConvertDPToPx(22f);
                            }

                            if (!flag)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_reward);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_reward_toggled);
                            }
                            newLabel.SetPadding(0, 0, 0, 0);
                        }
                        else
                        {
                            newLabel.Visibility = ViewStates.Gone;
                            if (!flag)
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_reward);
                            }
                            else
                            {
                                bottomImg.SetImageResource(Resource.Drawable.ic_menu_reward_toggled);
                            }
                        }
                        int specWidth = MeasureSpec.MakeMeasureSpec(0 /* any */, MeasureSpecMode.Unspecified);
                        v.Measure(specWidth, specWidth);
                        Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                        Canvas c = new Canvas(b);
                        v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(65f), (int)DPUtils.ConvertDPToPx(28f));
                        v.Draw(c);

                        var bitmapDrawable = new BitmapDrawable(b);
                        rewardsMenuItem.SetIcon(bitmapDrawable);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void NavigateToDashBoardFragment()
        {
            mPresenter.OnAccountSelectDashBoard();
        }

        public void ShowHomeDashBoard()
        {
            DashboardHomeActivity.GO_TO_INNER_DASHBOARD = false;
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            currentFragment = new HomeMenuFragment();
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, currentFragment)
                .CommitAllowingStateLoss();

            if (MyTNBAccountManagement.GetInstance().IsMaybeLaterFlag())
            {
                isWhatNewDialogOnHold = true;
            }

            if (isWhatNewDialogOnHold)
            {
                isWhatNewDialogOnHold = false;
                OnCheckWhatsNewTab();
            }

            UserEntity user = UserEntity.GetActive();

            if (UserSessions.HasHomeTutorialShown(this.mPref) && UserSessions.GetUpdateIdPopUp(this.mPref))
            {
                int loginCount = UserLoginCountEntity.GetLoginCount(user.Email);
                bool dbrPopUpHasShown = UserSessions.GetDBRPopUpFlag(this.mPref);
                bool popupID = UserSessions.GetUpdateIdPopUp(this.mPref);

                bool myHomeMarketingPopUpHasShown = UserSessions.MyHomeMarketingPopUpHasShown(this.mPref);

                if (!myHomeMarketingPopUpHasShown &&
                    popupID &&
                    MyHomeUtility.Instance.IsMarketingPopupEnabled &&
                    MyHomeUtility.Instance.IsAccountEligible)
                {
                    ShowMyHomeMarketingPopUp();
                    UserSessions.SetShownMyHomeMarketingPopUp(this.mPref);
                }
                else if (!dbrPopUpHasShown && loginCount == 1 && DBRUtility.Instance.ShouldShowHomeCard && popupID)
                {
                    ShowMarketingTooltip();
                    UserSessions.SaveDBRPopUpFlag(this.mPref, true);
                }
                else
                {
                    this.userActionsListener.OnCheckDraftForResume(PreferenceManager.GetDefaultSharedPreferences(this));
                }
            }

            //var sharedpref_data = UserSessions.GetCheckEmailVerified(this.mPref);
            //bool isUpdatePersonalDetail = bool.Parse(sharedpref_data);  //get from shared pref

            if (isFromLogin)
            {
                isFromLogin = false;
                UserSessions.SaveCheckEmailVerified(this.mPref, user.IsActivated.ToString());  //save sharedpref check email  //wan
                if (string.IsNullOrEmpty(user.IdentificationNo) || !user.IsActivated)
                {
                    isFromHomeMenu = true;
                    OnCheckProfileTab(true, isFromHomeMenu);
                }
            }
            //else if (string.IsNullOrEmpty(user.IdentificationNo) || !isUpdatePersonalDetail)
            else if (string.IsNullOrEmpty(user.IdentificationNo))
            {
                isFromHomeMenu = true;
                OnCheckProfileTab(true, isFromHomeMenu);
            }
            else
            {
                isFromHomeMenu = true;
                OnCheckProfileTab(false, isFromHomeMenu);
            }

            OnSetupFloatingButton();

            if (FloatingButtonUtils.GetFloatingButton() != null)
            {
                if (UserSessions.GetLanguageFBFlag(PreferenceManager.GetDefaultSharedPreferences(this)))
                {
                    this.userActionsListener.OnGetFloatingButtonItem();
                    UserSessions.UpdateLanguageFBFlag(PreferenceManager.GetDefaultSharedPreferences(this));
                }

            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void SetDashboardHomeCheck()
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_dashboard).SetChecked(true);
        }

        public void SetMenuMoreCheck()
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_more).SetChecked(true);
        }

        public void ShowToBeAddedToast()
        {
            Toast.MakeText(this, "Stay Tune!", ToastLength.Long).Show();
        }

        public void ShowHideActionBar(bool flag)
        {
            if (flag)
            {
                this.SupportActionBar.Show();
            }
            else
            {
                this.SupportActionBar.Hide();
            }
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            try
            {
                if (ev.Action == MotionEventActions.Down
                    && this.userActionsListener?.CheckCurrentDashboardMenu() == Resource.Id.menu_dashboard
                    && currentFragment.GetType() == typeof(HomeMenuFragment))
                {
                    View view = CurrentFocus;
                    if (view != null && view.GetType() != typeof(EditText))
                    {
                        Rect rect = new Rect();
                        view.GetGlobalVisibleRect(rect);
                        if (!rect.Contains((int)ev.RawX, (int)ev.RawY))
                        {
                            HomeMenuFragment fragment = (HomeMenuFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                            LinearLayout searchContainer = fragment.GetSearchLayout();
                            if (IsViewInBounds(searchContainer, (int)ev.RawX, (int)ev.RawY))
                            {
                                fragment.OnSearchOutFocus(true);
                            }
                            else
                            {
                                fragment.OnSearchClearFocus();
                            }
                        }
                    }
                }
                else if (ev.Action == MotionEventActions.Down
                    && this.userActionsListener?.CheckCurrentDashboardMenu() == Resource.Id.menu_dashboard
                    && currentFragment.GetType() == typeof(DashboardChartFragment))
                {
                    DashboardChartFragment fragment = (DashboardChartFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                    TextView kwhLabel = fragment.GetkwhLabel();
                    TextView rmLabel = fragment.GetRmLabel();
                    LinearLayout rmKwhSelection = fragment.GetRmKwhSelection();
                    int x = (int)ev.RawX;
                    int y = (int)ev.RawY;
                    if (!IsViewInBounds(kwhLabel, x, y) && !IsViewInBounds(rmLabel, x, y) && !IsViewInBounds(rmKwhSelection, x, y))
                    {
                        fragment.CheckRMKwhSelectDropDown();
                    }
                }
                else if (ev.Action == MotionEventActions.Down
                    && this.userActionsListener?.CheckCurrentDashboardMenu() == Resource.Id.menu_reward
                    && currentFragment.GetType() == typeof(RewardMenuFragment))
                {
                    if (RewardsMenuUtils.GetTouchDisable())
                    {
                        int x = (int)ev.RawX;
                        int y = (int)ev.RawY;
                        if (!IsViewInBounds(bottomNavigationView, x, y))
                        {
                            return true;
                        }
                    }
                }
                else if (ev.Action == MotionEventActions.Down
                    && this.userActionsListener?.CheckCurrentDashboardMenu() == Resource.Id.menu_promotion
                    && currentFragment.GetType() == typeof(WhatsNewMenuFragment))
                {
                    if (WhatsNewMenuUtils.GetTouchDisable())
                    {
                        int x = (int)ev.RawX;
                        int y = (int)ev.RawY;
                        if (!IsViewInBounds(bottomNavigationView, x, y))
                        {
                            return true;
                        }
                    }
                }
                return base.DispatchTouchEvent(ev);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return false;
        }

        private bool IsViewInBounds(View view, int x, int y)
        {
            Rect outRect = new Rect();
            int[] location = new int[2];

            view.GetDrawingRect(outRect);
            view.GetLocationOnScreen(location);
            outRect.Offset(location[0], location[1]);
            return outRect.Contains(x, y);
        }

        public void SetInnerDashboardToolbarBackground()
        {
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
        }

        public void UnsetToolbarBackground()
        {
            RemoveToolbarBackground();
        }

        public void OnSelectAccount()
        {

            this.userActionsListener.SelectSupplyAccount();
        }

        public void ShowNMREChart(UsageHistoryResponse response, AccountData selectedAccount, string errorCode, string errorMsg)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Visibility = ViewStates.Gone;
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            currentFragment = DashboardChartFragment.NewInstance(response, selectedAccount, errorCode, errorMsg);
            SupportFragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, currentFragment,
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
        }

        public void ShowSMChart(SMUsageHistoryResponse response, AccountData selectedAccount)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Visibility = ViewStates.Gone;
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }
            currentFragment = DashboardChartFragment.NewInstance(response, selectedAccount);
            SupportFragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, currentFragment,
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
        }

        // Show Bottom Navigation Bar in Fragment
        public void ShowBottomNavigationBar()
        {
            try
            {
                bottomNavigationView.Visibility = ViewStates.Visible;

                ViewGroup.MarginLayoutParams lp3 = (ViewGroup.MarginLayoutParams)contentLayout.LayoutParameters;

                lp3.BottomMargin = (int)DPUtils.ConvertDPToPx(48f);

                contentLayout.LayoutParameters = lp3;

                contentLayout.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        // Hide Bottom Navigation Bar in Fragment
        public void HideBottomNavigationBar()
        {
            try
            {
                bottomNavigationView.Visibility = ViewStates.Gone;

                ViewGroup.MarginLayoutParams lp3 = (ViewGroup.MarginLayoutParams)contentLayout.LayoutParameters;

                lp3.BottomMargin = 0;

                contentLayout.LayoutParameters = lp3;

                contentLayout.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void ShowUnreadRewards()
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem rewardMenuItem = bottomMenu.FindItem(Resource.Id.menu_reward);
                if (rewardMenuItem != null)
                {
                    if (UserSessions.HasRewardShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        int count = RewardsEntity.Count();
                        if (count > 0)
                        {
                            SetReadUnReadRewardNewBottomView(rewardMenuItem.IsChecked, true, count, rewardMenuItem);
                        }
                        else
                        {
                            HideUnreadRewards(rewardMenuItem.IsChecked);
                        }
                    }
                    else
                    {
                        SetNewRewardsBottomView(rewardMenuItem.IsChecked, Utility.GetLocalizedCommonLabel("new"), rewardMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void ShowUnreadRewards(bool flag)
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem rewardMenuItem = bottomMenu.FindItem(Resource.Id.menu_reward);
                if (rewardMenuItem != null)
                {
                    if (UserSessions.HasRewardShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        int count = RewardsEntity.Count();
                        if (count > 0)
                        {
                            SetReadUnReadRewardNewBottomView(flag, true, count, rewardMenuItem);
                        }
                        else
                        {
                            HideUnreadRewards(flag);
                        }
                    }
                    else
                    {
                        SetNewRewardsBottomView(flag, Utility.GetLocalizedCommonLabel("new"), rewardMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void HideUnreadRewards()
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem rewardMenuItem = bottomMenu.FindItem(Resource.Id.menu_reward);
                if (rewardMenuItem != null)
                {
                    if (UserSessions.HasRewardShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        SetReadUnReadRewardNewBottomView(rewardMenuItem.IsChecked, false, 0, rewardMenuItem);
                    }
                    else
                    {
                        SetNewRewardsBottomView(rewardMenuItem.IsChecked, Utility.GetLocalizedCommonLabel("new"), rewardMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void HideUnreadRewards(bool flag)
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem rewardMenuItem = bottomMenu.FindItem(Resource.Id.menu_reward);
                if (rewardMenuItem != null)
                {
                    if (UserSessions.HasRewardShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        SetReadUnReadRewardNewBottomView(flag, false, 0, rewardMenuItem);
                    }
                    else
                    {
                        SetNewRewardsBottomView(flag, Utility.GetLocalizedCommonLabel("new"), rewardMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void OnCheckUserReward(bool isSitecoreApiFailed)
        {
            try
            {
                if (isSitecoreApiFailed)
                {
                    RewardsParentEntity RewardsParentEntityManager = new RewardsParentEntity();
                    RewardsParentEntityManager.DeleteTable();
                    RewardsParentEntityManager.CreateTable();
                    RewardsMenuUtils.OnSetRewardLoading(false);
                    try
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                HideProgressDialog();
                                if (DeeplinkUtil.Instance.TargetScreen == Deeplink.ScreenEnum.Rewards)
                                {
                                    DeeplinkUtil.Instance.ClearDeeplinkData();
                                    ShowRewardFailedTooltip();
                                }
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }
                else
                {
                    try
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                _ = this.mPresenter.OnGetUserRewardList();
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }
            }
            catch (Exception e)
            {
                HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCheckUserWhatsNew(bool isSitecoreApiFailed)
        {
            try
            {
                if (isSitecoreApiFailed)
                {
                    WhatsNewParentEntity WhatsNewParentEntityManager = new WhatsNewParentEntity();
                    WhatsNewParentEntityManager.DeleteTable();
                    WhatsNewParentEntityManager.CreateTable();
                    WhatsNewMenuUtils.OnSetWhatsNewLoading(false);
                    try
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                HideProgressDialog();
                                MyTNBAccountManagement.GetInstance().SetMaybeLater(false);
                                //this.mPresenter.CheckWhatsNewCache();
                                if (DeeplinkUtil.Instance.TargetScreen == Deeplink.ScreenEnum.WhatsNew)
                                {
                                    DeeplinkUtil.Instance.ClearDeeplinkData();
                                    ShowWhatsNewFailedTooltip();
                                }
                                else
                                {
                                    DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                                    if (bcrmEntity != null && bcrmEntity.IsDown && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                                    {
                                        OnShowBCRMPopup(bcrmEntity);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }
                else
                {
                    try
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                MyTNBAccountManagement.GetInstance().SetMaybeLater(false);
                                this.mPresenter.CheckWhatsNewCache();
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }
            }
            catch (Exception e)
            {
                HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCheckUserRewardApiFailed()
        {
            try
            {
                RewardsMenuUtils.OnSetRewardLoading(false);
                if (DeeplinkUtil.Instance.TargetScreen == Deeplink.ScreenEnum.Rewards)
                {
                    HideProgressDialog();
                    DeeplinkUtil.Instance.ClearDeeplinkData();
                    ShowRewardFailedTooltip();
                }
            }
            catch (Exception e)
            {
                HideProgressDialog();
                RewardsMenuUtils.OnSetRewardLoading(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCheckRewardTab()
        {
            try
            {
                RewardsMenuUtils.OnSetRewardLoading(false);

                if (this.mPresenter != null)
                {
                    this.mPresenter.OnResumeUpdateRewardUnRead();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (DeeplinkUtil.Instance.TargetScreen == Deeplink.ScreenEnum.Rewards)
                {
                    HideProgressDialog();
                    string rewardID = DeeplinkUtil.Instance.ScreenKey;
                    DeeplinkUtil.Instance.ClearDeeplinkData();
                    if (rewardID.IsValid())
                    {
                        rewardID = "{" + rewardID + "}";
                        RewardsEntity wtManager = new RewardsEntity();
                        RewardsEntity item = wtManager.GetItem(rewardID);

                        if (item != null)
                        {
                            if (!item.Read)
                            {
                                this.mPresenter.UpdateRewardRead(item.ID, true);
                            }

                            Intent activity = new Intent(this, typeof(RewardDetailActivity));
                            activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, rewardID);
                            activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "rewards"));
                            StartActivity(activity);
                        }
                        else
                        {
                            IsRootTutorialShown = true;

                            bool isExpired = wtManager.CheckIsExpired(rewardID);
                            if (!isExpired)
                            {
                                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                    .SetTitle(Utility.GetLocalizedLabel("Error", "usedRewardTitle"))
                                    .SetMessage(Utility.GetLocalizedLabel("Error", "usedRewardMsg"))
                                    .SetCTALabel(Utility.GetLocalizedLabel("Common", "showMoreRewards"))
                                    .SetCTAaction(() =>
                                    {
                                        IsRootTutorialShown = false;
                                        OnSelectReward();
                                    })
                                    .Build().Show();
                            }
                            else
                            {
                                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                    .SetTitle(Utility.GetLocalizedLabel("Common", "rewardNotAvailableTitle"))
                                    .SetMessage(Utility.GetLocalizedLabel("Common", "rewardNotAvailableDesc"))
                                    .SetCTALabel(Utility.GetLocalizedLabel("Common", "showMoreRewards"))
                                    .SetCTAaction(() =>
                                    {
                                        IsRootTutorialShown = false;
                                        OnSelectReward();
                                    })
                                    .Build().Show();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCheckProfileTab(bool key, bool isfromHomeMenu)
        {
            try
            {
                //WhatsNewMenuUtils.OnSetWhatsNewLoading(false);

                if (this.mPresenter != null)
                {
                    this.mPresenter.OnResumeUpdateProfileUnRead(key, isfromHomeMenu);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCheckWhatsNewTab()
        {
            try
            {
                WhatsNewMenuUtils.OnSetWhatsNewLoading(false);

                if (this.mPresenter != null)
                {
                    this.mPresenter.OnResumeUpdateWhatsNewUnRead();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                DownTimeEntity bcrmEntity = null;

                try
                {
                    bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                }
                catch (Exception exe)
                {
                    Utility.LoggingNonFatalError(exe);
                }

                if (DeeplinkUtil.Instance.TargetScreen == Deeplink.ScreenEnum.WhatsNew)
                {
                    HideProgressDialog();
                    string whatsNewID = DeeplinkUtil.Instance.ScreenKey;
                    DeeplinkUtil.Instance.ClearDeeplinkData();
                    if (!string.IsNullOrEmpty(whatsNewID))
                    {
                        whatsNewID = "{" + whatsNewID + "}";
                        WhatsNewEntity wtManager = new WhatsNewEntity();
                        WhatsNewEntity item = wtManager.GetItem(whatsNewID);

                        if (item != null)
                        {
                            if (!item.Read)
                            {
                                this.mPresenter.UpdateWhatsNewRead(item.ID, true);
                            }

                            Intent activity = new Intent(this, typeof(WhatsNewDetailActivity));
                            activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, whatsNewID);
                            activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                            StartActivity(activity);
                        }
                        else
                        {
                            IsRootTutorialShown = true;
                            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(Utility.GetLocalizedLabel("Error", "whatsNewExpiredTitle"))
                            .SetMessage(Utility.GetLocalizedLabel("Error", "whatsNewExpiredMsg"))
                            .SetCTALabel(Utility.GetLocalizedLabel("Error", "whatsNewExpiredBtnText"))
                            .SetCTAaction(() =>
                            {
                                IsRootTutorialShown = false;
                                OnSelectWhatsNew();
                            })
                            .Build().Show();
                        }
                    }
                }
                else if (this.mPresenter.GetIsWhatsNewDialogShowNeed() && currentFragment.GetType() == typeof(HomeMenuFragment))
                {
                    HomeMenuFragment fragment = (HomeMenuFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                    bool flag = fragment.GetHomeTutorialCallState();
                    if (MyTNBAccountManagement.GetInstance().IsMaybeLaterFlag() || MyTNBAccountManagement.GetInstance().IsOnHoldWhatNew())
                    {
                        MyTNBAccountManagement.GetInstance().OnHoldWhatNew(false);
                        MyTNBAccountManagement.GetInstance().SetMaybeLater(false);
                        flag = true;
                    }
                    else if (SetEligibleEBUser())
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        MyTNBAccountManagement.GetInstance().SetFromLoginPage(false);
                        this.mPresenter.SetIsWhatsNewDialogShowNeed(false);
                        WhatsNewEntity wtManager = new WhatsNewEntity();
                        List<WhatsNewEntity> items = wtManager.GetActivePopupItems(
                            );
                        if (items != null && items.Count > 0)
                        {
                            List<WhatsNewEntity> MaintenancePopupItems = items.FindAll(x => x.Donot_Show_In_WhatsNew);

                            if (MaintenancePopupItems != null && MaintenancePopupItems.Count > 0)
                            {
                                List<WhatsNewEntity> FilteredMaintenancePopupItems = new List<WhatsNewEntity>();
                                WhatsNewEntity FilteredItem = new WhatsNewEntity();

                                for (int i = 0; i < MaintenancePopupItems.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        FilteredItem = MaintenancePopupItems[i];
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(MaintenancePopupItems[i].PublishDate) && !string.IsNullOrEmpty(FilteredItem.PublishDate))
                                            {
                                                DateTime showDateTime = DateTime.ParseExact(FilteredItem.PublishDate, "yyyyMMddTHHmmss",
                                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                                DateTime compareDateTime = DateTime.ParseExact(MaintenancePopupItems[i].PublishDate, "yyyyMMddTHHmmss",
                                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                                int result = DateTime.Compare(compareDateTime, showDateTime);
                                                if (result > 0)
                                                {
                                                    FilteredItem = MaintenancePopupItems[i];
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Utility.LoggingNonFatalError(ex);
                                        }
                                    }
                                }

                                if (!FilteredItem.SkipShowOnAppLaunch)
                                {
                                    if (FilteredItem.ShowEveryCountDays_PopUp < 0)
                                    {
                                        FilteredMaintenancePopupItems.Add(FilteredItem);
                                    }
                                    else if (!string.IsNullOrEmpty(FilteredItem.ShowDateForDay) && FilteredItem.ShowEveryCountDays_PopUp > 0)
                                    {
                                        DateTime nowDateTime = DateTime.Now;
                                        DateTime showDateTime = DateTime.ParseExact(FilteredItem.ShowDateForDay, "yyyyMMddTHHmmss",
                                        CultureInfo.InvariantCulture, DateTimeStyles.None);

                                        if ((showDateTime.Date == nowDateTime.Date && FilteredItem.ShowCountForDay >= FilteredItem.ShowEveryCountDays_PopUp))
                                        {

                                        }
                                        else
                                        {
                                            FilteredMaintenancePopupItems.Add(FilteredItem);
                                        }
                                    }
                                }

                                if (FilteredMaintenancePopupItems != null && FilteredMaintenancePopupItems.Count > 0)
                                {
                                    List<WhatsNewModel> list = new List<WhatsNewModel>();
                                    for (int index = 0; index < FilteredMaintenancePopupItems.Count; index++)
                                    {
                                        if (FilteredMaintenancePopupItems[index].ShowEveryCountDays_PopUp > 0)
                                        {
                                            string id = FilteredMaintenancePopupItems[index].ID;
                                            string recordDate = FilteredMaintenancePopupItems[index].ShowDateForDay;
                                            int count = FilteredMaintenancePopupItems[index].ShowCountForDay;

                                            DateTime showDateTime = DateTime.ParseExact(recordDate, "yyyyMMddTHHmmss",
                                                CultureInfo.InvariantCulture, DateTimeStyles.None);

                                            if (showDateTime.Date == DateTime.Now.Date)
                                            {
                                                count = count + 1;
                                            }
                                            else
                                            {
                                                recordDate = GetCurrentDate();
                                                count = 1;
                                            }

                                            wtManager.UpdateDialogCounterItem(id, recordDate, count);
                                        }
                                    }

                                    foreach (WhatsNewEntity item in FilteredMaintenancePopupItems)
                                    {
                                        list.Add(new WhatsNewModel()
                                        {
                                            ID = item.ID,
                                            PortraitImage_PopUp = item.PortraitImage_PopUp,
                                            PortraitImage_PopUpB64 = item.PortraitImage_PopUpB64,
                                            PopUp_HeaderImage = item.PopUp_HeaderImage,
                                            PopUp_HeaderImageB64 = item.PopUp_HeaderImageB64,
                                            PopUp_Text_Only = item.PopUp_Text_Only,
                                            PopUp_Text_Content = item.PopUp_Text_Content,
                                            SkipShowOnAppLaunch = item.SkipShowOnAppLaunch,
                                            Donot_Show_In_WhatsNew = item.Donot_Show_In_WhatsNew,
                                            Disable_DoNotShow_Checkbox = item.Disable_DoNotShow_Checkbox
                                        });
                                    }
                                    IsRootTutorialShown = true;
                                    WhatsNewDialogFragment dialogFragmnet = new WhatsNewDialogFragment(this);
                                    dialogFragmnet.Cancelable = false;
                                    Bundle extras = new Bundle();
                                    extras.PutString("whatsnew", JsonConvert.SerializeObject(list));
                                    dialogFragmnet.Arguments = extras;
                                    dialogFragmnet.Show(SupportFragmentManager, "WhatsNew Dialog");
                                }
                            }
                            else if (bcrmEntity != null && bcrmEntity.IsDown && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                            {
                                OnShowBCRMPopup(bcrmEntity);
                            }
                            else
                            {
                                List<WhatsNewEntity> FilteredPopupItems = new List<WhatsNewEntity>();
                                WhatsNewEntity FilteredItem = new WhatsNewEntity();

                                for (int i = 0; i < items.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        FilteredItem = items[i];
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(items[i].PublishDate) && !string.IsNullOrEmpty(FilteredItem.PublishDate))
                                            {
                                                DateTime showDateTime = DateTime.ParseExact(FilteredItem.PublishDate, "yyyyMMddTHHmmss",
                                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                                DateTime compareDateTime = DateTime.ParseExact(items[i].PublishDate, "yyyyMMddTHHmmss",
                                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                                int result = DateTime.Compare(compareDateTime, showDateTime);
                                                if (result > 0)
                                                {
                                                    FilteredItem = items[i];
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Utility.LoggingNonFatalError(ex);
                                        }
                                    }
                                }

                                if (!FilteredItem.SkipShowOnAppLaunch)
                                {
                                    if (FilteredItem.ShowEveryCountDays_PopUp < 0)
                                    {
                                        FilteredPopupItems.Add(FilteredItem);
                                    }
                                    else if (!string.IsNullOrEmpty(FilteredItem.ShowDateForDay) && FilteredItem.ShowEveryCountDays_PopUp > 0)
                                    {
                                        DateTime nowDateTime = DateTime.Now;
                                        DateTime showDateTime = DateTime.ParseExact(FilteredItem.ShowDateForDay, "yyyyMMddTHHmmss",
                                        CultureInfo.InvariantCulture, DateTimeStyles.None);

                                        if ((showDateTime.Date == nowDateTime.Date && FilteredItem.ShowCountForDay >= FilteredItem.ShowEveryCountDays_PopUp))
                                        {

                                        }
                                        else
                                        {
                                            FilteredPopupItems.Add(FilteredItem);
                                        }
                                    }
                                }

                                if (FilteredPopupItems != null && FilteredPopupItems.Count > 0)
                                {
                                    List<WhatsNewModel> list = new List<WhatsNewModel>();
                                    for (int index = 0; index < FilteredPopupItems.Count; index++)
                                    {
                                        if (FilteredPopupItems[index].ShowEveryCountDays_PopUp > 0)
                                        {
                                            string id = FilteredPopupItems[index].ID;
                                            string recordDate = FilteredPopupItems[index].ShowDateForDay;
                                            int count = FilteredPopupItems[index].ShowCountForDay;

                                            DateTime showDateTime = DateTime.ParseExact(recordDate, "yyyyMMddTHHmmss",
                                                CultureInfo.InvariantCulture, DateTimeStyles.None);

                                            if (showDateTime.Date == DateTime.Now.Date)
                                            {
                                                count = count + 1;
                                            }
                                            else
                                            {
                                                recordDate = GetCurrentDate();
                                                count = 1;
                                            }

                                            wtManager.UpdateDialogCounterItem(id, recordDate, count);
                                        }
                                    }

                                    foreach (WhatsNewEntity item in FilteredPopupItems)
                                    {
                                        list.Add(new WhatsNewModel()
                                        {
                                            ID = item.ID,
                                            PortraitImage_PopUp = item.PortraitImage_PopUp,
                                            PortraitImage_PopUpB64 = item.PortraitImage_PopUpB64,
                                            PopUp_HeaderImage = item.PopUp_HeaderImage,
                                            PopUp_HeaderImageB64 = item.PopUp_HeaderImageB64,
                                            PopUp_Text_Only = item.PopUp_Text_Only,
                                            PopUp_Text_Content = item.PopUp_Text_Content,
                                            SkipShowOnAppLaunch = item.SkipShowOnAppLaunch,
                                            Donot_Show_In_WhatsNew = item.Donot_Show_In_WhatsNew,
                                            Disable_DoNotShow_Checkbox = item.Disable_DoNotShow_Checkbox
                                        });
                                    }
                                    IsRootTutorialShown = true;
                                    WhatsNewDialogFragment dialogFragmnet = new WhatsNewDialogFragment(this);
                                    dialogFragmnet.Cancelable = false;
                                    Bundle extras = new Bundle();
                                    extras.PutString("whatsnew", JsonConvert.SerializeObject(list));
                                    dialogFragmnet.Arguments = extras;
                                    dialogFragmnet.Show(SupportFragmentManager, "WhatsNew Dialog");
                                }
                                else
                                {
                                    PopulateIdentificationDetails();
                                }
                            }
                        }
                        else if (bcrmEntity != null && bcrmEntity.IsDown && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                        {
                            Handler h = new Handler();
                            Action myAction = () =>
                            {
                                OnShowBCRMPopup(bcrmEntity);
                            };
                            h.PostDelayed(myAction, 8000);
                            //OnShowBCRMPopup(bcrmEntity);
                        }
                        else
                        {
                            if (!UserSessions.GetUpdateIdDialog(this.mPref))
                            {
                                PopulateIdentificationDetails();

                            }
                        }
                    }
                    else if (SetEligibleEBUser())
                    {
                        isWhatNewDialogOnHold = true;

                        if (MyTNBAccountManagement.GetInstance().IsFromLoginPage() && Utility.IsMDMSDownEnergyBudget())
                        {
                            MyTNBAccountManagement.GetInstance().SetFromLoginPage(false);
                            Handler h = new Handler();
                            Action myAction = () =>
                            {
                                OnCheckEnergyBudgetUser();
                            };
                            h.PostDelayed(myAction, 5);
                        }
                    }
                    else
                    {
                        if (UserSessions.GetUpdateIdDialog(this.mPref))
                        {
                            PopulateIdentificationDetails();

                        }
                        isWhatNewDialogOnHold = true;
                    }
                }
                else if (bcrmEntity != null && bcrmEntity.IsDown && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                {
                    OnShowBCRMPopup(bcrmEntity);
                }
                else if (SetEligibleEBUser())
                {
                    if (MyTNBAccountManagement.GetInstance().IsFromLoginPage() && Utility.IsMDMSDownEnergyBudget())
                    {
                        MyTNBAccountManagement.GetInstance().SetFromLoginPage(false);
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            OnCheckEnergyBudgetUser();
                        };
                        h.PostDelayed(myAction, 5);
                    }
                    MyTNBAccountManagement.GetInstance().OnHoldWhatNew(false);
                }
                //else
                if (UserSessions.HasHomeTutorialShown(this.mPref) && UserSessions.GetUpdateIdDialog(this.mPref))
                {
                    PopulateIdentificationDetails();
                    UserSessions.UpdateUpdateIdDialog(this.mPref);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private string GetCurrentDate()
        {
            DateTime currentDate = DateTime.Now;
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            return currentDate.ToString(@"yyyyMMddTHHmmss", currCult);
        }

        private void OnShowBCRMPopup(DownTimeEntity bcrmEntity)
        {
            //MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
            //        .SetHeaderImage(Resource.Drawable.maintenance_bcrm_new)
            //        .SetTitle(bcrmEntity.DowntimeTextMessage)
            //        .SetMessage(bcrmEntity.DowntimeMessage)
            //        .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.GOT_IT))
            //        .Build()
            //        .Show();

            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_WITH_FLOATING_IMAGE_ONE_BUTTON)
               .SetHeaderImage(Resource.Drawable.maintenance_bcrm_new)
               .SetTitle(bcrmEntity.DowntimeTextMessage)
               .SetMessage(bcrmEntity.DowntimeMessage)
               .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.GOT_IT))
               .Build()
               .Show();
            //MyTNBAccountManagement.GetInstance().SetIsMaintenanceDialogShown(true);
        }

        public void ReloadProfileMenu()
        {
            this.mPresenter.OnLoadMoreMenu();
            SetBottomNavigationLabels();
        }

        public bool GetIsRootTutorialShown()
        {
            return IsRootTutorialShown;
        }

        public void SetIsRootTutorialShown(bool flag)
        {
            IsRootTutorialShown = flag;
        }

        public bool GetIsRootTutorialDone()                 //testing //not using yet
        {
            return IsRootTutorialShown;
        }

        public void SetIsRootTutorialDone(bool flag)         //testing //not using yet
        {
            IsRootTutorialShown = flag;
        }

        private void OnSelectReward()
        {
            bottomNavigationView.SelectedItemId = Resource.Id.menu_reward;
            SetBottomNavigationLabels();
        }

        private void OnSelectWhatsNew()
        {
            bottomNavigationView.SelectedItemId = Resource.Id.menu_promotion;
            SetBottomNavigationLabels();
        }

        public void ShowRewardFailedTooltip()
        {
            try
            {
                IsRootTutorialShown = true;
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(Utility.GetLocalizedLabel("Error", "rewardsUnavailableTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("Error", "rewardsUnavailableMsg"))
                    .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                    .SetCTAaction(() =>
                    {
                        IsRootTutorialShown = false;
                        if (currentFragment.GetType() == typeof(HomeMenuFragment))
                        {
                            HomeMenuFragment fragment = (HomeMenuFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                            fragment.CallOnCheckShowHomeTutorial();
                        }
                    })
                    .Build().Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowWhatsNewFailedTooltip()
        {
            try
            {
                IsRootTutorialShown = true;
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(Utility.GetLocalizedLabel("Error", "whatsNewUnavailableTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("Error", "whatsNewUnavailableMsg"))
                    .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                    .SetCTAaction(() =>
                    {
                        IsRootTutorialShown = false;
                        if (currentFragment.GetType() == typeof(HomeMenuFragment))
                        {
                            HomeMenuFragment fragment = (HomeMenuFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                            fragment.CallOnCheckShowHomeTutorial();
                        }
                    })
                    .Build().Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedSSMRMeterReadingTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingTimeStamp = mSavedTimeStamp;
            }
            this.mPresenter.OnGetSmartMeterReadingWalkthroughtTimeStamp();
        }

        public void CheckSSMRMeterReadingTimeStamp()
        {
            try
            {
                SSMRMeterReadingScreensParentEntity wtManager = new SSMRMeterReadingScreensParentEntity();
                List<SSMRMeterReadingScreensParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingScreensParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingTimeStamp))
                        {
                            this.mPresenter.OnGetSSMRMeterReadingScreens();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedSSMRMeterReadingNoOCRTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingNoOCRTimeStamp = mSavedTimeStamp;
            }
            this.mPresenter.OnGetSmartMeterReadingWalkthroughtNoOCRTimeStamp();
        }

        public void CheckSSMRMeterReadingNoOCRTimeStamp()
        {
            try
            {
                SSMRMeterReadingScreensOCROffParentEntity wtManager = new SSMRMeterReadingScreensOCROffParentEntity();
                List<SSMRMeterReadingScreensOCROffParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingScreensOCROffParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingNoOCRTimeStamp))
                        {
                            this.mPresenter.OnGetSSMRMeterReadingScreensNoOCR();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedSSMRMeterReadingThreePhaseTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingThreePhaseTimeStamp = mSavedTimeStamp;
            }
            this.mPresenter.OnGetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();
        }

        public void CheckSSMRMeterReadingThreePhaseTimeStamp()
        {
            try
            {
                SSMRMeterReadingThreePhaseScreensParentEntity wtManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                List<SSMRMeterReadingThreePhaseScreensParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingThreePhaseScreensParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingThreePhaseTimeStamp))
                        {
                            this.mPresenter.OnGetSSMRMeterReadingThreePhaseScreens();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void OnSavedSSMRMeterReadingThreePhaseNoOCRTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingThreePhaseNoOCRTimeStamp = mSavedTimeStamp;
            }
            this.mPresenter.OnGetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();
        }

        public void CheckSSMRMeterReadingThreePhaseNoOCRTimeStamp()
        {
            try
            {
                SSMRMeterReadingThreePhaseScreensOCROffParentEntity wtManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                List<SSMRMeterReadingThreePhaseScreensOCROffParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingThreePhaseScreensOCROffParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingThreePhaseNoOCRTimeStamp))
                        {
                            this.mPresenter.OnGetSSMRMeterReadingThreePhaseScreensNoOCR();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool GetAlreadyStarted()
        {
            return alreadyStarted;
        }

        public void SetAlreadyStarted(bool flag)
        {
            alreadyStarted = flag;
        }

        public bool FilterComAndNEM()
        {
            try
            {
                bool flag = false;
                int totalSMCAEB = UserSessions.GetEnergyBudgetList().Count;
                int totalSMCA = CustomerBillingAccount.SMeterBudgetAccountListALL().Count;
                int diffSMCA = totalSMCA - totalSMCAEB;
                if (diffSMCA > 0 && totalSMCAEB == 0)
                {
                    flag = true;
                }
                MyTNBAccountManagement.GetInstance().SetIsCOMCLandNEM(flag);
                return flag;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                return false;
            }
        }

        public bool SetEligibleEBUser()
        {
            return UserSessions.GetEnergyBudgetList().Count > 0
                && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                && !UserSessions.GetSavePopUpCountEB(PreferenceManager.GetDefaultSharedPreferences(this)).Equals("2")
                && !MyTNBAccountManagement.GetInstance().COMCLandNEM();
        }

        public bool SetEligibleEBUserExtra()
        {
            return !UserSessions.GetSavePopUpCountEB(PreferenceManager.GetDefaultSharedPreferences(this)).Equals("2");
        }

        public bool CheckWhatNewPopupCount()
        {
            return this.mPresenter.GetIsWhatsNewDialogShowNeed();
        }

        public void CheckStatusEligibleEB()
        {
            if (MyTNBAccountManagement.GetInstance().IsFromApiEBFinish())
            {
                BaseCAListModel isEbfeature = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(EligibilitySessionCache.Features.EB);
                if (isEbfeature != null)
                {
                    if (EBUtility.Instance.IsAccountEligible)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsEBUser(true);
                    }
                    else
                    {
                        MyTNBAccountManagement.GetInstance().SetIsEBUser(false);
                    }
                }
                else
                {
                    MyTNBAccountManagement.GetInstance().SetIsEBUser(false);
                }
            }
        }

        public void CheckStatusEligibleSD()
        {
            try
            {
                BaseCAListModel isSDfeature = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(EligibilitySessionCache.Features.SD);
                if (isSDfeature != null)
                {
                    if (SDUtility.Instance.IsAccountEligible)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsSDUser(true);
                    }
                    else
                    {
                        MyTNBAccountManagement.GetInstance().SetIsSDUser(false);
                    }
                }
                else
                {
                    MyTNBAccountManagement.GetInstance().SetIsSDUser(false);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                MyTNBAccountManagement.GetInstance().SetIsSDUser(false);
            }
            //MyTNBAccountManagement.GetInstance().SetIsSDUser(true); //add for sit test alway true
        }

        public void OnCheckEnergyBudgetUser()
        {
            if (SetEligibleEBUserExtra())
            {
                if (UserSessions.GetSavePopUpCountEB(PreferenceManager.GetDefaultSharedPreferences(this)).Equals(string.Empty))
                {
                    UserSessions.SavePopUpCountEB(PreferenceManager.GetDefaultSharedPreferences(this), "1");
                }
                else if (UserSessions.GetSavePopUpCountEB(PreferenceManager.GetDefaultSharedPreferences(this)).Equals("1"))
                {
                    UserSessions.SavePopUpCountEB(PreferenceManager.GetDefaultSharedPreferences(this), "2");
                }

                try
                {
                    MyTNBAccountManagement.GetInstance().SetFromLoginPage(false);
                    isWhatNewDialogOnHold = false;
                    //mPresenter.DisableWalkthrough();
                    if (currentFragment.GetType() == typeof(HomeMenuFragment))
                    {
                        HomeMenuFragment fragment = (HomeMenuFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                        fragment.EBPopupActivity();
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        public void SetFloatingButtonSiteCoreDoneFlag(bool flag)
        {
            isFloatingButtonSiteCoreDone = flag;
        }

        public bool GetFloatingButtonSiteCoreDoneFlag()
        {
            return isFloatingButtonSiteCoreDone;
        }

        public void SetCustomFloatingButtonImage(FloatingButtonModel item)
        {
            try
            {
                if (!isFloatingButtonSiteCoreDone)
                {
                    try
                    {
                        if (item.ImageBitmap != null)
                        {
                            var bitmapDrawable = new BitmapDrawable(item.ImageBitmap);
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    floatingButtonImg.SetImageDrawable(bitmapDrawable);
                                }
                                catch (Exception ex)
                                {
                                    Utility.LoggingNonFatalError(ex);
                                }
                            });
                        }


                        //    DateTime startDateTime = DateTime.ParseExact(item.StartDateTime, "yyyyMMddTHHmmss",
                        //        CultureInfo.InvariantCulture, DateTimeStyles.None);
                        //    DateTime stopDateTime = DateTime.ParseExact(item.EndDateTime, "yyyyMMddTHHmmss",
                        //        CultureInfo.InvariantCulture, DateTimeStyles.None);
                        //    DateTime nowDateTime = DateTime.Now;
                        //    int startResult = DateTime.Compare(nowDateTime, startDateTime);
                        //    int endResult = DateTime.Compare(nowDateTime, stopDateTime);
                        //    if (startResult >= 0 && endResult <= 0)
                        //    {
                        //        try
                        //        {
                        //            int secondMilli = 0;
                        //            try
                        //            {
                        //                secondMilli = (int)(float.Parse(item.ShowForSeconds, CultureInfo.InvariantCulture.NumberFormat) * 1000);
                        //            }
                        //            catch (Exception nea)
                        //            {
                        //                Utility.LoggingNonFatalError(nea);
                        //            }

                        //            if (secondMilli == 0)
                        //            {
                        //                try
                        //                {
                        //                    secondMilli = Int32.Parse(item.ShowForSeconds) * 1000;
                        //                }
                        //                catch (Exception nea)
                        //                {
                        //                    Utility.LoggingNonFatalError(nea);
                        //                }
                        //            }

                        //            var bitmapDrawable = new BitmapDrawable(item.ImageBitmap);
                        //            RunOnUiThread(() =>
                        //            {
                        //                try
                        //                {
                        //                    floatingButtonImg.SetImageDrawable(bitmapDrawable);
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    Utility.LoggingNonFatalError(ex);
                        //                }
                        //            });

                        //            //this.userActionsListener.OnWaitSplashScreenDisplay(secondMilli);
                        //        }
                        //        catch (Exception ne)
                        //        {
                        //            //SetDefaultAppLaunchImage();
                        //            Utility.LoggingNonFatalError(ne);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //SetDefaultAppLaunchImage();
                        //    }
                        //}
                        //else
                        //{
                        //    //SetDefaultAppLaunchImage();
                        //}
                    }
                    catch (Exception ne)
                    {
                        //SetDefaultAppLaunchImage();
                        Utility.LoggingNonFatalError(ne);
                    }
                }

            }
            catch (Exception e)
            {
                //SetDefaultAppLaunchImage();
                Utility.LoggingNonFatalError(e);
            }
        }

        public static Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap;
            try
            {
                byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        public void OnSavedFloatingButtonTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    savedFloatingButtonTimeStamp = timestamp;
                }
                this.userActionsListener.OnGetFloatingButtonTimeStamp();
            }
            catch (Exception e)
            {
                this.userActionsListener.OnGetFloatingButtonCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    if (timestamp.Equals(savedTimeStamp))
                    {
                        MyTNBApplication.siteCoreUpdated = false;
                    }
                    else
                    {
                        MyTNBApplication.siteCoreUpdated = true;
                    }
                }
                else
                {
                    MyTNBApplication.siteCoreUpdated = true;
                }
                // RunOnUiThread(() => StartActivity(typeof(WalkThroughActivity)));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnFloatingButtonTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    if (timestamp.Equals(savedFloatingButtonTimeStamp))
                    {
                        this.userActionsListener.OnGetFloatingButtonCache();
                    }
                    else
                    {
                        this.userActionsListener.OnGetFloatingButtonItem();
                    }
                }
                else
                {
                    this.userActionsListener.OnGetFloatingButtonCache();
                }
            }
            catch (Exception e)
            {
                this.userActionsListener.OnGetFloatingButtonCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void NavigateToEnergyBudget()
        {
            Intent energy_budget_activity = new Intent(this, typeof(EnergyBudgetActivity));
            StartActivityForResult(energy_budget_activity, SELECT_SM_ACCOUNT_REQUEST_CODE);
        }

        public void NavigateToAddAccount()
        {
            this.ShowAddAccount();
        }

        public void NavigateToViewAccountStatement(CustomerBillingAccount account)
        {
            userActionsListener.ShowBillMenuWithAccount(account);
            this.ShowViewAccountStatement(account);
        }

        public void TriggerIneligiblePopUp()
        {
            userActionsListener.OnMenuSelect(Resource.Id.menu_bill, true);
            this.ShowIneligiblePopUp();
        }

        public void NavigateToNBR()
        {
            this.ShowNewBillRedesign();
        }

        public void NavigateToGSL()
        {
            this.ShowGSLInfoScreen();
        }

        public void OnCheckDraftResumePopUp()
        {
            this.userActionsListener.OnCheckDraftForResume(PreferenceManager.GetDefaultSharedPreferences(this));
        }

        public void OnShowDraftResumePopUp(MyHomeToolTipModel toolTipModel, List<PostGetDraftResponseItemModel> newList, bool isMultipleDraft)
        {
            if (currentFragment.GetType() == typeof(HomeMenuFragment))
            {
                this.ShowDraftResumePopUp(toolTipModel, newList, isMultipleDraft);
            }
        }
    }
}