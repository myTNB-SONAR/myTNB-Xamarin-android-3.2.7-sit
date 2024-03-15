namespace myTNB.AndroidApp.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string Background { set; get; }
        public string DynatraceVisitTag { set; get; }
        public string DynatraceActionTag { set; get; }
        public NewWalkthroughType Type { set; get; } = NewWalkthroughType.None;
    }

    public enum NewWalkthroughType
    {
        FontSize,
        None
    }
}