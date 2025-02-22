﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.DBR.DBRApplication.MVP;
using myTNB.AndroidApp.Src.DigitalBill.Activity;
using myTNB.AndroidApp.Src.DigitalBillRendering.ManageBillDelivery.Activity.MVP;
using myTNB.AndroidApp.Src.ManageBillDelivery.ManageBillDeliveryEmailList.Adapter;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Async;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using myTNB.AndroidApp.Src.Utils;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Models.DBR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB.Mobile.MobileEnums;

namespace myTNB.AndroidApp.Src.ManageBillDelivery.MVP
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

        [BindView(Resource.Id.viewPagerLayout)]
        LinearLayout viewPagerLayout;

        [BindView(Resource.Id.deliverigTitle)]
        TextView deliveringTitle;

        [BindView(Resource.Id.deliveringAddress)]
        TextView deliveringAddress;

        [BindView(Resource.Id.TenantDeliveringAddress)]
        TextView TenantDeliveringAddress;

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

        [BindView(Resource.Id.digital_container)]
        LinearLayout digital_container;

        [BindView(Resource.Id.bill_paper_icon)]
        ImageView bill_paper_icon;

        [BindView(Resource.Id.paperTitle)]
        TextView paperTitle;



        private const string PAGE_ID = "ManageDigitalBillLanding";
        private ManageBillDeliveryEmailListAdapter manageBillDeliveryEmailListAdapter;
        private GetBillRenderingResponse _billRenderingResponse;
        // private PostBREligibilityIndicatorsResponse _billRenderingTenantResponse;
        private RecyclerView.LayoutManager layoutManager;
        private ISharedPreferences mPref;
        private bool _isOwner { get; set; }
        private bool fromdashboard = false;
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
        private bool FromFloatingButtonMarketing = false;
        private bool fromProfile;
        private string message = "";
        private bool notEligible = false;
        LinearLayout.LayoutParams emailLayout;
        FrameLayout.LayoutParams viewerlayout;

        private CustomViewPager vPager;

        //========================================== FORM LIFECYCLE ==================================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            presenter = new ManageBillDeliveryPresenter(this);

            Bundle extras = Intent.Extras;
            digitalBillLabel.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "anotherDeliveryMethod");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "startDigitalBillCTA");
            deliveringTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "deliveringToTitle");
            btnUpdateDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "updateBillDeliveryMethodCTA");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "goPaperlessCTA");
            txtSelectedAccountTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "selectAccount");
            TextViewUtils.SetMuseoSans500Typeface(digitalBillLabel, btnStartDigitalBill, btnUpdateDigitalBill, deliveringTitle, txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(deliveringAddress, TenantDeliveringAddress, txtMessage, txtSelectedAccountTitle, txt_ca_name);
            TextViewUtils.SetTextSize12(digitalBillLabel, txtSelectedAccountTitle);
            TextViewUtils.SetTextSize16(btnStartDigitalBill, btnUpdateDigitalBill, deliveringTitle, txtTitle, txt_ca_name);
            TextViewUtils.SetTextSize14(deliveringAddress, TenantDeliveringAddress, txtMessage);

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
                        selectedAccountNumber = mSelectedAccountData.AccountNum;

                        CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(selectedAccountNumber);
                        _isOwner = account.isOwned && DBRUtility.Instance.IsCAEligible(selectedAccountNumber);
                    }
                    AddViewPager();
                    UpdateAccountListIndicator();
                }
                if (extras.ContainsKey("billRenderingResponse"))
                {
                    _billRenderingResponse = JsonConvert.DeserializeObject<GetBillRenderingResponse>(extras.GetString("billRenderingResponse"));
                }
                //if (extras.ContainsKey("billRenderingTenantResponse"))
                //{
                //    _billRenderingTenantResponse = JsonConvert.DeserializeObject<PostBREligibilityIndicatorsResponse>(extras.GetString("billRenderingTenantResponse"));
                //}
                if (extras.ContainsKey(Constants.APP_NAVIGATION_KEY))
                {
                    currentAppNavigation = extras.GetString(Constants.APP_NAVIGATION_KEY);
                    ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList(mSelectedAccountData));
                    vPager.Adapter = ManageBillDeliveryAdapter;
                    UpdateAccountListIndicator();
                }
                if (Intent.HasExtra("FromFloatingButtonMarketing"))
                {
                    FromFloatingButtonMarketing = Intent.Extras.GetBoolean("FromFloatingButtonMarketing", false);
                }
                if (Intent.HasExtra("fromProfile"))
                {
                    fromProfile = Intent.Extras.GetBoolean("fromProfile", true);
                }

                List<CustomerBillingAccount> AccountList = CustomerBillingAccount.List();
                bool tenantAllowOptIn = false;
                List<PostBREligibilityIndicatorsModel> tenantList = TenantDBRCache.Instance.IsTenantDBREligible();
                //if (_billRenderingTenantResponse != null
                //    && _billRenderingTenantResponse.StatusDetail != null
                //    && _billRenderingTenantResponse.StatusDetail.IsSuccess
                //    && _billRenderingTenantResponse.Content != null)
                //{
                //    bool isOwnerOverRule = _billRenderingTenantResponse.Content.Find(x => x.CaNo == selectedAccountNumber).IsOwnerOverRule;
                //    bool isOwnerAlreadyOptIn = _billRenderingTenantResponse.Content.Find(x => x.CaNo == selectedAccountNumber).IsOwnerAlreadyOptIn;
                //    bool isTenantAlreadyOptIn = _billRenderingTenantResponse.Content.Find(x => x.CaNo == selectedAccountNumber).IsTenantAlreadyOptIn;
                //    // bool AccountHasOwner = AccountList.Find(x => x.AccNum == selectedAccountNumber).AccountHasOwner;

                //    if (mSelectedAccountData.AccountHasOwner && !isOwnerOverRule && !isOwnerAlreadyOptIn && !isTenantAlreadyOptIn)
                //    {
                //        tenantAllowOptIn = true;
                //    }
                //}


                if (tenantList != null)
                {
                    if (tenantList.Find(x => x.CaNo == selectedAccountNumber) != null)
                    {
                        bool isOwnerOverRule = tenantList.Find(x => x.CaNo == selectedAccountNumber).IsOwnerOverRule;
                        bool isOwnerAlreadyOptIn = tenantList.Find(x => x.CaNo == selectedAccountNumber).IsOwnerAlreadyOptIn;
                        bool isTenantAlreadyOptIn = tenantList.Find(x => x.CaNo == selectedAccountNumber).IsTenantAlreadyOptIn;
                        bool AccountHasOwner = AccountList.Find(x => x.AccNum == selectedAccountNumber).AccountHasOwner;

                        if (mSelectedAccountData.AccountHasOwner && !isOwnerOverRule && !isOwnerAlreadyOptIn && !isTenantAlreadyOptIn)
                        {
                            tenantAllowOptIn = true;
                        }


                        if (!AccountHasOwner)
                        {
                            //copywriting display
                            //message = "Sorry, your account not eligible for DBR due to your owner dont have myTNB account";
                            paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "notEligibleAccountNoOwnerText");
                        }

                        if (isOwnerOverRule)
                        {
                            //copywriting display
                            paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "notEligibleOwnerOverRuleText");
                        }
                    }
                    else
                    {
                        //default copywriting when api null";
                        paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "defaultTenantApiText");

                    }
                }
                else
                {
                    //default copywriting when api null";
                    paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "defaultTenantApiText");
                }


                GetDeliveryDisplay(_billRenderingResponse, tenantAllowOptIn, fromProfile, message, notEligible);

                SetToolBarTitle(GetLabelByLanguage(_isOwner || tenantAllowOptIn ? "title" : "dbrViewBillDelivery"));
            }
            btnStartDigitalBill.Click += delegate
            {
                OnDisplayMicrosite();
            };
            btnUpdateDigitalBill.Click += delegate
            {
                OnDisplayMicrosite();
            };
        }

        private void SetAddress()
        {
            if (_billRenderingResponse != null
                && _billRenderingResponse.Content != null
                && _billRenderingResponse.Content.IsOwnerPostalBill
                && _billRenderingResponse.Content.OwnerPostalAddress.IsValid())
            {
                deliveringAddress.Text = _billRenderingResponse.Content.OwnerPostalAddress;
                TenantDeliveringAddress.Text = _billRenderingResponse.Content.OwnerPostalAddress;
            }
            else
            {
                deliveringAddress.Text = mSelectedAccountData.AddStreet;
                TenantDeliveringAddress.Text = mSelectedAccountData.AddStreet;
            }
        }

        private void AddViewPager()
        {
            ViewGroup.LayoutParams? viewPagerParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent
               , ViewGroup.LayoutParams.WrapContent)
            {
                Width = ViewGroup.LayoutParams.MatchParent,
                Height = ViewGroup.LayoutParams.WrapContent
            };
            vPager = new CustomViewPager(this)
            {
                Id = 42788,
                LayoutParameters = viewPagerParams
            };
            vPager.AddOnPageChangeListener(this);
            viewPagerLayout.AddView(vPager, 0);
            ManageBillDeliveryAdapter = new ManageBillDeliveryAdapter(SupportFragmentManager, this);
            ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList(mSelectedAccountData));
            vPager.Adapter = ManageBillDeliveryAdapter;
        }

        public void GetDeliveryDisplay(GetBillRenderingResponse getBillRenderingModel, bool tenantAllowOptIn, bool fromProfile, string message, bool notEligible)
        {
            if (getBillRenderingModel != null && getBillRenderingModel.Content != null)
            {
                UserEntity user = UserEntity.GetActive();
                TenantDeliveringAddress.Visibility = ViewStates.Gone;
                if (getBillRenderingModel.Content.IsInProgress)
                {
                    applicationIndicator.Visibility
                        = btnStartDigitalBillLayout.Visibility
                        = applicationIndicator.Visibility
                        = indicatorContainer.Visibility
                        = vPager.Visibility
                        = deliveringAddress.Visibility
                        = email_container.Visibility
                        = deliveringTitle.Visibility
                        = btnUpdateDigitalBillLayout.Visibility
                        = digitalBillLabelLayout.Visibility
                        = digitalBillLabelContainer.Visibility
                        = digital_container.Visibility
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
                    if (getBillRenderingModel.Content.DBRType == DBRTypeEnum.EBill)
                    {
                        applicationIndicator.Visibility
                            = viewPagerLayout.Visibility
                            = btnStartDigitalBillLayout.Visibility
                            = applicationIndicator.Visibility
                            = indicatorContainer.Visibility
                            = vPager.Visibility
                            = deliveringAddress.Visibility
                            = email_container.Visibility
                            = deliveringTitle.Visibility
                            = btnUpdateDigitalBillLayout.Visibility
                            = digital_container.Visibility
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
                                UserSessions.ManageBillDelivery = DBRTypeEnum.EBill;
                                OnShowManageBillDeliveryTutorialDialog();
                            }
                        };
                        h.PostDelayed(myAction, 50);
                    }
                    else if (getBillRenderingModel.Content.DBRType == DBRTypeEnum.EBillWithCTA)
                    {
                        if (fromProfile)
                        {
                            applicationIndicator.Visibility
                               = viewPagerLayout.Visibility
                               = btnStartDigitalBillLayout.Visibility
                               = applicationIndicator.Visibility
                               = indicatorContainer.Visibility
                               = vPager.Visibility
                               = deliveringAddress.Visibility
                               = email_container.Visibility
                               = digitalBillLabelContainer.Visibility
                               = digitalBillLabelLayout.Visibility
                               = deliveringTitle.Visibility
                               = digital_container.Visibility
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
                                        UserSessions.ManageBillDelivery = DBRTypeEnum.EBillWithCTA;
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
                        else
                        {
                            applicationIndicator.Visibility
                               = viewPagerLayout.Visibility
                               = btnStartDigitalBillLayout.Visibility
                               = applicationIndicator.Visibility
                               = indicatorContainer.Visibility
                               = vPager.Visibility
                               = deliveringAddress.Visibility
                               = email_container.Visibility
                               = deliveringTitle.Visibility
                               = btnUpdateDigitalBillLayout.Visibility
                               = digital_container.Visibility
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
                                    UserSessions.ManageBillDelivery = DBRTypeEnum.EBill;
                                    OnShowManageBillDeliveryTutorialDialog();
                                }
                            };
                            h.PostDelayed(myAction, 50);
                        }

                        fromProfile = false;
                    }
                    else if (getBillRenderingModel.Content.DBRType == DBRTypeEnum.Email)
                    {
                        layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                        manageBillDeliveryEmailRecyclerView.SetLayoutManager(layoutManager);
                        manageBillDeliveryEmailListAdapter = new ManageBillDeliveryEmailListAdapter(this, getBillRenderingModel.Content.EmailList);
                        manageBillDeliveryEmailRecyclerView.SetAdapter(manageBillDeliveryEmailListAdapter);

                        TenantDeliveringAddress.Visibility
                            = viewPagerLayout.Visibility
                            = applicationIndicator.Visibility
                            = btnStartDigitalBillLayout.Visibility
                            = applicationIndicator.Visibility
                            = indicatorContainer.Visibility
                            = vPager.Visibility
                            = btnUpdateDigitalBillLayout.Visibility
                            = deliveringAddress.Visibility
                            = digital_container.Visibility
                            = ViewStates.Gone;
                        email_layout.Visibility
                            = email_container.Visibility
                            = digitalBillLabelLayout.Visibility
                            = digitalBillLabelContainer.Visibility
                            = deliveringTitle.Visibility
                            = ViewStates.Visible;

                        img_display.SetImageResource(Resource.Drawable.display_emailbilling);
                        txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                            , _isOwner ? "emailBillTitle" : "nonOwnerEmailBillTitle");
                        txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                            , _isOwner ? "emailBillDescription" : "nonOwnerEmailBillDescription"));
                        mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                        if (_isOwner)
                        {
                            ic_ca_info.Visibility = manageBillDeliveryEmailRecyclerView.Visibility = ViewStates.Visible;
                            Handler h = new Handler();
                            Action myAction = () =>
                            {
                                NewAppTutorialUtils.ForceCloseNewAppTutorial();
                                if (!UserSessions.HasManageEmailBillDeliveryTutorialShown(this.mPref))
                                {
                                    UserSessions.ManageBillDelivery = DBRTypeEnum.Email;
                                    OnShowManageBillDeliveryTutorialDialog();
                                }
                            };
                            h.PostDelayed(myAction, 50);
                        }
                        else
                        {
                            digitalBillLabelContainer.Visibility
                                = ic_ca_info.Visibility
                                = digitalBillLabelLayout.Visibility
                                = ViewStates.Gone;
                        }
                    }
                    else if (getBillRenderingModel.Content.DBRType == DBRTypeEnum.EmailWithCTA)
                    {
                        if (fromProfile)
                        {
                            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                            manageBillDeliveryEmailRecyclerView.SetLayoutManager(layoutManager);
                            manageBillDeliveryEmailListAdapter = new ManageBillDeliveryEmailListAdapter(this, getBillRenderingModel.Content.EmailList);
                            manageBillDeliveryEmailRecyclerView.SetAdapter(manageBillDeliveryEmailListAdapter);

                            applicationIndicator.Visibility
                                = viewPagerLayout.Visibility
                                = btnStartDigitalBillLayout.Visibility
                                = applicationIndicator.Visibility
                                = indicatorContainer.Visibility
                                = vPager.Visibility
                                = deliveringAddress.Visibility
                                = digitalBillLabelContainer.Visibility
                                = digitalBillLabelLayout.Visibility
                                = digital_container.Visibility
                                = ViewStates.Gone;
                            email_layout.Visibility
                                = email_container.Visibility
                                = deliveringTitle.Visibility
                                = btnUpdateDigitalBillLayout.Visibility
                                = ic_ca_info.Visibility
                                = ViewStates.Visible;

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
                                    = manageBillDeliveryEmailRecyclerView.Visibility
                                    = ViewStates.Visible;
                                Handler h = new Handler();
                                Action myAction = () =>
                                {
                                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                                    if (!UserSessions.HasManageParallelEmailBillDeliveryTutorialShown(this.mPref))
                                    {
                                        UserSessions.ManageBillDelivery = DBRTypeEnum.EmailWithCTA;
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
                        else
                        {
                            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                            manageBillDeliveryEmailRecyclerView.SetLayoutManager(layoutManager);
                            manageBillDeliveryEmailListAdapter = new ManageBillDeliveryEmailListAdapter(this, getBillRenderingModel.Content.EmailList);
                            manageBillDeliveryEmailRecyclerView.SetAdapter(manageBillDeliveryEmailListAdapter);

                            TenantDeliveringAddress.Visibility
                                = viewPagerLayout.Visibility
                                = applicationIndicator.Visibility
                                = btnStartDigitalBillLayout.Visibility
                                = applicationIndicator.Visibility
                                = indicatorContainer.Visibility
                                = vPager.Visibility
                                = btnUpdateDigitalBillLayout.Visibility
                                = deliveringAddress.Visibility
                                = digital_container.Visibility
                                = ViewStates.Gone;
                            email_layout.Visibility
                                = email_container.Visibility
                                = digitalBillLabelLayout.Visibility
                                = digitalBillLabelContainer.Visibility
                                = deliveringTitle.Visibility
                                = ViewStates.Visible;

                            img_display.SetImageResource(Resource.Drawable.display_emailbilling);
                            txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                                , _isOwner ? "emailBillTitle" : "nonOwnerEmailBillTitle");
                            txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding"
                                , _isOwner ? "emailBillDescription" : "nonOwnerEmailBillDescription"));
                            mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                            if (_isOwner)
                            {
                                ic_ca_info.Visibility = manageBillDeliveryEmailRecyclerView.Visibility = ViewStates.Visible;
                                Handler h = new Handler();
                                Action myAction = () =>
                                {
                                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                                    if (!UserSessions.HasManageEmailBillDeliveryTutorialShown(this.mPref))
                                    {
                                        UserSessions.ManageBillDelivery = DBRTypeEnum.Email;
                                        OnShowManageBillDeliveryTutorialDialog();
                                    }
                                };
                                h.PostDelayed(myAction, 50);
                            }
                            else
                            {
                                digitalBillLabelContainer.Visibility
                                    = ic_ca_info.Visibility
                                    = digitalBillLabelLayout.Visibility
                                    = ViewStates.Gone;
                            }
                        }

                        fromProfile = false;
                    }
                    else if (getBillRenderingModel.Content.DBRType == DBRTypeEnum.Paper)
                    {
                        viewPagerLayout.Visibility = ViewStates.Visible;
                        deliveringTitle.Visibility = ViewStates.Visible;
                        if (_isOwner)
                        {
                            ic_ca_info.Visibility
                                = applicationIndicator.Visibility
                                = btnStartDigitalBillLayout.Visibility
                                = applicationIndicator.Visibility
                                = indicatorContainer.Visibility
                                = vPager.Visibility
                                = deliveringAddress.Visibility
                                = ViewStates.Visible;
                            email_layout.Visibility
                                = btnUpdateDigitalBillLayout.Visibility
                                = email_container.Visibility
                                = digitalBillLabelContainer.Visibility
                                = digitalBillLabelLayout.Visibility
                                = digital_container.Visibility
                                = ViewStates.Gone;
                            UserSessions.ManageBillDelivery = DBRTypeEnum.Paper;
                            ScrollPage();
                        }
                        else
                        {
                            if (tenantAllowOptIn)
                            {
                                manageBillDeliveryEmailRecyclerView.Visibility
                              = digitalBillLabelContainer.Visibility
                              = ic_ca_info.Visibility
                              = digitalBillLabelLayout.Visibility
                              = applicationIndicator.Visibility
                              = btnStartDigitalBillLayout.Visibility
                              = applicationIndicator.Visibility
                              = indicatorContainer.Visibility
                              = vPager.Visibility
                              = deliveringAddress.Visibility
                              = ViewStates.Visible;
                                TenantDeliveringAddress.Visibility
                                = email_layout.Visibility
                                = email_container.Visibility
                                = btnUpdateDigitalBillLayout.Visibility
                                = digital_container.Visibility
                                = ViewStates.Gone;
                                UserSessions.ManageBillDelivery = DBRTypeEnum.Paper;
                                ScrollPage();
                            }
                            else
                            {
                                manageBillDeliveryEmailRecyclerView.Visibility
                                = digitalBillLabelContainer.Visibility
                                = ic_ca_info.Visibility
                                = digitalBillLabelLayout.Visibility
                                = applicationIndicator.Visibility
                                = btnStartDigitalBillLayout.Visibility
                                = applicationIndicator.Visibility
                                = indicatorContainer.Visibility
                                = vPager.Visibility
                                = deliveringAddress.Visibility
                                = btnUpdateDigitalBillLayout.Visibility
                                = ViewStates.Gone;
                                TenantDeliveringAddress.Visibility
                                = email_layout.Visibility
                                = email_container.Visibility
                                = digital_container.Visibility
                                = ViewStates.Visible;

                            }

                            img_display.SetImageResource(Resource.Drawable.manage_bill_delivery_3);
                            txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "nonOwnerPaperTitle");
                            txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "nonOwnerPaperMessage"));
                        }
                        SetAddress();
                    }
                    else if (getBillRenderingModel.Content.DBRType == DBRTypeEnum.WhatsApp)
                    {

                    }
                    else if (getBillRenderingModel.Content.DBRType == DBRTypeEnum.None)
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
        }

        private void SetDynatraceScreenTags(GetBillRenderingResponse billRenderingResponse)
        {
            if (billRenderingResponse == null
                || _billRenderingResponse.Content == null)
            {
                return;
            }
            string dynatraceTag;
            switch (billRenderingResponse.Content.CurrentRenderingMethod)
            {
                case MobileEnums.RenderingMethodEnum.EBill:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.EBill;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Email:
                case MobileEnums.RenderingMethodEnum.Email:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.EBill_Email;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.EBill_Paper;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.Email_Paper:
                case MobileEnums.RenderingMethodEnum.EBill_Email_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.ManageBillDelivery.EBill_Email_Paper;
                        break;
                    }
                default:
                    {
                        dynatraceTag = string.Empty;
                        break;
                    }
            }
            DynatraceHelper.OnTrack(dynatraceTag);
        }

        private void SetDynatraceCTATags()
        {
            if (_billRenderingResponse == null
                || _billRenderingResponse.Content == null)
            {
                return;
            }
            string dynatraceTag;
            switch (_billRenderingResponse.Content.CurrentRenderingMethod)
            {
                case MobileEnums.RenderingMethodEnum.EBill:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.EBill;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Email:
                case MobileEnums.RenderingMethodEnum.Email:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.EBill_Email;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.EBill_Paper;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.Email_Paper:
                case MobileEnums.RenderingMethodEnum.EBill_Email_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.ManageBillDelivery.EBill_Email_Paper;
                        break;
                    }
                default:
                    {
                        dynatraceTag = string.Empty;
                        break;
                    }
            }
            DynatraceHelper.OnTrack(dynatraceTag);
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
                    if (page <= vPager.Adapter.Count)
                    //   if (page <= viewPager.Adapter.Count)
                    {
                        page++;
                    }
                    else
                    {
                        page = 0;
                    }
                    // viewPager.SetCurrentItem(page, true);
                    vPager.SetCurrentItem(page, true);

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
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
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
            bool isPilot = EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR,
                EligibilitySessionCache.FeatureProperty.TargetGroup);

            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetTitle(Utility.GetLocalizedLabel("ManageDigitalBillLanding", isPilot ? "notEligibleAccountsTitleNationwideV2" : "notEligibleAccountsTitleNationwideV2"))
                .SetMessage(Utility.GetLocalizedLabel("ManageDigitalBillLanding", isPilot ? "notEligibleAccountsMessageNationwideV2" : "notEligibleAccountsMessageNationwideV2"))
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
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == DBR_SELECT_ACCOUNT_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    selectedAccountNumber = data.GetStringExtra("SELECTED_ACCOUNT_NUMBER");
                    _billRenderingResponse = JsonConvert.DeserializeObject<GetBillRenderingResponse>(data.GetStringExtra("billrenderingresponse"));
                    //_billRenderingTenantResponse = JsonConvert.DeserializeObject<PostBREligibilityIndicatorsResponse>(data.GetStringExtra("billRenderingTenantResponse"));
                    List<PostBREligibilityIndicatorsModel> tenantList = TenantDBRCache.Instance.IsTenantDBREligible();


                    if (_billRenderingResponse != null
                        && _billRenderingResponse.StatusDetail != null
                        && _billRenderingResponse.StatusDetail.IsSuccess
                        && _billRenderingResponse.Content != null)
                    {
                        CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(selectedAccountNumber);
                        _isOwner = account.isOwned && DBRUtility.Instance.IsCAEligible(selectedAccountNumber);
                        _accountNumber = selectedAccountNumber;

                        bool tenantAllowOptIn = false;

                        if (tenantList != null)
                        {
                            if (tenantList.Find(x => x.CaNo == selectedAccountNumber) != null)
                            {
                                //For tenant checking DBR | Get a single data for specific ca from response list
                                List<CustomerBillingAccount> AccountList = CustomerBillingAccount.List();
                                bool isOwnerOverRule = tenantList.Find(x => x.CaNo == selectedAccountNumber).IsOwnerOverRule;
                                bool isOwnerAlreadyOptIn = tenantList.Find(x => x.CaNo == selectedAccountNumber).IsOwnerAlreadyOptIn;
                                bool isTenantAlreadyOptIn = tenantList.Find(x => x.CaNo == selectedAccountNumber).IsTenantAlreadyOptIn;
                                bool AccountHasOwner = AccountList.Find(x => x.AccNum == selectedAccountNumber).AccountHasOwner;


                                if (AccountHasOwner && !isOwnerOverRule && !isOwnerAlreadyOptIn && !isTenantAlreadyOptIn)
                                {
                                    tenantAllowOptIn = true;
                                }

                                if (!AccountHasOwner)
                                {
                                    //copywriting display
                                    //message = "Sorry, your account not eligible for DBR due to your owner dont have myTNB account";
                                    paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "notEligibleAccountNoOwnerText");
                                }

                                if (isOwnerOverRule)
                                {
                                    //copywriting display
                                    paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "notEligibleOwnerOverRuleText");
                                }

                            }
                            else
                            {
                                //default copywriting when api null";
                                paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "defaultTenantApiText");
                            }
                        }
                        else
                        {
                            //default copywriting when api null";
                            paperTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "defaultTenantApiText");
                        }

                        SetToolBarTitle(GetLabelByLanguage(_isOwner || tenantAllowOptIn ? "title" : "dbrViewBillDelivery"));

                        GetDeliveryDisplay(_billRenderingResponse, tenantAllowOptIn, fromProfile, message, notEligible);
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
                    deliveringAddress.Text = selectedEligibleAccount.accountAddress;
                    TenantDeliveringAddress.Text = selectedEligibleAccount.accountAddress;
                }
            }
        }

        public void SetAccountName(CustomerBillingAccount selectedAccount)
        {
            txt_ca_name.Text = selectedAccount.AccDesc + " - " + selectedAccount.AccNum;
            deliveringAddress.Text = selectedAccount.AccountStAddress;
            TenantDeliveringAddress.Text = selectedEligibleAccount.accountAddress;
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
                if (FromFloatingButtonMarketing)
                    ShowDashboard();
                else
                    this.Finish();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
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
                if (UserSessions.ManageBillDelivery == DBRTypeEnum.Email)
                {
                    NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.OnGenerateNewAppTutorialList(), false);
                }
                else
                {
                    NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.OnGenerateNewAppTutorialList(), true);
                }
            };
            h.PostDelayed(myAction, 100);
        }
        public List<NewAppModel> OnGenerateNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            if (UserSessions.ManageBillDelivery == DBRTypeEnum.EBillWithCTA)
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
            else if (UserSessions.ManageBillDelivery == DBRTypeEnum.Email)
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
            else if (UserSessions.ManageBillDelivery == DBRTypeEnum.EmailWithCTA)
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
            List<string> dBRCAs = DBRUtility.Instance.GetCAList();
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
                    if (account != null)
                    {
                        eligibleDBRAccountList.Add(account);
                    }
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
                        accountOwnerName = account.OwnerName,
                        IsOwner = account.isOwned
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
            return viewPagerLayout.Height;
        }
        public int GetBtnUpdateDigitalBillHeight()
        {
            return btnUpdateDigitalBillLayout.Height;
        }
        public int GetSelectAccountContainerHeight()
        {
            return selectAccountContainer.Height;
        }
        public int GetEmail_layoutrHeight()
        {
            return email_layout.Height;
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



