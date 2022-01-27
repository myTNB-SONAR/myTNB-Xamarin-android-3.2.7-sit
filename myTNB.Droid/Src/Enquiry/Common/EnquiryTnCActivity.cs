using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Enquiry.Common
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Enquiry")]
    public class EnquiryTnCActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.txtTnCData)]
        TextView txtTnCData;

        [BindView(Resource.Id.txtGSLRebateDisclaimer)]
        TextView txtGSLRebateDisclaimer;

        [BindView(Resource.Id.txtTNBTermOfUse)]
        TextView txtTNBTermOfUse;

        [BindView(Resource.Id.txtPrivacyPolicy)]
        TextView txtPrivacyPolicy;

        private string reqEmail;
        private bool isOwner;
        private string enteredNAme;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.ENTERED_NAME))
                    {
                        enteredNAme = extras.GetString(Constants.ENTERED_NAME);
                    }
                    if (extras.ContainsKey(Constants.REQ_EMAIL))
                    {
                        reqEmail = extras.GetString(Constants.REQ_EMAIL);
                    }
                    if (extras.ContainsKey(Constants.SELECT_REGISTERED_OWNER))
                    {
                        isOwner = bool.Parse(extras.GetString(Constants.SELECT_REGISTERED_OWNER));
                    }
                }

                SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.TNC));

                txtGSLRebateDisclaimer.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.PERSONAL_DISCLAIMER);
                txtTNBTermOfUse.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.TERMS_OF_USE);
                txtPrivacyPolicy.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.PRIVACY_POLICY_TITLE);

                TextViewUtils.SetMuseoSans300Typeface(txtTnCData);
                TextViewUtils.SetMuseoSans500Typeface(txtGSLRebateDisclaimer, txtTNBTermOfUse, txtPrivacyPolicy);
                TextViewUtils.SetTextSize14(txtGSLRebateDisclaimer, txtTnCData, txtTNBTermOfUse, txtPrivacyPolicy);

                string content = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, isOwner ? LanguageConstants.SubmitEnquiry.TNC_AGREE_OWNER :
                    LanguageConstants.SubmitEnquiry.TNC_AGREE_NON_OWNER);

                string acc = UserSessions.GetAccountIsExist(PreferenceManager.GetDefaultSharedPreferences(this));
                GetSearchForAccountResponse.GetSearchForAccountModel AccData = JsonConvert.DeserializeObject<GetSearchForAccountResponse.GetSearchForAccountModel>(acc);

                string temp = string.Format(content, enteredNAme, reqEmail, AccData.ContractAccount);

                txtTnCData.TextFormatted = GetFormattedText(temp);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.EnquiryTnCView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        private ISpanned GetFormattedText(string stringValue)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(stringValue, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                return Html.FromHtml(stringValue);
            }
        }

        [OnClick(Resource.Id.txtTNBTermOfUse)]
        void OnTNC(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.ANTI_SPAM_POLICY));
                webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.TERMS_OF_USE));
                this.StartActivity(webIntent);
            }
        }

        [OnClick(Resource.Id.txtPrivacyPolicy)]
        void OnPrivacyPolicy(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.PRIVACY_POLICY));
                webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.PRIVACY_POLICY_TITLE));
                this.StartActivity(webIntent);
            }
        }
    }
}
