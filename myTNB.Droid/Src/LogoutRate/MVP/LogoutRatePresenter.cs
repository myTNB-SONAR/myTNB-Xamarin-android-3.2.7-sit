using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System.Threading;
using Refit;
using System.Net;
using System.Net.Http;
using myTNB_Android.Src.LogoutRate.Api;
using Firebase.Iid;

namespace myTNB_Android.Src.LogoutRate.MVP
{
    public class LogoutRatePresenter : LogoutRateContract.IUserActionsListener
    {

        private LogoutRateContract.IView mView;

        CancellationTokenSource cts;

        public LogoutRatePresenter(LogoutRateContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public async void OnLogout(string deviceId)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            cts = new CancellationTokenSource();

            this.mView.ShowProgressDialog();

 


#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };

            var logoutApi = RestService.For<ILogoutApi>(httpClient);
#else
            var logoutApi = RestService.For<ILogoutApi>(Constants.SERVER_URL.END_POINT);
#endif
            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                var logoutResponse = await logoutApi.LogoutUser(new Request.LogoutRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Email = userEntity.Email,
                    DeviceId = deviceId
                } , cts.Token);

                if (!logoutResponse.Data.IsError)
                {
                    UserEntity.RemoveActive();
                    UserRegister.RemoveActive();
                    CustomerBillingAccount.RemoveActive();
                    NotificationFilterEntity.RemoveAll();
                    UserNotificationEntity.RemoveAll();
                    SMUsageHistoryEntity.RemoveAll();
                    UsageHistoryEntity.RemoveAll();
                    BillHistoryEntity.RemoveAll();
                    PaymentHistoryEntity.RemoveAll();
                    REPaymentHistoryEntity.RemoveAll();
                    AccountDataEntity.RemoveAll();
                    SummaryDashBoardAccountEntity.RemoveAll();
                    SelectBillsEntity.RemoveAll();
                    this.mView.ShowLogoutSuccess();
                }
                else
                {
                    this.mView.ShowErrorMessage(logoutResponse.Data.Message);
                }
            }
            catch (System.OperationCanceledException e)
            {

                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE

                this.mView.ShowRetryOptionsUnknownException(e);
            }


            this.mView.HideProgressDialog();
        }

        public void Start()
        {
            //
        }
    }
}