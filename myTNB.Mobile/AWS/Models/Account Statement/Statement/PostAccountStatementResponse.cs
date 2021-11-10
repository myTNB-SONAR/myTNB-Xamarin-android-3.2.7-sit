using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.AccountStatement
{
    public class PostAccountStatementResponse : BaseResponse<PostAccountStatementResponseModel>
    {

    }

    public class PostAccountStatementResponseModel
    {
        [JsonProperty("referenceNo")]
        public string ReferenceNo { set; get; }
        [JsonProperty("caNo")]
        public string CANo { set; get; }
        [JsonProperty("accountStatement")]
        public byte[] AccountStatement { set; get; }
        [JsonProperty("statementPeriod")]
        public string StatementPeriod { set; get; }
        [JsonProperty("isOwnedAccount")]
        public bool IsOwnedAccount { set; get; }
        [JsonProperty("language")]
        public string Language { set; get; }
    }
}