using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.AppLaunch.PostServices
{
    public class PostServicesResponse : BaseResponse<PostServicesModel>
    {

    }

    public class PostServicesModel
    {
        [JsonProperty("timeStamp")]
        public string TimeStamp { set; get; } = string.Empty;
        [JsonProperty("services")]
        public List<ServiceModel> Services { set; get; }
    }

    public class ServiceModel : ServicesBaseModel
    {
        public int DisplayType { set; get; }
        public List<ChildrenServiceModel> Children { set; get; }
    }

    public class ChildrenServiceModel : ServicesBaseModel
    {
       
    }

    public class ServicesBaseModel
    {
        public string ServiceId { set; get; } = string.Empty;
        public string ServiceName { set; get; } = string.Empty;
        public string ServiceIconUrl { set; get; } = string.Empty;
        public string DisabledServiceIconUrl { set; get; } = string.Empty;
        public string ServiceBannerUrl { set; get; } = string.Empty;
        public int OrderId { set; get; }
        public bool Enabled { set; get; }
        public string SSODomain { set; get; } = string.Empty;
        public string OriginURL { set; get; } = string.Empty;
        public string RedirectURL { set; get; } = string.Empty;
    }
}