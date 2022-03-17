using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.Activity;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.Fragment;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.DigitalSignature.NotificationDetails.Activity
{
    [Activity(Label = "DS Notification Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class DSNotificationDetailsActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.dsNotifDetailTitle)]
        TextView dsNotifDetailTitle;

        [BindView(Resource.Id.dsNotifDetailBtnVerifyNow)]
        Button dsNotifDetailBtnVerifyNow;

        [BindView(Resource.Id.identityVerificationListContainer)]
        LinearLayout identityVerificationListContainer;

        private const string PAGE_ID = "DSNotificationDetails";

        AlertDialog removeDialog;

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

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.NotificationDetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_delete_notification:
                    removeDialog = new AlertDialog.Builder(this)
                        .SetTitle(Utility.GetLocalizedLabel("PushNotificationList", "deleteTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("PushNotificationList", "deleteMessage"))
                        .SetNegativeButton(Utility.GetLocalizedCommonLabel("cancel"),
                        delegate
                        {
                            removeDialog.Dismiss();
                        })
                        .SetPositiveButton(Utility.GetLocalizedCommonLabel("ok"),
                        delegate
                        {
                            //mPresenter.DeleteNotificationDetail(notificationDetails);
                        })
                        .Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetUpViews();
            RenderContent();
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);

            TextViewUtils.SetMuseoSans500Typeface(dsNotifDetailTitle, dsNotifDetailBtnVerifyNow);
            TextViewUtils.SetTextSize16(dsNotifDetailTitle, dsNotifDetailBtnVerifyNow);

            dsNotifDetailTitle.Text = "Verify your identity now to enjoy more of myTNB!";
            dsNotifDetailBtnVerifyNow.Text = "Verify Now";
        }

        private void RenderContent()
        {
            try
            {
                Dictionary<string, List<SelectorModel>> notificationDetailsDictionary = LanguageManager.Instance.GetSelectorsByPage<SelectorModel>("PushNotificationDetails");
                if (notificationDetailsDictionary != null
                && notificationDetailsDictionary.Count > 0
                && notificationDetailsDictionary.ContainsKey("dsDescriptionList"))
                {
                    List<SelectorModel> notificationContentList = notificationDetailsDictionary["dsDescriptionList"];
                    if (notificationContentList.Count > 0)
                    {
                        for (int j = 0; j < notificationContentList.Count; j++)
                        {
                            string title = notificationContentList[j].Value;
                            string desc = notificationContentList[j].Description;
                            DSIdentityVerificationListItemComponent itemListComponent = new DSIdentityVerificationListItemComponent(this);
                            var resIcon = j switch
                            {
                                0 => Resource.Drawable.Icon_Notification_Details_1,
                                1 => Resource.Drawable.Icon_Notification_Details_2,
                                2 => Resource.Drawable.Icon_Notification_Details_3,
                                _ => Resource.Drawable.Icon_Notification_Details_1,
                            };
                            itemListComponent.SetItemTitleText(title);
                            itemListComponent.SetItemDescText(desc);
                            itemListComponent.SetItemIcon(resIcon);
                            identityVerificationListContainer.AddView(itemListComponent);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                identityVerificationListContainer.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.dsNotifDetailBtnVerifyNow)]
        void VerifyOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                NavigateToIdentityVerification();
            }
        }

        private void NavigateToIdentityVerification()
        {
            Intent nbrDiscoverMoreIntent = new Intent(this, typeof(DSIdentityVerificationActivity));
            StartActivity(nbrDiscoverMoreIntent);
        }
    }
}