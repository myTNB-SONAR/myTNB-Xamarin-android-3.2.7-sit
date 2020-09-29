using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.Extensions;
using System.Diagnostics;

namespace myTNB.Mobile.API.Managers.ApplicationStatus.Utilities
{
    internal static class GetApplicationDetailsUtility
    {
        private static List<SelectorModel> _mappingList;
        private static string _addFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "add");
        private static string _updateFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "update");
        private static string _removeFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "remove");
        private const string KEDAI = "KEDAI";
        private const string SAVED = "SAVED";

        internal static ApplicationDetailDisplay Parse(this GetApplicationDetailsResponse response
            , string searchApplicationType
            , string applicationModuleDescription
            , string applicationID
            , string applicationModuleId
            , string createdByRoleID
            , bool isPremiseServiceReady
            , bool isSavedApplication)
        {
            {
                ApplicationDetailDisplay displayModel = new ApplicationDetailDisplay
                {
                    Content = new GetApplicationStatusDisplay
                    {
                        //Mark: Unused by ApplicationDetail Service
                        ApplicationDetail = new ApplicationDetailDisplayModel(),
                        //applicationPaymentDetail = new ApplicationPaymentDetail(),
                        //ApplicationStatusDetail = new ApplicationStatusDetailDisplayModel(),
                        ApplicationActivityLogDetail = new List<ApplicationActivityLogDetailDisplay>()
                    },
                    StatusDetail = response.StatusDetail
                };
                if (response != null && response.Content != null)
                {
                    displayModel.Content.ApplicationTypeReference = applicationModuleDescription;
                    displayModel.Content.IsSaveMessageDisplayed = false;
                    displayModel.Content.IsFullApplicationTooltipDisplayed = true;
                    displayModel.Content.IsDeleteEnable = true;
                    displayModel.Content.IsSavedApplication = isSavedApplication;

                    displayModel.Content.ApplicationDetail.ApplicationId = applicationID;

                    if (response.Content.applicationPaymentDetail != null)
                    {
                        displayModel.Content.applicationPaymentDetail = response.Content.applicationPaymentDetail;
                        displayModel.Content.PaymentDisplay = new PaymentDisplayModel
                        {
                            outstandingChargesAmount = response.Content.applicationPaymentDetail.outstandingChargesAmount,
                            latestBillAmount = response.Content.applicationPaymentDetail.latestBillAmount,
                            oneTimeChargesAmount = response.Content.applicationPaymentDetail.oneTimeChargesAmount,
                            oneTimeChargesDetail = response.Content.applicationPaymentDetail.oneTimeChargesDetail,
                            totalPayableAmount = response.Content.applicationPaymentDetail.totalPayableAmount,
                            caNo = response.Content.applicationPaymentDetail.caNo,
                            sdDocumentNo = response.Content.applicationPaymentDetail.sdDocumentNo,
                            srNo = response.Content.applicationPaymentDetail.srNo
                        };
                    }
                    if (response.Content.ApplicationStatusDetail != null)
                    {
                        Dictionary<string, List<SelectorModel>> selectors = LanguageManager.Instance.GetSelectorsByPage("ApplicationStatusDetails");
                        _mappingList = new List<SelectorModel>();
                        if (selectors != null && selectors.ContainsKey("applicationTypeMapping"))
                        {
                            _mappingList = selectors["applicationTypeMapping"];
                        }

                        List<SelectorModel> additionalDisplayConfig = new List<SelectorModel>();
                        string key = searchApplicationType;
                        if (displayModel.Content.IsKedaiTenagaApplication)
                        {
                            key = KEDAI;
                        }
                        if (response.Content.savedApplicationDetail != null)
                        {
                            key = SAVED;
                        }
                        GetNCType(ref key
                            , searchApplicationType
                            , applicationModuleId
                            , createdByRoleID
                            , isPremiseServiceReady);
                        if (selectors != null && selectors.ContainsKey(key))
                        {
                            additionalDisplayConfig = selectors[key];
                        }
                        if (additionalDisplayConfig != null && additionalDisplayConfig.Count > 0)
                        {
                            displayModel.Content.AdditionalInfoList = new List<TitleValueModel>();
                            AddAdditionalInfo(ref displayModel
                                , response.Content
                                , additionalDisplayConfig
                                , searchApplicationType == "NC" ? searchApplicationType : key);
                        }

                        displayModel.Content.ApplicationStatusDetail = new ApplicationStatusDetailDisplayModel
                        {
                            StatusId = response.Content.ApplicationStatusDetail.StatusId,
                            StatusCode = response.Content.ApplicationStatusDetail.StatusCode,
                            StatusDescription = response.Content.ApplicationStatusDetail.StatusDescription,
                            StatusMessage = response.Content.ApplicationStatusDetail.StatusMessage,
                            UserAction = response.Content.ApplicationStatusDetail.UserAction,
                            IsPostPayment = response.Content.ApplicationStatusDetail.IsPostPayment
                        };
                        if (response.Content.ApplicationStatusDetail.StatusTracker != null)
                        {
                            displayModel.Content.ApplicationStatusDetail.StatusTracker = new List<StatusTrackerDisplay>();
                            displayModel.Content.ApplicationStatusDetail.StatusTracker = response.Content.ApplicationStatusDetail
                                .StatusTracker.Select(x => new StatusTrackerDisplay
                                {
                                    StatusDescription = x.StatusDescription,
                                    StatusMode = x.StatusMode,
                                    //Todo: Fix This
                                    /*ProgressDetail = new ProgressDetailDisplay
                                    {
                                        ProjectID = x.ProgressDetail.TNBProjectID,
                                        ProgressTrackers = x.ProgressDetail.ProgressTrackers
                                    },*/
                                    Sequence = x.Sequence
                                }).ToList();
                        }
                    }
                    if (response.Content.ApplicationActivityLogDetail != null)
                    {
                        displayModel.Content.ApplicationActivityLogDetail = new List<ApplicationActivityLogDetailDisplay>();
                        for (int i = 0; i < response.Content.ApplicationActivityLogDetail.Count; i++)
                        {
                            ApplicationActivityLogDetail logDetail = response.Content.ApplicationActivityLogDetail[i];

                            ApplicationActivityLogDetailDisplay displayItem = new ApplicationActivityLogDetailDisplay
                            {
                                StatusID = logDetail.StatusID,
                                StatusDescription = logDetail.StatusDescription,
                                Comment = logDetail.Comment,
                                CreatedBy = logDetail.CreatedBy,
                                CreatedDate = logDetail.CreatedDate
                            };
                            if (logDetail.Reasons != null)
                            {
                                displayItem.Reasons = logDetail.Reasons;
                            }
                            if (logDetail.ChangeLogs != null)
                            {
                                displayItem.ChangeLogs = new List<ChangeLogsDisplay>();
                                displayItem.ChangeLogs = logDetail.ChangeLogs.Select(x => new ChangeLogsDisplay
                                {
                                    ChangeType = x.ChangeType,
                                    FieldName = x.FieldName,
                                    FieldDescription = x.FieldDescription,
                                    BeforeValue = x.BeforeValue,
                                    BeforeValueDescription = x.BeforeValueDescription,
                                    AfterValue = x.AfterValue,
                                    AfterValueDescription = x.AfterValueDescription
                                }).ToList();

                                displayItem.DetailsUpdateList = new List<string>();
                                displayItem.DocumentsUpdateList = new List<string>();

                                for (int j = 0; j < displayItem.ChangeLogs.Count; j++)
                                {
                                    ChangeLogsDisplay log = displayItem.ChangeLogs[j];
                                    string change = string.Empty;
                                    if (log.Event == ChangeEvent.Add)
                                    {
                                        change = string.Format(_addFormat, log.FieldDescription, log.AfterValue);
                                    }
                                    else if (log.Event == ChangeEvent.Update)
                                    {
                                        change = string.Format(_updateFormat, log.FieldDescription, log.BeforeValue, log.AfterValue);
                                    }
                                    else
                                    {
                                        change = string.Format(_removeFormat, log.FieldDescription);
                                    }

                                    if (log.Type == ChangeType.Fields)
                                    {
                                        displayItem.DetailsUpdateList.Add(change);
                                    }
                                    else
                                    {
                                        displayItem.DocumentsUpdateList.Add(change);
                                    }
                                }
                            }
                            displayModel.Content.ApplicationActivityLogDetail.Add(displayItem);
                        }
                    }

                    SetPaymentDisplay(ref displayModel);
                }

                return displayModel;
            }
        }
        #region Additional Info
        private static void AddAdditionalInfo(ref ApplicationDetailDisplay displayModel
            , GetApplicationDetailsModel content
            , List<SelectorModel> additionalDisplayConfig
            , string applicationType)
        {
            if (_mappingList == null || _mappingList.Count == 0)
            {
                return;
            }
            string propertyName = _mappingList.Find(x => x.Key == applicationType).Value ?? string.Empty;
            if (!propertyName.IsValid())
            {
                return;
            }
            object props = GetObjectValue(content, propertyName);
            if (props != null)
            {
                if (GetObjectValue(props, "statusDate") is object statusDate && statusDate != null)
                {
                    displayModel.Content.ApplicationDetail.StatusDate = Convert.ToDateTime(statusDate);
                }
                if (GetObjectValue(props, "createdDate") is object createdDate && createdDate != null)
                {
                    displayModel.Content.ApplicationDetail.CreatedDate = Convert.ToDateTime(createdDate);
                }
                for (int i = 0; i < additionalDisplayConfig.Count; i++)
                {
                    SelectorModel item = additionalDisplayConfig[i];
                    object infoValue = GetObjectValue(props, item.Key);
                    if (infoValue != null && infoValue.ToString().IsValid())
                    {
                        displayModel.Content.AdditionalInfoList.Add(new TitleValueModel
                        {
                            Title = item.Description.ToUpper(),
                            Value = infoValue.ToString()
                        });
                    }
                }
            }
        }
        #endregion
        #region Evaluate NC Type
        private static void GetNCType(ref string key
            , string applicationType
            , string applicationModuleId
            , string createdByRoleID
            , bool isPremiseServiceReady)
        {
            if (applicationType != "NC" || key == KEDAI || key == SAVED) { return; }
            if (applicationModuleId == "101011" || applicationModuleId == "101010")
            {
                key = "NC_GROUPMOVEIN";
            }
            else if (applicationModuleId == "101012")
            {
                key = "NC_PREMISECREATION";
            }
            else if (applicationModuleId == "101001" && isPremiseServiceReady)
            {
                if (createdByRoleID == "16")
                {
                    key = "NC_PREMISEINDIVUDUAL";
                }
                else if (createdByRoleID == "2")
                {
                    key = "NC_PREMISECONTRACTOR";
                }
            }
            else if (createdByRoleID == "2")
            {
                key = "NC_CONTRACTOR";
            }
        }
        /*
        "NC": [],
    "NC_GROUPMOVEIN": [],       DONE
    "NC_PREMISECREATION": [],   DONE
    "NC_CONTRACTOR": [],
    "NC_PREMISEINDIVUDUAL": [], DONE
    "NC_PREMISECONTRACTOR": [], DONE
        */
        #endregion
        #region Payment Display
        private static void SetPaymentDisplay(ref ApplicationDetailDisplay displayModel)
        {
            if (displayModel.Content.applicationPaymentDetail != null)
            {
                displayModel.Content.PaymentDetailsList = new List<TitleValueModel>();
                Dictionary<string, List<SelectorModel>> selectors = LanguageManager.Instance.GetSelectorsByPage("ApplicationStatusPaymentDetails");
                if (selectors != null
                    && selectors.ContainsKey("chargesMapping")
                    && selectors["chargesMapping"] is List<SelectorModel> mappingList
                    && mappingList != null
                    && mappingList.Count > 0)
                {
                    for (int i = 0; i < mappingList.Count; i++)
                    {
                        SelectorModel item = mappingList[i];
                        if (GetObjectValue(displayModel.Content.applicationPaymentDetail.oneTimeChargesDetail, item.Key) is object value
                            && value != null)
                        {
                            if (Convert.ToDouble(value) is double convertedValue && convertedValue > 0)
                            {
                                displayModel.Content.PaymentDetailsList.Add(new TitleValueModel
                                {
                                    Title = item.Description,
                                    Value = convertedValue.ToAmountDisplayString(true)
                                });
                            }
                        }
                    }

                }
            }
        }
        #endregion

        private static object GetObjectValue(object props
            , string key)
        {
            object value = null;
            Type type = props.GetType();
            if (type == null)
            {
                return value;
            }
            PropertyInfo property = type.GetProperty(key);
            if (property == null)
            {
                return value;
            }
            object objectValue = property.GetValue(props, null);
            if (objectValue == null)
            {
                return value;
            }
            value = objectValue;
            return value;
        }
    }
}