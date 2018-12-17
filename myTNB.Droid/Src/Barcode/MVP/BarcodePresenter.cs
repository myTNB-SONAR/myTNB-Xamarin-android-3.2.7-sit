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

namespace myTNB_Android.Src.Barcode.MVP
{
    public class BarcodePresenter : BarcodeContract.IUserActionsListener
    {

        private BarcodeContract.IView mView;
        private readonly string BARCODE_PATTERN = "[0-9]{12,14}";

        public BarcodePresenter(BarcodeContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnResult(string result)
        {
            this.mView.HideInvalidBarCodeError();
            if (!Java.Util.Regex.Pattern.Compile(BARCODE_PATTERN).Matcher(result).Matches())
            {
                this.mView.ShowInvalidBarCodeError();
                return;
            }

            this.mView.ShowSuccessActivity(result);
        }

        public void Scan()
        {
            // NO IMPL
        }

        public void Start()
        {
            // WILL START SCANNING
        }
    }
}