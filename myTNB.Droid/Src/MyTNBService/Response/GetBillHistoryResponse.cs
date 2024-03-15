using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class GetBillHistoryResponse : BaseResponse<List<GetBillHistoryResponse.ResponseData>>
    {
        public List<ResponseData> GetData()
        {
            return Response.Data;
        }
        public class ResponseData
        {
            [JsonProperty("NrBill")]
            public string NrBill { get; set; }

            [JsonProperty("DtBill")]
            public string DtBill { get; set; }

            [JsonProperty("AmPayable")]
            public double AmPayable { get; set; }

            [JsonProperty("QtUnits")]
            public string QtUnits { get; set; }
        }

    }
}
