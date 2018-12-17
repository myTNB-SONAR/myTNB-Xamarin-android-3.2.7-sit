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
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using CheeseBind;
using myTNB_Android.Src.SelectSubmittedFeedback.Adapter;
using myTNB_Android.Src.SelectSubmittedFeedback.MVP;
using myTNB_Android.Src.Base.Models;
using Refit;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.Utils;
using Android.Support.Design.Widget;
using myTNB_Android.Src.FeedbackDetails.Activity;
using Newtonsoft.Json;
using Android.Preferences;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System.Runtime;

namespace myTNB_Android.Src.SelectSubmittedFeedback.Activity
{
    [Activity(Label = "@string/select_submitted_feedback_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.SelectFeedbackStateStyle")]
    public class SelectSubmittedFeedbackActivity : BaseToolbarAppCompatActivity , SelectSubmittedFeedbackContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.listView)]
        ListView listView;

        [BindView(Resource.Id.layout_empty_feedback)]
        LinearLayout layoutEmptyFeedback;

        [BindView(Resource.Id.txtEmptyFeedback)]
        TextView txtEmptyFeedback;

        SelectSubmittedFeedbackAdapter adapter;

        SelectSubmittedFeedbackContract.IUserActionsListener userActionsListener;
        SelectSubmittedFeedbackPresenter mPresenter;

        MaterialDialog progressDialog;
        LoadingOverlay loadingOverlay;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try {
            progressDialog = new MaterialDialog.Builder(this)
                .Title(Resource.String.select_submitted_feedback_dialog_title)
                .Content(Resource.String.select_submitted_feedback_dialog_content)
                .Cancelable(false)
                .Progress(true , 0)
                .Build();

            adapter = new SelectSubmittedFeedbackAdapter(this, true);
            listView.Adapter = adapter;
            listView.EmptyView = layoutEmptyFeedback;

            TextViewUtils.SetMuseoSans300Typeface(txtEmptyFeedback);

            mPresenter = new SelectSubmittedFeedbackPresenter(this , PreferenceManager.GetDefaultSharedPreferences(this));
            this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        [OnItemClick(Resource.Id.listView)]
        void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try {
            SubmittedFeedback feedback = adapter.GetItemObject(e.Position);
            this.userActionsListener.OnSelect(feedback);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }


        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackSubmittedView;
        }

        public void SetPresenter(SelectSubmittedFeedbackContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }


        public void ShowList(List<SubmittedFeedback> list)
        {
            adapter.AddAll(list);
        }

        public void ShowProgressDialog()
        {
            //if (progressDialog != null && !progressDialog.IsShowing)
            //{
            //    progressDialog.Show();
            //}
            try {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            //if (progressDialog != null && progressDialog.IsShowing)
            //{
            //    progressDialog.Dismiss();
            //}
            try {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_cancelled_exception_btn_retry), delegate {

                mCancelledExceptionSnackBar.Dismiss();
                this.userActionsListener.OnStartShowLoading(this.DeviceId());
            }
            );
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_api_exception_btn_retry), delegate {

                mApiExcecptionSnackBar.Dismiss();
                this.userActionsListener.OnStartShowLoading(this.DeviceId());
            }
            );
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_unknown_exception_btn_retry), delegate {

                mUknownExceptionSnackBar.Dismiss();
                this.userActionsListener.OnStartShowLoading(this.DeviceId());
            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void ShowStartLoading()
        {
            this.userActionsListener.OnStartShowLoading(this.DeviceId());
        }

        public void ClearList()
        {
            adapter.Clear();
        }

        public void ShowFeedbackDetailsBillRelated(SubmittedFeedbackDetails submittedFeedback)
        {
            var billIntent = new Intent(this , typeof(FeedbackDetailsBillRelatedActivity));
            StartActivity(billIntent);
        }

        public void ShowFeedbackDetailsFaultyLamps(SubmittedFeedbackDetails submittedFeedback)
        {
            var faultyIntent = new Intent(this, typeof(FeedbackDetailsFaultyLampsActivity));
            StartActivity(faultyIntent);
        }

        public void ShowFeedbackDetailsOthers(SubmittedFeedbackDetails submittedFeedback)
        {
            var othersIntent = new Intent(this, typeof(FeedbackDetailsOthersActivity));
            StartActivity(othersIntent);
        }

        private Snackbar bcrmExceptionSnackBar;
        public void ShowBCRMDownException(String exception)
        {
            if (bcrmExceptionSnackBar != null && bcrmExceptionSnackBar.IsShown)
            {
                bcrmExceptionSnackBar.Dismiss();

            }

            bcrmExceptionSnackBar = Snackbar.Make(rootView, exception, Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.manage_cards_btn_ok), delegate {

                bcrmExceptionSnackBar.Dismiss();
            }
            );
            View v = bcrmExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            bcrmExceptionSnackBar.Show();

        }


        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}