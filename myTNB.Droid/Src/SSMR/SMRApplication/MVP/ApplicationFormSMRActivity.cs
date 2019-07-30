
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

        //String email, mobileNumber;
        public override int ResourceId()
        {
            return Resource.Layout.ApplicationFormSMRLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ApplicationFormSMRPresenter(this);
            UserSessions.SetSelectAccountList(MyTNBAccountManagement.GetInstance().GetEligibleSMRBillingAccounts());

            // Create your application here
            //txtEmail.Text = Intent.GetStringExtra("email");
            //         txtMobileNumber.Text = Intent.GetStringExtra("mobileNumber");

            accountSMRValue.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
                StartActivity(intent);
            };

            TextViewUtils.SetMuseoSans300Typeface(accountSMRLabel,accountSMRValue);

            SMRAccount sMRAccount = UserSessions.GetSMRAccountList().Find(smrAccount =>
            {
                return smrAccount.accountSelected;
            });

            _ = mPresenter.GetCARegisteredContactInfoAsync(sMRAccount);
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
        }
    }
}
