using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Base.MVP;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB.AndroidApp.Src.FeedbackDetails.MVP
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

                void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string accountNoName, string feedback , List<FeedbackUpdate> feedbackUpdateList, string name, string email, string mobile, int? relationShip, string relationshipDesc , bool? isOwner,string EnquiryName);

                /// <summary>
                /// Show/Add to adapter
                /// </summary>
                /// <param name="list">List<paramref name="AttachedImage"/></param>
                void ShowImages(List<AttachedImage> list);

                void ShowProgressDialog();

                void HideProgressDialog();
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
                void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string state, string location, string pole_no, string feedback);

                /// <summary>
                /// Show/Add to adapter
                /// </summary>
                /// <param name="list">List<paramref name="AttachedImage"/></param>
                void ShowImages(List<AttachedImage> list);

                void ShowProgressDialog();

                void HideProgressDialog();
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

                void ShowProgressDialog();

                void HideProgressDialog();
            }

            public interface IUserActionsListener : IBasePresenter
            {

            }
        }


    }
}