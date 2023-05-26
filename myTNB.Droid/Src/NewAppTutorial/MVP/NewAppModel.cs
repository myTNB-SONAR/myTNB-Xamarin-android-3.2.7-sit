namespace myTNB_Android.Src.NewAppTutorial.MVP
{
    public enum ContentType
    {
        BottomLeft = 0,
        BottomRight = 1,
        TopLeft = 2,
        TopRight = 3
    }

    public enum FeatureType
    {
        None = 0,
        Accounts = 1,
        AccountsNC = 2,
        QuickAccess = 3,
        QuickActions = 4,
        MyHome = 5,
        NeedHelp = 6
    }

    public class NewAppModel
    {
        public FeatureType Feature { get; set; }
        public ContentType ContentShowPosition { get; set; }
        public string ContentTitle { get; set; }
        public string ContentMessage { get; set; }
        public int ItemCount { get; set; }
        public bool IsButtonShow { get; set; }
        public bool IsButtonUpdateShow { get; set; }
        public string DisplayMode { get; set; }
        public bool NeedHelpHide { get; set; }
        public string DynatraceVisitTag { set; get; }
        public string DynatraceActionTag { set; get; }
        public string Tag { set; get; }
    }
}
