
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Util;
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
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
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

        [BindView(Resource.Id.refreshContainer)]
        ViewGroup refreshContent;

        [BindView(Resource.Id.itemisedBillingHeaderImage)]
        ImageView itemisedBillingHeaderImage;

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

        [BindView(Resource.Id.btnViewDetails)]
        Button btnViewDetails;

        [BindView(Resource.Id.btnPayBill)]
        Button btnPayBill;

        [BindView(Resource.Id.itemisedBillingCTAContainer)]
        LinearLayout itemisedBillingCTAContainer;

        [BindView(Resource.Id.itemisedBillingList)]
        LinearLayout itemisedBillingList;

        [BindView(Resource.Id.accountSelection)]
        TextView accountSelection;

        [BindView(Resource.Id.itemisedBillingInfoShimmer)]
        ShimmerFrameLayout itemisedBillingInfoShimmer;

        [BindView(Resource.Id.itemisedBillingListShimmer)]
        LinearLayout itemisedBillingListShimmer;

        [BindView(Resource.Id.itemisedBillingInfoContainer)]
        LinearLayout itemisedBillingInfoContainer;

        ItemisedBillingMenuPresenter mPresenter;
        AccountData mSelectedAccountData;

        List<AccountChargeModel> selectedAccountChargesModelList;
        List<Item> itemFilterList = new List<Item>();
        List<ItemisedBillingHistoryModel> selectedBillingHistoryModelList;

        SimpleDateFormat dateParser = new SimpleDateFormat("yyyyMMdd");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");

        SimpleDateFormat billPdfDateParser = new SimpleDateFormat("dd MMM yyyy");
        SimpleDateFormat billPdfDateFormatter = new SimpleDateFormat("dd/MM/yyyy");


        const string SELECTED_ACCOUNT_KEY = "SELECTED_ACCOUNT";

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

        [OnClick(Resource.Id.btnViewDetails)]
        void OnViewDetails(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(Activity, typeof(BillingDetailsActivity));
            intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(mSelectedAccountData));
            intent.PutExtra("BILL_DETAILS",JsonConvert.SerializeObject(selectedAccountChargesModelList));
            StartActivity(intent);
        }

        [OnClick(Resource.Id.btnPayBill)]
        void OnPayment(object sender, EventArgs eventArgs)
        {
            Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            StartActivityForResult(payment_activity, DashboardHomeActivity.PAYMENT_RESULT_CODE);
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

        [OnClick(Resource.Id.bill_filter_icon)]
        void OnFilterBillHistory(object sender, EventArgs eventArgs)
        {
            try
            {
                Intent newIntent = new Intent(this.Activity,typeof(FilterBillHistoryActivity));
                newIntent.PutExtra("ITEM_LIST", JsonConvert.SerializeObject(itemFilterList));
                StartActivityForResult(newIntent, 12345);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnRefresh)]
        void OnButtonRefresh(object sender, EventArgs eventArgs)
        {
            try
            {
                ShowRefreshPage(false);
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
            itemisedBillingInfoShimmer = view.FindViewById<ShimmerFrameLayout>(Resource.Id.itemisedBillingInfoShimmer);
            itemisedBillingInfoShimmer.SetShimmer(ShimmerUtils.ShimmerBuilderConfig().Build());
            itemisedBillingInfoShimmer.StartShimmer();

            base.OnViewCreated(view, savedInstanceState);
            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.bg_smr);


            TextViewUtils.SetMuseoSans500Typeface(accountSelection, itemisedBillingInfoNote,
                btnViewDetails, btnPayBill, itemisedBillingInfoAmountCurrency, myBillHistoryTitle);
            TextViewUtils.SetMuseoSans300Typeface(itemisedBillingInfoDate, itemisedBillingInfoAmount);
            RenderUI();

            mPresenter.GetBillingHistoryDetails(mSelectedAccountData.AccountNum, mSelectedAccountData.IsOwner, (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");
        }

        public void RenderUI()
        {
            accountSelection.Text = mSelectedAccountData.AccountNickName;
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

        public void AddFilterItem(string itemTitle)
        {
            if (itemFilterList.Count == 0)
            {
                Item allFilterItem = new Item();
                allFilterItem.title = "All";
                allFilterItem.selected = true;
                itemFilterList.Add(allFilterItem);
            }

            int foundIndex = itemFilterList.FindIndex(item =>
            {
                return item.title == itemTitle;
            });

            if (foundIndex == -1)
            {
                Item newItem = new Item();
                newItem.title = itemTitle;
                newItem.selected = false;
                itemFilterList.Add(newItem);
            }
        }

        public void UpdateBillingHistory(string filterItemString)
        {
            Item selectedFilter = JsonConvert.DeserializeObject<Item>(filterItemString);
            itemFilterList.ForEach(filterItem =>
            {
                filterItem.selected = (filterItem.title == selectedFilter.title) ? true : false;
            });
            RenderBillingHistoryList(true, selectedFilter.title);
        }

        private void RenderBillingHistoryList(bool isUpdate, string historyType)
        {
            itemisedBillingList.RemoveAllViews();
            ItemisedBillingGroupComponent itemisedBillingGroupComponent;
            List<ItemisedBillingHistoryModel> filteredBillingList = new List<ItemisedBillingHistoryModel>();
            if (historyType == "All")
            {
                filteredBillingList.AddRange(selectedBillingHistoryModelList);
            }
            else
            {
                selectedBillingHistoryModelList.ForEach(billingHistory =>
                {
                    int foundIndex = billingHistory.BillingHistoryDataList.FindIndex(data =>
                    {
                        return data.HistoryTypeText == historyType;
                    });

                    if (foundIndex != -1)
                    {
                        filteredBillingList.Add(billingHistory);
                    }
                });
            }

            for (int i = 0; i < filteredBillingList.Count; i++)
            {
                itemisedBillingGroupComponent = new ItemisedBillingGroupComponent(Activity);
                ItemisedBillingGroupContentComponent content;

                ItemisedBillingHistoryModel model = filteredBillingList[i];
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
                    ItemisedBillingHistoryModel.BillingHistoryData data = model.BillingHistoryDataList[j];
                    //Rendering All History Type
                    if (historyType == "All")
                    {
                        content = new ItemisedBillingGroupContentComponent(Activity);
                        if (!isUpdate)
                        {
                            AddFilterItem(data.HistoryTypeText);
                        }
                        content.IsPayment(data.HistoryType.ToUpper() == "PAYMENT");
                        content.SetDateHistoryType(data.DateAndHistoryType);
                        content.SetPaidVia(data.PaidVia);
                        content.SetAmount(data.Amount);
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
                        if (historyType == data.HistoryTypeText)
                        {
                            content = new ItemisedBillingGroupContentComponent(Activity);
                            if (!isUpdate)
                            {
                                AddFilterItem(data.HistoryType.ToUpper());
                            }
                            content.IsPayment(data.HistoryType.ToUpper() == "PAYMENT");
                            content.SetDateHistoryType(data.DateAndHistoryType);
                            content.SetPaidVia(data.PaidVia);
                            content.SetAmount(data.Amount);
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

        public void PopulateBillingHistoryList(List<ItemisedBillingHistoryModel> billingHistoryModelList)
        {
            
            itemisedBillingList.Visibility = ViewStates.Visible;
            itemisedBillingListShimmer.Visibility = ViewStates.Gone;

            selectedBillingHistoryModelList = new List<ItemisedBillingHistoryModel>();
            selectedBillingHistoryModelList = billingHistoryModelList;
            itemFilterList = new List<Item>();
            RenderBillingHistoryList(false,"All");
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
            if (accountChargeModel.IsCleared)
            {
                itemisedBillingInfoNote.Text = "I’ve cleared all bills";
                itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));

                itemisedBillingInfoDate.Visibility = ViewStates.Gone;
            }
            else if (accountChargeModel.IsPaidExtra)
            {
                itemisedBillingInfoNote.Text = "I’ve paid extra";
                itemisedBillingInfoAmount.Text = (Math.Abs(accountChargeModel.AmountDue)).ToString("#,##0.00");
                itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#20bd4c"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#20bd4c"));

                itemisedBillingInfoDate.Visibility = ViewStates.Gone;
            }
            else if (accountChargeModel.IsNeedPay)
            {
                imageResource = Resource.Drawable.bill_need_to_pay_banner;
                itemisedBillingInfoNote.Text = "I need to pay";
                itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));

                itemisedBillingInfoDate.Visibility = ViewStates.Visible;
                accountChargeModel.DueDate = dateFormatter.Format(dateParser.Parse(accountChargeModel.DueDate));
                itemisedBillingInfoDate.Text = "by " + accountChargeModel.DueDate;
            }

            itemisedBillingHeaderImage.SetImageResource(imageResource);
        }

        public void ShowBillPDFPage(ItemisedBillingHistoryModel.BillingHistoryData billHistoryData)
        {
            BillHistoryV5 selectedBill = new BillHistoryV5();
            selectedBill.DtBill = billPdfDateFormatter.Format((billPdfDateParser.Parse(billHistoryData.PaidVia)));
            selectedBill.NrBill = billHistoryData.DetailedInfoNumber;
            Intent viewBill = new Intent(Activity, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
            StartActivity(viewBill);
        }

        public void ShowPayPDFPage(ItemisedBillingHistoryModel.BillingHistoryData billHistoryData)
        {
            Intent viewReceipt = new Intent(this.Activity, typeof(ViewReceiptMultiAccountNewDesignActivty));
            viewReceipt.PutExtra("merchantTransId", billHistoryData.DetailedInfoNumber);
            StartActivity(viewReceipt);
        }

        public override ViewGroup GetRootView()
        {
            return rootView;
        }

        public void ShowRefreshPage(bool show)
        {
            billingHistoryDetailsContent.Visibility = show ? ViewStates.Gone : ViewStates.Visible;
            refreshContent.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }

        class OnShowBillingDetailsListener : Java.Lang.Object, View.IOnClickListener
        {
            ItemisedBillingMenuFragment mView;
            ItemisedBillingHistoryModel.BillingHistoryData mBillHistoryData;

            public OnShowBillingDetailsListener(ItemisedBillingMenuFragment view, ItemisedBillingHistoryModel.BillingHistoryData billHistoryData)
            {
                mView = view;
                mBillHistoryData = billHistoryData;
            }
            public void OnClick(View v)
            {
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
}
