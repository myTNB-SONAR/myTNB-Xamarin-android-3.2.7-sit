using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.FindUs.Models
{
    public class LocationData
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "LocationType")]
        public string LocationType { get; set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "ImagePath")]
        public string ImagePath { get; set; }

        [JsonProperty(PropertyName = "Latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "Longitude")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "Address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "PinCode")]
        public string PinCode { get; set; }

        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "Phones")]
        public List<Phones> PhoneList { get; set; }

        [JsonProperty(PropertyName = "DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty(PropertyName = "DateLastUpdated")]
        public string DateLastUpdated { get; set; }

        [JsonProperty(PropertyName = "Distance")]
        public string Distance { get; set; }

        [JsonProperty(PropertyName = "Services")]
        public List<Services> ServicesList { get; set; }

        [JsonProperty(PropertyName = "OpeningHours")]
        public List<OpeningHours> OpeningHourList { get; set; }
    }

    public class Services
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }


        [JsonProperty(PropertyName = "LocationId")]
        public string LocationId { get; set; }
    }

    public class OpeningHours
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "LocationId")]
        public string LocationId { get; set; }
    }

    public class Phones
    {
        [JsonProperty(PropertyName = "PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}