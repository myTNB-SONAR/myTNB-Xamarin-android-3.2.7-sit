
using AndroidX.Fragment.App;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP;
using myTNB.AndroidApp.Src.SSMR.SSMRMeterReadingTooltip.MVP;
using System;
using System.Collections.Generic;
using System.Net;

namespace myTNB.AndroidApp.Src.Utils
{
    public class SMRPopUpUtils
    {
        private static SMRActivityInfoResponse smrResponse;
        private static bool fromUsage = false;
        private static bool fromUsageSubmitSuccessful = false;
        private static bool ssmrMeterReadingTutorialRefreshNeeded = false;

        public static SSMRMeterReadingDialogFragment OnShowSMRMeterReadingTooltipOnActivity(bool isSinglePhase, Android.App.Activity mActivity, FragmentManager mManager, List<SSMRMeterReadingModel> list)
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

        public static void SetSSMRMeterReadingRefreshNeeded(bool flag)
        {
            ssmrMeterReadingTutorialRefreshNeeded = flag;
        }

        public static void OnResetSSMRMeterReadingTimestamp()
        {
            try
            {
                SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();

                SSMRMeterReadingScreensParentManager.DeleteTable();
                SSMRMeterReadingScreensParentManager.CreateTable();

                SSMRMeterReadingScreensOCROffParentEntity SSMRMeterReadingScreensOCROffParentManager = new SSMRMeterReadingScreensOCROffParentEntity();

                SSMRMeterReadingScreensOCROffParentManager.DeleteTable();
                SSMRMeterReadingScreensOCROffParentManager.CreateTable();

                SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();

                SSMRMeterReadingThreePhaseScreensParentManager.DeleteTable();
                SSMRMeterReadingThreePhaseScreensParentManager.CreateTable();

                SSMRMeterReadingThreePhaseScreensOCROffParentEntity SSMRMeterReadingThreePhaseScreensOCROffParentManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();

                SSMRMeterReadingThreePhaseScreensOCROffParentManager.DeleteTable();
                SSMRMeterReadingThreePhaseScreensOCROffParentManager.CreateTable();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static bool GetSSMRMeterReadingRefreshNeeded()
        {
            return ssmrMeterReadingTutorialRefreshNeeded;
        }

        public static string GetTitle()
        {
            string title = Utility.GetLocalizedLabel("Usage", "prevReadingEmptyTitle");

            if (smrResponse != null && smrResponse.Response != null && !string.IsNullOrEmpty(smrResponse.Response.DisplayTitle))
            {
                title = smrResponse.Response.DisplayTitle;
            }

            return title;

        }

        public static string GetMessage()
        {
            string message = Utility.GetLocalizedLabel("Usage", "prevReadingEmptyMsg");

            if (smrResponse != null && smrResponse.Response != null && !string.IsNullOrEmpty(smrResponse.Response.DisplayMessage))
            {
                message = smrResponse.Response.DisplayMessage;
            }

            return message;

        }
    }
}