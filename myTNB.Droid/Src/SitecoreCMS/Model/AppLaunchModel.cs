using System.Collections.Generic;
using Android.Graphics;

namespace myTNB.SitecoreCMS.Model
{
	public class AppLaunchTimeStampResponseModel
	{
		public string Status { set; get; }
		public List<AppLaunchTimeStamp> Data { set; get; }
	}

	public class AppLaunchResponseModel
	{
		public string Status { set; get; }
		public List<AppLaunchModel> Data { set; get; }
	}

	public class AppLaunchModel
	{
		public string Title { set; get; }
		public string Description { set; get; }
		public string Image { set; get; }
        public string StartDateTime { set; get; }
        public string EndDateTime { set; get; }
        public string ShowForSeconds { set; get; }

        public Bitmap ImageBitmap { set; get; }

		public string ID { set; get; }
	}

	public class AppLaunchTimeStamp
	{
		public string Timestamp { set; get; }
		public string ID { set; get; }
	}
}