using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusListing.Models
{
    public class ApplicationStatusModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusCode")]
        public string StatusCode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("typeCode")]
        public string TypeCode { get; set; }

        [JsonProperty("actionMessage")]
        public string ActionMessage { get; set; }

        [JsonProperty("refCode")]
        public string RefCode { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("applicationNumber")]
        public string ApplicationNumber { get; set; }

        [JsonProperty("srNumber")]
        public string SrNumber { get; set; }

        [JsonProperty("isUpdated")]
        public bool IsUpdated { get; set; }

        [JsonProperty("applicationDate")]
        public ApplicationStatusDateModel ApplicationDate { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("isProgressDisplayed")]
        public bool IsProgressDisplayed { get; set; }

        [JsonProperty("progress")]
        public List<ApplicationStatusCodeModel> Progress { get; set; }
    }

    public class ApplicationStatusTypeModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("typeCode")]
        public string TypeCode { get; set; }

        public bool isChecked { get; set; }
    }

    public class ApplicationStatusStringSelectionModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public bool isChecked { get; set; }
    }

    public class ApplicationStatusDateModel
    {
        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("formattedDate")]
        public string FormattedDate { get; set; }
    }

    public class ApplicationStatusCodeModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("stateCode")]
        public string StateCode { get; set; }

        public bool isChecked { get; set; }
    }

    public class ApplicationStatusColorCodeModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("rgb")]
        public ApplicationStatusColorModel ColorList { get; set; }
    }

    public class ApplicationStatusColorModel
    {
        [JsonProperty("r")]
        public int Red { get; set; }

        [JsonProperty("g")]
        public int Green { get; set; }

        [JsonProperty("b")]
        public int Blue { get; set; }
    }
}