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
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.SubmittedNewEnquiry.MVP
{
   public   class SubmittedNewEnquiryContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string accountNoName, string feedback);

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

            void Start();

        }
    }
}