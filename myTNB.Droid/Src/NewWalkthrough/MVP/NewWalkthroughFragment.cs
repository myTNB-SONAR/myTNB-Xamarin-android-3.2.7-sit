
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughFragment : Fragment
    {
        private static string TITLE = "slide_title";
        private static string DESCRIPTION = "slide_description";
        private static string IMAGE = "image";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static NewWalkthroughFragment Instance(NewWalkthroughModel model)
        {
            NewWalkthroughFragment fragment = new NewWalkthroughFragment();
            Bundle args = new Bundle();
            args.PutString(IMAGE, model.Image);
            args.PutString(TITLE, model.Title);
            args.PutString(DESCRIPTION, model.Description);
            fragment.Arguments = args;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            string imageUrl = Arguments.GetString(IMAGE, "");
            string title = Arguments.GetString(TITLE, "");
            string description = Arguments.GetString(DESCRIPTION, "");
            ViewGroup viewGroup = (ViewGroup)inflater.Inflate(Resource.Layout.NewWalkthroughFragmentLayout, container, false);
            ScrollView bgLayout = viewGroup.FindViewById(Resource.Id.walkthrough_layout) as ScrollView;
            ImageView imageSource = viewGroup.FindViewById(Resource.Id.img_display) as ImageView;
            TextView titleView = viewGroup.FindViewById(Resource.Id.txtTitle) as TextView;
            TextView descriptionView = viewGroup.FindViewById(Resource.Id.txtMessage) as TextView;

            TextViewUtils.SetMuseoSans500Typeface(titleView);
            TextViewUtils.SetMuseoSans300Typeface(descriptionView);

            switch (imageUrl)
            {
                case "walkthrough_img_install_1":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_1);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughFirstBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int) DPUtils.ConvertDPToPx(75f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    LinearLayout.LayoutParams imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    
                    int imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    float heightRatio = 216f / 250f;
                    int imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_2":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_2);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughSecondBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(84f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    LinearLayout.LayoutParams secondImgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    int secondImgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    float secondHeightRatio = 207f / 250f;
                    int secondImgHeight = (int)(secondImgWidth * (secondHeightRatio));

                    secondImgParam.Width = secondImgWidth;
                    secondImgParam.Height = secondImgHeight;
                    break;
                case "walkthrough_img_install_3":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_3);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughThirdBg);
                    LinearLayout.LayoutParams thirdImgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(115.7f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    int thirdImgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    float thirdHeightRatio = 175f / 250f;
                    int thirdImgHeight = (int)(thirdImgWidth * (thirdHeightRatio));
                    thirdImgParam.Width = thirdImgWidth;
                    thirdImgParam.Height = thirdImgHeight;
                    break;
                case "walkthrough_img_update_1":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_update_1);
                    bgLayout.SetBackgroundResource(Resource.Drawable.UpdateWalkthroughFirstBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(102f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    LinearLayout.LayoutParams updateImgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    int updateImgWidth = GetDeviceHorizontalScaleInPixel(0.841f);
                    float updateHeightRatio = 189f / 269f;
                    int updateImgHeight = (int)(updateImgWidth * (updateHeightRatio));
                    updateImgParam.Width = updateImgWidth;
                    updateImgParam.Height = updateImgHeight;
                    break;
                default:
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_1);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughFirstBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(75f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    LinearLayout.LayoutParams defaultImgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    int defaultImgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    float defaultHeightRatio = 216f / 250f;
                    int defaultImgHeight = (int)(defaultImgWidth * (defaultHeightRatio));
                    defaultImgParam.Width = defaultImgWidth;
                    defaultImgParam.Height = defaultImgHeight;
                    break;
            }

            titleView.Text = title;
            descriptionView.Text = description;
            return viewGroup;
        }

        public int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        public int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
        }
    }
}
