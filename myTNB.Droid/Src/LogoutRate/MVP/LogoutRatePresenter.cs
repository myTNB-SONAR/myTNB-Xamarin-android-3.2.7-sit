using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
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

            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                var logoutResponse = await ServiceApiImpl.Instance.LogoutUser(new LogoutUserRequest());

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

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
        }
    }
}