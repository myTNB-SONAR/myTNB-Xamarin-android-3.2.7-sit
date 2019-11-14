
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;
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
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.MoreMenu;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP;
using myTNB_Android.Src.MyTNBService.Notification;
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
using static Android.Views.View;

namespace myTNB_Android.Src.myTNBMenu.Activity
{
    [Activity(Label = "@string/dashboard_activity_title"
              , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        ,Theme = "@style/Theme.DashboardHome"
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

        private bool isFromNotification = false;

        private LoadingOverlay loadingOverlay;

        private string savedTimeStamp = "0000000";

        public static Fragment currentFragment;

        public static bool GO_TO_INNER_DASHBOARD = false;

        private bool isSetToolbarClick = false;

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

            // Lin Siong Note to Sprint 3: Remove this on Sprint 3
            if (bottomNavigationView.Menu.FindItem(Resource.Id.menu_reward) != null)
            {
                bottomNavigationView.Menu.RemoveItem(Resource.Id.menu_reward);
            }

            bottomNavigationView.SetShiftMode(false, false);
            bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
            bottomNavigationView.ItemIconTintList = null;

            bottomNavigationView.NavigationItemSelected += BottomNavigationView_NavigationItemSelected;

            Bundle extras = Intent?.Extras;
            if (extras != null && extras.ContainsKey(Constants.PROMOTION_NOTIFICATION_VIEW))
            {
                bottomNavigationView.Menu.FindItem(Resource.Id.menu_promotion).SetChecked(true);
                this.userActionsListener?.OnMenuSelect(Resource.Id.menu_promotion);
            }

            if (extras != null && extras.ContainsKey("FROM_NOTIFICATION"))
            {
                mPresenter.OnAccountSelectDashBoard();
                isFromNotification = true;
                alreadyStarted = true;
            }

            if (!alreadyStarted)
            {
                this.userActionsListener.Start();
                alreadyStarted = true;
                ShowPromotion(true);
            }

            this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Click += DashboardHomeActivity_Click;

            ShowUnreadRewards();
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
            ShowBackButton(false);
            //this.SelectedAccountData = selectedAccount;
            //txtAccountName.Text = SelectedAccountData.AccountNickName;
            //currentFragment = new BillsMenuFragment();
            //FragmentManager.BeginTransaction()
            //    .Replace(Resource.Id.content_layout, BillsMenuFragment.NewInstance(selectedAccount))
            //    .CommitAllowingStateLoss();
            txtAccountName.Visibility = ViewStates.Gone;
            currentFragment = new ItemisedBillingMenuFragment();
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, ItemisedBillingMenuFragment.NewInstance(selectedAccount))
                .CommitAllowingStateLoss();
        }

