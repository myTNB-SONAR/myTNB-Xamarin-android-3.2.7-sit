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

namespace myTNB_Android.Src.ResetPasswordSuccess.MVP
{
    public class ResetPasswordSuccessContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows login
            /// </summary>
            void ShowLoginActivity();

            /// <summary>
            /// Shows previous activity
            /// </summary>
            void ShowBackActivity();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// User action on back key press
            /// </summary>
            void OnBackKeyPress();

            /// <summary>
            /// User actions on toolbar back button press
            /// </summary>
            void OnSupportNavigationKeyPress();

            /// <summary>
            /// Action to login
            /// </summary>
            void OnLogin();

            /// <summary>
            /// Action to closes
            /// </summary>
            void OnClose();
        }
    }
}