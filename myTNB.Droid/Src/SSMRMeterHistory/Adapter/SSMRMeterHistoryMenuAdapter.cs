using Android.Graphics;

using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.SSMRMeterHistory.Adapter
{
    public class SSMRMeterHistoryMenuAdapter : RecyclerView.Adapter
    {
        private List<SSMRMeterHistoryMenuModel> ssmrMeterHistoryMenuList = new List<SSMRMeterHistoryMenuModel>();

        public event EventHandler<int> ClickChanged;

        public SSMRMeterHistoryMenuAdapter(List<SSMRMeterHistoryMenuModel> data)
        {
            if (data == null)
            {
                this.ssmrMeterHistoryMenuList.Clear();
                this.NotifyDataSetChanged();
            }
            else
            {
                this.ssmrMeterHistoryMenuList.Clear();
                this.ssmrMeterHistoryMenuList.AddRange(data);
                this.NotifyDataSetChanged();
            }
        }

        public override int ItemCount => ssmrMeterHistoryMenuList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SSMRMeterHistoryMenuViewHolder vh = holder as SSMRMeterHistoryMenuViewHolder;

            SSMRMeterHistoryMenuModel model = ssmrMeterHistoryMenuList[position];

            vh.btnMenu.Text = model.MenuName;

            if (model.IsHighlighted == "true")
            {
                vh.btnMenu.SetTextColor(Color.ParseColor("#e44b21"));
                TextViewUtils.SetMuseoSans500Typeface(vh.btnMenu);
            }
            else
            {
                TextViewUtils.SetMuseoSans300Typeface(vh.btnMenu);
                vh.btnMenu.TextSize = TextViewUtils.GetFontSize(14f);
            }
            
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.SSMRMenuItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new SSMRMeterHistoryMenuViewHolder(itemView, OnClick);
        }

        void OnClick(SSMRMeterHistoryMenuViewHolder sender, int position)
        {
            try
            {
                ClickChanged(this, position);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public class SSMRMeterHistoryMenuViewHolder : RecyclerView.ViewHolder
        {

            public TextView btnMenu { get; private set; }

            public SSMRMeterHistoryMenuViewHolder(View itemView, Action<SSMRMeterHistoryMenuViewHolder, int> listener) : base(itemView)
            {
                btnMenu = itemView.FindViewById<TextView>(Resource.Id.btnMenu);

                btnMenu.Click += (s, e) => listener((this), base.LayoutPosition);
            }
        }

    }
}
