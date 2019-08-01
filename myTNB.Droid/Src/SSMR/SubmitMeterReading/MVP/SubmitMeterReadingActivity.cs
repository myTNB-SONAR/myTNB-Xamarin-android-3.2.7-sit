
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

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public enum METER_READING_TYPE
    {
        KWH,KVARH,KW
    }

    [Activity(Label = "Submit Meter Reading", Theme = "@style/Theme.Dashboard", MainLauncher = true)]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity
    {
        private METER_READING_TYPE meterType;
        public override int ResourceId()
        {
            return Resource.Layout.SubmitMeterReadingLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            meterType = METER_READING_TYPE.KWH;
            // Create your application here
            PopulateMeterReadingCard(METER_READING_TYPE.KWH);
            PopulateMeterReadingCard(METER_READING_TYPE.KVARH);
            PopulateMeterReadingCard(METER_READING_TYPE.KW);
        }

        public void PopulateMeterReadingCard(METER_READING_TYPE type)
        {
            LinearLayout linearLayoutContainer;
            string typeId;

            switch (type)
            {
                case METER_READING_TYPE.KWH:
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    typeId = "kWh";
                    break;
                case METER_READING_TYPE.KVARH:
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    typeId = "kVARh";
                    break;
                default:
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    typeId = "kW";
                    break;
            }

            TextView firstPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_1) as TextView;
            TextView secondPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_2) as TextView;
            TextView thirdPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_3) as TextView;
            TextView fourthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_4) as TextView;
            TextView fifthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_5) as TextView;
            TextView sixthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_6) as TextView;
            TextView seventhPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_7) as TextView;
            TextView eigthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_8) as TextView;
            TextView ninethPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_9) as TextView;

            EditText firstDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_1) as EditText;
            EditText secondDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_2) as EditText;
            EditText thirdDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_3) as EditText;
            EditText fourthDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_4) as EditText;
            EditText fifthDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_5) as EditText;
            EditText sixthDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_6) as EditText;
            EditText seventhDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_7) as EditText;
            EditText eigthDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_8) as EditText;
            EditText ninethDigit = linearLayoutContainer.FindViewById(Resource.Id.new_reading_9) as EditText;

            TextView readingErrorMessage = linearLayoutContainer.FindViewById(Resource.Id.reading_error_validation_msg) as TextView;
            TextView readingMeterType = linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type) as TextView;

            readingErrorMessage.Text = "";
            readingMeterType.Text = typeId;

        }
    }
}
