using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
    public class GetBillRenderingResponse : BaseResponse<GetBillRenderingModel>
    {

    }

    public class GetBillRenderingModel
    {
        [JsonProperty("contractAccountNumber")]
        public string ContractAccountNumber { set; get; }
        [JsonProperty("digitalBillEligibility")]
        public string DigitalBillEligibility { set; get; }
        [JsonProperty("digitalBillStatus")]
        public string DigitalBillStatus { set; get; }
        [JsonProperty("ownerBillRenderingMethod")]
        public string OwnerBillRenderingMethod { set; get; }
        [JsonProperty("ownerBillingEmail")]
        public string OwnerBillingEmail { set; get; }
        [JsonProperty("bcRecord")]
        public List<BCRecordModel> BCRecordModels { set; get; }
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
                    case "ZV02":
                        {
                            renderingType = MobileEnums.DBRTypeEnum.Paper;
                            break;
                        }
                    //Email
                    case "ZV03":
                        {
                            renderingType = IsUpdateCtaAllow
                                ? MobileEnums.DBRTypeEnum.EmailWithCTA
                                : MobileEnums.DBRTypeEnum.Email;
                            break;
                        }
                    //EBill
                    case "ZV04":
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
                    case "ZV02":
                        {
                            message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_PaperBill);
                            break;
                        }
                    //Email
                    case "ZV03":
                        {
                            message = LanguageManager.Instance.GetCommonValue(I18NConstants.DBR_Email);
                            break;
                        }
                    //EBill
                    case "ZV04":
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
                    case "ZV02":
                        {
                            image = "Icon-DBR-Paper-Bill";
                            break;
                        }
                    //Email
                    case "ZV03":
                        {
                            image = "Icon-DBR-EMail";
                            break;
                        }
                    //EBill
                    case "ZV04":
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
    }

    public class BCRecordModel
    {
        [JsonProperty("bcbpNumber")]
        public string BCBPNumber { set; get; }
        [JsonProperty("bcbpFirstName")]
        public string BCBPFirstName { set; get; }
        [JsonProperty("bcbpLastName")]
        public string BCBPLastName { set; get; }
        [JsonProperty("bcRenderingMethod")]
        public string BCRenderingMethod { set; get; }
        [JsonProperty("bcBillingEmail")]
        public string BCBillingEmail { set; get; }
    }
}