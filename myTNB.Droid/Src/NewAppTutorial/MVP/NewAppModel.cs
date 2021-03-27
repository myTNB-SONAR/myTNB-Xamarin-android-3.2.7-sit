namespace myTNB_Android.Src.NewAppTutorial.MVP
{
    public enum ContentType
    {
        BottomLeft = 0,
        BottomRight = 1,
        TopLeft = 2,
        TopRight = 3
    }

    public class NewAppModel
    {
        public ContentType ContentShowPosition { get; set; }
        public string ContentTitle { get; set; }
        public string ContentMessage { get; set; }
        public int ItemCount { get; set; }
        public bool IsButtonShow { get; set; }
        public bool IsButtonUpdateShow { get; set; }
        public string DisplayMode { get; set; }
        public bool NeedHelpHide { get; set; }
    }
}
