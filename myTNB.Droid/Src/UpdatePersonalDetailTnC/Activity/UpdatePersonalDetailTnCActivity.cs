using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.UpdatePersonalDetailTnC.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.UpdatePersonalDetailTnC.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait
        , WindowSoftInputMode = SoftInput.AdjustPan
        , Theme = "@style/Theme.FaultyStreetLamps")]
    public class UpdatePersonalDetailTnCActivity : BaseToolbarAppCompatActivity, UpdatePersonalDetailTnCContract.IView
    {
        [BindView(Resource.Id.TextView_tnc_data)]
        TextView TextView_tnc_data;

        [BindView(Resource.Id.TextView_updatePersonalDataDisclaim)]
        TextView TextView_updatePersonalDataDisclaim;

        [BindView(Resource.Id.TextView_TNB_TermOfUse)]
        TextView TextView_TNB_TermOfUse;

        [BindView(Resource.Id.TextView_privacypolicy)]
        TextView TextView_privacypolicy;

        UpdatePersonalDetailTnCPresenter mPresenter;
        private ISharedPreferences mSharedPref;
        UpdatePersonalDetailTnCContract.IUserActionsListener userActionsListener;

        private string reqEmail;
        private string caNumber;
        private string pageTitle;
        private string caIC;
        private bool isOwner;
        private string enteredNAme;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                //init shared pref
                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);

                //get from prev page data

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    ///PAGE TITLE FROM BEFORE PAGE
                    if (extras.ContainsKey(Constants.ACCOUNT_NUMBER))
                    {
                        caNumber = extras.GetString(Constants.ACCOUNT_NUMBER);
                    }
                    if (extras.ContainsKey(Constants.ENTERED_NAME))
                    {
                        enteredNAme = extras.GetString(Constants.ENTERED_NAME);
                    }

                    if (extras.ContainsKey(Constants.REQ_EMAIL))
                    {
                        reqEmail = extras.GetString(Constants.REQ_EMAIL);
                    }

                    if (extras.ContainsKey(Constants.REQ_IC))
                    {
                        caIC = extras.GetString(Constants.REQ_IC);
                    }

                    if (extras.ContainsKey(Constants.SELECT_REGISTERED_OWNER))
                    {
                        isOwner = bool.Parse(extras.GetString(Constants.SELECT_REGISTERED_OWNER));
                    }
                }

                //set presenter
                this.mPresenter = new UpdatePersonalDetailTnCPresenter(this);

                //pass title
                var test = Utility.GetLocalizedLabel("SubmitEnquiry", "tnc");

                //set tittle
                SetToolBarTitle(test);

                //set translation 
                TextView_updatePersonalDataDisclaim.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "personalDisclamer");
                TextView_TNB_TermOfUse.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "tnbTermUse");
                TextView_privacypolicy.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "privacyPolicyTitle");

                //set font 
                TextViewUtils.SetMuseoSans300Typeface(TextView_tnc_data); //inputLay
                TextViewUtils.SetMuseoSans500Typeface(TextView_updatePersonalDataDisclaim, TextView_TNB_TermOfUse, TextView_privacypolicy); //edit text
                TextViewUtils.SetTextSize14(TextView_updatePersonalDataDisclaim, TextView_tnc_data, TextView_TNB_TermOfUse, TextView_privacypolicy);

                //  CustomerBillingAccount selectedAcc;
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(caNumber);

                string data;
                // if owner tnc is different
                if (isOwner)
                {
                    data = Utility.GetLocalizedLabel("SubmitEnquiry", "tncAgreeOwner");
                }
                else
                {
                    data = Utility.GetLocalizedLabel("SubmitEnquiry", "tncAgreeNonOwner");
                }

                string acc = UserSessions.GetAccountIsExist(PreferenceManager.GetDefaultSharedPreferences(this));
                GetSearchForAccountResponse.GetSearchForAccountModel AccData = JsonConvert.DeserializeObject<GetSearchForAccountResponse.GetSearchForAccountModel>(acc);

                string temp = string.Format(data, enteredNAme, reqEmail, AccData.ContractAccount);

                TextView_tnc_data.TextFormatted = GetFormattedText(temp);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.UpdatePersonalDetailTnCView;
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

        public bool IsActive()
        {
            // needed when include contract
            return Window.DecorView.RootView.IsShown;
        }

        public void SetPresenter(UpdatePersonalDetailTnCContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        [OnClick(Resource.Id.TextView_TNB_TermOfUse)]
        void OnTNC(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel("SubmitEnquiry", "antiSpamPolicy"));
                webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "tnbTermUse"));
                this.StartActivity(webIntent);

            }

        }

        [OnClick(Resource.Id.TextView_privacypolicy)]
        void onPrivacyPolicy(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel("SubmitEnquiry", "privacyPolicy"));
                webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "privacyPolicyTitle"));
                this.StartActivity(webIntent);
            }
        }
    }
}