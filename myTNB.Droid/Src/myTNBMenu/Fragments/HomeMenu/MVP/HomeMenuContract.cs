using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuContract
    {
        public interface IHomeMenuView
        {
            void OnUpdateAccountListChanged(bool isSearchSubmit);
            void SetAccountListCards(List<SummaryDashBoardDetails> accountList);
            void UpdateAccountListCards(List<SummaryDashBoardDetails> accountList);
        }

        public interface IHomeMenuPresenter
        {
            Constants.GREETING GetGreeting();
            string GetAccountDisplay();
            void LoadAccounts();
        }

        public interface IHomeMenuService
        {
            Task<SummaryDashBoardResponse> GetLinkedSummaryInfo(SummaryDashBordRequest request);
        }
    }
}
