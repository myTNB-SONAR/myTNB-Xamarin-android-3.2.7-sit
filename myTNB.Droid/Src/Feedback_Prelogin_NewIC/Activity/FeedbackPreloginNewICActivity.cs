using AFollestad.MaterialDialogs;
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
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Barcode.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_Login_BillRelated.Activity;
using myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
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

        [BindView(Resource.Id.generalEnquiryConstraint)]
        ConstraintLayout generalEnquiryConstraint;

        [BindView(Resource.Id.txtAccountNo)]
        EditText txtAccountNo;

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

        [BindView(Resource.Id.infoLabeltxtWhereIsMyAcc)]
        TextView infoLabeltxtWhereIsMyAcc;

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


        FeedbackPreloginNewICContract.IUserActionsListener userActionsListener;
        FeedbackPreloginNewICPresenter mPresenter;
        CustomerBillingAccount selectedCustomerBillingAccount;


        private bool isClicked = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                //1 set presenter
                mPresenter = new FeedbackPreloginNewICPresenter(this);

               // Intent intent = Intent;
                SetToolBarTitle("Submit New Enquiry");
                //2 set font type , 300 normal 500 button
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNo);
                TextViewUtils.SetMuseoSans300Typeface(txtUpdatePersonalContent, txtGeneralEnquiry_subContent, txtAccountNo);
                TextViewUtils.SetMuseoSans500Typeface(infoLabeltxtWhereIsMyAcc, howCanWeHelpYou, txtGeneralEnquiry, txtUpdatePersonal, txtUpdatePersonalContent);

                //set translation of string 
                //txtTermsConditionsGeneralEnquiry.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                //StripUnderlinesFromLinks(txtTermsConditionsGeneralEnquiry);

                // txtInputLayoutAccountNo.Hint= GetLabelCommonByLanguage("email"); //sample of injecting hint using common lang

                txtAccountNo.SetOnTouchListener(this);  //set listener on dropdown arrow at TextLayout

                txtAccountNo.TextChanged += TextChange;  //adding listener on text change
                txtAccountNo.AddTextChangedListener(new InputFilterFormField(txtAccountNo, txtInputLayoutAccountNo));  //adding listener on text change

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

        [OnClick(Resource.Id.generalEnquiryConstraint)]
        void OnGeneralEnquiryConstraint(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                
          
                this.SetIsClicked(true);
                if (DownTimeEntity.IsBCRMDown())
                {
                    //OnBCRMDownTimeErrorMessage();
                    this.SetIsClicked(false);
                }
                else
                {

                    /// need to fix , error cant use passing the mview presenter dosent know why it dosent passing

                    this.userActionsListener.OnGeneralEnquiry();
                }
            }
        }

        [OnClick(Resource.Id.scanNewEnquiry)]
        void OnScanClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);
                Intent barcodeIntent = new Intent(this, typeof(BarcodeActivity));
                StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
            }
        }

        //[OnClick(Resource.Id.txtAccountNo)]
        //void OnSelectAccountLayout1(object sender, EventArgs eventArgs)
        //{
        //    if (!this.GetIsClicked())
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


        public void RemoveNumberErrorMessage()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            txtInputLayoutAccountNo.Error = "";
        }

        public void ShowGeneralEnquiry()
        {

            var feedbackGeneralEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepOneActivity));
            feedbackGeneralEnquiry.PutExtra("TITLE", GeneralEnquiry1of2_app_bar);
            StartActivity(feedbackGeneralEnquiry);

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






    }
}