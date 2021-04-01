using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AddAccountDisclaimer.Activity
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

        [BindView(Resource.Id.layoutHeader)]
        LinearLayout layoutHeader;

        [BindView(Resource.Id.layoutDetails)]
        LinearLayout layoutDetails;

        [BindView(Resource.Id.view1)]
        View view1;


        private string PAGE_ID = "AddAccount";
        UserEntity user = UserEntity.GetActive();
        private ImageView itemAction;
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

            detailsText.TextSize = TextViewUtils.GetFontSize(12);
            detailsText2.TextSize = TextViewUtils.GetFontSize(12);
            headerText3.TextSize = TextViewUtils.GetFontSize(14);
            headerText.TextSize = TextViewUtils.GetFontSize(14);
            headerText2.TextSize = TextViewUtils.GetFontSize(14);

            string email = user.Email;
            string data;
            data = Utility.GetLocalizedLabel("AddAccount", "DisclaimerDetails1");
            SetToolBarTitle(GetLabelByLanguage("tncAddAccTitle"));
            //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            headerText.TextFormatted = GetFormattedText(GetLabelByLanguage("DiclaimerHeaderAddElectricity"));
            headerText2.TextFormatted = GetFormattedText(GetLabelByLanguage("DiclaimerHeaderTNBTerm"));
            headerText3.TextFormatted = GetFormattedText(GetLabelByLanguage("DiclaimerHeaderPersonalData"));
           
            string temp = string.Format(data,email);
            detailsText.TextFormatted = GetFormattedText(temp);
            detailsText2.Text = GetLabelByLanguage("DisclaimerDetails2");
            itemAction = FindViewById<ImageView>(Resource.Id.itemAction);
            itemAction.SetBackgroundResource(Resource.Drawable.expand_down_arrow);
        }
       
        [OnClick(Resource.Id.layoutHeader)]
        void OnClickDetails(object sender, EventArgs eventArgs)
        {
            itemAction = FindViewById<ImageView>(Resource.Id.itemAction);
            
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    layoutDetails.Visibility = ViewStates.Gone;
                    view1.Visibility = ViewStates.Gone;
                    //headerText.SetCompoundDrawablesWithIntrinsicBounds(null, null, null,(Android.Graphics.Drawables.Drawable)Resource.Drawable.expand_right_arrow);
                    itemAction.SetBackgroundResource(Resource.Drawable.expand_right_arrow);
                }
                else
                {
                    this.SetIsClicked(false);
                    layoutDetails.Visibility = ViewStates.Visible;
                    view1.Visibility = ViewStates.Visible;
                    //headerText.SetCompoundDrawablesWithIntrinsicBounds(null, null, null,(Android.Graphics.Drawables.Drawable)Resource.Drawable.expand_down_arrow );
                    itemAction.SetBackgroundResource(Resource.Drawable.expand_down_arrow);
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                this.SetIsClicked(false);
            }
        }

        [OnClick(Resource.Id.headerText2)]
        void OnClickHeader1(object sender, EventArgs eventArgs)
        {


            StartActivity(typeof(TermsAndConditionActivity));
        }

        [OnClick(Resource.Id.headerText3)]
        void OnClickHeader2(object sender, EventArgs eventArgs)
        {


            StartActivity(typeof(TermsAndConditionActivity));
        }

        protected override void OnPause()
        {
            this.SetIsClicked(false);
            base.OnPause();
        }
    }
}