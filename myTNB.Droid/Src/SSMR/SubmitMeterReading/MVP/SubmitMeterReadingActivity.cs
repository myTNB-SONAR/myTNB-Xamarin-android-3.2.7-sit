
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
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public enum METER_READING_TYPE
    {
        KWH,KVARH,KW
    }

    [Activity(Label = "Submit Meter Reading", Theme = "@style/Theme.Dashboard", MainLauncher = true)]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity, SubmitMeterReadingContract.IView
    {
        private METER_READING_TYPE meterType;
        private SubmitMeterReadingPresenter mPresenter;
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
            mPresenter = new SubmitMeterReadingPresenter(this);
            // Create your application here
            TextView meterReadingTitle = FindViewById(Resource.Id.meterReadingTitle) as TextView;
            TextViewUtils.SetMuseoSans500Typeface(meterReadingTitle);

            Button btnTakePhoto = FindViewById(Resource.Id.btnTakePhoto) as Button;
            btnTakePhoto.Click += delegate
            {
                Intent photoIntent = new Intent(this, typeof(SubmitMeterTakePhotoActivity));
                StartActivity(photoIntent);
            };

            SMRActivityInfoResponse sMRActivityInfoResponse = new SMRActivityInfoResponse(); // This should coming from session
            SMRDashboardHistory smrInfo = new SMRDashboardHistory();
            smrInfo.isThreePhaseMeter = "false";
            smrInfo.previousReadingKwh = "";
            smrInfo.previousReadingKvarh = "";
            smrInfo.previousReadingKw = "205.5";

            List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsList = new List<SMRMROValidateRegisterDetails>();
            SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails = new SMRMROValidateRegisterDetails();
            sMRMROValidateRegisterDetails.RegisterNumber = "001";
            sMRMROValidateRegisterDetails.MroID = "0000002432432";
            sMRMROValidateRegisterDetails.PrevMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.SchMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.PrevMeterReading = "1234567";

            if (sMRMROValidateRegisterDetails.RegisterNumber == "001")
            {
                PopulateMeterReadingCard(METER_READING_TYPE.KWH, sMRMROValidateRegisterDetails);
            }
            else if (false)
            {
                PopulateMeterReadingCard(METER_READING_TYPE.KVARH, sMRMROValidateRegisterDetails);
            }
            else
            {
                PopulateMeterReadingCard(METER_READING_TYPE.KW, sMRMROValidateRegisterDetails);
            }

            List<MeterReading> meterReadingList = new List<MeterReading>();
            MeterReading meterReading = new MeterReading();
            meterReading.MroID = "00000000019018990405";
            meterReading.RegisterNumber = "001";
            meterReading.MeterReadingResult = "230";
            meterReadingList.Add(meterReading);
            mPresenter.SubmitMeterReading("220678784308",true, meterReadingList);
        }

        public void PopulateMeterReadingCard(METER_READING_TYPE type, SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails)
        {
            LinearLayout linearLayoutContainer;
            string typeId;

            switch (type)
            {
                case METER_READING_TYPE.KWH:
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    linearLayoutContainer.SetOnClickListener(new OnCardClickListener());
                    typeId = "kWh";
                    break;
                case METER_READING_TYPE.KVARH:
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    typeId = "kVARh";
                    break;
                default:
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    typeId = "kW";
                    break;
            }

            char[] amountInArray = sMRMROValidateRegisterDetails.PrevMeterReading.ToCharArray();


            TextView firstPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_1) as TextView;
            TextView secondPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_2) as TextView;
            TextView thirdPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_3) as TextView;
            TextView fourthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_4) as TextView;
            TextView fifthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_5) as TextView;
            TextView sixthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_6) as TextView;
            TextView seventhPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_7) as TextView;
            TextView eigthPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_8) as TextView;
            TextView ninethPrevDigit = linearLayoutContainer.FindViewById(Resource.Id.previous_reading_9) as TextView;

            int[] resourceIdList = new int[9];
            resourceIdList[0] = Resource.Id.previous_reading_1;
            resourceIdList[1] = Resource.Id.previous_reading_2;
            resourceIdList[2] = Resource.Id.previous_reading_3;
            resourceIdList[3] = Resource.Id.previous_reading_4;
            resourceIdList[4] = Resource.Id.previous_reading_5;
            resourceIdList[5] = Resource.Id.previous_reading_6;
            resourceIdList[6] = Resource.Id.previous_reading_7;
            resourceIdList[7] = Resource.Id.previous_reading_8;
            resourceIdList[8] = Resource.Id.previous_reading_9;
            int resourceCounter = 0;
            for (int i = amountInArray.Length; i != 0; i--)
            {
                resourceCounter++;
                if (resourceCounter > 9) //When more than 9 digits
                {
                    break;
                }
                TextView previousValueDigit = linearLayoutContainer.FindViewById(resourceIdList[resourceIdList.Length - resourceCounter]) as TextView;
                previousValueDigit.Text = amountInArray[i-1].ToString();
            }

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

            TextViewUtils.SetMuseoSans300Typeface(firstPrevDigit, secondPrevDigit, thirdPrevDigit, fourthPrevDigit, fifthPrevDigit, sixthPrevDigit,
                seventhPrevDigit, eigthPrevDigit, ninethPrevDigit);
            TextViewUtils.SetMuseoSans500Typeface(readingMeterType, readingErrorMessage, firstDigit, secondDigit, thirdDigit, fourthDigit,
                fifthDigit, sixthDigit, seventhDigit, eigthDigit, ninethDigit);


            readingErrorMessage.Visibility = ViewStates.Gone;
            readingMeterType.Text = typeId;

        }

        public class OnCardClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public void OnClick(View v)
            {
                //throw new NotImplementedException();
            }
        }

    }
}
