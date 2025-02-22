﻿using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;

using Android.Views;
using Android.Widget;
using CheeseBind;
using Dynatrace.Xamarin;
using Dynatrace.Xamarin.Binding.Android;
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.AppLaunch.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Enquiry.GSL.Activity;
using myTNB.AndroidApp.Src.FeedbackDetails.Activity;
using myTNB.AndroidApp.Src.OverVoltageFeedback.Activity;
using myTNB.AndroidApp.Src.SelectSubmittedFeedback.Adapter;
using myTNB.AndroidApp.Src.SelectSubmittedFeedback.MVP;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB.AndroidApp.Src.SelectSubmittedFeedback.Activity
{
    [Activity(Label = "@string/select_submitted_feedback_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class SelectSubmittedFeedbackActivity : BaseToolbarAppCompatActivity, SelectSubmittedFeedbackContract.IView
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

        List<SubmittedFeedback> listData;

        MaterialDialog progressDialog;
        MyTNBAppToolTipBuilder leaveDialog;
        public static string status, crStatusCode;
        public static string srNumber;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                if (Intent.HasExtra("TITLE") && !string.IsNullOrEmpty(Intent.GetStringExtra("TITLE")))
                {
                    SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                }
                progressDialog = new MaterialDialog.Builder(this)
                    .Title(Resource.String.select_submitted_feedback_dialog_title)
                    .Content(Resource.String.select_submitted_feedback_dialog_content)
                    .Cancelable(false)
                    .Progress(true, 0)
                    .Build();

                //adapter = new SelectSubmittedFeedbackAdapter(this, true);
                //listView.Adapter = adapter;
                //listView.EmptyView = layoutEmptyFeedback;

                TextViewUtils.SetMuseoSans300Typeface(txtEmptyFeedback);
                TextViewUtils.SetTextSize12(txtEmptyFeedback);
                txtEmptyFeedback.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryEmpty");

                mPresenter = new SelectSubmittedFeedbackPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
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
            try
            {
                this.SetIsClicked(false);
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    SubmittedFeedback feedback = adapter.GetItemObject(e.Position);
                    srNumber = feedback.FeedbackId;
                    //dynatrace 
                    var myAction = Agent.Instance.EnterAction(Constants.TOUCH_ON_VIEW_OVERVOLTAGE_CLAIM_FROM_LIST);  // DYNA
                    myAction.ReportValue("session_id", LaunchViewActivity.DynatraceSessionUUID);
                    myAction.ReportValue("sr_number", feedback.FeedbackId.ToString());
                    myAction.LeaveAction();
                    this.userActionsListener.OnSelect(feedback);
                }
            }
            catch (Exception ex)
            {
                this.SetIsClicked(false);
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (!string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(crStatusCode))
                {
                    int index = listData.FindIndex(s => s.FeedbackId.Equals(srNumber));
                    if (index != -1)
                    {
                        listData[index].StatusDesc = status;
                        listData[index].StatusCode = crStatusCode;
                        //SubmittedFeedback.d.data[index].StatusDesc = status;
                        //SubmittedFeedback.d.data[index].StatusCode = statusCode;
                        //srNumber = null;
                        status = null;
                        //statusCode = null;
                    }
                    ShowList(listData);
                }
                FirebaseAnalyticsUtils.SetScreenName(this, "Select Submitted Feedback");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowList(List<SubmittedFeedback> list)
        {
            //adapter.AddAll(list);
            if (list != null && list.Count > 0)
            {
                listData = new List<SubmittedFeedback>(list);
                ShowProgressDialog();
                adapter = new SelectSubmittedFeedbackAdapter(this, list, true);
                listView.Adapter = adapter;
                HideProgressDialog();
            }
            else
            {
                listView.EmptyView = layoutEmptyFeedback;
            }
            //listView.Adapter = adapter;
            //listView.EmptyView = layoutEmptyFeedback;
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }




        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException, string message)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, !string.IsNullOrEmpty(message) ? message : Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
                this.OnBackPressed();
            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException, string message)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, !string.IsNullOrEmpty(message) ? message : Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mApiExcecptionSnackBar.Dismiss();
                this.OnBackPressed();
            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
            this.SetIsClicked(false);
        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception, string message)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, !string.IsNullOrEmpty(message) ? message : Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void ShowStartLoading()
        {
            ShowProgressDialog();
            this.userActionsListener.OnStartShowLoading(this.DeviceId());
        }

        public void ClearList()
        {
            if (adapter != null)
            {
                adapter.Clear();
            }
        }

        public void ShowFeedbackDetailsBillRelated(SubmittedFeedbackDetails submittedFeedbackDetail, SubmittedFeedback submittedFeedback, bool isAboutMyBill)
        {
            var billIntent = new Intent(this, typeof(FeedbackDetailsBillRelatedActivity));
            billIntent.PutExtra("TITLE", !string.IsNullOrEmpty(submittedFeedback.FeedbackNameInListView) ? submittedFeedback.FeedbackNameInListView : submittedFeedback.FeedbackCategoryName);
            if (isAboutMyBill)
            {
                billIntent.PutExtra("ABOUTMYBILL", "true");
            }
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

        public void ShowFeedbackDetailsGSL()
        {
            Intent gslDetailsIntent = new Intent(this, typeof(GSLRebateSubmittedDetailsActivity));
            StartActivity(gslDetailsIntent);
        }
        
        public void ShowFeedbackDetailsOverVoltage(SubmittedFeedbackDetails submittedFeedbackdetail, SubmittedFeedback submittedFeedback, string ClaimId)
        {
            if (ClaimId != null)
            {
                var othersIntent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));
                othersIntent.PutExtra("TITLE", !string.IsNullOrEmpty(submittedFeedback.FeedbackNameInListView) ? submittedFeedback.FeedbackNameInListView : submittedFeedback.FeedbackCategoryName);
                othersIntent.PutExtra("ClaimId", ClaimId);
                StartActivity(othersIntent);
            }
            else
            {
                leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(Utility.GetLocalizedLabel("Error", "defaultErrorTitle"))
                .SetMessage(Utility.GetLocalizedLabel("Error", "defaultErrorMessage"))
                .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build();
                leaveDialog.Show();
            }
        }

        private Snackbar bcrmExceptionSnackBar;
        public void ShowBCRMDownException(String exception)
        {
            if (bcrmExceptionSnackBar != null && bcrmExceptionSnackBar.IsShown)
            {
                bcrmExceptionSnackBar.Dismiss();

            }

            bcrmExceptionSnackBar = Snackbar.Make(rootView, exception, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate
            {

                bcrmExceptionSnackBar.Dismiss();
            }
            );
            View v = bcrmExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            bcrmExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}
