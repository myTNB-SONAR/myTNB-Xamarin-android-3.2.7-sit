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
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.NotificationAddAccount.Activity
{
    [Activity(Label = "NotificationAddAccount", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class NotificationAddAccountActivity : BaseActivityCustom
    {

        [BindView(Resource.Id.notificationDetailTitle)]
        TextView notificationDetailTitle;

        [BindView(Resource.Id.notificationDetailMessage)]
        TextView notificationDetailMessage;

        [BindView(Resource.Id.notificationDetailMessage2)]
        TextView notificationDetailMessage2;

        [BindView(Resource.Id.btnManageAccess)]
        Button btnManageAccess;


        private string PAGE_ID = "NotificationAddAccount";

        AccountData accountData;
        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.NotificationAddAccLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            TextViewUtils.SetMuseoSans300Typeface(notificationDetailMessage, notificationDetailMessage2);
            TextViewUtils.SetMuseoSans500Typeface(notificationDetailTitle, btnManageAccess);

            //string nickname = accountData.AccountNickName;
            SetToolBarTitle(GetLabelByLanguage("title"));
            notificationDetailTitle.Text = GetLabelByLanguage("notificationTitle");
            //notificationDetailMessage.Text = ((string.Format(GetLabelByLanguage("notificationDetailMessage"), nickname)));
            notificationDetailMessage2.Text = GetLabelByLanguage("notificationDetailMessage");
            notificationDetailMessage2.Text = GetLabelByLanguage("notificationDetail");
            btnManageAccess.Text = GetLabelByLanguage("btnManageAccess");
        }
    }
}