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
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MyTNBService.Response;
using myTNB.Android.Src.SMRnewTncView.Activity;
using myTNB.Android.Src.SMRnewTncView.MVP;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SMRnewTncView.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait
        , WindowSoftInputMode = SoftInput.AdjustPan
        , Theme = "@style/Theme.FaultyStreetLamps")]
    public class SMRnewTncViewActivity : BaseToolbarAppCompatActivity, SMRnewTncViewContract.IView
    {
        [BindView(Resource.Id.TextView_SMRTermsnCondition)]
        TextView TextView_SMRTermsnCondition;

        [BindView(Resource.Id.TextView_SMRtnc_data)]
        TextView TextView_SMRtnc_data;

        [BindView(Resource.Id.TextView_TNB_SMRTermOfUse)]
        TextView TextView_TNB_SMRTermOfUse;
        
        [BindView(Resource.Id.TextView_SMRprivacypolicy)]
        TextView TextView_SMRprivacypolicy;

        SMRnewTncViewPresenter mPresenter;
        SMRnewTncViewContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //set presenter
            this.mPresenter = new SMRnewTncViewPresenter(this);

            //pass title
            var test = Utility.GetLocalizedLabel("SSMRApplicationTnc", "title");

            //set tittle
            SetToolBarTitle(test);

            TextView_SMRTermsnCondition.Text = Utility.GetLocalizedLabel("SSMRApplicationTnc", "termsnCondition");
            TextView_TNB_SMRTermOfUse.Text = Utility.GetLocalizedLabel("SSMRApplicationTnc", "termsofUse");
            TextView_SMRprivacypolicy.Text = Utility.GetLocalizedLabel("SSMRApplicationTnc", "pdpa");

            //set font 
            TextViewUtils.SetMuseoSans300Typeface(TextView_SMRtnc_data); //inputLay
            TextViewUtils.SetMuseoSans500Typeface(TextView_SMRTermsnCondition, TextView_TNB_SMRTermOfUse, TextView_SMRprivacypolicy); //edit text
            TextViewUtils.SetTextSize14(TextView_SMRTermsnCondition, TextView_SMRtnc_data, TextView_TNB_SMRTermOfUse, TextView_SMRprivacypolicy);


            string data;
            data = Utility.GetLocalizedLabel("SSMRApplicationTnc", "tncData");
            TextView_SMRtnc_data.TextFormatted = GetFormattedText(data);

        }

        public override int ResourceId()
        {
            return Resource.Layout.SMRnewTncViewLayout;
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

        public void SetPresenter(SMRnewTncViewContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        [OnClick(Resource.Id.TextView_TNB_SMRTermOfUse)]
        void OnTNC(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel("SSMRApplicationTnc", "termsofUsePolicy"));
                webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel("SSMRApplicationTnc", "termsofUse"));
                this.StartActivity(webIntent);

            }

        }

        [OnClick(Resource.Id.TextView_SMRprivacypolicy)]
        void onPrivacyPolicy(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel("SSMRApplicationTnc", "pdpaPolicy"));
                webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel("SSMRApplicationTnc", "pdpa"));
                this.StartActivity(webIntent);
            }
        }
    }
}