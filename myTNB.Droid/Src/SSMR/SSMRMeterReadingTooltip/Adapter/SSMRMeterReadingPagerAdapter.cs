using Android.Content;
using Android.Graphics;
using Android.OS;

using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Utils;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static myTNB_Android.Src.AppLaunch.Models.MasterDataResponse;

namespace myTNB_Android.Src.SSMR.SSMRMeterReadingTooltip.Adapter
{
    public class SSMRMeterReadingPagerAdapter : PagerAdapter
    {
        private Context mContext;
        private List<SSMRMeterReadingModel> list = new List<SSMRMeterReadingModel>();

        public SSMRMeterReadingPagerAdapter(Context ctx, List<SSMRMeterReadingModel> items)
        {
            this.mContext = ctx;
            this.list = items;
        }

        public SSMRMeterReadingPagerAdapter()
        {

        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            ViewGroup rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.SMRSubmitMeterReadingFragmentLayout, container, false);
            ImageView tooltipImg = rootView.FindViewById(Resource.Id.tooltipImg) as ImageView;
            TextView titleView = rootView.FindViewById(Resource.Id.applyTitle) as TextView;
            TextView descriptionView = rootView.FindViewById(Resource.Id.applyDescription) as TextView;
            descriptionView.MovementMethod = new ScrollingMovementMethod();

            TextViewUtils.SetMuseoSans500Typeface(titleView);
            TextViewUtils.SetMuseoSans300Typeface(descriptionView);

            SSMRMeterReadingModel model = list[position];

            if (model.Image.Contains("tooltip_bg_"))
            {
                if (model.Image.Contains("1"))
                {
                    tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_1);
                }
                else if (model.Image.Contains("2"))
                {
                    tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_2);
                }
                else
                {
                    tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_3);
                }
            }
            else if (model.ImageBitmap != null)
            {
                tooltipImg.SetImageBitmap(model.ImageBitmap);
            }
            else
            {
                Bitmap bitmap = null;
                bitmap = ImageUtils.GetImageBitmapFromUrl(model.Image);
                if (bitmap != null)
                {
                    tooltipImg.SetImageBitmap(bitmap);
                }
                else
                {
                    if (list.Count == 1)
                    {
                        tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_3);
                    }
                    else if (list.Count == 2)
                    {
                        bool smrAccountOCRDown = SMRPopUpUtils.OnGetIsOCRDownFlag();
                        if (MyTNBAccountManagement.GetInstance().IsOCRDown() || smrAccountOCRDown)
                        {
                            if (position == 0)
                            {
                                tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_1);
                            }
                            else
                            {
                                tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_3);
                            }
                        }
                        else
                        {
                            if (position == 0)
                            {
                                tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_2);
                            }
                            else
                            {
                                tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_3);
                            }
                        }
                    }
                    else if (list.Count == 3)
                    {
                        if (position == 0)
                        {
                            tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_1);
                        }
                        else if (position == 1)
                        {
                            tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_2);
                        }
                        else
                        {
                            tooltipImg.SetImageResource(Resource.Drawable.tooltip_bg_3);
                        }
                    }
                }
            }

            titleView.Text = model.Title;
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                descriptionView.TextFormatted = Html.FromHtml(model.Description, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                descriptionView.TextFormatted = Html.FromHtml(model.Description);
            }
            //descriptionView.Text = model.Description;

            container.AddView(rootView);
            return rootView;
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return PagerAdapter.PositionNone;
        }

        public override int Count => list.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

        private Bitmap Base64ToBitmap(String base64String)
        {
            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }
    }
}
