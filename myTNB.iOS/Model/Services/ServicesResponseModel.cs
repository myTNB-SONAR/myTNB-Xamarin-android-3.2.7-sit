using System.Collections.Generic;

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
    }
}