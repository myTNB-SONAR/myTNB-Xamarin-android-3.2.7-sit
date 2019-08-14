
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
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.SitecoreCM.Models;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.BillsMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.MoreMenu;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.Promotions.Fragments;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.SelectSupplyAccount.Activity;
using myTNB_Android.Src.SummaryDashBoard.SummaryListener;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using myTNB_Android.Src.ViewReceipt.Activity;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Activity
{
    [Activity(Label = "@string/dashboard_activity_title"
              , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        ,Theme = "@style/Theme.Dashboard"
        ,WindowSoftInputMode = SoftInput.AdjustNothing)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
            DataScheme = "mytnbapp",
            Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
    public class DashboardHomeActivity : BaseToolbarAppCompatActivity, DashboardHomeContract.IView, ISummaryFragmentToDashBoardActivtyListener
    {
        internal readonly string TAG = typeof(DashboardHomeActivity).Name;

        public readonly static int PAYMENT_RESULT_CODE = 5451;

        public static DashboardHomeActivity dashboardHomeActivity;

        private DashboardHomeContract.IUserActionsListener userActionsListener;
        private DashboardHomePresenter mPresenter;

        private bool urlSchemaCalled = false;
        private string urlSchemaData = "";

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

        bool alreadyStarted = false;

        bool mobileNoUpdated = false;

        private bool isBackButtonVisible = false;

        private LoadingOverlay loadingOverlay;

        private string savedTimeStamp = "0000000";

        public static Fragment currentFragment;

        public static bool GO_TO_INNER_DASHBOARD = false;

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
            Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
            PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(PreLoginIntent);
        }

        public override bool ShowBackArrowIndicator()
        {
            return isBackButtonVisible;
        }

        public void SetCurrentFragment(Fragment fragment)
        {
            currentFragment = fragment;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            dashboardHomeActivity = this;
            base.SetToolBarTitle(GetString(Resource.String.dashboard_activity_title));
            mPresenter = new DashboardHomePresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            TextViewUtils.SetMuseoSans500Typeface(txtAccountName);


            // Get CategoryBrowsable intent data
            var data = Intent?.Data?.EncodedAuthority;
            if (!String.IsNullOrEmpty(data))
            {
                urlSchemaCalled = true;
                urlSchemaData = data;
            }

            bottomNavigationView.SetShiftMode(false, false);
            bottomNavigationView.SetImageSize(28, 3);
            bottomNavigationView.ItemIconTintList = null;

            bottomNavigationView.NavigationItemSelected += BottomNavigationView_NavigationItemSelected;

            Bundle extras = Intent?.Extras;
            if (extras != null && extras.ContainsKey(Constants.PROMOTION_NOTIFICATION_VIEW))
            {
                bottomNavigationView.Menu.FindItem(Resource.Id.menu_promotion).SetChecked(true);
                this.userActionsListener?.OnMenuSelect(Resource.Id.menu_promotion);
            }

            this.userActionsListener?.OnNotificationCount();

        }

        public void ShowBackButton(bool flag)
        {
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(flag);
            this.SupportActionBar.SetDisplayShowHomeEnabled(flag);
        }

        public override void OnBackPressed()
        {
            try
            {
                if (currentFragment.GetType() == typeof(DashboardChartFragment) ||
                    currentFragment.GetType() == typeof(DashboardChartNonOwnerNoAccess) ||
                    currentFragment.GetType() == typeof(DashboardSmartMeterFragment) ||
                    currentFragment.GetType() == typeof(FeedbackMenuFragment))
                {
                    EnableDropDown(false);
                    HideAccountName();
                    ShowBackButton(false);
                    SetToolbarTitle(Resource.String.dashboard_activity_title);
                    ShowHomeDashBoard();

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

        public void ShowNoAccountDashboardChartMenu()
        {
            currentFragment = new DashboardChartNoTNBAccount();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, new DashboardChartNoTNBAccount())
                           .CommitAllowingStateLoss();
            ShowBackButton(false);
        }

        public void ShowNoAccountBillMenu()
        {
            ShowBackButton(false);
            FragmentManager.BeginTransaction()
                   .Replace(Resource.Id.content_layout, new BillingMenuNoTNBAccount())
                   .CommitAllowingStateLoss();
        }

        public void ShowOwnerDashboardNoInternetConnection(string accountName, UsageHistoryResponse response, AccountData selectedAccount)
        {
            txtAccountName.Text = accountName;
            currentFragment = new DashboardChartFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(true, response, selectedAccount),
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }

        public void ShowOwnerDashboardNoInternetConnection(string accountName, bool amountDueFailed, string contentTxt, string btnTxt, AccountData selectedAccount)
        {
            txtAccountName.Text = accountName;
            currentFragment = new DashboardChartFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(true, amountDueFailed, contentTxt, btnTxt, selectedAccount),
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }

        public void ShowOwnerDashboardNoInternetConnection(string accountName, UsageHistoryResponse response, AccountData selectedAccount, AccountDueAmountResponse amountDueResponse)
        {
            txtAccountName.Text = accountName;
            currentFragment = new DashboardChartFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(true, response, selectedAccount, amountDueResponse),
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }


        public void ShowOwnerBillsNoInternetConnection(AccountData selectedAccount)
        {
            ShowBackButton(false);
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountNickName;
            currentFragment = new BillsMenuFragment();
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, BillsMenuFragment.NewInstance(selectedAccount, true))
                .CommitAllowingStateLoss();

        }

        public void ShowChart(UsageHistoryData data, AccountData selectedAccount, AccountDueAmountResponse amountDueResponse)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountName;
            currentFragment = new DashboardChartFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(data, SelectedAccountData, amountDueResponse),
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }

        public void ShowChartWithError(UsageHistoryData data, AccountData selectedAccount, string errorCode, string errorMessage, AccountDueAmountResponse amountDueResponse)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountName;
            currentFragment = new DashboardChartFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(data, SelectedAccountData, errorCode, errorMessage, amountDueResponse),
                         typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }

        public void ShowSMChart(SMUsageHistoryData data, AccountData selectedAccount)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountName;
            currentFragment = new DashboardSmartMeterFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardSmartMeterFragment.NewInstance(data, SelectedAccountData),
                                    typeof(DashboardSmartMeterFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }

        public void ShowSMChartWithError(SMUsageHistoryData data, AccountData selectedAccount, bool noSMData)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountName;
            currentFragment = new DashboardSmartMeterFragment();
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, DashboardSmartMeterFragment.NewInstance(data, SelectedAccountData, noSMData),
                         typeof(DashboardSmartMeterFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }

        [OnClick(Resource.Id.txt_account_name)]
        void OnSelectSupplyAccount(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.SelectSupplyAccount();
        }

        public void ShowSelectSupplyAccount()
        {
            Intent supplyAccount = new Intent(this, typeof(SelectSupplyAccountActivity));
            StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);

        }

        public void ShowBillMenu(AccountData selectedAccount)
        {
            ShowBackButton(false);
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountNickName;
            currentFragment = new BillsMenuFragment();
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, BillsMenuFragment.NewInstance(selectedAccount))
                .CommitAllowingStateLoss();
        }

        public void PreShowBillMenu(AccountData selectedAccount)
        {
            ShowBackButton(false);
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountNickName;
            currentFragment = new BillsMenuFragment();
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, BillsMenuFragment.NewInstance(selectedAccount, "PRE_SHOW"))
                .CommitAllowingStateLoss();
        }

        public void ShowBillMenuWithError(string contextTxt, string btnTxt, AccountData selectedAccount)
        {
            ShowBackButton(false);
            currentFragment = new BillsMenuFragment();
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountNickName;
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, BillsMenuFragment.NewInstance(contextTxt, btnTxt, selectedAccount))
                .CommitAllowingStateLoss();
        }

        public void SetToolbarTitle(int stringResourceId)
        {
            this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = GetString(stringResourceId);
        }

        public void BillsMenuRefresh(AccountData accountData)
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetChecked(true);
            ShowAccountName();
            SetToolbarTitle(Resource.String.bill_menu_activity_title);
            ShowBillMenu(accountData);
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
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
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
                if (IsActive())
                {
                    if (loadingOverlay != null && loadingOverlay.IsShowing)
                    {
                        loadingOverlay.Dismiss();
                    }
                }

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
            FeedbackMenuFragment fragment = new FeedbackMenuFragment();
            currentFragment = fragment;
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, currentFragment)
                     .CommitAllowingStateLoss();
        }

        public void ShowPromotionsMenu(Weblink weblink)
        {
            ShowBackButton(false);
            PromotionListFragment fragment = new PromotionListFragment();
            currentFragment = fragment;
            FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_layout, fragment)
                        .CommitAllowingStateLoss();

        }

        public void ShowMoreMenu()
        {
            ShowBackButton(false);
            MoreMenuFragment moreMenuFragment = new MoreMenuFragment();
            currentFragment = moreMenuFragment;
            if (mobileNoUpdated)
            {
                Bundle extras = new Bundle();
                extras.PutBoolean(Constants.FORCE_UPDATE_PHONE_NO, mobileNoUpdated);
                moreMenuFragment.Arguments = extras;
                mobileNoUpdated = false;
            }
            FragmentManager.BeginTransaction()
                     .Replace(Resource.Id.content_layout, moreMenuFragment)
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
            if (!alreadyStarted)
            {
                this.userActionsListener.Start();
                alreadyStarted = true;
                ShowPromotion(true);
            }

        }

        public void EnableBillMenu()
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetEnabled(true);
        }

        public void DisableBillMenu()
        {
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetEnabled(false);
        }

        public void ShowNotificationCount(int count)
        {
            if (count <= 0)
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
            }
            else
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.ApplicationContext, count);
            }
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

        public void BillsMenuAccess()
        {
            ShowProgressDialog();
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetChecked(true);
            ShowAccountName();
            SetToolbarTitle(Resource.String.bill_menu_activity_title);
            this.userActionsListener?.OnMenuSelect(Resource.Id.menu_bill);
        }

        public void BillsMenuAccess(AccountData selectedAccount)
        {
            ShowProgressDialog();
            bottomNavigationView.Menu.FindItem(Resource.Id.menu_bill).SetChecked(true);
            ShowAccountName();
            SetToolbarTitle(Resource.String.bill_menu_activity_title);
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(selectedAccount.AccountNum);
            ShowBillMenu(selectedAccount);
            this.userActionsListener?.OnMenuSelect(Resource.Id.menu_bill);
        }

        public void ShowUnreadPromotions()
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    promotionMenuItem.SetIcon(Resource.Drawable.ic_menu_promotions_unread_selector);
                    bottomNavigationView.SetImageSize(28, 3);
                }
            }
        }

        public void HideUnreadPromotions()
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    promotionMenuItem.SetIcon(Resource.Drawable.ic_menu_promotions_selector);
                    bottomNavigationView.SetImageSize(28, 3);
                }
            }
        }

        public void ShowPromotionTimestamp(bool success)
        {
            if (success)
            {
                PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                List<PromotionsParentEntityV2> items = wtManager.GetAllItems();
                if (items != null)
                {
                    PromotionsParentEntityV2 entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedTimeStamp))
                        {
                            this.userActionsListener.OnGetPromotions();
                        }
                        else
                        {
                            ShowPromotion(true);
                        }
                    }
                }

            }
            else
            {
                ShowPromotion(false);
            }
        }

        public void OnSavedTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedTimeStamp = mSavedTimeStamp;
            }
            this.userActionsListener.OnGetPromotionsTimeStamp();
        }

        public void ShowPromotion(bool success)
        {
            if (success)
            {

                PromotionsEntityV2 wtManager = new PromotionsEntityV2();
                List<PromotionsEntityV2> items = wtManager.GetAllValidPromotions();
                List<PromotionsModelV2> promotions = new List<PromotionsModelV2>();
                if (items != null)
                {
                    promotions = new List<PromotionsModelV2>();
                    promotions.AddRange(items);
                }
                if (promotions.Count > 0)
                {
                    PromotionDialogFragmnet dialogFragmnet = new PromotionDialogFragmnet(this);
                    dialogFragmnet.Cancelable = false;
                    Bundle extras = new Bundle();
                    extras.PutString("promotions", JsonConvert.SerializeObject(promotions));
                    dialogFragmnet.Arguments = extras;
                    dialogFragmnet.Show(SupportFragmentManager, "Promotion Dialog");
                }
            }
        }

        public void NavigateToDashBoardFragment()
        {
            mPresenter.OnAccountSelectDashBoard();
        }

        public void ShowHomeDashBoard()
        {
            DashboardHomeActivity.GO_TO_INNER_DASHBOARD = false;
            currentFragment = new HomeMenuFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, new HomeMenuFragment())
                           .CommitAllowingStateLoss();
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
            if(flag)
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
                        HomeMenuFragment fragment = (HomeMenuFragment) FragmentManager.FindFragmentById(Resource.Id.content_layout);
                        fragment.OnSearchOutFocus();
                    }
                }
            }
            return base.DispatchTouchEvent(ev);
        }

        public void SetStatusBarBackground()
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                Drawable drawable = Resources.GetDrawable(Resource.Drawable.gradient_background);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetBackgroundDrawable(drawable);
            }
        }

        public void BillMenuRecalled()
        {
            try
            {
                this.mPresenter.BillMenuStartRefresh();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}
