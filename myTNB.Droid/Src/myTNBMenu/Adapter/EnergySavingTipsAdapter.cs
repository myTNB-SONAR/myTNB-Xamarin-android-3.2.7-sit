using Android.Graphics;

using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Adapter
{
	public class EnergySavingTipsAdapter : RecyclerView.Adapter
	{

		List<EnergySavingTipsModel> energyList = new List<EnergySavingTipsModel>();

		public event EventHandler<int> ClickChanged;

		private Android.App.Activity mActivity;

		public EnergySavingTipsAdapter(List<EnergySavingTipsModel> data, Android.App.Activity Activity)
		{
			if (data == null)
			{
				this.energyList.Clear();
			}
			else
			{
				this.energyList = data;
			}
			this.mActivity = Activity;
		}

		public override int ItemCount => energyList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
            EnergySavingTipsViewHolder vh = holder as EnergySavingTipsViewHolder;

            EnergySavingTipsModel model = energyList[position];

			try
			{
				vh.Title.Text = model.Title;

				if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
				{
					vh.Message.TextFormatted = Html.FromHtml(model.Description, FromHtmlOptions.ModeLegacy);
				}
				else
				{
					vh.Message.TextFormatted = Html.FromHtml(model.Description);
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}

			try
			{
                if (model.ImageBitmap != null)
                {
                    vh.Image.SetImageBitmap(model.ImageBitmap);
                }
                else
                {
                    _ = OnProcessEnergyTipsBitMap(vh, model);
                }

				TextViewUtils.SetMuseoSans500Typeface(vh.Title);
				TextViewUtils.SetMuseoSans300Typeface(vh.Message);

				ViewGroup.LayoutParams currentCard = vh.cardView.LayoutParameters;

				int cardWidth = (int)((this.mActivity.Resources.DisplayMetrics.WidthPixels / 1.07) - DPUtils.ConvertDPToPx(6f));
                if (DPUtils.ConvertPxToDP(cardWidth) < 288f)
                {
                    cardWidth = (int) DPUtils.ConvertDPToPx(288f);
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
            }
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var id = Resource.Layout.EnergySavingTipsCardLayout;
			var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
			return new EnergySavingTipsViewHolder(itemView, OnClick);
		}

		void OnClick(EnergySavingTipsViewHolder sender, int position)
		{
			try
			{
				// ClickChanged(this, position);
			}
			catch (System.Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}


		public class EnergySavingTipsViewHolder : RecyclerView.ViewHolder
		{

			public TextView Message { get; private set; }

			public TextView Title { get; private set; }

			public CardView cardView { get; private set; }

			public ImageView Image { get; private set; }

			public EnergySavingTipsViewHolder(View itemView, Action<EnergySavingTipsViewHolder, int> listener) : base(itemView)
			{
				Message = itemView.FindViewById<TextView>(Resource.Id.txtMessage);
				Title = itemView.FindViewById<TextView>(Resource.Id.txtTitle);
				cardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
                Image = itemView.FindViewById<ImageView>(Resource.Id.imgTips);

				cardView.Click += (s, e) => listener((this), base.LayoutPosition);
			}
		}

        private async Task OnProcessEnergyTipsBitMap(EnergySavingTipsViewHolder vh, EnergySavingTipsModel processModel)
        {
            try
            {
                Bitmap img = await GetPhoto(processModel.Image);
                vh.Image.SetImageBitmap(img);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private static async Task<Bitmap> GetPhoto(string imageUrl)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            try
            {
                await Task.Run(() =>
                {
                    imageBitmap = ImageUtils.GetImageBitmapFromUrl(imageUrl);
                }, cts.Token);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return imageBitmap;
        }

    }
}
