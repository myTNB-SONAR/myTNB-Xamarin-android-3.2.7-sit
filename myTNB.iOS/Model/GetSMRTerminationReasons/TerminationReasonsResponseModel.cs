using System.Collections.Generic;

namespace myTNB.Model
{
    public class TerminationReasonsResponseModel
    {
        public TerminationReasonsDataModel d { set; get; }
    }

    public class TerminationReasonsDataModel : BaseModelV2
    {
        public TerminationReasonseModel data { set; get; }
    }

    public class TerminationReasonseModel
    {
        public List<TerminationReasonsItemModel> reasons { set; get; }
    }

    public class TerminationReasonsItemModel
    {
        public string ReasonId { set; get; }
        public string ReasonName { set; get; }
        public string ReasonIcon { set; get; }
        public string ReasonCTA { set; get; }
        public string ReasonDescription { set; get; }
        public string OrderId { set; get; }
    }
}