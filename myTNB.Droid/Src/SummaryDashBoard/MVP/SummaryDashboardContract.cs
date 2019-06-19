using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.SummaryDashBoard.Models;
using System.Collections.Generic;

namespace myTNB_Android.Src.SummaryDashBoard.MVP
{
    public class SummaryDashboardContract
    {
        public SummaryDashboardContract()
        {

        }

        public enum eGreeting
        {
            MORNING,
            AFTERNOON,
            EVENING
        }

        public interface IView : IBaseView<ISummaryDashBoardListener>
        {
            void LoadREAccountData(List<SummaryDashBoardDetails> summaryDetails);

            void LoadNormalAccountData(List<SummaryDashBoardDetails> summaryDetails);

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgressDialog();

            void IsLoadMoreButtonVisible(bool isVisible);

            void SetUserName(string greetingText);

            bool HasNetworkConnection();
            void ShowNotification();
            void ShowNoInternetSnackbar();
            void SetGreetingImageAndText(eGreeting greeting, string text);

            void ShowRefreshSummaryDashboard(bool flag);

        }


        public interface ISummaryDashBoardListener : IBasePresenter
        {

            void FetchUserData();

            void FetchAccountSummary(bool makeSummaryApiCall = false);

            void DoLoadMoreAccount();

            void OnNotification();

            void EnableLoadMore();

            void RefreshAccountSummary();

            void LoadEmptySummaryDetails();
        }


    }
}
