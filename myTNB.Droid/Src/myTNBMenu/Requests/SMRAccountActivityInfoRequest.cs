using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Requests
{
	public class SMRAccountActivityInfoRequest
    {
		[JsonProperty("contractAccount")]
		public string AccountNumber { get; set; }

		[JsonProperty("isOwnedAccount")]
		public string IsOwnedAccount { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface userInterface { get; set; }
    }
}