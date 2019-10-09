﻿using AFollestad.MaterialDialogs;
using Android.Content;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_Login_BillRelated.Activity;
using myTNB_Android.Src.Feedback_Login_FaultyStreetLamps.Activity;
using myTNB_Android.Src.Feedback_Login_Others.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.SelectSubmittedFeedback.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu
{
    public class FeedbackMenuFragment : BaseFragment, FeedbackMenuContract.IView
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

        MaterialDialog progressDialog;
        LoadingOverlay loadingOverlay;

        FeedbackMenuContract.IUserActionsListener userActionsListener;
        FeedbackMenuPresenter mPresenter;

        string feedbackBillRelatedTitle = "";
        string feedbackStreetLampTitle = "";
        string feedbackOthersTitle = "";

        string submittedFeedbackTitle = "";


        public override int ResourceId()
        {
            return Resource.Layout.FeedbackMenuView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            mPresenter = new FeedbackMenuPresenter(this);

        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            progressDialog = new MaterialDialog.Builder(this.Activity)
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

            this.userActionsListener.Start();


        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                }
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Feedback");
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            base.OnAttach(context);
        }

        public override void OnResume()
        {
            base.OnResume();
            this.userActionsListener.OnResume();
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
        }

        public void SetPresenter(FeedbackMenuContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsResumed;
        }

        public void ShowBillingPayment()
        {
            var billingPaymentFeedback = new Intent(this.Activity, typeof(FeedbackLoginBillRelatedActivity));
            billingPaymentFeedback.PutExtra("TITLE", feedbackBillRelatedTitle);
            StartActivity(billingPaymentFeedback);
        }


        public void ShowFaultyStreetLamps()
        {
            // TODO : ADD FAULTY STREE
            var faultyStreetLamps = new Intent(this.Activity, typeof(FeedbackLoginFaultyStreetLampsActivity));
            faultyStreetLamps.PutExtra("TITLE",feedbackStreetLampTitle);
            StartActivity(faultyStreetLamps);
        }

        public void ShowOthers()
        {
            // TODO : ADD OTHERS
            var feedbackOthers = new Intent(this.Activity, typeof(FeedbackLoginOthersActivity));
            feedbackOthers.PutExtra("TITLE",feedbackOthersTitle);
            StartActivity(feedbackOthers);
        }

        public void ShowSubmittedFeedback()
        {
            var submittedFeedback = new Intent(this.Activity, typeof(SelectSubmittedFeedbackActivity));
            submittedFeedback.PutExtra("TITLE", submittedFeedbackTitle);
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
            //txtFeedbackNoOfSubmitted.Text = string.Format("{0}" , count);
        }


        public string GetDeviceId()
        {
            return this.DeviceId();
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

                loadingOverlay = new LoadingOverlay(Activity.ApplicationContext, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (System.Exception e)
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
            catch (System.Exception e)
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
        public void ShowRetryOptionsUnknownException(System.Exception exception)
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
                    else if (fc.Id.Equals("10"))
                    {
                        submittedFeedbackTitle = fc.Name;
                        txtSubmittedFeedback.Text = fc.Name;
                        txtSubmittedFeedbackContent.Text = fc.Desc;
                    }

                }
            }
            catch (System.Exception e)
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

            try
            {
                if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                {
                    mErrorMessageSnackBar.Dismiss();
                }


                if (string.IsNullOrEmpty(message))
                {
                    DownTimeEntity BCRMDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                    if (BCRMDownTime != null && !string.IsNullOrEmpty(BCRMDownTime.DowntimeTextMessage))
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
                v.SetPadding(0, 0, 0, 50);
                mErrorMessageSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}