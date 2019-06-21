using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
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
    public class TermsAndConditionActivity : BaseToolbarAppCompatActivity, TermsAndConditionContract.IView
    {
        private TermsAndConditionPresenter mPresenter;
        private TermsAndConditionContract.IUserActionsListener userActionsListener;

        TextView txtTitle;
        TextView txtVersion;
        //TextView txtTnCHtml;

        WebView tncWebView;

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
                                    if (obj.GeneralText != null && obj.PublishedDate != null)
                                    {
                                        string replacedString = obj.GeneralText.Replace("\\", string.Empty);
                                        Console.WriteLine("ReplaceStringOne::" + replacedString);
                                        replacedString = obj.GeneralText.Replace("\\n", string.Empty);
                                        Console.WriteLine("ReplaceStringTwo::" + replacedString);
                                        //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                                        //{
                                        //        txtTnCHtml.TextFormatted = Html.FromHtml(replacedString, FromHtmlOptions.ModeLegacy);
                                        //}
                                        //else
                                        //{
                                        //        txtTnCHtml.TextFormatted = Html.FromHtml(replacedString);
                                        //}
                                        tncWebView.LoadData(replacedString, "text/html", "UTF-8");
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

                if (MyTNBApplication.siteCoreUpdated)
                {
                    GetDataFromSiteCore();
                }
                else
                {
                    ShowTermsAndCondition(true);
                }
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
                string tnc_version = "14 Dec 2105";
                txtVersion.Text = "Version [" + tnc_version + "]";
                txtTitle.Text = GetString(Resource.String.tnc_title);
                // txtTnCHtml.TextFormatted = Html.FromHtml(GetString(Resource.String.tnc_html));
                tncWebView.LoadData(GetString(Resource.String.tnc_html), "text/html", "UTF-8");
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
                this.userActionsListener.GetTermsAndConditionData();
            }
            catch (Exception e)
            {
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