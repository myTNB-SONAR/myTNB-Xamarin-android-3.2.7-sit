using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DigitalBill.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.DigitalBill.Activity
{
    [Activity(Label = "@string/terms_conditions_activity_title",
        ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.TnC")]
    public class DigitalBillActivity : BaseActivityCustom, DigitalBillContract.IView
    {
        private DigitalBillPresenter mPresenter;
        private DigitalBillContract.IUserActionsListener userActionsListener;

        //TextView txtTitle;
        //TextView txtVersion;
        //TextView txtTnCHtml;

        WebView tncWebView;

        const string PAGE_ID = "TnC";

        private string mSavedTimeStamp = "0000000";

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DigitalBillView;
        }

        public void SetPresenter(DigitalBillContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowDigitalBill(bool success)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        SetDefaultData();
                    }
                    catch (System.Exception er)
                    {
                        Utility.LoggingNonFatalError(er);
                    }
                });
            }
            catch (Exception e)
            {
                progressBar.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                mPresenter = new DigitalBillPresenter(this);

                // Create your application here
                //txtTitle = FindViewById<TextView>(Resource.Id.txt_tnc_title);
                //txtVersion = FindViewById<TextView>(Resource.Id.txt_tnc_version);
                //txtTnCHtml = FindViewById<TextView>(Resource.Id.txt_tnc_html);
                //txtTnCHtml.MovementMethod = LinkMovementMethod.Instance;
                //txtTnCHtml.JustificationMode = JustificationMode.InterWord;

                tncWebView = FindViewById<WebView>(Resource.Id.tncWebView);
                //TextViewUtils.SetMuseoSans500Typeface(txtTitle);
                //TextViewUtils.SetMuseoSans300Typeface(txtVersion/*, txtTnCHtml*/);
                //TextViewUtils.SetTextSize14(txtTitle, txtVersion);

                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                //txtTitle.Text = "";

                GetDataFromSiteCore();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetDefaultData()
        {
            try
            {
                string HTMLText = "<html>" + "<body><b>MicroSite</b><br/><br/></body>" +
                               "</html>";

                //txtVersion.Text = "Version [" + tncArray[1] + "]";
                //txtTitle.Text = tncArray[0];
                // txtTnCHtml.TextFormatted = Html.FromHtml(GetString(Resource.String.tnc_html));
                tncWebView.LoadDataWithBaseURL("", HTMLText, "text/html", "UTF-8", "");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetDataFromSiteCore()
        {
            try
            {
                progressBar.Visibility = ViewStates.Visible;
                this.userActionsListener.GetSavedDigitalBillTimeStamp();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedTimeStamp(string savedTimeStamp)
        {
            if (savedTimeStamp != null)
            {
                this.mSavedTimeStamp = savedTimeStamp;
            }
            this.userActionsListener.OnGetDigitalBillTimeStamp();
        }

        public void ShowDigitalBillTimestamp(bool success)
        {
            try
            {
                if (success)
                {
                    TimeStampEntity wtManager = new TimeStampEntity();
                    List<TimeStampEntity> items = wtManager.GetAllItems();
                    if (items != null)
                    {
                        TimeStampEntity entity = items[0];
                        if (entity != null)
                        {
                            if (!entity.Timestamp.Equals(mSavedTimeStamp))
                            {
                                MyTNBApplication.siteCoreUpdated = true;
                                this.userActionsListener.GetDigitalBillData();
                            }
                            else
                            {
                                MyTNBApplication.siteCoreUpdated = false;
                                ShowDigitalBill(true);
                            }
                        }
                        else
                        {
                            MyTNBApplication.siteCoreUpdated = true;
                            this.userActionsListener.GetDigitalBillData();
                        }
                    }
                    else
                    {
                        MyTNBApplication.siteCoreUpdated = true;
                        this.userActionsListener.GetDigitalBillData();
                    }
                }
                else
                {
                    MyTNBApplication.siteCoreUpdated = true;
                    this.userActionsListener.GetDigitalBillData();
                }
            }
            catch (Exception e)
            {
                MyTNBApplication.siteCoreUpdated = true;
                this.userActionsListener.GetDigitalBillData();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressBar()
        {
            try
            {
                progressBar.Visibility = ViewStates.Gone;
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Terms And Conditions");
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

        //  TODO: AndroidX Temporary Fix for Android 5,5.1 
        //  TODO: AndroidX Due to this: https://github.com/xamarin/AndroidX/issues/131
        public override AssetManager Assets =>
            (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop && Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            ? Resources.Assets : base.Assets;

        //public override void OnTrimMemory(TrimMemory level)
        //{
        //    base.OnTrimMemory(level);

        //    switch (level)
        //    {
        //        case TrimMemory.RunningLow:
        //            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        //            GC.Collect();
        //            break;
        //        default:
        //            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        //            GC.Collect();
        //            break;
        //    }
        //}
    }
}