using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
	public class MyService
	{
		[JsonProperty("ServiceId")]
		public string ServiceCategoryId { get; set; }

		[JsonProperty("ServiceName")]
		public string serviceCategoryName { get; set; }

        [JsonProperty("ServiceIcon")]
        public string serviceCategoryIcon { get; set; }

        [JsonProperty("ServiceIconUrl")]
        public string serviceCategoryIconUrl { get; set; }

        [JsonProperty("ServiceDescription")]
        public string serviceCategoryDesc { get; set; }

    }
}