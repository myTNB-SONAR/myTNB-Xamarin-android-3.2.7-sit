﻿using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Castle.Core.Internal;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Barcode.Activity;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_Login_BillRelated.Activity;
using myTNB_Android.Src.Feedback_Prelogin_NewIC.Model;
using myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;



namespace myTNB_Android.Src.Feedback_Prelogin_NewIC.Activity
{
    [Activity(Label = "Submit New Enquiry"
  , ScreenOrientation = ScreenOrientation.Portrait
          , WindowSoftInputMode = SoftInput.AdjustPan
  , Theme = "@style/Theme.FaultyStreetLamps")]


    public class FeedbackPreloginNewICActivity : BaseToolbarAppCompatActivity, FeedbackPreloginNewICContract.IView, View.IOnTouchListener
    {


        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.generalEnquiryConstraint)]
        ConstraintLayout generalEnquiryConstraint;

        [BindView(Resource.Id.updatePersonalInfoConstraint)]
        ConstraintLayout updatePersonalInfoConstraint;

        [BindView(Resource.Id.txtAccountNo)]
        EditText txtAccountNo;

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

        [BindView(Resource.Id.infoLabeltxtWhereIsMyAcc)]
        TextView infoLabeltxtWhereIsMyAcc;

        [BindView(Resource.Id.infoLabelWhereIsMyAcc)]
        LinearLayout infoLabelWhereIsMyAcc;


        [BindView(Resource.Id.howCanWeHelpYou)]
        TextView howCanWeHelpYou;

        [BindView(Resource.Id.txtGeneralEnquiry)]
        TextView txtGeneralEnquiry;

        [BindView(Resource.Id.txtGeneralEnquiry_subContent)]
        TextView txtGeneralEnquiry_subContent;

        [BindView(Resource.Id.txtUpdatePersonal)]
        TextView txtUpdatePersonal;

        [BindView(Resource.Id.txtUpdatePersonalContent)]
        TextView txtUpdatePersonalContent;

        [BindView(Resource.Id.scanNewEnquiry)]
        ImageButton scanNewEnquiry;



        String GeneralEnquiry1of2_app_bar = "@string/bill_related_activity_title";

        FrameLayout rootview;

        private bool isAccChoosed = false;


        FeedbackPreloginNewICContract.IUserActionsListener userActionsListener;
        FeedbackPreloginNewICPresenter mPresenter;
        CustomerBillingAccount selectedCustomerBillingAccount;


        private bool isClicked = false;


        private ISharedPreferences mSharedPref;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {

                //init shared preferences 
                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                //1 set presenter
                mPresenter = new FeedbackPreloginNewICPresenter(this, mSharedPref);

              

                // Intent intent = Intent;
                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"));
                //2 set font type , 300 normal 500 button
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNo);
                TextViewUtils.SetMuseoSans300Typeface(txtUpdatePersonalContent, txtGeneralEnquiry_subContent, txtAccountNo);
                TextViewUtils.SetMuseoSans500Typeface(infoLabeltxtWhereIsMyAcc, howCanWeHelpYou, txtGeneralEnquiry, txtUpdatePersonal);

                //set translation of string 
                //txtTermsConditionsGeneralEnquiry.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                //StripUnderlinesFromLinks(txtTermsConditionsGeneralEnquiry);

                txtInputLayoutAccountNo.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberHint").ToUpper();
                infoLabeltxtWhereIsMyAcc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo");
                howCanWeHelpYou.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "infoHowcan");
                txtGeneralEnquiry.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryTitle");
                txtGeneralEnquiry_subContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryDescription");
                txtUpdatePersonal.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle");
                txtUpdatePersonalContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "personalDetailsDescription");

                // txtInputLayoutAccountNo.Hint= GetLabelCommonByLanguage("email"); //sample of injecting hint using common lang

                txtAccountNo.SetOnTouchListener(this);  //set listener on dropdown arrow at TextLayout
                txtAccountNo.TextChanged += TextChange;  //adding listener on text change
                txtAccountNo.AddTextChangedListener(new InputFilterFormField(txtAccountNo, txtInputLayoutAccountNo));  //adding listener on text change

                infoLabeltxtWhereIsMyAcc.Text = Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle");  // inject translation to text

                onGetTooltipImageContent();

                var sharedpref_data = UserSessions.GetFeedbackUpdateDetailDisabled(mSharedPref);

                bool isUpdatePersonalDetail = bool.Parse(sharedpref_data);  //get from shared pref


                if (isUpdatePersonalDetail == true)
                {
                    updatePersonalInfoConstraint.Visibility = ViewStates.Gone;
                }

               


            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public override int ResourceId()
        {   //todo change
            return Resource.Layout.FeedbackPreloginNewICView;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void  makeSetClick(bool setClick)
        {
            this.SetIsClicked(setClick);
        }

        public void SetPresenter(FeedbackPreloginNewICContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            const int DRAWABLE_LEFT = 0;
            const int DRAWABLE_TOP = 1;
            const int DRAWABLE_RIGHT = 2;
            const int DRAWABLE_BOTTOM = 3;
            if (v is EditText)
            {
                EditText eTxtView = v as EditText;
                if (eTxtView.Id == Resource.Id.txtAccountNo)
                {

                    if (e.Action == MotionEventActions.Up)
                    {
                        if (e.RawX >= (txtAccountNo.Right - txtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                        {
                            //this function listen to click on the dropdown drawable right
                            this.userActionsListener.OnSelectAccount();
                            return true;
                        }
                    }
                }
                else if (v.Id == Resource.Id.txtFeedback)
                {
                    v.Parent.RequestDisallowInterceptTouchEvent(true);
                    switch (e.Action & MotionEventActions.Mask)
                    {
                        case MotionEventActions.Up:
                            v.Parent.RequestDisallowInterceptTouchEvent(false);
                            break;
                    }
                }

            }
            return false;
        }



        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string accno = txtAccountNo.Text.ToString().Trim();
                this.userActionsListener.CheckRequiredFields(accno);

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }


        }


        public void toggleEnableClick()
        {
            isAccChoosed = true;
        }

        public void toggleDisableClick()
        {
            isAccChoosed = false;
        }



        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }





        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit New Enquiry");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
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

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        [OnClick(Resource.Id.scanNewEnquiry)]
        void OnScanClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);
                Intent barcodeIntent = new Intent(this, typeof(BarcodeActivity));
                barcodeIntent.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"));
                StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
            }
        }

        //[OnClick(Resource.Id.txtAccountNo)]
        //void OnSelectAccountLayout1(object sender, EventArgs eventArgs)
        //{
        //    if (!this.GetIsClicked())+
        //    {
        //        this.SetIsClicked(true);
        //        this.userActionsListener.OnSelectAccount();
        //    }
        //}


        public void ShowInvalidAccountNumberError()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            txtInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLength");
        }

        public void ShowEnterOrSelectAccNumber()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            // txtInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLength");  //todo  add translation for bm
            txtInputLayoutAccountNo.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "plsEnterAcc");

        }


        public void RemoveNumberErrorMessage()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            txtInputLayoutAccountNo.Error = "";
        }

        public void ShowGeneralEnquiry()
        {

            Intent generalEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepOneActivity));
            generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            StartActivity(generalEnquiry);

        }

        public void ShowSelectAccount(AccountData accountData)
        {
            Intent supplyAccount = new Intent(this, typeof(FeedbackSelectAccountActivity));
            supplyAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
        }

        //handle push page with waiting return data
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {

            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.BARCODE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {

                        string barcodeResultText = data.GetStringExtra(Constants.BARCODE_RESULT);
                        txtAccountNo.Text = barcodeResultText;

                    }
                }
                else if (requestCode == Constants.SELECT_ACCOUNT_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extras = data.Extras;

                        AccountData selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                        selectedCustomerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);
                        //injecting string into the accno
                        txtAccountNo.Text = selectedCustomerBillingAccount.AccNum;
                    }
                }


            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.generalEnquiryConstraint)]
        void OnGeneralEnquiryConstraint(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);


                if (DownTimeEntity.IsBCRMDown())
                {
                    OnBCRMDownTimeErrorMessage();
                    this.SetIsClicked(false);
                }
                else
                {
                    //please paste here
                    if (isAccChoosed)
                    {
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim() , false);
                        //this.userActionsListener.OnGeneralEnquiry();
                    }
                    else
                    {   //checking 
                        string accno = txtAccountNo.Text.ToString().Trim();
                        this.userActionsListener.CheckRequiredFields(accno);
                        this.SetIsClicked(false);
                    }


                }
            }
        }

        [OnClick(Resource.Id.updatePersonalInfoConstraint)]
        void OnupdatePersonalInfoConstraint(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                if (DownTimeEntity.IsBCRMDown())
                {
                    OnBCRMDownTimeErrorMessage();
                    this.SetIsClicked(false);
                }

                else if (isAccChoosed)
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim() , true);
                    //this.userActionsListener.onUpdatePersonalDetail();
                }
                else
                {
                    string accno = txtAccountNo.Text.ToString().Trim();
                    this.userActionsListener.CheckRequiredFields(accno);  // if person is not enter any acc or choose
                    this.SetIsClicked(false);
                }


            }
        }

        Snackbar mErrorMessageSnackBar;
        public void OnBCRMDownTimeErrorMessage(string message = null)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
                DownTimeEntity BCRMDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (!string.IsNullOrEmpty(BCRMDownTime.DowntimeTextMessage))
                {
                    message = BCRMDownTime.DowntimeTextMessage;
                }
                else
                {
                    message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
                }
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }


        public void showUpdatePersonalDetail()
        {

            var updatePersoanlInfo = new Intent(this, typeof(UpdatePersonalDetailStepOneActivity));

            updatePersoanlInfo.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            updatePersoanlInfo.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle"));
            updatePersoanlInfo.PutExtra(Constants.PAGE_STEP_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of3"));

            StartActivity(updatePersoanlInfo);

        }




        [OnClick(Resource.Id.infoLabelWhereIsMyAcc)]
        void OninfoLabelWhereIsMyAcc(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);
                this.userActionsListener.onShowWhereIsMyAcc();





            }
        }


        public async void ShowWhereIsMyAcc()

        {
            //List<WhereMyAccToolTipResponse> modelList = MyTNBAppToolTipData.GetWhereMyAccTipData();

            //if (modelList != null && modelList.Count > 0)
            //{
            //    if (!this.GetIsClicked())
            //    {
            //        this.SetIsClicked(true);
            //        MyTNBAppToolTipBuilder Tooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
            //           .SetHeaderImageBitmap(modelList[0].ImageBitmap)
            //           .SetTitle(modelList[0].PopUpTitle)
            //           .SetMessage(modelList[0].PopUpBody)
            //           .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //           .SetCTAaction(() => { this.SetIsClicked(false); })
            //           .Build();
            //           Tooltip.Show();
            //    }
            //}
            //else
            //{  //incase sitecoreCMSEntity return null 

            //    //please delete 

            //    var url =Utility.GetLocalizedLabel("AddAccount", "imageTest");
            //    Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(url);
            //    // .SetHeaderImage(Resource.Drawable.img_register_acct_no)
            //    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
            //     .SetHeaderImageBitmap(imageCache)
            //    .SetHeaderImage(Resource.Drawable.img_register_acct_no)
            //    .SetTitle(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle"))
            //    .SetMessage(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountDetails"))
            //    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //    .SetCTAaction(() => { this.SetIsClicked(false); })
            //    .Build();
            //     whereisMyacc.Show();
            //}

            //   var url = Utility.GetLocalizedLabel("SubmitEnquiry", "imageWhereAcc");



            ///   Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(SiteCoreConfig.SITECORE_URL + url);
            ///   

        
                

                string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

                if (!base64Image.IsNullOrEmpty())
                {
                    var imageCache = Base64ToBitmap(base64Image);

                    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                     .SetHeaderImageBitmap(imageCache)
                    .SetTitle(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountDetails"))
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .SetCTAaction(() => { this.SetIsClicked(false); })
                    .Build();
                    whereisMyacc.Show();


                }
                else
                {   //if sql lite data is somehow corrupted
                    Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageWhereAcc"));
                    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                    .SetHeaderImageBitmap(imageCache)
                    .SetTitle(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountDetails"))
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .SetCTAaction(() => { this.SetIsClicked(false); })
                    .Build();
                    whereisMyacc.Show();

                }


            

  

 

        }



        private Task onGetTooltipImageContent()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    //check if image is exist in sql lite
                    var imageWhereAccUrl = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageWhereAcc");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageWhereAccUrl, TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

                        var image = ImageUtils.GetImageBitmapFromUrl(imageWhereAccUrl);
                        var base64 = BitmapToBase64(image);

                        TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                        newImage_WHERE_MY_ACC.ImageBase64 = base64;
                        newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC.ToString();
                        newImage_WHERE_MY_ACC.Url = imageWhereAccUrl;

                        TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);
                        if (base64Image.IsNullOrEmpty())
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

                            var image = ImageUtils.GetImageBitmapFromUrl(imageWhereAccUrl);
                            var base64 = BitmapToBase64(image);

                            TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                            newImage_WHERE_MY_ACC.ImageBase64 = base64;
                            newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC.ToString();
                            newImage_WHERE_MY_ACC.Url = imageWhereAccUrl;

                            TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                        }

                    }

                    //check if image is exist in sql lite
                    var imageIC = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageCopyIC");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageIC, TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

                        var image = ImageUtils.GetImageBitmapFromUrl(imageIC);
                        var base64 = BitmapToBase64(image);

                        TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                        newImage_WHERE_MY_ACC.ImageBase64 = base64;
                        newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE.ToString();
                        newImage_WHERE_MY_ACC.Url = imageIC;

                        TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);
                        if (base64Image.IsNullOrEmpty())
                        {

                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

                            var image = ImageUtils.GetImageBitmapFromUrl(imageIC);
                            var base64 = BitmapToBase64(image);

                            TooltipImageDirectEntity newImage_IC_SAMPLE = new TooltipImageDirectEntity();
                            newImage_IC_SAMPLE.ImageBase64 = base64;
                            newImage_IC_SAMPLE.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE.ToString();
                            newImage_IC_SAMPLE.Url = imageIC;

                            TooltipImageDirectEntity.InsertItem(newImage_IC_SAMPLE);

                        }
                    }
                    //check if image is exist in sql lite
                    var imageConsent = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageConsent");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageConsent, TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);

                        var image_consent = ImageUtils.GetImageBitmapFromUrl(imageConsent);
                        var base64 = BitmapToBase64(image_consent);

                        TooltipImageDirectEntity newImage_PROOF_OF_CONSENT = new TooltipImageDirectEntity();
                        newImage_PROOF_OF_CONSENT.ImageBase64 = base64;
                        newImage_PROOF_OF_CONSENT.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT.ToString();
                        newImage_PROOF_OF_CONSENT.Url = imageConsent;

                        TooltipImageDirectEntity.InsertItem(newImage_PROOF_OF_CONSENT);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);
                        if (base64Image.IsNullOrEmpty())
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);

                            var image_consent = ImageUtils.GetImageBitmapFromUrl(imageConsent);
                            var base64 = BitmapToBase64(image_consent);

                            TooltipImageDirectEntity newImage_PROOF_OF_CONSENT = new TooltipImageDirectEntity();
                            newImage_PROOF_OF_CONSENT.ImageBase64 = base64;
                            newImage_PROOF_OF_CONSENT.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT.ToString();
                            newImage_PROOF_OF_CONSENT.Url = imageConsent;

                            TooltipImageDirectEntity.InsertItem(newImage_PROOF_OF_CONSENT);
                        }

                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Android.Util.Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        public static Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Android.Util.Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        Snackbar newErrorMessageSnackBar;
        public void OnSubmitError(string message = null)
        {
            if (newErrorMessageSnackBar != null && newErrorMessageSnackBar.IsShown)
            {
                newErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
                message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            newErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { newErrorMessageSnackBar.Dismiss(); }
            );
            View v = newErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            newErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }


    }
    
}