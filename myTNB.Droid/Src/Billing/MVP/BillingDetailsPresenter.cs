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
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Billing.MVP
{
    public class BillingDetailsPresenter : BillingDetailsContract.IPresenter
    {
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

            try
            {
                var billsHistoryResponse = await ServiceApiImpl.Instance.GetBillHistory(new MyTNBService.Request.GetBillHistoryRequest(selectedAccount.AccountNum, selectedAccount.IsOwner));

                this.mView.HideProgressDialog();

                if (billsHistoryResponse.IsSuccessResponse())
                {
                    if (billsHistoryResponse.GetData() != null && billsHistoryResponse.GetData().Count > 0)
                    {
                        this.mView.ShowBillPDF(JsonConvert.SerializeObject(billsHistoryResponse.GetData()[0]));
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
