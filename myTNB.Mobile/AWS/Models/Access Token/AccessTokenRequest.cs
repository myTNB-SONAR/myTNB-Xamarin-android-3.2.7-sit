using System.Collections.Generic;

namespace myTNB.Mobile.AWS.Models
{
    public class AccessTokenRequest
    {
        public string Channel { set; get; }
        public string UserId { set; get; }
    }

    public class UserInfoModel
    {
        public string Channel { set; get; }
        public string UserId { set; get; }
        public string UserName { set; get; }
        public List<int> RoleIds { set; get; }
    }
}