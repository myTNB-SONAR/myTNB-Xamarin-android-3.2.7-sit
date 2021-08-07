using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace myTNB.Mobile.AWS.Models
{
    public class PostInstallationDetailsResponse : BaseResponse<PostInstallationDetailsResponseModel>
    {

    }

    public class PostInstallationDetailsResponseModel
    {
        [JsonProperty("rateCategory")]
        public string RateCategory { set; get; }

        [JsonProperty("textRateCategory")]
        public string TextRateCategory { set; get; }

        [JsonProperty("meterReadingUnit")]
        public string MeterReadingUnit { set; get; }

        [JsonProperty("singleCharacterIndicator")]
        public string SingleCharacterIndicator { set; get; }

        [JsonProperty("rateFit")]
        public string RateFit { set; get; }

        [JsonProperty("rateFitCD")]
        public string RateFitCD { set; get; }

        [JsonProperty("rateInstalCAP")]
        public string RateInstalCAP { set; get; }

        [JsonProperty("rateKwhRec")]
        public string RateKwhRec { set; get; }

        [JsonProperty("disconnectionStatus")]
        public string DisconnectionStatus { set; get; }

        [JsonProperty("rateAccDaa")]
        public string RateAccDaa { set; get; }

        [JsonProperty("installationNumber")]
        public string InstallationNumber { set; get; }

        [JsonProperty("installationType")]
        public string InstallationType { set; get; }

        [JsonProperty("installationTypeDesc")]
        public string InstallationTypeDesc { set; get; }

        [JsonIgnore]
        public bool IsResidential
        {
            get
            {
                return true;
            }
        }
    }
}