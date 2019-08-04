
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
using myTNB_Android.Src.SSMR.Util;
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

    [Activity(Label = "@string/meter_reading_title", Theme = "@style/Theme.Dashboard", MainLauncher = true)]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity, SubmitMeterReadingContract.IView
    {
        private METER_READING_TYPE meterType;
        private SubmitMeterReadingPresenter mPresenter;
        private int[] previousMeterViews;
        private int[] currentMeterViews;
        private int MAX_DIGIT = 9;

        List<SMRMROValidateRegisterDetails> SMRValidateRegisterDetailList;

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

            EditText editText;
            List<EditText> editTextList;
            MeterReadingValuesViewHelper editViewHelper;

            //KWH
            LinearLayout kwHContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
            editText = kwHContainer.FindViewById<EditText>(Resource.Id.new_reading_9);
            editViewHelper = new MeterReadingValuesViewHelper(this, Resource.Id.kwhCard);

            editTextList = new List<EditText>();
            for (int i = 0; i < currentMeterViews.Length; i++)
            {
                editTextList.Add((EditText)kwHContainer.FindViewById(currentMeterViews[i]));
            }
            editViewHelper.SetEditTextList(editTextList);
            editViewHelper.SetEvent();

            //KVARH
            LinearLayout kVARhContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
            editText = kVARhContainer.FindViewById<EditText>(Resource.Id.new_reading_9);
            editViewHelper = new MeterReadingValuesViewHelper(this, Resource.Id.kVARhCard);

            editTextList = new List<EditText>();
            for (int i = 0; i < currentMeterViews.Length; i++)
            {
                editTextList.Add((EditText)kVARhContainer.FindViewById(currentMeterViews[i]));
            }
            editViewHelper.SetEditTextList(editTextList);
            editViewHelper.SetEvent();

            //KW
            LinearLayout kwContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
            editText = kwContainer.FindViewById<EditText>(Resource.Id.new_reading_9);
            editViewHelper = new MeterReadingValuesViewHelper(this, Resource.Id.kwCard);
            editTextList = new List<EditText>();
            for (int i = 0; i < currentMeterViews.Length; i++)
            {
                editTextList.Add((EditText)kwContainer.FindViewById(currentMeterViews[i]));
            }
            editViewHelper.SetEditTextList(editTextList);
            editViewHelper.SetEvent();
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (Intent != null)
            {
                List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsList = DummyData(); //This is coming from session
                SMRValidateRegisterDetailList = DummyData(); //Mock

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

        private void PopulateCurrentValues(LinearLayout container, SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails)
        {
            List<EditText> editTextList = new List<EditText>();
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
                editTextList.Add(currentValueDigit);
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
            TextView meterType;
            switch (type)
            {
                case METER_READING_TYPE.KWH:
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterType = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterType.Text = "kWh";
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                case METER_READING_TYPE.KVARH:
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterType = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterType.Text = "kVARh";
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                default:
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterType = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterType.Text = "kW";
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
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

        public void ValidateMeterInput(int meterCardResourceId)
        {
            LinearLayout meterCardContainer = FindViewById(meterCardResourceId) as LinearLayout;

            EditText currentValue8 = (EditText)meterCardContainer.FindViewById(currentMeterViews[8]);
            EditText currentValue7 = (EditText)meterCardContainer.FindViewById(currentMeterViews[7]);
            EditText currentValue6 = (EditText)meterCardContainer.FindViewById(currentMeterViews[6]);
            EditText currentValue5 = (EditText)meterCardContainer.FindViewById(currentMeterViews[5]);
            EditText currentValue4 = (EditText)meterCardContainer.FindViewById(currentMeterViews[4]);
            EditText currentValue3 = (EditText)meterCardContainer.FindViewById(currentMeterViews[3]);
            EditText currentValue2 = (EditText)meterCardContainer.FindViewById(currentMeterViews[2]);
            EditText currentValue1 = (EditText)meterCardContainer.FindViewById(currentMeterViews[1]);
            EditText currentValue0 = (EditText)meterCardContainer.FindViewById(currentMeterViews[0]);

            string currentAmountInString = currentValue0.Text + currentValue1.Text + currentValue2.Text + currentValue3.Text + currentValue4.Text +
                currentValue5.Text + currentValue6.Text + currentValue7.Text + currentValue8.Text;

            string RegisterNumber = "";
            if (meterCardResourceId == Resource.Id.kwhCard)
            {
                RegisterNumber = "001";
            }
            else if(meterCardResourceId == Resource.Id.kVARhCard)
            {
                RegisterNumber = "002";
            }
            else if (meterCardResourceId == Resource.Id.kwCard)
            {
                RegisterNumber = "003";
            }

            SMRMROValidateRegisterDetails validateRegisterDetails = SMRValidateRegisterDetailList.Find(detail =>
            {
                return detail.RegisterNumber == RegisterNumber;
            });
            int previousReading = 0;
            int currentReading = 0;
            
            if (validateRegisterDetails.PrevMeterReading != "")
            {
                previousReading = Int32.Parse(validateRegisterDetails.PrevMeterReading);
            }
            if (currentAmountInString != "")
            {
                currentReading = Int32.Parse(currentAmountInString);
            }

            TextView inlineError = (TextView) meterCardContainer.FindViewById(Resource.Id.reading_error_validation_msg);
            TextView meterType = (TextView)meterCardContainer.FindViewById(Resource.Id.reading_meter_type);
            if (currentReading == 0)
            {
                inlineError.Visibility = ViewStates.Gone;
                meterType.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_reading_label_background));
            }
            else
            {
                if (currentReading > previousReading)
                {
                    inlineError.Text = "This value is too high!";
                    inlineError.Visibility = ViewStates.Visible;
                    meterType.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_reading_label_background_error));
                }
                else
                {
                    inlineError.Visibility = ViewStates.Gone;
                    meterType.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_reading_label_background_ready));
                }
            }

            //reading_error_validation_msg

            //reading_meter_type

        }

    }
}
