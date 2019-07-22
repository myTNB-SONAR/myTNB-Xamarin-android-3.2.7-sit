﻿using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
	public class MyServiceAdapter : RecyclerView.Adapter
	{

		List<MyService> myServiceList = null;

        public event EventHandler<int> ClickChanged;


        public MyServiceAdapter(List<MyService> data)
		{
			this.myServiceList = new List<MyService>();
			this.myServiceList.AddRange(data);
		}

        public override int ItemCount => myServiceList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			MyServiceViewHolder vh = holder as MyServiceViewHolder;

			MyService model = myServiceList[position];

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                vh.serviceTitle.TextFormatted = Html.FromHtml(model.serviceCategoryName, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                vh.serviceTitle.TextFormatted = Html.FromHtml(model.serviceCategoryName);
            }

            switch(model.ServiceCategoryId)
            {
                case "1001":
                    vh.serviceImg.SetImageResource(Resource.Drawable.submit_meter);
                    break;
                case "1002":
                    vh.serviceImg.SetImageResource(Resource.Drawable.check_status);
                    break;
                case "1003":
                    vh.serviceImg.SetImageResource(Resource.Drawable.feedback);
                    break;
                case "1004":
                    vh.serviceImg.SetImageResource(Resource.Drawable.set_appointments);
                    break;
                case "1005":
                    vh.serviceImg.SetImageResource(Resource.Drawable.apply_autopay);
                    break;
                default:
                    vh.serviceImg.SetImageResource(Resource.Drawable.submit_meter);
                    break;

            }

			TextViewUtils.SetMuseoSans500Typeface(vh.serviceTitle);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var id = Resource.Layout.MyServiceComponentView;
			var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
			return new MyServiceViewHolder(itemView, OnClick);
		}

        void OnClick(MyServiceViewHolder sender, int position)
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


        public class MyServiceViewHolder : RecyclerView.ViewHolder
		{

			public ImageView serviceImg { get; private set; }

			public TextView serviceTitle { get; private set; }

			public CardView myServiceCardView { get; private set; }

			public MyServiceViewHolder(View itemView, Action<MyServiceViewHolder, int> listener) : base(itemView)
			{
				serviceImg = itemView.FindViewById<ImageView>(Resource.Id.service_img);
				serviceTitle = itemView.FindViewById<TextView>(Resource.Id.service_title);
				myServiceCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);

                myServiceCardView.Click += (s, e) => listener((this), base.LayoutPosition);
            }
		}

	}
}
