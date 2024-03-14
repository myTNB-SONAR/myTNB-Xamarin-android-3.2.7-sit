using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models
{
	public class GetIsSmrApplyAllowedResponse
    {

		[JsonProperty("d")]
		public GetIsSmrApplyAllowedData Data { get; set; }

		public class GetIsSmrApplyAllowedData
        {
			[JsonProperty(PropertyName = "ErrorCode")]
			public string ErrorCode { get; set; }

			[JsonProperty(PropertyName = "ErrorMessage")]
			public string ErrorMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayMessage")]
			public string DisplayMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayType")]
			public string DisplayType { get; set; }

			[JsonProperty(PropertyName = "data")]
			public List<GetIsSmrApplyAllowed> Data { get; set; }
		}

		public class GetIsSmrApplyAllowed
        {
			[JsonProperty(PropertyName = "ContractAccount")]
			public string ContractAccount { get; set; }

            [JsonProperty(PropertyName = "AllowApply")]
            public bool AllowApply { get; set; }
        }
	}
}