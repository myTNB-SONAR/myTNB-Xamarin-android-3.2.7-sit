
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
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public enum METER_READING_TYPE
    {
        KWH,KVARH,KW
    }

    [Activity(Label = "@string/meter_reading_title", Theme = "@style/Theme.Dashboard")]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity, SubmitMeterReadingContract.IView
    {
        private METER_READING_TYPE meterType;
        private SubmitMeterReadingPresenter mPresenter;
        private int[] previousMeterViews;
        private int[] currentMeterViews;
        private int MAX_DIGIT = 9;

        public override int ResourceId()
        {
            return Resource.Layout.SubmitMeterReadingLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public List<SMRMROValidateRegisterDetails> DummyData()
        {
            List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsList = new List<SMRMROValidateRegisterDetails>();
            SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails = new SMRMROValidateRegisterDetails();
            sMRMROValidateRegisterDetails.RegisterNumber = "001";
            sMRMROValidateRegisterDetails.MroID = "0000002432432";
            sMRMROValidateRegisterDetails.PrevMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.SchMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.PrevMeterReading = "1234567";
            sMRMROValidateRegisterDetailsList.Add(sMRMROValidateRegisterDetails);

            sMRMROValidateRegisterDetails = new SMRMROValidateRegisterDetails();
            sMRMROValidateRegisterDetails.RegisterNumber = "002";
            sMRMROValidateRegisterDetails.MroID = "0000002432432";
            sMRMROValidateRegisterDetails.PrevMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.SchMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.PrevMeterReading = "1234567";
            sMRMROValidateRegisterDetailsList.Add(sMRMROValidateRegisterDetails);

            sMRMROValidateRegisterDetails = new SMRMROValidateRegisterDetails();
            sMRMROValidateRegisterDetails.RegisterNumber = "003";
            sMRMROValidateRegisterDetails.MroID = "0000002432432";
            sMRMROValidateRegisterDetails.PrevMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.SchMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.PrevMeterReading = "1234567";
            sMRMROValidateRegisterDetailsList.Add(sMRMROValidateRegisterDetails);

            return sMRMROValidateRegisterDetailsList;
        }

        private void InitializePage()
        {
            TextView meterReadingTitle = FindViewById(Resource.Id.meterReadingTitle) as TextView;
            meterReadingTitle.Text = GetString(Resource.String.meter_reading_message);
            TextViewUtils.SetMuseoSans500Typeface(meterReadingTitle);

            Button btnTakePhoto = FindViewById(Resource.Id.btnTakePhoto) as Button;
            btnTakePhoto.Click += delegate
            {
                Intent photoIntent = new Intent(this, typeof(SubmitMeterTakePhotoActivity));
                StartActivity(photoIntent);
            };

            previousMeterViews = new int[9];
            previousMeterViews[0] = Resource.Id.previous_reading_1;
            previousMeterViews[1] = Resource.Id.previous_reading_2;
            previousMeterViews[2] = Resource.Id.previous_reading_3;
            previousMeterViews[3] = Resource.Id.previous_reading_4;
            previousMeterViews[4] = Resource.Id.previous_reading_5;
            previousMeterViews[5] = Resource.Id.previous_reading_6;
            previousMeterViews[6] = Resource.Id.previous_reading_7;
            previousMeterViews[7] = Resource.Id.previous_reading_8;
            previousMeterViews[8] = Resource.Id.previous_reading_9;

            currentMeterViews = new int[9];
            currentMeterViews[0] = Resource.Id.new_reading_1;
            currentMeterViews[1] = Resource.Id.new_reading_2;
            currentMeterViews[2] = Resource.Id.new_reading_3;
            currentMeterViews[3] = Resource.Id.new_reading_4;
            currentMeterViews[4] = Resource.Id.new_reading_5;
            currentMeterViews[5] = Resource.Id.new_reading_6;
            currentMeterViews[6] = Resource.Id.new_reading_7;
            currentMeterViews[7] = Resource.Id.new_reading_8;
            currentMeterViews[8] = Resource.Id.new_reading_9;
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (Intent != null)
            {
                List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsList = DummyData(); //This is coming from session

                foreach (SMRMROValidateRegisterDetails validateRegisterDetails in sMRMROValidateRegisterDetailsList)
                {
                    if (validateRegisterDetails.RegisterNumber == "001")
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KWH, validateRegisterDetails);
                    }
                    else if (validateRegisterDetails.RegisterNumber == "002")
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KVARH, validateRegisterDetails);
                    }
                    else if (validateRegisterDetails.RegisterNumber == "003")
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KW, validateRegisterDetails);
                    }
                    else
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KWH, validateRegisterDetails);
                    }
                }

                string ocrResultList = Intent.GetStringExtra("OCR_RESULTS");
                if (ocrResultList != null)
                {
                    mPresenter.EvaluateOCRReadingResponse(ocrResultList);
                }
            }
            else
            {
                List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsList = DummyData(); //This is coming from session

                foreach (SMRMROValidateRegisterDetails validateRegisterDetails in sMRMROValidateRegisterDetailsList)
                {
                    if (validateRegisterDetails.RegisterNumber == "001")
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KWH, validateRegisterDetails);
                    }
                    else if (validateRegisterDetails.RegisterNumber == "002")
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KVARH, validateRegisterDetails);
                    }
                    else if (validateRegisterDetails.RegisterNumber == "003")
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KW, validateRegisterDetails);
                    }
                    else
                    {
                        PopulateMeterReadingCard(METER_READING_TYPE.KWH, validateRegisterDetails);
                    }
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new SubmitMeterReadingPresenter(this);
            InitializePage();
        }

        private string GetType(string registerNumber)
        {
            switch (registerNumber)
            {
                case "001":
                    return GetString(Resource.String.meter_kwh);
                case "002":
                    return GetString(Resource.String.meter_kVARh);
                default:
                    return GetString(Resource.String.meter_kW);
            }
        }

        private void PopulatePreviousValues(LinearLayout container, SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails)
        {
            char[] amountInArray = sMRMROValidateRegisterDetails.PrevMeterReading.ToCharArray();
            TextView[] textViews = new TextView[MAX_DIGIT];
            int resourceCounter = 0;
            for (int i = amountInArray.Length; i != 0; i--)
            {
                resourceCounter++;
                if (resourceCounter > MAX_DIGIT) //When more than 9 digits
                {
                    break;
                }
                TextView previousValueDigit = container.FindViewById(previousMeterViews[previousMeterViews.Length - resourceCounter]) as TextView;
                if (previousValueDigit != null)
                {
                    TextViewUtils.SetMuseoSans300Typeface(previousValueDigit);
                }
                previousValueDigit.Text = amountInArray[i - 1].ToString();
            }
        }

        public class OnEditClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public void OnClick(View v)
            {
                //throw new NotImplementedException();

            }
        }

        public class OnFocusChangeListener : Java.Lang.Object, View.IOnFocusChangeListener
        {
            public void OnFocusChange(View v, bool hasFocus)
            {
                //throw new NotImplementedException();

            }
        }

        private void PopulateCurrentValues(LinearLayout container, SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails)
        {
            char[] amountInArray = sMRMROValidateRegisterDetails.PrevMeterReading.ToCharArray();
            int resourceCounter = 0;
            for (int i = amountInArray.Length; i != 0; i--)
            {
                resourceCounter++;
                if (resourceCounter > MAX_DIGIT) //When more than 9 digits
                {
                    break;
                }
                EditText currentValueDigit = container.FindViewById(currentMeterViews[currentMeterViews.Length - resourceCounter]) as EditText;

                if (currentValueDigit != null)
                {
                    TextViewUtils.SetMuseoSans500Typeface(currentValueDigit);
                }
                currentValueDigit.OnFocusChangeListener = new OnFocusChangeListener();
            }

            TextView readingErrorMessage = container.FindViewById(Resource.Id.reading_error_validation_msg) as TextView;
            TextView readingMeterType = container.FindViewById(Resource.Id.reading_meter_type) as TextView;
            TextViewUtils.SetMuseoSans500Typeface(readingErrorMessage, readingMeterType);
            readingErrorMessage.Visibility = ViewStates.Gone;
            readingMeterType.Text = GetType(sMRMROValidateRegisterDetails.RegisterNumber);
        }

        private void UpdateCurrentValues(LinearLayout container, GetMeterReadingOCRResponseDetails sMRMROValidateRegisterDetails)
        {
            char[] amountInArray = sMRMROValidateRegisterDetails.OCRValue.ToCharArray();
            EditText[] editTexts = new EditText[MAX_DIGIT];
            int resourceCounter = 0;
            for (int i = amountInArray.Length; i != 0; i--)
            {
                resourceCounter++;
                if (resourceCounter > MAX_DIGIT) //When more than 9 digits
                {
                    break;
                }
                EditText currentValueDigit = container.FindViewById(currentMeterViews[currentMeterViews.Length - resourceCounter]) as EditText;
                currentValueDigit.Text = amountInArray[i - 1].ToString();
            }

            TextView readingErrorMessage = container.FindViewById(Resource.Id.reading_error_validation_msg) as TextView;

            if (sMRMROValidateRegisterDetails.IsSuccess.ToLower() == "true")
            {
                readingErrorMessage.Visibility = ViewStates.Gone;
            }
            else
            {
                readingErrorMessage.Visibility = ViewStates.Visible;
            }
        }

        public void PopulateMeterReadingCard(METER_READING_TYPE type, SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails)
        {
            LinearLayout linearLayoutContainer;
            switch (type)
            {
                case METER_READING_TYPE.KWH:
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    linearLayoutContainer.SetOnClickListener(new OnCardClickListener());
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    PopulateCurrentValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                case METER_READING_TYPE.KVARH:
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    PopulateCurrentValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                default:
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    PopulateCurrentValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
            }
        }

        public class OnCardClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public void OnClick(View v)
            {
                //throw new NotImplementedException();
            }
        }

        private string GetRegisterNumber(string data)
        {
            string resultString = "";
            if (data.ToLower() == "kwh")
            {
                return "001";
            }
            else if (data.ToLower() == "kvarh")
            {
                return "002";
            }
            else if (data.ToLower() == "kw")
            {
                return "003";
            }
            return resultString;
        }

        public void UpdateCurrentMeterReading(List<GetMeterReadingOCRResponseDetails> ocrMeterReadingList)
        {
            foreach (GetMeterReadingOCRResponseDetails ocrResponse in ocrMeterReadingList)
            {
                LinearLayout linearLayoutContainer;
                string type = ocrResponse.OCRUnit.ToLower();
                if (type == "kwh")
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                }else if (type == "kvarh")
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                }else if (type == "kw")
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                }
            }

        }

    }
}
