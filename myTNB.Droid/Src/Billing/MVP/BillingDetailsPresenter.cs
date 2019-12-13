using System;
using System.Net.Http;
using System.Threading;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.Utils;
using Refit;
using System.Linq;
using System.Collections.Generic;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.Billing.MVP
{
    public class BillingDetailsPresenter : BillingDetailsContract.IPresenter
    {
        CancellationTokenSource cts;
        BillingDetailsContract.IView mView;

        public BillingDetailsPresenter(BillingDetailsContract.IView view)
        {
            mView = view;
        }

        public void GetBillHistory(AccountData selectedAccount)
        {
            LoadingBillsHistory(selectedAccount);
        }

        private async void LoadingBillsHistory(AccountData selectedAccount)
        {
            this.mView.ShowProgressDialog();
            cts = new CancellationTokenSource();
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new System.Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IBillsPaymentHistoryApi>(httpClient);
#else
            var api = RestService.For<IBillsPaymentHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var billsHistoryResponseApi = await api.GetBillHistoryV5(new BillHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    AccountNum = selectedAccount.AccountNum,
                    IsOwner = selectedAccount.IsOwner,
                    Email = UserEntity.GetActive().Email
                }, cts.Token);

                var billsHistoryResponseV5 = billsHistoryResponseApi;

                this.mView.HideProgressDialog();

                if (billsHistoryResponseV5 != null && billsHistoryResponseV5.Data != null)
                {
                    if (!billsHistoryResponseV5.Data.IsError && !string.IsNullOrEmpty(billsHistoryResponseV5.Data.Status)
                        && billsHistoryResponseV5.Data.Status.Equals("success"))
                    {
                        if (billsHistoryResponseV5.Data.BillHistory != null && billsHistoryResponseV5.Data.BillHistory.Count() > 0)
                        {
                            this.mView.ShowBillPDF(billsHistoryResponseV5.Data.BillHistory[0]);
                            return;
                        }
                        else
                        {
                            this.mView.ShowBillPDF();
                        }
                    }
                    else
                    {
                        this.mView.ShowBillPDF();
                    }
                }
                else
                {
                    this.mView.ShowBillPDF();
                }

            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillErrorSnackBar();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillErrorSnackBar();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillErrorSnackBar();
                Utility.LoggingNonFatalError(e);
            }
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("BillDetails", "tutorialTitle"),
                ContentMessage = Utility.GetLocalizedLabel("BillDetails", "tutorialDesc"),
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = true
            });

            return newList;
        }
    }
}
