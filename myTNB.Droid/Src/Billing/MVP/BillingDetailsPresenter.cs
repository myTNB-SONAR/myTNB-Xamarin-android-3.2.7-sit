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
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Billing;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.Parser;
using myTNB_Android.Src.Base;
using System.Net;
using myTNB_Android.Src.Base.Models;

namespace myTNB_Android.Src.Billing.MVP
{
    public class BillingDetailsPresenter : BillingDetailsContract.IPresenter
    {
        BillingDetailsContract.IView mView;
        BillingApiImpl billingApi;

        public BillingDetailsPresenter(BillingDetailsContract.IView view)
        {
            mView = view;
            billingApi = new BillingApiImpl();
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

        public async void ShowBillDetails(AccountData selectedAccount, bool isCheckPendingNeeded)
        {
            try
            {
                this.mView.ShowProgressDialog();

                List<string> accountList = new List<string>();
                accountList.Add(selectedAccount.AccountNum);

                if (isCheckPendingNeeded)
                {
                    try
                    {
                        CancellationTokenSource cts = new CancellationTokenSource();
                        ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                        var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                        var paymentStatusApi = RestService.For<IPaymentStatusApi>(httpClient);
#else
                var paymentStatusApi = RestService.For<IPaymentStatusApi>(Constants.SERVER_URL.END_POINT);
#endif

                        UserInterface currentUsrInf = new UserInterface()
                        {
                            eid = UserEntity.GetActive().Email,
                            sspuid = UserEntity.GetActive().UserID,
                            did = UserSessions.GetDeviceId(),
                            ft = FirebaseTokenEntity.GetLatest().FBToken,
                            lang = LanguageUtil.GetAppLanguage().ToUpper(),
                            sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                            sec_auth_k2 = "",
                            ses_param1 = "",
                            ses_param2 = ""
                        };

                        DeviceInterface currentDvdInf = new DeviceInterface()
                        {
                            DeviceId = UserSessions.GetDeviceId(),
                            AppVersion = DeviceIdUtils.GetAppVersionName(),
                            OsType = Constants.DEVICE_PLATFORM,
                            OsVersion = DeviceIdUtils.GetAndroidVersion(),
                            DeviceDesc = LanguageUtil.GetAppLanguage().ToUpper(),
                            VersionCode = ""
                        };

                        CheckPendingPaymentsResponse paymentStatusResponse = await paymentStatusApi.GetCheckPendingPayments(new CheckPendingPaymentRequest()
                        {
                            AccountList = accountList,
                            usrInf = currentUsrInf,
                            deviceInf = currentDvdInf
                        }, cts.Token);

                        if (paymentStatusResponse != null && paymentStatusResponse.Data != null && paymentStatusResponse.Data.ErrorCode == "7200")
                        {
                            if (paymentStatusResponse.Data.Data != null && paymentStatusResponse.Data.Data.Count > 0)
                            {
                                for (int j = 0; j < paymentStatusResponse.Data.Data.Count; j++)
                                {
                                    if (paymentStatusResponse.Data.Data[j].HasPendingPayment)
                                    {
                                        this.mView.OnUpdatePendingPayment(true);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (System.OperationCanceledException e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                    catch (ApiException apiException)
                    {
                        Utility.LoggingNonFatalError(apiException);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

                List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
                AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                    accountList,
                    selectedAccount.IsOwner
                    );
                AccountChargesResponse accountChargeseResponse = await billingApi.GetAccountsCharges<AccountChargesResponse>(accountChargeseRequest);
                this.mView.HideProgressDialog();
                if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "7200")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    accountChargeModelList = BillingResponseParser.GetAccountCharges(accountChargeseResponse.Data.ResponseData.AccountCharges);
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargeseResponse.Data.ResponseData.MandatoryChargesPopUpDetails));
                    this.mView.ShowBillDetails(accountChargeModelList);
                }
                else if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "8400")
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.DisplayMessage))
                    {
                        contentText = accountChargeseResponse.Data.DisplayMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Data.RefreshBtnText;
                    }

                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);

                    this.mView.ShowBillDetailsError(false, btnText, contentText);
                }
                else
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshMessage))
                    {
                        contentText = accountChargeseResponse.Data.RefreshMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Data.RefreshBtnText;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null)
                    {
                        Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    }

                    this.mView.ShowBillDetailsError(true, btnText, contentText);
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillDetailsError(true, "", "");
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillDetailsError(true, "", "");
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillDetailsError(true, "", "");
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
