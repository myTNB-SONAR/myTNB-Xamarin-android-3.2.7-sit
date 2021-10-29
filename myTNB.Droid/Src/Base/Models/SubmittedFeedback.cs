using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Enquiry;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Base.Models
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
                    EnquiryStatusCode.CL01 => Resource.Color.createdColorSubmit,
                    EnquiryStatusCode.CL02 => Resource.Color.inProgressColor,
                    EnquiryStatusCode.CL03 => Resource.Color.completedColor,
                    EnquiryStatusCode.CL04 => Resource.Color.completedColor,
                    EnquiryStatusCode.CL06 => Resource.Color.cancelledColor,
                    _ => Resource.Color.createdColorSubmit,
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
                    "GS01" => EnquiryGSLStatusCode.GS01,//Created***
                    "GL01" => EnquiryGSLStatusCode.GL01,//Created***
                    "GS08" => EnquiryGSLStatusCode.GS08,//Closed***
                    "GL05" => EnquiryGSLStatusCode.GL05,//Closed***
                    "GS06" => EnquiryGSLStatusCode.GS06,//Accepted***
                    "GL03" => EnquiryGSLStatusCode.GL03,//Accepted***
                    "GS07" => EnquiryGSLStatusCode.GS07,//Rejected***
                    "GL04" => EnquiryGSLStatusCode.GL04,//Rejected***
                    "GS05" => EnquiryGSLStatusCode.GS05,//Rejected***
                    "GS02" => EnquiryGSLStatusCode.GS02,//In Progress***
                    "GL02" => EnquiryGSLStatusCode.GL02,//In Progress***
                    "GS03" => EnquiryGSLStatusCode.GS03,//Pending Approval***
                    "GS04" => EnquiryGSLStatusCode.GS04,//Approved***
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
                    EnquiryGSLStatusCode.GS08 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GL01 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GL03 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GL05 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS02 => Resource.Color.inProgressColor,
                    EnquiryGSLStatusCode.GL02 => Resource.Color.inProgressColor,
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