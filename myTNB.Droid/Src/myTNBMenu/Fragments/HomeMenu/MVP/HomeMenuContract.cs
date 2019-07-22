using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuContract
    {
        public interface IHomeMenuView
        {
            void OnUpdateAccountListChanged(bool isSearchSubmit);
            void SetAccountListCards(List<SummaryDashBoardDetails> accountList);
            void UpdateAccountListCards(List<SummaryDashBoardDetails> accountList);
            void SetMyServiceRecycleView();
            void SetNewFAQRecycleView();
            void SetMyServiceResult(List<MyService> list);
            void SetNewFAQResult(List<NewFAQ> list);
            string GetDeviceId();
            void ShowMyServiceRetryOptions(string msg);
        }

        public interface IHomeMenuPresenter
        {
            Constants.GREETING GetGreeting();
            string GetAccountDisplay();
            void LoadAccounts();
            void LoadBatchSummaryAccounts();
            void InitiateService();
            Task InitiateMyService();
            Task InitiateNewFAQ();
            Task RetryMyService();
            List<MyService> LoadShimmerServiceList(int count);
            List<NewFAQ> LoadShimmerFAQList(int count);
        }

        public interface IHomeMenuService
        {
            Task<SummaryDashBoardResponse> GetLinkedSummaryInfo(SummaryDashBordRequest request);
            Task<GetServicesResponse> GetServices(GetServiceRequests request);
        }
    }
}
