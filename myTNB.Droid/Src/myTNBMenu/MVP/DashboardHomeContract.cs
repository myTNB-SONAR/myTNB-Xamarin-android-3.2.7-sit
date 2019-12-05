using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using System;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.MVP
{
	public class DashboardHomeContract
	{
		public interface IView : IBaseView<IUserActionsListener>
		{
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
            /// Set toolbar title
            /// </summary>
            /// <param name="stringResourceId">integer</param>
            void SetToolbarTitle(int stringResourceId);

            void SetAccountToolbarTitle(string accountName);

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
			/// Set the account name
			/// </summary>
			/// <param name="accountName">string</param>
			void SetAccountName(string accountName);

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
            /// Shows unread promotions
            /// </summary>
            void ShowUnreadPromotions(bool flag);

            void ShowUnreadPromotions();

            /// <summary>
            /// Hides unread promotions
            /// </summary>
            void HideUnreadPromotions(bool flag);

            void HideUnreadPromotions();

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


			void ShowHomeDashBoard();


			void ShowBackButton(bool flag);


			string GetDeviceId();

			void BillsMenuAccess();

			void BillsMenuAccess(AccountData selectedAccount);

			void BillsMenuRefresh(AccountData accountData);

            void SetDashboardHomeCheck();

            void ShowHideActionBar(bool flag);

            void ShowToBeAddedToast();

            void SetInnerDashboardToolbarBackground();

            void UnsetToolbarBackground();

            void ShowNMREChart(UsageHistoryResponse response, AccountData selectedAccount, string errorCode, string errorMsg);

            void ShowSMChart(SMUsageHistoryResponse response, AccountData selectedAccount);

            void ShowUnreadRewards(bool flag);

            void ShowUnreadRewards();

            void HideUnreadRewards(bool flag);

            void HideUnreadRewards();

            void ShowRewardsMenu();
        }

        public interface IUserActionsListener : IBasePresenter
		{
			/// <summary>
			/// Action to logout NOT USED
			/// </summary>
			void Logout();

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

            int CheckCurrentDashboardMenu();

            void BillMenuStartRefresh();
        }
	}
}