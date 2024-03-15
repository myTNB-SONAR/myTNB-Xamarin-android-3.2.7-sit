using System.Collections.Generic;
using myTNB.AndroidApp.Src.MyHome.Model;
using static myTNB.Mobile.MobileEnums;
using Android.Graphics;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.MyTNBService.Response;

namespace myTNB.AndroidApp.Src.MyHome.Model
{
    public class MyServiceModel
    {
        public string ServiceId { get; set; }
        public string ParentServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceIconUrl { get; set; }
        public string DisabledServiceIconUrl { get; set; }
        public string ServiceBannerUrl { get; set; }
        public bool Enabled { get; set; }
        public string SSODomain { get; set; }
        public string OriginURL { get; set; }
        public string RedirectURL { get; set; }
        public int DisplayType { get; set; }
        public string CancelURL { get; set; }
        public List<MyServiceModel> Children { get; set; }
        public ServiceEnum ServiceType { get; set; }
    }

    public class MyServiceIconModel
    {
        public string ServiceId { set; get; }

        public string ServiceIconUrl { set; get; }
        public string ServiceIconB64 { set; get; }
        public Bitmap ServiceIconBitmap { set; get; }

        public string ServiceBannerUrl { set; get; }
        public string ServiceBannerB64 { set; get; }
        public Bitmap ServiceBannerBitmap { set; get; }

        public string DisabledServiceIconUrl { set; get; }
        public string DisabledServiceIconB64 { set; get; }
        public Bitmap DisabledServiceIconBitmap { set; get; }

        public string TimeStamp { set; get; }
    }

    public class MyHomeModel : MyServiceModel { }

    public class MyHomeToolTipModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string PrimaryCTA { get; set; }
        public string SecondaryCTA { get; set; }
    }

    public class MyHomePaymentDetailsModel
    {
        public string AccountNumber { get; set; }
        public string AccountNickName { get; set; }
        public string AccountAddress { get; set; }
        public bool IsOwned { get; set; }
        public AccountChargesResponse AccountChargesResponse { get; set; }
        public RegisteredCardsResponse RegisteredCardsResponse { get; set; }
    }
}

