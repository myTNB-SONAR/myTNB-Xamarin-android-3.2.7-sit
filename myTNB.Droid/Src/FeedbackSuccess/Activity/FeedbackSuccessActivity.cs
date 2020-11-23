using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Globalization;
using System.Runtime;

namespace myTNB_Android.Src.FeedbackSuccess.Activity
{
    [Activity(
        ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.BillRelated")]
    public class FeedbackSuccessActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.txtTransactionScheduleTitle)]
        TextView txtTransactionScheduleTitle;

        [BindView(Resource.Id.txtFeedbackIdTitle)]
        TextView txtFeedbackIdTitle;

        [BindView(Resource.Id.txtTransactionScheduleContent)]
        TextView txtTransactionScheduleContent;

        [BindView(Resource.Id.txtFeedbackIdContent)]
        TextView txtFeedbackIdContent;

        [BindView(Resource.Id.btnBackToFeedback)]
        Button btnBackToFeedback;

        private string date;
        private string feedbackId;

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackSuccessView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    date = extras.GetString(Constants.RESPONSE_FEEDBACK_DATE);
                    feedbackId = extras.GetString(Constants.RESPONSE_FEEDBACK_ID);
                }

                TextViewUtils.SetMuseoSans300Typeface(txtContentInfo, txtFeedbackIdContent, txtTransactionScheduleContent);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, txtFeedbackIdTitle, txtTransactionScheduleTitle, btnBackToFeedback);

                txtTitleInfo.TextSize = TextViewUtils.GetFontSize(16f);
                txtContentInfo.TextSize = TextViewUtils.GetFontSize(14f);
                txtTransactionScheduleTitle.TextSize = TextViewUtils.GetFontSize(9f);
                txtFeedbackIdTitle.TextSize = TextViewUtils.GetFontSize(9f);
                txtTransactionScheduleContent.TextSize = TextViewUtils.GetFontSize(14f);
                txtFeedbackIdContent.TextSize = TextViewUtils.GetFontSize(14f);
                btnBackToFeedback.TextSize = TextViewUtils.GetFontSize(16f);
                txtFeedbackIdContent.Text = feedbackId;
                SetStaticLabels();

                string dateTime = "NA";

                if (!string.IsNullOrEmpty(date))
                {
                    try
                    {
                        dateTime = date;
                        DateTime dateTimeParse = DateTime.ParseExact(dateTime, "dd'/'MM'/'yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                        if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                        {
                            CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                            dateTime = dateTimeParse.ToString("dd MMM yyyy, h:mm tt", currCult);
                        }
                        else
                        {
                            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                            dateTime = dateTimeParse.ToString("dd MMM yyyy, h:mm tt", currCult);
                        }
                    }
                    catch (System.Exception e)
                    {
                        dateTime = "NA";
                        Utility.LoggingNonFatalError(e);
                    }
                }

                txtTransactionScheduleContent.Text = dateTime;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        [OnClick(Resource.Id.btnBackToFeedback)]
        void OnBack(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Finish();
            }
        }

        private void SetStaticLabels()
        {
            txtTitleInfo.Text = Utility.GetLocalizedLabel("Status", "feedbackSuccessTitle");
            txtContentInfo.Text = Utility.GetLocalizedLabel("Status", "feedbackSuccessMessage");
            txtTransactionScheduleTitle.Text = Utility.GetLocalizedLabel("Status", "feedbackDateTitle");
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("Status", "feedbackReferenceTitle");
            btnBackToFeedback.Text = Utility.GetLocalizedLabel("Status", "backToFeedback");
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                SetStaticLabels();
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit Feedback Success");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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