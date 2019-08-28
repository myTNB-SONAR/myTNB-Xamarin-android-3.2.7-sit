using Android.Graphics;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
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

        private bool isBillAvailable = true;

        public DashboardChartPresenter(DashboardChartContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
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

        private async Task GetAccountStatus()
        {
            try
            {
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
                    lang = Constants.DEFAULT_LANG.ToUpper(),
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
                    if (installDetailsResponse.Data.Data.DisconnectionStatus == "Available")
                    {
                        this.mView.ShowAccountStatus(null);
                        bool isSMR = IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                        if (isSMR)
                        {
                            await GetSSMRAccountStatus();
                        }
                        else
                        {
                            this.mView.HideSSMRDashboardView();
                        }
                    }
                    else
                    {
                        this.mView.ShowAccountStatus(installDetailsResponse.Data.Data);
                        this.mView.HideSSMRDashboardView();
                    }
                }
                else
                {
                    this.mView.ShowAccountStatus(null);
                    bool isSMR = IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                    if (isSMR)
                    {
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
                bool isSMR = IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                if (isSMR)
                {
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
                bool isSMR = IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                if (isSMR)
                {
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
                bool isSMR = IsOwnedSMR(this.mView.GetSelectedAccount().AccountNum);
                if (isSMR)
                {
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

                SMRActivityInfoResponse SMRAccountActivityInfoResponse = await ssmrAccountAPI.GetSMRAccountActivityInfo(new Requests.SMRAccountActivityInfoRequest()
                {
                    AccountNumber = this.mView.GetSelectedAccount().AccountNum,
                    IsOwnedAccount = this.mView.GetSelectedAccount().IsOwner ? "true" : "false",
                    userInterface = currentUsrInf
                }, cts.Token);


                if (SMRAccountActivityInfoResponse != null && SMRAccountActivityInfoResponse.Response != null && SMRAccountActivityInfoResponse.Response.ErrorCode == "7200")
                {
                    SMRPopUpUtils.OnSetSMRActivityInfoResponse(SMRAccountActivityInfoResponse);
                    this.mView.ShowSSMRDashboardView(SMRAccountActivityInfoResponse);
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

            if (this.mView.IsLoadUsageNeeded())
            {
                if (this.mView.GetSelectedAccount().SmartMeterCode.Equals("0"))
                {
                    await LoadNMREUsageHistory();
                }
                else
                {
                    // Lin Siong TODO: To split the usage calling for smart meter
                    await LoadNMREUsageHistory();
                }
            }

            try
            {
                if (this.mView.GetSelectedAccount().SmartMeterCode.Equals("0"))
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
                    // Lin Siong TODO: To check billing available flag
                    isBillAvailable = true;
                }

                cts = new CancellationTokenSource();
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var amountDueApi = RestService.For<IAmountDueApi>(httpClient);
#else
				var amountDueApi = RestService.For<IAmountDueApi>(Constants.SERVER_URL.END_POINT);
#endif

                AccountDueAmountResponse dueResponse = await amountDueApi.GetAccountDueAmount(new Requests.AccountDueAmountRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    AccNum = this.mView.GetSelectedAccount().AccountNum

                }, cts.Token);

                if (dueResponse != null && dueResponse.Data != null && dueResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
                {
                    this.mView.ShowAmountDueFailed();
                }
                else if (!dueResponse.Data.IsError)
                {
                    this.mView.ShowAmountDue(dueResponse.Data.Data);
                }
                else
                {
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
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                var usageHistoryResponse = await api.DoQuery(new Requests.UsageHistoryRequest()
                {
                    AccountNumber = this.mView.GetSelectedAccount().AccountNum,
                    isOwner = this.mView.GetSelectedAccount().IsOwner ? "true" : "false",
                    userInterface = currentUsrInf
                }, cts.Token);

                if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode != "7200")
                {
                    this.mView.ShowNoInternet(usageHistoryResponse.Data.RefreshMessage, usageHistoryResponse.Data.RefreshBtnText);
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
                    OnByRM();
                }
                else
                {
                    this.mView.ShowNoInternet(null, null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.ShowNoInternet(null, null);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowNoInternet(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
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


        private async void LoadingBillsHistory(AccountData selectedAccount)
        {
            this.mView.ShowProgress();
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

                this.mView.HideProgress();

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
                if (this.mView.IsBCRMDownFlag())
                {
                    this.mView.ShowNoInternet(null, null);
                    this.mView.ShowAmountDueFailed();
                }
                else
                {
                    if (!this.mView.IsLoadUsageNeeded())
                    {
                        OnByRM();
                    }

                    _ = LoadUsageHistory();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsOwnedSMR(string accountNumber)
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

            try
            {
                if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months.Count == 0))
                {
                    isHaveData = false;
                }
                else
                {
                    foreach (UsageHistoryData.ByMonthData.MonthData monthData in data.ByMonth.Months)
                    {
                        if ((string.IsNullOrEmpty(monthData.UsageTotal.ToString()) && string.IsNullOrEmpty(monthData.AmountTotal.ToString())) || (Math.Abs(monthData.UsageTotal) < 0.001 && Math.Abs(monthData.AmountTotal) < 0.001))
                        {
                            isHaveData = false;
                        }
                        else
                        {
                            isHaveData = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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


    }
}