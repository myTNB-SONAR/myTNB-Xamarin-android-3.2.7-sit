using Newtonsoft.Json;

namespace myTNB.Model
{
    public class AppVersionModel
    {
        public string Platform { get; set; }
        public string Version { get; set; }

        [JsonIgnore]
        public bool IsIos
        {
            get
            {
                if(!string.IsNullOrEmpty(Platform))
                {
                    if(string.Compare(Platform, TNBGlobal.DEVICE_PLATFORM_IOS) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
