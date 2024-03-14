using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Database.Model;
using System.Threading.Tasks;

namespace myTNB.Android.Src.PreLogin.MVP
{
    public class PreLoginContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows Login Activity
            /// </summary>
            void ShowLogin();

            /// <summary>
            /// Shows RegisterActivity
            /// </summary>
            void ShowRegister();

            /// <summary>
            /// Show feedback
            /// </summary>
            void ShowFeedback();

            /// <summary>
            /// Show pre login promotional
            /// </summary>
            /// <param name="success">bool</param>
            void ShowPreLoginPromotion(bool success);

            /// <summary>
            /// Show find us
            /// </summary>
            void ShowFindUS();

            /// <summary>
            /// Show Check Status
            /// </summary>
            void ShowCheckStatus();



            /// <summary>
            /// Show call us
            /// </summary>
            /// <param name="entity">WeblinkEntity</param>
            void ShowCallUs(WeblinkEntity entity);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Actions performed by user to navigate to login
            /// </summary>
            void NavigateToLogin();

            /// <summary>
            /// Actions performed by user to navigate to Register
            /// </summary>
            void NavigateToRegister();

            /// <summary>
            /// Action to navigate to find us
            /// </summary>
            void NavigateToFindUs();

            /// <summary>
            /// Action to navigate to call us
            /// </summary>
            void NavigateToCallUs();

            /// <summary>
            /// Action to navigate to check status
            /// </summary>
            void NavigateToCheckStatus();



            /// <summary>
            /// Action to navigate to feedback
            /// </summary>
            void NavigateToFeedback();

            /// <summary>
            /// Action to get pre login promo
            /// </summary>
            /// <returns>Task</returns>
            Task OnGetPreLoginPromo();

        }
    }
}