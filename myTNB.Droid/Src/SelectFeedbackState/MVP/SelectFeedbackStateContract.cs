using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Base.MVP;
using System.Collections.Generic;

namespace myTNB.Android.Src.SelectFeedbackState.MVP
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