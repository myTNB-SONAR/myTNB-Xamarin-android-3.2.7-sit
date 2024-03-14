using System;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.Base.Request
{
    public class APIBaseRequest
    {
        public UserInfo usrInf;
        public APIBaseRequest()
        {
            usrInf = new UserInfo();
        }
    }

    public class UserInfo
    {
        public string eid, sspuid, did, ft, lang, sec_auth_k1, sec_auth_k2, ses_param1, ses_param2;

		public UserInfo()
        {
            var deviceId = UserEntity.GetActive().DeviceId;
            UserEntity user = UserEntity.GetActive();
            eid = user.UserName;
            sspuid = user.UserID;
            did = deviceId;
            ft = FirebaseTokenEntity.GetLatest().FBToken;
            lang = LanguageUtil.GetAppLanguage().ToUpper();
            sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID;
            sec_auth_k2 = "";
            ses_param1 = "";
            ses_param2 = "";
        }

    }
}
