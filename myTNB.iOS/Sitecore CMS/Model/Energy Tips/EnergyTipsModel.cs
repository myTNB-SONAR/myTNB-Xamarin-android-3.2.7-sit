using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class EnergyTipsTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<EnergyTipsTimeStamp> Data { set; get; }
    }

    public class EnergyTipsResponseModel
    {
        public string Status { set; get; }
        public List<TipsModel> Data { set; get; }
    }

    public class TipsModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }

        public string ID { set; get; }
    }

    public class EnergyTipsTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
