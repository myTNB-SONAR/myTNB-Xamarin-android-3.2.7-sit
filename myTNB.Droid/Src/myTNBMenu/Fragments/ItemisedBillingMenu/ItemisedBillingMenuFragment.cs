
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Java.Text;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Billing.MVP;
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

        ItemisedBillingMenuPresenter mPresenter;
        AccountData mSelectedAccountData;

        List<AccountChargeModel> selectedAccountChargesModelList;
        List<Item> itemFilterList = new List<Item>();
        List<AccountBillPayHistoryModel> selectedBillingHistoryModelList;
        List<AccountBillPayFilter> billPayFilterList;

        SimpleDateFormat dateParser = new SimpleDateFormat("yyyyMMdd");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");

        SimpleDateFormat billPdfDateParser = new SimpleDateFormat("dd MMM yyyy");
        SimpleDateFormat billPdfDateFormatter = new SimpleDateFormat("dd/MM/yyyy");

        IMenuItem billFilterMenuItem;


        const string SELECTED_ACCOUNT_KEY = "SELECTED_ACCOUNT";
        const string PAGE_ID = "Bills";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ItemisedBillingMenuPresenter(this);
            Bundle extras = Arguments;

            if (extras.ContainsKey(SELECTED_ACCOUNT_KEY))
            {
                mSelectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString(SELECTED_ACCOUNT_KEY));
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
                case Resource.Id.action_notification:
                    ShowSelectFilter();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void ShowBillFilterToolbar(bool isShow)
        {
            if (isShow)
            {
                ((DashboardHomeActivity)this.Activity).SetToolBarTitle("My History");
                this.billFilterMenuItem.SetVisible(true);
            }
            else
            {
                ((DashboardHomeActivity)this.Activity).SetToolBarTitle("Bills");
                this.billFilterMenuItem.SetVisible(false);
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
                bool IsWidgetVisible = isViewVisible(v,mBillHistoryTitle);
                mOnScrollMethod(IsWidgetVisible);
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

        [OnClick(Resource.Id.btnViewDetails)]
        void OnViewDetails(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent intent = new Intent(Activity, typeof(BillingDetailsActivity));
                intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(mSelectedAccountData));
                intent.PutExtra("SELECTED_BILL_DETAILS", JsonConvert.SerializeObject(selectedAccountChargesModelList[0]));
                StartActivity(intent);
            }
        }

        [OnClick(Resource.Id.btnPayBill)]
        void OnPayment(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
                payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
                StartActivityForResult(payment_activity, DashboardHomeActivity.PAYMENT_RESULT_CODE);
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

        private void ShowSelectFilter()
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent newIntent = new Intent(this.Activity, typeof(FilterBillHistoryActivity));
                    newIntent.PutExtra("ITEM_LIST", JsonConvert.SerializeObject(itemFilterList));
                    StartActivityForResult(newIntent, 12345);
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

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == 12345)
                {
                    if (resultCode == Result.Ok)
                    {
                        UpdateBillingHistory(data.GetStringExtra("SELECTED_ITEM_FILTER"));
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        internal static ItemisedBillingMenuFragment NewInstance(AccountData selectedAccount)
        {
            ItemisedBillingMenuFragment billsMenuFragment = new ItemisedBillingMenuFragment();
            Bundle args = new Bundle();
            args.PutString(SELECTED_ACCOUNT_KEY, JsonConvert.SerializeObject(selectedAccount));
            billsMenuFragment.Arguments = args;
            return billsMenuFragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            itemisedBillingInfoShimmer = view.FindViewById<ShimmerFrameLayout>(Resource.Id.itemisedBillingInfoShimmer);
            itemisedBillingInfoShimmer.SetShimmer(ShimmerUtils.ShimmerBuilderConfig().Build());
            itemisedBillingInfoShimmer.StartShimmer();

            SetHasOptionsMenu(true);
            itemisedBillingScrollView.SetOnScrollChangeListener(new BillOnScrollChangeListener(ShowBillFilterToolbar, bills_list_title_container));
            TextViewUtils.SetMuseoSans500Typeface(accountSelection, itemisedBillingInfoNote,
                btnViewDetails, btnPayBill, itemisedBillingInfoAmountCurrency, myBillHistoryTitle, btnRefresh);
            TextViewUtils.SetMuseoSans300Typeface(itemisedBillingInfoDate, itemisedBillingInfoAmount, emptyBillingHistoryMessage, unavailableBillMsg);
            RenderUI();

            mPresenter.GetBillingHistoryDetails(mSelectedAccountData.AccountNum, mSelectedAccountData.IsOwner, (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");

            try
            {
                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.bg_smr);
                ((DashboardHomeActivity)Activity).SetToolBarTitle(GetLabelByLanguage("title"));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RenderUI()
        {
            accountSelection.Text = mSelectedAccountData.AccountNickName;
            myBillHistoryTitle.Text = GetLabelByLanguage("myHistory");
            emptyBillingHistoryMessage.Text = GetLabelByLanguage("noHistoryData");
            btnViewDetails.Text = GetLabelByLanguage("viewMore");
            btnPayBill.Text = GetLabelByLanguage("pay");
            if (mPresenter.IsEnableAccountSelection())
            {
                Drawable dropdown = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_spinner_dropdown);
                Drawable transparentDropDown = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_action_dropdown);
                transparentDropDown.Alpha = 0;
                accountSelection.SetCompoundDrawablesWithIntrinsicBounds(transparentDropDown, null, dropdown, null);
            }
            else
            {
                accountSelection.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
            }
        }

        public void ShowShimmerLoading()
        {
            itemisedBillingHeaderImage.SetImageResource(Resource.Drawable.bill_menu_loading_banner);

            itemisedBillingList.Visibility = ViewStates.Gone;
            itemisedBillingInfoContainer.Visibility = ViewStates.Gone;

            itemisedBillingListShimmer.Visibility = ViewStates.Visible;
            itemisedBillingInfoShimmer.Visibility = ViewStates.Visible;
        }

        public void UpdateBillingHistory(string filterItemString)
        {
            Item selectedFilter = JsonConvert.DeserializeObject<Item>(filterItemString);
            itemFilterList.ForEach(filterItem =>
            {
                filterItem.selected = (filterItem.type == selectedFilter.type) ? true : false;
            });
            RenderBillingHistoryList(selectedFilter.type);
        }

        private void RenderBillingHistoryList(string historyType)
        {
            itemisedBillingList.RemoveAllViews();
            ItemisedBillingGroupComponent itemisedBillingGroupComponent;
            List<AccountBillPayHistoryModel> filteredBillingList = new List<AccountBillPayHistoryModel>();
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
                            content.SetAmount("RM " + data.Amount);
                            if (data.DetailedInfoNumber != "")
                            {
                                content.SetShowBillingDetailsListener(new OnShowBillingDetailsListener(this, data));
                            }
                            else
                            {
                                content.SetShowBillingDetailsListener(null);
                            }

                            if (j == (model.BillingHistoryDataList.Count - 1))
                            {
                                content.ShowSeparator(false);
                            }
                            else
                            {
                                content.ShowSeparator(true);
                            }

                            itemisedBillingGroupComponent.AddContent(content);
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
                                content.SetAmount("RM " + data.Amount);
                                if (data.DetailedInfoNumber != "")
                                {
                                    content.SetShowBillingDetailsListener(new OnShowBillingDetailsListener(this, data));
                                }
                                else
                                {
                                    content.SetShowBillingDetailsListener(null);
                                }

                                if (j == (model.BillingHistoryDataList.Count - 1))
                                {
                                    content.ShowSeparator(false);
                                }
                                else
                                {
                                    content.ShowSeparator(true);
                                }

                                itemisedBillingGroupComponent.AddContent(content);
                            }
                        }
                    }
                    itemisedBillingList.AddView(itemisedBillingGroupComponent);
                }
            }
            else
            {
                emptyItemisedBillingList.Visibility = ViewStates.Visible;
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
            itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
            itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
            itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));
            itemisedBillingInfoDate.Visibility = ViewStates.Gone;
            itemisedBillingHeaderImage.SetImageResource(imageResource);


            itemisedBillingListShimmer.Visibility = ViewStates.Gone;
            itemisedBillingList.Visibility = ViewStates.Gone;
            emptyItemisedBillingList.Visibility = ViewStates.Visible;
            EnableActionButtons(false);
        }

        private void EnableActionButtons(bool isEnable)
        {
            billFilterIcon.Clickable = isEnable;
            btnViewDetails.Enabled = isEnable;
            btnPayBill.Enabled = isEnable;
            if (isEnable)
            {
                btnViewDetails.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.freshGreen)));
                btnViewDetails.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_button_background);
                btnPayBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.green_button_background);
            }
            else
            {
                btnViewDetails.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.silverChalice)));
                btnViewDetails.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_button_background_disabled);
                btnPayBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);
            }
        }

        private void UpdateFilterItems(List<AccountBillPayFilter> billPayFilters)
        {
            itemFilterList = new List<Item>();
            billPayFilters.ForEach(filter => {
                Item item = new Item();
                item.title = filter.Text;
                item.type = filter.Type;
                itemFilterList.Add(item);
            });
            itemFilterList[0].selected = true;
        }

        public void PopulateBillingHistoryList(List<AccountBillPayHistoryModel> billingHistoryModelList, List<AccountBillPayFilter> billPayFilters)
        {
            itemisedBillingListShimmer.Visibility = ViewStates.Gone;

            if (billingHistoryModelList.Count > 0)
            {
                UpdateFilterItems(billPayFilters);
                selectedBillingHistoryModelList = new List<AccountBillPayHistoryModel>();
                selectedBillingHistoryModelList = billingHistoryModelList;
                emptyItemisedBillingList.Visibility = ViewStates.Gone;
                itemisedBillingList.Visibility = ViewStates.Visible;
                billFilterIcon.Enabled = true;
                EnableActionButtons(true);
                RenderBillingHistoryList(null);
            }
            else
            {
                emptyItemisedBillingList.Visibility = ViewStates.Visible;
                itemisedBillingList.Visibility = ViewStates.Gone;
            }
        }

        public void PopulateAccountCharge(List<AccountChargeModel> accountChargesModelList)
        {
            itemisedBillingInfoShimmer.Visibility = ViewStates.Gone;
            itemisedBillingInfoContainer.Visibility = ViewStates.Visible;
            itemisedBillingCTAContainer.Visibility = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId) ? ViewStates.Gone : ViewStates.Visible;

            selectedAccountChargesModelList = accountChargesModelList.GetRange(0, accountChargesModelList.Count);
            AccountChargeModel accountChargeModel = accountChargesModelList[0];
            int imageResource = Resource.Drawable.bill_no_outstanding_banner;

            itemisedBillingInfoAmount.Text = accountChargeModel.AmountDue.ToString("#,##0.00");
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
                itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));
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

                    itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));
                }
                else
                {
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("paidExtra");

                    itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#20bd4c"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#20bd4c"));
                    itemisedBillingInfoDate.Visibility = ViewStates.Gone;
                }
                itemisedBillingInfoAmount.Text = (Math.Abs(accountChargeModel.AmountDue)).ToString("#,##0.00");
            }
            else if (accountChargeModel.IsNeedPay)
            {
                if (isREAccount)
                {
                    imageResource = Resource.Drawable.bill_paid_extra_re_banner;
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("beenPaidExtra");

                    itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#20bd4c"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#20bd4c"));
                }
                else
                {
                    imageResource = Resource.Drawable.bill_need_to_pay_banner;
                    itemisedBillingInfoNote.Text = GetLabelByLanguage("needToPay");
                    itemisedBillingInfoDate.Text = GetLabelByLanguage("by") + " " + dateFormatter.Format(dateParser.Parse(accountChargeModel.DueDate));
                    itemisedBillingInfoDate.Visibility = ViewStates.Visible;

                    itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
                    itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));
                }
                itemisedBillingInfoAmount.Text = (Math.Abs(accountChargeModel.AmountDue)).ToString("#,##0.00");
            }
            EnableActionButtons(true);
            itemisedBillingHeaderImage.SetImageResource(imageResource);
        }

        public void ShowBillPDFPage(AccountBillPayHistoryModel.BillingHistoryData billHistoryData)
        {
            BillHistoryV5 selectedBill = new BillHistoryV5();
            selectedBill.DtBill = billPdfDateFormatter.Format((billPdfDateParser.Parse(billHistoryData.PaidVia)));
            selectedBill.NrBill = billHistoryData.DetailedInfoNumber;
            Intent viewBill = new Intent(Activity, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
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

        public void ShowUnavailableBillContent(bool isShowRefresh)
        {
            billingHistoryDetailsContent.Visibility = ViewStates.Gone;
            unavailableBillContainer.Visibility = ViewStates.Visible;
            //If not refresh bill, show downtime
            if (isShowRefresh)
            {
                unavailableBillBannerImg.SetImageResource(Resource.Drawable.bg_application_status);
                unavailableBillMsg.TextFormatted = GetFormattedText(GetLabelCommonByLanguage("refreshDescription"));
                btnRefresh.Visibility = ViewStates.Visible;
            }
            else
            {
                unavailableBillBannerImg.SetImageResource(Resource.Drawable.downtime_banner);
                unavailableBillMsg.TextFormatted = GetFormattedText(GetLabelByLanguage("bcrmDownMessage"));
                btnRefresh.Visibility = ViewStates.Gone;
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
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            billFilterMenuItem = menu.FindItem(Resource.Id.action_notification);
            billFilterMenuItem.SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.bill_screen_filter_icon));
            billFilterMenuItem.SetVisible(false);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override string GetPageId()
        {
            return "";
        }
    }
}
