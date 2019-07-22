using Android.Support.V7.Widget;
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



        public MyServiceShimmerAdapter(List<MyService> data)
        {
            if (data == null)
            {
                this.shimmerList.Clear();
            }
            else
            {
                this.shimmerList = data;
            }
        }

        public override int ItemCount => shimmerList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyServiceShimmerViewHolder vh = holder as MyServiceShimmerViewHolder;

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

            public CardView myServiceCardView { get; private set; }

            public MyServiceShimmerViewHolder(View itemView) : base(itemView)
            {
                serviceImg = itemView.FindViewById<ImageView>(Resource.Id.service_img);
                serviceTitle = itemView.FindViewById<TextView>(Resource.Id.service_title);
                myServiceCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
            }
        }

    }
}
