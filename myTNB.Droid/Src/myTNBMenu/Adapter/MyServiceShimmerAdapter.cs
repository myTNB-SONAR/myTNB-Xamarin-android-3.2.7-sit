﻿using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Adapter
{
    public class MyServiceShimmerAdapter : RecyclerView.Adapter
    {

        List<MyService> shimmerList = null;



        public MyServiceShimmerAdapter(List<MyService> data)
        {
            this.shimmerList = new List<MyService>();
            this.shimmerList = data;
        }

        public override int ItemCount => shimmerList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyServiceShimmerViewHolder vh = holder as MyServiceShimmerViewHolder;

            MyService model = shimmerList[position];

            TextViewUtils.SetMuseoSans500Typeface(vh.serviceTitle);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.MyServiceShimmerComponent;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new MyServiceShimmerViewHolder(itemView);
        }




        public class MyServiceShimmerViewHolder : RecyclerView.ViewHolder
        {

            public ImageView serviceImg { get; private set; }

            public TextView serviceTitle { get; private set; }

            public MyServiceShimmerViewHolder(View itemView) : base(itemView)
            {
                serviceImg = itemView.FindViewById<ImageView>(Resource.Id.service_img);
                serviceTitle = itemView.FindViewById<TextView>(Resource.Id.service_title);
            }
        }

    }
}
