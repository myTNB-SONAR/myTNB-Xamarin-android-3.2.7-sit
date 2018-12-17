﻿using System;
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
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;

namespace myTNB_Android.Src.ManageSupplyAccount.MVP
{
    public class ManageSupplyAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show remove progress dialog
            /// </summary>
            void ShowRemoveProgress();

            /// <summary>
            /// Show error message from api response
            /// </summary>
            /// <param name="error">string</param>
            void ShowErrorMessageResponse(string error);

            /// <summary>
            /// Hide remove progress dialog
            /// </summary>
            void HideRemoveProgress();

            /// <summary>
            /// Show update nickname
            /// </summary>
            void ShowUpdateNickname();


            /// <summary>
            /// Show update nickname success
            /// </summary>
            /// <param name="accountData">AccountData</param>
            void ShowUpdateSuccessNickname(AccountData accountData);

            /// <summary>
            /// Show removed account success
            /// </summary>
            void ShowSuccessRemovedAccount();

            /// <summary>
            /// Show initial nickname
            /// </summary>
            /// <param name="nickname">string</param>
            void ShowNickname(string nickname);

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
            /// <summary>
            /// Action on update nickname
            /// </summary>
            void OnUpdateNickname();

            /// <summary>
            /// Action to remove account
            /// </summary>
            /// <param name="accountData">AccountData</param>
            void OnRemoveAccount(AccountData accountData);

            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);
        }
    }
}