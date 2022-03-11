using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
//using myTNB_Android.Src.DigitalSignature.NotificationDetails.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.DigitalSignature.NotificationDetails.Activity
{
    [Activity(Label = "DS Notification Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class DSNotificationDetailsActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.notificationDetailTitle)]
        TextView notificationDetailTitle;

        [BindView(Resource.Id.btnVerify)]
        Button btnVerify;

        private const string PAGE_ID = "DSNotificationDetails";

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DSNotificationDetailsView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans500Typeface(notificationDetailTitle, btnVerify);

            SetToolBarTitle(GetLabelByLanguage("title"));
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
            notificationDetailTitle.Text = GetLabelByLanguage("notificationTitle");
            btnVerify.Text = GetLabelByLanguage("btnVerify");
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
        }
    }
}