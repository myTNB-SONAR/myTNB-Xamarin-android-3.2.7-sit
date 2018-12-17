using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.MVP;
using System.Threading.Tasks;

namespace myTNB_Android.Src.WalkThrough.MVP
{
    public class WalkThroughContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {


            /// <summary>
            /// Calls the viewpager and show next walkthrough screens
            /// </summary>
            /// <param name="index"></param>
            void ShowNext(int index);
            /// <summary>
            /// Shows the prelogin screen either by skipping or last index of next called by view
            /// </summary>
            void ShowPreLogin();


            /// <summary>
            /// Shows the Done text 
            /// </summary>
            void ShowDone();

            /// <summary>
            /// Get Current Index
            /// </summary>
            /// <returns>integer value</returns>
            int GetCurrentItem();

            /// <summary>
            /// Get total items of the viewpager
            /// </summary>
            /// <returns></returns>
            int GetTotalItems();

            void ShowWalkThroughData(bool success);

            void OnSavedTimeStampRecievd(string timestamp);

            void OnTimeStampRecieved(string timestamp);

            void OnSiteCoreServiceFailed(string message);
        }

        public interface IUserActionsListener : IBasePresenter
        {


            /// <summary>
            /// Shows the prelogin screen either by skipping or last index of next called by presenter
            /// </summary>
            void NavigatePrelogin();


            /// <summary>
            /// Calls the view to show next walkthrough screens
            /// </summary>
            void NavigateNextScreen();

            /// <summary>
            /// Called by the page selection in viewPager
            /// </summary>
            /// <param name="position"></param>
            void OnPageSelected(int position);

            void OnSkip();

            /// <summary>
            /// Get walkthrough data from sitecore
            /// On success save walkthrough data in local DB
            /// </summary>
            /// <returns></returns>
            Task OnGetWalkThroughData();

            /// <summary>
            /// Get sitecore time stamp to check data is updated or not
            /// </summary>
            /// <returns></returns>
            Task OnGetTimeStamp();
        }
    }
}