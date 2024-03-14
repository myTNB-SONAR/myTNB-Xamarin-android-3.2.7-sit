using myTNB.Android.Src.Base.MVP;
using System.Threading.Tasks;

namespace myTNB.Android.Src.FAQ.MVP
{
    public class FAQContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowProgressBar();

            void HideProgressBar();

            /// <summary>
            /// return saved time stamp to view
            /// </summary>
            void OnSavedTimeStamp(string savedTimeStamp);

            /// <summary>
            /// Show promotion timestamp success/error
            /// Call get promotion service to get latest promotion list
            /// </summary>
            /// <param name="success"></param>
            void ShowFAQTimestamp(bool success);

            void ShowFAQ(bool success);
        }

        public interface IUserActionsListener : IBasePresenter
        {

            /// <summary>
            /// Get saved timestamp from database
            /// </summary>
            void GetSavedFAQTimeStamp();

            /// <summary>
            /// Sitecore service call to get promotion timestamp
            /// </summary>
            /// <returns></returns>
            Task OnGetFAQTimeStamp();

            Task OnGetFAQs();

        }
    }
}