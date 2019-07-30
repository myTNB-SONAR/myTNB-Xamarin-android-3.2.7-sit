
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
    [Activity(Label = "ApplicationFormSMRActivity", Theme = "@style/Theme.Dashboard")]
    public class ApplicationFormSMRActivity : BaseToolbarAppCompatActivity, ApplicationFormSMRContract.IView
    {

        //[BindView(Resource.Id.txtEmail)]
        //EditText txtEmail;

        //[BindView(Resource.Id.txtMobileNumber)]
        //EditText txtMobileNumber;

        //[OnClick(Resource.Id.selectedAccountContainer)]
        //void OnSelectSupplyAccount(object sender, EventArgs eventArgs)
        //{
        //    Intent intent = new Intent(this, typeof(SelectSupplyAccountActivity));
        //    StartActivity(intent);
        //}

        [BindView(Resource.Id.accountSMRLabel)]
        TextView accountSMRLabel;

        [BindView(Resource.Id.accountSMRValue)]
        TextView accountSMRValue;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtMobileNumber)]
        EditText txtMobileNumber;

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
            // Create your application here
            //txtEmail.Text = Intent.GetStringExtra("email");
            //         txtMobileNumber.Text = Intent.GetStringExtra("mobileNumber");

            accountSMRValue.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
                //StartActivity(intent);
                StartActivityForResult(intent,1);
            };

            TextViewUtils.SetMuseoSans300Typeface(accountSMRLabel,accountSMRValue);
            GetCARegisteredContactInfo();
        }

        public void GetCARegisteredContactInfo()
        {
            SMRAccount sMRAccount = UserSessions.GetSMRAccountList().Find(smrAccount =>
            {
                return smrAccount.accountSelected;
            });
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


            mPresenter.SubmitSMRRegistration(sMRAccount,txtMobileNumber.Text,txtEmail.Text,"");
        }

        public void ShowSubmitResult(string resultResponse)
        {
            SMRSubmitResponseData data = JsonConvert.DeserializeObject<SMRSubmitResponseData>(resultResponse);
            if (data.DisplayTitle == "ERROR")
            {
                Intent intent = new Intent(this, typeof(SubmiSMRAccountFailedActivity));
                StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(this, typeof(SubmiSMRAccountCompleteActivity));
                StartActivity(intent);
            }
        }
    }
}
