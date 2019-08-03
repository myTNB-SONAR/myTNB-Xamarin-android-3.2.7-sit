using Android.Content;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCM.Models;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Utils;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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

            Bitmap bitmap = ImageUtils.GetImageBitmapFromUrl(model.Image);
            tooltipImg.SetImageBitmap(bitmap);

            titleView.Text = model.Title;
            descriptionView.Text = model.Description;

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
    }
}