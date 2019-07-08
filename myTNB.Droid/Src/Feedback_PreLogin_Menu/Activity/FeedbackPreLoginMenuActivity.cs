﻿using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_PreLogin_BillRelated.Activity;
using myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.Activity;
using myTNB_Android.Src.Feedback_PreLogin_Menu.MVP;
using myTNB_Android.Src.Feedback_PreLogin_Others.Activity;
using myTNB_Android.Src.SelectSubmittedFeedback.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.Feedback_PreLogin_Menu.Activity
{
    [Activity(Label = "@string/menu_feedback"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Feedback")]
    public class FeedbackPreLoginMenuActivity : BaseToolbarAppCompatActivity, FeedbackPreLoginMenuContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;


        [BindView(Resource.Id.txtFeedbackBillingAndPayment)]
        TextView txtFeedbackBillingAndPayment;

        [BindView(Resource.Id.txtFeedbackBillingAndPaymentContent)]
        TextView txtFeedbackBillingAndPaymentContent;

        [BindView(Resource.Id.txtFeedbackFaultyStreetLamps)]
        TextView txtFeedbackFaultyStreetLamps;

        [BindView(Resource.Id.txtFeedbackFaultyStreetLampsContent)]
        TextView txtFeedbackFaultyStreetLampsContent;


        [BindView(Resource.Id.txtFeedbackOthers)]
        TextView txtFeedbackOthers;


        [BindView(Resource.Id.txtFeedbackOthersContent)]
        TextView txtFeedbackOthersContent;


        [BindView(Resource.Id.txtSubmittedFeedback)]
        TextView txtSubmittedFeedback;

        [BindView(Resource.Id.txtSubmittedFeedbackContent)]
        TextView txtSubmittedFeedbackContent;

        [BindView(Resource.Id.billRelatedContraint)]
        ConstraintLayout billRelatedConstraint;

        [BindView(Resource.Id.faultyStreetLampsContraint)]
        ConstraintLayout faultyStreetLampsContraint;

        [BindView(Resource.Id.othersContraint)]
        ConstraintLayout othersContraint;

        [BindView(Resource.Id.spaceBillRelated)]
        View spaceBillRelated;

        [BindView(Resource.Id.spaceFaultyStreetLamps)]
        View spaceFaultyStreetLamps;

        [BindView(Resource.Id.spaceOthers)]
        View spaceOthers;



        FeedbackPreLoginMenuContract.IUserActionsListener userActionsListener;
        FeedbackPreLoginMenuPresenter mPresenter;

        MaterialDialog progressDialog;
        LoadingOverlay loadingOverlay;

        string feedbackBillRelatedTitle = "";
        string feedbackStreetLampTitle = "";
        string feedbackOthersTitle = "";

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackPreloginMenuView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            try
            {
                progressDialog = new MaterialDialog.Builder(this)
                    .Title(Resource.String.select_submitted_feedback_dialog_title)
                    .Content(Resource.String.select_submitted_feedback_dialog_content)
                    .Cancelable(false)
                    .Progress(true, 0)
                    .Build();

                TextViewUtils.SetMuseoSans300Typeface(txtFeedbackBillingAndPaymentContent,
                            txtFeedbackFaultyStreetLampsContent,
                            txtSubmittedFeedbackContent,
                            txtFeedbackOthersContent);

                TextViewUtils.SetMuseoSans500Typeface(txtFeedbackBillingAndPayment,
                            txtFeedbackFaultyStreetLamps,
                            txtSubmittedFeedback,
                            txtFeedbackOthers);


                mPresenter = new FeedbackPreLoginMenuPresenter(this, this.DeviceId());
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void ShowBillingPayment()
        {
            var billingPaymentFeedback = new Intent(this, typeof(FeedbackPreLoginBillRelatedActivity));
            billingPaymentFeedback.PutExtra("TITLE", feedbackBillRelatedTitle);
            StartActivity(billingPaymentFeedback);
        }


        public void ShowFaultyStreetLamps()
        {
            // TODO : ADD FAULTY STREE
            var faultyStreetLamps = new Intent(this, typeof(FeedbackPreLoginFaultyStreetLampsActivity));
            faultyStreetLamps.PutExtra("TITLE", feedbackStreetLampTitle);
            StartActivity(faultyStreetLamps);
        }

        public void ShowOthers()
        {
            // TODO : ADD OTHERS
            var feedbackOthers = new Intent(this, typeof(FeedbackPreLoginOthersActivity));
            feedbackOthers.PutExtra("TITLE", feedbackOthersTitle);
            StartActivity(feedbackOthers);
        }

        public void ShowSubmittedFeedback()
        {
            var submittedFeedback = new Intent(this, typeof(SelectSubmittedFeedbackActivity));
            StartActivity(submittedFeedback);
        }

        [OnClick(Resource.Id.billRelatedContraint)]
        void OnBillingAndPayment(object sender, EventArgs eventArgs)
        {
            if (DownTimeEntity.IsBCRMDown())
            {
                OnBCRMDownTimeErrorMessage();
            }
            else
            {
                this.userActionsListener.OnBillingPayment();
            }

        }

        [OnClick(Resource.Id.faultyStreetLampsContraint)]
        void OnFaultyStreetLamps(object sender, EventArgs eventArgs)
        {
            if (DownTimeEntity.IsBCRMDown())
            {
                OnBCRMDownTimeErrorMessage();
            }
            else
            {
                this.userActionsListener.OnFaultyStreetLamps();
            }
        }

        [OnClick(Resource.Id.othersContraint)]
        void OnOthers(object sender, EventArgs eventArgs)
        {
            if (DownTimeEntity.IsBCRMDown())
            {
                OnBCRMDownTimeErrorMessage();
            }
            else
            {
                this.userActionsListener.OnOthers();
            }
        }

        [OnClick(Resource.Id.submittedFeedbackConstraint)]
        void OnSubmittedFeedback(object sender, EventArgs eventArgs)
        {
            //if (DownTimeEntity.IsBCRMDown())
            //{
            //    OnBCRMDownTimeErrorMessage();
            //}
            //else
            //{
            this.userActionsListener.OnSubmittedFeedback();
            //}
        }


        public void ShowSubmittedFeedbackCount(int count)
        {
            //txtFeedbackNoOfSubmitted.Text = string.Format("{0}", count);
        }

        public void ShowProgressDialog()
        {
            //if (progressDialog != null && !progressDialog.IsShowing)
            //{
            //    progressDialog.Show();
            //}
            try
            {
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
            try
            {
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
            .SetAction(GetString(Resource.String.login_cancelled_exception_btn_retry), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
                this.userActionsListener.OnRetry();
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
            .SetAction(GetString(Resource.String.login_api_exception_btn_retry), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
                this.userActionsListener.OnRetry();
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
            .SetAction(GetString(Resource.String.login_unknown_exception_btn_retry), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
                this.userActionsListener.OnRetry();
            }
            );
            mUknownExceptionSnackBar.Show();

        }


        public void SetPresenter(FeedbackPreLoginMenuContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.userActionsListener.OnResume();
        }

        public void ShowFeedbackMenu(List<FeedbackCategoryEntity> feedbackCategory)
        {
            try
            {
                billRelatedConstraint.Visibility = ViewStates.Gone;
                faultyStreetLampsContraint.Visibility = ViewStates.Gone;
                othersContraint.Visibility = ViewStates.Gone;
                spaceBillRelated.Visibility = ViewStates.Gone;
                spaceFaultyStreetLamps.Visibility = ViewStates.Gone;
                spaceOthers.Visibility = ViewStates.Gone;
                foreach (FeedbackCategoryEntity fc in feedbackCategory)
                {
                    if (fc.Id.Equals("1"))
                    {
                        billRelatedConstraint.Visibility = ViewStates.Visible;
                        spaceBillRelated.Visibility = ViewStates.Visible;
                        feedbackBillRelatedTitle = fc.Name;
                        txtFeedbackBillingAndPayment.Text = fc.Name;
                        txtFeedbackBillingAndPaymentContent.Text = fc.Desc;
                    }
                    else if (fc.Id.Equals("2"))
                    {
                        faultyStreetLampsContraint.Visibility = ViewStates.Visible;
                        spaceFaultyStreetLamps.Visibility = ViewStates.Visible;
                        feedbackStreetLampTitle = fc.Name;
                        txtFeedbackFaultyStreetLamps.Text = fc.Name;
                        txtFeedbackFaultyStreetLampsContent.Text = fc.Desc;
                    }
                    else if (fc.Id.Equals("3"))
                    {
                        othersContraint.Visibility = ViewStates.Visible;
                        spaceOthers.Visibility = ViewStates.Visible;
                        feedbackOthersTitle = fc.Name;
                        txtFeedbackOthers.Text = fc.Name;
                        txtFeedbackOthersContent.Text = fc.Desc;
                    }

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override bool CameraPermissionRequired()
        {
            return true;
        }

        public override bool StoragePermissionRequired()
        {
            return true;
        }

        public override bool TelephonyPermissionRequired()
        {
            return false;
        }


        Snackbar mErrorMessageSnackBar;
        public void OnBCRMDownTimeErrorMessage(string message = null)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
                DownTimeEntity BCRMDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (!string.IsNullOrEmpty(BCRMDownTime.DowntimeTextMessage))
                {
                    message = BCRMDownTime.DowntimeTextMessage;
                }
                else
                {
                    message = GetString(Resource.String.app_launch_http_exception_error);
                }
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
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