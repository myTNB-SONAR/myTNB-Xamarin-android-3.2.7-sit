using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;
using Android.Graphics;

namespace myTNB_Android.Src.WalkThrough.Fragment
{
    public class WalkThroughFragment : Android.Support.V4.App.Fragment
    {
        private static string IMAGE_ID = "slide_image";
        private static string HEADING = "slide_heading";
        private static string CONTENT = "slide_content";
        private static string IMAGE_URL = "image_url";

        public WalkThroughFragment()
        {

        }

        public static WalkThroughFragment newInstance(int imageId, string imageUrl, String heading, String content)
        {
            WalkThroughFragment fragment = new WalkThroughFragment();

            Bundle args = new Bundle();
            args.PutInt(IMAGE_ID, imageId);
            args.PutString(HEADING, heading);
            args.PutString(CONTENT, content);
            args.PutString(IMAGE_URL, imageUrl);
            fragment.Arguments = args;


            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            int imgId = Arguments.GetInt(IMAGE_ID, 1);
            string heading = Arguments.GetString(HEADING, "");
            string content = Arguments.GetString(CONTENT, "");
            string imageUrl = Arguments.GetString(IMAGE_URL, "");


            // Use this to return your custom view for this Fragment
            View rootView = inflater.Inflate(Resource.Layout.WalkThroughContentView, container, false);
            try {
            TextView txtHeading = (TextView)rootView.FindViewById(Resource.Id.heading);
            txtHeading.Text = heading;
            TextView txtContent = (TextView)rootView.FindViewById(Resource.Id.content);
            txtContent.Text = content;
            ImageView imgView = (ImageView)rootView.FindViewById(Resource.Id.tour_image);
            if (imageUrl != null && !imageUrl.Equals(""))
            {
                Bitmap bitmap = ImageUtils.GetImageBitmapFromUrl(imageUrl);
                imgView.SetImageBitmap(bitmap);
            }
            else
            {
                imgView.SetImageResource(imgId);
            }

            TextViewUtils.SetMuseoSans300Typeface( txtContent);
            TextViewUtils.SetMuseoSans500Typeface( txtHeading);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return rootView;
        }
    }
}