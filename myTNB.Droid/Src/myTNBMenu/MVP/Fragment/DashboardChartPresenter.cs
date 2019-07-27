﻿using Android.Util;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardChartPresenter : DashboardChartContract.IUserActionsListener
    {
        private DashboardChartContract.IView mView;
        CancellationTokenSource cts;

        public DashboardChartPresenter(DashboardChartContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnArrowBackClick()
        {
            try
            {
                if (this.mView.GetCurrentParentIndex() == (this.mView.GetMaxParentIndex() - 1))
                {
                    this.mView.EnableLeftArrow(false);
                }
                else
                {

                    int newIndex = (this.mView.GetCurrentParentIndex() + 1) % this.mView.GetMaxParentIndex();
                    this.mView.SetCurrentParentIndex(newIndex);
                    if (this.mView.GetCurrentParentIndex() == (this.mView.GetMaxParentIndex() - 1))
                    {
                        this.mView.EnableLeftArrow(false);
                    }
                    else
                    {
                        this.mView.EnableLeftArrow(true);
                    }
                }

                if (this.mView.GetCurrentParentIndex() == 0)
                {
                    this.mView.EnableRightArrow(false);
                }
                else
                {
                    this.mView.EnableRightArrow(true);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnArrowForwardClick()
        {
            try
            {
                if (this.mView.GetCurrentParentIndex() == 0)
                {
                    this.mView.EnableRightArrow(false);
                }
                else
                {

                    int newIndex = this.mView.GetCurrentParentIndex() - 1;
                    this.mView.SetCurrentParentIndex(newIndex);

                    if (this.mView.GetCurrentParentIndex() == 0)
                    {
                        this.mView.EnableRightArrow(false);
                    }
                    else
                    {
                        this.mView.EnableRightArrow(true);
                    }
                }

                if (this.mView.GetCurrentParentIndex() == (this.mView.GetMaxParentIndex() - 1))
                {
                    this.mView.EnableLeftArrow(false);
                }
                else
                {
                    this.mView.EnableLeftArrow(true);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnByDay()
        {
            try
            {
                if (!this.mView.HasNoInternet())
                {
                    this.mView.SetCurrentParentIndex(0);
                    if (!this.mView.IsByDayEmpty())
                    {
                        this.mView.EnableLeftArrow(true);
                        this.mView.EnableRightArrow(false);
                        this.mView.ShowByDay();
                    }
                    else
                    {
                        this.mView.EnableLeftArrow(false);
                        this.mView.ShowNotAvailableDayData();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnByMonth()
        {
            try
            {
                if (!this.mView.HasNoInternet())
                {
                    this.mView.EnableLeftArrow(false);
                    this.mView.EnableRightArrow(false);
                    this.mView.ShowByMonth();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnLearnMore()
        {
            WeblinkEntity weblinkEntity = WeblinkEntity.GetByCode("SMTR");
            if (weblinkEntity != null)
            {
                this.mView.ShowLearnMore(Weblink.Copy(weblinkEntity));
            }
        }

        public async void OnLoadAmount(string accountNum)
        {
            try
            {
                cts = new CancellationTokenSource();
                if (mView.IsActive())
                {
                    this.mView.ShowAmountProgress();
                }
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
    #if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var amountDueApi = RestService.For<IAmountDueApi>(httpClient);

    #else
                var amountDueApi = RestService.For<IAmountDueApi>(Constants.SERVER_URL.END_POINT);

    #endif 

                var amountDueResponse = await amountDueApi.GetAccountDueAmount(new Requests.AccountDueAmountRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    AccNum = accountNum

                }, cts.Token);


                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                if (amountDueResponse != null && amountDueResponse.Data != null && amountDueResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
                {
                    this.mView.ShowNoInternetWithWord(amountDueResponse.Data.RefreshMessage, amountDueResponse.Data.RefreshBtnText);
                }
                else if (!amountDueResponse.Data.IsError)
                {
                    this.mView.ShowAmountDue(amountDueResponse.Data.Data);
                }
                else
                {
                    this.mView.ShowNoInternetWithWord(null, null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.ShowNoInternetWithWord(null, null);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.ShowNoInternetWithWord(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.ShowNoInternetWithWord(null, null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnNotification()
        {
            this.mView.ShowNotification();
        }

        public void OnPay()
        {
            this.mView.ShowPayment();
        }

        public void OnTapRefresh()
        {
            this.mView.ShowTapRefresh();
        }

        public void OnViewBill(AccountData selectedAccount)
        {
            LoadingBillsHistory(selectedAccount);
        }


        private async void LoadingBillsHistory(AccountData selectedAccount)
        {
            if (this.mView.IsActive())
            {
                this.mView.ShowAmountProgress();
            }
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

                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }

                if (billsHistoryResponseV5 != null && billsHistoryResponseV5.Data != null)
                {
                    if (!billsHistoryResponseV5.Data.IsError && !string.IsNullOrEmpty(billsHistoryResponseV5.Data.Status)
                        && billsHistoryResponseV5.Data.Status.Equals("success"))
                    {
                        if (billsHistoryResponseV5.Data.BillHistory != null && billsHistoryResponseV5.Data.BillHistory.Count() > 0)
                        {
                            this.mView.ShowViewBill(billsHistoryResponseV5.Data.BillHistory[0]);
                            return;
                        }
                        else
                        {
                            this.mView.ShowViewBill();
                        }
                    }
                    else
                    {
                        this.mView.ShowViewBill();
                    }
                }
                else
                {
                    this.mView.ShowViewBill();
                }

            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                    this.mView.ShowLoadBillRetryOptions();
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                Log.Debug("BillPayment Presenter", "Stack " + apiException.StackTrace);
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                    this.mView.ShowLoadBillRetryOptions();
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                Log.Debug("BillPayment Presenter", "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                    this.mView.ShowLoadBillRetryOptions();
                }
                Utility.LoggingNonFatalError(e);
            }
        }


        public void Start()
        {
            try
            {
                // THIS SHOULD BE THE CHART DISPLAY ANIMATION
                DownTimeEntity bcrmEntity = new DownTimeEntity();
                bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (this.mView.HasNoInternet() || bcrmEntity.IsDown)
                {
                    this.mView.EnableLeftArrow(false);
                    this.mView.EnableRightArrow(false);
                    this.mView.ShowNoInternet();


                }
                else
                {

                    OnByMonth();

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void GetAccountStatus(string accountNum)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowAmountProgress();
            }

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var installDetailsApi = RestService.For<IGetInstallationDetailsApi>(httpClient);

#else
            var installDetailsApi = RestService.For<IGetInstallationDetailsApi>(Constants.SERVER_URL.END_POINT);

#endif 

            try
            {
                var installDetailsResponse = await installDetailsApi.GetInstallationDetails(new Requests.GetInstallationDetailsRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    AccNum = accountNum
                }, cts.Token);


                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }

                if (!installDetailsResponse.Data.IsError)
                {
                    if (installDetailsResponse.Data.Data.DisconnectionStatus == "Available")
                    {
                        this.mView.InitiateSSMRStatus();
                    }
                    else
                    {
                        this.mView.ShowAccountStatus(installDetailsResponse.Data.Data);
                    }
                }
                else
                {
                    this.mView.InitiateSSMRStatus();
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.InitiateSSMRStatus();
                this.mView.ShowDisconnectionRetrySnakebar();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.InitiateSSMRStatus();
                this.mView.ShowDisconnectionRetrySnakebar();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.InitiateSSMRStatus();
                this.mView.ShowDisconnectionRetrySnakebar();
                Utility.LoggingNonFatalError(e);
            }

        }

        public async void GetSSMRAccountStatus(string accountNum)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowAmountProgress();
            }

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            UserInterface currentUsrInf = new UserInterface()
            {
                eid = UserEntity.GetActive().Email,
                sspuid = UserEntity.GetActive().UserID,
                did = this.mView.GetDeviceId(),
                ft = FirebaseTokenEntity.GetLatest().FBToken,
                lang = Constants.DEFAULT_LANG.ToUpper(),
                sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                sec_auth_k2 = "",
                ses_param1 = "",
                ses_param2 = ""
            };
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var ssmrAccountAPI = RestService.For<ISMRAccountActivityInfoApi>(httpClient);

#else
            var ssmrAccountAPI = RestService.For<ISMRAccountActivityInfoApi>(Constants.SERVER_URL.END_POINT);
#endif 

            try
            {
                SMRActivityInfoResponse SMRAccountActivityInfoResponse = await ssmrAccountAPI.GetSMRAccountActivityInfo(new Requests.SMRAccountActivityInfoRequest()
                {
                    AccountNumber = accountNum,
                    IsOwnedAccount = "true",
                    userInterface = currentUsrInf
                }, cts.Token);


                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }

                if (SMRAccountActivityInfoResponse.Response.ErrorCode == "7200")
                {
                    this.mView.ShowSSMRDashboardView(SMRAccountActivityInfoResponse);
                }
                else
                {
                    this.mView.HideSSMRDashboardView();
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.HideSSMRDashboardView();
                this.mView.ShowSMRRetrySnakebar();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.HideSSMRDashboardView();
                this.mView.ShowSMRRetrySnakebar();
                Utility.LoggingNonFatalError(e);
            }
        }


    }
}