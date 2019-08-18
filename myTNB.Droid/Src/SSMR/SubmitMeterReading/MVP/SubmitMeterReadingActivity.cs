
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CheeseBind;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SSMRBase.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Listener;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.SSMRTerminate.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public enum METER_READING_TYPE
    {
        KWH,KVARH,KW
    }

    [Activity(Label = "@string/meter_reading_title", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity, SubmitMeterReadingContract.IView
    {
        private METER_READING_TYPE meterType;
        private SubmitMeterReadingPresenter mPresenter;
        private int[] previousMeterViews;
        private int[] currentMeterViews;
        private int MAX_DIGIT = 8;
        private List<MeterValidation> validationStateList;


        [BindView(Resource.Id.meterReadingTitle)]
		TextView meterReadingTitle;

		[BindView(Resource.Id.meterReadingNote)]
		TextView meterReadingNote;

		[BindView(Resource.Id.btnSubmitReading)]
        Button btnSubmitReading;

        [BindView(Resource.Id.btnTakePhoto)]
        Button btnTakePhoto;

        [BindView(Resource.Id.meterReadingError)]
        TextView meterReadingError;

        public readonly static int SSMR_SUBMIT_METER_ACTIVITY_CODE = 8796;
        public readonly static int SSMR_SUBMIT_METER_OCR_SUBMIT_CODE = 8797;
        private IMenu ssmrMenu;
        private static bool isFirstLaunch = true;

        private static List<SSMRMeterReadingModel> singlePhaseList;
        private static List<SSMRMeterReadingModel> threePhaseList;

        LoadingOverlay loadingOverlay;

        List <SMRMROValidateRegisterDetails> SMRValidateRegisterDetailList;
        AccountData selectedAccount;
        SMRActivityInfoResponse ssmrActivityInfoResponse;
        List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsResponse;

        public override int ResourceId()
        {
            return Resource.Layout.SubmitMeterReadingLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool CameraPermissionRequired()
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
			meterReadingTitle.TextFormatted = GetFormattedText(GetString(Resource.String.ssmr_submit_meter_reading_message));
			meterReadingNote.TextFormatted = GetFormattedText(GetString(Resource.String.ssmr_submit_meter_reading_note));

			TextViewUtils.SetMuseoSans300Typeface(meterReadingTitle,meterReadingNote);
            TextViewUtils.SetMuseoSans500Typeface(meterReadingError, btnTakePhoto, btnSubmitReading);

            btnTakePhoto.Click += delegate
            {
                Intent photoIntent = new Intent(this, typeof(SubmitMeterTakePhotoActivity));
                photoIntent.PutExtra("REQUEST_PHOTOS",JsonConvert.SerializeObject(validationStateList));
                photoIntent.PutExtra("IS_SINGLE_PHASE", validationStateList.Count == 1 ? true : false);
                photoIntent.PutExtra("CONTRACT_NUMBER", selectedAccount.AccountNum); //Mock
                StartActivityForResult(photoIntent, SSMR_SUBMIT_METER_OCR_SUBMIT_CODE);
            };

            btnSubmitReading.Click += delegate
            {
                string contractAccount = selectedAccount.AccountNum;
                bool isOwnedAccount = true;
                ShowProgressDialog();
                List<MeterReading> meterReadlingList = new List<MeterReading>();
                foreach (MeterValidation validatedMeter in validationStateList)
                {
                    MeterReading meterReading = new MeterReading();
                    meterReading.MroID = validatedMeter.mroID;
                    meterReading.RegisterNumber = validatedMeter.meterId;
                    meterReading.MeterReadingResult = validatedMeter.readingResult;
                    meterReading.Channel = "MyTNBAPP";
                    meterReading.MeterReadingDate = "";
                    meterReading.MeterReadingTime = "";
                    meterReadlingList.Add(meterReading);
                }
                mPresenter.SubmitMeterReading(contractAccount, isOwnedAccount, meterReadlingList);
            };

            EnableSubmitButton(false);
            TextViewUtils.SetMuseoSans500Typeface(btnTakePhoto, btnSubmitReading);

            previousMeterViews = new int[8];
            previousMeterViews[0] = Resource.Id.previous_reading_1;
            previousMeterViews[1] = Resource.Id.previous_reading_2;
            previousMeterViews[2] = Resource.Id.previous_reading_3;
            previousMeterViews[3] = Resource.Id.previous_reading_4;
            previousMeterViews[4] = Resource.Id.previous_reading_5;
            previousMeterViews[5] = Resource.Id.previous_reading_6;
            previousMeterViews[6] = Resource.Id.previous_reading_7;
            previousMeterViews[7] = Resource.Id.previous_reading_8;

            currentMeterViews = new int[8];
            currentMeterViews[0] = Resource.Id.new_reading_1;
            currentMeterViews[1] = Resource.Id.new_reading_2;
            currentMeterViews[2] = Resource.Id.new_reading_3;
            currentMeterViews[3] = Resource.Id.new_reading_4;
            currentMeterViews[4] = Resource.Id.new_reading_5;
            currentMeterViews[5] = Resource.Id.new_reading_6;
            currentMeterViews[6] = Resource.Id.new_reading_7;
            currentMeterViews[7] = Resource.Id.new_reading_8;

            EditText editText;
            List<EditText> editTextList;
            MeterReadingValuesViewHelper editViewHelper;

            //KWH
            LinearLayout kwHContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
            editText = kwHContainer.FindViewById<EditText>(Resource.Id.new_reading_8);
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
            editText = kVARhContainer.FindViewById<EditText>(Resource.Id.new_reading_8);
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
            editText = kwContainer.FindViewById<EditText>(Resource.Id.new_reading_8);
            editViewHelper = new MeterReadingValuesViewHelper(this, Resource.Id.kwCard);
            editTextList = new List<EditText>();
            for (int i = 0; i < currentMeterViews.Length; i++)
            {
                editTextList.Add((EditText)kwContainer.FindViewById(currentMeterViews[i]));
            }
            editViewHelper.SetEditTextList(editTextList);
            editViewHelper.SetEvent();

            //List<EditText> editTextList;

            ////KWH
            //LinearLayout kwHContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
            //editTextList = new List<EditText>();
            //for (int i = 0; i < currentMeterViews.Length; i++)
            //{
            //    editTextList.Add((EditText)kwHContainer.FindViewById(currentMeterViews[i]));
            //}
            //MeterInputTextWatcher.SetEditTextList(editTextList);

            ////KVARH
            //LinearLayout kVARhContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
            //editTextList = new List<EditText>();
            //for (int i = 0; i < currentMeterViews.Length; i++)
            //{
            //    editTextList.Add((EditText)kVARhContainer.FindViewById(currentMeterViews[i]));
            //}
            //MeterInputTextWatcher.SetEditTextList(editTextList);

            ////KW
            //LinearLayout kwContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
            //editTextList = new List<EditText>();
            //for (int i = 0; i < currentMeterViews.Length; i++)
            //{
            //    editTextList.Add((EditText)kwContainer.FindViewById(currentMeterViews[i]));
            //}
            //MeterInputTextWatcher.SetEditTextList(editTextList);
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == SSMR_SUBMIT_METER_OCR_SUBMIT_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Bundle extras = data.Extras;
                    mPresenter.EvaluateOCRReadingResponse(extras.GetString("OCR_RESULTS"));
                }
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        private void SetUpMeterCards(List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsResponse)
        {
            MeterValidation meterValidation;
            foreach (SMRMROValidateRegisterDetails validateRegisterDetails in sMRMROValidateRegisterDetailsResponse)
            {
                if (validateRegisterDetails.RegisterNumber == "001")
                {
                    meterValidation = new MeterValidation();
                    meterValidation.meterId = validateRegisterDetails.RegisterNumber;
                    meterValidation.mroID = validateRegisterDetails.MroID;
                    validationStateList.Add(meterValidation);
                    PopulateMeterReadingCard(METER_READING_TYPE.KWH, validateRegisterDetails);
                }
                else if (validateRegisterDetails.RegisterNumber == "002")
                {
                    meterValidation = new MeterValidation();
                    meterValidation.meterId = "002";
                    validationStateList.Add(meterValidation);
                    PopulateMeterReadingCard(METER_READING_TYPE.KVARH, validateRegisterDetails);
                }
                else if (validateRegisterDetails.RegisterNumber == "003")
                {
                    meterValidation = new MeterValidation();
                    meterValidation.meterId = "003";
                    validationStateList.Add(meterValidation);
                    PopulateMeterReadingCard(METER_READING_TYPE.KW, validateRegisterDetails);
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new SubmitMeterReadingPresenter(this);
            InitializePage();
            isFirstLaunch = true;

            validationStateList = new List<MeterValidation>();
            //Mock Data - Start
            SMRValidateRegisterDetailList = DummyData();
            SetUpMeterCards(SMRValidateRegisterDetailList);
            selectedAccount = new AccountData();
            selectedAccount.AccountNum = "1010101010";
            //Mock Data - End
            //Bundle intentExtras = Intent.Extras;

            //if (intentExtras.ContainsKey(Constants.SELECTED_ACCOUNT))
            //{
            //    selectedAccount = JsonConvert.DeserializeObject<AccountData>(intentExtras.GetString(Constants.SELECTED_ACCOUNT));
            //}

            //if (intentExtras.ContainsKey(Constants.SMR_RESPONSE_KEY))
            //{
            //    ssmrActivityInfoResponse = JsonConvert.DeserializeObject<SMRActivityInfoResponse>(intentExtras.GetString(Constants.SMR_RESPONSE_KEY));
            //    if (ssmrActivityInfoResponse.Response != null && ssmrActivityInfoResponse.Response.Data != null)
            //    {
            //        SMRValidateRegisterDetailList = ssmrActivityInfoResponse.Response.Data.SMRMROValidateRegisterDetails;
            //        SetUpMeterCards(SMRValidateRegisterDetailList);
            //    }
            //}
            OnGenerateTooltipData();
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
            char[] amountInArray = (sMRMROValidateRegisterDetails.OCRValue).ToCharArray();
            ((EditText)container.FindViewById(currentMeterViews[7])).Text = "";
            ((EditText)container.FindViewById(currentMeterViews[6])).Text = "";
            ((EditText)container.FindViewById(currentMeterViews[5])).Text = "";
            ((EditText)container.FindViewById(currentMeterViews[4])).Text = "";
            ((EditText)container.FindViewById(currentMeterViews[3])).Text = "";
            ((EditText)container.FindViewById(currentMeterViews[2])).Text = "";
            ((EditText)container.FindViewById(currentMeterViews[1])).Text = "";
            ((EditText)container.FindViewById(currentMeterViews[0])).Text = "";
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
                    ValidateMeterInput(Resource.Id.kwhCard);
                }else if (type == "kvarh")
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                    ValidateMeterInput(Resource.Id.kVARhCard);
                }
                else if (type == "kw")
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                    ValidateMeterInput(Resource.Id.kwCard);
                }
            }

        }

        public void ValidateMeterInput(int meterCardResourceId)
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            var currentFocus = this.CurrentFocus;
            if (currentFocus != null)
            {
              inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
            LinearLayout meterCardContainer = FindViewById(meterCardResourceId) as LinearLayout;

            EditText currentValue7 = (EditText)meterCardContainer.FindViewById(currentMeterViews[7]);
            EditText currentValue6 = (EditText)meterCardContainer.FindViewById(currentMeterViews[6]);
            EditText currentValue5 = (EditText)meterCardContainer.FindViewById(currentMeterViews[5]);
            EditText currentValue4 = (EditText)meterCardContainer.FindViewById(currentMeterViews[4]);
            EditText currentValue3 = (EditText)meterCardContainer.FindViewById(currentMeterViews[3]);
            EditText currentValue2 = (EditText)meterCardContainer.FindViewById(currentMeterViews[2]);
            EditText currentValue1 = (EditText)meterCardContainer.FindViewById(currentMeterViews[1]);
            EditText currentValue0 = (EditText)meterCardContainer.FindViewById(currentMeterViews[0]);

            string currentAmountInString = currentValue0.Text + currentValue1.Text + currentValue2.Text + currentValue3.Text + currentValue4.Text +
                currentValue5.Text + currentValue6.Text + currentValue7.Text;

            currentValue7.Text = "";
            currentValue6.Text = "";
            currentValue5.Text = "";
            currentValue4.Text = "";
            currentValue3.Text = "";
            currentValue2.Text = "";
            currentValue1.Text = "";
            currentValue0.Text = "";

            char[] parsedReading = currentAmountInString.ToCharArray();
            int resourceCounter = 0;
            for (int i = parsedReading.Length; i != 0; i--)
            {
                resourceCounter++;
                EditText currentValueDigit = meterCardContainer.FindViewById(currentMeterViews[currentMeterViews.Length - resourceCounter]) as EditText;
                if (resourceCounter > MAX_DIGIT) //When more than 9 digits
                {
                    break;
                }
                if ((currentMeterViews.Length - resourceCounter) == 7)
                {
                    if (currentValueDigit.Text == "")
                    {
                        i = parsedReading.Length;
                    }
                    else
                    {
                        continue;
                    }
                }

                currentValueDigit.Text = parsedReading[i - 1].ToString();
            }


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
                validationStateList.Find(meter =>
                {
                    return meter.meterId == RegisterNumber;
                }).validated = false;
            }
            else
            {
                meterType.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_reading_label_background_ready));
                validationStateList.Find(meter =>
                {
                    return meter.meterId == RegisterNumber;
                }).validated = true;
            }
            UpdateCurrentReadingValuesColor(meterCardContainer, currentReading != 0);
            EnableSubmitButton(validationStateList.TrueForAll(meter => { return meter.validated == true; }));
        }

        public void ShowMeterReadingOCRError(string errorMessage)
        {
            if (errorMessage != "")
            {
                meterReadingError.Visibility = ViewStates.Visible;
                meterReadingError.Text = errorMessage;
            }
            else
            {
                meterReadingError.Visibility = ViewStates.Gone;
            }
        }

        public void EnableSubmitButton(bool isEnabled)
        {
            if (isEnabled)
            {
                btnSubmitReading.Enabled = true;
                btnSubmitReading.Background = GetDrawable(Resource.Drawable.green_button_background);
            }
            else
            {
                btnSubmitReading.Enabled = false;
                btnSubmitReading.Background = GetDrawable(Resource.Drawable.silver_chalice_button_background);
            }
            TextViewUtils.SetMuseoSans500Typeface(btnTakePhoto, btnSubmitReading);
        }

        public void OnRequestSuccessful(SMRSubmitResponseData response)
        {
            Intent intent = new Intent(this, typeof(ResponseSuccessActivity));
            if (response != null)
            {
                intent.PutExtra("SUBMIT_RESULT", JsonConvert.SerializeObject(response));
            }
            StartActivityForResult(intent, SSMR_SUBMIT_METER_ACTIVITY_CODE);
        }

        public void OnRequestFailed(SMRSubmitResponseData response)
        {
            Intent intent = new Intent(this, typeof(ResponseFailedActivity));
            if (response != null)
            {
                intent.PutExtra("SUBMIT_RESULT", JsonConvert.SerializeObject(response));
            }
            StartActivityForResult(intent, SSMR_SUBMIT_METER_ACTIVITY_CODE);
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

        public void ShowMeterCardValidationError(List<MeterValidationData> validationDataList)
        {
            HideProgressDialog();
            if (validationDataList.Count > 0)
            {
                foreach(MeterValidationData validationData in validationDataList)
                {
                    LinearLayout linearLayoutContainer;
                    TextView inlineValidationMessage;
                    string type = validationData.registerNumber;
                    if (type == "001")
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                        inlineValidationMessage = linearLayoutContainer.FindViewById<TextView>(Resource.Id.reading_error_validation_msg);
                        UpdateCurrentReadingValuesColor(linearLayoutContainer, validationData.isSuccess);
                    }
                    else if (type == "002")
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                        inlineValidationMessage = linearLayoutContainer.FindViewById<TextView>(Resource.Id.reading_error_validation_msg);
                        UpdateCurrentReadingValuesColor(linearLayoutContainer, validationData.isSuccess);
                    }
                    else //(type == "003")
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                        inlineValidationMessage = linearLayoutContainer.FindViewById<TextView>(Resource.Id.reading_error_validation_msg);
                        UpdateCurrentReadingValuesColor(linearLayoutContainer, validationData.isSuccess);
                    }

                    if (validationData.isSuccess)
                    {
                        inlineValidationMessage.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        inlineValidationMessage.Text = validationData.message;
                        inlineValidationMessage.Visibility = ViewStates.Visible;
                        TextViewUtils.SetMuseoSans500Typeface(inlineValidationMessage);
                    }
                }
            }
        }

        public void UpdateCurrentReadingValuesColor(LinearLayout linearLayout, bool isSuccess)
        {
            for (int i = 0; i < currentMeterViews.Length; i++)
            {
                EditText editText =  (EditText)linearLayout.FindViewById(currentMeterViews[i]);
                editText.SetTextColor(isSuccess ? Color.ParseColor("#20bd4c") : Color.ParseColor("#e44b21"));
            }
            TextView meterTypeView = (TextView)linearLayout.FindViewById(Resource.Id.reading_meter_type);
            meterTypeView.SetBackgroundResource(isSuccess ? Resource.Drawable.meter_reading_label_background_ready : Resource.Drawable.meter_reading_label_background_error);
        }
    }
}
