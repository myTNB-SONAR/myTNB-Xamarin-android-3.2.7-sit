using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Base.Models
{
    public abstract class BaseResponse<T>
    {
        [JsonProperty(PropertyName = "d")]
        public List<T> response { get; set; }
    }
}