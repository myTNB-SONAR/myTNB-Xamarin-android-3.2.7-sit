using System;
using System.Collections.Generic;
using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Adapter
{
    public class ApplicationStatusDetailSubDetailAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        private List<TitleValueModel> mSubDetailList = new List<TitleValueModel>();

        public ApplicationStatusDetailSubDetailAdapter(BaseActivityCustom activity, List<TitleValueModel> data)
        {
            this.mActicity = activity;
            this.mSubDetailList.AddRange(data);
        }

        public override int ItemCount => mSubDetailList.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationStatusDetailItemContentLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ApplicationDetailSubDetailViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationDetailSubDetailViewHolder vh = holder as ApplicationDetailSubDetailViewHolder;

            TitleValueModel item = mSubDetailList[position];
            vh.PopulateData(item);
        }

        public class ApplicationDetailSubDetailViewHolder : RecyclerView.ViewHolder
        {
            public TextView TxtApplicationStatusHeader { get; private set; }
            public TextView TxtApplicationStatusDetail { get; private set; }

            private Context context;

            private TitleValueModel item = null;

            public ApplicationDetailSubDetailViewHolder(View itemView) : base(itemView)
            {
                context = itemView.Context;
                TxtApplicationStatusHeader = itemView.FindViewById<TextView>(Resource.Id.txtApplicationStatusHeader);
                TxtApplicationStatusDetail = itemView.FindViewById<TextView>(Resource.Id.txtApplicationStatusDetail);
                TextViewUtils.SetMuseoSans300Typeface(TxtApplicationStatusHeader, TxtApplicationStatusDetail);
            }


            public void PopulateData(TitleValueModel item)
            {
                this.item = item;
                try
                {
                    TxtApplicationStatusHeader.Text = this.item.Title;
                    TxtApplicationStatusDetail.Text = this.item.Value;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

        }
    }
}
