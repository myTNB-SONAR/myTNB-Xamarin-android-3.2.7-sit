﻿using System;
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
using Android.Preferences;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB.Mobile;
using System.Linq;
using myTNB.Mobile.AWS.Models;

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

        [BindView(Resource.Id.txt_ca_name)]
        TextView txt_ca_name;

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

        [BindView(Resource.Id.btnStartDigitalBillLayout)]
        LinearLayout btnStartDigitalBillLayout;

        [BindView(Resource.Id.applicationIndicatorLayout)]
        LinearLayout applicationIndicatorLayout;

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

        [BindView(Resource.Id.img_display)]
        ImageView img_display;

        [BindView(Resource.Id.txtTitle)]
        TextView txtTitle;

        [BindView(Resource.Id.txtMessage)]
        TextView txtMessage;

        [BindView(Resource.Id.deliveryEmail)]
        TextView deliveryEmail;

        [BindView(Resource.Id.deliveryUserName)]
        TextView deliveryUserName;

        GetBillRenderingModel getBillRenderingModel;

        ISharedPreferences mPref;

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
            digitalBillLabel.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "anotherDeliveryMethod");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "startDigitalBillCTA");
            deliverigTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "deliveringToTitle");
            btnUpdateDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "updateBillDeliveryMethodCTA");
            btnStartDigitalBill.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "goPaperlessCTA");
            TextViewUtils.SetMuseoSans500Typeface(digitalBillLabel, btnStartDigitalBill, deliverigTitle, txtTitle, deliveryEmail);
            TextViewUtils.SetMuseoSans300Typeface(deliverigAddress, txtMessage, deliveryUserName);
            TextViewUtils.SetTextSize12(digitalBillLabel, deliveryUserName);
            TextViewUtils.SetTextSize16(btnStartDigitalBill, deliverigTitle, txtTitle);
            TextViewUtils.SetTextSize14(deliverigAddress, txtMessage, deliveryEmail);
           
            if (extras != null)
            {
                if (extras.ContainsKey(SELECTED_ACCOUNT_KEY))
                {
                    mSelectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString(SELECTED_ACCOUNT_KEY));
                    txt_ca_name.Text = mSelectedAccountData.AccountNickName;
                    deliverigAddress.Text = mSelectedAccountData.AddStreet;
                }
                if (extras.ContainsKey("billrenderingresponse"))
                {
                    getBillRenderingModel = new GetBillRenderingModel();
                    getBillRenderingModel = JsonConvert.DeserializeObject<GetBillRenderingModel>(extras.GetString("billrenderingresponse"));

                    GetDeliveryDisplay(getBillRenderingModel);
                    
                }
                if (extras.ContainsKey(Constants.APP_NAVIGATION_KEY))
                {
                    currentAppNavigation = extras.GetString(Constants.APP_NAVIGATION_KEY);
                    ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList());
                    viewPager.Adapter = ManageBillDeliveryAdapter;
                    UpdateAccountListIndicator();
                }
            }
            btnStartDigitalBill.Click += delegate
            {
                InitiateDBRRequest(mSelectedAccountData);
            };
        }
        public void GetDeliveryDisplay(GetBillRenderingModel getBillRenderingModel)
        {
            UserEntity user = UserEntity.GetActive();
            if (getBillRenderingModel.DBRType == MobileEnums.DBRTypeEnum.EBill)
            {
                FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(296f) : (int)DPUtils.ConvertDPToPx(275f);
                applicationIndicator.Visibility = btnStartDigitalBillLayout.Visibility = applicationIndicator.Visibility = indicatorContainer.Visibility = viewPager.Visibility = deliverigAddress.Visibility = email_container.Visibility = deliverigTitle.Visibility = digitalBillLabelContainer.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                email_layout.Visibility = btnUpdateDigitalBillLayout.Visibility = ViewStates.Visible;
                img_display.SetImageResource(Resource.Drawable.icons_display_digital_ebill);
                txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "eBillTitle");
                txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "eBillDescription"));
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
            else if (getBillRenderingModel.DBRType == MobileEnums.DBRTypeEnum.EBillWithCTA)
            {
                FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(280f) : (int)DPUtils.ConvertDPToPx(265f);
                applicationIndicator.Visibility = btnStartDigitalBillLayout.Visibility = applicationIndicator.Visibility = indicatorContainer.Visibility = viewPager.Visibility = deliverigAddress.Visibility = email_container.Visibility = deliverigTitle.Visibility = digitalBillLabelContainer.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                email_layout.Visibility = btnUpdateDigitalBillLayout.Visibility = ViewStates.Visible;
                img_display.SetImageResource(Resource.Drawable.icons_display_digital_ebill);
                txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "eBillTitle");
                txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "eBillDescription"));
                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
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
            else if (getBillRenderingModel.DBRType == MobileEnums.DBRTypeEnum.Email)
            {
                FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(435f) : (int)DPUtils.ConvertDPToPx(375f);
                applicationIndicator.Visibility = btnStartDigitalBillLayout.Visibility = applicationIndicator.Visibility = indicatorContainer.Visibility = viewPager.Visibility = deliverigAddress.Visibility = btnUpdateDigitalBillLayout.Visibility = ViewStates.Gone;
                email_layout.Visibility = email_container.Visibility = digitalBillLabelContainer.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Visible;

                deliveryUserName.Text = user.DisplayName + Utility.GetLocalizedLabel("ManageDigitalBillLanding", "you");
                deliveryEmail.Text = user.Email;
                img_display.SetImageResource(Resource.Drawable.display_emailbilling);
                txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "emailBillTitle");
                txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "emailBillDescription"));
                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
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
            else if (getBillRenderingModel.DBRType == MobileEnums.DBRTypeEnum.EmailWithCTA)
            {
                FrameLayout.LayoutParams layout = email_layout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(400f) : (int)DPUtils.ConvertDPToPx(355f);
                applicationIndicator.Visibility = btnStartDigitalBillLayout.Visibility = applicationIndicator.Visibility = indicatorContainer.Visibility = viewPager.Visibility = deliverigAddress.Visibility = digitalBillLabelContainer.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                email_layout.Visibility = email_container.Visibility = btnUpdateDigitalBillLayout.Visibility = ViewStates.Visible;

                deliveryUserName.Text = user.DisplayName + Utility.GetLocalizedLabel("ManageDigitalBillLanding", "you");
                deliveryEmail.Text = user.Email;
                img_display.SetImageResource(Resource.Drawable.display_emailbilling);
                txtTitle.Text = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "emailBillTitle");
                txtMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "emailBillDescription"));
                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
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
            else if (getBillRenderingModel.DBRType == MobileEnums.DBRTypeEnum.Paper)
            {

                applicationIndicator.Visibility = btnStartDigitalBillLayout.Visibility = applicationIndicator.Visibility = indicatorContainer.Visibility = viewPager.Visibility = deliverigAddress.Visibility = ViewStates.Visible;
                email_layout.Visibility = btnUpdateDigitalBillLayout.Visibility = email_container.Visibility = digitalBillLabelContainer.Visibility = digitalBillLabelLayout.Visibility = ViewStates.Gone;
                UserSessions.ManageBillDelivery = MobileEnums.DBRTypeEnum.Paper;
                FrameLayout.LayoutParams layout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(510f + (float)(deliverigAddress.Text.Length / 1.7)) : (int)DPUtils.ConvertDPToPx(455f);
                ScrollPage();
            }
            else if (getBillRenderingModel.DBRType == MobileEnums.DBRTypeEnum.WhatsApp)
            {

            }
            else if (getBillRenderingModel.DBRType == MobileEnums.DBRTypeEnum.None)
            {

            }
        }
        public void InitiateDBRRequest(AccountData mSelectedAccountData)
        {
            try
            {
                Intent intent = new Intent(this, typeof(DigitalBillActivity));
                intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
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
                dbrAccountList = GetEligibleDBRAccountList();
                if (dbrAccountList != null && dbrAccountList.Count > 0)
                {
                    this.presenter.CheckDBRAccountEligibility(dbrAccountList);
                }
                else
                {
                   // Intent intent = new Intent(this, typeof(SelectDBRAccountActivity));
                    //StartActivityForResult(intent, DBR_SELECT_ACCOUNT_ACTIVITY_CODE);
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

        public void ShowManageBillDeliveryPopups()
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
                   .Build().Show();
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
                    getBillRenderingModel = new GetBillRenderingModel();
                    getBillRenderingModel = JsonConvert.DeserializeObject<GetBillRenderingModel>(data.GetStringExtra("billrenderingresponse"));
                    if (getBillRenderingModel != null)
                    {
                        GetDeliveryDisplay(getBillRenderingModel);
                    }
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
                    txt_ca_name.Text = selectedAccountNickName;
                    deliverigAddress.Text = selectedEligibleAccount.accountAddress;
                    
                    CustomerBillingAccount.RemoveSelected();
                    CustomerBillingAccount.SetSelected(selectedEligibleAccount.accountNumber);
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
        public void SetAccountName(CustomerBillingAccount selectedAccount)
        {
            txt_ca_name.Text = selectedAccount.AccDesc;
            deliverigAddress.Text = selectedAccount.AccountStAddress;
            if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.Paper)
            {
                FrameLayout.LayoutParams layout = viewPagerLyout.LayoutParameters as FrameLayout.LayoutParams;
                layout.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(510f + (float)(deliverigAddress.Text.Length / 1.7)) : (int)DPUtils.ConvertDPToPx(455f);
                
            }
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

            if(UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EBill)
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
            List<string> dBRCAs = EligibilitySessionCache.Instance.GetDBRCAs();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            List<CustomerBillingAccount> eligibleDBRAccountList = new List<CustomerBillingAccount>();
            CustomerBillingAccount account = new CustomerBillingAccount();
            List<DBRAccount> dbrEligibleAccountList = new List<DBRAccount>();

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
                    dbrEligibleAccount = new DBRAccount();
                    dbrEligibleAccount.accountNumber = account.AccNum;
                    dbrEligibleAccount.accountName = account.AccDesc;
                    dbrEligibleAccount.accountSelected = account.IsSelected;
                    dbrEligibleAccount.isTaggedSMR = account.IsTaggedSMR;
                    dbrEligibleAccount.accountAddress = account.AccountStAddress.ToUpper();
                    dbrEligibleAccount.accountOwnerName = account.OwnerName;
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
    }
}