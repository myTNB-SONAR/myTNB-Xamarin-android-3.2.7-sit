using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AppLaunch.Async;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Refit;

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
                string email = null, sspUserID = null;
                if (UserEntity.IsCurrentlyActive())
                {
                    email = UserEntity.GetActive().UserName;
                    sspUserID = UserEntity.GetActive().UserID;
                }

                var masterDataResponse = await masterDataApi.GetAppLaunchMasterData(new MasterDataRequest()
                {
                    ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceId = this.mView.GetDeviceId(),
                    AppVersion = DeviceIdUtils.GetAppVersionName(),
                    Email = email,
                    SSPUserId = sspUserID,
                    OsType = Constants.DEVICE_PLATFORM,
                    OsVersion = DeviceIdUtils.GetAndroidVersion()
                }, cts.Token);

                if (!masterDataResponse.Data.IsError && !masterDataResponse.Data.Status.ToUpper().Equals(Constants.MAINTENANCE_MODE))
                {
                    this.mView.ShowLaunchViewActivity();
                }
                else if (masterDataResponse.Data.Status.ToUpper().Equals(Constants.MAINTENANCE_MODE))
                {

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