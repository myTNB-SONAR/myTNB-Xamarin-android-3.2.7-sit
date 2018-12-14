using System.Collections.Generic;

namespace myTNB.Model
{
    public class UserNotificationModel : BaseModel
    {
        public List<UserNotificationDataModel> data { set; get; }
    }
}