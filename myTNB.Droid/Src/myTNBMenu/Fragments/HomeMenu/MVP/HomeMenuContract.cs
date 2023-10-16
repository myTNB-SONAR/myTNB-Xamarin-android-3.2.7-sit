using System.Collections.Generic;
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
using myTNB_Android.Src.MyHome.Model;

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
            //void StopShimmerDiscoverMore();
            void SetMyServicesResult(List<MyServiceModel> list);
            void SetNewFAQResult(List<NewFAQ> list);
            void SetDiscoverResult(bool IsAccountDBREligible);
            string GetDeviceId();
            void ShowMyServiceRetryOptions(string msg);
            void OnSavedTimeStamp(string savedTimeStamp);
            void ShowFAQTimestamp(bool success);
            AccountsRecyclerViewAdapter GetAccountAdapter();

            void ShowRefreshScreen(bool isRefresh, string contentMsg, string buttonMsg);

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

            void HideNewFAQ();

            void HideDiscoverViewView();

            bool CheckNeedHelpHide();

            void SetMyServiceRefreshView(string contentTxt, string buttonTxt);

            void SetMyServiceHideView();

            void OnHideBottomView();

            bool OnGetIsRootTooltipShown();

            void ShowFAQFromHide();

            bool GetHomeTutorialCallState();

            void EBPopupActivity();

            void ShowDiscoverMoreLayout();

            void OnSetNotificationNewLabel(bool hasNoti, int noticount);

            void FilterCOMCLandNEM();

            void NavigateToSSMRPage();
            
            bool ClickQuickActionSMR();
        }

        public interface IHomeMenuPresenter
        {
            Constants.GREETING GetGreeting();
            string GetAccountDisplay();
            void SetDynaUserTAG();
            void LoadAccounts();
            void InitiateService();
            Task InitiateMyService();
            Task RetryMyService();
            List<MyServiceModel> LoadShimmerServiceList(int count);
            List<NewFAQ> LoadShimmerFAQList(int count);
            void GetSavedNewFAQTimeStamp();
            Task OnGetFAQTimeStamp();
            void ReadNewFAQFromCache();
            Task OnGetFAQs();
            void LoadLocalAccounts();
            void LoadSummaryDetailsInBatch(List<string> accountNumbers);

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

            List<NewAppModel> OnGeneraNewAppTutorialList();

            void OnCheckToCallHomeMenuTutorial();

            void RestoreCurrentMyServiceState();

            void RestoreCurrentAccountState();

            void RestoreQueryAccounts();

            void OnCheckMyServiceNewFAQState();

            bool GetIsMyServiceRefreshNeeded();

            bool GetIsAccountRefreshNeeded();

            void UpdateNewFAQCompleteState();

            bool GetIsLoadedHomeDone();

            void OnCheckNewFAQState();

            Task OnCheckSMRAccount(List<string> smrAccountListOwnerOnly, string r);

            Task GetIsSmrApplyAllowedService(List<string> customerBillingAccountListOwnerOnly);

            //void UserNotificationsCount();
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
