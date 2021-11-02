using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Bills.AccountStatement.MVP
{
    public class AccountStatementLoadingContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();
            void OnShowAccountStamentScreen(string pdfFilePath);
            void OnShowTimeOutScreen(bool isEmpty);
            void ShowRefreshView();
            void ShowLoadingView();
            void APICallHasFinished();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            void SetSelectedAccount(AccountData accountData);
            AccountData GetSelectedAccount();
            void SetPreferredMonths(string value);
            void RequestAccountStatement();
        }
    }
}
