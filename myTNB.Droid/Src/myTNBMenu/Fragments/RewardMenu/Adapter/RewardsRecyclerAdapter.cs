using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter
{
	public class RewardsRecyclerAdapter : RecyclerView.Adapter
	{

		List<RewardsModel> rewardsList = new List<RewardsModel>();

		public event EventHandler<int> ClickChanged;
        public event EventHandler<int> SavedClickChanged;

        private Android.App.Activity mActivity;

		public RewardsRecyclerAdapter(List<RewardsModel> data, Android.App.Activity Activity)
		{
			if (data == null)
			{
				this.rewardsList.Clear();
			}
			else
			{
				this.rewardsList = data;
			}
			this.mActivity = Activity;
		}

		public override int ItemCount => rewardsList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			try
			{
				RewardViewHolder vh = holder as RewardViewHolder;

                RewardsModel model = rewardsList[position];

				try
				{
                    TextViewUtils.SetMuseoSans500Typeface(vh.txtTitle);

                    LinearLayout.LayoutParams currentShimmerImg = vh.rewardMainShimmerImgLayout.LayoutParameters as LinearLayout.LayoutParams;
                    LinearLayout.LayoutParams currentMainImgLayout = vh.rewardMainImgLayout.LayoutParameters as LinearLayout.LayoutParams;
                    int currentImgWidth = this.mActivity.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(16f) - (int)DPUtils.ConvertDPToPx(16f);
                    float currentImgRatio = 112f / 288f;
                    int currentImgHeight = (int) (currentImgWidth * currentImgRatio);
                    currentShimmerImg.Height = currentImgHeight;
                    currentMainImgLayout.Height = currentImgHeight;


                    ViewGroup.LayoutParams currentCard = vh.rewardCardView.LayoutParameters;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(currentCard.Width,
                    currentCard.Height);
                    layoutParams.LeftMargin = (int)DPUtils.ConvertDPToPx(13f);
                    layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(13f);
                    if (position == 0)
                    {
                        layoutParams.TopMargin = (int)DPUtils.ConvertDPToPx(16f);
                        layoutParams.BottomMargin = (int)DPUtils.ConvertDPToPx(5f);
                    }
                    if ((position + 1) == rewardsList.Count)
                    {
                        layoutParams.TopMargin = (int)DPUtils.ConvertDPToPx(5f);
                        layoutParams.BottomMargin = (int)DPUtils.ConvertDPToPx(16f);
                    }
                    else
                    {
                        layoutParams.TopMargin = (int)DPUtils.ConvertDPToPx(5f);
                        layoutParams.BottomMargin = (int)DPUtils.ConvertDPToPx(5f);
                    }
                    vh.rewardCardView.LayoutParameters = layoutParams;
                }
				catch (Exception e)
				{
					Utility.LoggingNonFatalError(e);
				}

				try
				{
					if (string.IsNullOrEmpty(rewardsList[position].Image) || string.IsNullOrEmpty(rewardsList[position].ImageB64))
                    {
                        // Image Shimmer Start
                        if (string.IsNullOrEmpty(rewardsList[position].Image))
                        {
                            // Just Shimmer
                            vh.rewardMainImgLayout.Visibility = ViewStates.Gone;
                            vh.rewardMainShimmerImgLayout.Visibility = ViewStates.Visible;

                            try
                            {
                                var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                                if (shimmerBuilder != null)
                                {
                                    vh.shimmerRewardImageLayout.SetShimmer(shimmerBuilder?.Build());
                                }
                                vh.shimmerRewardImageLayout.StartShimmer();
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        }
                        else
                        {
                            // Shimmer, Pull Image, then update back the image here
                            vh.rewardMainImgLayout.Visibility = ViewStates.Gone;
                            vh.rewardMainShimmerImgLayout.Visibility = ViewStates.Visible;

                            try
                            {
                                var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                                if (shimmerBuilder != null)
                                {
                                    vh.shimmerRewardImageLayout.SetShimmer(shimmerBuilder?.Build());
                                }
                                vh.shimmerRewardImageLayout.StartShimmer();
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        }
                    }
                    else
                    {
                        // Image Shimmer Stop
                        vh.rewardMainShimmerImgLayout.Visibility = ViewStates.Gone;
                        if (vh.shimmerRewardImageLayout.IsShimmerStarted)
                        {
                            vh.shimmerRewardImageLayout.StopShimmer();
                        }

                        vh.rewardMainImgLayout.Visibility = ViewStates.Visible;
                        // update vh.rewardImg, set btnRewardSaveImg
                    }
                }
				catch (Exception e)
				{
					Utility.LoggingNonFatalError(e);
				}

                try
                {
                    if (string.IsNullOrEmpty(rewardsList[position].DisplayName))
                    {
                        // Text Shimmer Start
                        vh.rewardBottomView.Visibility = ViewStates.Gone;
                        vh.rewardMainShimmerTxtLayout.Visibility = ViewStates.Visible;

                        try
                        {
                            var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                            if (shimmerBuilder != null)
                            {
                                vh.shimmerRewardTxtLayout.SetShimmer(shimmerBuilder?.Build());
                            }
                            vh.shimmerRewardTxtLayout.StartShimmer();
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else
                    {
                        // Text Shimmer Stop
                        vh.rewardMainShimmerTxtLayout.Visibility = ViewStates.Gone;
                        if (vh.shimmerRewardTxtLayout.IsShimmerStarted)
                        {
                            vh.shimmerRewardTxtLayout.StopShimmer();
                        }

                        vh.rewardBottomView.Visibility = ViewStates.Visible;
                        vh.txtTitle.Text = rewardsList[position].DisplayName;
                        // update vh.txtTitle, set rewardUnreadImg and margin
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
			catch (Exception ne)
			{
				Utility.LoggingNonFatalError(ne);
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var id = Resource.Layout.RewardsCardLayout;
			var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
			return new RewardViewHolder(itemView, OnClick, OnSaveClick);
		}

		void OnClick(RewardViewHolder sender, int position)
		{
			try
			{
				ClickChanged(this, position);
			}
			catch (System.Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

        void OnSaveClick(RewardViewHolder sender, int position)
        {
            try
            {
                SavedClickChanged(this, position);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Bitmap ToGrayscale(Bitmap srcImage)
        {

            Bitmap bmpGrayscale = Bitmap.CreateBitmap(srcImage.Width, srcImage.Height, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(bmpGrayscale);
            Paint paint = new Paint();

            ColorMatrix cm = new ColorMatrix();
            cm.SetSaturation(0);
            paint.SetColorFilter(new ColorMatrixColorFilter(cm));
            canvas.DrawBitmap(srcImage, 0, 0, paint);

            return bmpGrayscale;
        }

        public class RewardViewHolder : RecyclerView.ViewHolder
		{
            // CardView, Enable click to see detail
            public CardView rewardCardView { get; private set; }

            // Main Image, enable when have img url and already pull
            public RelativeLayout rewardMainImgLayout { get; private set; }

            // Main Image, set image here
            public ImageView rewardImg { get; private set; }

            // Reward Save Button, Enable click to save / unsave reward
            public ImageView btnRewardSaveImg { get; private set; }

            // Image Shimmer Main Layout, enable when no img url / have img url but need pull
            public LinearLayout rewardMainShimmerImgLayout  { get; private set; }

            // Start Shimmer if need shimmer on Image, Stop otherview
            public ShimmerFrameLayout shimmerRewardImageLayout { get; private set; }

            // Main Txt, enable when have txt
            public RelativeLayout rewardBottomView { get; private set; }

            // Main Txt, insert Main Text Here, 500
            public TextView txtTitle { get; private set; }

            // Reward Unread Indicator, hide when already readed
            public ImageView rewardUnreadImg { get; private set; }

            // Txt Shimmer Main Layout, enable when no text
            public LinearLayout rewardMainShimmerTxtLayout { get; private set; }

            // Start Shimmer if need shimmer on Image, Stop otherview
            public ShimmerFrameLayout shimmerRewardTxtLayout { get; private set; }




			public RewardViewHolder(View itemView, Action<RewardViewHolder, int> listener, Action<RewardViewHolder, int> saveListener) : base(itemView)
			{
                rewardCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);

                rewardMainImgLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.rewardMainImg);
                rewardImg = itemView.FindViewById<ImageView>(Resource.Id.rewardImg);
                btnRewardSaveImg = itemView.FindViewById<ImageView>(Resource.Id.btnRewardSaveImg);

                rewardMainShimmerImgLayout = itemView.FindViewById<LinearLayout>(Resource.Id.rewardMainShimmerImgLayout);
                shimmerRewardImageLayout = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerRewardImageLayout);

                rewardBottomView = itemView.FindViewById<RelativeLayout>(Resource.Id.rewardBottomView);
                txtTitle = itemView.FindViewById<TextView>(Resource.Id.txtTitle);
                rewardUnreadImg = itemView.FindViewById<ImageView>(Resource.Id.rewardUnreadImg);

                rewardMainShimmerTxtLayout = itemView.FindViewById<LinearLayout>(Resource.Id.rewardMainShimmerTxtLayout);
                shimmerRewardTxtLayout = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerRewardTxtLayout);

                rewardCardView.Click += (s, e) => listener((this), base.LayoutPosition);
                btnRewardSaveImg.Click += (s, e) => saveListener((this), base.LayoutPosition);

            }
		}

	}
}
