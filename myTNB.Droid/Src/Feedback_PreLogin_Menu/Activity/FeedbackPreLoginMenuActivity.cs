using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Feedback_PreLogin_FaultyStreetLamps.Activity;
using myTNB.AndroidApp.Src.Feedback_PreLogin_Menu.MVP;
using myTNB.AndroidApp.Src.Feedback_Prelogin_NewIC.Activity;
using myTNB.AndroidApp.Src.Feedback_PreLogin_Others.Activity;
using myTNB.AndroidApp.Src.SelectSubmittedFeedback.Activity;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB.AndroidApp.Src.Feedback_PreLogin_Menu.Activity
{
    [Activity(Label = "@string/menu_feedback"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Feedback")]
    public class FeedbackPreLoginMenuActivity : BaseActivityCustom, FeedbackPreLoginMenuContract.IView
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

        [BindView(Resource.Id.feedbackMenuHeaderImage)]
        ImageView feedbackMenuHeaderImage;

        [BindView(Resource.Id.billRelatedIcon)]
        ImageView billRelatedIcon;


        FeedbackPreLoginMenuContract.IUserActionsListener userActionsListener;
        FeedbackPreLoginMenuPresenter mPresenter;

        MaterialDialog progressDialog;

        string feedbackBillRelatedTitle = "";
        string feedbackStreetLampTitle = "";
        string feedbackOthersTitle = "";
        string submittedFeedbackTitle = "";

        const string PAGE_ID = "SubmitEnquiry";

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

                TextViewUtils.SetTextSize12(txtFeedbackBillingAndPaymentContent
                    , txtFeedbackFaultyStreetLampsContent, txtFeedbackOthersContent, txtSubmittedFeedbackContent);
                TextViewUtils.SetTextSize16(txtFeedbackBillingAndPayment, txtFeedbackFaultyStreetLamps
                    , txtFeedbackOthers, txtSubmittedFeedback);

                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "enquireTitle"));

                mPresenter = new FeedbackPreLoginMenuPresenter(this, this.DeviceId());

                feedbackMenuHeaderImage.Visibility = ViewStates.Gone;

                this.userActionsListener.Start();

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (bcrmEntity.IsDown)
                {
                    txtFeedbackBillingAndPayment.SetTextColor(Android.Graphics.Color.Gray);
                    txtFeedbackBillingAndPaymentContent.SetTextColor(Android.Graphics.Color.Gray);
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowBillingPayment()
        {
            //  var billingPaymentFeedback = new Intent(this, typeof(FeedbackPreLoginBillRelatedActivity));
            //  billingPaymentFeedback.PutExtra("TITLE", feedbackBillRelatedTitle);
            //  StartActivity(billingPaymentFeedback);

            var billingPaymentFeedback = new Intent(this, typeof(FeedbackPreloginNewICActivity));
            //billingPaymentFeedback.PutExtra("TITLE", feedbackNewIc);

            //StartActivityForResult(billingPaymentFeedback, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
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
            submittedFeedback.PutExtra("TITLE", submittedFeedbackTitle);
            StartActivity(submittedFeedback);
        }

        [OnClick(Resource.Id.billRelatedContraint)]
        void OnBillingAndPayment(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.GetDownTime();

                // if (DownTimeEntity.IsBCRMDown())
                // {
                //     //OnBCRMDownTimeErrorMessage();
                //     this.SetIsClicked(false);

                //     DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_RS_SYSTEM);
                //     OnBCRMDownTimeErrorMessageV2(bcrmEntity);

                // }
                // else
                // {
                //     this.userActionsListener.OnBillingPayment();
                // }
            }
        }

        [OnClick(Resource.Id.faultyStreetLampsContraint)]
        void OnFaultyStreetLamps(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnFaultyStreetLamps();
            
            }
        }

        [OnClick(Resource.Id.othersContraint)]
        void OnOthers(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnOthers();
                
            }
        }

        [OnClick(Resource.Id.submittedFeedbackConstraint)]
        void OnSubmittedFeedback(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnSubmittedFeedback();
            }
        }


        public void ShowSubmittedFeedbackCount(int count)
        {
            //txtFeedbackNoOfSubmitted.Text = string.Format("{0}", count);
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
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
                this.userActionsListener.OnRetry();
            }
            );
            View snackbarView = mCancelledExceptionSnackBar.View;
            TextView tv = snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();
            this.SetIsClicked(false);

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
                this.userActionsListener.OnRetry();
            }
            );
            View snackbarView = mApiExcecptionSnackBar.View;
            TextView tv = snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
            this.SetIsClicked(false);

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
                this.userActionsListener.OnRetry();
            }
            );
            View snackbarView = mUknownExceptionSnackBar.View;
            TextView tv = snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);

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
                        feedbackBillRelatedTitle = Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"); //fc.Name;
                        txtFeedbackBillingAndPayment.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle");//fc.Name;
                        txtFeedbackBillingAndPaymentContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryDescription");//fc.Desc;
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
                        submittedFeedbackTitle = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry"); // fc.Name;Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle");
                        txtSubmittedFeedback.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry"); //fc.Name;
                        txtSubmittedFeedbackContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewEnquiryTitle");//fc.Desc;
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
                    message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
                }
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }

         public void OnCheckBCRMDowntime()
        {
            this.SetIsClicked(false);
            //OnBCRMDownTimeErrorMessage();
            DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_RS_SYSTEM);
            OnBCRMDownTimeErrorMessageV2(bcrmEntity);

            billRelatedIcon.SetImageResource(Resource.Drawable.ic_feedback_bill_disable);
            txtFeedbackBillingAndPayment.SetTextColor(Android.Graphics.Color.Gray);
            txtFeedbackBillingAndPaymentContent.SetTextColor(Android.Graphics.Color.Gray);
        }

        public void OnBCRMDownTimeErrorMessageV2(DownTimeEntity bcrmEntity)
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
            .SetHeaderImage(Resource.Drawable.maintenance_bcrm)
            .SetTitle(bcrmEntity.DowntimeTextMessage)
            .SetMessage(bcrmEntity.DowntimeMessage)
            .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.GOT_IT))
            .Build()
            .Show();
        }
    }
}
