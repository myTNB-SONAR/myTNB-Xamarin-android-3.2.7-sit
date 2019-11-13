using System.Collections.Generic;

namespace myTNB.Model
{
    public class NotificationTypeResponseModel
    {
        public NotificationTypeModel d { set; get; }
    }

    public class NotificationChannelResponseModel : NotificationTypeResponseModel { }

    public class NotificationTypeModel : BaseModelV2
    {
        public List<NotificationPreferenceModel> data { set; get; }
    }
}