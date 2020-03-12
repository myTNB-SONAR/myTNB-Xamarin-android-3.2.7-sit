using System.Collections.Generic;

namespace myTNB.Model
{
    public class GetLocationsResponseModel
    {
        public GetLocationsModel d { set; get; }
    }

    public class GetLocationsModel : BaseModelV2
    {
        public List<GetLocationsDataModel> data { set; get; }
    }

    public class GetLocationsDataModel
    {
        public string Id { set; get; }
        public string LocationType { set; get; }
        public string SiteCoreItemId { set; get; }
        public string SiteCoreItemPath { set; get; }
        public string ImagePath { set; get; }

        public string Title { set; get; }
        public double Latitude { set; get; }
        public double Longitude { set; get; }
        public string Address { set; get; }
        public string City { set; get; }

        public string PinCode { set; get; }
        public string State { set; get; }
        public List<PhoneModel> Phones { set; get; }
        public string DateCreated { set; get; }
        public string DateLastUpdated { set; get; }

        public string IsDeleted { set; get; }
        public string Distance { set; get; }

        public List<ServicesModel> Services { set; get; }
        public List<OpeningHoursModel> OpeningHours { set; get; }
    }
}