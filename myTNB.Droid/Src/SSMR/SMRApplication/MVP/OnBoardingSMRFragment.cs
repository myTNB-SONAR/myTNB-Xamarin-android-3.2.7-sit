
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;

using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class OnBoardingSMRFragment : Fragment
    {
        private static string TITLE = "slide_title";
        private static string DESCRIPTION = "slide_description";
        private static string IMAGE_URL = "image_url";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static OnBoardingSMRFragment Instance(ApplySSMRModel model)
        {
            OnBoardingSMRFragment fragment = new OnBoardingSMRFragment();
            Bundle args = new Bundle();
            args.PutString(IMAGE_URL, model.Image);
            args.PutString(TITLE, model.Title);
            args.PutString(DESCRIPTION, model.Description);
            fragment.Arguments = args;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            string imageUrl = Arguments.GetString(IMAGE_URL, "");
            string title = Arguments.GetString(TITLE, "");
            string description = Arguments.GetString(DESCRIPTION, "");
            ViewGroup viewGroup = (ViewGroup)inflater.Inflate(Resource.Layout.OnBoardingSMRFragmentLayout, container, false);
            ImageView imageSource = viewGroup.FindViewById(Resource.Id.applyImage) as ImageView;
            TextView titleView = viewGroup.FindViewById(Resource.Id.applyTitle) as TextView;
            TextView descriptionView = viewGroup.FindViewById(Resource.Id.applyDescription) as TextView;

            TextViewUtils.SetMuseoSans500Typeface(titleView);
            TextViewUtils.SetMuseoSans300Typeface(descriptionView);

            Bitmap bitmap = ImageUtils.GetImageBitmapFromUrl(imageUrl);
            int imageUrlResource = Resource.Drawable.onboarding_bg_1;
            if (bitmap == null)
            {
                if (imageUrl.Contains("onboarding1"))
                {
                    imageUrlResource = Resource.Drawable.onboarding_bg_1;
                }

                if (imageUrl.Contains("onboarding2"))
                {
                    imageUrlResource = Resource.Drawable.onboarding_bg_2;
                }
                imageSource.SetImageResource(imageUrlResource);
            }
            else
            {
                imageSource.SetImageBitmap(bitmap);
            }

            titleView.Text = title;
            descriptionView.Text = description;
            return viewGroup;
        }
    }
}
