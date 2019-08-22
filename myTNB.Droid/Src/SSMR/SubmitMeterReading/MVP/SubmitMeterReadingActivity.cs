
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

    [Activity(Label = "@string/meter_reading_title", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
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

        [BindView(Resource.Id.meterReadingContent)]
        LinearLayout meterReadingContent;

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
        List<MeterReadingInputLayout> meterReadingInputLayoutList;

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
                ShowProgressDialog();
                string contractAccount = selectedAccount.AccountNum;
                bool isOwnedAccount = true;
                List<MeterReading> meterReadlingList = new List<MeterReading>();
                MeterReading meterReading;
                
                validationStateList.ForEach(validatedMeter => {
                    meterReading = new MeterReading();
                    meterReading.MroID = validatedMeter.mroID;
                    meterReading.RegisterNumber = validatedMeter.registerNumber;
                    MeterReadingInputLayout readingInput = meterReadingInputLayoutList.Find(meterInput => meterInput.GetMeterId() == validatedMeter.meterId);
                    meterReading.MeterReadingResult = readingInput.GetMeterReadingInput();
                    meterReading.Channel = "MyTNBAPP";
                    meterReading.MeterReadingDate = "";
                    meterReading.MeterReadingTime = "";

                    if (readingInput.GetMeterReadingInput() != "")
                    {
                        meterReadlingList.Add(meterReading);
                    }
                });
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
            meterReadingInputLayoutList = new List<MeterReadingInputLayout>();
            foreach (SMRMROValidateRegisterDetails validateRegisterDetails in sMRMROValidateRegisterDetailsResponse)
            {
                if (validateRegisterDetails.ReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KWH)
                {
                    meterValidation = new MeterValidation();
                    meterValidation.registerNumber = validateRegisterDetails.RegisterNumber;
                    meterValidation.meterId = validateRegisterDetails.ReadingUnit.ToUpper();
                    meterValidation.mroID = validateRegisterDetails.MroID;
                    validationStateList.Add(meterValidation);
                    PopulateMeterReadingCard(METER_READING_TYPE.KWH, validateRegisterDetails);
                }
                else if (validateRegisterDetails.ReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KVAR)
                {
                    meterValidation = new MeterValidation();
                    meterValidation.registerNumber = validateRegisterDetails.RegisterNumber;
                    meterValidation.meterId = validateRegisterDetails.ReadingUnit.ToUpper();
                    meterValidation.mroID = validateRegisterDetails.MroID;
                    validationStateList.Add(meterValidation);
                    PopulateMeterReadingCard(METER_READING_TYPE.KVARH, validateRegisterDetails);
                }
                else if (validateRegisterDetails.ReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KW)
                {
                    meterValidation = new MeterValidation();
                    meterValidation.registerNumber = validateRegisterDetails.RegisterNumber;
                    meterValidation.meterId = validateRegisterDetails.ReadingUnit.ToUpper();
                    meterValidation.mroID = validateRegisterDetails.MroID;
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
            ////Mock Data - Start
            //SMRValidateRegisterDetailList = DummyData();
            //SetUpMeterCards(SMRValidateRegisterDetailList);
            //selectedAccount = new AccountData();
            //selectedAccount.AccountNum = "1010101010";
            ////Mock Data - End
            Bundle intentExtras = Intent.Extras;

            if (intentExtras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(intentExtras.GetString(Constants.SELECTED_ACCOUNT));
            }

            if (intentExtras.ContainsKey(Constants.SMR_RESPONSE_KEY))
            {
                ssmrActivityInfoResponse = JsonConvert.DeserializeObject<SMRActivityInfoResponse>(intentExtras.GetString(Constants.SMR_RESPONSE_KEY));
                if (ssmrActivityInfoResponse.Response != null && ssmrActivityInfoResponse.Response.Data != null)
                {
                    SMRValidateRegisterDetailList = ssmrActivityInfoResponse.Response.Data.SMRMROValidateRegisterDetails;
                    SetUpMeterCards(SMRValidateRegisterDetailList);
                }
            }
            OnGenerateTooltipData();
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

        private void UpdateCurrentValues(LinearLayout container, GetMeterReadingOCRResponseDetails sMRMROValidateRegisterDetails)
        {
            char[] amountInArray = (sMRMROValidateRegisterDetails.OCRValue).ToCharArray();
            MeterReadingInputLayout meterReadingInputLayout = container.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
            meterReadingInputLayout.UpdateMeterReadingInput(sMRMROValidateRegisterDetails.OCRValue);
        }

        public void PopulateMeterReadingCard(METER_READING_TYPE type, SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails)
        {
            LinearLayout linearLayoutContainer;
            TextView meterType;
            MeterReadingInputLayout meterReadingInputLayout;
            
            switch (type)
            {
                case METER_READING_TYPE.KWH:
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    meterReadingInputLayout = linearLayoutContainer.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
                    meterReadingInputLayout.SetMeterId(sMRMROValidateRegisterDetails.ReadingUnit.ToUpper());
                    meterReadingInputLayout.SetOnValidateInput(this);
                    meterReadingInputLayoutList.Add(meterReadingInputLayout);
                    linearLayoutContainer.SetOnTouchListener(new OnMeterCardTouchListener(this));

                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterType = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterType.Text = sMRMROValidateRegisterDetails.ReadingUnitDisplayTitle;
					PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                case METER_READING_TYPE.KVARH:
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    meterReadingInputLayout = linearLayoutContainer.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
                    meterReadingInputLayout.SetMeterId(sMRMROValidateRegisterDetails.ReadingUnit.ToUpper());
                    meterReadingInputLayout.SetOnValidateInput(this);
                    meterReadingInputLayoutList.Add(meterReadingInputLayout);
                    linearLayoutContainer.SetOnTouchListener(new OnMeterCardTouchListener(this));

                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterType = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterType.Text = sMRMROValidateRegisterDetails.ReadingUnitDisplayTitle;
					PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
                default:
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    meterReadingInputLayout = linearLayoutContainer.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
                    meterReadingInputLayout.SetMeterId(sMRMROValidateRegisterDetails.ReadingUnit.ToUpper());
                    meterReadingInputLayout.SetOnValidateInput(this);
                    meterReadingInputLayoutList.Add(meterReadingInputLayout);
                    linearLayoutContainer.SetOnTouchListener(new OnMeterCardTouchListener(this));

                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterType = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterType.Text = sMRMROValidateRegisterDetails.ReadingUnitDisplayTitle;
					PopulatePreviousValues(linearLayoutContainer, sMRMROValidateRegisterDetails);
                    break;
            }
        }

        public class OnMeterCardTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public SubmitMeterReadingContract.IView ownerView;
            public OnMeterCardTouchListener(SubmitMeterReadingContract.IView view)
            {
                ownerView = view;
            }
            public bool OnTouch(View v, MotionEvent e)
            {
                ownerView.OnUpdateSubmitMeterButton();
                return false;
            }
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
                    ValidateMeterInput(linearLayoutContainer, ocrResponse);
                }else if (type == "kvarh" || type == "kvar")
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                    ValidateMeterInput(linearLayoutContainer, ocrResponse);
                }
                else if (type == "kw")
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                    ValidateMeterInput(linearLayoutContainer, ocrResponse);
                }
            }

        }

        public void ValidateMeterInput(LinearLayout meterCardContainer, GetMeterReadingOCRResponseDetails ocrResponse)
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            var currentFocus = this.CurrentFocus;
            if (currentFocus != null)
            {
              inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
            UpdateCurrentReadingValuesColor(meterCardContainer, (ocrResponse.IsSuccess.ToLower() == "true"));
            OnUpdateSubmitMeterButton();
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
                    if (validationData.meterReadingUnit == Constants.SMR_METER_UNIT_KWH)
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                        inlineValidationMessage = linearLayoutContainer.FindViewById<TextView>(Resource.Id.reading_error_validation_msg);
                        UpdateCurrentReadingValuesColor(linearLayoutContainer, validationData.isSuccess);
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
                    else if (validationData.meterReadingUnit == Constants.SMR_METER_UNIT_KVAR)
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                        inlineValidationMessage = linearLayoutContainer.FindViewById<TextView>(Resource.Id.reading_error_validation_msg);
                        UpdateCurrentReadingValuesColor(linearLayoutContainer, validationData.isSuccess);
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
                    else if(validationData.meterReadingUnit == Constants.SMR_METER_UNIT_KW)
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                        inlineValidationMessage = linearLayoutContainer.FindViewById<TextView>(Resource.Id.reading_error_validation_msg);
                        UpdateCurrentReadingValuesColor(linearLayoutContainer, validationData.isSuccess);
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

                    validationStateList.Find(meter =>
                    {
                        return meter.meterId == validationData.meterReadingUnit.ToUpper();
                    }).validated = false;
                }
            }
        }

        public void UpdateCurrentReadingValuesColor(LinearLayout linearLayout, bool isSuccess)
        {
            MeterReadingInputLayout meterReadingInput = linearLayout.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
            meterReadingInput.SetInputColor(isSuccess ? Color.ParseColor("#20bd4c") : Color.ParseColor("#e44b21"));
            TextView meterTypeView = (TextView)linearLayout.FindViewById(Resource.Id.reading_meter_type);
            meterTypeView.SetBackgroundResource(isSuccess ? Resource.Drawable.meter_reading_label_background_ready : Resource.Drawable.meter_reading_label_background_error);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit Meter Reading Screen");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnUpdateSubmitMeterButton()
        {
            int isValidIndex = meterReadingInputLayoutList.FindIndex(meterReadingInput =>
            {
                return meterReadingInput.HasReadingInput();
            });
            EnableSubmitButton((isValidIndex != -1));
        }
    }
}
