using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class MyServiceShimmerAdapter : RecyclerView.Adapter
    {

        List<MyService> shimmerList = null;



        public MyServiceShimmerAdapter(List<MyService> data)
        {
            if (data == null)
            {
                this.shimmerList = new List<MyService>();
            }
            else
            {
                this.shimmerList = new List<MyService>();
                this.shimmerList.AddRange(data);
            }
        }

        public override int ItemCount => shimmerList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyServiceShimmerViewHolder vh = holder as MyServiceShimmerViewHolder;

            TextViewUtils.SetMuseoSans500Typeface(vh.serviceTitle);
            var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
            if(shimmerBuilder != null)
            {
                vh.myServiceShimmerCircle.SetShimmer(shimmerBuilder?.Build());
                vh.myServiceShimmerText.SetShimmer(shimmerBuilder?.Build());
            }
            vh.myServiceShimmerCircle.StartShimmer();
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

            public ShimmerFrameLayout myServiceShimmerCircle { get; private set; }

            public ShimmerFrameLayout myServiceShimmerText { get; private set; }

            public MyServiceShimmerViewHolder(View itemView) : base(itemView)
            {
                serviceImg = itemView.FindViewById<ImageView>(Resource.Id.service_img);
                serviceTitle = itemView.FindViewById<TextView>(Resource.Id.service_title);
                myServiceCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
                myServiceShimmerCircle = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerCircle);
                myServiceShimmerText = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerText);
            }
        }

    }
}
