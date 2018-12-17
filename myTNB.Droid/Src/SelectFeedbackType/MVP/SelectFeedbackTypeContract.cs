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

namespace myTNB_Android.Src.SelectFeedbackType.MVP
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