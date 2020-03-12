﻿using Android.Text;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ViewBill.Activity
{
    public class ViewBillPresenter : ViewBillContract.IUserActionsListener
    {
        private ViewBillContract.IView mView;

        public ViewBillPresenter(ViewBillContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        private async Task OnGetBilling(AccountData selectedAccount)
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
                        this.mView.ShowBillPDF(billsHistoryResponse.GetData()[0]);
                        return;
                    }
                    else
                    {
                        this.mView.ShowViewBillError(billsHistoryResponse.Response.DisplayTitle, billsHistoryResponse.Response.DisplayMessage);
                    }
                }
                else
                {
                    this.mView.ShowBillErrorSnackBar();
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

        public void LoadingBillsHistory(AccountData selectedAccount)
        {
            Task.Run(() =>
            {
                _ = OnGetBilling(selectedAccount);
            });
        }

        public void Start()
        {
            //
        }
    }
}
