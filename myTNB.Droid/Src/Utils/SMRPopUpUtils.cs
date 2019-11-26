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
        private static bool fromUsage = false;
        private static bool fromUsageSubmitSuccessful = false;

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

        public static bool OnGetIsOCRDownFlag()
        {
            bool isOCRDown = false;
            if (smrResponse != null && smrResponse.Response != null)
            {
                if (smrResponse.Response.IsOCRDisabled || smrResponse.Response.IsOCRDown)
                {
                    isOCRDown = true;
                }
            }

            return isOCRDown;
        }

        public static bool IsOCRDisabled()
        {
            bool isOCRDisabled = false;
            if (smrResponse != null && smrResponse.Response != null)
            {
                isOCRDisabled = smrResponse.Response.IsOCRDisabled;
            }

            return isOCRDisabled;
        }

        public static bool IsOCRDown()
        {
            bool IsOCRDown = false;
            if (smrResponse != null && smrResponse.Response != null)
            {
                IsOCRDown = smrResponse.Response.IsOCRDown;
            }

            return IsOCRDown;
        }

        public static void SetFromUsageFlag(bool flag)
        {
            fromUsage = flag;
        }

        public static bool GetFromUsageFlag()
        {
            return fromUsage;
        }

        public static void SetFromUsageSubmitSuccessfulFlag(bool flag)
        {
            fromUsageSubmitSuccessful = flag;
        }

        public static bool GetFromUsageSubmitSuccessfulFlag()
        {
            return fromUsageSubmitSuccessful;
        }

        public static string GetTitle()
        {
            string title = "Sorry, we are unable to perform this action right now.";

            if (smrResponse != null && smrResponse.Response != null && !string.IsNullOrEmpty(smrResponse.Response.DisplayTitle))
            {
                title = smrResponse.Response.DisplayTitle;
            }

            return title;

        }

        public static string GetMessage()
        {
            string message = "Please try again later. If this problem persists, contact the <b><a href=\"tel:1300885454\">TNB Careline</a></b> and we will help you.";

            if (smrResponse != null && smrResponse.Response != null && !string.IsNullOrEmpty(smrResponse.Response.DisplayMessage))
            {
                message = smrResponse.Response.DisplayMessage;
            }

            return message;

        }
    }
}