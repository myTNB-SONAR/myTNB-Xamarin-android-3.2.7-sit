using System;
using System.Collections.Generic;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ProfileMenu;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.SelectSupplyAccount.Activity;
using myTNB_Android.Src.SummaryDashBoard.SummaryListener;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Activity;
using static Android.Views.View;
using myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using Newtonsoft.Json;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.WhatsNewDialog;
using System.Globalization;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.BottomNavigation;
using AndroidX.Core.Content;
using Android.Text;
using Android.Text.Style;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using myTNB.Mobile;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB_Android.Src.DeviceCache;
using myTNB.Mobile.AWS.Models;
using System.Linq;

namespace myTNB_Android.Src.myTNBMenu.Activity
{
    [Activity(Label = "@string/dashboard_activity_title"
        , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.DashboardHome"
        , WindowSoftInputMode = SoftInput.AdjustNothing)]
    public class DashboardHomeActivity : BaseToolbarAppCompatActivity, DashboardHomeContract.IView, ISummaryFragmentToDashBoardActivtyListener
    {
        internal readonly string TAG = typeof(DashboardHomeActivity).Name;

        public readonly static int PAYMENT_RESULT_CODE = 5451;

        public static DashboardHomeActivity dashboardHomeActivity;

        private DashboardHomeContract.IUserActionsListener userActionsListener;
        private DashboardHomePresenter mPresenter;

