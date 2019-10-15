using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class ServicesResponseModel
    {
        public ServicesDataModel d { set; get; }
    }

    public class ServicesDataModel : BaseModelV2
    {
        public HomeServiceModel data { set; get; }
    }

    public class HomeServiceModel
    {
        public List<ServiceItemModel> services { set; get; }
    }

    public class ServiceItemModel
    {
        public string ServiceId { set; get; }
        public string ServiceName { set; get; }
        public string ServiceIcon { set; get; }
        public string ServiceIconUrl { set; get; }
        public string ServiceDescription { set; get; }
        public string ServiceCategory { set; get; }
        [JsonIgnore]
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
                        default:
                            servicetType = ServiceEnum.None;
                            break;
                    }
                }
                return servicetType;
            }
        }
    }

    public enum ServiceEnum
    {
        None = 0,
        SELFMETERREADING,
        SUBMITFEEDBACK,
        PAYBILL,
        VIEWBILL
    }
}