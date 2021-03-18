using Android.Content;
using Android.Graphics;
using Android.Util;
using Java.Util.Regex;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Parser;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static myTNB_Android.Src.AppLaunch.Models.MasterDataResponse;
using static myTNB_Android.Src.MyTNBService.Response.AccountChargesResponse;
using Java.Util.Regex;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using Android.App;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardChartPresenter : DashboardChartContract.IUserActionsListener
    {
        private DashboardChartContract.IView mView;
        CancellationTokenSource cts;
        ISharedPreferences mPref;
        private bool isSMRReady = false;
        private bool isDashboardReady = false;

        private bool isBillAvailable = true;
        private bool isREFirstBill = false;

        public DashboardChartPresenter(DashboardChartContract.IView mView, ISharedPreferences pref)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.mPref = pref;
        }

        public void OnByDay()
        {
            try
            {
                this.mView.ShowByDay();
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
                this.mView.ShowByMonth();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnByKwh()
        {
            try
            {
                this.mView.ShowByKwh();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnByRM()
        {
            try
            {
                this.mView.ShowByRM();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnLearnMore()
        {

        }

        public void SaveEnergyBudgetAmmount(string accnum, int AmmSaveBudget)
        {
            SetEnergyBudget(accnum, AmmSaveBudget);
        }

        private async Task GetAccountStatus()
        {
            try
            {
                isSMRReady = false;

                cts = new CancellationTokenSource();

                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var installDetailsApi = RestService.For<IGetInstallationDetailsApi>(httpClient);

#else
            var installDetailsApi = RestService.For<IGetInstallationDetailsApi>(Constants.SERVER_URL.END_POINT);

#endif

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                var installDetailsResponse = await installDetailsApi.GetInstallationDetails(new Requests.GetInstallationDetailsRequest()
                {
                    AccountNumber = this.mView.GetSelectedAccount().AccountNum,
                    IsOwner = this.mView.GetSelectedAccount().IsOwner ? "true" : "false",
                    usrInf = currentUsrInf
                }, cts.Token);


                if (installDetailsResponse != null && installDetailsResponse.Data != null && installDetailsResponse.Data.ErrorCode == "7200")
                {
                    if (!string.IsNullOrEmpty(installDetailsResponse.Data.Data.DisconnectionStatus) && installDetailsResponse.Data.Data.DisconnectionStatus.ToUpper() != Constants.ENERGY_DISCONNECTION_KEY)
                    {
                        this.mView.ShowAccountStatus(installDetailsResponse.Data.Data);
                        this.mView.HideSSMRDashboardView();
                    }
                    else
                    {
                        this.mView.ShowAccountStatus(null);
                        bool isSMR = await IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                        if (isSMR)
                        {
                            this.mView.CheckSMRAccountValidaty();
                            await GetSSMRAccountStatus();
                        }
                        else
                        {
                            this.mView.HideSSMRDashboardView();
                        }
                    }
                }
                else
                {
                    this.mView.ShowAccountStatus(null);
                    bool isSMR = await IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                    if (isSMR)
                    {
                        this.mView.CheckSMRAccountValidaty();
                        await GetSSMRAccountStatus();
                    }
                    else
                    {
                        this.mView.HideSSMRDashboardView();
                    }
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.ShowAccountStatus(null);
                Utility.LoggingNonFatalError(e);
                bool isSMR = await IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                if (isSMR)
                {
                    this.mView.CheckSMRAccountValidaty();
                    await GetSSMRAccountStatus();
                }
                else
                {
                    this.mView.HideSSMRDashboardView();
                }
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowAccountStatus(null);
                Utility.LoggingNonFatalError(apiException);
                bool isSMR = await IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                if (isSMR)
                {
                    this.mView.CheckSMRAccountValidaty();
                    await GetSSMRAccountStatus();
                }
                else
                {
                    this.mView.HideSSMRDashboardView();
                }
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowAccountStatus(null);
                Utility.LoggingNonFatalError(e);
                bool isSMR = await IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                if (isSMR)
                {
                    this.mView.CheckSMRAccountValidaty();
                    await GetSSMRAccountStatus();
                }
                else
                {
                    this.mView.HideSSMRDashboardView();
                }
            }

        }

        private async Task GetSSMRAccountStatus()
        {
            try
            {
                cts = new CancellationTokenSource();

                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
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

                SMRActivityInfoResponse SMRAccountActivityInfoResponse = await ssmrAccountAPI.GetSMRAccountActivityInfo(new Requests.SMRAccountActivityInfoRequest()
                {
                    AccountNumber = this.mView.GetSelectedAccount().AccountNum,
                    IsOwnedAccount = this.mView.GetSelectedAccount().IsOwner ? "true" : "false",
                    userInterface = currentUsrInf
                }, cts.Token);


                if (SMRAccountActivityInfoResponse != null && SMRAccountActivityInfoResponse.Response != null && SMRAccountActivityInfoResponse.Response.ErrorCode == "7200")
                {
                    SMRPopUpUtils.OnSetSMRActivityInfoResponse(SMRAccountActivityInfoResponse);
                    MyTNBAppToolTipData.SetSMRActivityInfo(SMRAccountActivityInfoResponse.Response);
                    this.mView.ShowSSMRDashboardView(SMRAccountActivityInfoResponse);
                    isSMRReady = true;
                    OnCheckToCallDashboardTutorial();
                }
                else
                {
                    this.mView.HideSSMRDashboardView();
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideSSMRDashboardView();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.HideSSMRDashboardView();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.HideSSMRDashboardView();
                Utility.LoggingNonFatalError(e);
            }
        }

        public async Task LoadUsageHistory()
        {
            await GetAccountStatus();

            isBillAvailable = false;

            isDashboardReady = false;

            if (this.mView.IsLoadUsageNeeded())
            {
                if (!this.mView.GetIsSMAccount())
                {
                    await LoadNMREUsageHistory();
                }
                else
                {
                    await LoadSMUsageHistory();
                }
            }
            else
            {
                if (!this.mView.GetIsSMAccount())
                {
                    isDashboardReady = true;
                    OnCheckToCallDashboardTutorial();
                }
            }

            try
            {
                if (!isBillAvailable)
                {
                    if (!this.mView.GetIsSMAccount())
                    {
                        if (IsCheckHaveByMonthData(this.mView.GetUsageHistoryData()))
                        {
                            isBillAvailable = true;
                        }
                        else
                        {
                            isBillAvailable = false;
                        }
                    }
                    else
                    {
                        if (IsCheckHaveByMonthData(this.mView.GetSMUsageHistoryData()))
                        {
                            isBillAvailable = true;
                        }
                        else
                        {
                            isBillAvailable = false;
                        }
                    }
                }

                if (this.mView.GetIsREAccount() && isREFirstBill)
                {
                    isBillAvailable = false;
                }

                cts = new CancellationTokenSource();
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var amountDueApi = RestService.For<IAmountDueApi>(httpClient);
                var paymentStatusApi = RestService.For<IPaymentStatusApi>(httpClient);
#else
				var amountDueApi = RestService.For<IAmountDueApi>(Constants.SERVER_URL.END_POINT);
                var paymentStatusApi = RestService.For<IPaymentStatusApi>(Constants.SERVER_URL.END_POINT);
#endif

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                bool isPendingPayment = false;

                if (!this.mView.GetIsREAccount())
                {
                    try
                    {
                        DeviceInterface currentDvdInf = new DeviceInterface()
                        {
                            DeviceId = UserSessions.GetDeviceId(),
                            AppVersion = DeviceIdUtils.GetAppVersionName(),
                            OsType = Constants.DEVICE_PLATFORM,
                            OsVersion = DeviceIdUtils.GetAndroidVersion(),
                            DeviceDesc = LanguageUtil.GetAppLanguage().ToUpper(),
                            VersionCode = ""
                        };

                        List<string> accountList = new List<string>();
                        accountList.Add(this.mView.GetSelectedAccount().AccountNum);

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
                                        isPendingPayment = true;
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

                cts = new CancellationTokenSource();
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;


                AccountDueAmountResponse dueResponse = await amountDueApi.GetAccountDueAmount(new Requests.AccountDueAmountRequest()
                {
                    AccountNumber = this.mView.GetSelectedAccount().AccountNum,
                    IsOwnedAccount = this.mView.GetSelectedAccount().IsOwner ? "true" : "false",
                    usrInf = currentUsrInf
                }, cts.Token);

                if (dueResponse != null && dueResponse.Data != null && dueResponse.Data.ErrorCode != "7200")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!dueResponse.Data.IsPayEnabled);
                    this.mView.ShowAmountDueFailed();
                }
                else if (dueResponse != null && dueResponse.Data != null && dueResponse.Data.ErrorCode == "7200")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!dueResponse.Data.IsPayEnabled);
                    this.mView.ShowAmountDue(dueResponse.Data.Data.AmountDueData, isPendingPayment);
                    OnCleanUpNotifications(this.mView.GetSelectedAccount(), dueResponse.Data.Data.AmountDueData);
                }
                else
                {
                    if (dueResponse != null && dueResponse.Data != null)
                    {
                        Utility.SetIsPayDisableNotFromAppLaunch(!dueResponse.Data.IsPayEnabled);
                    }
                    this.mView.ShowAmountDueFailed();
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.ShowAmountDueFailed();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowAmountDueFailed();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.ShowAmountDueFailed();
                Utility.LoggingNonFatalError(e);
            }

        }

        private async Task LoadNMREUsageHistory()
        {
            try
            {
                isREFirstBill = false;

                cts = new CancellationTokenSource();
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var api = RestService.For<IUsageHistoryApi>(httpClient);
#else
				var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                var usageHistoryResponse = await api.DoQuery(new Requests.UsageHistoryRequest()
                {
                    AccountNumber = this.mView.GetSelectedAccount().AccountNum,
                    isOwner = this.mView.GetSelectedAccount().IsOwner ? "true" : "false",
                    accountType = this.mView.GetIsREAccount() ? "RE" : "NM",
                    userInterface = currentUsrInf
                }, cts.Token);

                if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "7201")
                {
                    isBillAvailable = true;
                    if (this.mView.GetIsREAccount())
                    {
                        isREFirstBill = true;
                    }
                    this.mView.SetUsageData(usageHistoryResponse.Data.UsageHistoryData);
                    this.mView.ShowNewAccountView(usageHistoryResponse.Data.DisplayTitle);
                }
                else if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "7200")
                {
                    if (IsCheckHaveByMonthData(usageHistoryResponse.Data.UsageHistoryData))
                    {
                        UsageHistoryEntity smUsageModel = new UsageHistoryEntity();
                        smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                        smUsageModel.JsonResponse = JsonConvert.SerializeObject(usageHistoryResponse);
                        smUsageModel.AccountNo = this.mView.GetSelectedAccount().AccountNum;
                        UsageHistoryEntity.InsertItem(smUsageModel);
                    }

                    this.mView.SetUsageData(usageHistoryResponse.Data.UsageHistoryData);
                    if (!usageHistoryResponse.Data.IsMonthlyTariffBlocksDisabled && !usageHistoryResponse.Data.IsMonthlyTariffBlocksUnavailable)
                    {
                        this.mView.OnSetBackendTariffDisabled(false);
                    }
                    else
                    {
                        this.mView.OnSetBackendTariffDisabled(true);
                    }
                    OnByRM();
                    isDashboardReady = true;
                    OnCheckToCallDashboardTutorial();
                }
                else if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "8400")
                {
                    isBillAvailable = true;
                    string contentTxt = "";
                    if (usageHistoryResponse != null && usageHistoryResponse.Data != null && !string.IsNullOrEmpty(usageHistoryResponse.Data.DisplayMessage))
                    {
                        contentTxt = usageHistoryResponse.Data.DisplayMessage;
                    }

                    this.mView.OnShowPlannedDowntimeScreen(contentTxt);
                }
                else
                {
                    isBillAvailable = true;
                    string contentTxt = "";
                    string buttonTxt = "";
                    if (usageHistoryResponse != null && usageHistoryResponse.Data != null && !string.IsNullOrEmpty(usageHistoryResponse.Data.RefreshMessage))
                    {
                        contentTxt = usageHistoryResponse.Data.RefreshMessage;
                    }
                    if (usageHistoryResponse != null && usageHistoryResponse.Data != null && !string.IsNullOrEmpty(usageHistoryResponse.Data.RefreshBtnText))
                    {
                        buttonTxt = usageHistoryResponse.Data.RefreshBtnText;
                    }

                    this.mView.ShowNoInternet(contentTxt, buttonTxt);
                }
            }
            catch (System.OperationCanceledException e)
            {
                isBillAvailable = true;
                this.mView.ShowNoInternet(null, null);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                isBillAvailable = true;
                this.mView.ShowNoInternet(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                isBillAvailable = true;
                this.mView.ShowNoInternet(null, null);
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task LoadSMUsageHistory()
        {
            try
            {
                cts = new CancellationTokenSource();
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var api = RestService.For<IUsageHistoryApi>(httpClient);
#else
				var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                var usageHistoryResponse = await api.DoSMQueryV2(new Requests.SMUsageHistoryRequest()
                {
                    AccountNumber = this.mView.GetSelectedAccount().AccountNum,
                    isOwner = this.mView.GetSelectedAccount().IsOwner ? "true" : "false",
                    MeterCode = this.mView.GetSelectedAccount().SmartMeterCode,
                    userInterface = currentUsrInf
                }, cts.Token);

                if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "7201")
                {
                    isBillAvailable = true;
                    this.mView.SetSMUsageData(usageHistoryResponse.Data.SMUsageHistoryData);
                    this.mView.ShowNewAccountView(usageHistoryResponse.Data.DisplayTitle);
                }
                else if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "7204")
                {
                    this.mView.SetISMDMSDown(true);
                    this.mView.SetSMUsageData(usageHistoryResponse.Data.SMUsageHistoryData);
                    this.mView.SetMDMSDownRefreshMessage(usageHistoryResponse);
                    OnByRM();
                }
                else if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "8304")
                {
                    this.mView.SetISMDMSDown(true);
                    this.mView.SetSMUsageData(usageHistoryResponse.Data.SMUsageHistoryData);
                    this.mView.SetMDMSDownMessage(usageHistoryResponse);
                    OnByRM();
                }
                else if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "7200" )
                {
                    if (IsCheckHaveByMonthData(usageHistoryResponse.Data.SMUsageHistoryData))
                    {
                        SMUsageHistoryEntity smUsageModel = new SMUsageHistoryEntity();
                        smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                        smUsageModel.JsonResponse = JsonConvert.SerializeObject(usageHistoryResponse);
                        smUsageModel.AccountNo = this.mView.GetSelectedAccount().AccountNum;
                        SMUsageHistoryEntity.InsertItem(smUsageModel);
                    }

                    if (!usageHistoryResponse.Data.IsMonthlyTariffBlocksDisabled && !usageHistoryResponse.Data.IsMonthlyTariffBlocksUnavailable)
                    {
                        this.mView.OnSetBackendTariffDisabled(false);
                    }
                    else
                    {
                        this.mView.OnSetBackendTariffDisabled(true);
                    }
                    //this.mView.SetISMDMSDown(false);
                    this.mView.SetISMDMSDown(true);     //debug data
                    this.mView.SetSMUsageData(usageHistoryResponse.Data.SMUsageHistoryData);
                    OnByRM();
                }
                else if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode == "8400")
                {
                    isBillAvailable = true;
                    string contentTxt = "";
                    if (usageHistoryResponse != null && usageHistoryResponse.Data != null && !string.IsNullOrEmpty(usageHistoryResponse.Data.DisplayMessage))
                    {
                        contentTxt = usageHistoryResponse.Data.DisplayMessage;
                    }

                    this.mView.OnShowPlannedDowntimeScreen(contentTxt);
                }
                else
                {
                    isBillAvailable = true;

                    string contentTxt = "";
                    string buttonTxt = "";
                    if (usageHistoryResponse != null && usageHistoryResponse.Data != null && !string.IsNullOrEmpty(usageHistoryResponse.Data.RefreshMessage))
                    {
                        contentTxt = usageHistoryResponse.Data.RefreshMessage;
                    }
                    if (usageHistoryResponse != null && usageHistoryResponse.Data != null && !string.IsNullOrEmpty(usageHistoryResponse.Data.RefreshBtnText))
                    {
                        buttonTxt = usageHistoryResponse.Data.RefreshBtnText;
                    }

                    this.mView.ShowNoInternet(contentTxt, buttonTxt);
                }
            }
            catch (System.OperationCanceledException e)
            {
                isBillAvailable = true;
                this.mView.ShowNoInternet(null, null);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                isBillAvailable = true;
                this.mView.ShowNoInternet(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                isBillAvailable = true;
                this.mView.ShowNoInternet(null, null);
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

        public void OnViewBillDetails(AccountData selectedAccount)
		{
			ShowBillDetails(selectedAccount);
		}

        private async void ShowBillDetails(AccountData selectedAccount)
        {
            try
            {
                this.mView.ShowProgress();
                List<string> accountList = new List<string>();
                accountList.Add(selectedAccount.AccountNum);
                List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
                AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                    accountList,
                    selectedAccount.IsOwner
                    );
                AccountChargesResponse accountChargeseResponse = await ServiceApiImpl.Instance.GetAccountsCharges(accountChargeseRequest);
                this.mView.HideProgress();
                if (accountChargeseResponse.IsSuccessResponse())
                {
                    accountChargeModelList = BillingResponseParser.GetAccountCharges(accountChargeseResponse.GetData().AccountCharges);
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargeseResponse.GetData().MandatoryChargesPopUpDetails));
                    this.mView.ShowBillDetails(selectedAccount, accountChargeModelList);
                }
                else
                {
                    this.mView.ShowLoadBillRetryOptions();
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void LoadingBillsHistory(AccountData selectedAccount)
        {
            this.mView.ShowProgress();
            try
            {
                var billsHistoryResponse = await ServiceApiImpl.Instance.GetBillHistory(new MyTNBService.Request.GetBillHistoryRequest(selectedAccount.AccountNum, selectedAccount.IsOwner));

                this.mView.HideProgress();

                if (billsHistoryResponse.IsSuccessResponse())
                {
                    if (billsHistoryResponse.GetData() != null && billsHistoryResponse.GetData().Count > 0)
                    {
                        this.mView.ShowViewBill(billsHistoryResponse.GetData()[0]);
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
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void SetEnergyBudget(string accnum, int amountEnergybudget)
        {
            this.mView.ShowProgress();
            try
            {
                var saveEnergyBudgetResponse = await ServiceApiImpl.Instance.SaveEnergyBudget(new MyTNBService.Request.SaveEnergyBudgetRequest(accnum, amountEnergybudget));

                this.mView.HideProgress();

                if (saveEnergyBudgetResponse.IsSuccessResponse())
                {
                    this.mView.ShowEnergyBudgetSuccess();
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(saveEnergyBudgetResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
        }


        public void Start()
        {
            try
            {
                //if (this.mView.IsBCRMDownFlag())
                //{
                //    this.mView.ShowNoInternet(null, null);
                //    this.mView.ShowAmountDueFailed();
                //}
                //else
                //{
                    if (!this.mView.IsLoadUsageNeeded())
                    {
                        OnByRM();
                    }

                    _ = LoadUsageHistory();
                //}
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async Task<bool> IsOwnedSMR(string accountNumber)
        {
            bool IsSMRFeatureDisabled = false;
            if (MyTNBAccountManagement.GetInstance().IsSMRFeatureDisabled())
            {
                IsSMRFeatureDisabled = true;
            }

            if (IsSMRFeatureDisabled)
            {
                return false;
            }
            else
            {
                try
                {
                    List<CustomerBillingAccount> eligibleSMRAccountList = CustomerBillingAccount.GetEligibleAndSMRAccountList();
                    foreach (CustomerBillingAccount account in eligibleSMRAccountList)
                    {
                        if (account.AccNum == accountNumber)
                        {
#if DEBUG || STUB
                            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new System.Uri(Constants.SERVER_URL.END_POINT) };
                            var api = RestService.For<IAccountsSMRStatusApi>(httpClient);
#else
                            var api = RestService.For<IAccountsSMRStatusApi>(Constants.SERVER_URL.END_POINT);
#endif

                            UserInterface currentUsrInf = new UserInterface()
                            {
                                eid = UserEntity.GetActive().Email,
                                sspuid = UserEntity.GetActive().UserID,
                                did = this.mView.GetDeviceId(),
                                ft = FirebaseTokenEntity.GetLatest().FBToken,
                                lang = LanguageUtil.GetAppLanguage().ToUpper(),
                                sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                                sec_auth_k2 = "",
                                ses_param1 = "",
                                ses_param2 = ""
                            };

                            List<string> accountList = new List<string>();
                            accountList.Add(accountNumber);

                            AccountSMRStatusResponse accountSMRResponse = await api.AccountsSMRStatusApi(new AccountsSMRStatusRequest()
                            {
                                ContractAccounts = accountList,
                                UserInterface = currentUsrInf
                            }, new CancellationTokenSource().Token);

                            List<AccountSMRStatus> updateSMRStatus = new List<AccountSMRStatus>();
                            if (accountSMRResponse.Response.ErrorCode == "7200" && accountSMRResponse.Response.Data.Count > 0)
                            {
                                bool selectedUpdateIsTaggedSMR = false;

                                updateSMRStatus = accountSMRResponse.Response.Data;
                                foreach (AccountSMRStatus status in updateSMRStatus)
                                {
                                    CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(status.ContractAccount);
                                    if (status.IsTaggedSMR == "true")
                                    {
                                        selectedUpdateIsTaggedSMR = true;
                                    }

                                    if (selectedUpdateIsTaggedSMR != cbAccount.IsTaggedSMR)
                                    {
                                        CustomerBillingAccount.UpdateIsSMRTagged(status.ContractAccount, selectedUpdateIsTaggedSMR);
                                    }
                                }
                                List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();
                                List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
                                if (currentSMRBillingAccounts.Count > 0)
                                {
                                    foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccounts)
                                    {
                                        SMRAccount currentSMRAccount = new SMRAccount();
                                        currentSMRAccount.accountNumber = billingAccount.AccNum;
                                        currentSMRAccount.accountName = billingAccount.AccDesc;
                                        currentSMRAccount.accountAddress = billingAccount.AccountStAddress;
                                        currentSMRAccount.accountSelected = false;
                                        currentSmrAccountList.Add(currentSMRAccount);
                                    }
                                }
                                UserSessions.SetSMRAccountList(currentSmrAccountList);

                                List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
                                List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
                                if (eligibleSMRBillingAccounts.Count > 0)
                                {
                                    foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccounts)
                                    {
                                        SMRAccount currentSMRAccount = new SMRAccount();
                                        currentSMRAccount.accountNumber = billingAccount.AccNum;
                                        currentSMRAccount.accountName = billingAccount.AccDesc;
                                        currentSMRAccount.accountAddress = billingAccount.AccountStAddress;
                                        currentSMRAccount.accountSelected = false;
                                        eligibleSmrAccountList.Add(currentSMRAccount);
                                    }
                                }
                                UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);
                                UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);

                                if (selectedUpdateIsTaggedSMR)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                catch (System.OperationCanceledException cancelledException)
                {
                    Utility.LoggingNonFatalError(cancelledException);
                }
                catch (ApiException apiException)
                {
                    Utility.LoggingNonFatalError(apiException);
                }
                catch (Exception unknownException)
                {
                    Utility.LoggingNonFatalError(unknownException);
                }
            }
            return false;
        }

        public bool IsOwnedSMRLocal(string accountNumber)
        {
            try
            {
                foreach (SMRAccount smrAccount in UserSessions.GetSMRAccountList())
                {
                    if (smrAccount.accountNumber == accountNumber)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return false;
        }

        private bool IsCheckHaveByMonthData(UsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }
            else
            {
                for (int i = 0; i < data.ByMonth.Months.Count; i++)
                {
                    if ((string.IsNullOrEmpty(data.ByMonth.Months[i].UsageTotal.ToString()) && string.IsNullOrEmpty(data.ByMonth.Months[i].AmountTotal.ToString())) || (Math.Abs(data.ByMonth.Months[i].UsageTotal) < 0.001 && Math.Abs(data.ByMonth.Months[i].AmountTotal) < 0.001))
                    {
                        isHaveData = false;
                    }
                    else
                    {
                        isHaveData = true;
                        break;
                    }
                }
            }

            return isHaveData;
        }

        private bool IsCheckHaveByMonthData(SMUsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }
            else
            {
                for (int i = 0; i < data.ByMonth.Months.Count; i++)
                {
                    if ((string.IsNullOrEmpty(data.ByMonth.Months[i].UsageTotal.ToString()) && string.IsNullOrEmpty(data.ByMonth.Months[i].AmountTotal.ToString())) || (Math.Abs(data.ByMonth.Months[i].UsageTotal) < 0.001 && Math.Abs(data.ByMonth.Months[i].AmountTotal) < 0.001))
                    {
                        isHaveData = false;
                    }
                    else
                    {
                        isHaveData = true;
                        break;
                    }
                }
            }

            return isHaveData;
        }

        private bool IsCheckDataReadyData(UsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }

            return isHaveData;
        }

        private bool IsCheckDataReadyData(SMUsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }

            return isHaveData;
        }

        public bool IsBillingAvailable()
        {
            return isBillAvailable;
        }

        public Task OnGetEnergySavingTips()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    EnergySavingTipsEntity EnergySavingTipsManager = new EnergySavingTipsEntity();
                    List<EnergySavingTipsEntity> energyList = EnergySavingTipsManager.GetAllItems();
                    if (energyList.Count > 0)
                    {
                        List<EnergySavingTipsModel> savedList = new List<EnergySavingTipsModel>();
                        foreach (EnergySavingTipsEntity item in energyList)
                        {

                            savedList.Add(new EnergySavingTipsModel()
                            {
                                Title = item.Title,
                                Description = item.Description,
                                Image = item.Image,
                                isUpdateNeeded = true,
                                ImageBitmap = null,
                                ID = item.ID
                            });
                        }

                        _ = OnRandomizeEnergyTipsView(savedList);
                    }
                    else
                    {
                        this.mView.HideEnergyTipsShimmerView();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        public async Task OnRandomizeEnergyTipsView(List<EnergySavingTipsModel> list)
        {
            List<EnergySavingTipsModel> energyList = new List<EnergySavingTipsModel>();
            var random = new System.Random();
            List<int> listNumbers = new List<int>();
            int number;
            if (list.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    do
                    {
                        number = random.Next(1, list.Count);
                    } while (listNumbers.Contains(number));
                    listNumbers.Add(number);
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    listNumbers.Add(i);
                }
            }


            for (int j = 0; j < listNumbers.Count; j++)
            {
                energyList.Add(list[listNumbers[j]]);
            }

            foreach (EnergySavingTipsModel energyItem in energyList)
            {
                if (energyItem.ImageBitmap == null)
                {
                    energyItem.ImageBitmap = await GetPhoto(energyItem.Image);
                    energyItem.isUpdateNeeded = false;
                }
            }

            this.mView.ShowEnergyTipsView(energyList);

        }

        private static async Task<Bitmap> GetPhoto(string imageUrl)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            try
            {
                await Task.Run(() =>
                {
                    imageBitmap = ImageUtils.GetImageBitmapFromUrl(imageUrl);
                }, cts.Token);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return imageBitmap;
        }

        public List<EnergySavingTipsModel> OnLoadEnergySavingTipsShimmerList(int count)
        {
            List<EnergySavingTipsModel> energyList = new List<EnergySavingTipsModel>();

            for(int i = 0; i < count; i++)
            {
                energyList.Add(new EnergySavingTipsModel()
                {
                    Title = ""
                });
            }
            return energyList;
        }

        public void OnByZoom()
        {
            try
            {
                this.mView.ByZoomDayView();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCheckToCallDashboardTutorial()
        {
            if (isDashboardReady && isSMRReady)
            {
                if (!UserSessions.HasSMRDashboardTutorialShown(this.mPref))
                {
                    this.mView.OnShowDashboardFragmentTutorialDialog();
                }
            }
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Usage", "tutorialSMRTitle"),
                ContentMessage = Utility.GetLocalizedLabel("Usage", "tutorialSMRDesc"),
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }

        private void OnCleanUpNotifications(AccountData accountData, AccountDueAmountData amtDueData)
        {
            try
            {
                if (MyTNBAccountManagement.GetInstance().IsNotificationServiceCompleted())
                {
                    List<Notifications.Models.UserNotificationData> ToBeDeleteList = new List<Notifications.Models.UserNotificationData>();

                    double amtDue = 0.00;
                    if (this.mView.GetIsREAccount())
                    {
                        amtDue = amtDueData.AmountDue * -1;
                    }
                    else
                    {
                        amtDue = amtDueData.AmountDue;
                    }

                    if (amtDue <= 0.00)
                    {
                        List<UserNotificationEntity> billDueList = UserNotificationEntity.ListFilteredNotificationsByBCRMType(accountData.AccountNum, Constants.BCRM_NOTIFICATION_BILL_DUE_ID);
                        if (billDueList != null && billDueList.Count > 0)
                        {
                            for (int j = 0; j < billDueList.Count; j++)
                            {
                                UserNotificationEntity.UpdateIsDeleted(billDueList[j].Id, true);
                                Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                temp.Id = billDueList[j].Id;
                                temp.NotificationType = billDueList[j].NotificationType;
                                ToBeDeleteList.Add(temp);
                            }
                        }

                        List<UserNotificationEntity> disconnectNoticeList = UserNotificationEntity.ListFilteredNotificationsByBCRMType(accountData.AccountNum, Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID);
                        if (disconnectNoticeList != null && disconnectNoticeList.Count > 0)
                        {
                            for (int j = 0; j < disconnectNoticeList.Count; j++)
                            {
                                UserNotificationEntity.UpdateIsDeleted(disconnectNoticeList[j].Id, true);
                                Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                temp.Id = disconnectNoticeList[j].Id;
                                temp.NotificationType = disconnectNoticeList[j].NotificationType;
                                ToBeDeleteList.Add(temp);
                            }
                        }
                    }

                    if (ToBeDeleteList != null && ToBeDeleteList.Count > 0)
                    {
                        _ = OnBatchDeleteNotifications(ToBeDeleteList);
                    }
                }
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        private async Task OnBatchDeleteNotifications(List<Notifications.Models.UserNotificationData> accountList)
        {
            try
            {
                if (accountList != null && accountList.Count > 0)
                {
                    UserNotificationDeleteResponse notificationDeleteResponse = await ServiceApiImpl.Instance.DeleteUserNotification(new UserNotificationDeleteRequest(accountList));

                    if (notificationDeleteResponse.IsSuccessResponse())
                    {

                    }
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }
    }
}
