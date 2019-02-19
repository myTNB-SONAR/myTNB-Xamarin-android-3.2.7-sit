using System;
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
using Android.Support.Design.Widget;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.PaymentSuccessExperienceRating.MVP;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Response;
using myTNB_Android.Src.Database.Model;
using AFollestad.MaterialDialogs;

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

            mPresenter = new PaymentSuccessExperienceRatingPresenter(this);

            Bundle extras = Intent.Extras;
            if (extras != null && extras.ContainsKey(Constants.SELECTED_RATING))
            {
                ratingFromServer = extras.GetInt(Constants.SELECTED_RATING , 1);
                ratingBar.Rating = (float)ratingFromServer;
                CheckRatingText((int)ratingFromServer);
            }

            // Create your application here
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutComments);
            TextViewUtils.SetMuseoSans300Typeface(txtComments , txtContentInfo);
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

        private void RatingBar_RatingBarChange(object sender, RatingBar.RatingBarChangeEventArgs e)
        {
            if (e.FromUser)
            {
                ratingBar.Rating = e.Rating;
                CheckRatingText((int)e.Rating);
            }
        }

        private void CheckRatingText(int Rating)
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

        public void OnShowRating(int value)
        {
            
        }

        public void ShowProgressDialog()
        {
            if (progress != null && !progress.IsShowing)
            {
                progress.Show();
            }
        }

        public void HideProgressDialog()
        {
            if (progress != null && progress.IsShowing)
            {
                progress.Dismiss();
            }
        }

        public void ShowSubmitRatingSuccess(SubmitExperienceRatingResponse response)
        {
            mSubmitRatingDialog = new MaterialDialog.Builder(this)
                       .Title(GetString(Resource.String.payment_success_experience_rating_thank_you))
                       .Content(GetString(Resource.String.payment_success_experience_rating_thank_you_message))
                       .Cancelable(false)
                       .PositiveText("Ok")
                       .OnPositive((dialog, which) => this.Finish()).Show();
        }

        public void ShowSubmitRatingError(string error)
        {
            mSubmitRatingDialog = new MaterialDialog.Builder(this)
                      .Title(GetString(Resource.String.payment_success_experience_rating_sorry))
                      .Content(GetString(Resource.String.payment_success_experience_rating_sorry_message))
                      .Cancelable(false)
                      .PositiveText("Ok")
                      .OnPositive((dialog, which) => mSubmitRatingDialog.Dismiss()).Show();
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
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void OnValidateSubmitRating()
        {
            string rating = ratingBar.Rating.ToString();
            string ratingMsg = string.IsNullOrEmpty(txtComments.Text) ? "" : txtComments.Text;
            string ratingFor = "PAY";

            if(ratingBar.Rating == 0)
            {
                ShowErrorMessage(GetString(Resource.String.error_empty_rating));
                return;
            }

            this.userActionsListener.SubmitRating(rating, ratingMsg, ratingFor);
        }
    }
}