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
using myTNB_Android.Src.AppLaunch.Models;

namespace myTNB_Android.Src.SelectFeedbackState.MVP
{
    public class SelectFeedbackStateContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show selected feedback state success
            /// </summary>
            /// <param name="feedbackState">FeedbackState</param>
            void ShowSelectedSuccess(FeedbackState feedbackState);

            /// <summary>
            /// Show feedback state list
            /// </summary>
            /// <param name="feedbackStateList">List<paramref name="FeedbackState"/></param>
            void ShowList(List<FeedbackState> feedbackStateList);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to select feedback state
            /// </summary>
            /// <param name="feedbackState">FeedbackState</param>
            void OnSelect(FeedbackState feedbackState);
        }
    }
}