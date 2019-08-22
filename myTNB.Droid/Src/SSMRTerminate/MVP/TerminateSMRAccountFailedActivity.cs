
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "TerminateSMRAccountFailedActivity", Theme = "@style/Theme.BillRelated")]
    public class TerminateSMRAccountFailedActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.txtTitleInfoError)]
        TextView txtTitleInfoError;

        [BindView(Resource.Id.txtMessageInfoError)]
        TextView txtMessageInfoError;


        [BindView(Resource.Id.btnBackToHomeFailed)]
        Button btnBackToHomeFailed;

        [BindView(Resource.Id.btnTryAgainFailed)]
        Button btnTryAgainFailed;

        private AccountData selectedAccount;


        public override int ResourceId()
        {
            return Resource.Layout.TerminateSMRAccountFailedView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfoError, btnBackToHomeFailed, btnTryAgainFailed);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfoError);

            btnBackToHomeFailed.Text = "Back to My Usage";
            btnTryAgainFailed.Text = "Try Again";

            Bundle extras = Intent.Extras;

            if (extras != null && extras.ContainsKey("SUBMIT_RESULT"))
            {
                SMRregistrationSubmitResponse response = JsonConvert.DeserializeObject<SMRregistrationSubmitResponse>(extras.GetString("SUBMIT_RESULT"));
                if (response != null && response.Data != null && response.Data.DisplayTitle != null)
                {
                    txtTitleInfoError.Text = response.Data.DisplayTitle;
                }
                else
                {
                    txtTitleInfoError.Text = "Please Try Again";
                }
                if (response != null && response.Data != null && response.Data.DisplayMessage != null)
                {
                    txtMessageInfoError.Text = response.Data.DisplayMessage;
                }
                else
                {
                    txtMessageInfoError.Text = "It looks like we can't process your application at the moment.";
                }
            }
            else
            {
                txtTitleInfoError.Text = "Please Try Again";
                txtMessageInfoError.Text = "It looks like we can't process your application at the moment.";
            }

            if (extras != null && extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }
        }

        [OnClick(Resource.Id.btnBackToHomeFailed)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            if (selectedAccount != null)
            {
                CustomerBillingAccount.RemoveSelected();
                CustomerBillingAccount.SetSelected(selectedAccount.AccountNum);
            }
            SetResult(Result.Ok);
            Finish();
        }

        [OnClick(Resource.Id.btnTryAgainFailed)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            OnBackPressed();
        }
    }
}
