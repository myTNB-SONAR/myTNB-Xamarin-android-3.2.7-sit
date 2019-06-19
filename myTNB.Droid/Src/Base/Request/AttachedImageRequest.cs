using Newtonsoft.Json;

namespace myTNB_Android.Src.Base.Request
{
    public class AttachedImageRequest
    {
        //{"imageHex":"","fileSize": "12345","fileName": "Image 1"},

        [JsonProperty("imageHex")]
        public string ImageHex { get; set; }

        [JsonProperty("fileSize")]
        public int FileSize { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}