        public void SetToolbarTitle(int stringResourceId)
        {
            try
            {
                this.toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = GetString(stringResourceId);
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

        public void ShowUnreadPromotions(bool flag)
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    if (UserSessions.HasWhatNewShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        int count = PromotionsEntityV2.Count();
                        if (count > 0)
                        {
                            SetReadUnReadNewBottomView(flag, true, count, promotionMenuItem);
                        }
                        else
                        {
                            HideUnreadPromotions(flag);
                        }
                    }
                    else
                    {
                        SetNewWhatsNewBottomView(flag, "New", promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void ShowUnreadPromotions()
        {
            if (bottomNavigationView != null && bottomNavigationView.Menu != null)
            {
                IMenu bottomMenu = bottomNavigationView.Menu;

                IMenuItem promotionMenuItem = bottomMenu.FindItem(Resource.Id.menu_promotion);
                if (promotionMenuItem != null)
                {
                    if (UserSessions.HasWhatNewShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        int count = PromotionsEntityV2.Count();
                        if (count > 0)
                        {
                            SetReadUnReadNewBottomView(promotionMenuItem.IsChecked, true, count, promotionMenuItem);
                        }
                        else
                        {
                            HideUnreadPromotions(promotionMenuItem.IsChecked);
                        }
                    }
                    else
                    {
                        SetNewWhatsNewBottomView(promotionMenuItem.IsChecked, "New", promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        public void HideUnreadPromotions(bool flag)
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
                        SetNewWhatsNewBottomView(flag, "New", promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
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
                    if (UserSessions.HasWhatNewShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        SetReadUnReadNewBottomView(promotionMenuItem.IsChecked, false, 0, promotionMenuItem);
                    }
                    else
                    {
                        SetNewWhatsNewBottomView(promotionMenuItem.IsChecked, "New", promotionMenuItem);
                    }
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }

        private void SetReadUnReadNewBottomView(bool flag, bool isGotRead, int count,IMenuItem promotionMenuItem)
        {
            RunOnUiThread(() =>
            {
                View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayout, null, false);
                LinearLayout newLabel = v.FindViewById<LinearLayout>(Resource.Id.newLabel);
                TextView txtNewLabel = v.FindViewById<TextView>(Resource.Id.txtNewLabel);
                ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                if (isGotRead && count > 0)
                {
                    newLabel.Visibility = ViewStates.Visible;
                    newLabel.SetBackgroundResource(Resource.Drawable.notification_indication_bg);
                    TextViewUtils.SetMuseoSans500Typeface(txtNewLabel);
                    RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                    newLabelParam.TopMargin = 0;
                    newLabelParam.Height = (int)DPUtils.ConvertDPToPx(16f);
                    txtNewLabel.SetTextSize(Android.Util.ComplexUnitType.Dip, 10f);
                    txtNewLabel.Text = count.ToString();
                    txtNewLabel.SetTextColor(Resources.GetColor(Resource.Color.white));
                    if (count > 0 && count <= 9)
                    {
                        newLabelParam.Width = (int)DPUtils.ConvertDPToPx(14f);
                        newLabelParam.LeftMargin = (int)DPUtils.ConvertDPToPx(-16f);
                    }
                    else
                    {
                        if (count > 99)
                        {
                            txtNewLabel.Text = "99+";
                        }
                        newLabelParam.Width = (int)DPUtils.ConvertDPToPx(18f);
                        newLabelParam.LeftMargin = (int)DPUtils.ConvertDPToPx(-20f);
                    }

                    if (!flag)
                    {
                        bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo);
                    }
                    else
                    {
                        bottomImg.SetImageResource(Resource.Drawable.ic_menu_promo_toggled);
                    }
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
                Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(28f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                Canvas c = new Canvas(b);
                v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(28f), (int)DPUtils.ConvertDPToPx(28f));
                v.Draw(c);

                var bitmapDrawable = new BitmapDrawable(b);
                promotionMenuItem.SetIcon(bitmapDrawable);
            });
        }

        private void SetNewWhatsNewBottomView(bool flag, string word, IMenuItem promotionMenuItem)
        {
            RunOnUiThread(() =>
            {
                View v = this.LayoutInflater.Inflate(Resource.Layout.BottomViewNavigationItemLayout, null, false);
                LinearLayout newLabel = v.FindViewById<LinearLayout>(Resource.Id.newLabel);
                TextView txtNewLabel = v.FindViewById<TextView>(Resource.Id.txtNewLabel);
                ImageView bottomImg = v.FindViewById<ImageView>(Resource.Id.bottomViewImg);
                newLabel.Visibility = ViewStates.Visible;
                newLabel.SetBackgroundResource(Resource.Drawable.new_label);
                RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                newLabelParam.Width = (int)DPUtils.ConvertDPToPx(26f);
                newLabelParam.LeftMargin = (int)DPUtils.ConvertDPToPx(-15f);
                newLabelParam.Height = (int)DPUtils.ConvertDPToPx(14f);
                newLabelParam.TopMargin = 0;
                txtNewLabel.SetTextSize(Android.Util.ComplexUnitType.Dip, 8f);
                txtNewLabel.Text = word;
                txtNewLabel.SetTextColor(Resources.GetColor(Resource.Color.charcoalGrey));
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
                Bitmap b = Bitmap.CreateBitmap((int)DPUtils.ConvertDPToPx(38f), (int)DPUtils.ConvertDPToPx(28f), Bitmap.Config.Argb8888);
                Canvas c = new Canvas(b);
                v.Layout(0, 0, (int)DPUtils.ConvertDPToPx(38f), (int)DPUtils.ConvertDPToPx(28f));
                v.Draw(c);

                var bitmapDrawable = new BitmapDrawable(b);
                promotionMenuItem.SetIcon(bitmapDrawable);
            });
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
                try
                {
                    DashboardChartFragment fragment = (DashboardChartFragment)FragmentManager.FindFragmentById(Resource.Id.content_layout);
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
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            return base.DispatchTouchEvent(ev);
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

        public void OnSelectAccount()
        {
            this.userActionsListener.SelectSupplyAccount();
        }

        public void ShowNMREChart(UsageHistoryResponse response, AccountData selectedAccount, string errorCode, string errorMsg)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountNickName;
            currentFragment = new DashboardChartFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(response, selectedAccount, errorCode, errorMsg),
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
        }

        public void ShowSMChart(SMUsageHistoryResponse response, AccountData selectedAccount)
        {
            this.SelectedAccountData = selectedAccount;
            txtAccountName.Text = SelectedAccountData.AccountNickName;
            currentFragment = new DashboardChartFragment();
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(response, selectedAccount),
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
            ShowBackButton(true);
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
                    rewardMenuItem.SetIcon(Resource.Drawable.ic_menu_reward_unread_selector);
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
                    rewardMenuItem.SetIcon(Resource.Drawable.ic_menu_reward_selector);
                    bottomNavigationView.SetImageFontSize(this, 28, 3, 10f);
                }
            }
        }
    }
}
