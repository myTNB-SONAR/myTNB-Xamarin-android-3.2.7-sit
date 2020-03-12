namespace myTNB.Model.Language
{
    public class LanguageResponseModel
    {
        public LanguageDataModel d { set; get; }
    }

    public class LanguageDataModel : BaseModelV2
    {
        public LanguageData data { set; get; }
    }

    public class LanguageData
    {
        public string lang { set; get; } = string.Empty;
    }
}