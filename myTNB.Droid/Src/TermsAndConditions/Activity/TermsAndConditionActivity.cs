using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Android.Content.PM;
using Android.Text;
using myTNB_Android.Src.TermsAndConditions.MVP;
using myTNB_Android.Src.Database.Model;
using CheeseBind;
using Android.Text.Method;

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
        TextView txtTnCHtml;

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
            RunOnUiThread(() => {
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
                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                                    {
                                        txtTnCHtml.TextFormatted = Html.FromHtml(obj.GeneralText, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        txtTnCHtml.TextFormatted = Html.FromHtml(obj.GeneralText);
                                    }
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new TermsAndConditionPresenter(this);

            // Create your application here
            txtTitle = FindViewById<TextView>(Resource.Id.txt_tnc_title);
            txtVersion = FindViewById<TextView>(Resource.Id.txt_tnc_version);
            txtTnCHtml = FindViewById<TextView>(Resource.Id.txt_tnc_html);
            txtTnCHtml.MovementMethod = LinkMovementMethod.Instance;

            TextViewUtils.SetMuseoSans500Typeface(txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtVersion, txtTnCHtml);

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

        public void SetDefaultData()
        {
            string tnc_version = "14 Dec 2105";
            txtVersion.Text = "Version [" + tnc_version + "]";
            txtTitle.Text = GetString(Resource.String.tnc_title);
            txtTnCHtml.TextFormatted = Html.FromHtml(GetString(Resource.String.tnc_html));
        }

        public void GetDataFromSiteCore()
        {
            progressBar.Visibility = ViewStates.Visible;
            this.userActionsListener.GetTermsAndConditionData();
        }
    }
}