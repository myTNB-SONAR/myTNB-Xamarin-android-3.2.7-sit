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
using myTNB_Android.Src.Base.Models;

namespace myTNB_Android.Src.FeedbackDetails.MVP
{
    public class FeedbackDetailsContract
    {

        public class BillRelated
        {
            public interface IView : IBaseView<IUserActionsListener>
            {

                /// <summary>
                /// Shows input data
                /// </summary>
                /// <param name="feedbackId">string</param>
                /// <param name="feedbackStatus">string status description</param>
                /// <param name="feedbackCode">string status code</param>
                /// <param name="dateTime">string</param>
                /// <param name="accountNoName">string</param>
                /// <param name="feedback">string</param>

                void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string accountNoName, string feedback);

                /// <summary>
                /// Show/Add to adapter
                /// </summary>
                /// <param name="list">List<paramref name="AttachedImage"/></param>
                void ShowImages(List<AttachedImage> list);
            }

            public interface IUserActionsListener : IBasePresenter
            {

            }
        }

        public class FaultyLamps
        {
            public interface IView : IBaseView<IUserActionsListener>
            {
                /// <summary>
                /// Shows input data
                /// </summary>
                /// <param name="feedbackId">string</param>
                /// <param name="feedbackStatus">string status description</param>
                /// <param name="feedbackCode">string status code</param>
                /// <param name="dateTime">string</param>
                /// <param name="state">string</param>
                /// <param name="location">string</param>
                /// <param name="pole_no">string</param>
                /// <param name="feedback">string</param>
                void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string state , string location , string pole_no , string feedback);

                /// <summary>
                /// Show/Add to adapter
                /// </summary>
                /// <param name="list">List<paramref name="AttachedImage"/></param>
                void ShowImages(List<AttachedImage> list);
            }

            public interface IUserActionsListener : IBasePresenter
            {

            }
        }

        public class Others
        {
            public interface IView : IBaseView<IUserActionsListener>
            {
                /// <summary>
                /// Shows input data
                /// </summary>
                /// <param name="feedbackId">string</param>
                /// <param name="feedbackStatus">string status description</param>
                /// <param name="feedbackCode">string status code</param>
                /// <param name="dateTime">string</param>
                /// <param name="feedbackType">string</param>
                /// <param name="feedback">string</param>
                void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string feedbackType, string feedback);

                /// <summary>
                /// Show/Add to adapter
                /// </summary>
                /// <param name="list">List<paramref name="AttachedImage"/></param>
                void ShowImages(List<AttachedImage> list);
            }

            public interface IUserActionsListener : IBasePresenter
            {

            }
        }


    }
}