using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.Base.MVP;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.SelectFeedbackType.MVP
{
    public class SelectFeedbackTypeContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show selected feedback type success
            /// </summary>
            /// <param name="feedbackType">FeedbackType</param>
            void ShowSelectedSuccess(FeedbackType feedbackType);

            /// <summary>
            /// Show feedback type list
            /// </summary>
            /// <param name="feedbackTypeList">List<paramref name="FeedbackType"/></param>
            void ShowList(List<FeedbackType> feedbackTypeList);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to select feedback type
            /// </summary>
            /// <param name="feedbackType">FeedbackType</param>
            void OnSelect(FeedbackType feedbackType);
        }
    }
}