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
using myTNB_Android.Src.TermsAndConditions.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.TermsAndConditions.Activity
{
    [Activity(Label = "@string/terms_conditions_activity_title",
        ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.TnC")]
    public class TermsAndConditionActivity : BaseActivityCustom, TermsAndConditionContract.IView
    {
        private TermsAndConditionPresenter mPresenter;
        private TermsAndConditionContract.IUserActionsListener userActionsListener;

        TextView txtTitle;
        TextView txtVersion;
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
            return Resource.Layout.TermsAndConditionView;
        }

        public void SetPresenter(TermsAndConditionContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowTermsAndCondition(bool success)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        if (success)
                        {
                            FullRTEPagesEntity wtManager = new FullRTEPagesEntity();
                            List<FullRTEPagesEntity> items = wtManager.GetAllItems();
                            if (items != null)
                            {
                                if (items.Count == 0)
                                {
                                    GetDataFromSiteCore();
                                }
                                else
                                {
                                    foreach (FullRTEPagesEntity obj in items)
                                    {
                                        if (!string.IsNullOrEmpty(obj.GeneralText) && !string.IsNullOrEmpty(obj.PublishedDate))
                                        {
                                            string replacedString = obj.GeneralText.Replace("\\", string.Empty);
                                            Console.WriteLine("ReplaceStringOne::" + replacedString);
                                            replacedString = obj.GeneralText.Replace("\\n", string.Empty);
                                            Console.WriteLine("ReplaceStringTwo::" + replacedString);
                                            tncWebView.LoadDataWithBaseURL("", replacedString, "text/html", "UTF-8", "");
                                            txtVersion.Text = "Version [" + obj.PublishedDate + "]";
                                            txtTitle.Text = "" + obj.Title;
                                        }
                                        else
                                        {
                                            SetDefaultData();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                GetDataFromSiteCore();
                            }
                        }
                        else
                        {
                            SetDefaultData();
                        }
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
                mPresenter = new TermsAndConditionPresenter(this);

                // Create your application here
                txtTitle = FindViewById<TextView>(Resource.Id.txt_tnc_title);
                txtVersion = FindViewById<TextView>(Resource.Id.txt_tnc_version);
                //txtTnCHtml = FindViewById<TextView>(Resource.Id.txt_tnc_html);
                //txtTnCHtml.MovementMethod = LinkMovementMethod.Instance;
                //txtTnCHtml.JustificationMode = JustificationMode.InterWord;

                tncWebView = FindViewById<WebView>(Resource.Id.tncWebView);
                TextViewUtils.SetMuseoSans500Typeface(txtTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtVersion/*, txtTnCHtml*/);

                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                txtTitle.Text = "";

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
                string tnc = "";

                if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                {
                    tnc = TnCManager.Instance.GetTnC(TnCManager.Language.MS);
                }
                else
                {
                    tnc = TnCManager.Instance.GetTnC();
                }

                string replacedString = tnc.Replace("\\", string.Empty);
                Console.WriteLine("ReplaceStringOne::" + replacedString);
                replacedString = replacedString.Replace("\\n", string.Empty);
                replacedString = replacedString.Replace("\n", string.Empty);

                string[] tncArray = replacedString.Split("<br>");

                txtVersion.Text = "Version [" + tncArray[1] + "]";
                txtTitle.Text = tncArray[0];
                // txtTnCHtml.TextFormatted = Html.FromHtml(GetString(Resource.String.tnc_html));
                tncWebView.LoadDataWithBaseURL("", tncArray[3], "text/html", "UTF-8", "");
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
                this.userActionsListener.GetSavedTermsAndConditionTimeStamp();
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
            this.userActionsListener.OnGetTermsAndConditionTimeStamp();
        }

        public void ShowTermsAndConditionTimestamp(bool success)
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
                                this.userActionsListener.GetTermsAndConditionData();
                            }
                            else
                            {
                                MyTNBApplication.siteCoreUpdated = false;
                                ShowTermsAndCondition(true);
                            }
                        }
                        else
                        {
                            MyTNBApplication.siteCoreUpdated = true;
                            this.userActionsListener.GetTermsAndConditionData();
                        }
                    }
                    else
                    {
                        MyTNBApplication.siteCoreUpdated = true;
                        this.userActionsListener.GetTermsAndConditionData();
                    }
                }
                else
                {
                    MyTNBApplication.siteCoreUpdated = true;
                    this.userActionsListener.GetTermsAndConditionData();
                }
            }
            catch (Exception e)
            {
                MyTNBApplication.siteCoreUpdated = true;
                this.userActionsListener.GetTermsAndConditionData();
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

        // AndroidX TODO: Temporary Fix for Android 5,5.1 
        // AndroidX TODO: Due to this: https://github.com/xamarin/AndroidX/issues/131
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