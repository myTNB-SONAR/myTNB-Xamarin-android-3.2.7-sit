
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

    [Activity(Label = "@string/meter_reading_title", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.SubmitMeterReadingInput")]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity, SubmitMeterReadingContract.IView
    {
        private METER_READING_TYPE meterType;
        private SubmitMeterReadingPresenter mPresenter;
        private int[] previousMeterViews;
        private int[] currentMeterViews;
        private int MAX_DIGIT = 8;
        private List<MeterValidation> validationStateList;
        private List<MeterReadingModel> meterReadingModelList;


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

        [BindView(Resource.Id.previous_reading_1)]
        TextView prevReading1;

        [BindView(Resource.Id.previous_reading_2)]
        TextView prevReading2;

        [BindView(Resource.Id.previous_reading_3)]
        TextView prevReading3;

        [BindView(Resource.Id.previous_reading_4)]
        TextView prevReading4;

        [BindView(Resource.Id.previous_reading_5)]
        TextView prevReading5;

        [BindView(Resource.Id.previous_reading_6)]
        TextView prevReading6;

        [BindView(Resource.Id.previous_reading_7)]
        TextView prevReading7;

        [BindView(Resource.Id.previous_reading_8)]
        TextView prevReading8;

        public readonly static int SSMR_SUBMIT_METER_ACTIVITY_CODE = 8796;
        public readonly static int SSMR_SUBMIT_METER_OCR_SUBMIT_CODE = 8797;
        private IMenu ssmrMenu;
        private static bool isFirstLaunch = true;

        private List<SSMRMeterReadingModel> singlePhaseList;
        private List<SSMRMeterReadingModel> threePhaseList;

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

        [OnClick(Resource.Id.btnSubmitReading)]
        internal void OnSubmitMeterReading(object sender, EventArgs eventArgs)
        {
            ShowProgressDialog();
            string contractAccount = selectedAccount.AccountNum;
            bool isOwnedAccount = true;
            List<MeterReading> meterReadingRequestList = new List<MeterReading>();
            MeterReading meterReadingRequest;
            meterReadingModelList.ForEach(model =>
            {
                meterReadingRequest = new MeterReading();
                meterReadingRequest.MroID = model.mroID;
                meterReadingRequest.RegisterNumber = model.registerNumber;
                meterReadingRequest.MeterReadingResult = meterReadingInputLayoutList.Find(meterInput => meterInput.GetMeterId() == model.meterReadingUnit).GetMeterReadingInput();
                meterReadingRequest.Channel = "MyTNBAPP";
                meterReadingRequest.MeterReadingDate = "";
                meterReadingRequest.MeterReadingTime = "";
                meterReadingRequestList.Add(meterReadingRequest);
            });
            this.mPresenter.SubmitMeterReading(contractAccount, isOwnedAccount, meterReadingRequestList);
        }

        [OnClick(Resource.Id.btnTakePhoto)]
        internal void OnTakePhotoReading(object sender, EventArgs eventArgs)
        {
            Intent photoIntent = new Intent(this, typeof(SubmitMeterTakePhotoActivity));
            photoIntent.PutExtra("REQUESTED_PHOTOS", JsonConvert.SerializeObject(meterReadingModelList));
            photoIntent.PutExtra("IS_SINGLE_PHASE", meterReadingModelList.Count == 1 ? true : false);
            photoIntent.PutExtra("CONTRACT_NUMBER", selectedAccount.AccountNum);
            StartActivityForResult(photoIntent, SSMR_SUBMIT_METER_OCR_SUBMIT_CODE);
        }

        private void InitializePage()
        {
			meterReadingNote.TextFormatted = GetFormattedText(GetString(Resource.String.ssmr_submit_meter_reading_note));

			TextViewUtils.SetMuseoSans300Typeface(meterReadingTitle,meterReadingNote, prevReading1, prevReading2, prevReading3, prevReading4, prevReading5,
                prevReading6, prevReading7, prevReading8);
            TextViewUtils.SetMuseoSans500Typeface(meterReadingError, btnTakePhoto, btnSubmitReading);

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
            if (resultCode == Result.Canceled)
            {
                HideProgressDialog();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new SubmitMeterReadingPresenter(this);
            InitializePage();
            isFirstLaunch = true;

            validationStateList = new List<MeterValidation>();
            ////Mock Start
            //meterReadingModelList = this.mPresenter.GetDummyData();
            //selectedAccount = new AccountData();
            //selectedAccount.AccountNum = "1010101010";
            ////Mock End

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
                    meterReadingModelList = this.mPresenter.GetMeterReadingModelList(ssmrActivityInfoResponse.Response.Data.SMRMROValidateRegisterDetails);
                }
            }
            meterReadingTitle.TextFormatted = (meterReadingModelList.Count == 1) ? GetFormattedText(GetString(Resource.String.ssmr_submit_meter_reading_message_single))
                : GetFormattedText(GetString(Resource.String.ssmr_submit_meter_reading_message_multiple));
            SetMeterReadingCards();
            OnGenerateTooltipData();
            OnUpdateSubmitMeterButton();
        }

        private void SetMeterReadingCards()
        {
            LinearLayout linearLayoutContainer;
            TextView meterTypeView;
            MeterReadingInputLayout meterReadingInputLayout;
            meterReadingInputLayoutList = new List<MeterReadingInputLayout>();
            meterReadingModelList.ForEach(meterReadingModel =>
            {
                if (meterReadingModel.meterReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KWH)
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterReadingInputLayout = linearLayoutContainer.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
                    meterReadingInputLayout.SetMeterId(meterReadingModel.meterReadingUnit.ToUpper());
                    meterReadingInputLayout.SetOnValidateInput(this);
                    meterReadingInputLayout.InitializeInputBoxes();



                    meterTypeView = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterTypeView.Text = meterReadingModel.meterReadingUnitDisplay;
                    meterReadingInputLayoutList.Add(meterReadingInputLayout);
                    PopulatePreviousMeterReadingValues(linearLayoutContainer,meterReadingModel);
                }
                else if (meterReadingModel.meterReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KW)
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterReadingInputLayout = linearLayoutContainer.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
                    meterReadingInputLayout.SetMeterId(meterReadingModel.meterReadingUnit.ToUpper());
                    meterReadingInputLayout.SetOnValidateInput(this);
                    meterReadingInputLayout.InitializeInputBoxes();

                    meterTypeView = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterTypeView.Text = meterReadingModel.meterReadingUnitDisplay;
                    meterReadingInputLayoutList.Add(meterReadingInputLayout);
                    PopulatePreviousMeterReadingValues(linearLayoutContainer, meterReadingModel);
                }
                else if (meterReadingModel.meterReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KVAR)
                {
                    linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                    linearLayoutContainer.Visibility = ViewStates.Visible;
                    meterReadingInputLayout = linearLayoutContainer.FindViewById<MeterReadingInputLayout>(Resource.Id.meterReadingInputContainer);
                    meterReadingInputLayout.SetMeterId(meterReadingModel.meterReadingUnit.ToUpper());
                    meterReadingInputLayout.SetOnValidateInput(this);
                    meterReadingInputLayout.InitializeInputBoxes();

                    meterTypeView = (TextView)linearLayoutContainer.FindViewById(Resource.Id.reading_meter_type);
                    meterTypeView.Text = meterReadingModel.meterReadingUnitDisplay;
                    meterReadingInputLayoutList.Add(meterReadingInputLayout);
                    PopulatePreviousMeterReadingValues(linearLayoutContainer, meterReadingModel);
                }
            });
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

        private void PopulatePreviousMeterReadingValues(LinearLayout container, MeterReadingModel meterReadingModel)
        {
            char[] amountInArray = meterReadingModel.previousMeterReadingValue.ToCharArray();
            int resourceCounter = 0;
            for (int i = amountInArray.Length; i != 0; i--)
            {
                resourceCounter++;
                if (resourceCounter > MAX_DIGIT)
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
                int foundIndex = meterReadingModelList.FindIndex(meterReadingModel => {
                    return meterReadingModel.meterReadingUnit.ToUpper() == ocrResponse.OCRUnit.ToUpper();});
                if (foundIndex != -1)
                {
                    LinearLayout linearLayoutContainer;
                    string type = ocrResponse.OCRUnit.ToUpper();
                    if (type == Constants.SMR_METER_UNIT_KWH)
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kwhCard) as LinearLayout;
                        UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                        ValidateMeterInput(linearLayoutContainer, ocrResponse);
                    }
                    else if (type == Constants.SMR_METER_UNIT_KVAR)
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kVARhCard) as LinearLayout;
                        UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                        ValidateMeterInput(linearLayoutContainer, ocrResponse);
                    }
                    else if (type == Constants.SMR_METER_UNIT_KW)
                    {
                        linearLayoutContainer = FindViewById(Resource.Id.kwCard) as LinearLayout;
                        UpdateCurrentValues(linearLayoutContainer, ocrResponse);
                        ValidateMeterInput(linearLayoutContainer, ocrResponse);
                    }
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
            StartActivity(intent);
            Finish();
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
            if (meterReadingModelList.Count > 1)
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
            if (!MyTNBAccountManagement.GetInstance().GetSMRMeterReadingThreePhaseOnboardingShown())
            {
                ShowMeterReadingTooltip();
                isFirstLaunch = false;
            }
            MyTNBAccountManagement.GetInstance().UpdateIsSMRMeterReadingThreePhaseOnboardingShown(true);
        }

        public void OnUpdateOnePhaseTooltipData(List<SSMRMeterReadingModel> list)
        {
            if (list != null && list.Count > 0)
            {
                singlePhaseList = list;
            }
            HideProgressDialog();
            if (!MyTNBAccountManagement.GetInstance().GetSMRMeterReadingOnePhaseOnboardingShown())
            {
                ShowMeterReadingTooltip();
                isFirstLaunch = false;
            }
            MyTNBAccountManagement.GetInstance().UpdateIsSMRMeterReadingOnePhaseOnboardingShown(true);
        }

        private void ShowMeterReadingTooltip()
        {
            if (meterReadingModelList.Count > 1)
            {
                List<string> meterReadingUnitList = new List<string>();
                for (int i = 0; i < meterReadingModelList.Count; i++)
                {
                    meterReadingUnitList.Add(meterReadingModelList[i].meterReadingUnitDisplay);
                }
                string meterReadingListToString = String.Join(",", meterReadingUnitList.ToArray());
                threePhaseList[0].Description = String.Format(threePhaseList[0].Description, meterReadingUnitList.Count, meterReadingListToString);
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

                    int existingMeterIndex = meterReadingModelList.FindIndex(model => {
                        return model.meterReadingUnit.ToUpper() == validationData.meterReadingUnit.ToUpper();
                    });

                    if (existingMeterIndex != -1) //Checking for existing validated meter
                    {
                        if (validationData.meterReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KWH)
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
                        else if (validationData.meterReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KVAR)
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
                        else if (validationData.meterReadingUnit.ToUpper() == Constants.SMR_METER_UNIT_KW)
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

                        meterReadingModelList.ForEach(meterReadingModel =>
                        {
                            if (meterReadingModel.meterReadingUnit.ToUpper() == validationData.meterReadingUnit.ToUpper())
                            {
                                meterReadingModel.isValidated = validationData.isSuccess;
                            }
                        });
                    }
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Input Meter Reading");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnUpdateSubmitMeterButton()
        {
            bool hasInputAll = (meterReadingInputLayoutList.FindAll(meterReadingInput =>
            {
                return meterReadingInput.HasReadingInput();
            }).Count == meterReadingInputLayoutList.Count);

            bool allValidated = (meterReadingModelList.FindAll(model =>
            {
                return model.isValidated;
            }).Count == meterReadingModelList.Count);

            meterReadingModelList.ForEach(model =>
            {
                if (meterReadingInputLayoutList.Count > 0)
                {
                    MeterReadingInputLayout inputLayout = meterReadingInputLayoutList.Find(input => { return input.GetMeterId().ToUpper() == model.meterReadingUnit.ToUpper(); });
                    if (inputLayout != null)
                    {
                        model.isValidated = inputLayout.HasReadingInput();
                    }
                }
            });

            EnableSubmitButton(hasInputAll);
        }
    }
}
