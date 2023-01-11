using System;
using Android.Graphics;

namespace myTNB_Android.Src.MyHome.Model
{
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
}

