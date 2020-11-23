using Android.App;
using Android.Content;
using Android.OS;

using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Utils;
using System;

namespace myTNB_Android.Src.Rating.Fargment
{
    public class ThankYouFragment : AndroidX.Fragment.App.Fragment
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
                txtTitleInfo.TextSize = TextViewUtils.GetFontSize(16f);
                txtContentInfo.TextSize = TextViewUtils.GetFontSize(14f);
                txtTransactionScheduleTitle.TextSize = TextViewUtils.GetFontSize(9f);
                txtFeedbackIdTitle.TextSize = TextViewUtils.GetFontSize(9f);
                txtTransactionScheduleContent.TextSize = TextViewUtils.GetFontSize(14f);
                txtFeedbackIdContent.TextSize = TextViewUtils.GetFontSize(14f);
                btnBackToFeedback.TextSize = TextViewUtils.GetFontSize(16f);

                btnBackToFeedback.Text = Utility.GetLocalizedLabel("RatingResults", "backToHome");
                txtTitleInfo.Text = Utility.GetLocalizedLabel("RatingResults", "thankyou");
                txtContentInfo.Text = Utility.GetLocalizedLabel("RatingResults", "description");

                ratingActivity.HideToolBar();

                btnBackToFeedback.Click += delegate
                {
                    //ratingActivity.Finish();
                    Intent DashboardIntent = new Intent(ratingActivity, typeof(DashboardHomeActivity));
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();
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