using System;
using Castle.Core.Internal;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class BaseRequestRegister
    {
        public UserInfoNew usremail;
        public BaseRequestRegister()
        {
            usremail = new UserInfoNew();
        }

        public void SetUserName(string userName)
        {
            usremail.eid = userName;
        }

        public void SetSSPID(string sspID)
        {
            usremail.sspuid = sspID;
        }
    }

    public class UserInfoNew
    {
        public string eid, sspuid, did, ft, lang, sec_auth_k1, sec_auth_k2, ses_param1, ses_param2;

        public UserInfoNew()
        {
            eid = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().UserName : "";
            sspuid = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().UserID : "";
            did = UserSessions.GetDeviceId();
            ft = FirebaseTokenEntity.HasLatest() && FirebaseTokenEntity.GetLatest().FBToken != null ? FirebaseTokenEntity.GetLatest().FBToken : "";
            lang = LanguageUtil.GetAppLanguage().ToUpper();
            sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID;
            sec_auth_k2 = "";
            ses_param1 = "";
            ses_param2 = "";
        }

    }
}
