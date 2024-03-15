using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.LogoutRate.MVP
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

        public void OnLogout(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            Task.Run(() =>
            {
                _ = Logout();
            });

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

            if (mView.IsActive())
            {
                this.mView.HideProgressDialog();
            }
        }

        public async Task Logout()
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            cts = new CancellationTokenSource();

            _ = await ServiceApiImpl.Instance.LogoutUser(new LogoutUserRequest());
        }

        public void Start() { }
    }
}