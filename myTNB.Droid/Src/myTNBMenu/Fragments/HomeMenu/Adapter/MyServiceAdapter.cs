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

            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    if (model.serviceCategoryName.Contains("<br/>") || model.serviceCategoryName.Contains("\n"))
                    {
                        string newStringValue = "";
                        if (model.serviceCategoryName.Contains("\n"))
                        {
                            newStringValue = model.serviceCategoryName.Replace("\n", "<br/>");
                        }
                        else if (model.serviceCategoryName.Contains("\r\n"))
                        {
                            newStringValue = model.serviceCategoryName.Replace("\r\n", "<br/>");
                        }
                        else
                        {
                            newStringValue = model.serviceCategoryName;
                        }
                        vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        string[] splittedString = model.serviceCategoryName.Trim().Split(" ");
                        string newStringName = "";
                        if (splittedString.Length > 4)
                        {
                            for (int i = 0; i < splittedString.Length; i++)
                            {
                                if (i == 2)
                                {
                                    newStringName += splittedString[i] + "<br/>";
                                }
                                else if (i == splittedString.Length - 1)
                                {
                                    newStringName += splittedString[i];
                                }
                                else
                                {
                                    newStringName += splittedString[i] + " ";
                                }
                            }
                        }
                        else if (splittedString.Length == 3 || splittedString.Length == 4)
                        {
                            for (int i = 0; i < splittedString.Length; i++)
                            {
                                if (i == 1)
                                {
                                    newStringName += splittedString[i] + "<br/>";
                                }
                                else if (i == splittedString.Length - 1)
                                {
                                    newStringName += splittedString[i];
                                }
                                else
                                {
                                    newStringName += splittedString[i] + " ";
                                }
                            }
                        }
                        else if (splittedString.Length == 2)
                        {
                            for (int i = 0; i < splittedString.Length; i++)
                            {
                                if (i == 0)
                                {
                                    newStringName += splittedString[i] + "<br/>";
                                }
                                else if (i == splittedString.Length - 1)
                                {
                                    newStringName += splittedString[i];
                                }
                                else
                                {
                                    newStringName += splittedString[i] + " ";
                                }
                            }
                        }
                        else
                        {
                            newStringName = model.serviceCategoryName;
                        }

                        vh.serviceTitle.TextFormatted = Html.FromHtml(newStringName, FromHtmlOptions.ModeLegacy);
                    }
                }
                else
                {
                    if (model.serviceCategoryName.Contains("<br/>") || model.serviceCategoryName.Contains("\n"))
                    {
                        string newStringValue = "";
                        if (model.serviceCategoryName.Contains("\n"))
                        {
                            newStringValue = model.serviceCategoryName.Replace("\n", "<br/>");
                        }
                        else if (model.serviceCategoryName.Contains("\r\n"))
                        {
                            newStringValue = model.serviceCategoryName.Replace("\r\n", "<br/>");
                        }
                        else
                        {
                            newStringValue = model.serviceCategoryName;
                        }
                        vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue);
                    }
                    else
                    {
                        string[] splittedString = model.serviceCategoryName.Trim().Split(" ");
                        string newStringName = "";
                        if (splittedString.Length > 4)
                        {
                            for (int i = 0; i < splittedString.Length; i++)
                            {
                                if (i == 2)
                                {
                                    newStringName += splittedString[i] + "<br/>";
                                }
                                else if (i == splittedString.Length - 1)
                                {
                                    newStringName += splittedString[i];
                                }
                                else
                                {
                                    newStringName += splittedString[i] + " ";
                                }
                            }
                        }
                        else if (splittedString.Length == 3 || splittedString.Length == 4)
                        {
                            for (int i = 0; i < splittedString.Length; i++)
                            {
                                if (i == 1)
                                {
                                    newStringName += splittedString[i] + "<br/>";
                                }
                                else if (i == splittedString.Length - 1)
                                {
                                    newStringName += splittedString[i];
                                }
                                else
                                {
                                    newStringName += splittedString[i] + " ";
                                }
                            }
                        }
                        else if (splittedString.Length == 3)
                        {
                            for (int i = 0; i < splittedString.Length; i++)
                            {
                                if (i == 1)
                                {
                                    newStringName += splittedString[i] + "<br/>";
                                }
                                else if (i == splittedString.Length - 1)
                                {
                                    newStringName += splittedString[i];
                                }
                                else
                                {
                                    newStringName += splittedString[i] + " ";
                                }
                            }
                        }
                        else if (splittedString.Length == 2)
                        {
                            for (int i = 0; i < splittedString.Length; i++)
                            {
                                if (i == 0)
                                {
                                    newStringName += splittedString[i] + "<br/>";
                                }
                                else if (i == splittedString.Length - 1)
                                {
                                    newStringName += splittedString[i];
                                }
                                else
                                {
                                    newStringName += splittedString[i] + " ";
                                }
                            }
                        }
                        else
                        {
                            newStringName = model.serviceCategoryName;
                        }

                        vh.serviceTitle.TextFormatted = Html.FromHtml(newStringName);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                switch (model.ServiceCategoryId)
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

                }

                ViewGroup.LayoutParams currentCard = vh.myServiceCardView.LayoutParameters;

                int cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(9f);
                float heightRatio = 84f / 88f;
                int cardHeight = (int)(cardWidth * (heightRatio));
                if (DPUtils.ConvertDPToPixel(cardWidth) > 99f && DPUtils.ConvertPxToDP(cardWidth) <= 111f)
                {
                    cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(7f);
                    cardHeight = cardWidth;
                }
                else if (DPUtils.ConvertPxToDP(cardWidth) <= 99f)
                {
                    cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(6f);
                    cardHeight = cardWidth;
                }

                currentCard.Height = cardHeight;
                currentCard.Width = cardWidth;

                TextViewUtils.SetMuseoSans500Typeface(vh.serviceTitle);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


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

            public MyServiceItemDecoration(int spanCount, int dpSpacing, bool includeEdge, Android.App.Activity Activity)
            {
                this.spanCount = spanCount;
                this.spacing = DPUtils.ConvertDPToPx(dpSpacing);
                int cardWidth = (Activity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(9f);
                if (DPUtils.ConvertDPToPixel(cardWidth) > 99f && DPUtils.ConvertPxToDP(cardWidth) <= 111f)
                {
                    this.spacing = DPUtils.ConvertDPToPx(dpSpacing - 1);
                }
                else if (DPUtils.ConvertPxToDP(cardWidth) <= 99f)
                {
                    this.spacing = DPUtils.ConvertDPToPx(dpSpacing - 2);
                }
                this.includeEdge = includeEdge;
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                int position = parent.GetChildAdapterPosition(view); // item position
                int column = position % spanCount; // item column

                outRect.Left = (int)(column * spacing / spanCount); // column * ((1f / spanCount) * spacing)
                outRect.Right = (int)(spacing - (column + 1) * spacing / spanCount); // spacing - (column + 1) * ((1f /    spanCount) * spacing)
            }
        }

    }
}
