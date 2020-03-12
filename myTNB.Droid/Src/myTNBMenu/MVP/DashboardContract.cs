using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using System;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.MVP
{
    public class DashboardContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show the bar charts
            /// </summary>
            /// <param name="data">UsageHistoryData</param>
            /// <param name="selectedAccount">AccountData</param>
            void ShowChart(UsageHistoryData data, AccountData selectedAccount);

            /// <summary>
            /// Show the bar charts with error msg
            /// </summary>
            /// <param name="data">UsageHistoryData</param>
            /// <param name="selectedAccount">AccountData</param>
            void ShowChartWithError(UsageHistoryData data, AccountData selectedAccount, String errorCode, String errorMsg);

            /// <summary>
            /// Show the bar charts
            /// </summary>
            /// <param name="data">UsageHistoryData</param>
            /// <param name="selectedAccount">AccountData</param>
            void ShowSMChart(SMUsageHistoryData data, AccountData selectedAccount);

            /// <summary>
            /// Show the bar charts
            /// </summary>
            /// <param name="data">UsageHistoryData</param>
            /// <param name="selectedAccount">AccountData</param>
            void ShowSMChartWithError(SMUsageHistoryData data, AccountData selectedAccount, bool noSMData);

            /// <summary>
            /// Shows the bill menu
            /// </summary>
            /// <param name="selectedAccount">AccountData</param>
            void ShowBillMenu(AccountData selectedAccount);

            /// <summary>
            /// Enable bill menu
            /// </summary>
            void EnableBillMenu();

            /// <summary>
            /// Disable bill menu
            /// </summary>
            void DisableBillMenu();

            /// <summary>
            /// Show feedback menu
            /// </summary>
            void ShowFeedbackMenu();

            /// <summary>
            /// Show promotions menu
            /// </summary>
            /// <param name="weblink">Weblink</param>
            void ShowPromotionsMenu(Weblink weblink);

            /// <summary>
            /// Show more menu
            /// </summary>
            void ShowMoreMenu();

            /// <summary>
            /// Show select account activity
            /// </summary>
            void ShowSelectSupplyAccount();

            /// <summary>
            /// Show no account in dashboard
            /// </summary>
            void ShowNoAccountDashboardChartMenu();

            /// <summary>
            /// Show no account in bill menu
            /// </summary>
            void ShowNoAccountBillMenu();

            /// <summary>
            /// Show no internet connection in dashboard
            /// </summary>
            /// <param name="accountName">string</param>
            void ShowOwnerDashboardNoInternetConnection(string accountName);

            /// <summary>
            /// Show no internet connection in bill menu
            /// </summary>
            /// <param name="selectedAccount"></param>
            void ShowOwnerBillsNoInternetConnection(AccountData selectedAccount);

            /// <summary>
            /// NOT USED
            /// </summary>
            void ShowOwnerNonSmartMeterDay();

            /// <summary>
            /// NOT USED
            /// </summary>
            void ShowOwnerNonSmartMeterMonth();

            /// <summary>
            /// Show owner data
            /// </summary>
            /// <param name="selectedAccount">AccountData</param>
            void ShowNonOWner(AccountData selectedAccount);

            /// <summary>
            /// Set toolbar title
            /// </summary>
            /// <param name="stringResourceId">integer</param>
            void SetToolbarTitle(int stringResourceId);

            /// <summary>
            /// Set the active bottom menu
            /// </summary>
            /// <param name="resourceId">integer</param>
            void SetBottomMenu(int resourceId);

            /// <summary>
            /// Enable dropdown
            /// </summary>
            /// <param name="enable">Boolean</param>
            void EnableDropDown(Boolean enable);

            /// <summary>
            /// Shows the leaf account icon in drop down
            /// Enable dropdown
            /// </summary>
            void ShowREAccount(Boolean enable);

            /// <summary>
            /// Retrieves the selected account data
            /// </summary>
            /// <returns>AccountData</returns>
            AccountData GetSelectedAccountData();

            /// <summary>
            /// Set the account name
            /// </summary>
            /// <param name="accountName">string</param>
            void SetAccountName(string accountName);

#if STUB || DEVELOP
            string GetUsageHistoryStub();

            string GetAccountDetailsStub(string accNum);
#endif
            /// <summary>
            /// Show the prelogin
            /// </summary>
            void ShowPreLogin();

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Hide account name
            /// </summary>
            void HideAccountName();

            /// <summary>
            /// Show account name
            /// </summary>
            void ShowAccountName();

            /// <summary>
            /// Show notification count
            /// </summary>
            /// <param name="count">integer</param>
            void ShowNotificationCount(int count);

            /// <summary>
            /// Shows unread promotions
            /// </summary>
            void ShowUnreadPromotions();

            /// <summary>
            /// Hides unread promotions
            /// </summary>
            void HideUnreadPromotions();

            /// <summary>
            /// Hides unread promotions
            /// </summary>
            void ShowError(string errorMsg);


            /// <summary>
            /// return saved time stamp to view
            /// </summary>
            void OnSavedTimeStamp(string savedTimeStamp);

            /// <summary>
            /// Show promotion timestamp success/error
            /// Call get promotion service to get latest promotion list
            /// </summary>
            /// <param name="success"></param>
            void ShowPromotionTimestamp(bool success);

            /// <summary>
            /// Show promotion success/error
            /// </summary>
            /// <param name="success"></param>
            void ShowPromotion(bool success);


            /// <summary>
            /// Show downtime 
            /// </summary>
            /// <param name="success"></param>
            void ShowDownTimeView(string system, string accountName);


            void ShowSummaryDashBoard();


            void ShowBackButton(bool flag);


            string GetDeviceId();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to logout NOT USED
            /// </summary>
            void Logout();

            /// <summary>
            /// Action to navigate to notification list
            /// </summary>
            void OnNotificationCount();

            /// <summary>
            /// Action to navigate to select accuont
            /// </summary>
            void SelectSupplyAccount();

            /// <summary>
            /// Action to select menu
            /// </summary>
            /// <param name="resourceId">integer</param>
            void OnMenuSelect(int resourceId);

            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            /// <summary>
            /// Handles actions triggered by real time data
            /// </summary>
            void OnValidateData();

            /// <summary>
            /// Get saved timestamp from database
            /// </summary>
            void GetSavedPromotionTimeStamp();

            /// <summary>
            /// Sitecore service call to get promotion timestamp
            /// </summary>
            /// <returns></returns>
            Task OnGetPromotionsTimeStamp();

            /// <summary>
            /// Sitecore service call to get latest promotions 
            /// </summary>
            /// <returns></returns>
            Task OnGetPromotions();

            void OnTapToRefresh();
        }
    }
}