using Android.App;
using Android.Content;
using Android.OS;

using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.MyHome;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.Rating.Activity;
using myTNB.AndroidApp.Src.Utils;
using System;

namespace myTNB.AndroidApp.Src.Rating.Fargment
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
                bool isCOTCOAFlow = false;

                if (Arguments.ContainsKey(MyHomeConstants.IS_COTCOA_PAYMENT_FLOW))
                {
                    isCOTCOAFlow = Arguments.GetBoolean(MyHomeConstants.IS_COTCOA_PAYMENT_FLOW);
                }

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
                TextViewUtils.SetTextSize9(txtTransactionScheduleTitle, txtFeedbackIdTitle);
                TextViewUtils.SetTextSize14(txtContentInfo, txtTransactionScheduleContent, txtFeedbackIdContent);
                TextViewUtils.SetTextSize16(txtTitleInfo, btnBackToFeedback);

                btnBackToFeedback.Text = isCOTCOAFlow ? Utility.GetLocalizedCommonLabel(LanguageConstants.Common.CONTINUE)
                    : Utility.GetLocalizedLabel(LanguageConstants.RATING_RESULTS, LanguageConstants.RatingResults.BACK_TO_HOME);
                txtTitleInfo.Text = Utility.GetLocalizedLabel(LanguageConstants.RATING_RESULTS, LanguageConstants.RatingResults.THANK_YOU);
                txtContentInfo.Text = Utility.GetLocalizedLabel(LanguageConstants.RATING_RESULTS, LanguageConstants.RatingResults.DESCRIPTION);

                ratingActivity.HideToolBar();

                btnBackToFeedback.Click += delegate
                {
                    if (isCOTCOAFlow)
                    {
                        Intent intent = new Intent();
                        intent.PutExtra(MyHomeConstants.IS_RATING_SUCCESSFUL, true);
                        ratingActivity.SetResult(Result.Ok, intent);
                        ratingActivity.Finish();
                    }
                    else
                    {
                        Intent DashboardIntent = new Intent(ratingActivity, typeof(DashboardHomeActivity));
                        MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                        HomeMenuUtils.ResetAll();
                        DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        StartActivity(DashboardIntent);
                    }
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