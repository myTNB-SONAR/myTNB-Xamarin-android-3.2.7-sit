﻿
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
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.SelectSupplyAccount.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "ApplicationFormSMRActivity", Theme = "@style/Theme.Dashboard")]
    public class ApplicationFormSMRActivity : BaseToolbarAppCompatActivity
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

			// Create your application here
			//txtEmail.Text = Intent.GetStringExtra("email");
   //         txtMobileNumber.Text = Intent.GetStringExtra("mobileNumber");
        }
    }
}
