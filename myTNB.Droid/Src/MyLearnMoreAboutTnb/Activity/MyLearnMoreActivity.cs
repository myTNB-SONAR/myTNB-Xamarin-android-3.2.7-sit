using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.CompoundView;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.FAQ.Activity;
using myTNB.Android.Src.FindUs.Activity;
using myTNB.Android.Src.MyLearnMoreAboutTnb.MVP;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.TermsAndConditions.Activity;
using myTNB.Android.Src.UpdateMobileNo.Activity;
using myTNB.Android.Src.UpdatePassword.Activity;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.Android.Src.MyLearnMoreAboutTnb.Activity
{
    [Activity(Label = "Learn More About TNB"
      //, NoHistory = false
      //, Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class MyLearnMoreActivity : BaseActivityCustom, MyLearnMoreContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.profileMenuItemsContent)]
        LinearLayout profileMenuItemsContent;

        [BindView(Resource.Id.appVersion)]
        TextView appVersion;

        [BindView(Resource.Id.btnLogout)]
        Button btnLogout;

        MyLearnMorePresenter mPresenter;

        const string PAGE_ID = "LearnMoreAboutTnb";

        MyLearnMoreContract.IUserActionsListener userActionsListener;


        public override int ResourceId()
        {
            return Resource.Layout.ProfileDetailPage;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                LearnMoreTnbItemComponent helpSupportItem = GetHelpSupportItems();
                profileMenuItemsContent.AddView(helpSupportItem);

                appVersion.Text = Utility.GetAppVersionName(this);
                mPresenter = new MyLearnMorePresenter(this);
                //this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]

        private LearnMoreTnbItemComponent GetHelpSupportItems()
        {

            LearnMoreTnbItemComponent helpSupportItem = new LearnMoreTnbItemComponent(this);

            List<View> helpSupportItems = new List<View>();

            ProfileMenuItemSingleContentComponent billInquiry = new ProfileMenuItemSingleContentComponent(this);
            if (WeblinkEntity.HasRecord("TNBCLE"))
            {
                WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLE");
                if (entity != null && !string.IsNullOrEmpty(entity.Title))
                {
                    billInquiry.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "callUsBill"));
                }
                else 
                {
                    billInquiry.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "callUsBill"));
                }
            }
            else
            {
                billInquiry.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "callUsBill"));
            }
            billInquiry.SetItemActionCall(ShowCallUsBilling);
            helpSupportItems.Add(billInquiry);

            ProfileMenuItemSingleContentComponent outage = new ProfileMenuItemSingleContentComponent(this);
            if (WeblinkEntity.HasRecord("TNBCLO"))
            {
                WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLO");
                if (entity != null && !string.IsNullOrEmpty(entity.Title))
                {
                    outage.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "callUsOutage"));
                }
                else
                {
                    outage.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "callUsOutage")); 
                }
            }
            else
            {
                outage.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "callUsOutage"));
            }
            outage.SetItemActionCall(ShowCallUsOutage);
            helpSupportItems.Add(outage);

            ProfileMenuItemSingleContentComponent findUs = new ProfileMenuItemSingleContentComponent(this);
            findUs.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "findUs")); 
            findUs.SetItemActionCall(ShowFindUs);
            helpSupportItems.Add(findUs);

            ProfileMenuItemSingleContentComponent faq = new ProfileMenuItemSingleContentComponent(this);
            faq.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "faq"));
            faq.SetItemActionCall(ShowFAQ);
            helpSupportItems.Add(faq);

            ProfileMenuItemSingleContentComponent TnC = new ProfileMenuItemSingleContentComponent(this);
            TnC.SetTitle(Utility.GetLocalizedLabel("LearnMoreAboutTnb", "tnc"));
            TnC.SetItemActionCall(ShowTnC);
            helpSupportItems.Add(TnC);

            helpSupportItem.AddComponentView(helpSupportItems);
            return helpSupportItem;
        }

        private void ShowFindUs()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(new Intent(this, typeof(MapActivity)));
            }
        }

        private void ShowCallUsBilling()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (WeblinkEntity.HasRecord("TNBCLE"))
                {
                    WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLE");
                    if (entity.OpenWith.Equals("PHONE"))
                    {
                        var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                        var intent = new Intent(Intent.ActionDial, uri);
                        StartActivity(intent);
                    }
                }
            }
        }

        private void ShowCallUsOutage()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (WeblinkEntity.HasRecord("TNBCLO"))
                {
                    WeblinkEntity entity = WeblinkEntity.GetByCode("TNBCLO");
                    if (entity.OpenWith.Equals("PHONE"))
                    {
                        var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                        var intent = new Intent(Intent.ActionDial, uri);
                        StartActivity(intent);
                    }
                }
            }
        }

        private void ShowFAQ()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(new Intent(this, typeof(FAQListActivity)));
            }
        }

        private void ShowTnC()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(new Intent(this, typeof(TermsAndConditionActivity)));
            }
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            try
            {
                if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
                {
                    mCancelledExceptionSnackBar.Dismiss();
                }

                mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {

                    mCancelledExceptionSnackBar.Dismiss();
                }
                );
                View v = mCancelledExceptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mCancelledExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            try
            {
                if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
                {
                    mApiExcecptionSnackBar.Dismiss();
                }

                mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();
                }
                );
                View v = mApiExcecptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mApiExcecptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            try
            {
                if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
                {
                    mUknownExceptionSnackBar.Dismiss();

                }

                mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {

                    mUknownExceptionSnackBar.Dismiss();
                }
                );
                View v = mUknownExceptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mUknownExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "LearnMoreAboutTnb");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void HideShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(MyLearnMoreContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                //this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

       
        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
