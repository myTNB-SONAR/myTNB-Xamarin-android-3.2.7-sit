using System.Collections.Generic;
using System.Linq;
using myTNB.Mobile.Constants;

namespace myTNB.Mobile.AWS.Managers.ApplicationStatus
{
    internal static class NCDraftUtility
    {
        internal static void UpdateNCDraftResponse(ref PostGetNCDraftResponse response
            , List<string> localNCList)
        {
            if (response != null
                && response.StatusDetail != null
                && response.StatusDetail.IsSuccess
                && response.Content != null
                && response.Content.Applications != null
                && response.Content.Applications.Count > 0)
            {
                response.Content.NCApplicationList = new List<string>();
                List<string> newLocalNCList = new List<string>();
                string title = string.Empty;
                string message = string.Empty;

                if (localNCList != null && localNCList.Count > 0)
                {
                    List<string> responseNCList = response.Content.Applications.Select(x => x.ReferenceNo).ToList();
                    newLocalNCList = responseNCList.Except(localNCList).ToList();
                    response.Content.NCApplicationList.AddRange(localNCList);
                }
                else
                {
                    newLocalNCList = response.Content.Applications.Select(x => x.ReferenceNo).ToList();
                }
                if (newLocalNCList.Count > 1)
                {
                    string ncString = string.Empty;
                    for (int i = 0; i < newLocalNCList.Count; i++)
                    {
                        ncString += newLocalNCList[i];
                        if (i < newLocalNCList.Count - 1)
                        {
                            ncString += ", ";
                        }
                    }
                    title = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationsTitle);
                    message = string.Format(LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationsMessage), ncString);
                }
                else if (newLocalNCList.Count == 1)
                {
                    title = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationTitle);
                    message = string.Format(LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                        , MarketingPopup.MyHome.I18N_ResumeApplicationMessage), newLocalNCList[0]);
                }

                response.Content.ReminderTitle = title;
                response.Content.ReminderMessage = message;
                response.Content.PrimaryCTA = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                    , MarketingPopup.MyHome.I18N_ResumeApplicationPrimaryCTA);
                response.Content.SecondaryCTA = LanguageManager.Instance.GetPageValueByKey("MarketingPopup"
                    , MarketingPopup.MyHome.I18N_ResumeApplicationSecondaryCTA);
                response.Content.NCApplicationList.AddRange(newLocalNCList);
            }
        }
    }
}