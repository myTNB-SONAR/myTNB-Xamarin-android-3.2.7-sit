using Newtonsoft.Json;

namespace myTNB
{
    public class ServiceStatus
    {
        [JsonProperty("code")]
        public string Code
        {
            set;
            get;
        } = "default";

        [JsonProperty("title")]
        public string Title
        {
            set;
            get;
        } = LanguageManager.Instance.GetErrorValue("defaultErrorTitle");

        [JsonProperty("message")]
        public string Message
        {
            set;
            get;
        } = LanguageManager.Instance.GetErrorValue("defaultErrorMessage");


        [JsonProperty("ctaTitle")]
        public string CTATitle
        {
            set;
            get;
        } = LanguageManager.Instance.GetCommonValue("ok");

        [JsonProperty("isSuccess")]
        public bool IsSuccess
        {
            set;
            get;
        } = false;

        [JsonProperty("type")]
        public string Type
        {
            set;
            get;
        } = string.Empty;

        [JsonIgnore]
        public StatusDisplayType DisplayType
        {
            get
            {
                StatusDisplayType presentationType = StatusDisplayType.Default;
                switch (Type)
                {
                    case "toast":
                        {
                            presentationType = StatusDisplayType.Toast;
                            break;
                        }
                    case "refresh":
                        {
                            presentationType = StatusDisplayType.Refresh;
                            break;
                        }
                    case "fullscreen":
                        {
                            presentationType = StatusDisplayType.FullScreen;
                            break;
                        }
                    case "default":
                    case "":
                    case null:
                    default:
                        {
                            break;
                        }
                }
                return presentationType;
            }
        }
    }

    public enum StatusDisplayType
    {
        // MARK: Android = SnackBar iOS = Popup
        Default,
        // MARK: Android = SnackBar iOS = Toast
        Toast,
        FullScreen,
        Refresh
    }
}