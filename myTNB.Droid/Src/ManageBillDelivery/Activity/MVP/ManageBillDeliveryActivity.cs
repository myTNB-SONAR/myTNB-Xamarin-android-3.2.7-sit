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
    public class ManageBillDeliveryActivity : BaseActivityCustom
        , ViewPager.IOnPageChangeListener
        , ManageBillDeliveryContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txt_ca_name)]
        TextView txt_ca_name;

        [BindView(Resource.Id.btnStartDigitalBill)]
        Button btnStartDigitalBill;

        [BindView(Resource.Id.btnUpdateDigitalBill)]
        Button btnUpdateDigitalBill;

        [BindView(Resource.Id.viewPager)]
        ViewPager viewPager;

        [BindView(Resource.Id.digitalBillLabel)]
        TextView digitalBillLabel;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.btnStartDigitalBillLayout)]
        LinearLayout btnStartDigitalBillLayout;

        [BindView(Resource.Id.email_layout)]
        LinearLayout email_layout;

        [BindView(Resource.Id.email_container)]
        LinearLayout email_container;

        [BindView(Resource.Id.image_layout)]
        LinearLayout image_layout;

        [BindView(Resource.Id.deliveryLayout)]
        LinearLayout deliveryLayout;

        [BindView(Resource.Id.btnUpdateDigitalBillLayout)]
        LinearLayout btnUpdateDigitalBillLayout;

        [BindView(Resource.Id.digitalBillLabelContainer)]
        LinearLayout digitalBillLabelContainer;

        [BindView(Resource.Id.selectAccountContainer)]
        LinearLayout selectAccountContainer;

        [BindView(Resource.Id.digitalBillLabelLayout)]
        LinearLayout digitalBillLabelLayout;

        [BindView(Resource.Id.applicationIndicator)]
        RelativeLayout applicationIndicator;

        [BindView(Resource.Id.viewPagerLyout)]
        FrameLayout viewPagerLyout;

        [BindView(Resource.Id.deliverigTitle)]
        TextView deliverigTitle;

        [BindView(Resource.Id.deliverigAddress)]
        TextView deliverigAddress;

        [BindView(Resource.Id.TenantDeliverigAddress)]
        TextView TenantDeliverigAddress;

        [BindView(Resource.Id.img_display)]
        ImageView img_display;

        [BindView(Resource.Id.txtTitle)]
        TextView txtTitle;

        [BindView(Resource.Id.txtMessage)]
        TextView txtMessage;

        [BindView(Resource.Id.ic_ca_info)]
        ImageView ic_ca_info;

        [BindView(Resource.Id.txtSelectedAccountTitle)]
        TextView txtSelectedAccountTitle;

        [BindView(Resource.Id.manageBillDeliveryEmailRecyclerView)]
        RecyclerView manageBillDeliveryEmailRecyclerView;

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
            digitalBillLabel.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "anotherDeliveryMethod");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "startDigitalBillCTA");
            deliverigTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "deliveringToTitle");
            btnUpdateDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "updateBillDeliveryMethodCTA");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "goPaperlessCTA");
            txtSelectedAccountTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "selectAccount");
            TextViewUtils.SetMuseoSans500Typeface(digitalBillLabel, btnStartDigitalBill, deliverigTitle, txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(deliverigAddress, TenantDeliverigAddress, txtMessage, txtSelectedAccountTitle);
            TextViewUtils.SetTextSize12(digitalBillLabel, txtSelectedAccountTitle);
            TextViewUtils.SetTextSize16(btnStartDigitalBill, deliverigTitle, txtTitle);
            TextViewUtils.SetTextSize14(deliverigAddress, TenantDeliverigAddress, txtMessage);
            
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
                        TenantDeliverigAddress.Text = mSelectedAccountData.AddStreet;
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
            }
            btnStartDigitalBill.Click += delegate
            {
                InitiateDBRRequest(mSelectedAccountData);
            };
        }

        public void GetDeliveryDisplay(GetBillRenderingResponse getBillRenderingModel)
        {
            UserEntity user = UserEntity.GetActive();
            TenantDeliverigAddress.Visibility = ViewStates.Gone;
            if (getBillRenderingModel.Content.IsInProgress)
            {
                FrameLayout.LayoutParams layout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts
                    ? (int)DPUtils.ConvertDPToPx(520f + (float)(deliverigAddress.Text.Length / 1.7))
                    : (int)DPUtils.ConvertDPToPx(465f);
                applicationIndicator.Visibility
                    = btnStartDigitalBillLayout.Visibility
                    = applicationIndicator.Visibility
                    = indicatorContainer.Visibility
                    = viewPager.Visibility
                    = deliverigAddress.Visibility
                    = email_container.Visibility
                    = deliverigTitle.Visibility
                    = btnUpdateDigitalBillLayout.Visibility
                    = digitalBillLabelLayout.Visibility
                    = digitalBillLabelContainer.Visibility
                    = ViewStates.Gone;
                email_layout.Visibility = ViewStates.Visible;
                img_display.SetImageResource(Resource.Drawable.dbr_inprogress);
                txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "updatingTitle");
                txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "updatingDescription"));
                if (!_isOwner)
                {
                    digitalBillLabelContainer.Visibility = ic_ca_info.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.EBill)
                {
                    FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                    layout.Height = TextViewUtils.IsLargeFonts
                        ? (int)DPUtils.ConvertDPToPx(296f)
                        : (int)DPUtils.ConvertDPToPx(295f);
                    applicationIndicator.Visibility
                        = btnStartDigitalBillLayout.Visibility
                        = applicationIndicator.Visibility
                        = indicatorContainer.Visibility
                        = viewPager.Visibility
                        = deliverigAddress.Visibility
                        = email_container.Visibility
                        = deliverigTitle.Visibility
                        = btnUpdateDigitalBillLayout.Visibility
                        = ViewStates.Gone;
                    email_layout.Visibility
                        = digitalBillLabelLayout.Visibility
                        = digitalBillLabelContainer.Visibility
                        = ViewStates.Visible;
                    img_display.SetImageResource(Resource.Drawable.icons_display_digital_ebill);
                    txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "eBillTitle" : "nonOwnerEBillTitle");
                    txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "eBillDescription" : "nonOwnerEBillDescription"));
                    if (!_isOwner)
                    {
                        digitalBillLabelContainer.Visibility = ic_ca_info.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                    }
                    mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        NewAppTutorialUtils.ForceCloseNewAppTutorial();
                        if (!UserSessions.HasManageEBillDeliveryTutorialShown(this.mPref))
                        {
                            UserSessions.ManageBillDelivery = MobileEnums.DBRTypeEnum.EBill;
                            OnShowManageBillDeliveryTutorialDialog();
                        }
                    };
                    h.PostDelayed(myAction, 50);

                }
                else if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.EBillWithCTA)
                {
                    FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                    layout.Height = TextViewUtils.IsLargeFonts
                        ? (int)DPUtils.ConvertDPToPx(280f)
                        : (int)DPUtils.ConvertDPToPx(265f);
                    applicationIndicator.Visibility
                        = btnStartDigitalBillLayout.Visibility
                        = applicationIndicator.Visibility
                        = indicatorContainer.Visibility
                        = viewPager.Visibility
                        = deliverigAddress.Visibility
                        = email_container.Visibility
                        = digitalBillLabelContainer.Visibility
                        = digitalBillLabelLayout.Visibility
                        = deliverigTitle.Visibility
                        = ViewStates.Gone;
                    email_layout.Visibility
                        = btnUpdateDigitalBillLayout.Visibility
                        = ic_ca_info.Visibility
                        = ViewStates.Visible;
                    img_display.SetImageResource(Resource.Drawable.icons_display_digital_ebill);
                    txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "eBillTitle" : "nonOwnerEBillTitle");
                    txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "eBillDescription" : "nonOwnerEBillDescription"));
                    mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    if (_isOwner)
                    {
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            NewAppTutorialUtils.ForceCloseNewAppTutorial();
                            if (!UserSessions.HasManageOptedEBillDeliveryTutorialShown(this.mPref))
                            {
                                UserSessions.ManageBillDelivery = MobileEnums.DBRTypeEnum.EBillWithCTA;
                                OnShowManageBillDeliveryTutorialDialog();
                            }
                        };
                        h.PostDelayed(myAction, 50);
                    }
                    else
                    {
                        btnUpdateDigitalBillLayout.Visibility = digitalBillLabelContainer.Visibility = ic_ca_info.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                    }

                }
                else if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.Email)
                {
                    layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    manageBillDeliveryEmailRecyclerView.SetLayoutManager(layoutManager);
                    manageBillDeliveryEmailListAdapter = new ManageBillDeliveryEmailListAdapter(this, getBillRenderingModel.Content.EmailList);
                    manageBillDeliveryEmailRecyclerView.SetAdapter(manageBillDeliveryEmailListAdapter);
                    FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                    layout.Height = TextViewUtils.IsLargeFonts
                        ? (int)DPUtils.ConvertDPToPx(300f)
                        : (int)DPUtils.ConvertDPToPx(310f);
                    TenantDeliverigAddress.Visibility
                        = applicationIndicator.Visibility
                        = btnStartDigitalBillLayout.Visibility
                        = applicationIndicator.Visibility
                        = indicatorContainer.Visibility
                        = viewPager.Visibility
                        = btnUpdateDigitalBillLayout.Visibility
                        = deliverigAddress.Visibility
                        = ViewStates.Gone;
                    email_layout.Visibility
                        = email_container.Visibility
                        = digitalBillLabelLayout.Visibility
                        = digitalBillLabelContainer.Visibility
                        = deliverigTitle.Visibility
                        = ViewStates.Visible;

                    if (getBillRenderingModel.Content.EmailList.Count >= 2)
                    {
                        layout.Height = TextViewUtils.IsLargeFonts
                       ? layout.Height + manageBillDeliveryEmailRecyclerView.Height
                       : layout.Height + manageBillDeliveryEmailRecyclerView.Height;
                    }
                    else
                    {
                        layout.Height = TextViewUtils.IsLargeFonts
                         ? layout.Height + manageBillDeliveryEmailRecyclerView.Height + (int)DPUtils.ConvertDPToPx(45f)
                         : layout.Height + manageBillDeliveryEmailRecyclerView.Height + +(int)DPUtils.ConvertDPToPx(50f);
                    }

                    img_display.SetImageResource(Resource.Drawable.display_emailbilling);
                    txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "emailBillTitle" : "nonOwnerEmailBillTitle");
                    txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "emailBillDescription" : "nonOwnerEmailBillDescription"));
                    mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    if (_isOwner)
                    {
                        ic_ca_info.Visibility = ViewStates.Visible;
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            NewAppTutorialUtils.ForceCloseNewAppTutorial();
                            if (!UserSessions.HasManageEmailBillDeliveryTutorialShown(this.mPref))
                            {
                                UserSessions.ManageBillDelivery = MobileEnums.DBRTypeEnum.Email;
                                OnShowManageBillDeliveryTutorialDialog();
                            }
                        };
                        h.PostDelayed(myAction, 50);
                    }
                    else
                    {
                        digitalBillLabelContainer.Visibility = ic_ca_info.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                    }
                }
                else if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.EmailWithCTA)
                {
                    layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    manageBillDeliveryEmailRecyclerView.SetLayoutManager(layoutManager);
                    manageBillDeliveryEmailListAdapter = new ManageBillDeliveryEmailListAdapter(this, getBillRenderingModel.Content.EmailList);
                    manageBillDeliveryEmailRecyclerView.SetAdapter(manageBillDeliveryEmailListAdapter);
                    FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                    layout.Height = TextViewUtils.IsLargeFonts
                        ? (int)DPUtils.ConvertDPToPx(200f)
                        : (int)DPUtils.ConvertDPToPx(255f);
                    applicationIndicator.Visibility
                        = btnStartDigitalBillLayout.Visibility
                        = applicationIndicator.Visibility
                        = indicatorContainer.Visibility
                        = viewPager.Visibility
                        = deliverigAddress.Visibility
                        = digitalBillLabelContainer.Visibility
                        = digitalBillLabelLayout.Visibility
                        = ViewStates.Gone;
                    email_layout.Visibility
                        = email_container.Visibility
                        = deliverigTitle.Visibility
                        = btnUpdateDigitalBillLayout.Visibility
                        = ic_ca_info.Visibility
                        = ViewStates.Visible;


                    if (getBillRenderingModel.Content.EmailList.Count >= 2)
                    {
                        layout.Height = TextViewUtils.IsLargeFonts
                       ? layout.Height + manageBillDeliveryEmailRecyclerView.Height
                       : layout.Height + manageBillDeliveryEmailRecyclerView.Height;
                    }
                    else
                    {
                        layout.Height = TextViewUtils.IsLargeFonts
                         ? layout.Height + manageBillDeliveryEmailRecyclerView.Height + (int)DPUtils.ConvertDPToPx(45f)
                         : layout.Height + manageBillDeliveryEmailRecyclerView.Height + +(int)DPUtils.ConvertDPToPx(50f);
                    }

                    img_display.SetImageResource(Resource.Drawable.display_emailbilling);
                    txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "emailBillTitle" : "nonOwnerEmailBillTitle");
                    txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                        , _isOwner ? "emailBillDescription" : "nonOwnerEmailBillDescription"));
                    mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    if (_isOwner)
                    {
                        ic_ca_info.Visibility
                            = btnUpdateDigitalBillLayout.Visibility
                            = ViewStates.Visible;
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            NewAppTutorialUtils.ForceCloseNewAppTutorial();
                            if (!UserSessions.HasManageParallelEmailBillDeliveryTutorialShown(this.mPref))
                            {
                                UserSessions.ManageBillDelivery = MobileEnums.DBRTypeEnum.EmailWithCTA;
                                OnShowManageBillDeliveryTutorialDialog();
                            }
                        };
                        h.PostDelayed(myAction, 50);
                    }
                    else
                    {
                        btnUpdateDigitalBillLayout.Visibility
                            = digitalBillLabelContainer.Visibility
                            = ic_ca_info.Visibility
                            = digitalBillLabelLayout.Visibility
                            = ViewStates.Gone;
                    }
                }
                else if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.Paper)
                {
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
                        email_layout.Visibility
                            = btnUpdateDigitalBillLayout.Visibility
                            = email_container.Visibility
                            = digitalBillLabelContainer.Visibility
                            = digitalBillLabelLayout.Visibility
                            = ViewStates.Gone;
                        UserSessions.ManageBillDelivery = MobileEnums.DBRTypeEnum.Paper;
                        FrameLayout.LayoutParams layout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
                        layout.Height = TextViewUtils.IsLargeFonts
                            ? (int)DPUtils.ConvertDPToPx(480f + (float)(deliverigAddress.Text.Length / 1.7))
                            : (int)DPUtils.ConvertDPToPx(455f);
                        ScrollPage();
                    }
                    else
                    {
                        FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                        layout.Height = TextViewUtils.IsLargeFonts
                            ? (int)DPUtils.ConvertDPToPx(435f)
                            : (int)DPUtils.ConvertDPToPx(375f);
                        manageBillDeliveryEmailRecyclerView.Visibility
                            = digitalBillLabelContainer.Visibility
                            = ic_ca_info.Visibility
                            = digitalBillLabelLayout.Visibility
                            = applicationIndicator.Visibility
                            = btnStartDigitalBillLayout.Visibility
                            = applicationIndicator.Visibility
                            = indicatorContainer.Visibility
                            = viewPager.Visibility
                            = btnUpdateDigitalBillLayout.Visibility
                            = deliverigAddress.Visibility
                            = ViewStates.Gone;
                        TenantDeliverigAddress.Visibility
                            = email_layout.Visibility
                            = email_container.Visibility
                            = deliverigTitle.Visibility
                            = ViewStates.Visible;
                        img_display.SetImageResource(Resource.Drawable.manage_bill_delivery_3);
                        txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "nonOwnerPaperTitle");
                        txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "nonOwnerPaperMessage"));
                    }
                }
                else if (getBillRenderingModel.Content.DBRType == MobileEnums.DBRTypeEnum.WhatsApp)
                {

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

        public void InitiateDBRRequest(AccountData mSelectedAccountData)
        {
            try
            {
                SetDynatraceCTATags();
                Intent intent = new Intent(this, typeof(DigitalBillActivity));
                intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
                intent.PutExtra("billrenderingresponse", JsonConvert.SerializeObject(_billRenderingResponse));
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

        [OnClick(Resource.Id.digitalBillLabelContainer)]
        void OnTapManageBillDeliveryTooltip(object sender, EventArgs eventArgs)
        {
            ShowManageBillDeliveryPopup();
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
                    txt_ca_name.Text = selectedAccountNickName + " - " + mSelectedAccountData.AccountNum;
                    deliverigAddress.Text = selectedEligibleAccount.accountAddress;
                    TenantDeliverigAddress.Text = selectedEligibleAccount.accountAddress;
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public void SetAccountName(CustomerBillingAccount selectedAccount)
        {
            txt_ca_name.Text = selectedAccount.AccDesc + " - " + mSelectedAccountData.AccountNum;
            deliverigAddress.Text = selectedAccount.AccountStAddress;
            TenantDeliverigAddress.Text = selectedEligibleAccount.accountAddress;
            if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.Paper)
            {
                FrameLayout.LayoutParams layout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts
                    ? (int)DPUtils.ConvertDPToPx(510f + (float)(deliverigAddress.Text.Length / 1.7))
                    : (int)DPUtils.ConvertDPToPx(455f);
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
        public void OnShowManageBillDeliveryTutorialDialog()
        {
            Handler h = new Handler();
            Action myAction = () =>
            {
                NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.OnGenerateNewAppTutorialList(), true);
            };
            h.PostDelayed(myAction, 100);
        }
        public List<NewAppModel> OnGenerateNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EBillWithCTA)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodUpdateCTATitle"),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodUpdateCTAMessage"),
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
            }
            else if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.Email)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodEmailTitle"),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodEmailMessage"),
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
            }
            else if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EmailWithCTA)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodEmailTitle"),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodEmailMessage"),
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodUpdateCTATitle"),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", "manageDeliveryMethodUpdateCTAMessage"),
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
            }
            return newList;
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
        public int GetImageHeight()
        {
            return image_layout.Height + txtTitle.Height + txtMessage.Height;
        }
        public int GetEmailDeliveryHeight()
        {
            return deliveryLayout.Height;
        }
        public int GetdigitalBillLabelHeight()
        {
            return digitalBillLabelContainer.Height;
        }

        public int GetviewPagerHeight()
        {
            return viewPagerLyout.Height;
        }
        public int GetBtnUpdateDigitalBillHeight()
        {
            return btnUpdateDigitalBillLayout.Height;
        }
        public int GetSelectAccountContainerHeight()
        {
            return selectAccountContainer.Height;
        }
        public int GetTopHeight()
        {
            int i = 0;

            try
            {
                Rect offsetViewBounds = new Rect();
                //returns the visible bounds
                btnUpdateDigitalBillLayout.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent

                rootView.OffsetDescendantRectToMyCoords(btnUpdateDigitalBillLayout, offsetViewBounds);

                i = offsetViewBounds.Top;

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }
    }
}