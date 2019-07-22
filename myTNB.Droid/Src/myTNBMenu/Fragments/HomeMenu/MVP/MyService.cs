using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
	public class MyService
	{
		[JsonProperty("ServiceCategoryId")]
		public string ServiceCategoryId { get; set; }

		[JsonProperty("serviceCategoryName")]
		public string serviceCategoryName { get; set; }

        [JsonProperty("serviceCategoryIcon")]
        public string serviceCategoryIcon { get; set; }

        [JsonProperty("serviceCategoryIconUrl")]
        public string serviceCategoryIconUrl { get; set; }

        [JsonProperty("serviceCategoryDesc")]
        public string serviceCategoryDesc { get; set; }

    }
}