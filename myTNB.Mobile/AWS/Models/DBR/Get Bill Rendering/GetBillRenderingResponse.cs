﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using static myTNB.Mobile.AWSConstants;
using static myTNB.Mobile.MobileConstants;

namespace myTNB.Mobile.AWS.Models
{
    public class GetBillRenderingResponse : BaseResponse<GetBillRenderingModel>
    {

    }

    public class GetBillRenderingModel
    {
        [JsonProperty("caNo")]
        public string ContractAccountNumber { set; get; }
        [JsonProperty("bpNo")]
        public string BPNo { set; get; }
        [JsonProperty("digitalBillEligibility")]
        public string DigitalBillEligibility { set; get; }
        [JsonProperty("digitalBillStatus")]
        public string DigitalBillStatus { set; get; }
        [JsonProperty("previousBillRendering")]
        public PreviousBillRenderingModel PreviousBillRendering { set; get; }
        [JsonProperty("ownerBillRenderingMethod")]
        public string OwnerBillRenderingMethod { set; get; }
        [JsonProperty("ownerBillingEmail")]
        public string OwnerBillingEmail { set; get; }
        [JsonProperty("bcRecord")]
        public List<BCRecordModel> BCRecord { set; get; }
        [JsonProperty("isInProgress")]
        public bool IsInProgress { set; get; }
        [JsonProperty("isUpdateCtaAllow")]
        public bool IsUpdateCtaAllow { set; get; }
        [JsonProperty("isUpdateCtaOptInPaperBill")]
        public bool IsUpdateCtaOptInPaperBill { set; get; }

        /// <summary>
        /// Use this in deciding which UI to show.
        /// </summary>
        [JsonIgnore]
        public MobileEnums.DBRTypeEnum DBRType
        {
            get
            {
                MobileEnums.DBRTypeEnum renderingType;
                switch (OwnerBillRenderingMethod)
                {
                    //Paper
                    case BillRenderingCodes.Owner_Paper:
                        {
                            renderingType = MobileEnums.DBRTypeEnum.Paper;
                            break;
                        }
                    //Email
                    case BillRenderingCodes.Owner_EMail:
                        {
                            renderingType = IsUpdateCtaAllow
                                ? MobileEnums.DBRTypeEnum.EmailWithCTA
                                : MobileEnums.DBRTypeEnum.Email;
                            break;
                        }
                    //EBill
                    case BillRenderingCodes.Owner_EBill:
                        {
                            renderingType = IsUpdateCtaAllow
                                ? MobileEnums.DBRTypeEnum.EBillWithCTA
                                : MobileEnums.DBRTypeEnum.EBill;
                            break;
                        }
                    default:
                        {
                            renderingType = MobileEnums.DBRTypeEnum.None;
                            break;
                        }
                }
                return renderingType;
            }
        }

        /// <summary>
        /// Use to display message in Bills and Bill Details
        /// </summary>
        [JsonIgnore]
        public string SegmentMessage
        {
            get
            {
                string message;
                switch (OwnerBillRenderingMethod)
                {
                    //Paper
                    case BillRenderingCodes.Owner_Paper:
                        {
                            message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_PaperBill);
                            break;
                        }
                    //Email
                    case BillRenderingCodes.Owner_EMail:
                        {
                            message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_Email);
                            break;
                        }
                    //EBill
                    case BillRenderingCodes.Owner_EBill:
                        {
                            message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_EBill);
                            break;
                        }
                    default:
                        {
                            message = string.Empty;
                            break;
                        }
                }
                return message;
            }
        }

        /// <summary>
        /// Use to display icon image in Bill and Bill Details
        /// Please make sure that assets are named accordingly
        /// </summary>
        [JsonIgnore]
        public string SegmentIcon
        {
            get
            {
                string image;
                switch (OwnerBillRenderingMethod)
                {
                    //Paper
                    case BillRenderingCodes.Owner_Paper:
                        {
                            image = "Icon-DBR-Paper-Bill";
                            break;
                        }
                    //Email
                    case BillRenderingCodes.Owner_EMail:
                        {
                            image = "Icon-DBR-EMail";
                            break;
                        }
                    //EBill
                    case BillRenderingCodes.Owner_EBill:
                        {
                            image = "Icon-DBR-EBill";
                            break;
                        }
                    default:
                        {
                            image = string.Empty;
                            break;
                        }
                }
                return image;
            }
        }

        [JsonIgnore]
        public List<EmailModel> EmailList
        {
            get
            {
                try
                {
                    if (OwnerBillRenderingMethod == BillRenderingCodes.Owner_EMail)
                    {
                        List<EmailModel> emails = new List<EmailModel>
                        {
                            new EmailModel
                            {
                                IsOwner = true,
                                Email = OwnerBillingEmail ?? string.Empty,
                                Name = string.Empty
                            }
                        };
                        if (BCRecord != null && BCRecord.Count > 0)
                        {
                            for (int i = 0; i < BCRecord.Count; i++)
                            {
                                if (BCRecord[i].RenderingMethod == BillRenderingCodes.BC_EMail)
                                {
                                    emails.Add(new EmailModel
                                    {
                                        IsOwner = false,
                                        Email = BCRecord[i].BillingEmail ?? string.Empty,
                                        Name = string.Format("{0} {1}", BCRecord[i].FirstName ?? string.Empty, BCRecord[i].LastName ?? string.Empty).Trim()
                                    });
                                }
                            }
                        }
                        return emails;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[DEBUG] EmailList Error: " + e.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// Use this as the Redirect URL to generate signature
        /// </summary>
        [JsonIgnore]
        public string RedirectURL
        {
            get
            {
                return DBRType == MobileEnums.DBRTypeEnum.Paper
                    ? URLs.StartDBRRedirectURL
                    : URLs.PaperBillOptInDBRRedirectURL;
            }
        }

    }

    public class BCRecordModel
    {
        [JsonProperty("bpNo")]
        public string BPNo { set; get; }
        [JsonProperty("firstName")]
        public string FirstName { set; get; }
        [JsonProperty("lastName")]
        public string LastName { set; get; }
        [JsonProperty("renderingMethod")]
        public string RenderingMethod { set; get; }
        [JsonProperty("billingEmail")]
        public string BillingEmail { set; get; }
    }

    public class PreviousBillRenderingModel
    {
        [JsonProperty("ownerBillRenderingMethod")]
        public string OwnerBillRenderingMethod { set; get; }
        [JsonProperty("ownerBillingEmail")]
        public string OwnerBillingEmail { set; get; }
        [JsonProperty("bcRecord")]
        public List<BCRecordModel> BCRecord { set; get; }
    }

    public class EmailModel
    {
        /// <summary>
        /// true for owner and name should be from myTNB Account and add the (You) Suffix
        /// </summary>
        public bool IsOwner { set; get; }
        public string Email { set; get; }
        public string Name { set; get; }
    }
}