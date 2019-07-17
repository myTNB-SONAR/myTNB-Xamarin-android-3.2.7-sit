using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Utils;
using System;

namespace myTNB_Android.Src.Rating.Fargment
{
    public class ThankYouFragment : Fragment
    {
        private RatingActivity ratingActivity;

        CoordinatorLayout coordinatorLayout;
        TextView txtTitleInfo;
        TextView txtContentInfo;
        TextView txtTransactionScheduleTitle;
        TextView txtFeedbackIdTitle;
        TextView txtTransactionScheduleContent;
        TextView txtFeedbackIdContent;
        Button btnBackToFeedback;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.FeedbackSuccessView, container, false);

            try
            {
                ratingActivity = ((RatingActivity)Activity);

                coordinatorLayout = rootView.FindViewById<CoordinatorLayout>(Resource.Id.rootView);
                txtTitleInfo = rootView.FindViewById<TextView>(Resource.Id.txtTitleInfo);
                txtContentInfo = rootView.FindViewById<TextView>(Resource.Id.txtContentInfo);
                txtTransactionScheduleTitle = rootView.FindViewById<TextView>(Resource.Id.txtTransactionScheduleTitle);
                txtFeedbackIdTitle = rootView.FindViewById<TextView>(Resource.Id.txtFeedbackIdTitle);
                txtTransactionScheduleContent = rootView.FindViewById<TextView>(Resource.Id.txtTransactionScheduleContent);
                txtFeedbackIdContent = rootView.FindViewById<TextView>(Resource.Id.txtFeedbackIdContent);

                btnBackToFeedback = rootView.FindViewById<Button>(Resource.Id.btnBackToFeedback);

                coordinatorLayout.SetBackgroundResource(Resource.Drawable.GradientBackground);
                txtTransactionScheduleContent.Visibility = ViewStates.Gone;
                txtFeedbackIdTitle.Visibility = ViewStates.Gone;
                txtTransactionScheduleTitle.Visibility = ViewStates.Gone;
                txtFeedbackIdContent.Visibility = ViewStates.Gone;

                TextViewUtils.SetMuseoSans300Typeface(txtContentInfo, txtFeedbackIdContent, txtTransactionScheduleContent);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, txtFeedbackIdTitle, txtTransactionScheduleTitle, btnBackToFeedback);

                btnBackToFeedback.Text = GetString(Resource.String.get_access_success_btn_dashboard);
                txtTitleInfo.Text = GetString(Resource.String.rating_thank_you);
                txtContentInfo.Text = GetString(Resource.String.rating_thank_you_message);

                ratingActivity.HideToolBar();

                btnBackToFeedback.Click += delegate
                {
                    //ratingActivity.Finish();
                    Intent DashboardIntent = new Intent(ratingActivity, typeof(DashboardHomeActivity));
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(DashboardIntent);
                };

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return rootView;
        }
    }
}