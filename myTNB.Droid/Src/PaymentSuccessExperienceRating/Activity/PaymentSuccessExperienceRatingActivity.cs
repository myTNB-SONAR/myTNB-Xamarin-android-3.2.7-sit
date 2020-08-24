using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.PaymentSuccessExperienceRating.MVP;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Response;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;

namespace myTNB_Android.Src.PaymentSuccessExperienceRating.Activity
{
    [Activity(Label = "@string/payment_success_experience_rating_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class PaymentSuccessExperienceRatingActivity : BaseToolbarAppCompatActivity, PaymentSuccessExperienceRatingContract.IView
    {

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.txtInputLayoutComments)]
        TextInputLayout txtInputLayoutComments;

        [BindView(Resource.Id.txtComments)]
        EditText txtComments;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        [BindView(Resource.Id.ratingBar)]
        RatingBar ratingBar;

        int ratingFromServer = 1;

        PaymentSuccessExperienceRatingContract.IUserActionsListener userActionsListener;
        PaymentSuccessExperienceRatingPresenter mPresenter;

        MaterialDialog progress;

        Snackbar mErrorMessageSnackBar;

        private MaterialDialog mSubmitRatingDialog;

        public override int ResourceId()
        {
            return Resource.Layout.PaymentSuccessExperienceRatingView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new PaymentSuccessExperienceRatingPresenter(this);

                Bundle extras = Intent.Extras;
                if (extras != null && extras.ContainsKey(Constants.SELECTED_RATING))
                {
                    ratingFromServer = extras.GetInt(Constants.SELECTED_RATING, 1);
                    ratingBar.Rating = (float)ratingFromServer;
                    CheckRatingText((int)ratingFromServer);
                }

                // Create your application here
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutComments);
                TextViewUtils.SetMuseoSans300Typeface(txtComments, txtContentInfo);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);


                ratingBar.RatingBarChange += RatingBar_RatingBarChange;

                btnSubmit.Click += delegate
                {
                    OnValidateSubmitRating();
                };

                progress = new MaterialDialog.Builder(this)
                   .Title(GetString(Resource.String.payment_success_experience_rating_title))
                   .Content(GetString(Resource.String.payment_success_experience_rating_message))
                   .Progress(true, 0)
                   .Cancelable(false)
                   .Build();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void RatingBar_RatingBarChange(object sender, RatingBar.RatingBarChangeEventArgs e)
        {
            try
            {
                if (e.FromUser)
                {
                    ratingBar.Rating = e.Rating;
                    CheckRatingText((int)e.Rating);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void CheckRatingText(int Rating)
        {
            try
            {
                if (Rating == 0)
                {
                    txtContentInfo.Text = GetString(Resource.String.payment_success_experience_rating_txt_content_0);
                }
                else if (Rating == 1)
                {
                    txtContentInfo.Text = GetString(Resource.String.payment_success_experience_rating_txt_content_1);
                }
                else if (Rating == 2)
                {
                    txtContentInfo.Text = GetString(Resource.String.payment_success_experience_rating_txt_content_2);
                }
                else if (Rating == 3)
                {
                    txtContentInfo.Text = GetString(Resource.String.payment_success_experience_rating_txt_content_3);
                }
                else if (Rating == 4)
                {
                    txtContentInfo.Text = GetString(Resource.String.payment_success_experience_rating_txt_content_4);
                }
                else if (Rating == 5)
                {
                    txtContentInfo.Text = GetString(Resource.String.payment_success_experience_rating_txt_content_5);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void OnShowRating(int value)
        {

        }

        public void ShowProgressDialog()
        {
            try
            {
                if (progress != null && !progress.IsShowing)
                {
                    progress.Show();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                if (progress != null && progress.IsShowing)
                {
                    progress.Dismiss();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Post Payment Rating & Feedback");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowSubmitRatingSuccess(SubmitExperienceRatingResponse response)
        {
            try
            {
                mSubmitRatingDialog = new MaterialDialog.Builder(this)
                           .Title(GetString(Resource.String.payment_success_experience_rating_thank_you))
                           .Content(GetString(Resource.String.payment_success_experience_rating_thank_you_message))
                           .Cancelable(false)
                           .PositiveText(Utility.GetLocalizedCommonLabel("ok"))
                           .OnPositive((dialog, which) => this.Finish()).Show();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowSubmitRatingError(string error)
        {
            try
            {
                mSubmitRatingDialog = new MaterialDialog.Builder(this)
                          .Title(GetString(Resource.String.payment_success_experience_rating_sorry))
                          .Content(GetString(Resource.String.payment_success_experience_rating_sorry_message))
                          .Cancelable(false)
                          .PositiveText(Utility.GetLocalizedCommonLabel("ok"))
                          .OnPositive((dialog, which) => mSubmitRatingDialog.Dismiss()).Show();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void SetPresenter(PaymentSuccessExperienceRatingContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void ShowErrorMessage(string error)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, error, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void OnValidateSubmitRating()
        {
            try
            {
                string rating = ratingBar.Rating.ToString();
                string ratingMsg = string.IsNullOrEmpty(txtComments.Text) ? "" : txtComments.Text;
                string ratingFor = "PAY";

                if (ratingBar.Rating == 0)
                {
                    ShowErrorMessage(GetString(Resource.String.error_empty_rating));
                    return;
                }

                this.userActionsListener.SubmitRating(rating, ratingMsg, ratingFor);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
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
