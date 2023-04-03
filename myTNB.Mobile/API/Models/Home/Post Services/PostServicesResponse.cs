using System.Collections.Generic;
using myTNB.Mobile.API.Base;
using Newtonsoft.Json;
using static myTNB.Mobile.MobileEnums;

namespace myTNB.Mobile.API.Models.Home.PostServices
{
    public class PostServicesResponse : BaseResponseV2<PostServicesModel>
    {

    }

    public class PostServicesModel
    {
        [JsonProperty("timestamp")]
        public string TimeStamp { set; get; } = string.Empty;
        [JsonProperty("services")]
        public List<ServicesModel> Services { set; get; }

        public string SavedTimeStamp { set; get; } = string.Empty;
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
        public string AppVersion { set; get; }

        public string ServiceBannerPreferenceKey { get { return string.Format("QuicklinksBanner{0}", ServiceId); } }
        public string ServiceIconPreferenceKey { get { return string.Format("Quicklinks{0}", ServiceId); } }
        public string DisabledServiceIconPreferenceKey { get { return string.Format("QuicklinksDisabled{0}", ServiceId); } }

        public ServiceEnum ServiceType
        {
            get
            {
                ServiceEnum servicetType = default(ServiceEnum);

                if (!string.IsNullOrEmpty(ServiceId))
                {
                    switch (ServiceId)
                    {
                        case "1001":
                            servicetType = ServiceEnum.SELFMETERREADING;
                            break;
                        case "1003":
                            servicetType = ServiceEnum.SUBMITFEEDBACK;
                            break;
                        case "1004":
                            servicetType = ServiceEnum.PAYBILL;
                            break;
                        case "1005":
                            servicetType = ServiceEnum.VIEWBILL;
                            break;
                        case "1006":
                            servicetType = ServiceEnum.APPLICATIONSTATUS;
                            break;
                        case "1007":
                            servicetType = ServiceEnum.ENERGYBUDGET;
                            break;
                        case "1008":
                            servicetType = ServiceEnum.MYHOME;
                            break;
                        case "1009":
                            servicetType = ServiceEnum.CONNECTMYPREMISE;
                            break;
                        case "1010":
                            servicetType = ServiceEnum.HMO;
                            break;
                        default:
                            servicetType = ServiceEnum.None;
                            break;
                    }
                }
                return servicetType;
            }
        }
    }
}