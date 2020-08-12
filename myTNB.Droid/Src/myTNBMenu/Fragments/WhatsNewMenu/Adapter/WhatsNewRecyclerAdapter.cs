using Android.Graphics;


using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.Adapter
{
	public class WhatsNewRecyclerAdapter : RecyclerView.Adapter
	{

		List<WhatsNewModel> whatsNewList = new List<WhatsNewModel>();

		public event EventHandler<int> ClickChanged;

		private Android.App.Activity mActivity;

		private WHATSNEWITEMLISTMODE mListMode;

		private Bitmap mDefaultBitmap;

		public WhatsNewRecyclerAdapter(List<WhatsNewModel> data, Android.App.Activity Activity, WHATSNEWITEMLISTMODE listMode)
		{
			if (data == null)
			{
				this.whatsNewList.Clear();
			}
			else
			{
				this.whatsNewList = data;
			}
			this.mActivity = Activity;

			this.mListMode = listMode;

			BitmapFactory.Options opt = new BitmapFactory.Options();
			opt.InMutable = true;

			this.mDefaultBitmap = BitmapFactory.DecodeResource(this.mActivity.Resources, Resource.Drawable.promotions_default_image, opt);

		}

		public void RefreshList(List<WhatsNewModel> data)
		{
			if (data == null)
			{
				this.whatsNewList.Clear();
			}
			else
			{
				this.whatsNewList = data;
			}
			NotifyDataSetChanged();
		}

		public override int ItemCount => whatsNewList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			try
			{
                WhatsNewViewHolder vh = holder as WhatsNewViewHolder;

				WhatsNewModel model = whatsNewList[position];

				try
				{
					TextViewUtils.SetMuseoSans500Typeface(vh.txtTitle);

                    LinearLayout.LayoutParams currentShimmerImg = vh.whatsNewMainShimmerImgLayout.LayoutParameters as LinearLayout.LayoutParams;
					LinearLayout.LayoutParams currentMainImgLayout = vh.whatsNewMainImgLayout.LayoutParameters as LinearLayout.LayoutParams;
					int currentImgWidth = this.mActivity.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(16f) - (int)DPUtils.ConvertDPToPx(16f);
					float currentImgRatio = 112f / 288f;
					int currentImgHeight = (int)(currentImgWidth * currentImgRatio);
					currentShimmerImg.Height = currentImgHeight;
					currentMainImgLayout.Height = currentImgHeight;

					ViewGroup.LayoutParams currentCard = vh.whatsNewCardView.LayoutParameters;
					LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(currentCard.Width,
					currentCard.Height);
					layoutParams.LeftMargin = (int)DPUtils.ConvertDPToPx(13f);
					layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(13f);
					if (position == 0)
					{
						layoutParams.TopMargin = (int)DPUtils.ConvertDPToPx(16f);
						layoutParams.BottomMargin = (int)DPUtils.ConvertDPToPx(5f);
					}
					if ((position + 1) == whatsNewList.Count)
					{
						layoutParams.TopMargin = (int)DPUtils.ConvertDPToPx(5f);
						layoutParams.BottomMargin = (int)DPUtils.ConvertDPToPx(16f);
					}
					else
					{
						layoutParams.TopMargin = (int)DPUtils.ConvertDPToPx(5f);
						layoutParams.BottomMargin = (int)DPUtils.ConvertDPToPx(5f);
					}
					vh.whatsNewCardView.LayoutParameters = layoutParams;
				}
				catch (Exception e)
				{
					Utility.LoggingNonFatalError(e);
				}

				try
				{
					if (string.IsNullOrEmpty(whatsNewList[position].Image) || string.IsNullOrEmpty(whatsNewList[position].ImageB64))
					{
						// Image Shimmer Start
						if (string.IsNullOrEmpty(whatsNewList[position].Image))
						{
							if (mListMode == WHATSNEWITEMLISTMODE.INITIATE)
							{
								// Just Shimmer
								vh.whatsNewMainImgLayout.Visibility = ViewStates.Gone;
								vh.whatsNewMainShimmerImgLayout.Visibility = ViewStates.Visible;

								try
								{
									if (vh.shimmerWhatsNewImageLayout.IsShimmerStarted)
									{
										vh.shimmerWhatsNewImageLayout.StopShimmer();
									}
									var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
									if (shimmerBuilder != null)
									{
										vh.shimmerWhatsNewImageLayout.SetShimmer(shimmerBuilder?.Build());
									}
									vh.shimmerWhatsNewImageLayout.StartShimmer();
								}
								catch (Exception e)
								{
									Utility.LoggingNonFatalError(e);
								}
							}
							else
							{
                                vh.whatsNewImg.SetImageBitmap(this.mDefaultBitmap);

                                vh.whatsNewMainShimmerImgLayout.Visibility = ViewStates.Gone;

								vh.whatsNewMainImgLayout.Visibility = ViewStates.Visible;
							}
						}
						else
						{
							// Shimmer, Pull Image, then update back the image here
							vh.whatsNewMainImgLayout.Visibility = ViewStates.Gone;
							vh.whatsNewMainShimmerImgLayout.Visibility = ViewStates.Visible;

							try
							{
								if (vh.shimmerWhatsNewImageLayout.IsShimmerStarted)
								{
									vh.shimmerWhatsNewImageLayout.StopShimmer();
								}
								var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
								if (shimmerBuilder != null)
								{
									vh.shimmerWhatsNewImageLayout.SetShimmer(shimmerBuilder?.Build());
								}
								vh.shimmerWhatsNewImageLayout.StartShimmer();
							}
							catch (Exception e)
							{
								Utility.LoggingNonFatalError(e);
							}

							_ = GetImageAsync(vh, whatsNewList[position]);
						}
					}
					else
					{
						SetWhatsNewImg(vh, whatsNewList[position]);
					}
				}
				catch (Exception e)
				{
					Utility.LoggingNonFatalError(e);
				}

				try
				{
					if (string.IsNullOrEmpty(whatsNewList[position].TitleOnListing))
					{
						// Text Shimmer Start
						vh.whatsNewBottomView.Visibility = ViewStates.Gone;
						vh.whatsNewMainShimmerTxtLayout.Visibility = ViewStates.Visible;

						try
						{
							var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
							if (shimmerBuilder != null)
							{
								vh.shimmerWhatsNewTxtLayout.SetShimmer(shimmerBuilder?.Build());
							}
							vh.shimmerWhatsNewTxtLayout.StartShimmer();
						}
						catch (Exception e)
						{
							Utility.LoggingNonFatalError(e);
						}
					}
					else
					{
						// Text Shimmer Stop
						vh.whatsNewMainShimmerTxtLayout.Visibility = ViewStates.Gone;
						if (vh.shimmerWhatsNewTxtLayout.IsShimmerStarted)
						{
							vh.shimmerWhatsNewTxtLayout.StopShimmer();
						}

						vh.whatsNewBottomView.Visibility = ViewStates.Visible;
						vh.txtTitle.Text = whatsNewList[position].TitleOnListing;

                        bool isDateAvailable = false;

                        try
                        {
                            if (!string.IsNullOrEmpty(whatsNewList[position].PublishDate))
                            {
                                try
                                {
                                    DateTime publishDateTime = DateTime.ParseExact(whatsNewList[position].PublishDate, "yyyyMMddTHHmmss",
                                        CultureInfo.InvariantCulture, DateTimeStyles.None);

                                    if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                                    {
                                        CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                                        vh.txtDate.Text = publishDateTime.ToString("dd MMM yyyy", currCult);
                                        isDateAvailable = true;
                                    }
                                    else
                                    {
                                        CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                        vh.txtDate.Text = publishDateTime.ToString("dd MMM yyyy", currCult);
                                        isDateAvailable = true;
                                    }
                                }
                                catch (Exception e)
                                {
                                    vh.txtDate.Text = "";
                                    Utility.LoggingNonFatalError(e);
                                }
                            }
                            else
                            {
                                vh.txtDate.Text = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            vh.txtDate.Text = "";
                            Utility.LoggingNonFatalError(ex);
                        }

                        if (isDateAvailable)
                        {
                            vh.txtDate.Visibility = ViewStates.Visible;
                            if (whatsNewList[position].Read)
                            {
                                vh.whatsNewUnreadImg.Visibility = ViewStates.Gone;
                                TextViewUtils.SetMuseoSans300Typeface(vh.txtDate);
                                vh.txtDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.mActivity, Resource.Color.new_grey)));
                                RelativeLayout.LayoutParams txtDateParam = vh.txtDate.LayoutParameters as RelativeLayout.LayoutParams;
                                txtDateParam.RightMargin = (int)DPUtils.ConvertDPToPx(16f);
                                RelativeLayout.LayoutParams txtTitleParam = vh.txtTitle.LayoutParameters as RelativeLayout.LayoutParams;
                                txtTitleParam.RightMargin = (int)DPUtils.ConvertDPToPx(86f);
                            }
                            else
                            {
                                vh.whatsNewUnreadImg.Visibility = ViewStates.Visible;
                                TextViewUtils.SetMuseoSans500Typeface(vh.txtDate);
                                vh.txtDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.mActivity, Resource.Color.tunaGrey)));
                                RelativeLayout.LayoutParams txtDateParam = vh.txtDate.LayoutParameters as RelativeLayout.LayoutParams;
                                txtDateParam.RightMargin = (int)DPUtils.ConvertDPToPx(28f);
                                RelativeLayout.LayoutParams txtTitleParam = vh.txtTitle.LayoutParameters as RelativeLayout.LayoutParams;
                                txtTitleParam.RightMargin = (int)DPUtils.ConvertDPToPx(98f);
                            }
                        }
                        else
                        {
                            vh.txtDate.Visibility = ViewStates.Gone;
                            if (whatsNewList[position].Read)
                            {
                                vh.whatsNewUnreadImg.Visibility = ViewStates.Gone;
                                RelativeLayout.LayoutParams txtTitleParam = vh.txtTitle.LayoutParameters as RelativeLayout.LayoutParams;
                                txtTitleParam.RightMargin = (int)DPUtils.ConvertDPToPx(16f);
                            }
                            else
                            {
                                vh.whatsNewUnreadImg.Visibility = ViewStates.Visible;
                                RelativeLayout.LayoutParams txtTitleParam = vh.txtTitle.LayoutParameters as RelativeLayout.LayoutParams;
                                txtTitleParam.RightMargin = (int)DPUtils.ConvertDPToPx(34f);
                            }
                        }

                        vh.txtTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.mActivity, Resource.Color.powerBlue)));

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
			var id = Resource.Layout.WhatsNewCardLayout;
			var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
			return new WhatsNewViewHolder(itemView, OnClick);
		}

		void OnClick(WhatsNewViewHolder sender, int position)
		{
			try
			{
				if (position != -1)
				{
					WhatsNewModel targetItem = whatsNewList[position];
					if (targetItem != null)
					{
						if (!targetItem.Read)
						{
							sender.whatsNewUnreadImg.Visibility = ViewStates.Gone;
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

		public async Task GetImageAsync(WhatsNewViewHolder viewHolder, WhatsNewModel item)
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
                WhatsNewEntity wtManager = new WhatsNewEntity();
				wtManager.UpdateCacheImage(item.ID, item.ImageB64);
                SetWhatsNewImg(viewHolder, item);
			}
			else
			{
                viewHolder.whatsNewImg.SetImageBitmap(this.mDefaultBitmap);

                viewHolder.whatsNewMainShimmerImgLayout.Visibility = ViewStates.Gone;

				viewHolder.whatsNewMainImgLayout.Visibility = ViewStates.Visible;
			}
		}

		public void SetWhatsNewImg(WhatsNewViewHolder viewHolder, WhatsNewModel item)
		{
			try
			{
				if (item.ImageBitmap != null)
				{
					viewHolder.whatsNewImg.SetImageBitmap(item.ImageBitmap);
				}
				else if (!string.IsNullOrEmpty(item.ImageB64))
				{
					item.ImageBitmap = Base64ToBitmap(item.ImageB64);
					viewHolder.whatsNewImg.SetImageBitmap(item.ImageBitmap);
				}
				else if (!string.IsNullOrEmpty(item.Image))
				{
					_ = GetImageAsync(viewHolder, item);
					return;
				}
				else
				{
                    viewHolder.whatsNewImg.SetImageBitmap(this.mDefaultBitmap);
                }

				viewHolder.whatsNewMainShimmerImgLayout.Visibility = ViewStates.Gone;
				if (viewHolder.shimmerWhatsNewImageLayout.IsShimmerStarted)
				{
					viewHolder.shimmerWhatsNewImageLayout.StopShimmer();
				}

				viewHolder.whatsNewMainImgLayout.Visibility = ViewStates.Visible;

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

		public class WhatsNewViewHolder : RecyclerView.ViewHolder
		{
			// CardView, Enable click to see detail
			public CardView whatsNewCardView { get; private set; }

			// Main Image, enable when have img url and already pull
			public LinearLayout whatsNewMainImgLayout { get; private set; }

			// Main Image, set image here
			public ImageView whatsNewImg { get; private set; }

			// Image Shimmer Main Layout, enable when no img url / have img url but need pull
			public LinearLayout whatsNewMainShimmerImgLayout { get; private set; }

			// Start Shimmer if need shimmer on Image, Stop otherview
			public ShimmerFrameLayout shimmerWhatsNewImageLayout { get; private set; }

			// Main Txt, enable when have txt
			public RelativeLayout whatsNewBottomView { get; private set; }

			// Main Txt, insert Main Text Here, 500
			public TextView txtTitle { get; private set; }

            // Date Txt, insert Date Text Here, Unread 500 / Read 300
            public TextView txtDate { get; private set; }

            // What's New Unread Indicator, hide when already readed
            public ImageView whatsNewUnreadImg { get; private set; }

			// Txt Shimmer Main Layout, enable when no text
			public LinearLayout whatsNewMainShimmerTxtLayout { get; private set; }

			// Start Shimmer if need shimmer on Image, Stop otherview
			public ShimmerFrameLayout shimmerWhatsNewTxtLayout { get; private set; }



			public WhatsNewViewHolder(View itemView, Action<WhatsNewViewHolder, int> listener) : base(itemView)
			{
                whatsNewCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);

                whatsNewMainImgLayout = itemView.FindViewById<LinearLayout>(Resource.Id.whatsNewMainImg);
                whatsNewImg = itemView.FindViewById<ImageView>(Resource.Id.whatsNewImg);

                whatsNewMainShimmerImgLayout = itemView.FindViewById<LinearLayout>(Resource.Id.whatsNewMainShimmerImgLayout);
                shimmerWhatsNewImageLayout = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerWhatsNewImageLayout);

                whatsNewBottomView = itemView.FindViewById<RelativeLayout>(Resource.Id.whatsNewBottomView);
				txtTitle = itemView.FindViewById<TextView>(Resource.Id.txtTitle);
                txtDate = itemView.FindViewById<TextView>(Resource.Id.txtDate);
                whatsNewUnreadImg = itemView.FindViewById<ImageView>(Resource.Id.whatsNewUnreadImg);

                whatsNewMainShimmerTxtLayout = itemView.FindViewById<LinearLayout>(Resource.Id.whatsNewMainShimmerTxtLayout);
                shimmerWhatsNewTxtLayout = itemView.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerWhatsNewTxtLayout);

                whatsNewCardView.Click += (s, e) => listener((this), base.LayoutPosition);

			}
		}

	}
}
