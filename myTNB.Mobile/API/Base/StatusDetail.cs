using Newtonsoft.Json;

namespace myTNB
{
    public class StatusDetail
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


        [JsonProperty("primaryCTATitle")]
        public string PrimaryCTATitle
        {
            set;
            get;
        } = LanguageManager.Instance.GetCommonValue("ok");

        [JsonProperty("secondaryCTATitle")]
        public string SecondaryCTATitle
        {
            set;
            get;
        } = string.Empty;

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

        [JsonProperty("isTimeout")]
        public bool IsTimeout
        {
            set;
            get;
        } = false;

        [JsonIgnore]
        public StatusDisplayType DisplayType
        {
            get
            {
                StatusDisplayType presentationType = StatusDisplayType.Default;
                switch (Type.ToLower())
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
                    case "popup":
                        {
                            presentationType = StatusDisplayType.Popup;
                            break;
                        }
                    case "none":
                        {
                            presentationType = StatusDisplayType.None;
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
        Popup,
        FullScreen,
        Refresh,
        None
    }
}