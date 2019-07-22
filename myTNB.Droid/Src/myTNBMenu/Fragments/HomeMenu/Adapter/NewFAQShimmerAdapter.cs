using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
	public class NewFAQShimmerAdapter : RecyclerView.Adapter
	{

		List<NewFAQ> faqList = new List<NewFAQ>();



		public NewFAQShimmerAdapter(List<NewFAQ> data)
		{
            if (data == null)
            {
                this.faqList.Clear();
            }
            else
            {
                this.faqList = data;
            }
		}

		public override int ItemCount => faqList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			NewFAQShimmerViewHolder vh = holder as NewFAQShimmerViewHolder;
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

            public NewFAQShimmerViewHolder(View itemView) : base(itemView)
			{
				faqCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
            }
		}

	}
}
