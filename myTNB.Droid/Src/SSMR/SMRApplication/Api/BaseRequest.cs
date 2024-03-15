using System;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.SSMR.SMRApplication.Api
{
    public class BaseRequest
    {
        public UserInfo usrInf;
        public BaseRequest()
        {
            usrInf = new UserInfo();
        }
    }

    public class UserInfo
    {
        public string eid, sspuid, did, ft, lang, sec_auth_k1, sec_auth_k2, ses_param1, ses_param2;
        public UserInfo()
        {
            UserEntity user = UserEntity.GetActive();
            eid = user.UserName;
            sspuid = user.UserID;
            did = "";
            ft = FirebaseTokenEntity.GetLatest().FBToken;
            lang = LanguageUtil.GetAppLanguage().ToUpper();
            sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID;
            sec_auth_k2 = "test";
            ses_param1 = "test";
            ses_param2 = "test";
        }

    }
}
