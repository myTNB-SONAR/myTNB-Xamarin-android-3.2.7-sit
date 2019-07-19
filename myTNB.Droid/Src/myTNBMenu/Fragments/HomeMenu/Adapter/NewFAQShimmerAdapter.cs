using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
	public class NewFAQShimmerAdapter : RecyclerView.Adapter
	{

		List<NewFAQ> faqList = null;



		public NewFAQShimmerAdapter(List<NewFAQ> data)
		{
			this.faqList = new List<NewFAQ>();
			this.faqList = data;
		}

		public override int ItemCount => faqList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			NewFAQShimmerViewHolder vh = holder as NewFAQShimmerViewHolder;
            var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
            if(shimmerBuilder != null)
            {
                vh.faqShimmerLayout.SetShimmer(shimmerBuilder?.Build());
            }
            vh.faqShimmerLayout.StartShimmer();
        }

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var id = Resource.Layout.NewFAQShimmerComponentView;
			var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
			return new NewFAQShimmerViewHolder(itemView);
		}




		public class NewFAQShimmerViewHolder : RecyclerView.ViewHolder
		{
			public CardView faqCardView { get; private set; }
            public ShimmerFrameLayout faqShimmerLayout { get; private set; }

            public NewFAQShimmerViewHolder(View itemView) : base(itemView)
			{
				faqCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
                faqShimmerLayout = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerContainer);
            }
		}

	}
}
