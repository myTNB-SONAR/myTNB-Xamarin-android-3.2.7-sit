﻿using myTNB.AndroidApp.Src.Base.MVP;

namespace myTNB.AndroidApp.Src.ForgotPasswordVerificationCodeSuccess.MVP
{
    public class ForgotPasswordVerificationCodeSuccessContract
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