using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.Extensions;
using System.Diagnostics;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using myTNB.Mobile.SessionCache;

namespace myTNB.Mobile.API.Managers.ApplicationStatus.Utilities
{
    internal static class GetApplicationDetailsUtility
    {
        private static List<SelectorModel> _mappingList;
        private static readonly string _addFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "add");
        private static readonly string _updateFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "update");
        private static readonly string _removeFormat = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusActivityLog", "remove");
        private const string KEDAI = "KEDAI";
        private const string SAVED = "SAVED";

        internal static ApplicationDetailDisplay Parse(this GetApplicationDetailsResponse response
            , string searchApplicationType
            , string applicationID
            , string system
            , string savedApplicationID
            , bool isSavedApplication)
        {
            {
                string applicationModuleDescription = SearchApplicationTypeCache.Instance.GetApplicationTypeDescription(searchApplicationType);
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
                displayModel.EvaluateStatus();
                if (response != null && response.Content != null)
                {
                    displayModel.Content.ApplicationTypeReference = applicationModuleDescription;
                    displayModel.Content.IsSaveMessageDisplayed = false;
                    displayModel.Content.IsFullApplicationTooltipDisplayed = true;
                    displayModel.Content.IsSavedApplication = isSavedApplication;

                    displayModel.Content.ApplicationDetail.ApplicationId = applicationID;
                    displayModel.Content.System = system ?? string.Empty;
                    displayModel.Content.SavedApplicationID = savedApplicationID ?? string.Empty;
                    displayModel.Content.ApplicationTypeCode = searchApplicationType ?? string.Empty;

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
                            srNo = response.Content.applicationPaymentDetail.srNo,
                            hasInvoiceAttachment = response.Content.applicationPaymentDetail.hasInvoiceAttachment
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
                            , response.Content
                            , out bool isPremiseServiceReady);
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
                                , key.Contains("NC") ? searchApplicationType : key
                                , isPremiseServiceReady);

                            if (_mappingList != null && _mappingList.Count > 0)
                            {
                                string propertyName = _mappingList.Find(x =>
                                    x.Key == (key.Contains("NC") ? searchApplicationType : key))?.Value ?? string.Empty;
                                object props = GetObjectValue(response.Content, propertyName);
                                if (props != null)
                                {
                                    if (GetObjectValue(props, "srNo") is string srNo && srNo.IsValid())
                                    {
                                        displayModel.Content.SRNumber = srNo;
                                    }
                                    if (GetObjectValue(props, "srType") is string srType && srType.IsValid())
                                    {
                                        displayModel.Content.SRType = srType;
                                    }
                                    if (GetObjectValue(props, "snNo") is string snNo && snNo.IsValid())
                                    {
                                        displayModel.Content.SNNumber = snNo;
                                    }
                                    if (GetObjectValue(props, "isOwnApplication") is bool isOwnApplication)
                                    {
                                        displayModel.Content.IsOwnApplication = isOwnApplication;
                                    }
                                    if (GetObjectValue(props, "contractAccountNo") is string contractAccountNo && contractAccountNo.IsValid())
                                    {
                                        displayModel.Content.ContractAccountNo = contractAccountNo;
                                    }
                                    if (GetObjectValue(props, "businessArea") is string businessArea && businessArea.IsValid())
                                    {
                                        displayModel.Content.CABusinessArea = businessArea;
                                    }
                                }
                            }
                        }

                        displayModel.Content.ApplicationStatusDetail = new ApplicationStatusDetailDisplayModel
                        {
                            StatusId = response.Content.ApplicationStatusDetail.StatusId,
                            StatusCode = response.Content.ApplicationStatusDetail.StatusCode,
                            StatusDescription = response.Content.ApplicationStatusDetail.StatusDescription,
                            StatusMessage = response.Content.ApplicationStatusDetail.StatusMessage,
                            UserAction = response.Content.ApplicationStatusDetail.UserAction,
                            IsPostPayment = response.Content.ApplicationStatusDetail.IsPostPayment,
                            StatusDescriptionColor = response.Content.ApplicationStatusDetail.StatusDescriptionColor
                        };
                        if (response.Content.ApplicationStatusDetail.StatusTracker != null)
                        {
                            displayModel.Content.ApplicationStatusDetail.StatusTracker = new List<StatusTrackerDisplay>();
                            displayModel.Content.ApplicationStatusDetail.StatusTracker = response.Content.ApplicationStatusDetail
                                .StatusTracker.Select(x => new StatusTrackerDisplay
                                {
                                    StatusDescription = x.StatusDescription,
                                    StatusMode = x.StatusMode,
                                    Sequence = x.Sequence,
                                    StatusDate = x.StatusDate
                                }).ToList();
                        }
                        if (displayModel.Content.ApplicationStatusDetail != null)
                        {
                            displayModel.Content.ApplicationStatusDetail.IsPayment = displayModel.Content.IsPayment;
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
                    if (response.Content.ApplicationRatingDetail != null)
                    {
                        displayModel.Content.ApplicationRatingDetail = response.Content.ApplicationRatingDetail;
                    }
                    if (response.Content.ApplicationAppointmentDetail != null)
                    {
                        displayModel.Content.ApplicationAppointmentDetail = response.Content.ApplicationAppointmentDetail;
                    }
                    SetPaymentDisplay(ref displayModel);
                    displayModel.Content.IsDeleteEnable = isSavedApplication && !displayModel.Content.IsOwnApplication;
                }

                return displayModel;
            }
        }
        #region Additional Info
        private static void AddAdditionalInfo(ref ApplicationDetailDisplay displayModel
            , GetApplicationDetailsModel content
            , List<SelectorModel> additionalDisplayConfig
            , string applicationType
            , bool isPremiseServiceReady)
        {
            try
            {
                if (_mappingList == null || _mappingList.Count == 0)
                {
                    return;
                }
                string propertyName = _mappingList.Find(x => x.Key == applicationType)?.Value ?? string.Empty;
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
                    if (GetObjectValue(props, "businessArea") is string businessArea && businessArea.IsValid())
                    {
                        displayModel.Content.BusinessArea = businessArea;
                    }
                    if (GetObjectValue(props, "premiseAddress") is string premiseAddress && premiseAddress.IsValid())
                    {
                        displayModel.Content.PremisesAddress = premiseAddress;
                    }
                    if (GetObjectValue(props, "signApplicationURL") is string signApplicationURL && signApplicationURL.IsValid())
                    {
                        displayModel.Content.ApplicationDetail.SignApplicationURL = signApplicationURL;
                    }
                    if (GetObjectValue(props, "isVerifyNow") is bool isVerifyNow)
                    {
                        displayModel.Content.ApplicationDetail.IsVerifyNow = isVerifyNow;
                    }
                    if (GetObjectValue(props, "isContractorApplied") is bool isContractorApplied)
                    {
                        displayModel.Content.ApplicationDetail.IsContractorApplied = isContractorApplied;
                    }
                    if (!displayModel.Content.ApplicationDetail.ReferenceNo.IsValid()
                        && GetObjectValue(props, "referenceNo") is string referenceNo && referenceNo.IsValid())
                    {
                        displayModel.Content.ApplicationDetail.ReferenceNo = referenceNo;
                    }
                    bool shouldShowLinkedWith = false;

                    for (int i = 0; i < additionalDisplayConfig.Count; i++)
                    {
                        SelectorModel item = additionalDisplayConfig[i];
                        object infoValue = GetObjectValue(props, item.Key);
                        if (infoValue != null
                            && infoValue.ToString().IsValid())
                        {
                            displayModel.Content.AdditionalInfoList.Add(new TitleValueModel
                            {
                                Title = item.Description.ToUpper(),
                                Value = infoValue.ToString()
                            });
                        }
                        else if (item.Key == "shouldShowLinkedWith"
                            && bool.Parse(item.Description) is bool shouldShowLinkedWithValue
                            && shouldShowLinkedWithValue)
                        {
                            shouldShowLinkedWith = shouldShowLinkedWithValue;
                        }
                        else if (infoValue == null && item.Key == "referenceNo")
                        {
                            if (GetObjectValue(props, "backendApplicationType") is string backendApplicationtype && backendApplicationtype.IsValid()
                                && GetObjectValue(props, "backendReferenceNo") is string backendReferenceNo && backendReferenceNo.IsValid())
                            {
                                if (backendApplicationtype.ToUpper() == "SR")
                                {
                                    displayModel.Content.AdditionalInfoList.Add(new TitleValueModel
                                    {
                                        Title = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "sr").ToUpper(),
                                        Value = backendReferenceNo
                                    });
                                }
                                else if (backendApplicationtype.ToUpper() == "SN")
                                {
                                    displayModel.Content.AdditionalInfoList.Add(new TitleValueModel
                                    {
                                        Title = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "sn").ToUpper(),
                                        Value = backendReferenceNo
                                    });
                                }
                            }
                        }
                    }

                    if (shouldShowLinkedWith)
                    {
                        if (applicationType == "ASR"
                            && GetObjectValue(props, "linkedApplicationId") is string linkedApplicationId && linkedApplicationId.IsValid()
                            && GetObjectValue(props, "linkedApplicationNo") is string linkedApplicationNo && linkedApplicationNo.IsValid()
                            && GetObjectValue(props, "linkedApplicationType") is string linkedApplicationType && linkedApplicationType.IsValid())
                        {
                            displayModel.Content.LinkedWithDisplay = new LinkedWithDisplay
                            {
                                ID = linkedApplicationId,
                                ReferenceNo = linkedApplicationNo,
                                Type = linkedApplicationType,
                                IsPremiseServiceReady = isPremiseServiceReady
                            };
                        }
                        else if (GetObjectValue(props, "linkedAsrId") is string linkedAsrId && linkedAsrId.IsValid()
                            && GetObjectValue(props, "linkedAsrReferenceNo") is string linkedAsrReferenceNo && linkedAsrReferenceNo.IsValid())
                        {
                            displayModel.Content.LinkedWithDisplay = new LinkedWithDisplay
                            {
                                ID = linkedAsrId,
                                ReferenceNo = linkedAsrReferenceNo,
                                Type = "ASR",
                                IsPremiseServiceReady = isPremiseServiceReady
                            };
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] AddAdditionalInfo Error: " + e.Message);
            }
        }
        #endregion
        #region Evaluate NC Type
        private static void GetNCType(ref string key
            , string applicationType
            , GetApplicationDetailsModel content
            , out bool isPremiseServiceReady)
        {
            try
            {
                isPremiseServiceReady = false;
                if (applicationType != "NC" || key == KEDAI || key == SAVED)
                {
                    isPremiseServiceReady = false;
                    return;
                }
                string createdByRoleID = content.newConnectionDetail.createdByRoleId;
                string applicationModuleId = content.newConnectionDetail.applicationModuleId;
                isPremiseServiceReady = content.newConnectionDetail.isPremiseServiceReady == null
                    ? false
                    : content.newConnectionDetail.isPremiseServiceReady.Value;

                if (applicationModuleId == "101011" || applicationModuleId == "101010")
                {
                    key = "NC_GROUPMOVEIN";
                }
                else if (applicationModuleId == "101012")
                {
                    key = "NC_PREMISECREATION";
                }
                else if (applicationModuleId == "101001")// && isPremiseServiceReady)
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
            catch (Exception e)
            {
                isPremiseServiceReady = false;
                Debug.WriteLine("[DEBUG] GetNCType Error: " + e.Message);
            }
        }
        #endregion
        #region Payment Display
        private static void SetPaymentDisplay(ref ApplicationDetailDisplay displayModel)
        {
            try
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
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] SetPaymentDisplay Error: " + e.Message);
            }
        }
        #endregion

        private static object GetObjectValue(object props
            , string key)
        {
            object value = null;
            try
            {
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
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetObjectValue App Details Error: " + e.Message);
            }
            return value;
        }

        internal static void ParseDisplayModel(this ApplicationDetailDisplay detail
            , PostApplicationsPaidDetailsResponse paymentResponse)
        {
            try
            {
                if (paymentResponse != null && paymentResponse.D != null && paymentResponse.D.IsError == "false")
                {
                    if (paymentResponse.D.Data != null && paymentResponse.D.Data.Count > 0)
                    {
                        detail.Content.ReceiptDisplay = paymentResponse.D.Data.Select(x => new ReceiptDisplay
                        {
                            SRNumber = x.SRNumber,
                            MerchantTransID = x.MerchantTransID,
                            PaymentDoneDate = x.PaymentDoneDate,
                            Amount = x.Amount,
                            AccNumber = x.AccNumber,
                            IsPaymentSuccess = x.IsPaymentSuccess,
                            AccountPayments = x.AccountPayments
                        }).ToList();
                    }

                    detail.Content.IsPaymentAllowed = paymentResponse.D.AllowApplicationPayment;
                    detail.Content.IsPaymentEnabled = !paymentResponse.D.ApplicationPaymentDisabled;
                    detail.Content.IsPaymentAvailable = !paymentResponse.D.ApplicationPaymentUnavailable;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][ParseDisplayModel Payment Receipt]General Exception: " + ex.Message);
#endif
            }

            try
            {
                if (detail.Content.IsTaxInvoiceDisplayed)
                {
                    if (paymentResponse != null && paymentResponse.D != null && paymentResponse.D.IsError == "false"
                        && paymentResponse.D.Data != null && paymentResponse.D.Data.Count > 0)
                    {
                        detail.Content.TaxInvoiceDisplay = new TaxInvoiceDisplay();
                        for (int i = 0; i < paymentResponse.D.Data.Count; i++)
                        {
                            PostApplicationsPaidDetailsDataModel paymentDetail = paymentResponse.D.Data[i];
                            List<AccountPaymentsModel> accountPayments = paymentDetail.AccountPayments;
                            if (accountPayments != null && accountPayments.Count > 0)
                            {
                                for (int j = 0; j < accountPayments.Count; j++)
                                {
                                    AccountPaymentsModel accountPayment = accountPayments[j];
                                    if (accountPayment.PaymentType == "CONNECTIONCHARGES")
                                    {
                                        detail.Content.TaxInvoiceDisplay = new TaxInvoiceDisplay
                                        {
                                            SRNumber = paymentDetail.SRNumber,
                                            Amount = accountPayment.PaymentAmount
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][ParseDisplayModel Tax Invoice]General Exception: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// This determines if the Status of the BCRM is Online or Offline
        /// 7001 Status code for Offline
        /// </summary>
        /// <param name="displayModel"></param>
        private static void EvaluateStatus(this ApplicationDetailDisplay displayModel)
        {
            try
            {
                if (displayModel.StatusDetail != null
                    && displayModel.StatusDetail.Code.IsValid()
                    && displayModel.StatusDetail.Code == "7001")
                {
                    displayModel.Content.IsOffLine = true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] EvaluateStatus App Details Error: " + e.Message);
            }
        }
    }
}