        private bool urlSchemaCalled = false;
        private string urlSchemaData = "";
        private string urlSchemaPath = "";

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.mainView)]
        LinearLayout mainView;

        [BindView(Resource.Id.content_layout)]
        FrameLayout contentLayout;

        [BindView(Resource.Id.txt_account_name)]
        TextView txtAccountName;

        [BindView(Resource.Id.bottom_navigation)]
        BottomNavigationView bottomNavigationView;

        AccountData SelectedAccountData;

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

        private bool IsRootTutorialShown = false;

        private static bool isFirstInitiate = false;

        private static bool isWhatNewDialogOnHold = false;

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

        public override bool ShowBackArrowIndicator()
        {
            return isBackButtonVisible;
        }

        public void SetCurrentFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            currentFragment = fragment;
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
            mPresenter = new DashboardHomePresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            TextViewUtils.SetMuseoSans500Typeface(txtAccountName);

            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
            if (IsRewardsDisabled)
            {
                if (bottomNavigationView.Menu.FindItem(Resource.Id.menu_reward) != null)
                {
                    bottomNavigationView.Menu.RemoveItem(Resource.Id.menu_reward);
                }
            }

            IsRootTutorialShown = false;

            SetBottomNavigationLabels();
            bottomNavigationView.SetShiftMode(false, false);
            bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
            bottomNavigationView.ItemIconTintList = null;

            bottomNavigationView.NavigationItemSelected += BottomNavigationView_NavigationItemSelected;

            RewardsMenuUtils.OnSetRewardLoading(false);

            WhatsNewMenuUtils.OnSetWhatsNewLoading(false);

            Bundle extras = Intent?.Extras;

            if (extras != null && extras.ContainsKey("FROM_NOTIFICATION"))
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

            if (extras != null && extras.ContainsKey("urlSchemaData"))
            {
                urlSchemaCalled = true;
                urlSchemaData = extras.GetString("urlSchemaData");
                if (extras != null && extras.ContainsKey("urlSchemaPath"))
                {
                    urlSchemaPath = extras.GetString("urlSchemaPath");
                }
            }

            this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Click += DashboardHomeActivity_Click;

            try
            {
                if (!alreadyStarted)
                {
                    this.userActionsListener.Start();
                    OnSetupSSMRMeterReadingTutorial();
                    this.mPresenter.OnGetEPPTooltipContentDetail();
                    this.mPresenter.OnGetBillTooltipContent();
                    alreadyStarted = true;
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

            try
            {
                new SyncSRApplicationAPI(this).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, string.Empty);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Sync SR Error: " + e.Message);
            }
            
            try
            {
               new EligibilityAPI(this).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, string.Empty);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] EligibilityAPI Error: " + e.Message);
            }
        }

        private async void RouteToApplicationLanding()
        {
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                ShowProgressDialog();
                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                }
                HideProgressDialog();
            }
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
            {
                AllApplicationsCache.Instance.Clear();
                AllApplicationsCache.Instance.Reset();
                Intent applicationLandingIntent = new Intent(this, typeof(ApplicationStatusLandingActivity));
                StartActivity(applicationLandingIntent);
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

        private void BottomNavigationView_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            this.userActionsListener.OnMenuSelect(e.Item.ItemId);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
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

        public void ShowBillMenu(AccountData selectedAccount)
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetChecked(true);
            txtAccountName.Visibility = ViewStates.Gone;
            if (currentFragment != null)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = null;
            }

            if(EligibilitySessionCache.Instance.IsAccountDBREligible)
            {
                GetBillRenderingAsync(selectedAccount);
            }
            else
            {
                currentFragment = ItemisedBillingMenuFragment.NewInstance(selectedAccount);
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_layout, currentFragment)
                    .CommitAllowingStateLoss();
            }
        }
        private async void GetBillRenderingAsync(AccountData selectedAccount)
        {
            try
            {
                ShowProgressDialog();
                GetBillRenderingModel getBillRenderingModel = new GetBillRenderingModel();
                AccountData dbrAccount = selectedAccount;
                if (!AccessTokenCache.Instance.HasTokenSaved(this))
                {
                    string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                    AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                }
                GetBillRenderingResponse response = await DBRManager.Instance.GetBillRendering(dbrAccount.AccountNum, AccessTokenCache.Instance.GetAccessToken(this));
                bool _isOwner = EligibilitySessionCache.Instance.IsCADBREligible(dbrAccount.AccountNum);
                HideProgressDialog();
                //Nullity Check
                if (response != null
                   && response.StatusDetail != null
                   && response.StatusDetail.IsSuccess)
                {
                    currentFragment = ItemisedBillingMenuFragment.NewInstance(selectedAccount, response.Content, _isOwner);
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_layout, currentFragment)
                        .CommitAllowingStateLoss();
                }
                else
                {
                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                        .SetTitle(response.StatusDetail.Title)
                                        .SetMessage(response.StatusDetail.Message)
                                        .SetCTALabel(response.StatusDetail.PrimaryCTATitle)
                                        .Build();
                    errorPopup.Show();
                }

            }
            catch (System.Exception e)
            {
                HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }
        public string GetEligibleDBRAccount(AccountData selectedAccount)
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
            List<string> dBRCAs = EligibilitySessionCache.Instance.GetDBRCAs();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            CustomerBillingAccount account = new CustomerBillingAccount();
            string dbraccount = string.Empty;
            if (dBRCAs.Count > 0)
            {
                foreach (var dbrca in dBRCAs)
                {
                    dbraccount = dBRCAs.Where(x => x == selectedAccount.AccountNum).FirstOrDefault();
                    if (dbraccount != null)
                    {
                        return dbraccount;
                    }
                }
            }
            else
            {
                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(Utility.GetLocalizedLabel("Error", "defaultErrorTitle"))
                                    .SetMessage(Utility.GetLocalizedLabel("Error", "defaultErrorMessage"))
                                    .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                     .Build();
                errorPopup.Show();
            }
            return dbraccount;
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

        public async void OnDataSchemeShow()
        {
            try
            {
                if (urlSchemaCalled)
                {
                    if (urlSchemaData != null)
                    {
                        if (urlSchemaData.Contains("receipt"))
                        {

                            string transID = urlSchemaData.Substring(urlSchemaData.LastIndexOf("=") + 1);
                            if (!String.IsNullOrEmpty(transID))
                            {
                                Intent viewReceipt = new Intent(this, typeof(ViewReceiptMultiAccountNewDesignActivty));
                                viewReceipt.PutExtra("merchantTransId", transID);
                                StartActivity(viewReceipt);
                                urlSchemaCalled = false;
                            }
                        }
                        else if (urlSchemaData.Contains("rating"))
                        {
                            int ratings = int.Parse(urlSchemaData.Substring(urlSchemaData.LastIndexOf("=") + 1));
                            int lastIndexOfMerchantID = (urlSchemaData.IndexOf("&") - 1) - urlSchemaData.IndexOf("=");
                            string merchantTransId = urlSchemaData.Substring(urlSchemaData.IndexOf("=") + 1, lastIndexOfMerchantID);
                            Intent payment_activity = new Intent(this, typeof(RatingActivity));
                            payment_activity.PutExtra(Constants.MERCHANT_TRANS_ID, merchantTransId);
                            payment_activity.PutExtra(Constants.SELECTED_RATING, ratings);
                            payment_activity.PutExtra(Constants.QUESTION_ID_CATEGORY, ((int)QuestionCategoryID.Payment));
                            StartActivity(payment_activity);
                            urlSchemaCalled = false;
                        }
                        else if (urlSchemaData.Contains("rewards") && !string.IsNullOrEmpty(urlSchemaPath))
                        {
                            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();

                            if (!IsRewardsDisabled && bottomNavigationView.Menu.FindItem(Resource.Id.menu_reward) != null)
                            {
                                string rewardID = urlSchemaPath.Substring(urlSchemaPath.LastIndexOf("=") + 1);
                                if (!string.IsNullOrEmpty(rewardID))
                                {
                                    rewardID = "{" + rewardID + "}";

                                    RewardsEntity wtManager = new RewardsEntity();

                                    RewardsEntity item = wtManager.GetItem(rewardID);

                                    this.mPresenter.OnStartRewardThread();
                                }
                            }
                            else
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
                        }
                        else if (!string.IsNullOrEmpty(urlSchemaPath) && urlSchemaPath.Contains("rewards"))
                        {
                            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();

                            if (!IsRewardsDisabled && bottomNavigationView.Menu.FindItem(Resource.Id.menu_reward) != null)
                            {
                                urlSchemaData = "rewards";
                                string rewardID = urlSchemaPath.Substring(urlSchemaPath.LastIndexOf("=") + 1);
                                if (!string.IsNullOrEmpty(rewardID))
                                {
                                    rewardID = "{" + rewardID + "}";

                                    RewardsEntity wtManager = new RewardsEntity();

                                    RewardsEntity item = wtManager.GetItem(rewardID);

                                    this.mPresenter.OnStartRewardThread();
                                }
                            }
                            else
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
                        }
                        else if (urlSchemaData.Contains("whatsnew") && !string.IsNullOrEmpty(urlSchemaPath))
                        {
                            string whatsNewID = urlSchemaPath.Substring(urlSchemaPath.LastIndexOf("=") + 1);
                            if (!string.IsNullOrEmpty(whatsNewID))
                            {
                                whatsNewID = "{" + whatsNewID + "}";

                                WhatsNewEntity wtManager = new WhatsNewEntity();

                                WhatsNewEntity item = wtManager.GetItem(whatsNewID);

                                this.mPresenter.OnStartWhatsNewThread();
                            }
                        }
                        else if (!string.IsNullOrEmpty(urlSchemaPath) && urlSchemaPath.Contains("whatsnew"))
                        {
                            urlSchemaData = "whatsnew";
                            string whatsNewID = urlSchemaPath.Substring(urlSchemaPath.LastIndexOf("=") + 1);
                            if (!string.IsNullOrEmpty(whatsNewID))
                            {
                                whatsNewID = "{" + whatsNewID + "}";

                                WhatsNewEntity wtManager = new WhatsNewEntity();

                                WhatsNewEntity item = wtManager.GetItem(whatsNewID);

                                this.mPresenter.OnStartWhatsNewThread();
                            }
                        }
                        else if (!string.IsNullOrEmpty(urlSchemaPath) && urlSchemaPath.Contains("applicationListing"))
                        {
                            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
                            if (searchApplicationTypeResponse == null)
                            {
                                ShowProgressDialog();
                                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                                if (searchApplicationTypeResponse != null
                                    && searchApplicationTypeResponse.StatusDetail != null
                                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                                {
                                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                                }
                                HideProgressDialog();
                            }
                            if (searchApplicationTypeResponse != null
                                && searchApplicationTypeResponse.StatusDetail != null
                                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                            {
                                AllApplicationsCache.Instance.Clear();
                                AllApplicationsCache.Instance.Reset();
                                Intent applicationLandingIntent = new Intent(this, typeof(ApplicationStatusLandingActivity));
                                StartActivity(applicationLandingIntent);
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
                        }
                        else if (!string.IsNullOrEmpty(urlSchemaPath) && urlSchemaPath.Contains("applicationDetails"))
                        {
                            ShowProgressDialog();
                            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
                            if (searchApplicationTypeResponse == null)
                            {
                                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                                if (searchApplicationTypeResponse != null
                                    && searchApplicationTypeResponse.StatusDetail != null
                                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                                {
                                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                                }
                            }
                            if (searchApplicationTypeResponse != null
                                && searchApplicationTypeResponse.StatusDetail != null
                                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                            {
                                ApplicationDetailDisplay detailsResponse = await ApplicationStatusManager.Instance.GetApplicationDetail(ApplicationDetailsDeeplinkCache.Instance.SaveID
                                    , ApplicationDetailsDeeplinkCache.Instance.ID
                                    , ApplicationDetailsDeeplinkCache.Instance.Type
                                    , ApplicationDetailsDeeplinkCache.Instance.System);

                                if (detailsResponse.StatusDetail.IsSuccess)
                                {
                                    Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                                    applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(detailsResponse.Content));
                                    StartActivity(applicationStatusDetailIntent);
                                }
                                else
                                {
                                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                     .SetTitle(detailsResponse.StatusDetail.Title)
                                     .SetMessage(detailsResponse.StatusDetail.Message)
                                     .SetCTALabel(detailsResponse.StatusDetail.PrimaryCTATitle)
                                     .Build();
                                    errorPopup.Show();
                                }
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
                            HideProgressDialog();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        
        public void ShowFeedbackMenu()
        {
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
            ProfileMenuFragment profileMenuFragment = new ProfileMenuFragment();
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

            if (isWhatNewDialogOnHold)
            {
                isWhatNewDialogOnHold = false;
                OnCheckWhatsNewTab();
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

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void SetDashboardHomeCheck()
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_dashboard).SetChecked(true);
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
                                if (urlSchemaCalled && !string.IsNullOrEmpty(urlSchemaData) && urlSchemaData.Contains("rewards"))
                                {
                                    urlSchemaCalled = false;
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
                                if (urlSchemaCalled && !string.IsNullOrEmpty(urlSchemaData) && urlSchemaData.Contains("whatsnew"))
                                {
                                    urlSchemaCalled = false;
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
                if (urlSchemaCalled && !string.IsNullOrEmpty(urlSchemaData) && urlSchemaData.Contains("rewards"))
                {
                    HideProgressDialog();
                    urlSchemaCalled = false;
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
                if (urlSchemaCalled && !string.IsNullOrEmpty(urlSchemaData) && urlSchemaData.Contains("rewards"))
                {
                    HideProgressDialog();
                    urlSchemaCalled = false;
                    if (urlSchemaData != null)
                    {
                        if (urlSchemaData.Contains("rewards"))
                        {
                            string rewardID = urlSchemaPath.Substring(urlSchemaPath.LastIndexOf("=") + 1);
                            if (!string.IsNullOrEmpty(rewardID))
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

                if (urlSchemaCalled && !string.IsNullOrEmpty(urlSchemaData) && urlSchemaData.Contains("whatsnew"))
                {
                    HideProgressDialog();
                    urlSchemaCalled = false;
                    if (urlSchemaData != null)
                    {
                        if (urlSchemaData.Contains("whatsnew"))
                        {
                            string whatsNewID = urlSchemaPath.Substring(urlSchemaPath.LastIndexOf("=") + 1);
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
                    }
                }
                else if (this.mPresenter.GetIsWhatsNewDialogShowNeed() && currentFragment.GetType() == typeof(HomeMenuFragment))
                {
                    HomeMenuFragment fragment = (HomeMenuFragment)SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                    bool flag = fragment.GetHomeTutorialCallState();
                    flag = true;
                    if (flag)
                    {
                        this.mPresenter.SetIsWhatsNewDialogShowNeed(false);
                        WhatsNewEntity wtManager = new WhatsNewEntity();
                        List<WhatsNewEntity> items = wtManager.GetActivePopupItems();
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
                            }
                        }
                        else if (bcrmEntity != null && bcrmEntity.IsDown && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                        {
                            OnShowBCRMPopup(bcrmEntity);
                        }
                    }
                    else
                    {
                        isWhatNewDialogOnHold = true;
                    }
                }
                else if (bcrmEntity != null && bcrmEntity.IsDown && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                {
                    OnShowBCRMPopup(bcrmEntity);
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
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                    .SetHeaderImage(Resource.Drawable.maintenance_bcrm)
                    .SetTitle(bcrmEntity.DowntimeTextMessage)
                    .SetMessage(bcrmEntity.DowntimeMessage)
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .Build()
                    .Show();
            MyTNBAccountManagement.GetInstance().SetIsMaintenanceDialogShown(true);
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
    }
}
