using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
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

        private Android.App.Activity mActivity;

        public NewFAQShimmerAdapter(List<NewFAQ> data, Android.App.Activity Activity)
		{
            if (data == null)
            {
                this.faqList.Clear();
            }
            else
            {
                this.faqList = data;
            }
            this.mActivity = Activity;
        }

		public override int ItemCount => faqList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			NewFAQShimmerViewHolder vh = holder as NewFAQShimmerViewHolder;
            ViewGroup.LayoutParams currentCard = vh.faqCardView.LayoutParameters;

            DisplayMetrics displaymetrics = new DisplayMetrics();
            this.mActivity.WindowManager.DefaultDisplay.GetMetrics(displaymetrics);
            int devicewidth = (int)((displaymetrics.WidthPixels / 2.85) - DPUtils.ConvertDPToPx(20f));
            currentCard.Height = devicewidth;
            currentCard.Width = devicewidth;
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
