using SQLite;

namespace myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class NewFAQ
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string Image { set; get; }

        public string BGStartColor { set; get; }

        public string BGEndColor { set; get; }

        public string BGDirection { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }

        public string TopicBodyTitle { set; get; }

        public string TopicBodyContent { set; get; }

        public string CTA { set; get; }

        public string Tags { set; get; }

        public string TargetItem { set; get; }
    }
}