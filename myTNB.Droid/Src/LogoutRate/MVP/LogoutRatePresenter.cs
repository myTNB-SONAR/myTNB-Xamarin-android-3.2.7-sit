using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogoutRate.Api;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

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

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }



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
                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

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
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE

                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void Start()
        {
            //
        }
    }
}