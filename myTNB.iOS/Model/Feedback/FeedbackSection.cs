using System.Collections.Generic;

namespace myTNB.Model.Feedback
{
    public class FeedbackSection
    {
        public string SectionTitle { set; get; }
        public List<string> Contents { set; get; }
        public List<string> SubContents { set; get; }
        public List<string> Icons { set; get; }
    }
}