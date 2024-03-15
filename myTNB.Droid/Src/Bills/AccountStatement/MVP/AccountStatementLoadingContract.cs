using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;

namespace myTNB.AndroidApp.Src.Bills.AccountStatement.MVP
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
            void OnShowTimeOutScreen();
            void ShowRefreshView();
            void ShowLoadingView();
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
