using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Enquiry;
using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Models
{
    public class SubmittedFeedback
    {
        [JsonProperty("ServiceReqNo")]
        public string FeedbackId { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("FeedbackMessage")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("FeedbackCategoryName")]
        public string FeedbackCategoryName { get; set; }

        [JsonProperty("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [JsonProperty("FeedbackNameInListView")]
        public string FeedbackNameInListView { get; set; }

        [JsonProperty("StatusCode")]
        public string StatusCode { get; set; }

        [JsonProperty("StatusDesc")]
        public string StatusDesc { get; set; }

        [JsonProperty("IsRead")]
        public string IsRead { get; set; }

        [JsonIgnore]
        public EnquiryStatusCode EnquiryStatusCode
        {
            get
            {
                return StatusCode switch
                {
                    "CLO1" => EnquiryStatusCode.CL01,
                    "CL02" => EnquiryStatusCode.CL02,
                    "CL03" => EnquiryStatusCode.CL03,
                    "CL04" => EnquiryStatusCode.CL04,
                    "CLO6" => EnquiryStatusCode.CL06,
                    "CLO7" => EnquiryStatusCode.CL07,
                    "CLO8" => EnquiryStatusCode.CL08,
                    "CLO9" => EnquiryStatusCode.CL09,
                    "CL10" => EnquiryStatusCode.CL10,
                    "CL11" => EnquiryStatusCode.CL11,
                    "CL12" => EnquiryStatusCode.CL12,
                    "CL13" => EnquiryStatusCode.CL13,
                    "CL14" => EnquiryStatusCode.CL14,
                    "CL15" => EnquiryStatusCode.CL15,
                    "CL16" => EnquiryStatusCode.CL16,
                    "CL17" => EnquiryStatusCode.CL17,
                    "CL18" => EnquiryStatusCode.CL18,
                    "CL19" => EnquiryStatusCode.CL19,
                    _ => EnquiryStatusCode.NONE,
                };
            }
        }

        [JsonIgnore]
        public int StatusColor
        {
            get
            {
                return EnquiryStatusCode switch
                {
                    EnquiryStatusCode.CL01 => Resource.Color.tunagrey,
                    EnquiryStatusCode.CL02 => Resource.Color.black,
                    EnquiryStatusCode.CL03 => Resource.Color.completedColor,
                    EnquiryStatusCode.CL04 => Resource.Color.completedColor,
                    EnquiryStatusCode.CL06 => Resource.Color.black,
                    EnquiryStatusCode.CL07 => Resource.Color.black,
                    EnquiryStatusCode.CL08 => Resource.Color.lightOrange,
                    EnquiryStatusCode.CL09 => Resource.Color.tunagrey,
                    EnquiryStatusCode.CL10 => Resource.Color.tunagrey,
                    EnquiryStatusCode.CL11 => Resource.Color.lightOrange,
                    EnquiryStatusCode.CL12 => Resource.Color.tunagrey,
                    EnquiryStatusCode.CL13 => Resource.Color.lightOrange,
                    EnquiryStatusCode.CL14 => Resource.Color.lightOrange,
                    EnquiryStatusCode.CL15 => Resource.Color.tunagrey,
                    EnquiryStatusCode.CL16 => Resource.Color.freshGreen,
                    EnquiryStatusCode.CL17 => Resource.Color.tunagrey,
                    EnquiryStatusCode.CL18 => Resource.Color.tunagrey,
                    EnquiryStatusCode.CL19 => Resource.Color.lightOrange,
                    _ => Resource.Color.black,
                };
            }
        }

        [JsonIgnore]
        public EnquiryGSLStatusCode GSLStatusCode
        {
            get
            {
                return StatusCode switch
                {
                    "GS01" => EnquiryGSLStatusCode.GS01,
                    "GL01" => EnquiryGSLStatusCode.GL01,
                    "GS08" => EnquiryGSLStatusCode.GS08,
                    "GL05" => EnquiryGSLStatusCode.GL05,
                    "GS06" => EnquiryGSLStatusCode.GS06,
                    "GL03" => EnquiryGSLStatusCode.GL03,
                    "GS07" => EnquiryGSLStatusCode.GS07,
                    "GL04" => EnquiryGSLStatusCode.GL04,
                    "GS05" => EnquiryGSLStatusCode.GS05,
                    "GS02" => EnquiryGSLStatusCode.GS02,
                    "GL02" => EnquiryGSLStatusCode.GL02,
                    "GS03" => EnquiryGSLStatusCode.GS03,
                    "GS04" => EnquiryGSLStatusCode.GS04,
                    _ => EnquiryGSLStatusCode.NONE,
                };
            }
        }

        [JsonIgnore]
        public int GSLStatusColor
        {
            get
            {
                return GSLStatusCode switch
                {
                    EnquiryGSLStatusCode.GS01 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS03 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS04 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS06 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GL01 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GL03 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS02 => Resource.Color.inProgressColor,
                    EnquiryGSLStatusCode.GL02 => Resource.Color.inProgressColor,
                    EnquiryGSLStatusCode.GL05 => Resource.Color.completedColor,
                    EnquiryGSLStatusCode.GS08 => Resource.Color.completedColor,
                    EnquiryGSLStatusCode.GS05 => Resource.Color.cancelledColor,
                    EnquiryGSLStatusCode.GS07 => Resource.Color.cancelledColor,
                    EnquiryGSLStatusCode.GL04 => Resource.Color.cancelledColor,
                    _ => Resource.Color.createdColorSubmit,
                };
            }
        }

        public static SubmittedFeedback Copy(SubmittedFeedbackEntity entity)
        {
            return new SubmittedFeedback()
            {
                FeedbackId = entity.Id,
                DateCreated = entity.DateCreated,
                FeedbackMessage = entity.FeedbackMessage,
                FeedbackCategoryId = entity.FeedbackCategoryId,
                FeedbackCategoryName = entity.FeedbackCategoryName,
                FeedbackNameInListView = entity.FeedbackNameInListView
            };
        }
    }
}