using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.Base.Request
{
    public class FeedbackRequest
    {
        //        {
        //    "apiKeyID": "9515F2FA-C267-42C9-8087-FABA77CB84DF",
        //    "feedbackCategoryId": "1",
        //    "feedbackTypeId":"",
        //    "accountNum": "220033747101",
        //    "name": "tester",
        //    "phoneNum": "0123333333",
        //    "email": "mytnb.sit.101@gmail.com",
        //    "deviceId": "123",
        //    "feedbackMesage": "Some Feedback message",
        //    "stateId":"",
        //    "location":"",
        //    "poleNum":"",
        //    "images": [
        //                     {"imageHex":"","fileSize": "12345","fileName": "Image 1"},
        //                     {"imageHex":"","fileSize": "12345","fileName": "Image 2"}
        //      ]   
        //}
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("feedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [JsonProperty("feedbackTypeId")]
        public string FeedbackTypeId { get; set; }

        [JsonProperty("accountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phoneNum")]
        public string PhoneNum { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("feedbackMesage")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("stateId")]
        public string StateId { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("poleNum")]
        public string PoleNum { get; set; }

        [JsonProperty("images")]
        public List<AttachedImageRequest> Images { get; set; }

    }
}