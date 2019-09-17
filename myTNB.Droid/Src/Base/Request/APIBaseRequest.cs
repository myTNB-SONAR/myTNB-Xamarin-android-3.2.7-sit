﻿using System;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Base.Request
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
            UserEntity user = UserEntity.GetActive();
            eid = user.UserName;
            sspuid = user.UserID;
            did = "";
            ft = FirebaseTokenEntity.GetLatest().FBToken;
            lang = "EN";
            sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID;
            sec_auth_k2 = "test";
            ses_param1 = "test";
            ses_param2 = "test";
        }

    }
}
