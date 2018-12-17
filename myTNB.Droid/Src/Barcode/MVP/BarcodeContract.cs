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

namespace myTNB_Android.Src.Barcode.MVP
{
    public class BarcodeContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Hides invalid barcode error dialog
            /// </summary>
            void HideInvalidBarCodeError();

            /// <summary>
            /// Shows invalid barcode error dialog
            /// </summary>
            void ShowInvalidBarCodeError();

            /// <summary>
            ///  Shows the success activity currently the previously activity
            /// </summary>
            /// <param name="result">string representation of barcode result</param>
            void ShowSuccessActivity(string result);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to scan barcode
            /// </summary>
            void Scan();

            /// <summary>
            /// The callback of the result of the barcode
            /// </summary>
            /// <param name="result">string representation of barcode result</param>
            void OnResult(string result);
        }
    }
}