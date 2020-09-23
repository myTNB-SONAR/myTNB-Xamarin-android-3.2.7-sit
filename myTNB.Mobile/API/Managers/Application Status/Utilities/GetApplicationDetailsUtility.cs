using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.API.Managers.ApplicationStatus.Utilities
{
    internal static class GetApplicationDetailsUtility
    {
        private static List<SelectorModel> _mappingList;
        private static string _addFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "add");
        private static string _updateFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "update");
        private static string _removeFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "remove");

        internal static ApplicationDetailDisplay Parse(this GetApplicationDetailsResponse response
            , string applicationType
            , string applicationTypeTitle
            , string applicationId)
        {
            {
                ApplicationDetailDisplay displayModel = new ApplicationDetailDisplay
                {
                    Content = new GetApplicationStatusDisplay
                    {
                        //Mark: Unused by ApplicationDetail Service
                        ApplicationDetail = new ApplicationDetailDisplayModel(),
                        ApplicationPaymentDetail = new ApplicationPaymentDisplayModel(),
                        ApplicationStatusDetail = new ApplicationStatusDetailDisplayModel(),
                        ApplicationActivityLogDetail = new List<ApplicationActivityLogDetailDisplay>()
                    },
                    StatusDetail = response.StatusDetail
                };
                if (response != null && response.Content != null)
                {
                    displayModel.Content.ApplicationType = applicationTypeTitle;
                    displayModel.Content.IsSaveMessageDisplayed = false;
                    displayModel.Content.IsFullApplicationTooltipDisplayed = true;
                    displayModel.Content.IsDeleteEnable = true;

                    //Todo: Check where to get last updated date
                    displayModel.Content.ApplicationDetail.CreatedDate = DateTime.Now;
                    displayModel.Content.ApplicationDetail.ApplicationId = applicationId;

                    Dictionary<string, List<SelectorModel>> selectors = LanguageManager.Instance.GetSelectorsByPage("ApplicationStatusDetails");
                    _mappingList = new List<SelectorModel>();
                    if (selectors != null && selectors.ContainsKey("applicationTypeMapping"))
                    {
                        _mappingList = selectors["applicationTypeMapping"];
                    }

                    List<SelectorModel> additionalDisplayConfig = new List<SelectorModel>();
                    string key = applicationType;
                    if (displayModel.Content.IsKedaiTenagaApplication)
                    {
                        key = "KEDAI";
                    }
                    if (response.Content.savedApplicationDetail != null)
                    {
                        key = "SAVED";
                    }
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
                            , key);
                    }

                    if (response.Content.ApplicationPaymentDetail != null)
                    {
                        displayModel.Content.ApplicationPaymentDetail = new ApplicationPaymentDisplayModel
                        {
                            OutstandingChargesAmount = response.Content.ApplicationPaymentDetail.OneTimeChargesAmount,
                            LatestBillAmount = response.Content.ApplicationPaymentDetail.LatestBillAmount,
                            OneTimeChargesAmount = response.Content.ApplicationPaymentDetail.OneTimeChargesAmount,
                            OneTimeChargesDetail = new OneTimeChargesDisplayModel
                            {
                                ConnectionChargesAmount = response.Content.ApplicationPaymentDetail.OneTimeChargesDetail.ConnectionChargesAmount,
                                SecurityDepositAmount = response.Content.ApplicationPaymentDetail.OneTimeChargesDetail.SecurityDepositAmount,
                                MeterFeeAmount = response.Content.ApplicationPaymentDetail.OneTimeChargesDetail.MeterFeeAmount,
                                StampDutyAmount = response.Content.ApplicationPaymentDetail.OneTimeChargesDetail.StampDutyAmount,
                                ProcessingFeeAmount = response.Content.ApplicationPaymentDetail.OneTimeChargesDetail.ProcessingFeeAmount
                            },
                            TotalPayableAmount = response.Content.ApplicationPaymentDetail.TotalPayableAmount,
                            CANo = response.Content.ApplicationPaymentDetail.CANo,
                            SDDocumentNo = response.Content.ApplicationPaymentDetail.SDDocumentNo,
                            SRNo = response.Content.ApplicationPaymentDetail.SRNo
                        };
                    }
                    if (response.Content.ApplicationStatusDetail != null)
                    {
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
                    displayModel.Content.ApplicationActivityLogDetail = new List<ApplicationActivityLogDetailDisplay>();
                    if (response.Content.ApplicationActivityLogDetail != null)
                    {
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
                                    ChangeLogsDisplay log = displayItem.ChangeLogs[i];
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
        #endregion
    }
}