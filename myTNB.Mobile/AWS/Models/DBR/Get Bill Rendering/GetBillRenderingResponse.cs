using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.Mobile.Extensions;
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
        [JsonProperty("isOwnerPostalBill")]
        public bool IsOwnerPostalBill { set; get; }
        [JsonProperty("ownerPostalAddress")]
        public string OwnerPostalAddress { set; get; }
        [JsonProperty("ownerPostalAddressDetail")]
        public PostalAddressModel OwnerPostalAddressDetail { set; get; }
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
                MobileEnums.DBRTypeEnum renderingType = MobileEnums.DBRTypeEnum.None;
                if (IsPaper)
                {
                    renderingType = MobileEnums.DBRTypeEnum.Paper;
                }
                else if (OwnerBillRenderingMethod == BillRenderingCodes.Owner_EMail)
                {
                    renderingType = IsUpdateCtaAllow
                        ? MobileEnums.DBRTypeEnum.EmailWithCTA
                        : MobileEnums.DBRTypeEnum.Email;
                }
                else if (OwnerBillRenderingMethod == BillRenderingCodes.Owner_EBill)
                {
                    if (BCRecord != null && BCRecord.Count > 0)
                    {
                        bool isEbill = true;
                        for (int i = 0; i < BCRecord.Count; i++)
                        {
                            if (BCRecord[i].RenderingMethod == BillRenderingCodes.BC_EMail)
                            {
                                isEbill = false;
                                break;
                            }
                        }
                        if (isEbill)
                        {
                            renderingType = IsUpdateCtaAllow
                                ? MobileEnums.DBRTypeEnum.EBillWithCTA
                                : MobileEnums.DBRTypeEnum.EBill;
                        }
                        else
                        {
                            renderingType = IsUpdateCtaAllow
                                ? MobileEnums.DBRTypeEnum.EmailWithCTA
                                : MobileEnums.DBRTypeEnum.Email;
                        }
                    }
                    else
                    {
                        renderingType = IsUpdateCtaAllow
                            ? MobileEnums.DBRTypeEnum.EBillWithCTA
                            : MobileEnums.DBRTypeEnum.EBill;
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
                string message = string.Empty;
                if (DBRType == MobileEnums.DBRTypeEnum.Paper)
                {
                    message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_PaperBill);
                }
                else if (DBRType == MobileEnums.DBRTypeEnum.Email
                    || DBRType == MobileEnums.DBRTypeEnum.EmailWithCTA)
                {
                    message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_Email);
                }
                else if (DBRType == MobileEnums.DBRTypeEnum.EBill
                    || DBRType == MobileEnums.DBRTypeEnum.EBillWithCTA)
                {
                    message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_EBill);
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
                string image = string.Empty;
                if (DBRType == MobileEnums.DBRTypeEnum.Paper)
                {
                    image = "Icon-DBR-Paper-Bill";
                }
                else if (DBRType == MobileEnums.DBRTypeEnum.Email
                    || DBRType == MobileEnums.DBRTypeEnum.EmailWithCTA)
                {
                    image = "Icon-DBR-EMail";
                }
                else if (DBRType == MobileEnums.DBRTypeEnum.EBill
                    || DBRType == MobileEnums.DBRTypeEnum.EBillWithCTA)
                {
                    image = "Icon-DBR-EBill";
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
                    if (DBRType == MobileEnums.DBRTypeEnum.Email
                        || DBRType == MobileEnums.DBRTypeEnum.EmailWithCTA)
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
                                        Name = string.Format("{0} {1}"
                                            , BCRecord[i].FirstName ?? string.Empty
                                            , BCRecord[i].LastName ?? string.Empty).Trim()
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
                    ? Domains.StartDigitalBill
                    : Domains.OptInToPaperBill;
            }
        }

        /// <summary>
        /// Origin URL
        /// </summary>
        [JsonIgnore]
        public string OriginURL
        {
            get
            {
                return BackToApp;
            }
        }

        private bool IsPaper
        {
            get
            {
                if (OwnerBillRenderingMethod == BillRenderingCodes.Owner_Paper)
                {
                    return true;
                }
                if (BCRecord != null && BCRecord.Count > 0)
                {
                    int index = BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_Paper);
                    return index > -1;
                }
                return false;
            }
        }

        /// <summary>
        /// Determines if CA is Pre or Post Conversion
        /// True - Post
        /// False - Pre
        /// </summary>
        [JsonIgnore]
        public bool IsPostConversion
        {
            get
            {
                return PreviousBillRendering != null;
            }
        }

        [JsonIgnore]
        public MobileEnums.RenderingMethodEnum CurrentRenderingMethod
        {
            get
            {
                MobileEnums.RenderingMethodEnum rMethod = MobileEnums.RenderingMethodEnum.None;
                if (OwnerBillRenderingMethod == BillRenderingCodes.Owner_EBill)
                {
                    if (BCRecord != null && BCRecord.Count > 0)
                    {
                        if (BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_Paper) > -1
                            && BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EMail) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email_Paper;
                        }
                        else if (BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_Paper) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                        }
                        else if (BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EMail) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                        }
                        else
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill;
                        }
                    }
                    else
                    {
                        rMethod = MobileEnums.RenderingMethodEnum.EBill;
                    }
                }
                else if (OwnerBillRenderingMethod == BillRenderingCodes.Owner_EMail)
                {
                    if (BCRecord != null && BCRecord.Count > 0)
                    {
                        if (BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_Paper) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email_Paper;
                        }
                        else if (BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EBill) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                        }
                        else
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                        }
                    }
                    else
                    {
                        rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                    }

                }
                else if (OwnerBillRenderingMethod == BillRenderingCodes.Owner_Paper)
                {
                    if (BCRecord != null && BCRecord.Count > 0)
                    {
                        if (BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EMail) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email_Paper;
                        }
                        else if (BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EBill) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                        }
                        else
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                        }
                    }
                    else
                    {
                        rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                    }
                }
                return rMethod;
            }
        }

        [JsonIgnore]
        public MobileEnums.RenderingMethodEnum PreviousRenderingMethod
        {
            get
            {
                MobileEnums.RenderingMethodEnum rMethod = MobileEnums.RenderingMethodEnum.None;
                if (!IsPostConversion || PreviousBillRendering == null)
                {
                    return rMethod;
                }

                if (PreviousBillRendering.OwnerBillRenderingMethod == BillRenderingCodes.Owner_EBill)
                {
                    if (PreviousBillRendering.BCRecord != null && PreviousBillRendering.BCRecord.Count > 0)
                    {
                        if (PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_Paper) > -1
                            && PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EMail) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email_Paper;
                        }
                        else if (PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_Paper) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                        }
                        else if (PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EMail) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                        }
                        else
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill;
                        }
                    }
                    else
                    {
                        rMethod = MobileEnums.RenderingMethodEnum.EBill;
                    }
                }
                else if (PreviousBillRendering.OwnerBillRenderingMethod == BillRenderingCodes.Owner_EMail)
                {
                    if (PreviousBillRendering.BCRecord != null && PreviousBillRendering.BCRecord.Count > 0)
                    {
                        if (PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_Paper) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email_Paper;
                        }
                        else if (PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EBill) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                        }
                        else
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                        }
                    }
                    else
                    {
                        rMethod = MobileEnums.RenderingMethodEnum.EBill_Email;
                    }
                }
                else if (PreviousBillRendering.OwnerBillRenderingMethod == BillRenderingCodes.Owner_Paper)
                {
                    if (PreviousBillRendering.BCRecord != null && PreviousBillRendering.BCRecord.Count > 0)
                    {
                        if (PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EMail) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Email_Paper;
                        }
                        else if (PreviousBillRendering.BCRecord.FindIndex(x => x.RenderingMethod == BillRenderingCodes.BC_EBill) > -1)
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                        }
                        else
                        {
                            rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                        }
                    }
                    else
                    {
                        rMethod = MobileEnums.RenderingMethodEnum.EBill_Paper;
                    }
                }
                return rMethod;
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
        [JsonProperty("isOwnerPostalBill")]
        public bool IsOwnerPostalBill { set; get; }
        [JsonProperty("ownerPostalAddressDetail")]
        public PostalAddressModel OwnerPostalAddressDetail { set; get; }
    }

    public class PostalAddressModel
    {
        [JsonProperty("houseNo")]
        public string HouseNo { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("district")]
        public string District { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
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