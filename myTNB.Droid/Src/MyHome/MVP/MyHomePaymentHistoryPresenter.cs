using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.MyTNBService.Response.AccountBillPayHistoryResponse;

namespace myTNB_Android.Src.MyHome.MVP
{
    public class MyHomePaymentHistoryPresenter : MyHomePaymentHistoryContract.IUserActionsListener
    {
        private readonly MyHomePaymentHistoryContract.IView mView;
        private BaseAppCompatActivity mActivity;
        private Context mContext;

        private string _contractAccount;
        private bool _isOwnedAccount;
        private string _accountType;

        private List<AccountBillPayHistoryModel> _billingHistoryList;

        public MyHomePaymentHistoryPresenter(MyHomePaymentHistoryContract.IView view, BaseAppCompatActivity activity, Context context)
        {
            this.mActivity = activity;
            this.mContext = context;
            this.mView = view;
            this.mView?.SetPresenter(this);
        }

        public void OnInitialize(Bundle bundle)
        {
            _billingHistoryList = new List<AccountBillPayHistoryModel>();

            if (bundle.ContainsKey(MyHomeConstants.PAYMENT_HISTORY_CA))
            {
                _contractAccount = bundle.GetString(MyHomeConstants.PAYMENT_HISTORY_CA);
            }

            if (bundle.ContainsKey(MyHomeConstants.PAYMENT_HISTORY_IS_OWNED))
            {
                _isOwnedAccount = bundle.GetBoolean(MyHomeConstants.PAYMENT_HISTORY_IS_OWNED);
            }

            if (bundle.ContainsKey(MyHomeConstants.PAYMENT_HISTORY_ACCOUNT_TYPE))
            {
                _accountType = bundle.GetString(MyHomeConstants.PAYMENT_HISTORY_ACCOUNT_TYPE);
            }

            this.mView?.SetUpViews();
        }

        public void Start() { }

        public void GetAccountBillPayHistory()
        {
            Task.Run(() =>
            {
                _ = OnGetAccountBillPayHistory();
            });
        }

        private async Task OnGetAccountBillPayHistory()
        {
            AccountBillPayHistoryRequest accountBillPayRequest = new AccountBillPayHistoryRequest(
                    _contractAccount,
                    _isOwnedAccount,
                    _accountType);

            AccountBillPayHistoryResponse accountBillPayResponse = await ServiceApiImpl.Instance.GetAccountBillPayHistory(accountBillPayRequest);

            this.mActivity.RunOnUiThread(() =>
            {
                this.mView.UpdateShimmerLoadingState(false);
            });

            if (accountBillPayResponse.IsSuccessResponse())
            {
                _billingHistoryList = GetBillingHistoryModelList(accountBillPayResponse.GetData().BillPayHistories);
                this.mActivity.RunOnUiThread(() =>
                {
                    if (_billingHistoryList.Count > 0)
                    {
                        this.mView?.PopulatePaymentHistoryList(_billingHistoryList);
                    }
                    else
                    {
                        //STUB
                        this.mView?.ShowEmptyListWithMessage(Utility.GetLocalizedLabel(LanguageConstants.BILLS, LanguageConstants.Bills.EMPTY_PAYMENT_HISTORY));
                    }
                });
            }
            else if (accountBillPayResponse != null
                && accountBillPayResponse.Response != null
                && accountBillPayResponse.Response.ErrorCode == "8400")
            {
                string contentText = Utility.GetLocalizedLabel(LanguageConstants.BILLS, LanguageConstants.Bills.NO_HISTORY_DATA);

                if (accountBillPayResponse.Response.DisplayMessage.IsValid())
                {
                    contentText = accountBillPayResponse.Response.DisplayMessage;
                }

                this.mView.ShowRefreshStateWithMessage(false, contentText, string.Empty);
            }
            else
            {
                //STUB
                string contentText = "";
                string btnText = Utility.GetLocalizedCommonLabel(LanguageConstants.Common.REFRESH_NOW);

                if (accountBillPayResponse != null
                    && accountBillPayResponse.Response != null
                    && accountBillPayResponse.Response.RefreshMessage.IsValid())
                {
                    contentText = accountBillPayResponse.Response.RefreshMessage;
                }

                if (accountBillPayResponse != null
                    && accountBillPayResponse.Response != null
                    && accountBillPayResponse.Response.RefreshBtnText.IsValid())
                {
                    btnText = accountBillPayResponse.Response.RefreshBtnText;
                }

                this.mView.ShowRefreshStateWithMessage(true, contentText, btnText);
            }
        }

        private List<AccountBillPayHistoryModel> GetBillingHistoryModelList(List<BillPayHistory> billPayHistoryList)
        {
            List<AccountBillPayHistoryModel> modelList = new List<AccountBillPayHistoryModel>();
            List<AccountBillPayHistoryModel.BillingHistoryData> dataList;
            AccountBillPayHistoryModel.BillingHistoryData data;
            AccountBillPayHistoryModel model;
            billPayHistoryList.ForEach(history =>
            {
                dataList = new List<AccountBillPayHistoryModel.BillingHistoryData>();
                model = new AccountBillPayHistoryModel();
                model.MonthYear = history.MonthYear;

                history.BillPayHistoryData.ForEach(historyData =>
                {
                    if (historyData.HistoryType.ToUpper() == MyHomeConstants.PAYMENT_HISTORY_PAYMENT)
                    {
                        data = new AccountBillPayHistoryModel.BillingHistoryData();
                        data.HistoryType = historyData.HistoryType;
                        data.DateAndHistoryType = historyData.DateAndHistoryType;
                        data.Amount = historyData.Amount;
                        data.DetailedInfoNumber = historyData.DetailedInfoNumber;
                        data.PaidVia = historyData.PaidVia;
                        data.HistoryTypeText = historyData.HistoryTypeText;
                        data.IsPaymentPending = historyData.IsPaymentPending;
                        dataList.Add(data);
                    }
                });

                if (dataList.Count > 0)
                {
                    model.BillingHistoryDataList = dataList;
                    modelList.Add(model);
                }
            });

            return modelList;
        }

        public string GetContractAccount()
        {
            return _contractAccount;
        }
    }
}

