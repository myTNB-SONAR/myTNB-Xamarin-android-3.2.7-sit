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
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.Parser;
using myTNB_Android.Src.Base;
using System.Net;
using myTNB_Android.Src.Base.Models;
using Java.Util.Regex;

namespace myTNB_Android.Src.Billing.MVP
{
    public class BillingDetailsPresenter : BillingDetailsContract.IPresenter
    {
        BillingDetailsContract.IView mView;

        public BillingDetailsPresenter(BillingDetailsContract.IView view)
        {
            mView = view;
        }

        private async void LoadingBillsHistory(AccountData selectedAccount)
        {
            bool isViewBillDisable = true;

            try
            {
                var billsHistoryResponse = await ServiceApiImpl.Instance.GetBillHistory(new MyTNBService.Request.GetBillHistoryRequest(selectedAccount.AccountNum, selectedAccount.IsOwner));

                if (billsHistoryResponse.IsSuccessResponse() && billsHistoryResponse.GetData() != null && billsHistoryResponse.GetData().Count > 0)
                {
                    isViewBillDisable = false;
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

            if (isViewBillDisable)
            {
                this.mView.EnableDisableViewBillButtons(false);
            }
            else
            {
                this.mView.EnableDisableViewBillButtons(true);
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

                LoadingBillsHistory(selectedAccount);

                List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
                AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                    accountList,
                    selectedAccount.IsOwner
                    );
                AccountChargesResponse accountChargeseResponse = await ServiceApiImpl.Instance.GetAccountsCharges(accountChargeseRequest);
                this.mView.HideProgressDialog();
                if (accountChargeseResponse.IsSuccessResponse())
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Response.IsPayEnabled);
                    accountChargeModelList = BillingResponseParser.GetAccountCharges(accountChargeseResponse.GetData().AccountCharges);
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargeseResponse.GetData().MandatoryChargesPopUpDetails));
                    this.mView.ShowBillDetails(accountChargeModelList);
                }
                else if (accountChargeseResponse.Response != null && accountChargeseResponse.Response.ErrorCode == "8400")
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Response != null && !string.IsNullOrEmpty(accountChargeseResponse.Response.DisplayMessage))
                    {
                        contentText = accountChargeseResponse.Response.DisplayMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Response != null && !string.IsNullOrEmpty(accountChargeseResponse.Response.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Response.RefreshBtnText;
                    }

                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Response.IsPayEnabled);

                    this.mView.ShowBillDetailsError(false, btnText, contentText);
                }
                else
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Response != null && !string.IsNullOrEmpty(accountChargeseResponse.Response.RefreshMessage))
                    {
                        contentText = accountChargeseResponse.Response.RefreshMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Response != null && !string.IsNullOrEmpty(accountChargeseResponse.Response.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Response.RefreshBtnText;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Response != null)
                    {
                        Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Response.IsPayEnabled);
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
                IsButtonShow = false
            });

            return newList;
        }

        public List<string> ExtractUrls(string text)
        {
            List<string> containedUrls = new List<string>();
            string urlRegex = "\\(?\\b(https://|http://|www[.])[-A-Za-z0-9+&amp;@#/%?=~_()|!:,.;]*[-A-Za-z0-9+&amp;@#/%=~_()|]";
            Pattern pattern = Pattern.Compile(urlRegex);
            Matcher urlMatcher = pattern.Matcher(text);

            try
            {
                while (urlMatcher.Find())
                {
                    string urlStr = urlMatcher.Group();
                    if (urlStr.StartsWith("(") && urlStr.EndsWith(")"))
                    {
                        urlStr = urlStr.Substring(1, urlStr.Length - 1);
                    }

                    if (!containedUrls.Contains(urlStr))
                    {
                        containedUrls.Add(urlStr);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return containedUrls;
        }
    }
}
