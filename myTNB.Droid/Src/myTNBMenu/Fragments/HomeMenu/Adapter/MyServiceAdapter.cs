using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
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

		List<MyService> myServiceList = new List<MyService>();

        public event EventHandler<int> ClickChanged;

        private Android.App.Activity mActivity;


        public MyServiceAdapter(List<MyService> data, Android.App.Activity Activity)
		{
            if (data == null)
            {
                this.myServiceList.Clear();
            }
            else
            {
                this.myServiceList = data;
            }
            this.mActivity = Activity;
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

            ViewGroup.LayoutParams currentCard = vh.myServiceCardView.LayoutParameters;

            DisplayMetrics displaymetrics = new DisplayMetrics();
            this.mActivity.WindowManager.DefaultDisplay.GetMetrics(displaymetrics);
            int devicewidth = (displaymetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
            currentCard.Height = devicewidth - (int)DPUtils.ConvertDPToPx(4f);
            currentCard.Width = devicewidth;

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

        public class MyServiceItemDecoration : RecyclerView.ItemDecoration
        {

            private int spanCount;
            private float spacing;
            private bool includeEdge;

            public MyServiceItemDecoration(int spanCount, int dpSpacing, bool includeEdge)
            {
                this.spanCount = spanCount;
                this.spacing = DPUtils.ConvertDPToPx(dpSpacing);
                this.includeEdge = includeEdge;
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                int position = parent.GetChildAdapterPosition(view); // item position
                int column = position % spanCount; // item column

                if (includeEdge)
                {
                    outRect.Left = (int) (spacing - column * spacing / spanCount); // spacing - column * ((1f / spanCount) * spacing)
                    outRect.Right = (int) ((column + 1) * spacing / spanCount); // (column + 1) * ((1f / spanCount) * spacing)

                    if (position < spanCount)
                    { // top edge
                        outRect.Top = (int) spacing;
                    }
                    outRect.Bottom = (int) spacing; // item bottom
                }
                else
                {
                    outRect.Left = (int) (column * spacing / spanCount); // column * ((1f / spanCount) * spacing)
                    outRect.Right = (int) (spacing - (column + 1) * spacing / spanCount); // spacing - (column + 1) * ((1f /    spanCount) * spacing)
                    if (position >= spanCount)
                    {
                        outRect.Top = (int)spacing / 4; // item top
                    }

                    outRect.Bottom = (int)spacing / 4; // item bottom
                }
            }
        }

    }
}
