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
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using CheeseBind;
using myTNB_Android.Src.Utils;
using Java.Text;
using Java.Util;

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

            Bundle extras = Intent.Extras;
            if (extras != null )
            {
                date = extras.GetString(Constants.RESPONSE_FEEDBACK_DATE);
                feedbackId = extras.GetString(Constants.RESPONSE_FEEDBACK_ID);
            }

            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo , txtFeedbackIdContent, txtTransactionScheduleContent);
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo , txtFeedbackIdTitle, txtTransactionScheduleTitle ,btnBackToFeedback);

            
            txtFeedbackIdContent.Text = feedbackId;

            Date d = null;

            try
            {
                d = simpleDateTimeParser.Parse(date);
            }
            catch (Java.Text.ParseException e)
            {

            }

            if (d != null)
            {
                txtTransactionScheduleContent.Text = simpleDateTimeFormat.Format(d);
            }
        }

        [OnClick(Resource.Id.btnBackToFeedback)]
        void OnBack(object sender , EventArgs eventArgs)
        {
            Finish();
        }
    }
}