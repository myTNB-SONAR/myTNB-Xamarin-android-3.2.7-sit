using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.FindUs.Response
{
    public class GetLocationTypesResponse
    {
        [JsonProperty(PropertyName = "d")]
        public LocationTypesResponse D { get; set; }

        public class LocationTypesResponse
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<LocationType> LocationTypes { get; set; }
        }


        public class LocationType
        {

            [JsonProperty(PropertyName = "Id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "Title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "Description")]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "ImagePath")]
            public string ImagePath { get; set; }
        }
    }
}