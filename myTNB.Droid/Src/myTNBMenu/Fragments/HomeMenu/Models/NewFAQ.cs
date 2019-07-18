using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models
{
	public class NewFAQ
	{
		[JsonProperty("Id")]
		public string Id { get; set; }

		[JsonProperty("NewFAQString")]
		public string NewFAQString { get; set; }

	}
}