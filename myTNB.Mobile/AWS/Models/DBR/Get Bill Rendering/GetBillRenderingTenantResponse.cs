using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using static myTNB.Mobile.AWSConstants;
using static myTNB.Mobile.MobileConstants;

namespace myTNB.Mobile.AWS.Models
{
    public class GetBillRenderingTenantResponse : BaseResponse<List<GetBillRenderingTenantModel>>
    {
       

    }

    public class GetBillRenderingTenantModel
    {
        [JsonProperty("CaNo")]
        public string CaNo { get; set; }
        [JsonProperty("IsOwnerOverRule")]
        public bool IsOwnerOverRule { get; set; }
        [JsonProperty("IsOwnerAlreadyOptIn")]
        public bool IsOwnerAlreadyOptIn { get; set; }
        [JsonProperty("IsTenantAlreadyOptIn")]
        public bool IsTenantAlreadyOptIn { get; set; }

    }
}