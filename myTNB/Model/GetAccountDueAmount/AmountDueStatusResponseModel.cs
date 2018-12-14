using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class AmountDueStatusResponseModel : BaseModel
    {
        public AmountDueStatusResponseModel()
        {
        }
        public AmountDueStatusResponseModel(BaseModel model) : this()
        {
            isError = model.isError;
            status = model.status;
            message = model.message;
            __type = model.__type;
            StatusCode = model.StatusCode;
        }

        [JsonProperty("data")]
        public List<DueAmountDataModel> AccountDues { get; set; }
    }
}
