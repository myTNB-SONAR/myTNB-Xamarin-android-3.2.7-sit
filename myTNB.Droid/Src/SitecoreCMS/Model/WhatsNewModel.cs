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
		public string PublishDate { set; get; }
		public string CTA { set; get; }
		public string Image_DetailsView { set; get; }
		public string Image_DetailsViewB64 { set; get; }
		public Bitmap Image_DetailsViewBitmap { set; get; }
		public string Styles_DetailsView { set; get; }
		public string Description_Images { set; get; }
		public List<WhatsNewDetailImageDBModel> Description_Images_List { set; get; }
		public string PortraitImage_PopUp { set; get; }
		public Bitmap PortraitImage_PopUpBitmap { set; get; }
		public string PortraitImage_PopUpB64 { set; get; }
		public int ShowEveryCountDays_PopUp { set; get; }
		public int ShowForTotalCountDays_PopUp { set; get; }
		public bool ShowAtAppLaunchPopUp { set; get; }
		public bool PopUp_Text_Only { set; get; }
        public string PopUp_HeaderImage { set; get; }
		public string PopUp_HeaderImageB64 { set; get; }
		public Bitmap PopUp_HeaderImageBitmap { set; get; }
		public string PopUp_Text_Content { set; get; }
        public bool Donot_Show_In_WhatsNew { set; get; }
		public bool Disable_DoNotShow_Checkbox { set; get; }
		public string ShowDateForDay { set; get; }
		public int ShowCountForDay { set; get; }
		public bool SkipShowOnAppLaunch { set; get; }
		public bool Read { set; get; }
		public string ReadDateTime { set; get; }
	}

	public class WhatsNewDetailImageModel
    {
		public string ExtractedImageTag { set; get; }
		public string ExtractedImageUrl { set; get; }
		public Bitmap ExtractedImageBitmap { set; get; }
	}

	public class WhatsNewDetailImageDBModel
	{
		public string ExtractedImageTag { set; get; }
		public string ExtractedImageUrl { set; get; }
		public string ExtractedImageB64 { set; get; }
	}

	public class WhatsNewTimeStamp
    {
		public string Timestamp { set; get; }
		public string ID { set; get; }
	}
}