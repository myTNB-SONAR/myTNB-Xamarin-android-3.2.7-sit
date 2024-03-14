using System;
using System.Collections.Generic;
using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.ManageCards.Models;
using myTNB.Android.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB.Android.Src.MyLearnMoreAboutTnb.MVP
{
    public class MyLearnMoreContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
           
            /// <summary>
            /// Shows a cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">the returned exception</param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException">the returned exception</param>
            void ShowRetryOptionsApiException(ApiException apiException);


            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowRetryOptionsUnknownException(Exception exception);
        }

        public interface IUserActionsListener : IBasePresenter
        {

        }
    }
}
