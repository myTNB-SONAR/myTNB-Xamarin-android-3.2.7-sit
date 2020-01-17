using System.Collections.Generic;
using Android.Graphics;

namespace myTNB.SitecoreCMS.Model
{
	public class WhatsNewTimeStampResponseModel
    {
		public string Status { set; get; }
		public List<WhatsNewTimeStamp> Data { set; get; }
	}

	public class WhatsNewResponseModel
    {
		public string Status { set; get; }
		public List<WhatsNewCategoryModel> Data { set; get; }
	}

	public class WhatsNewCategoryModel
    {
		public string ID { set; get; }
		public string CategoryName { set; get; }
		public List<WhatsNewModel> WhatsNewList { set; get; }
	}

	public class WhatsNewModel
    {
		public string CategoryID { set; get; }
		public string CategoryName { set; get; }
		public string ID { set; get; }
		public string Title { set; get; }
		public string TitleOnListing { set; get; }
		public string Description { set; get; }
		public string Image { set; get; }
		public string ImageB64 { set; get; }
		public Bitmap ImageBitmap { set; get; }
		public string StartDate { set; get; }
		public string EndDate { set; get; }
		public string CTA { set; get; }
		public bool Read { set; get; }
		public string ReadDateTime { set; get; }
        public string Language { set; get; }
    }

	public class WhatsNewTimeStamp
    {
		public string Timestamp { set; get; }
		public string ID { set; get; }
	}
}