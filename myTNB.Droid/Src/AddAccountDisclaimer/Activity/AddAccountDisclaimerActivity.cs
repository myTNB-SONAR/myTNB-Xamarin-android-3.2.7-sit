using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;
using System;

namespace myTNB.AndroidApp.Src.AddAccountDisclaimer.Activity
{
    [Activity(Label = "DisclaimerAddAccount", ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class AddAccountDisclaimerActivity : BaseActivityCustom
    {

        [BindView(Resource.Id.headerText)]
        TextView headerText;

        [BindView(Resource.Id.headerText2)]
        TextView headerText2;

        [BindView(Resource.Id.headerText3)]
        TextView headerText3;

        [BindView(Resource.Id.detailsText)]
        TextView detailsText;

        [BindView(Resource.Id.detailsText2)]
        TextView detailsText2;

        //[BindView(Resource.Id.layoutHeader)]
        //LinearLayout layoutHeader;

        [BindView(Resource.Id.ContainerAction)]
        LinearLayout ContainerAction;

        [BindView(Resource.Id.ContainerAction2)]
        LinearLayout ContainerAction2;

        [BindView(Resource.Id.ContainerAction3)]
        LinearLayout ContainerAction3;


        private string PAGE_ID = "AddAccount";
        UserEntity user = UserEntity.GetActive();
        //private ImageView itemAction;
        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.AddAccDisclaimerView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            TextViewUtils.SetMuseoSans500Typeface(headerText, headerText2, headerText3);
            TextViewUtils.SetMuseoSans300Typeface(detailsText, detailsText2);
            TextViewUtils.SetTextSize14(headerText, headerText2, headerText3);
            TextViewUtils.SetTextSize12(detailsText, detailsText2);

            string email = user.Email;
            string data;
            data = Utility.GetLocalizedLabel("AddAccount", "DisclaimerDetails1");
            SetToolBarTitle(GetLabelByLanguage("tncAddAccTitle"));
            //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            headerText.TextFormatted = GetFormattedText(GetLabelByLanguage("DiclaimerHeaderAddElectricity"));
            headerText2.TextFormatted = GetFormattedText(GetLabelByLanguage("DiclaimerHeaderTNBTerm"));
            headerText3.TextFormatted = GetFormattedText(GetLabelByLanguage("DiclaimerHeaderPersonalData"));

            string temp = string.Format(data, email);
            detailsText.TextFormatted = GetFormattedText(temp);
            detailsText2.Text = GetLabelByLanguage("DisclaimerDetails2");
            //itemAction = FindViewById<ImageView>(Resource.Id.itemAction);
            //itemAction.SetBackgroundResource(Resource.Drawable.expand_down_arrow);
        }

        [OnClick(Resource.Id.ContainerAction2)]
        void OnClickHeader1(object sender, EventArgs eventArgs)
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

        [OnClick(Resource.Id.ContainerAction3)]
        void OnClickHeader2(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                string stringA = Utility.GetLocalizedLabel("AddAccount", "DiclaimerHeaderPersonalData").Replace("<b>", "");
                string stringb = stringA.Replace("</b>", "");
                Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel("SubmitEnquiry", "privacyPolicy"));
                webIntent.PutExtra(Constants.IN_APP_TITLE, stringb);
                this.StartActivity(webIntent);
            }
        }

        protected override void OnPause()
        {
            this.SetIsClicked(false);
            base.OnPause();
        }
    }
}