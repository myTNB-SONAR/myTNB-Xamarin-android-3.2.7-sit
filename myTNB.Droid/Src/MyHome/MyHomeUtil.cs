﻿using System;
using Android.Content.Res;
using myTNB.AndroidApp.Src.Utils.Deeplink;
using static myTNB.AndroidApp.Src.MyTNBService.Response.PaymentTransactionIdResponse;

namespace myTNB.AndroidApp.Src.MyHome
{
	public sealed class MyHomeUtil
	{
        static MyHomeUtil instance;

        public MyHomeDetails MyHomeDetails;
        public bool IsCOTCOAFlow;
        public string ApplicationType;
        public string ReferenceNo;

        public static MyHomeUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyHomeUtil();
                }
                return instance;
            }
        }

        public void SetMyHomeDetails(MyHomeDetails details)
        {
            MyHomeDetails = details;
        }

        public void SetIsCOTCOAFlow()
        {
            IsCOTCOAFlow = true;
        }

        public void SetApplicationType(string type)
        {
            ApplicationType = type;
        }

        public void SetReferenceNo(string refNo)
        {
            ReferenceNo = refNo;
        }

        public void ClearCache()
        {
            MyHomeDetails = null;
            IsCOTCOAFlow = false;
            ApplicationType = string.Empty;
            ReferenceNo = string.Empty;
        }
    }
}

