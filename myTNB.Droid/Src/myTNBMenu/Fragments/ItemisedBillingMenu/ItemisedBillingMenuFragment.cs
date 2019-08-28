
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
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu
{
    public class ItemisedBillingMenuFragment : BaseFragment, ItemisedBillingContract.IView
    {
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

        ItemisedBillingMenuPresenter mPresenter;
        AccountData mSelectedAccountData;

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
            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.bg_smr);


            TextViewUtils.SetMuseoSans500Typeface(accountSelection, itemisedBillingInfoNote,
                btnViewDetails, btnPayBill, itemisedBillingInfoAmountCurrency, myBillHistoryTitle);
            TextViewUtils.SetMuseoSans300Typeface(itemisedBillingInfoDate, itemisedBillingInfoAmount);
            RenderUI();

            mPresenter.GetAccountsCharges(mSelectedAccountData.AccountNum,
                mSelectedAccountData.IsOwner);
            mPresenter.GetAccountBillPayHistory(mSelectedAccountData.AccountNum,
                mSelectedAccountData.IsOwner,
                (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");
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

            itemisedBillingCTAContainer.Visibility = mPresenter.IsREAccount(mSelectedAccountData.AccountCategoryId) ? ViewStates.Gone : ViewStates.Visible;
        }

        public void PopulateBillingHistoryList(List<ItemisedBillingHistoryModel> billingHistoryModelList)
        {
            ItemisedBillingGroupComponent itemisedBillingGroupComponent;
            for (int i = 0; i < billingHistoryModelList.Count; i++)
            {
                itemisedBillingGroupComponent = new ItemisedBillingGroupComponent(Activity);
                ItemisedBillingGroupContentComponent content;

                ItemisedBillingHistoryModel model = billingHistoryModelList[i];
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
                    content = new ItemisedBillingGroupContentComponent(Activity);
                    content.SetDateHistoryType(data.DateAndHistoryType);
                    content.SetPaidVia(data.PaidVia);
                    content.SetAmount(data.Amount);
                    if (data.DetailedInfoNumber != "")
                    {
                        content.SetShowBillingDetailsListener(new OnShowBillingDetailsListener(this));
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

                itemisedBillingList.AddView(itemisedBillingGroupComponent);
            }
        }

        public void GetAccountBillPayHistory()
        {
            mPresenter.GetAccountBillPayHistory(mSelectedAccountData.AccountNum,
                true,
                (mSelectedAccountData.AccountCategoryId != "2") ? "UTIL" : "RE");
        }

        public void PopulateAccountCharge(List<AccountChargeModel> accountChargesModelList)
        {
            AccountChargeModel accountChargeModel = accountChargesModelList[0];
            int imageResource = Resource.Drawable.bill_no_outstanding_banner;

            mPresenter.EvaluateAccountCharge(accountChargeModel);

            itemisedBillingInfoAmount.Text = accountChargeModel.AmountDue.ToString();
            if (mPresenter.isCleared)
            {
                itemisedBillingInfoNote.Text = "I’ve cleared all bills";
                itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));

                itemisedBillingInfoDate.Visibility = ViewStates.Gone;
            }
            else if (mPresenter.isOverPaid)
            {
                itemisedBillingInfoNote.Text = "I’ve paid extra";
                itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#20bd4c"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#20bd4c"));

                itemisedBillingInfoDate.Visibility = ViewStates.Gone;
            }
            else if (mPresenter.isNeedToPay)
            {
                imageResource = Resource.Drawable.bill_need_to_pay_banner;
                itemisedBillingInfoNote.Text = "I need to pay";
                itemisedBillingInfoNote.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmount.SetTextColor(Color.ParseColor("#49494a"));
                itemisedBillingInfoAmountCurrency.SetTextColor(Color.ParseColor("#49494a"));

                itemisedBillingInfoDate.Visibility = ViewStates.Visible;
                itemisedBillingInfoDate.Text = "by " + accountChargeModel.DueDate;
            }

            itemisedBillingHeaderImage.SetImageResource(imageResource);
        }

        class OnShowBillingDetailsListener : Java.Lang.Object, View.IOnClickListener
        {
            ItemisedBillingMenuFragment mView;
            public OnShowBillingDetailsListener(ItemisedBillingMenuFragment view)
            {
                mView = view;
            }
            public void OnClick(View v)
            {
                //throw new NotImplementedException();
                //mView.GetAccountBillPayHistory();
            }
        }
    }
}
