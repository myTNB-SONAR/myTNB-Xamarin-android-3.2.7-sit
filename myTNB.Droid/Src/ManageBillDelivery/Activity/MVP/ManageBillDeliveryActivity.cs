using System;
using System.Collections.Generic;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.ViewPager.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using myTNB_Android.Src.DBR.DBRApplication.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MultipleAccountPayment.Fragment;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.DigitalBill.Activity;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    [Activity(Label = "@string/managebilldelivery_activity_title"
        , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Notification")]
    public class ManageBillDeliveryActivity : BaseActivityCustom, ViewPager.IOnPageChangeListener, ManageBillDeliveryContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txt_notification_name)]
        TextView txtNotificationName;

        [BindView(Resource.Id.selectAllCheckBox)]
        CheckBox selectAllCheckboxButton;

        [BindView(Resource.Id.btnRefresh)]
        Button btnNewRefresh;

        [BindView(Resource.Id.btnStartDigitalBill)]
        Button btnStartDigitalBill;

        [BindView(Resource.Id.btnUpdateDigitalBill)]
        Button btnUpdateDigitalBill;

        [BindView(Resource.Id.refresh_content)]
        TextView txtNewRefreshMessage;

        [BindView(Resource.Id.refresh_image)]
        ImageView refresh_image;

        [BindView(Resource.Id.viewPager)]
        ViewPager viewPager;

        [BindView(Resource.Id.digitalBillLabel)]
        TextView digitalBillLabel;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.applicationIndicator)]
        RelativeLayout applicationIndicator;

        [BindView(Resource.Id.viewPagerLyout)]
        FrameLayout viewPagerLyout;

        [BindView(Resource.Id.deliverigTitle)]
        TextView deliverigTitle;

        [BindView(Resource.Id.deliverigAddress)]
        TextView deliverigAddress;

        //[BindView(Resource.Id.pagerLyout)]
        //LinearLayout pagerLyout;
        

        ManageBillDeliveryContract.IUserActionsListener userActionsListener;

        private List<DBRAccount> dbrAccountList = new List<DBRAccount>();
        const string PAGE_ID = "ManageDigitalBillLanding";
        const string SELECTED_ACCOUNT_KEY = ".selectedAccount";
        public readonly static int DBR_SELECT_ACCOUNT_ACTIVITY_CODE = 8798;
        private string selectedAccountNumber;
        private DBRAccount selectedEligibleAccount;
        private string selectedAccountNickName;
        AccountData mSelectedAccountData;

        ManageBillDeliveryPresenter presenter;
        ManageBillDeliveryAdapter ManageBillDeliveryAdapter;
        string currentAppNavigation;

        //========================================== FORM LIFECYCLE ==================================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            presenter = new ManageBillDeliveryPresenter(this);
            viewPager = (ViewPager)FindViewById(Resource.Id.viewPager);
            viewPager.AddOnPageChangeListener(this);
            ManageBillDeliveryAdapter = new ManageBillDeliveryAdapter(SupportFragmentManager);

            ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList());

            viewPager.Adapter = ManageBillDeliveryAdapter;

            UpdateAccountListIndicator();
            Bundle extras = Intent.Extras;
            digitalBillLabel.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "whatIfIStillWantPaperBills");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "startDigitalBillCTA");
            TextViewUtils.SetMuseoSans500Typeface(digitalBillLabel, btnStartDigitalBill, deliverigTitle);
            TextViewUtils.SetMuseoSans300Typeface(deliverigAddress);
            TextViewUtils.SetTextSize12(digitalBillLabel);
            TextViewUtils.SetTextSize16(btnStartDigitalBill, deliverigTitle);
            TextViewUtils.SetTextSize14(deliverigAddress);

            dbrAccountList = this.presenter.GetEligibleDBRAccountList();
            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APP_NAVIGATION_KEY))
                {
                    currentAppNavigation = extras.GetString(Constants.APP_NAVIGATION_KEY);

                    ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList());

                    viewPager.Adapter = ManageBillDeliveryAdapter;

                    UpdateAccountListIndicator();

                }
                if (extras.ContainsKey(SELECTED_ACCOUNT_KEY))
                {
                    mSelectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString(SELECTED_ACCOUNT_KEY));
                    txtNotificationName.Text = mSelectedAccountData.AccountNickName;
                }
            }
            
            ScrollPage();

            btnStartDigitalBill.Click += delegate
            {
                   
                    InitiateDBRRequest();
            };
            float h1 = 490f;
            float h2 = 435f;
            FrameLayout.LayoutParams layout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
            layout.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(h1) : (int)DPUtils.ConvertDPToPx(h2);
        }
        public void InitiateDBRRequest()
        {
            try
            {
                StartActivity(typeof(DigitalBillActivity));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] InitiatePaymentRequest: " + e.Message);
                Utility.LoggingNonFatalError(e);
            }
        }
        public void ScrollPage()
        {
            System.Timers.Timer? timer = new System.Timers.Timer
            {
                Interval = 5000,
                Enabled = true
            };
            int page = 0;
            timer.Elapsed += (sender, args) =>
            {
                RunOnUiThread(() =>
                {
                    if (page <= viewPager.Adapter.Count)
                    {
                        page++;
                    }
                    else
                    {
                        page = 0;
                    }
                    viewPager.SetCurrentItem(page, true);
                });
            };
        }

        public string GetAppString(int id)
        {
            return this.GetString(id);
        }

        public void OnPageScrollStateChanged(int state)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] OnPageScrollStateChanged Error");
        }

        public void OnPageSelected(int position)
        {
            if (ManageBillDeliveryAdapter != null && ManageBillDeliveryAdapter.Count > 1)
            {
                for (int i = 0; i < ManageBillDeliveryAdapter.Count; i++)
                {
                    ImageView selectedDot = (ImageView)indicatorContainer.GetChildAt(i);
                    if (position == i)
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                    }
                    else
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                    }
                }
            }
        }

        private void UpdateAccountListIndicator()
        {
            if (ManageBillDeliveryAdapter != null && ManageBillDeliveryAdapter.Count > 1)
            {
                indicatorContainer.Visibility = ViewStates.Visible;
                for (int i = 0; i < ManageBillDeliveryAdapter.Count; i++)
                {
                    ImageView image = new ImageView(this);
                    image.Id = i;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.RightMargin = 8;
                    layoutParams.LeftMargin = 8;
                    image.LayoutParameters = layoutParams;
                    if (i == 0)
                    {
                        image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                    }
                    else
                    {
                        image.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                    }
                    indicatorContainer.AddView(image, i);
                }
            }
            else
            {
                applicationIndicator.Visibility = ViewStates.Gone;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Manage Bill Delivery");
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

        //===========================================================================================================================

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
        public override int ResourceId()
        {
            return Resource.Layout.ManageBillDelivery;
        }

        public void SetPresenter(ManageBillDeliveryContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        [OnClick(Resource.Id.txt_notification_name)]
        void OnNotificationFilter(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (dbrAccountList != null && dbrAccountList.Count > 0)
                {
                    this.presenter.CheckDBRAccountEligibility(dbrAccountList);
                }
                else
                {
                    Intent intent = new Intent(this, typeof(SelectDBRAccountActivity));
                    StartActivityForResult(intent, DBR_SELECT_ACCOUNT_ACTIVITY_CODE);
                }
            }
        }

        public void ShowDBREligibleAccountList(List<DBRAccount> dbrEligibleAccountList)
        {
            Intent intent = new Intent(this, typeof(SelectDBRAccountActivity));
            intent.PutExtra("DBR_ELIGIBLE_ACCOUNT_LIST", JsonConvert.SerializeObject(dbrEligibleAccountList));
            StartActivityForResult(intent, DBR_SELECT_ACCOUNT_ACTIVITY_CODE);
        }

        [OnClick(Resource.Id.digitalBillLabelContainer)]
        void OnTapManageBillDeliveryTooltip(object sender, EventArgs eventArgs)
        {
            ShowManageBillDeliveryPopup();
        }

        public void ShowManageBillDeliveryPopup()
        {
            MyTNBAppToolTipBuilder dbrTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER_TWO_BUTTON)
                .SetTitle(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "whatIfIStillWantPaperBillsTitle"))
                .SetMessage(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "whatIfIStillWantPaperBillsDetails"))
                .SetCTALabel(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "iWant"))
                .SetCTAaction(() => AddDigitalBill())
                .SetSecondaryCTALabel(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "nevermind"))
                .SetSecondaryCTAaction(() => { this.SetIsClicked(false); })
                .Build();
            dbrTooltip.Show();
        }

        public void AddDigitalBill()
        {

        }

        public void ShowSelectSupplyAccount()
        {
            this.SetIsClicked(true);
            Intent supplyAccount = new Intent(this, typeof(SelectDBRAccountActivity));
            supplyAccount.PutExtra(Constants.DBR_KEY, Constants.SELECT_ACCOUNT_DBR_REQUEST_CODE);
            StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {

            if (requestCode == DBR_SELECT_ACCOUNT_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    selectedAccountNumber = data.GetStringExtra("SELECTED_ACCOUNT_NUMBER");
                    foreach (DBRAccount account in dbrAccountList)
                    {
                        if (account.accountNumber == selectedAccountNumber)
                        {
                            selectedEligibleAccount = account;
                            account.accountSelected = true;
                        }
                        else
                        {
                            account.accountSelected = false;
                        }
                    }
                    selectedAccountNickName = selectedEligibleAccount.accountName;
                    txtNotificationName.Text = selectedAccountNickName;
                    CustomerBillingAccount.RemoveSelected();
                    CustomerBillingAccount.SetSelected(selectedEligibleAccount.accountNumber);
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
        public void SetAccountName(string accountName)
        {
            txtNotificationName.Text = accountName;
        }

        public void ShowRefreshView(bool isRefresh, string contentTxt, string btnTxt)
        {
            try
            {
                FindViewById(Resource.Id.emptyLayout).Visibility = ViewStates.Gone;
                btnNewRefresh.Text = string.IsNullOrEmpty(btnTxt) ? GetLabelCommonByLanguage("refreshNow") : btnTxt;

                if (isRefresh)
                {
                    refresh_image.SetImageResource(Resource.Drawable.refresh_1);

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt)
                            ? Html.FromHtml(Utility.GetLocalizedErrorLabel("refreshMessage"), FromHtmlOptions.ModeLegacy)
                            : Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt)
                            ? Html.FromHtml(Utility.GetLocalizedErrorLabel("refreshMessage"))
                            : Html.FromHtml(contentTxt);
                    }

                    btnNewRefresh.Visibility = ViewStates.Visible;
                }
                else
                {

                    refresh_image.SetImageResource(Resource.Drawable.maintenance_new);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt)
                            ? Html.FromHtml(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"), FromHtmlOptions.ModeLegacy)
                            : Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt)
                            ? Html.FromHtml(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"))
                            : Html.FromHtml(contentTxt);
                    }

                    btnNewRefresh.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            this.userActionsListener.SelectSupplyAccount();
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override void OnBackPressed()
        {
            try
            {
                base.OnBackPressed();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }
    }
}