using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
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
            MyServiceShimmerViewHolder vh = holder as MyServiceShimmerViewHolder;

            TextViewUtils.SetMuseoSans500Typeface(vh.serviceTitle);

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

            var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
            if (shimmerBuilder != null)
            {
                vh.myServiceShimmerImg.SetShimmer(shimmerBuilder?.Build());
                vh.myServiceShimmerText.SetShimmer(shimmerBuilder?.Build());
            }
            vh.myServiceShimmerImg.StartShimmer();
            vh.myServiceShimmerText.StartShimmer();
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

            public CardView myServiceCardView { get; private set; }

            public ShimmerFrameLayout myServiceShimmerImg { get; private set; }

            public ShimmerFrameLayout myServiceShimmerText { get; private set; }

            public MyServiceShimmerViewHolder(View itemView) : base(itemView)
            {
                serviceImg = itemView.FindViewById<ImageView>(Resource.Id.service_img);
                serviceTitle = itemView.FindViewById<TextView>(Resource.Id.service_title);
                myServiceCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
                myServiceShimmerImg = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerMyServiceImg);
                myServiceShimmerText = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerMyServiceTitle);
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
