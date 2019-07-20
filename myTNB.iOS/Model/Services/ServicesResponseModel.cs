using System.Collections.Generic;

namespace myTNB.Model
{
    public class ServicesResponseModel
    {
        public HomeServicesModel d { set; get; }
    }

    public class HomeServicesModel : BaseModelV2
    {
        public List<ServiceItemModel> data { set; get; }
    }

    public class ServiceItemModel
    {
        public string ServiceCategoryId { set; get; }
        public string ServiceCategoryName { set; get; }
        public string ServiceCategoryIcon { set; get; }
        public string ServiceCategoryIconUrl { set; get; }
        public string ServiceCategoryDesc { set; get; }
    }
}