using System.Collections.Generic;
using System.Linq;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.API.Managers.ApplicationStatus
{
    internal static class GetApplicationStatusUtility
    {
        private static string _addFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "add");
        private static string _updateFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "update");
        private static string _removeFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "remove");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="applicationTypeID"></param>
        /// <param name="applicationTypeTitle"></param>
        /// <param name="searchTypeTitle"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        internal static ApplicationDetailDisplay Parse(this GetApplicationStatusResponse response
            , string applicationTypeID
            , string applicationTypeTitle
            , string searchTypeTitle
            , string searchTerm)
        {
            ApplicationDetailDisplay displayModel = new ApplicationDetailDisplay
            {
                Content = new GetApplicationStatusDisplay
                {
                    ApplicationDetail = new ApplicationDetailDisplayModel(),
                    ApplicationPaymentDetail = new ApplicationPaymentDisplayModel(),
                    ApplicationStatusDetail = new ApplicationStatusDetailDisplayModel(),
                    ApplicationActivityLogDetail = new List<ApplicationActivityLogDetailDisplay>()
                },
                StatusDetail = response.StatusDetail
            };
            if (response.Content != null)
            {
                displayModel.Content.ApplicationType = applicationTypeTitle;
                displayModel.Content.ApplicationTypeID = applicationTypeID;
                //Mark: Values are hardcoded since this is a GetApplicationStatus API
                displayModel.Content.IsSaveMessageDisplayed = true;
                displayModel.Content.IsFullApplicationTooltipDisplayed = false;

                if (response.Content.ApplicationDetail != null)
                {
                    displayModel.Content.ApplicationDetail = new ApplicationDetailDisplayModel
                    {
                        ApplicationId = response.Content.ApplicationDetail.ApplicationId,
                        ReferenceNo = response.Content.ApplicationDetail.ReferenceNo,
                        ApplicationModuleId = response.Content.ApplicationDetail.ApplicationModuleId,
                        SRNo = response.Content.ApplicationDetail.SRNo,
                        SRType = response.Content.ApplicationDetail.SRType,
                        StatusID = response.Content.ApplicationDetail.StatusID,
                        StatusCode = response.Content.ApplicationDetail.StatusCode,
                        CreatedDate = response.Content.ApplicationDetail.CreatedDate
                    };
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
                        TotalPayableAmount = response.Content.ApplicationPaymentDetail.TotalPayableAmount
                    };
                }
                if (response.Content.ApplicationStatusDetail != null)
                {
                    displayModel.Content.ApplicationStatusDetail = new ApplicationStatusDetailDisplayModel
                    {
                        StatusId = response.Content.ApplicationStatusDetail.StatusId,
                        StatusCode = response.Content.ApplicationStatusDetail.StatusCode,
                        StatusDescription = response.Content.ApplicationStatusDetail.StatusDescription,
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
                                StatusMessage = x.StatusMessage,
                                ProgressDetail = x.ProgressDetail,
                                Sequence = x.Sequence
                            }).ToList();
                        if (displayModel.Content.ApplicationStatusDetail != null
                            && displayModel.Content.ApplicationStatusDetail.UserAction.IsValid()
                            && !displayModel.Content.IsPayment)
                        {
                            if (displayModel.Content.ApplicationStatusDetail.StatusTracker != null
                                && displayModel.Content.ApplicationStatusDetail.StatusTracker.Count > 0
                                && displayModel.Content.ApplicationStatusDetail.StatusTracker
                                    [displayModel.Content.ApplicationStatusDetail.StatusTracker.Count - 1].StatusMode == "active")
                            {
                                //Mark: For ASR, only 127 can be completed
                                if (displayModel.Content.ApplicationTypeID == "ASR"
                                    && displayModel.Content.ApplicationStatusDetail.StatusId != 127)
                                {
                                    displayModel.Content.ApplicationStatusDetail.IsLastStatusCompleted = false;
                                }
                                else
                                {
                                    displayModel.Content.ApplicationStatusDetail.IsLastStatusCompleted = true;
                                }
                            }
                        }
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
                            displayItem.Reasons = logDetail.Reasons.Select(x => string.Format("• {0}", x)).ToList();
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

                displayModel.Content.AdditionalInfoList.Add(new TitleValueModel
                {
                    Title = searchTypeTitle.ToUpper(),
                    Value = searchTerm
                });
                displayModel.Content.AdditionalInfoList.Add(new TitleValueModel
                {
                    Title = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "creationDate").ToUpper(),
                    Value = displayModel.Content.ApplicationDetail.CreatedDateDisplay ?? string.Empty
                });
            }
            return displayModel;
        }
    }
}