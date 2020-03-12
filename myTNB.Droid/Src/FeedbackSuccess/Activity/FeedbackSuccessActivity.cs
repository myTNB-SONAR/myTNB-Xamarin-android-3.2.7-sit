﻿using Android.App;
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

        //17/07/2017
        SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy h:mm a");


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


                txtFeedbackIdContent.Text = feedbackId;

                Date d = null;

                try
                {
                    d = simpleDateTimeParser.Parse(date);
                }
                catch (Java.Text.ParseException e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                if (d != null)
                {
                    txtTransactionScheduleContent.Text = simpleDateTimeFormat.Format(d);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        [OnClick(Resource.Id.btnBackToFeedback)]
        void OnBack(object sender, EventArgs eventArgs)
        {
            Finish();
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