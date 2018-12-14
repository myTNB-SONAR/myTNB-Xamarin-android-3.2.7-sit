using System.Collections.Generic;

namespace myTNB.Model
{
    public class NotificationTypeModel : BaseModel
    {
        public List<NotificationPreferenceModel> data { set; get; }
    }
}