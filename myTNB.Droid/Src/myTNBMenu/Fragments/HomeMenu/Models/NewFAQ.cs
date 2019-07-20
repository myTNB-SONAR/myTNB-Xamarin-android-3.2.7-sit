using SQLite;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models
{
    public class NewFAQ
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string Image { set; get; }

        public string BgStartColor { set; get; }

        public string BgEndColor { set; get; }

        public string BgDirection { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }

        public string TopicBodyTitle { set; get; }

        public string TopicBodyContent { set; get; }

        public string CTA { set; get; }

        public string Tag { set; get; }
    }
}