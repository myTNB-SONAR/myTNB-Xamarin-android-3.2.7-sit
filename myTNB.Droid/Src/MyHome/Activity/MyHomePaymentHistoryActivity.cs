
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.CompoundView;
using myTNB.Android.Src.MyHome.MVP;
using myTNB.Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB.Android.Src.MyTNBService.Model;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.ViewReceipt.Activity;
using myTNB.Mobile.Constants;

namespace myTNB.Android.Src.MyHome.Activity
{
	[Activity(Label = "MyHomePaymentHistoryActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class MyHomePaymentHistoryActivity : BaseActivityCustom, MyHomePaymentHistoryContract.IView
    {
        [BindView(Resource.Id.myHomePaymentHistoryTitle)]
        TextView myHomePaymentHistoryTitle;

        [BindView(Resource.Id.myHomePaymentHistoryList)]
        LinearLayout myHomePaymentHistoryList;

        [BindView(Resource.Id.myHomePaymentHistoryListShimmer)]
        LinearLayout myHomePaymentHistoryListShimmer;

        [BindView(Resource.Id.myHomePaymentHistoryEmptyContainer)]
        LinearLayout myHomePaymentHistoryEmptyContainer;

        [BindView(Resource.Id.myHomePaymentHistoryEmptyMessage)]
        TextView myHomePaymentHistoryEmptyMessage;

        [BindView(Resource.Id.myHomePaymentHistoryRefreshContainer)]
        LinearLayout myHomePaymentHistoryRefreshContainer;

        [BindView(Resource.Id.myHomePaymentHistoryRefreshMessage)]
        TextView myHomePaymentHistoryRefreshMessage;

        [BindView(Resource.Id.btnMyHomePaymentHistoryRefresh)]
        Button btnMyHomePaymentHistoryRefresh;

        private MyHomePaymentHistoryContract.IUserActionsListener presenter;

        private DecimalFormat mDecimalFormat = new DecimalFormat("#,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));

        const string PAGE_ID = "PaymentHistory";

        protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

            try
            {
                _ = new MyHomePaymentHistoryPresenter(this, this, this);

                SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

                SetToolBarTitle(GetLabelByLanguage(myHome.PaymentHistory.I18N_PaymentHistoryTitle));
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    this.presenter?.OnInitialize(extras);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.MyHomePaymentHistoryView;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public void SetPresenter(MyHomePaymentHistoryContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void SetUpViews()
        {
            TextViewUtils.SetMuseoSans500Typeface(myHomePaymentHistoryTitle, btnMyHomePaymentHistoryRefresh);
            TextViewUtils.SetMuseoSans300Typeface(myHomePaymentHistoryEmptyMessage, myHomePaymentHistoryRefreshMessage);
            TextViewUtils.SetTextSize16(myHomePaymentHistoryTitle, btnMyHomePaymentHistoryRefresh);
            TextViewUtils.SetTextSize14(myHomePaymentHistoryEmptyMessage);
            TextViewUtils.SetTextSize12(myHomePaymentHistoryRefreshMessage);

            myHomePaymentHistoryTitle.Text = GetLabelByLanguage(myHome.PaymentHistory.I18N_PaymentHistoryListTitle);

            UpdateShimmerLoadingState(true);
            this.presenter.GetAccountBillPayHistory();
        }

        public void UpdateShimmerLoadingState(bool show)
        {
            myHomePaymentHistoryListShimmer.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }

        public void ShowEmptyListWithMessage(string msg)
        {
            myHomePaymentHistoryEmptyContainer.Visibility = ViewStates.Visible;
            myHomePaymentHistoryEmptyMessage.Text = msg;
        }

        public void ShowRefreshStateWithMessage(bool showRefreshButton, string msg, string refreshBtnMsg)
        {
            myHomePaymentHistoryRefreshContainer.Visibility = ViewStates.Visible;
            btnMyHomePaymentHistoryRefresh.Visibility = showRefreshButton ? ViewStates.Visible : ViewStates.Gone;
            btnMyHomePaymentHistoryRefresh.Text = refreshBtnMsg;
            myHomePaymentHistoryRefreshMessage.Text = msg;
        }

        public void PopulatePaymentHistoryList(List<AccountBillPayHistoryModel> billingHistoryModelList)
        {
            myHomePaymentHistoryList.Visibility = ViewStates.Visible;
            myHomePaymentHistoryList.RemoveAllViews();
            ItemisedBillingGroupComponent itemisedBillingGroupComponent;

            for (int i = 0; i < billingHistoryModelList.Count; i++)
            {
                itemisedBillingGroupComponent = new ItemisedBillingGroupComponent(this);
                ItemisedBillingGroupContentComponent content;

                AccountBillPayHistoryModel model = billingHistoryModelList[i];
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

                    content = new ItemisedBillingGroupContentComponent(this);
                    content.IsPayment(true);
                    content.SetDateHistoryType(data.DateAndHistoryType);
                    content.SetPaidVia(data.PaidVia);
                    if (double.Parse(data.Amount, currCult) < 0)
                    {
                        content.SetAmount("- RM " + mDecimalFormat.Format(double.Parse(data.Amount, currCult) * -1), data.IsPaymentPending);
                    }
                    else
                    {
                        content.SetAmount("RM " + mDecimalFormat.Format(double.Parse(data.Amount, currCult)), data.IsPaymentPending);
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
                myHomePaymentHistoryList.AddView(itemisedBillingGroupComponent);
            }
        }

        public void ShowPayPDFPage(AccountBillPayHistoryModel.BillingHistoryData billHistoryData)
        {
            Intent viewReceipt = new Intent(this, typeof(ViewReceiptMultiAccountNewDesignActivty));
            viewReceipt.PutExtra("SELECTED_ACCOUNT_NUMBER", this.presenter.GetContractAccount());
            viewReceipt.PutExtra("DETAILED_INFO_NUMBER", billHistoryData.DetailedInfoNumber);
            viewReceipt.PutExtra("IS_OWNED_ACCOUNT", true);
            viewReceipt.PutExtra("IS_SHOW_ALL_RECEIPT", false);
            StartActivity(viewReceipt);
        }

        class OnShowBillingDetailsListener : Java.Lang.Object, View.IOnClickListener
        {
            MyHomePaymentHistoryActivity mActivity;
            AccountBillPayHistoryModel.BillingHistoryData mBillHistoryData;

            public OnShowBillingDetailsListener(MyHomePaymentHistoryActivity activity, AccountBillPayHistoryModel.BillingHistoryData billHistoryData)
            {
                mActivity = activity;
                mBillHistoryData = billHistoryData;
            }
            public void OnClick(View v)
            {
                if (!mActivity.GetIsClicked())
                {
                    mActivity.SetIsClicked(true);
                    mActivity.ShowPayPDFPage(mBillHistoryData);
                }
            }
        }
    }
}

