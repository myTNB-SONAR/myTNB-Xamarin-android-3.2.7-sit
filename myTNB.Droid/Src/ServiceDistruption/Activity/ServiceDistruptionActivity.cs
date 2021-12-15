using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using DynatraceAndroid;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ServiceDistruption.MVP;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.ServiceDistruption.Activity
{
    [Activity(Label = "@string/app_name"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.PreLogin")]
    public class ServiceDisruptionActivity : BaseAppCompatActivity, ServiceDistruptionContract.IView
    {

        private ServiceDistruptionPresenter mPresenter;
        private ServiceDistruptionContract.IUserActionsListener userActionsListener;

        [BindView(Resource.Id.txtTitleSD)]
        TextView txtTitleSD;

        [BindView(Resource.Id.btnOkay)]
        Button btnOkay;

        [BindView(Resource.Id.txtParaOne)]
        TextView txtParaOne;

        [BindView(Resource.Id.txtParaTwo)]
        TextView txtParaTwo;

        [BindView(Resource.Id.txtTnC)]
        TextView txtTnC;

        IDTXAction dynaTrace;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new ServiceDistruptionPresenter(this);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleSD);
                TextViewUtils.SetMuseoSans500Typeface(btnOkay);
                TextViewUtils.SetMuseoSans300Typeface(txtParaOne, txtParaTwo, txtTnC);

                TextViewUtils.SetTextSize16(txtTitleSD);
                TextViewUtils.SetTextSize16(btnOkay);
                TextViewUtils.SetTextSize13(txtParaOne, txtParaTwo);
                TextViewUtils.SetTextSize11(txtTnC);

                txtTitleSD.Text = Utility.GetLocalizedLabel("ServiceDistruptionComm", "title");
                txtParaOne.Text = Utility.GetLocalizedLabel("ServiceDistruptionComm", "subTitle1");
                txtParaTwo.Text = Utility.GetLocalizedLabel("ServiceDistruptionComm", "subTitle2");
                txtTnC.Text = Utility.GetLocalizedLabel("ServiceDistruptionComm", "note");
                btnOkay.Text = Utility.GetLocalizedLabel("ServiceDistruptionComm", "buttonCTA");
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnOkay)]
        internal void OnOkayBtn(object sender, EventArgs e)
        {
            try
            {
                base.OnBackPressed();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ServiceDistruptionView;
        }

        public void SetPresenter(ServiceDistruptionContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                //FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_initiate_Duration);
                //dynaTrace = DynatraceAndroid.Dynatrace.EnterAction(Constants.EB_initiate_Duration);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            try
            {
                //dynaTrace.LeaveAction();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
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

        public void DismissProgressDialog()
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

        private ISpanned GetFormattedText(string stringValue)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(stringValue, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                return Html.FromHtml(stringValue);
            }
        }
    }
}
