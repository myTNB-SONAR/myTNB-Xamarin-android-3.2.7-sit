using System.Collections.Generic;
using myTNB.Mobile.API.Base;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Home.PostServices
{
    public class PostServicesResponse : BaseResponseV2<PostServicesModel>
    {

    }

    public class PostServicesModel
    {
        [JsonProperty("timestamp")]
        public string TimeStamp { set; get; }
        [JsonProperty("services")]
        public List<ServicesModel> Services { set; get; }

        public string SavedTimeStamp { set; get; }
        public bool ShouldUpdateImages
        {
            get
            {
                return !TimeStamp.Equals(SavedTimeStamp);
            }
        }
    }

    public class ServicesModel : ServicesBaseModel
    {
        public List<ServicesBaseModel> Children { set; get; }
        public int DisplayType { set; get; }
    }

    public class ServicesBaseModel
    {
        public string ServiceId { set; get; }
        public string ServiceName { set; get; }
        public string ServiceIconUrl { set; get; }
        public string DisabledServiceIconUrl { set; get; }
        public string ServiceBannerUrl { set; get; }
        public bool Enabled { set; get; }
        public string SSODomain { set; get; }
        public string OriginURL { set; get; }
        public string RedirectURL { set; get; }

        public string ServiceBannerPreferenceKey { get { return string.Format("QuicklinksBanner{0}", ServiceId); } }
        public string ServiceIconPreferenceKey { get { return string.Format("Quicklinks{0}", ServiceId); } }
        public string DisabledServiceIconPreferenceKey { get { return string.Format("QuicklinksDisabled{0}", ServiceId); } }
    }
}