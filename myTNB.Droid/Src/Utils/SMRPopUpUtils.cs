using AFollestad.MaterialDialogs;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMR.SSMRMeterReadingTooltip.MVP;
using System.Collections.Generic;
using System.Net;

namespace myTNB_Android.Src.Utils
{
    public class SMRPopUpUtils
    {
        private static SMRActivityInfoResponse smrResponse;

        public static SSMRMeterReadingDialogFragment OnShowSMRMeterReadingTooltipOnActivity(bool isSinglePhase, Android.App.Activity mActivity, Android.Support.V4.App.FragmentManager mManager, List<SSMRMeterReadingModel> list)
        {
            SSMRMeterReadingDialogFragment dialogFragmnet = new SSMRMeterReadingDialogFragment(mActivity, isSinglePhase, list);
            dialogFragmnet.Cancelable = false;
            dialogFragmnet.Show(mManager, "SMRMeterReading Dialog");
            return dialogFragmnet;
        }

        public static void OnSetSMRActivityInfoResponse(SMRActivityInfoResponse response)
        {
            smrResponse = response;
        }
    }
}