
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class NewPromotionShimmerAdapter : RecyclerView.Adapter
    {

        List<NewPromotion> promotionList = new List<NewPromotion>();


        private Android.App.Activity mActivity;

        public NewPromotionShimmerAdapter(List<NewPromotion> data, Android.App.Activity Activity)
        {
            if (data == null)
            {
                this.promotionList.Clear();
            }
            else
            {
                this.promotionList = data;
            }
            this.mActivity = Activity;
        }

        public override int ItemCount => promotionList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            NewPromotionShimmerViewHolder vh = holder as NewPromotionShimmerViewHolder;

            try
            {
                ViewGroup.LayoutParams currentCard = vh.cardView.LayoutParameters;

                int cardWidth = (int)((this.mActivity.Resources.DisplayMetrics.WidthPixels / 1.35) - DPUtils.ConvertDPToPx(6f));
                currentCard.Width = cardWidth;
                
                var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                if (shimmerBuilder != null)
                {
                    vh.shimmerPromoImage.SetShimmer(shimmerBuilder?.Build());
                    vh.shimmerPromotitle.SetShimmer(shimmerBuilder?.Build());
                    vh.shimmertTxtMessage1.SetShimmer(shimmerBuilder?.Build());
                    vh.shimmertTxtMessage2.SetShimmer(shimmerBuilder?.Build());
                    vh.shimmertTxtMessage3.SetShimmer(shimmerBuilder?.Build());
                }
                vh.shimmerPromoImage.StartShimmer();
                vh.shimmerPromotitle.StartShimmer();
                vh.shimmertTxtMessage1.StartShimmer();
                vh.shimmertTxtMessage2.StartShimmer();
                vh.shimmertTxtMessage3.StartShimmer();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.PromotionCardShimmerLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new NewPromotionShimmerViewHolder(itemView);
        }


        public class NewPromotionShimmerViewHolder : RecyclerView.ViewHolder
        {
            public CardView cardView { get; private set; }

            public ShimmerFrameLayout shimmerPromoImage { get; private set; }

            public ShimmerFrameLayout shimmerPromotitle { get; private set; }

            public ShimmerFrameLayout shimmertTxtMessage1 { get; private set; }

            public ShimmerFrameLayout shimmertTxtMessage2 { get; private set; }

            public ShimmerFrameLayout shimmertTxtMessage3 { get; private set; }

            public NewPromotionShimmerViewHolder(View itemView) : base(itemView)
            {
                cardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
                shimmerPromoImage = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerPromoImage);
                shimmerPromotitle = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerPromotitle);
                shimmertTxtMessage1 = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmertTxtMessage1);
                shimmertTxtMessage2 = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmertTxtMessage2);
                shimmertTxtMessage3 = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmertTxtMessage3);
            }
        }

    }
}
