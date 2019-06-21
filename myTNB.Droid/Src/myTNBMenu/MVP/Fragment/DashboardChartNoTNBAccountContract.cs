using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardChartNoTNBAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show add account
            /// </summary>
            void ShowAddAccount();

            /// <summary>
            /// Show notification
            /// </summary>
            void ShowNotification();

            /// <summary>
            /// Return connectivity
            /// </summary>
            /// <returns>bool</returns>
            bool HasInternet();

            /// <summary>
            /// Show no internet snackbar
            /// </summary>
            void ShowNoInternetSnackbar();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate add account
            /// </summary>
            void OnAddAccount();

            /// <summary>
            /// Action to notification
            /// </summary>
            void OnNotification();
        }
    }
}