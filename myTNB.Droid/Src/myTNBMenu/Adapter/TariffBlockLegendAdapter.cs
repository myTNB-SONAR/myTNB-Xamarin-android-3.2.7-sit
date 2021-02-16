using Android.Graphics;
using Android.Graphics.Drawables;


using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Adapter
{
    public class TariffBlockLegendAdapter : RecyclerView.Adapter
    {

        List<TariffBlocksLegendData> tariffList = new List<TariffBlocksLegendData>();

        private Android.App.Activity mActivity;

        private bool isLastListHighlighted = false;

        public TariffBlockLegendAdapter(List<TariffBlocksLegendData> data, Android.App.Activity Activity, bool flag)
        {
            if (data == null)
            {
                this.tariffList.Clear();
            }
            else
            {
                this.tariffList = data;
            }
            this.mActivity = Activity;

            this.isLastListHighlighted = flag;
        }

        public override int ItemCount => tariffList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TariffBlockLegendViewHolder vh = holder as TariffBlockLegendViewHolder;

            TariffBlocksLegendData model = tariffList[position];

            try
            {
                vh.TariffBlockName.Text = model.BlockRange;
                vh.TariffBlockUnit.Text = model.BlockPrice;

                GradientDrawable shape = (GradientDrawable)vh.ImgTariff.Drawable;
                shape.SetColor(Color.Rgb(model.Color.RedColor, model.Color.GreenColor, model.Color.BlueData));

                if (isLastListHighlighted && (position == 0))
                {
                    vh.TariffBlockName.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mActivity, Resource.Color.sunGlow)));
                    vh.TariffBlockUnit.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mActivity, Resource.Color.sunGlow)));
                }
                else
                {
                    vh.TariffBlockName.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mActivity, Resource.Color.white)));
                    vh.TariffBlockUnit.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mActivity, Resource.Color.white)));
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                TextViewUtils.SetMuseoSans300Typeface(vh.TariffBlockName, vh.TariffBlockUnit);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.TariffBlockLegendItem;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new TariffBlockLegendViewHolder(itemView);
        }


        public class TariffBlockLegendViewHolder : RecyclerView.ViewHolder
        {

            public TextView TariffBlockName { get; private set; }

            public TextView TariffBlockUnit { get; private set; }

            public ImageView ImgTariff { get; private set; }

            public TariffBlockLegendViewHolder(View itemView) : base(itemView)
            {
                TariffBlockName = itemView.FindViewById<TextView>(Resource.Id.tariffBlockName);
                TariffBlockUnit = itemView.FindViewById<TextView>(Resource.Id.tariffBlockUnit);
                ImgTariff = itemView.FindViewById<ImageView>(Resource.Id.imgTariff);

                TariffBlockName.TextSize = TextViewUtils.GetFontSize(11f);
                TariffBlockUnit.TextSize = TextViewUtils.GetFontSize(11f);
            }
        }

    }
}
