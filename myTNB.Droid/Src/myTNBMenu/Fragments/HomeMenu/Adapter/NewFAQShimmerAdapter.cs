
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
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
            try
            {
                NewFAQShimmerViewHolder vh = holder as NewFAQShimmerViewHolder;
                ViewGroup.LayoutParams currentCard = vh.faqCardView.LayoutParameters;

                int cardWidth = (int)((this.mActivity.Resources.DisplayMetrics.WidthPixels / 2.95) - DPUtils.ConvertDPToPx(12f));

                float heightRatio = 56f / 92f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                currentCard.Height = cardHeight;
                currentCard.Width = cardWidth;

                ShimmerFrameLayout.LayoutParams layoutParams = new ShimmerFrameLayout.LayoutParams(cardWidth,
                cardHeight);
                if (position == 0)
                {
                    layoutParams.LeftMargin = (int)DPUtils.ConvertDPToPx(11f);
                    layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(2f);
                }
                if ((position + 1) == faqList.Count)
                {
                    layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(11f);
                }
                else
                {
                    layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(2f);
                }
                vh.faqCardView.LayoutParameters = layoutParams;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
