using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Request
{
    public class AttachedImageRequest
    {
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