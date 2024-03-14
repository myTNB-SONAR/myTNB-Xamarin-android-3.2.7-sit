﻿using System;
using myTNB.Android.Src.Base.MVP;

namespace myTNB.Android.Src.DigitalSignature.WebView.MVP
{
    public class DSWebViewContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            /// <summary>
            /// Method for triggering actions when screen has started
            /// </summary>
            void OnStart();
        }
    }
}
