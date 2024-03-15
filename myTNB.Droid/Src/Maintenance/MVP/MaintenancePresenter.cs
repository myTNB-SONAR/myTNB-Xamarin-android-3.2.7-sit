using Android.Content;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.Base.Api;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Net.Http;
using System.Threading;
using static myTNB.AndroidApp.Src.AppLaunch.Models.MasterDataRequest;

namespace myTNB.AndroidApp.Src.Maintenance.MVP
{
    public class MaintenancePresenter : MaintenanceContract.IUserActionsListener
    {
        MaintenanceContract.IView mView;
        private ISharedPreferences mSharedPref;
        public static readonly string TAG = "MaintenanceActivity";
        CancellationTokenSource cts;

        public MaintenancePresenter(MaintenanceContract.IView mView, ISharedPreferences sharedPreferences)
        {
            this.mView = mView;
            this.mSharedPref = sharedPreferences;
            this.mView.SetPresenter(this);
        }

        public void OnResume()
        {
            LoadAccounts();
        }

        private async void LoadAccounts()
        {
            cts = new CancellationTokenSource();
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var masterDataApi = RestService.For<GetMasterDataApi>(httpClient);
#else
            var masterDataApi = RestService.For<GetMasterDataApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                Context mContext = MyTNBApplication.Context;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = "",
                    sspuid = "",
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                if (UserEntity.IsCurrentlyActive())
                {
                    currentUsrInf.eid = UserEntity.GetActive().Email;
                    currentUsrInf.sspuid = UserEntity.GetActive().UserID;
                }

                /*MasterDataRequest.DeviceInterface currentDeviceInf = new MasterDataRequest.DeviceInterface()
                {
                    DeviceId = this.mView.GetDeviceId(),
                    AppVersion = DeviceIdUtils.GetAppVersionName(),
                    OsType = int.Parse(Constants.DEVICE_PLATFORM),
                    OsVersion = DeviceIdUtils.GetAndroidVersion(),
                    DeviceDesc = Constants.DEFAULT_LANG,
                    VersionCode = ""

                };*/

                DeviceInfoRequest currentDeviceInf = new DeviceInfoRequest()
                {
                    DeviceId = this.mView.GetDeviceId(),
                    AppVersion = DeviceIdUtils.GetAppVersionName(),
                    OsType = Constants.DEVICE_PLATFORM,
                    OsVersion = DeviceIdUtils.GetAndroidVersion(),
                    DeviceDesc = Constants.DEFAULT_LANG,
                    VersionCode = ""

                };

                AppLaunchMasterDataResponseAWS masterDataResponse = await ServiceApiImpl.Instance.GetAppLaunchMasterDataAWS(new AppLaunchMasterDataRequestAWS(currentDeviceInf));
                /*var masterDataResponse = await masterDataApi.GetAppLaunchMasterData(new MasterDataRequest()
                {
                    deviceInf = currentDeviceInf,
                    usrInf = currentUsrInf
                }, cts.Token);*/

                if (masterDataResponse != null && masterDataResponse.Data != null)
                {
                    if (masterDataResponse.ErrorCode == "7200")
                    {
                        this.mView.ShowLaunchViewActivity();
                    }
                    else if (masterDataResponse.ErrorCode == "7000")
                    {
                        string title = "";
                        string message = "";

                        if (!string.IsNullOrEmpty(masterDataResponse.DisplayTitle))
                        {
                            title = masterDataResponse.DisplayTitle;
                        }

                        if (!string.IsNullOrEmpty(masterDataResponse.DisplayMessage))
                        {
                            message = masterDataResponse.DisplayMessage;
                        }

                        this.mView.OnUpdateMaintenanceWord(title, message);
                    }
                }
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            //
        }
    }
}
