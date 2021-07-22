using Android.Graphics;

using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class MyServiceShimmerAdapter : RecyclerView.Adapter
    {

        List<MyService> shimmerList = new List<MyService>();

        private Android.App.Activity mActivity;

        public MyServiceShimmerAdapter(List<MyService> data, Android.App.Activity Activity)
        {
            if (data == null)
            {
                this.shimmerList.Clear();
            }
            else
            {
                this.shimmerList = data;
            }
            this.mActivity = Activity;
        }

        public override int ItemCount => shimmerList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                MyServiceShimmerViewHolder vh = holder as MyServiceShimmerViewHolder;

                try
                {
                    TextViewUtils.SetMuseoSans500Typeface(vh.serviceTitle);

                    ViewGroup.LayoutParams currentCard = vh.myServiceCardView.LayoutParameters;
                    ViewGroup.LayoutParams currentImg = vh.serviceImg.LayoutParameters;

                    int cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                    float heightRatio = 84f / 96f;
                    int cardHeight = (int)(cardWidth * (heightRatio));
                    if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                    {
                        cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                        cardHeight = cardWidth;
                    }
                    else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                    {
                        cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                        cardHeight = cardWidth;
                    }

                    currentCard.Height = cardHeight;
                    currentCard.Width = cardWidth;

                    float imgHeightRatio = 24f / 96f;
                    int imgHeight = (int)(cardWidth * (imgHeightRatio));
                    if (DPUtils.ConvertPxToDP(imgHeight) < 24f)
                    {
                        imgHeight = (int)DPUtils.ConvertDPToPx(24f);
                    }
                    currentImg.Height = imgHeight;
                    currentImg.Width = imgHeight;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }


                try
                {
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        vh.myServiceShimmerImg.SetShimmer(shimmerBuilder?.Build());
                        vh.myServiceShimmerText.SetShimmer(shimmerBuilder?.Build());
                        vh.myServiceShimmerTextTwo.SetShimmer(shimmerBuilder?.Build());
                    }
                    vh.myServiceShimmerImg.StartShimmer();
                    vh.myServiceShimmerText.StartShimmer();
                    vh.myServiceShimmerTextTwo.StartShimmer();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
            public TextView serviceTitle_Two { get; private set; }

            public LinearLayout myServiceCardView { get; private set; }

            public ShimmerFrameLayout myServiceShimmerImg { get; private set; }

            public ShimmerFrameLayout myServiceShimmerText { get; private set; }

            public ShimmerFrameLayout myServiceShimmerTextTwo { get; private set; }

            public MyServiceShimmerViewHolder(View itemView) : base(itemView)
            {
                serviceImg = itemView.FindViewById<ImageView>(Resource.Id.service_img);
                serviceTitle = itemView.FindViewById<TextView>(Resource.Id.service_title);
                serviceTitle_Two = itemView.FindViewById<TextView>(Resource.Id.service_title_two);
                myServiceCardView = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                myServiceShimmerImg = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerMyServiceImg);
                myServiceShimmerText = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerMyServiceTitle);
                myServiceShimmerTextTwo = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerMyServiceTitleTwo);
                TextViewUtils.SetTextSize12(serviceTitle, serviceTitle_Two);
            }
        }

        public class MyServiceShimmerItemDecoration : RecyclerView.ItemDecoration
        {

            private int spanCount;
            private float spacing;
            private bool includeEdge;

            public MyServiceShimmerItemDecoration(int spanCount, int dpSpacing, bool includeEdge, Android.App.Activity Activity)
            {
                this.spanCount = spanCount;
                this.spacing = DPUtils.ConvertDPToPx(dpSpacing);
                int cardWidth = (Activity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                {
                    this.spacing = DPUtils.ConvertDPToPx(dpSpacing - 1);
                }
                else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
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
