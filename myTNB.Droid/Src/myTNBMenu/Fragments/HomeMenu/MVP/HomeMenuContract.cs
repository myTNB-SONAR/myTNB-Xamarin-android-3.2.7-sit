﻿using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.MyTNBService.Response;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuContract
    {
        public interface IHomeMenuView
        {
            void OnUpdateAccountListChanged(bool isSearchSubmit);
            void SetAccountListCards(List<SummaryDashBoardDetails> accountList);
            void UpdateAccountListCards(List<SummaryDashBoardDetails> accountList);
            void SetAccountListCardsFromLocal(List<SummaryDashBoardDetails> accountList);
            void ShowAccountDetails(string accountNumber);
            void SetMyServiceRecycleView();
            void SetNewFAQRecycleView();
            void SetMyServiceResult(List<MyService> list);
            void SetNewFAQResult(List<NewFAQ> list);
            string GetDeviceId();
            void ShowMyServiceRetryOptions(string msg);
            void OnSavedTimeStamp(string savedTimeStamp);
            void ShowFAQTimestamp(bool success);
            AccountsRecyclerViewAdapter GetAccountAdapter();

            void CheckSSMRMeterReadingTimeStamp();

            void OnSavedSSMRMeterReadingTimeStamp(string mSavedTimeStamp);

            void OnSavedSSMRMeterReadingThreePhaseTimeStamp(string mSavedTimeStamp);

            void CheckSSMRMeterReadingThreePhaseTimeStamp();

            void ShowRefreshScreen(string contentMsg, string buttonMsg);

            void UpdateCurrentSMRAccountList();

            void UpdateEligibilitySMRAccountList();

            void OnSavedEnergySavingTipsTimeStamp(string mSavedTimeStamp);

            void CheckEnergySavingTipsTimeStamp();

            void UpdateQueryListing(string searchText);

            void IsLoadMoreButtonVisible(bool isVisible, bool isRotate);

            void SetHeaderActionVisiblity(List<SummaryDashBoardDetails> accountList);

            void IsMyServiceLoadMoreButtonVisible(bool isVisible, bool isRotate);

            void SetBottomLayoutBackground(bool isMyServiceExpand);

            void ShowProgressDialog();

            void HideProgressDialog();

            void ShowBillErrorSnackBar();

            void ShowBillPDF(AccountData selectedAccountData, GetBillHistoryResponse.ResponseData selectedBill = null);

            void ShowNotificationCount(int count);

            void OnSavedSSMRMeterReadingNoOCRTimeStamp(string mSavedTimeStamp);

            void CheckSSMRMeterReadingNoOCRTimeStamp();

            void OnSavedSSMRMeterReadingThreePhaseNoOCRTimeStamp(string mSavedTimeStamp);

            void CheckSSMRMeterReadingThreePhaseNoOCRTimeStamp();

            void OnShowHomeMenuFragmentTutorialDialog();

            void HomeMenuCustomScrolling(int xPosition);

            void OnSearchOutFocus(bool isSearchLayoutInRange);

            bool CheckIsScrollable();

            void ResetNewFAQScroll();

            int OnGetEndOfScrollView();

            int CheckNewFaqList();

            int CheckMyServiceList();

            void ShowSearchAction(bool isShow);

            void CheckSearchEditAction();

            void RestartHomeMenu();

        }

        public interface IHomeMenuPresenter
        {
            Constants.GREETING GetGreeting();
            string GetAccountDisplay();
            void LoadAccounts();
            void InitiateService();
            Task InitiateMyService();
            Task RetryMyService();
            List<MyService> LoadShimmerServiceList(int count);
            List<NewFAQ> LoadShimmerFAQList(int count);
            void GetSavedNewFAQTimeStamp();
            Task OnGetFAQTimeStamp();
            void ReadNewFAQFromCache();
            Task OnGetFAQs();
            void LoadLocalAccounts();
			void LoadSummaryDetailsInBatch(List<string> accountNumbers);

            void GetSmartMeterReadingWalkthroughtTimeStamp();

            Task OnGetSmartMeterReadingWalkthroughtTimeStamp();

            Task OnGetSSMRMeterReadingScreens();

            void GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();

            Task OnGetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();

            Task OnGetSSMRMeterReadingThreePhaseScreens();

            void GetEnergySavingTipsTimeStamp();

            Task OnGetEnergySavingTipsTimeStamp();

            Task OnGetEnergySavingTips();

            Task OnSetEnergySavingTipsToCache();

            void OnCancelToken();

            void DoLoadMoreAccount();

            void RefreshAccountSummary();

            void LoadQueryAccounts(string searchText);

            void DoMySerivceLoadMoreAccount();

            void InitiateMyServiceRefresh();

            void LoadingBillsHistory(CustomerBillingAccount selectedAccount);

            void GetUserNotifications();

            void SetQueryClose();

            void GetSmartMeterReadingWalkthroughtNoOCRTimeStamp();

            Task OnGetSmartMeterReadingWalkthroughtNoOCRTimeStamp();

            Task OnGetSSMRMeterReadingScreensNoOCR();

            void GetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();

            Task OnGetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();

            Task OnGetSSMRMeterReadingThreePhaseScreensNoOCR();

            List<NewAppModel> OnGeneraNewAppTutorialList();

            void OnCheckToCallHomeMenuTutorial();

            void RestoreCurrentMyServiceState();

            void RestoreCurrentAccountState();

            void RestoreQueryAccounts();

            void OnCheckMyServiceState();
        }

        public interface IHomeMenuService
        {
            Task<SummaryDashBoardResponse> GetLinkedSummaryInfo(SummaryDashBordRequest request, System.Threading.CancellationToken token);
            Task<GetServicesResponse> GetServices(GetServiceRequests request);
            Task<AccountSMRStatusResponse> GetSMRAccountStatus(AccountsSMRStatusRequest request);
            Task<GetIsSmrApplyAllowedResponse> GetIsSmrApplyAllowed(GetIsSmrApplyAllowedRequest request);
            Task<SummaryDashBoardResponse> GetLinkedSummaryInfoQuery(SummaryDashBordRequest request, System.Threading.CancellationToken token);
        }
    }
}
