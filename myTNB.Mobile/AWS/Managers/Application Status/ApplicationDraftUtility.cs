using System.Collections.Generic;
using System.Linq;
using myTNB.Mobile.Constants;

namespace myTNB.Mobile.AWS.Managers.ApplicationStatus
{
    internal static class ApplicationDraftUtility
    {
        internal static void UpdateDraftResponse(ref PostGetDraftResponse response
            , List<string> localList)
        {
            if (response != null
                && response.StatusDetail != null
                && response.StatusDetail.IsSuccess
                && response.Content != null
                && response.Content.Applications != null
                && response.Content.Applications.Count > 0)
            {
                response.Content.ApplicationList = new List<string>();
                List<string> newLocalList = new List<string>();
                string title = string.Empty;
                string message = string.Empty;

                if (localList != null && localList.Count > 0)
                {
                    List<string> responseNCList = response.Content.Applications.Select(x => x.ReferenceNo).ToList();
                    newLocalList = responseNCList.Except(localList).ToList();
                    response.Content.ApplicationList.AddRange(localList);
                }
                else
                {
                    newLocalList = response.Content.Applications.Select(x => x.ReferenceNo).ToList();
                }
                if (newLocalList.Count > 1)
                {
                    response.Content.IsMultipleDraft = true;
                    string applicationString = string.Empty;
                    for (int i = 0; i < newLocalList.Count; i++)
                    {
                        applicationString += newLocalList[i];
                        if (i < newLocalList.Count - 1)
                        {
                            applicationString += ", ";
                        }
                    }
                    title = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationsTitle);
                    message = string.Format(LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationsMessage), applicationString);
                }
                else if (newLocalList.Count == 1)
                {
                    title = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationTitle);
                    message = string.Format(LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationMessage), newLocalList[0]);
                }

                response.Content.ReminderTitle = title;
                response.Content.ReminderMessage = message;
                response.Content.PrimaryCTA = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                    , MarketingPopup.MyHome.I18N_ResumeApplicationPrimaryCTA);
                response.Content.SecondaryCTA = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                    , MarketingPopup.MyHome.I18N_ResumeApplicationSecondaryCTA);
                response.Content.ApplicationList.AddRange(newLocalList);
            }
        }
    }
}