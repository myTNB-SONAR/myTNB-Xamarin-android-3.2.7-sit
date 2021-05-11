using System;
using Castle.Core.Internal;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class BaseRequest
    {
        public UserInfo usrInf;
        public BaseRequest()
        {
            usrInf = new UserInfo();
        }

        public void SetUserName(string userName)
        {
            usrInf.eid = userName;
        }

        public void SetSSPID(string sspID)
        {
            usrInf.sspuid = sspID;
        }
    }

    public class BaseRequestV2
    {
        public UserInfoV2 usrInf;
        public BaseRequestV2()
        {
            usrInf = new UserInfoV2();
        }

        public void SetIsWhiteList(bool whitelist)
        {
            usrInf.IsWhiteList = whitelist;
        }

        //public void SetSSPID(string sspID)
        //{
        //    usrInf.sspuid = sspID;
        //}
    }

    public class UserInfo
    {
        public string eid, sspuid, did, ft, lang, sec_auth_k1, sec_auth_k2, ses_param1, ses_param2;

        public UserInfo()
        {
            eid = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().UserName : "";
            sspuid = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().UserID : "";
            did = UserSessions.GetDeviceId();
             ft = FirebaseTokenEntity.HasLatest() && FirebaseTokenEntity.GetLatest() != null
                ? FirebaseTokenEntity.GetLatest().FBToken ?? string.Empty //Todo: Shouldn't return empty
                : string.Empty;
            lang = LanguageUtil.GetAppLanguage().ToUpper();
            sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID;
            sec_auth_k2 = "";
            ses_param1 = "";
            ses_param2 = "";
        }

    }

    public class UserInfoV2
    {
        public string eid, sspuid, did, ft, lang, sec_auth_k1, sec_auth_k2, ses_param1, ses_param2;
        public bool IsWhiteList;

        public UserInfoV2()
        {
            eid = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().UserName : "";
            sspuid = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().UserID : "";
            did = UserSessions.GetDeviceId();
            ft = FirebaseTokenEntity.HasLatest() && FirebaseTokenEntity.GetLatest() != null
               ? FirebaseTokenEntity.GetLatest().FBToken ?? string.Empty //Todo: Shouldn't return empty
               : string.Empty;
            lang = LanguageUtil.GetAppLanguage().ToUpper();
            sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID;
            sec_auth_k2 = "";
            ses_param1 = "";
            ses_param2 = "";
            IsWhiteList = false;
        }

    }
}