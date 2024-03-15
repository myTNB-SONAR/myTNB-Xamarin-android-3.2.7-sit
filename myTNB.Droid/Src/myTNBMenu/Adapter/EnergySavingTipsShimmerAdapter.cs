using Android.Graphics;

using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.myTNBMenu.Adapter
{
	public class EnergySavingTipsShimmerAdapter : RecyclerView.Adapter
	{

		List<EnergySavingTipsModel> energyList = new List<EnergySavingTipsModel>();

		private Android.App.Activity mActivity;

		public EnergySavingTipsShimmerAdapter(List<EnergySavingTipsModel> list, Android.App.Activity Activity)
		{

            if (list == null)
            {
                this.energyList.Clear();
            }
            else
            {
                this.energyList = list;
            }
			this.mActivity = Activity;
		}

		public override int ItemCount => energyList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
            EnergySavingTipsShimmerViewHolder vh = holder as EnergySavingTipsShimmerViewHolder;

			try
			{
				ViewGroup.LayoutParams currentCard = vh.cardView.LayoutParameters;

				int cardWidth = (int)((this.mActivity.Resources.DisplayMetrics.WidthPixels / 1.07) - DPUtils.ConvertDPToPx(6f));
				if (DPUtils.ConvertPxToDP(cardWidth) < 288f)
				{
					cardWidth = (int)DPUtils.ConvertDPToPx(288f);
				}
				currentCard.Width = cardWidth;

				LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(currentCard.Width,
				currentCard.Height);
				if (position == 0)
				{
					layoutParams.LeftMargin = (int)DPUtils.ConvertDPToPx(10f);
				}
				if ((position + 1) == energyList.Count)
				{
					layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(12f);
				}
				else
				{
					layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(4f);
				}
				vh.cardView.LayoutParameters = layoutParams;

                int secondTxtViewWidth = (int) (cardWidth * 0.70);

                ViewGroup.LayoutParams currentTxtView2LayoutParams = vh.shimmerTipsTxtView2.LayoutParameters;
                currentTxtView2LayoutParams.Width = secondTxtViewWidth;

                vh.shimmerTipsTxtView2.LayoutParameters = currentTxtView2LayoutParams;

                try
                {
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        vh.shimmerTipsTitleView.SetShimmer(shimmerBuilder?.Build());
                        vh.shimmerImgTipsView.SetShimmer(shimmerBuilder?.Build());
                        vh.shimmerTipsTxtView1.SetShimmer(shimmerBuilder?.Build());
                        vh.shimmerTipsTxtView2.SetShimmer(shimmerBuilder?.Build());
                    }
                    vh.shimmerTipsTitleView.StartShimmer();
                    vh.shimmerImgTipsView.StartShimmer();
                    vh.shimmerTipsTxtView1.StartShimmer();
                    vh.shimmerTipsTxtView2.StartShimmer();
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
			var id = Resource.Layout.EnergyTipShimmerCardLayout;
			var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
			return new EnergySavingTipsShimmerViewHolder(itemView);
		}


		public class EnergySavingTipsShimmerViewHolder : RecyclerView.ViewHolder
		{

			public ShimmerFrameLayout shimmerTipsTitleView { get; private set; }

            public ShimmerFrameLayout shimmerImgTipsView { get; private set; }

            public ShimmerFrameLayout shimmerTipsTxtView1 { get; private set; }

            public ShimmerFrameLayout shimmerTipsTxtView2 { get; private set; }

            public CardView cardView { get; private set; }

            public EnergySavingTipsShimmerViewHolder(View itemView) : base(itemView)
			{
                shimmerTipsTitleView = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerTipsTitleView);
                shimmerImgTipsView = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerImgTipsView);
                shimmerTipsTxtView1 = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerTipsTxtView1);
                shimmerTipsTxtView2 = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerTipsTxtView2);
                cardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);

            }
		}
	}
}
