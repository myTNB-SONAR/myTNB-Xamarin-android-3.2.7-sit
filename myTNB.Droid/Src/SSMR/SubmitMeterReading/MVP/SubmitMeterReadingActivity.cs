
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
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public enum METER_READING_TYPE
    {
        KWH,KVARH,KW
    }

    [Activity(Label = "@string/meter_reading_title", Theme = "@style/Theme.Dashboard" )]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity, SubmitMeterReadingContract.IView
    {
        private METER_READING_TYPE meterType;
        private SubmitMeterReadingPresenter mPresenter;
        private int[] previousMeterViews;
        private int[] currentMeterViews;
        private int MAX_DIGIT = 9;
        private IMenu ssmrMenu;
        private static bool isFirstLaunch = true;

        private static List<SSMRMeterReadingModel> singlePhaseList;
        private static List<SSMRMeterReadingModel> threePhaseList;

        LoadingOverlay loadingOverlay;

        List <SMRMROValidateRegisterDetails> SMRValidateRegisterDetailList;

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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SSMRMeterSubmitMenu, menu);
            ssmrMenu = menu;
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_ssmr_meter_reading_more:
                    ShowMeterReadingTooltip();
                    break;
            }
            return base.OnOptionsItemSelected(item);
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

            OnGenerateTooltipData();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new SubmitMeterReadingPresenter(this);
            InitializePage();
            isFirstLaunch = true;
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
            switch (type)
            {
                case METER_READING_TYPE.KWH:
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    //PopulateCurrentValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                case METER_READING_TYPE.KVARH:
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    //PopulateCurrentValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                default:
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    //PopulateCurrentValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
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


        }

        private void OnGenerateTooltipData()
        {
            if (SMRValidateRegisterDetailList.Count > 1)
            {
                if (threePhaseList == null)
                {
                    threePhaseList = new List<SSMRMeterReadingModel>();
                    this.mPresenter.OnGetThreePhaseData();
                }
                else
                {
                    if (!MyTNBAccountManagement.GetInstance().GetSMRMeterReadingThreePhaseOnboardingShown() && isFirstLaunch)
                    {
                        ShowMeterReadingTooltip();
                        isFirstLaunch = false;
                    }
                }
            }
            else
            {
                if (singlePhaseList == null)
                {
                    singlePhaseList = new List<SSMRMeterReadingModel>();
                    this.mPresenter.OnGetOnePhaseData();
                }
                else
                {
                    if (!MyTNBAccountManagement.GetInstance().GetSMRMeterReadingOnePhaseOnboardingShown() && isFirstLaunch)
                    {
                        ShowMeterReadingTooltip();
                        isFirstLaunch = false;
                    }
                }
            }
        }

        public void OnUpdateThreePhaseTooltipData(List<SSMRMeterReadingModel> list)
        {
            if (list != null && list.Count > 0)
            {
                threePhaseList = list;
            }
            HideProgressDialog();
            if (!MyTNBAccountManagement.GetInstance().GetSMRMeterReadingThreePhaseOnboardingShown() && isFirstLaunch)
            {
                ShowMeterReadingTooltip();
                isFirstLaunch = false;
            }
        }

        public void OnUpdateOnePhaseTooltipData(List<SSMRMeterReadingModel> list)
        {
            if (list != null && list.Count > 0)
            {
                singlePhaseList = list;
            }
            HideProgressDialog();
            if (!MyTNBAccountManagement.GetInstance().GetSMRMeterReadingOnePhaseOnboardingShown() && isFirstLaunch)
            {
                ShowMeterReadingTooltip();
                isFirstLaunch = false;
            }
        }

        private void ShowMeterReadingTooltip()
        {
            if (SMRValidateRegisterDetailList.Count > 1)
            {
                SMRPopUpUtils.OnShowSMRMeterReadingTooltipOnActivity(false, this, SupportFragmentManager, threePhaseList);
            }
            else
            {
                SMRPopUpUtils.OnShowSMRMeterReadingTooltipOnActivity(true, this, SupportFragmentManager, singlePhaseList);
            }
        }

        public void ShowProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}
