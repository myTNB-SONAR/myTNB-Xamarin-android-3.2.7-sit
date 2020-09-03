﻿using System;
using System.Collections.Generic;
using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Adapter
{
    public class ApplicationStatusDetailProgressAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        private List<DetailProgressModel> mProgressList = new List<DetailProgressModel>();
        public event EventHandler<int> ItemClick;

        public void Clear()
        {
            this.mProgressList.Clear();
            this.NotifyDataSetChanged();
        }

        public ApplicationStatusDetailProgressAdapter(BaseActivityCustom activity, List<DetailProgressModel> data)
        {
            this.mActicity = activity;
            this.mProgressList.AddRange(data);
        }

        public override int ItemCount => mProgressList.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationStatusDetailItemStatusLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ApplicationDetailProgressViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationDetailProgressViewHolder vh = holder as ApplicationDetailProgressViewHolder;

            DetailProgressModel item = mProgressList[position];
            vh.PopulateData(item);
        }

        public class ApplicationDetailProgressViewHolder : RecyclerView.ViewHolder
        {
            public ImageView ImgApplicationStatusDone { get; private set; }
            public ImageView ImgApplicationStatusOther { get; private set; }
            public ImageView ImgApplicationStatusNew { get; private set; }
            public View ApplicationStatusLine { get; private set; }
            public TextView TxtApplicationStatusDetailWord { get; private set; }
            public TextView TxtApplicationStatusDetailCTA { get; private set; }

            private Context context;

            private DetailProgressModel item = null;

            public ApplicationDetailProgressViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                context = itemView.Context;
                ImgApplicationStatusDone = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationStatusDone);
                ImgApplicationStatusOther = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationStatusOther);
                ImgApplicationStatusNew = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationStatusnNew);
                ApplicationStatusLine = itemView.FindViewById<View>(Resource.Id.applicationStatusLine);
                TxtApplicationStatusDetailWord = itemView.FindViewById<TextView>(Resource.Id.txtApplicationStatusDetailCTA);
                TxtApplicationStatusDetailCTA = itemView.FindViewById<TextView>(Resource.Id.txtApplicationStatusDetailCTA);
                TxtApplicationStatusDetailCTA.Clickable = true;
                TxtApplicationStatusDetailCTA.Click += (sender, e) => listener(base.LayoutPosition);
            }


            public void PopulateData(DetailProgressModel item)
            {
                this.item = item;
                try
                {
                    //  TODO: ApplicationStatus Setup whole view

                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

        }
    }
}
