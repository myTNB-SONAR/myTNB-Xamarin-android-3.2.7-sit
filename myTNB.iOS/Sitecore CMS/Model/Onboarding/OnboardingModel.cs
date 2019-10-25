using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class OnboardingResponseModel
    {
        public List<OnboardingItemModel> Data { set; get; }
    }

    public class OnboardingItemModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string ID { set; get; }
    }
}
