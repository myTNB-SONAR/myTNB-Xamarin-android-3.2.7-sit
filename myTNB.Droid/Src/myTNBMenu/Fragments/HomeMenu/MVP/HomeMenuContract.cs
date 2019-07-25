using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;

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
        }

        public interface IHomeMenuPresenter
        {
            Constants.GREETING GetGreeting();
            string GetAccountDisplay();
            void LoadAccounts();
            void LoadBatchSummaryAccounts();
            void InitiateService();
            Task InitiateMyService();
            Task RetryMyService();
            List<MyService> LoadShimmerServiceList(int count);
            List<NewFAQ> LoadShimmerFAQList(int count);
            void GetSavedNewFAQTimeStamp();
            Task OnGetFAQTimeStamp();
            void ReadNewFAQFromCache();
            Task OnGetFAQs();
            void LoadBatchSummarDetailsByIndex(int batchIndex);
            void LoadLocalAccounts();
			void LoadSummaryDetailsInBatch(List<string> accountNumbers);
		}

        public interface IHomeMenuService
        {
            Task<SummaryDashBoardResponse> GetLinkedSummaryInfo(SummaryDashBordRequest request);
            Task<GetServicesResponse> GetServices(GetServiceRequests request);
        }
    }
}
