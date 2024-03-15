using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using Refit;
using System;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using static myTNB.AndroidApp.Src.myTNBMenu.Models.GetInstallationDetailsResponse;
using myTNB.SitecoreCMS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using myTNB.AndroidApp.Src.MyTNBService.Response;

namespace myTNB.AndroidApp.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardChartContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show by Month chart
            /// </summary>
            void ShowByMonth();

            /// <summary>
            /// Show by Day chart
            /// </summary>
            void ShowByDay();

            /// <summary>
            /// Show by kWh chart
            /// </summary>
            void ShowByKwh();

            /// <summary>
            /// Show by RM chart
            /// </summary>
            void ShowByRM();

            /// <summary>
            /// Show ViewBill
            /// </summary>
            void ShowViewBill(GetBillHistoryResponse.ResponseData selectedBill = null);

            /// <summary>
            /// Show Payment
            /// </summary>
            void ShowPayment();

            /// <summary>
            /// Show no internet
            /// </summary>
            void ShowNoInternet(string contentTxt, string buttonTxt);

            /// <summary>
            ///  Show no internet snackbar
            /// </summary>
            void ShowNoInternetSnackbar();

            /// <summary>
            /// Show tab refresh
            /// </summary>
            void ShowTapRefresh();

            /// <summary>
            /// Show notification list
            /// </summary>
            void ShowNotification();

            /// <summary>
            /// Show amount due
            /// </summary>
            /// <param name="accountDueAmount">AccountDueAmount</param>
            void ShowAmountDue(AccountDueAmountData accountDueAmount, bool isPendingPayment);

            /// <summary>
            /// Enable pay button
            /// </summary>
            void EnablePayButton();

            /// <summary>
            /// Disable pay button
            /// </summary>
            void DisablePayButton();

            void ShowLoadBillRetryOptions();

            /// <summary>
            /// Show Account Status
            /// </summary>
            /// <param name="accountStatus">AccountStatusData</param>
            void ShowAccountStatus(AccountStatusData accountStatus);

            void ShowSSMRDashboardView(SMRActivityInfoResponse response);

            void HideSSMRDashboardView();

            void ShowDisconnectionRetrySnakebar();

            void ShowSMRRetrySnakebar();

            string GetDeviceId();

            void ShowProgress();

            void HideProgress();

            bool isSMDataError();

            bool IsBCRMDownFlag();

            bool IsLoadUsageNeeded();

            AccountData GetSelectedAccount();

            void DisableViewBillButton();

            void EnableViewBillButton();

            void ShowAmountDueFailed();

            void SetUsageData(UsageHistoryData data);

            void SetSMUsageData(SMUsageHistoryData data);

            UsageHistoryData GetUsageHistoryData();

            SMUsageHistoryData GetSMUsageHistoryData();

            void HideEnergyTipsShimmerView();

            void ShowEnergyTipsView(List<EnergySavingTipsModel> list);

            bool GetIsMDMSDown();

            void SetISMDMSDown(bool flag);

            bool GetIsSMAccount();

            void ShowSMStatisticCard();

            void OnSetBackendTariffDisabled(bool flag);

            void ByZoomDayView();

            void ShowNewAccountView(string contentTxt);

            void ShowBillDetails(AccountData accountData, List<AccountChargeModel> selectedAccountChargesModelList);

            bool GetIsREAccount();

            void OnShowDashboardFragmentTutorialDialog();

            void SetMDMSDownMessage(SMUsageHistoryResponse response);

            void CheckSMRAccountValidaty();

            void SetMDMSDownRefreshMessage(SMUsageHistoryResponse response);

            void OnShowPlannedDowntimeScreen(string contentTxt);

            void ShowEnergyBudgetSuccess();

            void ShowErrorMessageResponse(string displayMessage);

            void UpdateEnergyBudgetLocal(string v, string accnum);

            void GetFeedbackTwoQuestions(GetRateUsQuestionResponse questionRespone);

            void ShowFeedBackPageRating();

            void CheckOnPaperFromBillRendering();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action by Kwh
            /// </summary>
            void OnByKwh();

            /// <summary>
            /// Action by RM
            /// </summary>
            void OnByRM();

            /// <summary>
            /// Action to navigate to view bill
            /// </summary>
            void OnViewBill(AccountData selectedAccount);

            /// <summary>
            /// Action to navigate to view bill details
            /// </summary>
            void OnViewBillDetails(AccountData selectedAccount);

            /// <summary>
            /// Action to navigate to pay
            /// </summary>
            void OnPay();

            /// <summary>
            /// Action on Tap refresh
            /// </summary>
            void OnTapRefresh();

            /// <summary>
            /// Action to navigate to notification
            /// </summary>
            void OnNotification();

            /// <summary>
            /// Action to navigate to learn more
            /// </summary>
            void OnLearnMore();

            bool IsOwnedSMRLocal(string accountNumber);

            bool IsBillingAvailable();

            Task OnRandomizeEnergyTipsView(List<EnergySavingTipsModel> list);


            List<EnergySavingTipsModel> OnLoadEnergySavingTipsShimmerList(int count);

            void OnByDay();

            void OnByMonth();

            void OnByZoom();

            List<NewAppModel> OnGeneraNewAppTutorialList();

            void SaveEnergyBudgetAmmount(string accountNum, int text);
        }
    }
}
