using System;
using System.Collections.Generic;

namespace myTNB.Feedback.Enquiry.EnquiryStatusPage
{
    public class EnquiryStatusPageConstants
    {
        public static Dictionary<string, string> FeedbackI18NDictionary = new Dictionary<string, string>
        {
            { "success", "feedbackSuccessTitle"},
            { "fail", "feedbackFailTitle"},
            { "successMessage", "feedbackSuccessMessage" },
            { "failMessage", "feedbackFailMessage" },
            { "referenceTitle", "feedbackReferenceTitle" },
            { "dateTitle", "feedbackDateTitle"}
        };

        public static Dictionary<string, string> SSMRApplyI18NDictionary = new Dictionary<string, string>
        {
            { "success", "ssmrApplySuccessTitle"},
            { "fail", "ssmrApplyFailTitle"},
            { "successMessage", "ssmrApplySuccessMessage" },
            { "failMessage", "ssmrApplyFailMessage" },
            { "referenceTitle", "ssmrApplyReferenceTitle" },
            { "dateTitle", "ssmrApplyDateTitle"}
        };

        public static Dictionary<string, string> SSMRDiscontinueI18NDictionary = new Dictionary<string, string>
        {
            { "success", "ssmrDiscontinueSuccessTitle"},
            { "fail", "ssmrDiscontinueFailTitle"},
            { "successMessage", "ssmrDiscontinueSuccessMessage" },
            { "failMessage", "ssmrDiscontinueFailMessage" },
            { "referenceTitle", "ssmrApplyReferenceTitle" },
            { "dateTitle", "ssmrApplyDateTitle"}
        };

        public static Dictionary<string, string> SSMRReadingI18NDictionary = new Dictionary<string, string>
        {
            { "success", "ssmrReadingSuccessTitle"},
            { "fail", "ssmrReadingFailTitle"},
            { "successMessage", "ssmrReadingSuccessMessage" },
            { "failMessage", "ssmrReadingFailMessage" }
        };

        public static string Success = "success";
        public static string Fail = "fail";
        public static string SuccessMessage = "successMessage";
        public static string FailMessage = "failMessage";
        public static string ReferenceTitle = "referenceTitle";
        public static string DateTitle = "dateTitle";

        public static string IMG_Success = "Circle-With-Check-Green";
        public static string IMG_Fail = "Red-Cross";

        //I18N
        public static string I18N_BackToFeedback = "backToFeedback";
        public static string I18N_SSMRTrackApplication = "ssmrTrackApplication";
        public static string I18N_SSMRBacktoUsage = "ssmrBackToUsage";
        public static string I18N_SSMRTrackUsage = "ssmrTrackUsage";
        public static string I18N_SSMRViewReadHistory = "ssmrViewReadHistory";
        public static string I18N_BacktoHome = "backToHome";
        public static string I18N_TryAgain = "tryAgain";
        public static string I18N_SSMRBackToReadingHistory = "ssmrBackToReadingHistory";

        public static string PageName = "Status";
    }
}
