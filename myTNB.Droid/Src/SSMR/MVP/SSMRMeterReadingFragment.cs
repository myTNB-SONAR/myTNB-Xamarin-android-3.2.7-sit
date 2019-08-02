
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.MVP
{
	public class SSMRMeterReadingFragment : Fragment
	{
		private static string TITLE = "slide_title";
		private static string DESCRIPTION = "slide_description";
		private static string IMAGE_URL = "image_url";

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public static SSMRMeterReadingFragment Instance(SSMRMeterReadingModel model)
		{
            SSMRMeterReadingFragment fragment = new SSMRMeterReadingFragment();
			Bundle args = new Bundle();
			args.PutString(IMAGE_URL, model.Image);
			args.PutString(TITLE, model.Title);
			args.PutString(DESCRIPTION, model.Description);
            fragment.Arguments = args;
			return fragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			string imageUrl = Arguments.GetString(IMAGE_URL, "");
			string title = Arguments.GetString(TITLE, "");
			string description = Arguments.GetString(DESCRIPTION, "");
            ViewGroup viewGroup = (ViewGroup)inflater.Inflate(Resource.Layout.SMRSubmitMeterReadingFragmentLayout, container, false);

			ImageView tooltipImg = viewGroup.FindViewById(Resource.Id.tooltipImg) as ImageView;
			TextView titleView = viewGroup.FindViewById(Resource.Id.applyTitle) as TextView;
			TextView descriptionView = viewGroup.FindViewById(Resource.Id.applyDescription) as TextView;

			TextViewUtils.SetMuseoSans500Typeface(titleView);
			TextViewUtils.SetMuseoSans300Typeface(descriptionView);

			if (imageUrl.Contains("tooltip_bg"))
			{
				int imageUrlResource = Resource.Drawable.tooltip_bg_1;
				if (imageUrl != null)
				{
					if (imageUrl == "tooltip_bg_1")
					{
						imageUrlResource = Resource.Drawable.tooltip_bg_1;
					}
					else if (imageUrl == "tooltip_bg_2")
					{
						imageUrlResource = Resource.Drawable.tooltip_bg_2;
					}
					else
					{
						imageUrlResource = Resource.Drawable.tooltip_bg_3;
					}
				}

                tooltipImg.SetImageResource(imageUrlResource);
			}
			else
			{
				Bitmap bitmap = ImageUtils.GetImageBitmapFromUrl(imageUrl);
                tooltipImg.SetImageBitmap(bitmap);
			}

			titleView.Text = title;
			descriptionView.Text = description;
			return viewGroup;
		}
	}
}
