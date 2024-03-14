using myTNB.Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Models
{
	public class DeviceInterface
    {
		[JsonProperty("DeviceId")]
		public string DeviceId { get; set; }

		[JsonProperty("AppVersion")]
		public string AppVersion { get; set; }

		[JsonProperty("OsType")]
		public string OsType { get; set; }

		[JsonProperty("OsVersion")]
		public string OsVersion { get; set; }

		[JsonProperty("DeviceDesc")]
		public string DeviceDesc { get; set; } 

		[JsonProperty("VersionCode")]
		public string VersionCode { get; set; } 

	}
}