using System;
using System.Collections.Generic;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
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
using myTNB_Android.Src.DigitalBill.Activity;
using Android.Preferences;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB.Mobile;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.ManageBillDelivery.ManageBillDeliveryEmailList.Adapter;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.SessionCache;
using Android.Graphics;
using static myTNB.Mobile.MobileEnums;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    [Activity(Label = "@string/managebilldelivery_activity_title"
        , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Notification")]
    public class PaperBillDeliveryActivity : BaseActivityCustom
        , ViewPager.IOnPageChangeListener
        , ManageBillDeliveryContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.selectAccountContainer)]
        LinearLayout selectAccountContainer;

        [BindView(Resource.Id.txtSelectedAccountTitle)]
        TextView txtSelectedAccountTitle;

        [BindView(Resource.Id.txt_ca_name)]
        TextView txt_ca_name;

        [BindView(Resource.Id.ic_ca_info)]
        ImageView ic_ca_info;

        [BindView(Resource.Id.viewPagerLyout)]
        FrameLayout viewPagerLyout;

        [BindView(Resource.Id.viewPager)]
        ViewPager viewPager;


        [BindView(Resource.Id.applicationIndicator)]
        RelativeLayout applicationIndicator;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.btnStartDigitalBillLayout)]
        LinearLayout btnStartDigitalBillLayout;

        [BindView(Resource.Id.deliveryLayout)]
        LinearLayout deliveryLayout;

        [BindView(Resource.Id.btnStartDigitalBill)]
        Button btnStartDigitalBill;

        [BindView(Resource.Id.deliverigTitle)]
        TextView deliverigTitle;

        [BindView(Resource.Id.image_layout)]
        LinearLayout image_layout;

        [BindView(Resource.Id.deliverigAddress)]
        TextView deliverigAddress;

        private const string PAGE_ID = "ManageDigitalBillLanding";
        private ManageBillDeliveryEmailListAdapter manageBillDeliveryEmailListAdapter;
        private GetBillRenderingResponse _billRenderingResponse;
        private RecyclerView.LayoutManager layoutManager;
        private ISharedPreferences mPref;
        private bool _isOwner { get; set; }
        private ManageBillDeliveryContract.IUserActionsListener userActionsListener;
        private List<DBRAccount> dbrEligibleAccountList = new List<DBRAccount>();
        public readonly static int DBR_SELECT_ACCOUNT_ACTIVITY_CODE = 8798;
        private string selectedAccountNumber;
        private DBRAccount selectedEligibleAccount;
        private string selectedAccountNickName;
        private AccountData mSelectedAccountData;
        private ManageBillDeliveryPresenter presenter;
        private ManageBillDeliveryAdapter ManageBillDeliveryAdapter;
        private string currentAppNavigation;
        private string _accountNumber = string.Empty;
        LinearLayout.LayoutParams emailLayout;
        FrameLayout.LayoutParams viewerlayout;

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
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "startDigitalBillCTA");
            deliverigTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "deliveringToTitle");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "goPaperlessCTA");
            txtSelectedAccountTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "selectAccount");
            TextViewUtils.SetMuseoSans500Typeface( btnStartDigitalBill, deliverigTitle);
            TextViewUtils.SetMuseoSans300Typeface(deliverigAddress, txtSelectedAccountTitle);
            TextViewUtils.SetTextSize12( txtSelectedAccountTitle);
            TextViewUtils.SetTextSize16(btnStartDigitalBill, deliverigTitle);
            TextViewUtils.SetTextSize14(deliverigAddress);

            if (extras != null)
            {
                if (extras.ContainsKey("accountNumber"))
                {
                    _accountNumber = extras.GetString("accountNumber");
                    List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
                    int accountIndex = allAccountList.FindIndex(x => x.AccNum == _accountNumber);
                    if (accountIndex > -1)
                    {
                        mSelectedAccountData = AccountData.Copy(allAccountList[accountIndex], true);
                        txt_ca_name.Text = mSelectedAccountData.AccountNickName + " - " + mSelectedAccountData.AccountNum;
                        deliverigAddress.Text = mSelectedAccountData.AddStreet;
                        selectedAccountNumber = mSelectedAccountData.AccountNum;
                    }
                }
                if (extras.ContainsKey("isOwner"))
                {
                    _isOwner = extras.GetBoolean("isOwner");
                }
                if (extras.ContainsKey("billRenderingResponse"))
                {
                    _billRenderingResponse = JsonConvert.DeserializeObject<GetBillRenderingResponse>(extras.GetString("billRenderingResponse"));
                    GetDeliveryDisplay(_billRenderingResponse);
                }
                if (extras.ContainsKey(Constants.APP_NAVIGATION_KEY))
                {
                    currentAppNavigation = extras.GetString(Constants.APP_NAVIGATION_KEY);
                    ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList());
                    viewPager.Adapter = ManageBillDeliveryAdapter;
                    UpdateAccountListIndicator();
                }

                SetToolBarTitle(GetLabelByLanguage(_isOwner ? "title" : "dbrViewBillDelivery"));

               /* emailLayout = email_layout.LayoutParameters as LinearLayout.LayoutParams;
                emailLayout.Height = TextViewUtils.IsLargeFonts
                    ? (int)DPUtils.ConvertDPToPx(520f + (float)(deliverigAddress.Text.Length / 1.7))
                    : (int)DPUtils.ConvertDPToPx(465f);

                viewerlayout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
                viewerlayout.Height = TextViewUtils.IsLargeFonts
                    ? (int)DPUtils.ConvertDPToPx(380f)
                    : (int)DPUtils.ConvertDPToPx(255f);*/
            }
            btnStartDigitalBill.Click += delegate
            {
                OnDisplayMicrosite();
            };
        }

        public void GetDeliveryDisplay(GetBillRenderingResponse getBillRenderingModel)
        {
            if (getBillRenderingModel != null && getBillRenderingModel.Content != null)
            {
                UserEntity user = UserEntity.GetActive();
                
                    
                  if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.Paper)
                    {
                        viewPagerLyout.Visibility = ViewStates.Visible;
                        if (_isOwner)
                        {
                            ic_ca_info.Visibility
                                = applicationIndicator.Visibility
                                = btnStartDigitalBillLayout.Visibility
                                = applicationIndicator.Visibility
                                = indicatorContainer.Visibility
                                = viewPager.Visibility
                                = deliverigAddress.Visibility
                                = ViewStates.Visible;
                           
                            UserSessions.ManageBillDelivery = MobileEnums.DBRTypeEnum.Paper;
                           
                            ScrollPage();
                        }
                        else
                        {
                            ic_ca_info.Visibility
                                = applicationIndicator.Visibility
                                = btnStartDigitalBillLayout.Visibility
                                = applicationIndicator.Visibility
                                = indicatorContainer.Visibility
                                = viewPager.Visibility
                                = deliverigAddress.Visibility
                                = ViewStates.Gone;
                            deliverigTitle.Visibility
                                = ViewStates.Visible;
                        }
                    }
                    else if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.None)
                    {
                        MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(Utility.GetLocalizedLabel("Error", "defaultErrorTitle"))
                            .SetMessage(Utility.GetLocalizedLabel("Error", "defaultErrorMessage"))
                            .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                            .Build();
                        errorPopup.Show();
                    }
                    SetDynatraceScreenTags(getBillRenderingModel);
                
            }

        }

        private void SetDynatraceScreenTags(GetBillRenderingResponse billRenderingResponse)
        {
            if (billRenderingResponse == null
                || _billRenderingResponse.Content == null)
            {
                return;
            }
            string dynatraceTag = string.Empty;
            if (billRenderingResponse.Content.IsPostConversion)
            {
                if (billRenderingResponse.Content.PreviousRenderingMethod == RenderingMethodEnum.None
                    && billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill)
                {
                    dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.Post_EBill;
                }
                else if (billRenderingResponse.Content.PreviousRenderingMethod == RenderingMethodEnum.EBill_Email_Paper
                   && billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill_Email)
                {
                    dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.Post_EBill_Email;
                }
            }
            else
            {
                if (billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.Paper
                    || billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill_Paper)
                {
                    dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.Pre_EBill_Paper;
                }
                else if (billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill_Email_Paper)
                {
                    dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.Pre_EBill_Email_Paper;
                }
            }
            if (dynatraceTag.IsValid())
            {
                DynatraceHelper.OnTrack(dynatraceTag);
            }
        }

        private void SetDynatraceCTATags()
        {
            if (_billRenderingResponse == null
                || _billRenderingResponse.Content == null)
            {
                return;
            }
            string dynatraceTag = string.Empty;
            if (_billRenderingResponse.Content.IsPostConversion)
            {
                if (_billRenderingResponse.Content.PreviousRenderingMethod == RenderingMethodEnum.None
                    && _billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill)
                {
                    dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.Post_EBill;
                }
                else if (_billRenderingResponse.Content.PreviousRenderingMethod == RenderingMethodEnum.EBill_Email_Paper
                   && _billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill_Email)
                {
                    dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.Post_EBill_Email;
                }
            }
            else
            {
                if (_billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.Paper
                    || _billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill_Paper)
                {
                    dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.Pre_EBill_Paper;
                }
                else if (_billRenderingResponse.Content.CurrentRenderingMethod == RenderingMethodEnum.EBill_Email_Paper)
                {
                    dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.Pre_EBill_Email_Paper;
                }
            }
            if (dynatraceTag.IsValid())
            {
                DynatraceHelper.OnTrack(dynatraceTag);
            }
        }

        public void OnDisplayMicrosite()
        {
            try
            {
                SetDynatraceCTATags();
                Intent intent = new Intent(this, typeof(DigitalBillActivity));
                intent.PutExtra(Constants.SELECTED_ACCOUNT, _accountNumber);
                intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(_billRenderingResponse));
                StartActivity(intent);
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
            return Resource.Layout.PaperBillDelivery;
        }

        public void SetPresenter(ManageBillDeliveryContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        [OnClick(Resource.Id.txt_ca_name)]
        void OnCANameFilter(object sender, EventArgs eventArgs)
        {
            dbrEligibleAccountList = GetEligibleDBRAccountList();
            if (dbrEligibleAccountList != null && dbrEligibleAccountList.Count > 0)
            {
                this.presenter.CheckDBRAccountEligibility(dbrEligibleAccountList);
            }
            else
            {
                // Intent intent = new Intent(this, typeof(SelectDBRAccountActivity));
                //StartActivityForResult(intent, DBR_SELECT_ACCOUNT_ACTIVITY_CODE);
            }
        }

        public void ShowDBREligibleAccountList(List<DBRAccount> dbrEligibleAccountList)
        {
            foreach (DBRAccount account in dbrEligibleAccountList)
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

            Intent intent = new Intent(this, typeof(SelectDBRAccountActivity));
            intent.PutExtra("DBR_ELIGIBLE_ACCOUNT_LIST", JsonConvert.SerializeObject(dbrEligibleAccountList));
            intent.PutExtra("SELECTED_ACCOUNT_NUMBER", JsonConvert.SerializeObject(selectedAccountNumber));
            StartActivityForResult(intent, DBR_SELECT_ACCOUNT_ACTIVITY_CODE);
        }

       
        [OnClick(Resource.Id.ic_ca_info)]
        void OnDisplayNotEligibleTooltip(object sender, EventArgs eventArgs)
        {
            bool isPilot = EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                , EligibilitySessionCache.FeatureProperty.TargetGroup);

            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetTitle(Utility.GetLocalizedLabel("ManageDigitalBillLanding", isPilot ? "notEligibleAccountsTitlePilot" : "notEligibleAccountsTitleNationwide"))
                .SetMessage(Utility.GetLocalizedLabel("ManageDigitalBillLanding", isPilot ? "notEligibleAccountsMessagePilot" : "notEligibleAccountsMessageNationwide"))
                .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build()
                .Show();
        }

        public void ShowManageBillDeliveryTooltip()
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

        public void ShowManageBillDeliveryPopup()
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetTitle(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "nonOwnerPreLaunchAnotherDeliveryMethodTitle"))
                .SetMessage(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "nonOwnerPreLaunchAnotherDeliveryMethodDetails"))
                .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build()
                .Show();
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
                    _billRenderingResponse = JsonConvert.DeserializeObject<GetBillRenderingResponse>(data.GetStringExtra("billrenderingresponse"));
                    if (_billRenderingResponse != null
                        && _billRenderingResponse.StatusDetail != null
                        && _billRenderingResponse.StatusDetail.IsSuccess
                        && _billRenderingResponse.Content != null)
                    {
                        _isOwner = DBRUtility.Instance.IsCADBREligible(selectedAccountNumber);
                        _accountNumber = selectedAccountNumber;
                        SetToolBarTitle(GetLabelByLanguage(_isOwner ? "title" : "dbrViewBillDelivery"));
                        GetDeliveryDisplay(_billRenderingResponse);
                    }
                    foreach (DBRAccount account in dbrEligibleAccountList)
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
                    txt_ca_name.Text = selectedAccountNickName + " - " + selectedEligibleAccount.accountNumber;
                    deliverigAddress.Text = selectedEligibleAccount.accountAddress;
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public void SetAccountName(CustomerBillingAccount selectedAccount)
        {
            txt_ca_name.Text = selectedAccount.AccDesc + " - " + selectedAccount.AccNum;
            deliverigAddress.Text = selectedAccount.AccountStAddress;
            /*if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.Paper)
            {
                FrameLayout.LayoutParams layout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts
                    ? (int)DPUtils.ConvertDPToPx(510f + (float)(deliverigAddress.Text.Length / 1.7))
                    : (int)DPUtils.ConvertDPToPx(455f);
            }*/
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
       
       

        public List<DBRAccount> GetEligibleDBRAccountList()
        {
            List<string> dBRCAs = EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                , EligibilitySessionCache.FeatureProperty.TargetGroup)
                    ? DBRUtility.Instance.GetDBRCAs()
                    : AccountTypeCache.Instance.DBREligibleCAs;
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            List<CustomerBillingAccount> eligibleDBRAccountList = new List<CustomerBillingAccount>();
            CustomerBillingAccount account = new CustomerBillingAccount();
            dbrEligibleAccountList = new List<DBRAccount>();

            if (dBRCAs.Count > 0)
            {
                ShowProgressDialog();
                foreach (var dbrca in dBRCAs)
                {
                    account = allAccountList.Where(x => x.AccNum == dbrca).FirstOrDefault();
                    eligibleDBRAccountList.Add(account);
                }

                DBRAccount dbrEligibleAccount;
                eligibleDBRAccountList.ForEach(account =>
                {
                    dbrEligibleAccount = new DBRAccount
                    {
                        accountNumber = account.AccNum,
                        accountName = account.AccDesc,
                        accountSelected = account.IsSelected,
                        isTaggedSMR = account.IsTaggedSMR,
                        accountAddress = account.AccountStAddress.ToUpper(),
                        accountOwnerName = account.OwnerName
                    };
                    dbrEligibleAccountList.Add(dbrEligibleAccount);
                });
                HideProgressDialog();
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
            return dbrEligibleAccountList;
        }
        
        public int GetEmailDeliveryHeight()
        {
            return deliveryLayout.Height;
        }

        public int GetviewPagerHeight()
        {
            return viewPagerLyout.Height;
        }
        
        public int GetSelectAccountContainerHeight()
        {
            return selectAccountContainer.Height;
        }
        
    }
}



