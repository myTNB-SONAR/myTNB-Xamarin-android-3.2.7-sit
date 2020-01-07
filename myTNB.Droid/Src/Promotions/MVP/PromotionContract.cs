using myTNB_Android.Src.Base.MVP;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Promotions.MVP
{
    public class PromotionContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// show progress bar
            /// </summary>
            void ShowProgressBar();

            /// <summary>
            /// Hide progress bar
            /// </summary>
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
            void ShowPromotionTimestamp(bool success);

            /// <summary>
            /// Show promotion success/error
            /// </summary>
            /// <param name="success"></param>
            void ShowPromotion(bool success);

            void OnGetPromotionTimestamp();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Get saved timestamp from database
            /// </summary>
            void GetSavedPromotionTimeStamp();

            /// <summary>
            /// Sitecore service call to get promotion timestamp
            /// </summary>
            /// <returns></returns>
            Task OnGetPromotionsTimeStamp();

            /// <summary>
            /// Sitecore service call to get latest promotions 
            /// </summary>
            /// <returns></returns>
            Task OnGetPromotions();

            Task OnRecheckPromotionStatus();

        }
    }
}