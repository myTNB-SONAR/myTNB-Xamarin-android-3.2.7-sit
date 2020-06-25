using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;

using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.MVP;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.Activity
{

    [Activity(Label = "@string/GeneralEnquiry2of2_app_bar"
   , ScreenOrientation = ScreenOrientation.Portrait
           , WindowSoftInputMode = SoftInput.AdjustPan
   , Theme = "@style/Theme.FaultyStreetLamps")]


    public class FeedbackGeneralEnquiryStepTwoActivity : BaseActivityCustom, FeedbackGeneralEnquiryStepTwoContract.IView
    {

        FeedbackGeneralEnquiryStepTwoContract.IUserActionsListener userActionsListener;
        FeedbackGeneralEnquiryStepTwoPresenter mPresenter;

        [BindView(Resource.Id.WhoShouldWeContact)]
        TextView WhoShouldWeContact;

        [BindView(Resource.Id.txtName)]
        TextView txtName;

        [BindView(Resource.Id.txtEmail)]
        TextView txtEmail;

        [BindView(Resource.Id.txtPhoneNumber)]
        TextView txtPhoneNumber;

        [BindView(Resource.Id.txtTermsConditionsGeneralEnquiry)]
        TextView txtTermsConditionsGeneralEnquiry;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        [BindView(Resource.Id.txtInputLayoutName)]
        TextInputLayout txtInputLayoutName;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutPhoneNumber)]
        TextInputLayout txtInputLayoutPhoneNumber;


        const string PAGE_ID = "Register";


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {   
                //1 set presenter
                mPresenter = new FeedbackGeneralEnquiryStepTwoPresenter(this);

                Intent intent = Intent;
                SetToolBarTitle(Intent.GetStringExtra("TITLE"));


                //2 set font type , 300 normal 500 button
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutName, txtInputLayoutEmail, txtInputLayoutPhoneNumber);
                TextViewUtils.SetMuseoSans300Typeface(txtPhoneNumber, txtEmail, txtName, txtTermsConditionsGeneralEnquiry);
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit , WhoShouldWeContact);

                //set translation of string 
                txtTermsConditionsGeneralEnquiry.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                StripUnderlinesFromLinks(txtTermsConditionsGeneralEnquiry);



            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StripUnderlinesFromLinks(TextView textView)
        {
            var spannable = new SpannableStringBuilder(textView.TextFormatted);
            var spans = spannable.GetSpans(0, spannable.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
            foreach (URLSpan span in spans)
            {
                var start = spannable.GetSpanStart(span);
                var end = spannable.GetSpanEnd(span);
                spannable.RemoveSpan(span);
                var newSpan = new URLSpanNoUnderline(span.URL);
                spannable.SetSpan(newSpan, start, end, 0);
            }
            textView.TextFormatted = spannable;
        }

        class URLSpanNoUnderline : URLSpan
        {
            public URLSpanNoUnderline(string url) : base(url)
            {
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = false;
            }
        }


        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {   //todo change
            return Resource.Layout.FeedbackGeneralEnquiryStepTwoView;
        }

        public void SetPresenter(FeedbackGeneralEnquiryStepTwoContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        [OnClick(Resource.Id.txtTermsConditionsGeneralEnquiry)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.NavigateToTermsAndConditions();
            }
        }

        public void ShowNavigateToTermsAndConditions()
        {
            StartActivity(typeof(TermsAndConditionActivity));
        }












    }
}