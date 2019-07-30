
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SelectSupplyAccount.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.CARegisteredContactInfoResponse;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.SMRregistrationSubmitResponse;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "Apply Self Meter Reading", Theme = "@style/Theme.Dashboard")]
    public class ApplicationFormSMRActivity : BaseToolbarAppCompatActivity, ApplicationFormSMRContract.IView
    {
        [BindView(Resource.Id.applySMRForLabel)]
        TextView applySMRForLabel;

        [BindView(Resource.Id.accountSMRLabel)]
        TextView accountSMRLabel;

        [BindView(Resource.Id.accountSMRValue)]
        TextView accountSMRValue;

        [BindView(Resource.Id.applySMRAddress)]
        TextView applySMRAddress;

        [BindView(Resource.Id.applySMRContactLabel)]
        TextView applySMRContactLabel;

        [BindView(Resource.Id.txtTermsAndCondition)]
        TextView txtTermsAndCondition;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtMobileNumber)]
        EditText txtMobileNumber;

        [BindView(Resource.Id.selectAccountContainer)]
        RelativeLayout selectAccountContainer;

        ApplicationFormSMRPresenter mPresenter;
        LoadingOverlay loadingOverlay;

        //String email, mobileNumber;
        public override int ResourceId()
        {
            return Resource.Layout.ApplicationFormSMRLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                if (resultCode == Result.Canceled)
                {
                    GetCARegisteredContactInfo();
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ApplicationFormSMRPresenter(this);
            selectAccountContainer.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
                StartActivityForResult(intent,1);
            };

            TextViewUtils.SetMuseoSans300Typeface(accountSMRLabel,accountSMRValue, applySMRAddress, txtTermsAndCondition);
            TextViewUtils.SetMuseoSans500Typeface(applySMRForLabel, applySMRContactLabel);
            GetCARegisteredContactInfo();
        }

        public void GetCARegisteredContactInfo()
        {
            SMRAccount sMRAccount = UserSessions.GetSMRAccountList().Find(smrAccount =>
            {
                return smrAccount.accountSelected;
            });
            accountSMRValue.Text = sMRAccount.accountName;
            applySMRAddress.Text = sMRAccount.accountAddress;
            if (sMRAccount.email != txtEmail.Text || sMRAccount.mobileNumber != txtMobileNumber.Text)
            {
                ShowProgressDialog();
                mPresenter.GetCARegisteredContactInfoAsync(sMRAccount);
            }
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        public void ShowSelectAccount()
        {
            //throw new NotImplementedException();
        }

        public void UpdateSMRInfo(SMRAccount account)
        {
            accountSMRValue.Text = account.accountName;
            txtEmail.Text = account.email;
            txtMobileNumber.Text = account.mobileNumber;

            List<SMRAccount> updatedSMRAccountList = new List<SMRAccount>();
            foreach (SMRAccount smrAccount in UserSessions.GetSMRAccountList())
            {
                if (smrAccount.accountNumber == account.accountNumber)
                {
                    smrAccount.email = account.email;
                    smrAccount.mobileNumber = account.mobileNumber;
                    smrAccount.accountSelected = account.accountSelected;
                    updatedSMRAccountList.Add(smrAccount);
                }
                else
                {

                    updatedSMRAccountList.Add(smrAccount);
                }
            }
            UserSessions.SetSMRAccountList(updatedSMRAccountList);
            HideProgressDialog();
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

        [OnClick(Resource.Id.btnSubmitRegistration)]
        void SubmitRegistration(object sender, EventArgs eventArgs)
        {
            SMRAccount sMRAccount = UserSessions.GetSMRAccountList().Find(smrAccount =>
            {
                return smrAccount.accountSelected;
            });

            ShowProgressDialog();
            mPresenter.SubmitSMRRegistration(sMRAccount,txtMobileNumber.Text,txtEmail.Text,"");
        }

        public void ShowSubmitSuccessResult(string jsonResponse)
        {
            Intent intent = new Intent(this, typeof(SubmiSMRAccountCompleteActivity));
            intent.PutExtra("SUBMIT_RESULT", jsonResponse);
            StartActivity(intent);
            HideProgressDialog();
        }

        public void ShowSubmitFailedResult(string jsonResponse)
        {
            Intent intent = new Intent(this, typeof(SubmiSMRAccountFailedActivity));
            intent.PutExtra("SUBMIT_RESULT", jsonResponse);
            StartActivity(intent);
            HideProgressDialog();
        }
    }
}
