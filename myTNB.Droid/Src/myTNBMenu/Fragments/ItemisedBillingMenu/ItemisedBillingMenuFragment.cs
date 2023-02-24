using System;
using System.Collections.Generic;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.Core.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Google.Android.Material.Snackbar;
using Java.Text;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewBill.Activity;
using myTNB_Android.Src.ViewReceipt.Activity;
using Newtonsoft.Json;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB.Mobile;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.Database.Model;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Bills.AccountStatement.Activity;
using myTNB.Mobile.AWS.Models.DBR;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu
{
    public class ItemisedBillingMenuFragment : BaseFragmentCustom, ItemisedBillingContract.IView
    {
        [BindView(Resource.Id.rootView)]
        ViewGroup rootView;

        [BindView(Resource.Id.smrReadingHistoryDetailContent)]
        ViewGroup billingHistoryDetailsContent;

        [BindView(Resource.Id.unavailableBillContainer)]
        ViewGroup unavailableBillContainer;

        [BindView(Resource.Id.unavailableBillBannerImg)]
        ImageView unavailableBillBannerImg;

        [BindView(Resource.Id.unavailableBillMsg)]
        TextView unavailableBillMsg;

        [BindView(Resource.Id.itemisedBillingHeaderImage)]
        ImageView itemisedBillingHeaderImage;

        [BindView(Resource.Id.bill_filter_icon)]
        ImageView billFilterIcon;

        [BindView(Resource.Id.download_bill_icon)]
        ImageView download_bill_icon;

        [BindView(Resource.Id.itemisedBillingInfoNote)]
        TextView itemisedBillingInfoNote;

        [BindView(Resource.Id.itemisedBillingInfoAmount)]
        TextView itemisedBillingInfoAmount;

        [BindView(Resource.Id.itemisedBillingInfoAmountCurrency)]
        TextView itemisedBillingInfoAmountCurrency;

        [BindView(Resource.Id.itemisedBillingInfoDate)]
        TextView itemisedBillingInfoDate;

        [BindView(Resource.Id.myBillHistoryTitle)]
        TextView myBillHistoryTitle;

        [BindView(Resource.Id.emptyBillingHistoryMessage)]
        TextView emptyBillingHistoryMessage;

        [BindView(Resource.Id.btnViewDetails)]
        Button btnViewDetails;

        [BindView(Resource.Id.btnChargeRefresh)]
        Button btnChargeRefresh;

        [BindView(Resource.Id.btnRefresh)]
        Button btnRefresh;

        [BindView(Resource.Id.btnPayBill)]
        Button btnPayBill;

        [BindView(Resource.Id.itemisedBillingCTAContainer)]
        LinearLayout itemisedBillingCTAContainer;

        [BindView(Resource.Id.itemisedBillingList)]
        LinearLayout itemisedBillingList;

        [BindView(Resource.Id.emptyItemisedBillingList)]
        LinearLayout emptyItemisedBillingList;

        [BindView(Resource.Id.accountSelection)]
        TextView accountSelection;

        [BindView(Resource.Id.itemisedBillingInfoShimmer)]
        ShimmerFrameLayout itemisedBillingInfoShimmer;

        [BindView(Resource.Id.itemisedBillingListShimmer)]
        LinearLayout itemisedBillingListShimmer;

        [BindView(Resource.Id.itemisedBillingInfoContainer)]
        LinearLayout itemisedBillingInfoContainer;

        [BindView(Resource.Id.itemisedBillingScrollView)]
        NestedScrollView itemisedBillingScrollView;

        [BindView(Resource.Id.bills_list_title_container)]
        LinearLayout bills_list_title_container;

        [BindView(Resource.Id.chargeAvailableContainer)]
        LinearLayout chargeAvailableContainer;

        [BindView(Resource.Id.chargeAvailableNoCTAContainer)]
        LinearLayout chargeAvailableNoCTAContainer;


        [BindView(Resource.Id.unavailableChargeContainer)]
        LinearLayout unavailableChargeContainer;

        [BindView(Resource.Id.unavailableChargeImg)]
        ImageView unavailableChargeImg;

        [BindView(Resource.Id.unavailableChargeMsg)]
        TextView unavailableChargeMsg;

        [BindView(Resource.Id.refreshItemisedBillingList)]
        LinearLayout refreshItemisedBillingList;

        [BindView(Resource.Id.refreshItemisedBillingImg)]
        ImageView refreshItemisedBillingImg;

        [BindView(Resource.Id.refreshBillingHistoryMessage)]
        TextView refreshBillingHistoryMessage;

        [BindView(Resource.Id.btnBillingHistoryRefresh)]
        Button btnBillingHistoryRefresh;

        [BindView(Resource.Id.accountSelectionRefresh)]
        TextView accountSelectionRefresh;

        [BindView(Resource.Id.digital_container)]
        LinearLayout digital_container;

        [BindView(Resource.Id.bill_paperless_icon)]
        ImageView bill_paperless_icon;

        [BindView(Resource.Id.paperlessTitle)]
        TextView paperlessTitle;

        ItemisedBillingMenuPresenter mPresenter;
        AccountData mSelectedAccountData;

        GetBillRenderingResponse billRenderingResponse;
        PostBREligibilityIndicatorsResponse billRenderingTenantResponse;
        List<AccountChargeModel> selectedAccountChargesModelList;
        List<Item> itemFilterList = new List<Item>();
        List<AccountBillPayHistoryModel> selectedBillingHistoryModelList;
        List<AccountBillPayFilter> billPayFilterList;
        internal bool _isOwner { get; set; }
        public bool _isBillStatement { get; set; }

        SimpleDateFormat dateParser = new SimpleDateFormat("yyyyMMdd", LocaleUtils.GetDefaultLocale());
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy", LocaleUtils.GetCurrentLocale());

        SimpleDateFormat billPdfDateParser = new SimpleDateFormat("dd MMM yyyy", LocaleUtils.GetCurrentLocale());
        SimpleDateFormat billPdfDateFormatter = new SimpleDateFormat("dd/MM/yyyy", LocaleUtils.GetDefaultLocale());

        private DecimalFormat mDecimalFormat = new DecimalFormat("#,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));

        IMenuItem billFilterMenuItem;
        IMenuItem billDownloadMenuItem;

        const string SELECTED_ACCOUNT_KEY = "SELECTED_ACCOUNT";
        const string INELIGIBLE_POP_UP_ACTIVE_KEY = "isIneligiblePopUpActive";
        const string PAGE_ID = "Bills";
        private bool isFiltered = false;
        private string myHistoryTitle = "";
        private string billTitle = "";
        private bool isViewBillDisable = false;
        private bool isDigitalContainerVisible = false;
        private bool isPendingPayment = false;
        private bool isIneligiblePopUpActive = false;
        private bool isPaymentButtonEnable = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ItemisedBillingMenuPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));
            Bundle extras = Arguments;

            if (extras.ContainsKey(SELECTED_ACCOUNT_KEY))
            {
                mSelectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString(SELECTED_ACCOUNT_KEY));
                GetBillRenderingAsync(mSelectedAccountData);
            }
            if (extras.ContainsKey(INELIGIBLE_POP_UP_ACTIVE_KEY))
            {
                isIneligiblePopUpActive = extras.GetBoolean(INELIGIBLE_POP_UP_ACTIVE_KEY);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.ItemisedBillingMenuLayout;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_bills_filter:
                    ShowSelectFilter();
                    return true;
                case Resource.Id.action_bills_download:
                    if (_isBillStatement)
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Bill.View_Account_Statement);
                        ShowDownloadBill();
                        return true;
                    }
                    return false;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void ShowBillFilterToolbar(bool isShow)
        {
            if (isShow)
            {
                ((DashboardHomeActivity)this.Activity).SetToolBarTitle(myHistoryTitle);
                billFilterMenuItem.SetVisible(true);
                billDownloadMenuItem.SetVisible(_isBillStatement);
                UpdateFilterIcon();
            }
            else
            {
                ((DashboardHomeActivity)this.Activity).SetToolBarTitle(billTitle);
                billFilterMenuItem.SetVisible(false);
                billDownloadMenuItem.SetVisible(false);
            }
        }

        class BillOnScrollChangeListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
        {
            LinearLayout mBillHistoryTitle;
            Action<bool> mOnScrollMethod;
            public BillOnScrollChangeListener(Action<bool> onScrollMethod, LinearLayout billHistoryTitle)
            {
                mBillHistoryTitle = billHistoryTitle;
                mOnScrollMethod = onScrollMethod;
            }
            public void OnScrollChange(NestedScrollView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
            {
                try
                {
                    bool IsWidgetVisible = isViewVisible(v, mBillHistoryTitle);
                    mOnScrollMethod(IsWidgetVisible);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            private bool isViewVisible(NestedScrollView v, View view)
            {
                Rect scrollBounds = new Rect();
                v.GetDrawingRect(scrollBounds);

                float top = view.GetY() + view.Height;
                float bottom = top + view.Height;

                if (scrollBounds.Top < top && scrollBounds.Bottom > bottom)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [OnClick(Resource.Id.digital_container)]
        void OnManageBillDelivery(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                SetDynatraceCTATags();
                this.SetIsClicked(true);
                Intent intent = new Intent(Activity, typeof(ManageBillDeliveryActivity));
                intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(billRenderingResponse));
                intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(billRenderingTenantResponse));
                intent.PutExtra("accountNumber", mSelectedAccountData.AccountNum);
                StartActivity(intent);
            }
        }

        [OnClick(Resource.Id.btnViewDetails)]
        void OnViewDetails(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent intent = new Intent(Activity, typeof(BillingDetailsActivity));
                intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(mSelectedAccountData));
                intent.PutExtra("SELECTED_BILL_DETAILS", JsonConvert.SerializeObject(selectedAccountChargesModelList[0]));
                intent.PutExtra("PENDING_PAYMENT", isPendingPayment);
                intent.PutExtra("IS_VIEW_BILL_DISABLE", isViewBillDisable);
                intent.PutExtra("billrenderingresponse", JsonConvert.SerializeObject(billRenderingResponse));
                intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(billRenderingTenantResponse));

                StartActivity(intent);
            }
        }

        [OnClick(Resource.Id.btnPayBill)]
        void OnPayment(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                if (isPaymentButtonEnable)
                {
                    this.SetIsClicked(true);
                    Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
                    payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
                    StartActivityForResult(payment_activity, DashboardHomeActivity.PAYMENT_RESULT_CODE);
                }
                else
                {
                    DownTimeEntity pgXEntity = DownTimeEntity.GetByCode(Constants.PG_SYSTEM);
                    Utility.ShowBCRMDOWNTooltip(this.Activity, pgXEntity, () =>
                    {
                        this.SetIsClicked(false);

                    });
                }
            }
        }

        [OnClick(Resource.Id.accountSelection)]
        void OnSelectSupplyAccount(object sender, EventArgs eventArgs)
        {
            try
            {
                ((DashboardHomeActivity)Activity).OnSelectAccount();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.accountSelectionRefresh)]
        void OnSelectSupplyAccountRefresh(object sender, EventArgs eventArgs)
        {
            try
            {
                ((DashboardHomeActivity)Activity).OnSelectAccount();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowSelectFilter()
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent newIntent = new Intent(this.Activity, typeof(SelectItemActivity));
                    string filterDescription = "NONRE";
                    bool isREAccount = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId);
                    if (isREAccount)
                    {
                        filterDescription = "RE";
                    }
                    newIntent.PutExtra("FILTER_DESCRIPTION", filterDescription);
                    newIntent.PutExtra("LIST_TITLE", Utility.GetLocalizedLabel(LanguageConstants.BILL_FILTER, LanguageConstants.BillFilter.FILTER_TITLE));
                    newIntent.PutExtra("LIST_DESCRIPTION", Utility.GetLocalizedLabel(LanguageConstants.BILL_FILTER, LanguageConstants.BillFilter.FILTER_DESC));
                    newIntent.PutExtra("ITEM_LIST", JsonConvert.SerializeObject(itemFilterList));
                    StartActivityForResult(newIntent, 12345);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowDownloadBill()
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent newIntent = new Intent(this.Activity, typeof(AccountStatementSelectionActivity));
                    newIntent.PutExtra(Constants.BILL_HISTORY_IS_EMPTY, selectedBillingHistoryModelList != null ? selectedBillingHistoryModelList.Count == 0 : true);
                    newIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
                    StartActivity(newIntent);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.bill_filter_icon)]
        void OnFilterBillHistory(object sender, EventArgs eventArgs)
        {
            ShowSelectFilter();
        }

        [OnClick(Resource.Id.download_bill_icon)]
        void OnDownloadBillHistory(object sender, EventArgs eventArgs)
        {
            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Bill.View_Account_Statement);
            ShowDownloadBill();
        }

        [OnClick(Resource.Id.btnRefresh)]
        void OnButtonRefresh(object sender, EventArgs eventArgs)
        {
            try
            {
                mPresenter.GetBillingHistoryDetails(mSelectedAccountData.AccountNum, mSelectedAccountData.IsOwner, (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnChargeRefresh)]
        void OnButtonChargeRefresh(object sender, EventArgs eventArgs)
        {
            try
            {
                chargeAvailableContainer.Visibility = ViewStates.Visible;
                unavailableChargeContainer.Visibility = ViewStates.Gone;
                mPresenter.GetBillingChargeHistoryDetails(mSelectedAccountData.AccountNum, mSelectedAccountData.IsOwner, (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnBillingHistoryRefresh)]
        void OnButtonBillRefresh(object sender, EventArgs eventArgs)
        {
            try
            {
                itemisedBillingListShimmer.Visibility = ViewStates.Visible;
                itemisedBillingList.Visibility = ViewStates.Gone;
                emptyItemisedBillingList.Visibility = ViewStates.Gone;
                refreshItemisedBillingList.Visibility = ViewStates.Gone;
                mPresenter.GetBillingBillHistoryDetails(mSelectedAccountData.AccountNum, mSelectedAccountData.IsOwner, (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == 12345)
                {
                    if (resultCode == (int)Result.Ok)
                    {
                        itemFilterList = JsonConvert.DeserializeObject<List<Item>>(data.GetStringExtra("SELECTED_ITEM_LIST"));
                        Item selectedFilter = itemFilterList.Find(itemFilter =>
                        {
                            return itemFilter.selected;
                        });

                        if (_isBillStatement)
                        {
                            string dynatraceTag = string.Empty;
                            if (selectedFilter.type.ToUpper() == "ALL")
                            {
                                dynatraceTag = DynatraceConstants.BR.CTAs.BillFilter.All;
                            }
                            else if (selectedFilter.type.ToUpper() == "BILL")
                            {
                                dynatraceTag = DynatraceConstants.BR.CTAs.BillFilter.Bills;
                            }
                            else if (selectedFilter.type.ToUpper() == "ADVICE")
                            {
                                dynatraceTag = DynatraceConstants.BR.CTAs.BillFilter.Advice;
                            }
                            else if (selectedFilter.type.ToUpper() == "PAYMENT")
                            {
                                dynatraceTag = DynatraceConstants.BR.CTAs.BillFilter.Payments;
                            }
                            DynatraceHelper.OnTrack(dynatraceTag);
                        }

                        UpdateBillingHistory(selectedFilter);
                        itemisedBillingScrollView.ScrollTo(0, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        internal static ItemisedBillingMenuFragment NewInstance(AccountData selectedAccount
            , bool isIneligiblePopUpActive = false)
        {
            ItemisedBillingMenuFragment billsMenuFragment = new ItemisedBillingMenuFragment();
            Bundle args = new Bundle();
            args.PutString(SELECTED_ACCOUNT_KEY, JsonConvert.SerializeObject(selectedAccount));
            args.PutBoolean(INELIGIBLE_POP_UP_ACTIVE_KEY, isIneligiblePopUpActive);
            billsMenuFragment.Arguments = args;
            return billsMenuFragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            itemisedBillingInfoShimmer = view.FindViewById<ShimmerFrameLayout>(Resource.Id.itemisedBillingInfoShimmer);
            paperlessTitle = view.FindViewById<TextView>(Resource.Id.paperlessTitle);
            // paperlessTitle.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Common", "dbrPaperBill"));
            itemisedBillingInfoShimmer.SetShimmer(ShimmerUtils.ShimmerBuilderConfig().Build());
            itemisedBillingInfoShimmer.StartShimmer();
            billFilterIcon.Enabled = false;
            download_bill_icon.Enabled = false;
            this.HasOptionsMenu = true;
            BillOnScrollChangeListener billOnScrollChangeListener = new BillOnScrollChangeListener(ShowBillFilterToolbar, bills_list_title_container);
            itemisedBillingScrollView.SetOnScrollChangeListener(billOnScrollChangeListener);
            TextViewUtils.SetMuseoSans500Typeface(accountSelection, itemisedBillingInfoNote,
                btnViewDetails, btnPayBill, itemisedBillingInfoAmountCurrency, myBillHistoryTitle, btnRefresh,
                btnChargeRefresh, btnBillingHistoryRefresh, accountSelectionRefresh);
            TextViewUtils.SetMuseoSans300Typeface(itemisedBillingInfoDate, itemisedBillingInfoAmount
                , emptyBillingHistoryMessage, unavailableBillMsg, unavailableChargeMsg, refreshBillingHistoryMessage, paperlessTitle);
            TextViewUtils.SetTextSize12(refreshBillingHistoryMessage, paperlessTitle);
            TextViewUtils.SetTextSize14(unavailableChargeMsg, itemisedBillingInfoNote, itemisedBillingInfoDate
                , emptyBillingHistoryMessage, unavailableBillMsg);
            TextViewUtils.SetTextSize16(itemisedBillingInfoAmountCurrency, btnViewDetails, btnPayBill, myBillHistoryTitle
                , accountSelection, accountSelectionRefresh, btnChargeRefresh, btnBillingHistoryRefresh, btnRefresh);
            TextViewUtils.SetTextSize36(itemisedBillingInfoAmount);
            RenderUI();

            SetShowAccountStatementIcon();

            mPresenter.GetBillingHistoryDetails(mSelectedAccountData.AccountNum, mSelectedAccountData.IsOwner, (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");
            digital_container.Visibility = ViewStates.Gone;

            try
            {
                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                ((DashboardHomeActivity)Activity).SetToolBarTitle(billTitle);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RenderUI()
        {
            accountSelection.Text = mSelectedAccountData.AccountNickName;
            accountSelectionRefresh.Text = mSelectedAccountData.AccountNickName;
            myBillHistoryTitle.Text = GetLabelByLanguage("myHistory");
            emptyBillingHistoryMessage.Text = GetLabelByLanguage("noHistoryData");
            btnViewDetails.Text = GetLabelByLanguage("viewMore");
            btnPayBill.Text = GetLabelByLanguage("pay");
            myHistoryTitle = GetLabelByLanguage("myHistory");
            billTitle = GetLabelByLanguage("title");
            if (mPresenter.IsEnableAccountSelection())
            {
                Drawable dropdown = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_spinner_dropdown);
                Drawable transparentDropDown = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_action_dropdown);
                transparentDropDown.Alpha = 0;
                accountSelection.SetCompoundDrawablesWithIntrinsicBounds(transparentDropDown, null, dropdown, null);
                accountSelectionRefresh.SetCompoundDrawablesWithIntrinsicBounds(transparentDropDown, null, dropdown, null);
            }
            else
            {
                accountSelection.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                accountSelectionRefresh.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
            }
        }

        private async void GetBillRenderingAsync(AccountData selectedAccount)
        {
            try
            {
                Activity.RunOnUiThread(async () =>
                {
                    try
                    {
                        GetBillRenderingModel getBillRenderingModel = new GetBillRenderingModel();
                        AccountData dbrAccount = selectedAccount;

                        if (DBRUtility.Instance.IsAccountEligible && DBRUtility.Instance.IsCAEligible(selectedAccount.AccountNum))
                        {
                            if (!AccessTokenCache.Instance.HasTokenSaved(this.Activity))
                            {
                                string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                                AccessTokenCache.Instance.SaveAccessToken(this.Activity, accessToken);
                            }
                            billRenderingResponse = await DBRManager.Instance.GetBillRendering(dbrAccount.AccountNum
                                , AccessTokenCache.Instance.GetAccessToken(this.Activity), dbrAccount.IsOwner);
                            _isOwner = selectedAccount.IsOwner && DBRUtility.Instance.IsCAEligible(dbrAccount.AccountNum);

                            if (billRenderingResponse != null
                                && billRenderingResponse.StatusDetail != null
                                && billRenderingResponse.StatusDetail.IsSuccess
                                && billRenderingResponse.Content != null)
                            {
                                //For tenant checking DBR 
                                List<string> dBRCAs = DBRUtility.Instance.GetCAList();
                                List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                                billRenderingTenantResponse = await DBRManager.Instance.PostBREligibilityIndicators(dBRCAs, UserEntity.GetActive().UserID, AccessTokenCache.Instance.GetAccessToken(this.Activity));
                                bool tenantAllowOptIn = false;

                                if (billRenderingTenantResponse != null
                                    && billRenderingTenantResponse.StatusDetail != null
                                    && billRenderingTenantResponse.StatusDetail.IsSuccess
                                    && billRenderingTenantResponse.Content != null)
                                {
                                    bool isOwnerOverRule = billRenderingTenantResponse.Content.Find(x => x.CaNo == dbrAccount.AccountNum).IsOwnerOverRule;
                                    bool isOwnerAlreadyOptIn = billRenderingTenantResponse.Content.Find(x => x.CaNo == dbrAccount.AccountNum).IsOwnerAlreadyOptIn;
                                    bool isTenantAlreadyOptIn = billRenderingTenantResponse.Content.Find(x => x.CaNo == dbrAccount.AccountNum).IsTenantAlreadyOptIn;
                                    bool AccountHasOwner = accounts.Find(x => x.AccNum == dbrAccount.AccountNum).AccountHasOwner;

                                    if (AccountHasOwner && !isOwnerOverRule && !isOwnerAlreadyOptIn && !isTenantAlreadyOptIn)
                                    {
                                        tenantAllowOptIn = true;
                                    }
                                }

                                if (billRenderingResponse.Content.DBRType == MobileEnums.DBRTypeEnum.None)
                                {
                                    isDigitalContainerVisible = false;
                                    digital_container.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    isDigitalContainerVisible = true;
                                    digital_container.Visibility = ViewStates.Visible;
                                    if (billRenderingResponse.Content.DBRType == MobileEnums.DBRTypeEnum.EBill
                                        || billRenderingResponse.Content.DBRType == MobileEnums.DBRTypeEnum.EBillWithCTA)
                                    {
                                        bill_paperless_icon.SetImageResource(Resource.Drawable.icon_digitalbill);
                                        paperlessTitle.TextFormatted = GetFormattedText(billRenderingResponse.Content.SegmentMessage ?? string.Empty);
                                    }
                                    else if (billRenderingResponse.Content.DBRType == MobileEnums.DBRTypeEnum.Email
                                        || billRenderingResponse.Content.DBRType == MobileEnums.DBRTypeEnum.EmailWithCTA)
                                    {
                                        bill_paperless_icon.SetImageResource(Resource.Drawable.Icon_DBR_EMail);
                                        paperlessTitle.TextFormatted = GetFormattedText(billRenderingResponse.Content.SegmentMessage ?? string.Empty);
                                    }
                                    if (billRenderingResponse.Content.DBRType == MobileEnums.DBRTypeEnum.Paper)
                                    {
                                        bill_paperless_icon.SetImageResource(Resource.Drawable.Icon_DBR_EBill);

                                        if (_isOwner)
                                        {
                                            paperlessTitle.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Common", "dbrPaperBill"));
                                        }
                                        else
                                        {
                                            if (tenantAllowOptIn)
                                            {
                                                paperlessTitle.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Common", "dbrPaperBill"));
                                            }
                                            else
                                            {
                                                paperlessTitle.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Common", "dbrPaperBillNonOwner"));
                                            }

                                        }
                                    }
                                    SetDynatraceScreenTags();
                                }

                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public int GetItemisedBillingHeaderImageHeight()
        {
            return itemisedBillingHeaderImage.Height;
        }

        public int GetitemisedBillingInfoContainerHeight()
        {
            return itemisedBillingInfoContainer.Height;
        }

        public int GetitemisedBillingInfoShimmerHeight()
        {
            return itemisedBillingInfoShimmer.Height;
        }

        public int GetChargeAvailableContainerHeight()
        {
            return chargeAvailableContainer.Height;
        }

        public int GetchargeAvailableNoCTAContainerHeight()
        {
            return chargeAvailableNoCTAContainer.Height;
        }

        public bool IsCADBREligible()
        {
            return DBRUtility.Instance.IsCAEligible(mSelectedAccountData.AccountNum);
        }

        public bool IsDigitalContainerVisible()
        {
            return isDigitalContainerVisible;
        }

        private void SetDynatraceScreenTags()
        {
            string dynatraceTag;
            switch (billRenderingResponse.Content.CurrentRenderingMethod)
            {
                case MobileEnums.RenderingMethodEnum.EBill:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.Bills.EBill;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Email:
                case MobileEnums.RenderingMethodEnum.Email:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.Bills.EBill_Email;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.Bills.EBill_Paper;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.Email_Paper:
                case MobileEnums.RenderingMethodEnum.EBill_Email_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.Screens.Bills.EBill_Email_Paper;
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
            string dynatraceTag;
            switch (billRenderingResponse.Content.CurrentRenderingMethod)
            {
                case MobileEnums.RenderingMethodEnum.EBill:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.Bills.EBill;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Email:
                case MobileEnums.RenderingMethodEnum.Email:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.Bills.EBill_Email;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.EBill_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.Bills.EBill_Paper;
                        break;
                    }
                case MobileEnums.RenderingMethodEnum.Email_Paper:
                case MobileEnums.RenderingMethodEnum.EBill_Email_Paper:
                    {
                        dynatraceTag = DynatraceConstants.DBR.CTAs.Bills.EBill_Email_Paper;
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

        public void ShowShimmerLoading()
        {
            itemisedBillingHeaderImage.SetImageResource(Resource.Drawable.bill_menu_loading_banner);

            itemisedBillingList.Visibility = ViewStates.Gone;
            itemisedBillingInfoContainer.Visibility = ViewStates.Gone;

            itemisedBillingListShimmer.Visibility = ViewStates.Visible;
            itemisedBillingInfoShimmer.Visibility = ViewStates.Visible;
        }

        public void UpdateBillingHistory(Item selectedFilter)
        {
            itemFilterList.ForEach(filterItem =>
            {
                filterItem.selected = (filterItem.type == selectedFilter.type) ? true : false;
            });
            isFiltered = (selectedFilter.type == "ALL") ? false : true;
            RenderBillingHistoryList(selectedFilter.type);
        }

        private void RenderBillingHistoryList(string historyType)
        {
            itemisedBillingList.RemoveAllViews();
            ItemisedBillingGroupComponent itemisedBillingGroupComponent;
            List<AccountBillPayHistoryModel> filteredBillingList = new List<AccountBillPayHistoryModel>();
            UpdateFilterIcon();
            if (string.IsNullOrEmpty(historyType) || historyType == "ALL")
            {
                filteredBillingList.AddRange(selectedBillingHistoryModelList);
            }
            else
            {
                selectedBillingHistoryModelList.ForEach(billingHistory =>
                {
                    int foundIndex = billingHistory.BillingHistoryDataList.FindIndex(data =>
                    {
                        return data.HistoryType == historyType;
                    });

                    if (foundIndex != -1)
                    {
                        filteredBillingList.Add(billingHistory);
                    }
                });
            }

            if (filteredBillingList.Count > 0)
            {
                emptyItemisedBillingList.Visibility = ViewStates.Gone;
                itemisedBillingList.Visibility = ViewStates.Visible;
                for (int i = 0; i < filteredBillingList.Count; i++)
                {
                    itemisedBillingGroupComponent = new ItemisedBillingGroupComponent(Activity);
                    ItemisedBillingGroupContentComponent content;

                    AccountBillPayHistoryModel model = filteredBillingList[i];
                    itemisedBillingGroupComponent.SetMonthYearLabel(model.MonthYear);

                    if (i == 0)
                    {
                        itemisedBillingGroupComponent.ShowSeparator(false);
                    }
                    else
                    {
                        itemisedBillingGroupComponent.ShowSeparator(true);
                        itemisedBillingGroupComponent.SetBackground();
                    }

                    System.Globalization.CultureInfo currCult = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

                    for (int j = 0; j < model.BillingHistoryDataList.Count; j++)
                    {
                        AccountBillPayHistoryModel.BillingHistoryData data = model.BillingHistoryDataList[j];
                        //Rendering All History Type
                        if (string.IsNullOrEmpty(historyType) || historyType == "ALL")
                        {
                            content = new ItemisedBillingGroupContentComponent(Activity);
                            content.IsPayment(data.HistoryType.ToUpper() == "PAYMENT");
                            content.SetDateHistoryType(data.DateAndHistoryType);
                            content.SetPaidVia(data.PaidVia);
                            if (double.Parse(data.Amount, currCult) < 0)
                            {
                                content.SetAmount("- RM " + mDecimalFormat.Format(double.Parse(data.Amount, currCult) * -1), data.IsPaymentPending);
                            }
                            else
                            {
                                content.SetAmount("RM " + mDecimalFormat.Format(double.Parse(data.Amount, currCult)), data.IsPaymentPending);  //ori
                            }


                            if (data.DetailedInfoNumber != "" && !data.IsPaymentPending)
                            {
                                content.SetShowBillingDetailsListener(new OnShowBillingDetailsListener(this, data));
                            }
                            else
                            {
                                content.SetShowBillingDetailsListener(null);
                            }
                            itemisedBillingGroupComponent.AddContent(content);
                            itemisedBillingGroupComponent.ShowContentSeparators();


                        }
                        //Rendering Filtered History Type
                        else
                        {
                            if (historyType == data.HistoryType)
                            {
                                content = new ItemisedBillingGroupContentComponent(Activity);
                                content.IsPayment(data.HistoryType.ToUpper() == "PAYMENT");
                                content.SetDateHistoryType(data.DateAndHistoryType);
                                content.SetPaidVia(data.PaidVia);
                                if (double.Parse(data.Amount, currCult) < 0)
                                {
                                    content.SetAmount("- RM " + mDecimalFormat.Format(double.Parse(data.Amount, currCult) * -1), data.IsPaymentPending);
                                }
                                else
                                {
                                    content.SetAmount("RM " + mDecimalFormat.Format(double.Parse(data.Amount, currCult)), data.IsPaymentPending);  //ori
                                }


                                if (data.DetailedInfoNumber != "" && !data.IsPaymentPending)
                                {
                                    content.SetShowBillingDetailsListener(new OnShowBillingDetailsListener(this, data));
                                }
                                else
                                {
                                    content.SetShowBillingDetailsListener(null);
                                }
                                itemisedBillingGroupComponent.AddContent(content);
                                itemisedBillingGroupComponent.ShowContentSeparators();
                            }
                        }
                    }
                    itemisedBillingList.AddView(itemisedBillingGroupComponent);
                }
            }
            else
            {
                emptyItemisedBillingList.Visibility = ViewStates.Visible;
                bool isREAccount = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId);
                if (isREAccount)
                {
                    emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyHistoryRE");
                }
                else
                {
                    emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyHistory");
                }
                if (historyType.ToUpper() == Constants.ITEMIZED_BILLING_PAYMENT_KEY)
                {
                    if (isREAccount)
                    {
                        emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyPaymentHistoryRE");
                    }
                    else
                    {
                        emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyPaymentHistory");
                    }
                }
                else if (historyType.ToUpper() == Constants.ITEMIZED_BILLING_BILL_KEY)
                {
                    if (isREAccount)
                    {
                        emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyBillHistoryRE");
                    }
                    else
                    {
                        emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyBillHistory");
                    }
                }
                else if (historyType.ToUpper() == Constants.ITEMIZED_BILLING_ADVICE_KEY)
                {
                    if (isREAccount)
                    {
                        emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyBillHistoryRE");
                    }
                    else
                    {
                        emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyBillHistory");
                    }
                }

                itemisedBillingList.Visibility = ViewStates.Gone;
            }
        }

        public void ShowEmptyState()
        {
            int imageResource = Resource.Drawable.bill_menu_loading_banner;
            itemisedBillingInfoShimmer.Visibility = ViewStates.Gone;
            itemisedBillingInfoContainer.Visibility = ViewStates.Visible;
            itemisedBillingCTAContainer.Visibility = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId) ? ViewStates.Gone : ViewStates.Visible;
            itemisedBillingInfoNote.Text = GetLabelByLanguage("needToPay");
            itemisedBillingInfoAmount.Text = "0.00";
            itemisedBillingInfoNote.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
            itemisedBillingInfoAmount.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
            itemisedBillingInfoAmountCurrency.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
            itemisedBillingInfoDate.Visibility = ViewStates.Gone;
            itemisedBillingHeaderImage.SetImageResource(imageResource);


            itemisedBillingListShimmer.Visibility = ViewStates.Gone;
            itemisedBillingList.Visibility = ViewStates.Gone;
            emptyItemisedBillingList.Visibility = ViewStates.Visible;
            EnableActionButtons(false);
        }

        private void EnableActionButtons(bool isEnable)
        {
            btnViewDetails.Enabled = isEnable;

            if (isEnable)
            {
                btnViewDetails.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.freshGreen)));
                btnViewDetails.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_button_background);
            }
            else
            {
                btnViewDetails.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.silverChalice)));
                btnViewDetails.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_button_background_disabled);
            }

            isPaymentButtonEnable = Utility.IsEnablePayment();
            btnPayBill.Enabled = true;
            if (isPaymentButtonEnable)
            {
                btnPayBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.green_button_background);
            }
            else
            {
                btnPayBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);
            }
        }

        private void UpdateFilterItems(List<AccountBillPayFilter> billPayFilters)
        {
            itemFilterList = new List<Item>();
            billPayFilters.ForEach(filter =>
            {
                Item item = new Item();
                item.title = filter.Text;
                item.type = filter.Type;
                itemFilterList.Add(item);
            });
            itemFilterList[0].selected = true;
        }
        private void SetShowAccountStatementIcon()
        {
            _isBillStatement = BillRedesignUtility.Instance.IsAccountStatementEligible(mSelectedAccountData.AccountNum, mSelectedAccountData.IsOwner);
            download_bill_icon.Visibility = _isBillStatement ? ViewStates.Visible : ViewStates.Gone;
            download_bill_icon.Enabled = false;
        }
        public void PopulateBillingHistoryList(List<AccountBillPayHistoryModel> billingHistoryModelList, List<AccountBillPayFilter> billPayFilters)
        {
            itemisedBillingListShimmer.Visibility = ViewStates.Gone;

            if (billPayFilters != null && billPayFilters.Count > 0)
            {
                UpdateFilterItems(billPayFilters);
            }

            billFilterIcon.Visibility = ViewStates.Visible;
            billFilterIcon.Enabled = true;
            download_bill_icon.Enabled = true;

            if (billingHistoryModelList.Count > 0)
            {
                selectedBillingHistoryModelList = new List<AccountBillPayHistoryModel>();
                selectedBillingHistoryModelList = billingHistoryModelList;
                emptyItemisedBillingList.Visibility = ViewStates.Gone;
                itemisedBillingList.Visibility = ViewStates.Visible;
                EnableActionButtons(true);
                RenderBillingHistoryList(null);

                isPendingPayment = false;

                for (int i = 0; i < billingHistoryModelList.Count; i++)
                {
                    if (billingHistoryModelList[i].BillingHistoryDataList != null && billingHistoryModelList[i].BillingHistoryDataList.Count > 0)
                    {
                        for (int j = 0; j < billingHistoryModelList[i].BillingHistoryDataList.Count; j++)
                        {
                            if (billingHistoryModelList[i].BillingHistoryDataList[j].IsPaymentPending)
                            {
                                isPendingPayment = true;
                                break;
                            }
                        }
                    }

                    if (isPendingPayment)
                    {
                        break;
                    }
                }

                if (isPendingPayment && unavailableChargeContainer.Visibility != ViewStates.Visible)
                {
                    UpdatePendingPaymentCharge();
                }
            }
            else
            {
                bool isREAccount = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId);
                if (isREAccount)
                {
                    emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyHistoryRE");
                }
                else
                {
                    emptyBillingHistoryMessage.Text = GetLabelByLanguage("emptyHistory");
                }
                emptyItemisedBillingList.Visibility = ViewStates.Visible;
                itemisedBillingList.Visibility = ViewStates.Gone;
            }

            isViewBillDisable = true;
            if (billingHistoryModelList != null && billingHistoryModelList.Count > 0)
            {
                for (int i = 0; i < billingHistoryModelList.Count; i++)
                {
                    var find = billingHistoryModelList[i].BillingHistoryDataList.Find(x => x.HistoryType.ToUpper() == Constants.ITEMIZED_BILLING_BILL_KEY);

                    if (find != null && find.HistoryType.ToUpper() == Constants.ITEMIZED_BILLING_BILL_KEY)
                    {
                        isViewBillDisable = false;
                    }

                    if (!isViewBillDisable)
                    {
                        break;
                    }
                }
            }
        }

        public void UpdatePendingPaymentCharge()
        {
            itemisedBillingHeaderImage.SetImageResource(Resource.Drawable.bill_payment_processing_banner);
            itemisedBillingInfoNote.Text = Utility.GetLocalizedCommonLabel("paymentPendingMsg");
            itemisedBillingInfoAmount.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.lightOrange)));
            itemisedBillingInfoAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.lightOrange)));
            itemisedBillingInfoDate.Visibility = ViewStates.Gone;
        }

        public void PopulateAccountCharge(List<AccountChargeModel> accountChargesModelList)
        {
            itemisedBillingInfoShimmer.Visibility = ViewStates.Gone;
            itemisedBillingInfoContainer.Visibility = ViewStates.Visible;
            itemisedBillingCTAContainer.Visibility = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId) ? ViewStates.Gone : ViewStates.Visible;

            selectedAccountChargesModelList = accountChargesModelList.GetRange(0, accountChargesModelList.Count);
            AccountChargeModel accountChargeModel = accountChargesModelList[0];
            int imageResource = Resource.Drawable.bill_no_outstanding_banner;

            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            itemisedBillingInfoAmount.Text = accountChargeModel.AmountDue.ToString("#,##0.00", currCult);
            bool isREAccount = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId);
            if (accountChargeModel.IsCleared)
            {
                if (isREAccount)
                {
                    imageResource = Resource.Drawable.bill_paid_extra_re_banner;
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("myEarnings");
                }
                else
                {
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("clearedBills");
                }
                itemisedBillingInfoNote.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                itemisedBillingInfoDate.Visibility = ViewStates.Gone;
            }
            else if (accountChargeModel.IsPaidExtra)
            {
                if (isREAccount)
                {
                    imageResource = Resource.Drawable.bill_paid_extra_re_banner;
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("myEarnings");
                    itemisedBillingInfoDate.Text = GetLabelByLanguage("getBy") + " " + dateFormatter.Format(dateParser.Parse(accountChargeModel.DueDate));
                    itemisedBillingInfoDate.Visibility = ViewStates.Visible;

                    itemisedBillingInfoNote.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                }
                else
                {
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("paidExtra");

                    itemisedBillingInfoNote.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Android.Graphics.Color.ParseColor("#20bd4c"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Android.Graphics.Color.ParseColor("#20bd4c"));
                    itemisedBillingInfoDate.Visibility = ViewStates.Gone;
                }
                itemisedBillingInfoAmount.Text = (Math.Abs(accountChargeModel.AmountDue)).ToString("#,##0.00", currCult);
            }
            else if (accountChargeModel.IsNeedPay)
            {
                if (isREAccount)
                {
                    imageResource = Resource.Drawable.bill_paid_extra_re_banner;
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("beenPaidExtra");
                    itemisedBillingInfoDate.Visibility = ViewStates.Gone;
                    itemisedBillingInfoNote.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                }
                else
                {
                    imageResource = Resource.Drawable.bill_need_to_pay_banner;
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("needToPay");
                    itemisedBillingInfoDate.Text = GetLabelByLanguage("by") + " " + dateFormatter.Format(dateParser.Parse(accountChargeModel.DueDate));
                    itemisedBillingInfoDate.Visibility = ViewStates.Visible;

                    itemisedBillingInfoNote.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                }
                itemisedBillingInfoAmount.Text = (Math.Abs(accountChargeModel.AmountDue)).ToString("#,##0.00", currCult);
            }
            EnableActionButtons(true);
            itemisedBillingHeaderImage.SetImageResource(imageResource);

            if (isPendingPayment)
            {
                UpdatePendingPaymentCharge();
            }
        }

        public void ShowBillPDFPage(AccountBillPayHistoryModel.BillingHistoryData billHistoryData)
        {
            BillHistoryV5 selectedBill = new BillHistoryV5();
            selectedBill.DtBill = billPdfDateFormatter.Format((billPdfDateParser.Parse(billHistoryData.PaidVia)));
            selectedBill.NrBill = billHistoryData.DetailedInfoNumber;
            Intent viewBill = new Intent(Activity, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
            if (_isBillStatement)
            {
                DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Bill.View_Bill);
                DynatraceHelper.OnTrack(DynatraceConstants.BR.Screens.Bill.View_Bill);
            }
            StartActivity(viewBill);
        }

        public void ShowPayPDFPage(AccountBillPayHistoryModel.BillingHistoryData billHistoryData)
        {
            Intent viewReceipt = new Intent(this.Activity, typeof(ViewReceiptMultiAccountNewDesignActivty));
            viewReceipt.PutExtra("SELECTED_ACCOUNT_NUMBER", mSelectedAccountData.AccountNum);
            viewReceipt.PutExtra("DETAILED_INFO_NUMBER", billHistoryData.DetailedInfoNumber);
            viewReceipt.PutExtra("IS_OWNED_ACCOUNT", true);
            viewReceipt.PutExtra("IS_SHOW_ALL_RECEIPT", false);
            StartActivity(viewReceipt);
        }

        public void ShowUnavailableContent(bool isShowRefresh, string btnText, string contentText)
        {
            billingHistoryDetailsContent.Visibility = ViewStates.Gone;
            unavailableBillContainer.Visibility = ViewStates.Visible;
            //If not refresh bill, show downtime
            if (isShowRefresh)
            {
                unavailableBillBannerImg.SetImageResource(Resource.Drawable.bg_application_status);

                if (!string.IsNullOrEmpty(contentText))
                {
                    unavailableBillMsg.TextFormatted = GetFormattedText(contentText);
                }
                else
                {
                    unavailableBillMsg.TextFormatted = GetFormattedText(Utility.GetLocalizedErrorLabel("refreshBillDetails"));
                }
                if (!string.IsNullOrEmpty(btnText))
                {
                    btnRefresh.Text = btnText;
                }
                else
                {
                    btnRefresh.Text = Utility.GetLocalizedCommonLabel("refreshNow");
                }
                btnRefresh.Visibility = ViewStates.Visible;
            }
            else
            {
                unavailableBillBannerImg.SetImageResource(Resource.Drawable.downtime_banner);
                if (!string.IsNullOrEmpty(contentText))
                {
                    unavailableBillMsg.TextFormatted = GetFormattedText(contentText);
                }
                else
                {
                    unavailableBillMsg.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"));
                }
                btnRefresh.Visibility = ViewStates.Gone;
            }
        }

        public void ShowUnavailableChargeContent(bool isShowRefresh, string btnText, string contentText)
        {
            chargeAvailableContainer.Visibility = ViewStates.Gone;
            unavailableChargeContainer.Visibility = ViewStates.Visible;
            //If not refresh bill, show downtime
            if (isShowRefresh)
            {
                unavailableChargeImg.SetImageResource(Resource.Drawable.refresh_1);
                if (!string.IsNullOrEmpty(contentText))
                {
                    unavailableChargeMsg.TextFormatted = GetFormattedText(contentText);
                }
                else
                {
                    unavailableChargeMsg.TextFormatted = GetFormattedText(Utility.GetLocalizedErrorLabel("refreshBillDetails"));
                }
                if (!string.IsNullOrEmpty(btnText))
                {
                    btnChargeRefresh.Text = btnText;
                }
                else
                {
                    btnChargeRefresh.Text = Utility.GetLocalizedCommonLabel("refreshNow");
                }
                btnChargeRefresh.Visibility = ViewStates.Visible;
            }
            else
            {
                unavailableChargeImg.SetImageResource(Resource.Drawable.maintenance_new);
                if (!string.IsNullOrEmpty(contentText))
                {
                    unavailableChargeMsg.TextFormatted = GetFormattedText(contentText);
                }
                else
                {
                    unavailableChargeMsg.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"));
                }
                btnChargeRefresh.Visibility = ViewStates.Gone;
            }
        }

        public void ShowUnavailableBillContent(bool isShowRefresh, string btnText, string contentText)
        {
            itemisedBillingListShimmer.Visibility = ViewStates.Gone;
            itemisedBillingList.Visibility = ViewStates.Gone;
            emptyItemisedBillingList.Visibility = ViewStates.Gone;
            refreshItemisedBillingList.Visibility = ViewStates.Visible;
            //If not refresh bill, show downtime
            if (isShowRefresh)
            {
                refreshItemisedBillingImg.SetImageResource(Resource.Drawable.refresh_1);
                if (!string.IsNullOrEmpty(contentText))
                {
                    refreshBillingHistoryMessage.TextFormatted = GetFormattedText(contentText);
                }
                else
                {
                    refreshBillingHistoryMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedErrorLabel("refreshBillPayHistory"));
                }
                if (!string.IsNullOrEmpty(btnText))
                {
                    btnBillingHistoryRefresh.Text = btnText;
                }
                else
                {
                    btnBillingHistoryRefresh.Text = Utility.GetLocalizedCommonLabel("refreshNow");
                }
                btnBillingHistoryRefresh.Visibility = ViewStates.Visible;
            }
            else
            {
                refreshItemisedBillingImg.SetImageResource(Resource.Drawable.maintenance_new);
                if (!string.IsNullOrEmpty(contentText))
                {
                    refreshBillingHistoryMessage.TextFormatted = GetFormattedText(contentText);
                }
                else
                {
                    refreshBillingHistoryMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"));
                }
                btnBillingHistoryRefresh.Visibility = ViewStates.Gone;
            }
        }

        public void ShowDowntimeSnackbar(string message)
        {
            Snackbar downtimeSnackBar = Snackbar.Make(rootView,
                            message,
                            Snackbar.LengthLong);
            View v = downtimeSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(4);
            if (!mSelectedAccountData.AccountCategoryId.Equals("2"))
            {
                downtimeSnackBar.Show();
            }
            this.SetIsClicked(false);
        }

        public void ShowAvailableBillContent()
        {
            billingHistoryDetailsContent.Visibility = ViewStates.Visible;
            unavailableBillContainer.Visibility = ViewStates.Gone;
        }

        class OnShowBillingDetailsListener : Java.Lang.Object, View.IOnClickListener
        {
            ItemisedBillingMenuFragment mView;
            AccountBillPayHistoryModel.BillingHistoryData mBillHistoryData;

            public OnShowBillingDetailsListener(ItemisedBillingMenuFragment view, AccountBillPayHistoryModel.BillingHistoryData billHistoryData)
            {
                mView = view;
                mBillHistoryData = billHistoryData;
            }
            public void OnClick(View v)
            {
                if (!mView.GetIsClicked())
                {
                    mView.SetIsClicked(true);
                    if (mBillHistoryData.HistoryType.ToUpper() == "PAYMENT")
                    {
                        mView.ShowPayPDFPage(mBillHistoryData);
                    }
                    else
                    {
                        mView.ShowBillPDFPage(mBillHistoryData);
                    }
                }
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
            ShowBackButton(false);

            try
            {
                ((DashboardHomeActivity)this.Activity).RemoveHeaderDropDown();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            Handler h = new Handler();
            Action myAction = () =>
            {
                try
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    if (this.mPresenter != null && this.mPresenter.IsTutorialShowNeeded())
                    {
                        this.mPresenter.OnCheckToCallItemizedTutorial();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            };
            h.PostDelayed(myAction, 50);
        }

        public void ShowBackButton(bool flag)
        {
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(flag);
            actionBar.SetDisplayShowHomeEnabled(flag);
        }

        public override void OnPause()
        {
            base.OnPause();
            try
            {
                NewAppTutorialUtils.ForceCloseNewAppTutorial();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.BillsToolbarMenu, menu);

            billFilterMenuItem = menu.FindItem(Resource.Id.action_bills_filter);
            billFilterMenuItem.SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.bill_screen_filter_icon));
            billFilterMenuItem.SetVisible(false);
            billDownloadMenuItem = menu.FindItem(Resource.Id.action_bills_download);
            billDownloadMenuItem.SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.Icon_Acct_Stmnt_Download_White));
            billDownloadMenuItem.SetVisible(false);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void OnShowItemizedFragmentTutorialDialog()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        StopScrolling();
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
                NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), this.mPresenter.OnGeneraNewAppTutorialList(_isOwner, isDigitalContainerVisible, _isBillStatement, mSelectedAccountData));

            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ItemizedBillingCustomScrolling(int yPosition)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        itemisedBillingScrollView.ScrollTo(0, yPosition);
                        itemisedBillingScrollView.RequestLayout();
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool CheckIsScrollable()
        {
            View child = (View)itemisedBillingScrollView.GetChildAt(0);

            return itemisedBillingScrollView.Height < child.Height + itemisedBillingScrollView.PaddingTop + itemisedBillingScrollView.PaddingBottom;
        }

        public int GetButtonWidth()
        {
            return btnPayBill.Width;
        }

        public int GetButtonHeight()
        {
            return btnPayBill.Height;
        }

        public int GetDigitalContainerHeight()
        {
            return digital_container.Height;
        }

        public int OnGetEndOfScrollView()
        {
            View child = (View)itemisedBillingScrollView.GetChildAt(0);

            return child.Height + itemisedBillingScrollView.PaddingTop + itemisedBillingScrollView.PaddingBottom;
        }

        public void StopScrolling()
        {
            try
            {
                itemisedBillingScrollView.SmoothScrollBy(0, 0);
                itemisedBillingScrollView.ScrollTo(0, 0);
                itemisedBillingScrollView.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        private void UpdateFilterIcon()
        {
            if (isFiltered)
            {
                billFilterMenuItem.SetIcon(Resource.Drawable.filter_filled);
                billFilterIcon.SetImageResource(Resource.Drawable.filter_blue);
            }
            else
            {
                billFilterMenuItem.SetIcon(Resource.Drawable.Icon_Bills_Filter_White);
                billFilterIcon.SetImageResource(Resource.Drawable.bill_screen_filter_icon);
            }
        }

        public bool GetIsPendingPayment()
        {
            return isPendingPayment;
        }

        public bool GetIsIneligiblePopUpActive()
        {
            return isIneligiblePopUpActive;
        }
    }
}
