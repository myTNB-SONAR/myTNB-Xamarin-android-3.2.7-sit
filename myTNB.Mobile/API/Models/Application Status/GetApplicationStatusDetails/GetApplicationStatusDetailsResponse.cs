using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class GetApplicationStatusDetailsResponse : BaseResponse<GetApplicationStatusDetailsModel>
    {

    }

    public class GetApplicationStatusDetailsModel
    {
        [JsonProperty("applicationStatus")]
        public ApplicationStatusDetails ApplicationStatus { set; get; }
    }

    public class ApplicationStatusDetails : Details
    {
        public string ActionMessage { set; get; }
        public bool IsUpdated { set; get; }
        public string Note { set; get; }
        public bool IsProgressDisplayed { set; get; }
        public List<Progress> Progress { set; get; }
        public string CreationDate { set; get; }
        public string StartDate { set; get; }
        public string TypeOfPremise { set; get; }
        public string AccountType { set; get; }
        public bool IsBinded { set; get; }
    }
}