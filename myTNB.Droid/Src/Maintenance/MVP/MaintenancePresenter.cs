using Android.Content;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net.Http;
using System.Threading;
using static myTNB_Android.Src.AppLaunch.Models.MasterDataRequest;

namespace myTNB_Android.Src.Maintenance.MVP
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

                DeviceInterface currentDeviceInf = new DeviceInterface()
                {
                    DeviceId = this.mView.GetDeviceId(),
                    AppVersion = DeviceIdUtils.GetAppVersionName(),
                    OsType = int.Parse(Constants.DEVICE_PLATFORM),
                    OsVersion = DeviceIdUtils.GetAndroidVersion(),
                    DeviceDesc = Constants.DEFAULT_LANG

                };

                var masterDataResponse = await masterDataApi.GetAppLaunchMasterData(new MasterDataRequest()
                {
                    deviceInf = currentDeviceInf,
                    usrInf = currentUsrInf
                }, cts.Token);

                if (masterDataResponse != null && masterDataResponse.Data != null)
                {

                if (masterDataResponse.Data.ErrorCode != "7000" && masterDataResponse.Data.ErrorCode == "7200")
                {
                    this.mView.ShowLaunchViewActivity();
                }
                else if (masterDataResponse.Data.ErrorCode == "7000")
                {

                }
                else
                {
                    Console.WriteLine("Excution time enters else");
                    // TODO : SHOW ERROR
                    this.mView.ShowRetryOptionApiException(null);
                }
                }
                else
                {
                    Console.WriteLine("Excution time enters else");
                    // TODO : SHOW ERROR
                    this.mView.ShowRetryOptionApiException(null);
                }




            }
            catch (ApiException apiException)
            {
                //Log.Debug(TAG, "Api Exception " + apiException.GetContentAs<string>());
                //Log.Debug(TAG, "Api Exception " + apiException.StatusCode);
                //Log.Debug(TAG, "Api Exception " + apiException);
                this.mView.ShowRetryOptionApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                this.mView.ShowRetryOptionUknownException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                this.mView.ShowRetryOptionUknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            //
        }
    }
}
