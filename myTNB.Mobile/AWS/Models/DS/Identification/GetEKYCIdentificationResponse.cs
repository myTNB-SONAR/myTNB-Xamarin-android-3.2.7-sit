using System.Collections.Generic;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DS.Identification
{
    public class GetEKYCIdentificationResponse : BaseResponse<GetEKYCIdentificationModel>
    {

    }

    public class GetEKYCIdentificationModel
    {
        [JsonProperty("userID")]
        public string UserID { set; get; }
        [JsonProperty("identificationType")]
        public int? IdentificationType { set; get; } = null;
        [JsonProperty("identificationNo")]
        public string IdentificationNo { set; get; }

        [JsonIgnore]
        public string Status { set; get; }

        [JsonIgnore]
        public bool IsCompletedOnOtherDevice
        {
            get
            {
                return Status.IsValid()
                    && (Status.ToUpper() == "VERIFIED" || Status.ToUpper() == "PENDING");
            }
        }

        [JsonIgnore]
        public string IDTypeName
        {
            get
            {
                Dictionary<string, List<SelectorModel>> dsLandingDictionary = LanguageManager.Instance.GetSelectorsByPage("DSLanding");
                if (dsLandingDictionary != null
                    && dsLandingDictionary.Count > 0
                    && dsLandingDictionary.ContainsKey("idType")
                    && dsLandingDictionary["idType"] is List<SelectorModel> idTypeList
                    && idTypeList != null
                    && idTypeList.Count > 0
                    && IdentificationType != null
                    && IdentificationType.Value.ToString() is string idTypeString
                    && idTypeString.IsValid()
                    && idTypeList.FindIndex(x => x.Key == idTypeString) is int idTypeIndex
                    && idTypeIndex > -1)
                {
                    return idTypeList[idTypeIndex].Description;
                }
                return string.Empty;
            }
        }
    }
}