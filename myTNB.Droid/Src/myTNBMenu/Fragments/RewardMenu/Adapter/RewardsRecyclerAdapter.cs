using Android.Graphics;
using Android.Graphics.Drawables;


using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter
{
	public class RewardsRecyclerAdapter : RecyclerView.Adapter
	{

		List<RewardsModel> rewardsList = new List<RewardsModel>();

		public event EventHandler<int> ClickChanged;
        public event EventHandler<int> SavedClickChanged;

        private Android.App.Activity mActivity;

        private REWARDSITEMLISTMODE mListMode;

        private Bitmap mDefaultBitmap;

        public RewardsRecyclerAdapter(List<RewardsModel> data, Android.App.Activity Activity, REWARDSITEMLISTMODE listMode)
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

            this.mListMode = listMode;

            BitmapFactory.Options opt = new BitmapFactory.Options();
            opt.InMutable = true;

            this.mDefaultBitmap = BitmapFactory.DecodeResource(this.mActivity.Resources, Resource.Drawable.ic_image_reward_empty, opt);

        }

        public void RefreshList(List<RewardsModel> data)
        {
            if (data == null)
            {
                this.rewardsList.Clear();
            }
            else
            {
                this.rewardsList = data;
            }
            NotifyDataSetChanged();
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
                    TextViewUtils.SetMuseoSans500Typeface(vh.txtTitle, vh.txtRewardUsed);

                    vh.txtRewardUsed.Text = Utility.GetLocalizedLabel("Rewards", "used");

                    LinearLayout.LayoutParams currentShimmerImg = vh.rewardMainShimmerImgLayout.LayoutParameters as LinearLayout.LayoutParams;
                    LinearLayout.LayoutParams currentMainImgLayout = vh.rewardMainImgLayout.LayoutParameters as LinearLayout.LayoutParams;
                    int currentImgWidth = this.mActivity.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(16f) - (int)DPUtils.ConvertDPToPx(16f);
                    float currentImgRatio = 112f / 288f;
                    int currentImgHeight = (int) (currentImgWidth * currentImgRatio);
                    currentShimmerImg.Height = currentImgHeight;
                    currentMainImgLayout.Height = currentImgHeight;

                    RelativeLayout.LayoutParams currentSaveRewardLayout = vh.btnRewardSaveImg.LayoutParameters as RelativeLayout.LayoutParams;
                    int currentSaveRewardWidth = (int) ((18f/288f) * currentImgWidth);
                    int currentSaveRewardHeight = (int)((15f / 112f) * currentImgHeight);
                    currentSaveRewardLayout.Height = currentSaveRewardHeight;
                    currentSaveRewardLayout.Width = currentSaveRewardWidth;

                    RelativeLayout.LayoutParams currentSaveRewardShadowLayout = vh.rewardSaveImgShadow.LayoutParameters as RelativeLayout.LayoutParams;
                    int currentSaveRewardShadowHeight = (int)((40f / 112f) * currentImgHeight);
                    currentSaveRewardShadowLayout.Height = currentSaveRewardShadowHeight;

                    RelativeLayout.LayoutParams currentRewardLayout = vh.rewardImg.LayoutParameters as RelativeLayout.LayoutParams;
                    currentRewardLayout.Height = currentImgHeight;


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
                            if (mListMode == REWARDSITEMLISTMODE.INITIATE)
                            {
                                // Just Shimmer
                                vh.rewardMainImgLayout.Visibility = ViewStates.Gone;
                                vh.rewardMainShimmerImgLayout.Visibility = ViewStates.Visible;

                                try
                                {
                                    if (vh.shimmerRewardImageLayout.IsShimmerStarted)
                                    {
                                        vh.shimmerRewardImageLayout.StopShimmer();
                                    }
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
                                vh.rewardUsedLayout.Visibility = ViewStates.Gone;

                                if (rewardsList[position].IsUsed)
                                {
                                    vh.rewardUsedLayout.Visibility = ViewStates.Visible;
                                    vh.rewardImg.SetImageBitmap(ToGrayscale(this.mDefaultBitmap));
                                }
                                else
                                {
                                    vh.rewardImg.SetImageBitmap(this.mDefaultBitmap);
                                }

                                if (rewardsList[position].IsSaved)
                                {
                                    vh.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_saved);
                                }
                                else
                                {
                                    vh.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_unsaved);
                                }

                                vh.rewardMainShimmerImgLayout.Visibility = ViewStates.Gone;

                                vh.rewardMainImgLayout.Visibility = ViewStates.Visible;
                            }
                        }
                        else
                        {
                            // Shimmer, Pull Image, then update back the image here
                            vh.rewardMainImgLayout.Visibility = ViewStates.Gone;
                            vh.rewardMainShimmerImgLayout.Visibility = ViewStates.Visible;

                            try
                            {
                                if (vh.shimmerRewardImageLayout.IsShimmerStarted)
                                {
                                    vh.shimmerRewardImageLayout.StopShimmer();
                                }
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

                            _ = GetImageAsync(vh, rewardsList[position]);
                        }
                    }
                    else
                    {
                        SetRewardImg(vh, rewardsList[position]);
                    }
                }
				catch (Exception e)
				{
					Utility.LoggingNonFatalError(e);
				}

                try
                {
                    if (string.IsNullOrEmpty(rewardsList[position].TitleOnListing))
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
                        vh.txtTitle.Text = rewardsList[position].TitleOnListing;
                        if (rewardsList[position].Read)
                        {
                            vh.rewardUnreadImg.Visibility = ViewStates.Gone;
                            RelativeLayout.LayoutParams txtTitleParam = vh.txtTitle.LayoutParameters as RelativeLayout.LayoutParams;
                            txtTitleParam.RightMargin = (int)DPUtils.ConvertDPToPx(16f);
                        }
                        else
                        {
                            vh.rewardUnreadImg.Visibility = ViewStates.Visible;
                            RelativeLayout.LayoutParams txtTitleParam = vh.txtTitle.LayoutParameters as RelativeLayout.LayoutParams;
                            txtTitleParam.RightMargin = (int) DPUtils.ConvertDPToPx(34f);
                        }

                        if (rewardsList[position].IsUsed)
                        {
                            vh.txtTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.mActivity, Resource.Color.charcoalGrey)));
                        }
                        else
                        {
                            vh.txtTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.mActivity, Resource.Color.powerBlue)));
                        }

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
                if (position != -1)
                {
                    RewardsModel targetItem = rewardsList[position];
                    if (targetItem != null)
                    {
                        if (!targetItem.Read)
                        {
                            sender.rewardUnreadImg.Visibility = ViewStates.Gone;
                            RelativeLayout.LayoutParams txtTitleParam = sender.txtTitle.LayoutParameters as RelativeLayout.LayoutParams;
                            txtTitleParam.RightMargin = (int)DPUtils.ConvertDPToPx(16f);
                        }
                    }
                    ClickChanged(this, position);
                }
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
                if (position != -1)
                {
                    RewardsModel targetItem = rewardsList[position];
                    if (targetItem != null)
                    {
                        if (targetItem.IsSaved)
                        {
                            sender.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_unsaved);
                        }
                        else
                        {
                            sender.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_saved);
                        }
                    }
                    SavedClickChanged(this, position);
                }
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

            float[] colorMatrixElements = { 0.33f, 0.33f, 0.33f, 0, 0, 0.33f, 0.33f, 0.33f, 0, 0, 0.33f, 0.33f, 0.33f, 0, 0, 0, 0, 0, 1, 0 };
            ColorMatrix cm = new ColorMatrix(colorMatrixElements);
            paint.SetColorFilter(new ColorMatrixColorFilter(cm));
            canvas.DrawBitmap(srcImage, 0, 0, paint);

            return bmpGrayscale;
        }

        public async Task GetImageAsync(RewardViewHolder viewHolder, RewardsModel item)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            await Task.Run(() =>
            {
                imageBitmap = GetImageBitmapFromUrl(item.Image);
            }, cts.Token);

            if (imageBitmap != null)
            {
                item.ImageBitmap = imageBitmap;
                item.ImageB64 = BitmapToBase64(imageBitmap);
                RewardsEntity wtManager = new RewardsEntity();
                wtManager.UpdateCacheImage(item.ID, item.ImageB64);
                SetRewardImg(viewHolder, item);
            }
            else
            {
                viewHolder.rewardUsedLayout.Visibility = ViewStates.Gone;

                if (item.IsUsed)
                {
                    viewHolder.rewardUsedLayout.Visibility = ViewStates.Visible;
                    viewHolder.rewardImg.SetImageBitmap(ToGrayscale(this.mDefaultBitmap));
                }
                else
                {
                    viewHolder.rewardImg.SetImageBitmap(this.mDefaultBitmap);
                }

                if (item.IsSaved)
                {
                    viewHolder.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_saved);
                }
                else
                {
                    viewHolder.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_unsaved);
                }

                viewHolder.rewardMainShimmerImgLayout.Visibility = ViewStates.Gone;

                viewHolder.rewardMainImgLayout.Visibility = ViewStates.Visible;
            }
        }

        public void SetRewardImg(RewardViewHolder viewHolder, RewardsModel item)
        {
            try
            {
                viewHolder.rewardUsedLayout.Visibility = ViewStates.Gone;
                if (item.ImageBitmap != null)
                {
                    if (item.IsUsed)
                    {
                        item.ImageBitmap = ToGrayscale(item.ImageBitmap);
                        viewHolder.rewardUsedLayout.Visibility = ViewStates.Visible;
                    }
                    viewHolder.rewardImg.SetImageBitmap(item.ImageBitmap);
                }
                else if (!string.IsNullOrEmpty(item.ImageB64))
                {
                    item.ImageBitmap = Base64ToBitmap(item.ImageB64);
                    if (item.IsUsed)
                    {
                        item.ImageBitmap = ToGrayscale(item.ImageBitmap);
                        viewHolder.rewardUsedLayout.Visibility = ViewStates.Visible;
                    }
                    viewHolder.rewardImg.SetImageBitmap(item.ImageBitmap);
                }
                else if (!string.IsNullOrEmpty(item.Image))
                {
                    _ = GetImageAsync(viewHolder, item);
                    return;
                }
                else
                {
                    if (item.IsUsed)
                    {
                        viewHolder.rewardUsedLayout.Visibility = ViewStates.Visible;
                        viewHolder.rewardImg.SetImageBitmap(ToGrayscale(this.mDefaultBitmap));
                    }
                    else
                    {
                        viewHolder.rewardImg.SetImageBitmap(this.mDefaultBitmap);
                    }
                }

                if (item.IsSaved)
                {
                    viewHolder.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_saved);
                }
                else
                {
                    viewHolder.btnRewardSaveImg.SetImageResource(Resource.Drawable.ic_card_reward_unsaved);
                }

                viewHolder.rewardMainShimmerImgLayout.Visibility = ViewStates.Gone;
                if (viewHolder.shimmerRewardImageLayout.IsShimmerStarted)
                {
                    viewHolder.shimmerRewardImageLayout.StopShimmer();
                }

                viewHolder.rewardMainImgLayout.Visibility = ViewStates.Visible;

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Android.Graphics.Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap image = null;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        image = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return image;
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        public Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
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
            public ImageView rewardSaveImgShadow { get; private set; }

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

            // Used Layout, show when is used
            public LinearLayout rewardUsedLayout { get; private set; }

            // 500
            public TextView txtRewardUsed { get; private set; }



            public RewardViewHolder(View itemView, Action<RewardViewHolder, int> listener, Action<RewardViewHolder, int> saveListener) : base(itemView)
			{
                rewardCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);

                rewardMainImgLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.rewardMainImg);
                rewardImg = itemView.FindViewById<ImageView>(Resource.Id.rewardImg);
                btnRewardSaveImg = itemView.FindViewById<ImageView>(Resource.Id.btnRewardSaveImg);
                rewardSaveImgShadow = itemView.FindViewById<ImageView>(Resource.Id.rewardSaveImgShadow);

                rewardMainShimmerImgLayout = itemView.FindViewById<LinearLayout>(Resource.Id.rewardMainShimmerImgLayout);
                shimmerRewardImageLayout = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerRewardImageLayout);

                rewardBottomView = itemView.FindViewById<RelativeLayout>(Resource.Id.rewardBottomView);
                txtTitle = itemView.FindViewById<TextView>(Resource.Id.txtTitle);
                rewardUnreadImg = itemView.FindViewById<ImageView>(Resource.Id.rewardUnreadImg);

                rewardMainShimmerTxtLayout = itemView.FindViewById<LinearLayout>(Resource.Id.rewardMainShimmerTxtLayout);
                shimmerRewardTxtLayout = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerRewardTxtLayout);

                rewardUsedLayout = itemView.FindViewById<LinearLayout>(Resource.Id.rewardUsedLayout);
                txtRewardUsed = itemView.FindViewById<TextView>(Resource.Id.txtRewardUsed);

                txtRewardUsed.TextSize = TextViewUtils.GetFontSize(12f);
                txtTitle.TextSize = TextViewUtils.GetFontSize(12f);
            
                rewardCardView.Click += (s, e) => listener((this), base.LayoutPosition);
                btnRewardSaveImg.Click += (s, e) => saveListener((this), base.LayoutPosition);

            }
		}

	}
}
