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

			void ShowHomeDBRCard(bool IsAccountDBREligible);

			void ShowWhatsNewMenu();

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

            /// <summary>
            /// Removed Header dropdown
            /// </summary>
            void RemoveHeaderDropDown();

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

            void ShowUnreadWhatsNew(bool flag);

            void ShowUnreadWhatsNew();

            void HideUnreadWhatsNew(bool flag);

            void HideUnreadWhatsNew();

			void ShowHomeDashBoard();


			void ShowBackButton(bool flag);


			string GetDeviceId();

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

            void OnCheckRewardTab();

            void OnDataSchemeShow();

            void OnCheckUserReward(bool isSitecoreApiFailed);

            void OnCheckUserRewardApiFailed();

            void OnSavedSSMRMeterReadingNoOCRTimeStamp(string mSavedTimeStamp);

            void CheckSSMRMeterReadingNoOCRTimeStamp();

            void OnSavedSSMRMeterReadingThreePhaseNoOCRTimeStamp(string mSavedTimeStamp);

            void CheckSSMRMeterReadingThreePhaseNoOCRTimeStamp();

            void CheckSSMRMeterReadingTimeStamp();

            void OnSavedSSMRMeterReadingTimeStamp(string mSavedTimeStamp);

            void OnSavedSSMRMeterReadingThreePhaseTimeStamp(string mSavedTimeStamp);

            void CheckSSMRMeterReadingThreePhaseTimeStamp();

            void OnCheckUserWhatsNew(bool isSitecoreApiFailed);

            void OnCheckWhatsNewTab();

            bool GetAlreadyStarted();

            void SetAlreadyStarted(bool flag);

			void OnResetEppTooltip();

			void OnResetWhereIsMyAccNumber();

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

			void OnTapToRefresh();

            int CheckCurrentDashboardMenu();

            void OnResumeUpdateRewardUnRead();

            void OnStartRewardThread();

            Task OnGetUserRewardList();

            void UpdateRewardRead(string itemID, bool flag);

            void GetSmartMeterReadingWalkthroughtNoOCRTimeStamp();

            Task OnGetSmartMeterReadingWalkthroughtNoOCRTimeStamp();

            Task OnGetSSMRMeterReadingScreensNoOCR();

            void GetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();

            Task OnGetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();

            Task OnGetSSMRMeterReadingThreePhaseScreensNoOCR();

            void GetSmartMeterReadingWalkthroughtTimeStamp();

            Task OnGetSmartMeterReadingWalkthroughtTimeStamp();

            Task OnGetSSMRMeterReadingScreens();

            void GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();

            Task OnGetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();

            Task OnGetSSMRMeterReadingThreePhaseScreens();

            void OnResetRewardPromotionThread();

            void OnResumeUpdateWhatsNewUnRead();

            void OnStartWhatsNewThread();

            void UpdateWhatsNewRead(string itemID, bool flag);

            void UpdateTrackDashboardMenu(int resId);

            void OnLoadMoreMenu();

			bool GetIsWhatsNewDialogShowNeed();

			void SetIsWhatsNewDialogShowNeed(bool flag);

			Task OnGetEPPTooltipContentDetail();

			Task OnWhereIsMyAccNumberContentDetail();
			Task OnGetBillTooltipContent();

		}
	}
}