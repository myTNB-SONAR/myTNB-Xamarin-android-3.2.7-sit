using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;


using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Com.Davemorrissey.Labs.Subscaleview;
using DynatraceAndroid;
using Facebook.Shimmer;
using Firebase.DynamicLinks;
using Google.Android.Material.Snackbar;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.PDFView;
using myTNB_Android.Src.Utils.ZoomImageView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime;

namespace myTNB_Android.Src.WhatsNewDetail.MVP
{
	[Activity(Label = "What’s New"
              , Icon = "@drawable/ic_launcher"
	  , ScreenOrientation = ScreenOrientation.Portrait
	  , Theme = "@style/Theme.Dashboard")]
	public class WhatsNewDetailActivity : BaseToolbarAppCompatActivity, WhatsNewDetailContract.IWhatsNewDetaillView, Android.Gms.Tasks.IOnSuccessListener, Android.Gms.Tasks.IOnFailureListener, Android.Gms.Tasks.IOnCompleteListener
	{
		[BindView(Resource.Id.rootView)]
		CoordinatorLayout rootView;

		[BindView(Resource.Id.whatsNewMainImg)]
        LinearLayout whatsNewMainImg;

		[BindView(Resource.Id.whatsNewImg)]
		ImageView whatsNewImg;

        [BindView(Resource.Id.shimmerWhatsNewImageLayout)]
        ShimmerFrameLayout shimmerWhatsNewImageLayout;

        [BindView(Resource.Id.whatsNewMainShimmerImgLayout)]
		LinearLayout whatsNewMainShimmerImgLayout;

		[BindView(Resource.Id.txtTitle)]
		TextView txtTitle;

		[BindView(Resource.Id.txtDescription)]
		TextView txtDescription;

		[BindView(Resource.Id.txtFooter)]
		TextView txtFooter;

		[BindView(Resource.Id.whatsNewNormalDetailLayout)]
		LinearLayout whatsNewNormalDetailLayout;

		[BindView(Resource.Id.whatsNewFullScreenShimmerLayout)]
		LinearLayout whatsNewFullScreenShimmerLayout;

		[BindView(Resource.Id.shimmerFullScreenLayout)]
		ShimmerFrameLayout shimmerFullScreenLayout;

		[BindView(Resource.Id.whatsNewFullImageDetailLayout)]
		LinearLayout whatsNewFullImageDetailLayout;

		[BindView(Resource.Id.imgFullView)]
		ZoomImageView imgFullView;

		[BindView(Resource.Id.whatsNewFullPDFDetailLayout)]
		LinearLayout whatsNewFullPDFDetailLayout;

		[BindView(Resource.Id.pdfFullView)]
		PDFView pdfFullView;

		WhatsNewDetailContract.IWhatsNewDetailPresenter presenter;

		private WhatsNewModel LocalItem = new WhatsNewModel();

		private string ItemID = "";

		private string Title = "What’s New";

		private IMenu menu;

		private bool linkGenerationSuccessful = false;

		private string generatedLink = "";

		private Snackbar mNoInternetSnackbar;

		private bool fullScreenFirstLoaded = false;

		private IDTXAction DynAction;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			try
			{
				TextViewUtils.SetMuseoSans500Typeface(txtTitle);
				TextViewUtils.SetMuseoSans300Typeface(txtDescription, txtFooter);
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}

			try
			{
				presenter = new WhatsNewDetailPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}

			try
			{
				fullScreenFirstLoaded = false;

				Bundle extras = Intent.Extras;

				if (extras != null)
				{
					if (extras.ContainsKey(Constants.WHATS_NEW_DETAIL_TITLE_KEY) && !string.IsNullOrEmpty(extras.GetString(Constants.WHATS_NEW_DETAIL_TITLE_KEY)))
					{
						SetToolBarTitle(extras.GetString(Constants.WHATS_NEW_DETAIL_TITLE_KEY));
						Title = extras.GetString(Constants.WHATS_NEW_DETAIL_TITLE_KEY);
					}

					if (extras.ContainsKey(Constants.WHATS_NEW_DETAIL_ITEM_KEY) && !string.IsNullOrEmpty(extras.GetString(Constants.WHATS_NEW_DETAIL_ITEM_KEY)))
					{
						ItemID = extras.GetString(Constants.WHATS_NEW_DETAIL_ITEM_KEY);
					}
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void dynaAction(string eventName)
        {
			this.DynAction = DynatraceAndroid.Dynatrace.EnterAction(eventName);
		
		}

		protected override void OnStart()
		{
			base.OnStart();

            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(ItemID))
                        {
                            this.Finish();
                        }
                        else
                        {
							if (!fullScreenFirstLoaded)
                            {
								whatsNewNormalDetailLayout.Visibility = ViewStates.Visible;
								whatsNewFullScreenShimmerLayout.Visibility = ViewStates.Gone;
								whatsNewFullImageDetailLayout.Visibility = ViewStates.Gone;
								whatsNewFullPDFDetailLayout.Visibility = ViewStates.Gone;
								SetupImageParam();
								this.presenter.GetActiveWhatsNew(ItemID);
							}
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
			{
				SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
				SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
			}
			catch (System.Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public override void OnBackPressed()
		{
			this.DynAction.LeaveAction();
			this.DynAction.Dispose();
			base.OnBackPressed();
		}

		public override Boolean ShowCustomToolbarTitle()
		{
			return true;
		}

		public bool IsActive()
		{
			return Window.DecorView.RootView.IsShown;
		}

		public override int ResourceId()
		{
			return Resource.Layout.WhatsNewDetailLayout;
		}

		protected override void OnResume()
		{
			base.OnResume();
			try
			{
				FirebaseAnalyticsUtils.SetScreenName(this, Title);
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

		public override void OnTrimMemory(TrimMemory level)
		{
			base.OnTrimMemory(level);

			switch (level)
			{
				case TrimMemory.RunningLow:
					GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
					GC.Collect();
					break;
				default:
					GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
					GC.Collect();
					break;
			}
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.PromotionDetailMenu, menu);
			this.menu = menu;
			return base.OnCreateOptionsMenu(menu);
		}

		protected override void OnDestroy()
        {
            base.OnDestroy();
        }

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_share_promotion:
					if (!this.GetIsClicked())
					{
						this.SetIsClicked(true);
						HideNoInternetSnackbar();

						if (ConnectionUtils.ChceckInternetConnection(this))
						{
							if (LocalItem != null)
							{
								if (!string.IsNullOrEmpty(generatedLink))
								{
									Intent shareIntent = new Intent(Intent.ActionSend);
									shareIntent.SetType("text/plain");
									shareIntent.PutExtra(Intent.ExtraSubject, LocalItem.Title);
									shareIntent.PutExtra(Intent.ExtraText, generatedLink);
									StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
									this.SetIsClicked(false);
								}
								else
								{
									string ID = LocalItem.ID;
									ID = ID.Replace("{", "");
									ID = ID.Replace("}", "");
									string deepLinkUrl = Constants.SERVER_URL.END_POINT + "/whatsnew/redirect.aspx/wnid=" + ID;

									ShowProgressDialog();
									String buildLink = new LinkBuilder().setDomain(Constants.SERVER_URL.FIREBASE_DEEP_LINK_END_POINT)
									   .setLink(deepLinkUrl)
									   .setApn(ApplicationContext.PackageName)
									   .setAmv(Constants.DYNAMIC_LINK_ANDROID_MIN_VER_CODE.ToString())
									   .setIbi(ApplicationContext.PackageName)
									   .setImv(Constants.DYNAMIC_LINK_IOS_MIN_VER_CODE)
									   .setIsi(Constants.DYNAMIC_LINK_IOS_APP_ID)
									   .build();

									FirebaseDynamicLinks.Instance.CreateDynamicLink()
										.SetLongLink(Android.Net.Uri.Parse(buildLink))
										.BuildShortDynamicLink()
										.AddOnSuccessListener(this, this)
										.AddOnFailureListener(this, this)
										.AddOnCompleteListener(this, this);
								}
							}
						}
						else
						{
							this.SetIsClicked(false);
							ShowNoInternetSnackbar();
						}
					}
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}

		public void UpdateWhatsNewDetail(WhatsNewModel item)
        {
			try
            {
				if (item != null)
                {
					LocalItem = item;
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void SetWhatsNewDetail(WhatsNewModel item)
		{
			try
			{
				if (item != null)
				{
					LocalItem = item;
					txtTitle.Text = item.Title;
                    txtDescription.TextFormatted = GetFormattedTextNoExtraSpacing(item.Description);

					if (item.Description != null)
					{
						txtDescription = LinkRedirectionUtils
							.Create(this, Title)
							.SetTextView(txtDescription)
							.SetMessage(item.Description)
							.SetAction(HideNoInternetSnackbar)
							.Build()
							.GetProcessedTextView();
					}

					if (item.Description != null && (item.Description.Contains("<img")))
					{
						if (!string.IsNullOrEmpty(item.Description_Images))
                        {
							try
                            {
								List<WhatsNewDetailImageDBModel> dbList = JsonConvert.DeserializeObject<List<WhatsNewDetailImageDBModel>>(item.Description_Images);

								if (dbList.Count > 0)
                                {
									_ = this.presenter.ProcessWhatsNewDetailImage(dbList);
									return;
								}
							}
							catch (Exception ex)
							{
								Utility.LoggingNonFatalError(ex);
							}
                        }

						List<WhatsNewDetailImageModel> containedImage = this.presenter.ExtractImage(item.Description);
						if (containedImage.Count > 0)
						{
							try
                            {
								SpannableString whatsNewDetailString = new SpannableString(txtDescription.TextFormatted);
								var imageSpans = whatsNewDetailString.GetSpans(0, whatsNewDetailString.Length(), Java.Lang.Class.FromType(typeof(ImageSpan)));
								if (imageSpans != null && imageSpans.Length > 0)
								{
									List<Bitmap> mShimmerBitmapList = new List<Bitmap>();
									Drawable mShimmerDrawable = ContextCompat.GetDrawable(this, Resource.Drawable.shimmer_rectangle);
									string urlHeightWidthRegex = "(<img\\b|(?!^)\\G)[^>]*?\\b(src|width|height)=([\"']?)([^\"]*)\\3";
									System.Text.RegularExpressions.MatchCollection matcheImgSrc = System.Text.RegularExpressions.Regex.Matches(item.Description, urlHeightWidthRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
									bool foundWidth = false;
									bool foundHeight = false;
									float textImgWidth = 0;
									float textImgHeight = 0;
									for (int index = 0; index < matcheImgSrc.Count; index++)
									{
										if (matcheImgSrc[index].Groups[2].Value == "width")
										{
											if (foundHeight)
											{
												textImgWidth = float.Parse(matcheImgSrc[index].Groups[4].Value);
												float deviceWidth = this.Resources.DisplayMetrics.WidthPixels - DPUtils.ConvertDPToPx(36f);
												float calImgRatio = deviceWidth / textImgWidth;
												float deviceHeight = textImgHeight * calImgRatio;

												mShimmerBitmapList.Add(BitmapUtils.CreateBitmapFromDrawable(mShimmerDrawable, (int)deviceWidth, (int)deviceHeight));

												foundHeight = false;
											}
											else
											{
												foundWidth = true;
												textImgWidth = float.Parse(matcheImgSrc[index].Groups[4].Value);
											}
										}
										else if (matcheImgSrc[index].Groups[2].Value == "height")
										{
											if (foundWidth)
											{
												textImgHeight = float.Parse(matcheImgSrc[index].Groups[4].Value);
												float deviceWidth = this.Resources.DisplayMetrics.WidthPixels - DPUtils.ConvertDPToPx(36f);
												float calImgRatio = deviceWidth / textImgWidth;
												float deviceHeight = textImgHeight * calImgRatio;

												mShimmerBitmapList.Add(BitmapUtils.CreateBitmapFromDrawable(mShimmerDrawable, (int)deviceWidth, (int)deviceHeight));

												foundWidth = false;
											}
											else
											{
												foundHeight = true;
												textImgHeight = float.Parse(matcheImgSrc[index].Groups[4].Value);
											}
										}
									}

									for (int j = 0; j < imageSpans.Length; j++)
									{
										ImageSpan imageSpan = new ImageSpan(this, mShimmerBitmapList[j], SpanAlign.Baseline);
										ImageSpan ImageItem = imageSpans[j] as ImageSpan;
										int startIndex = whatsNewDetailString.GetSpanStart(imageSpans[j]);
										int endIndex = whatsNewDetailString.GetSpanEnd(imageSpans[j]);
										whatsNewDetailString.RemoveSpan(imageSpans[j]);
										whatsNewDetailString.SetSpan(imageSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
									}

									txtDescription.TextFormatted = whatsNewDetailString;
								}
							}
							catch (Exception ex)
							{
								Utility.LoggingNonFatalError(ex);
							}

							_ = this.presenter.FetchWhatsNewDetailImage(containedImage);
						}
					}
				}
				else
				{
					this.Finish();
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void SetWhatsNewDetailImage(List<WhatsNewDetailImageModel> containedImage)
		{
			try
			{
				SpannableString whatsNewDetailString = new SpannableString(txtDescription.TextFormatted);

				var imageSpans = whatsNewDetailString.GetSpans(0, whatsNewDetailString.Length(), Java.Lang.Class.FromType(typeof(ImageSpan)));

				if (imageSpans != null && imageSpans.Length > 0)
				{
					for (int j = 0; j < imageSpans.Length; j++)
					{
						if (containedImage[j].ExtractedImageBitmap != null)
                        {
							float currentImgWidth = this.Resources.DisplayMetrics.WidthPixels - DPUtils.ConvertDPToPx(36f);
							float calImgRatio = currentImgWidth / containedImage[j].ExtractedImageBitmap.Width;
							int currentImgHeight = (int) (containedImage[j].ExtractedImageBitmap.Height * calImgRatio);
							ImageSpan imageSpan = new ImageSpan(this, Bitmap.CreateScaledBitmap(containedImage[j].ExtractedImageBitmap, (int) currentImgWidth, currentImgHeight, false), SpanAlign.Baseline);
							ImageSpan ImageItem = imageSpans[j] as ImageSpan;
							int startIndex = whatsNewDetailString.GetSpanStart(imageSpans[j]);
							int endIndex = whatsNewDetailString.GetSpanEnd(imageSpans[j]);
							whatsNewDetailString.RemoveSpan(imageSpans[j]);
							whatsNewDetailString.SetSpan(imageSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
						}
					}

					txtDescription.TextFormatted = whatsNewDetailString;
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void HideWhatsNewDetailImage()
        {
			try
            {
				whatsNewMainShimmerImgLayout.Visibility = ViewStates.Gone;
				if (shimmerWhatsNewImageLayout.IsShimmerStarted)
				{
					shimmerWhatsNewImageLayout.StopShimmer();
				}

				whatsNewImg.Visibility = ViewStates.Gone;
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void SetWhatsNewImage(Bitmap imgSrc)
		{
			try
			{
				if (imgSrc == null)
				{
					BitmapFactory.Options opt = new BitmapFactory.Options();
					opt.InMutable = true;

					Bitmap mDefaultBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.promotions_default_image, opt);

                    whatsNewImg.SetImageBitmap(mDefaultBitmap);
				}
				else if (imgSrc != null)
				{
					LocalItem.ImageBitmap = imgSrc;
                    whatsNewImg.SetImageBitmap(imgSrc);
				}
				else
				{
					this.presenter.FetchWhatsNewImage(LocalItem);
					return;
				}

				whatsNewMainShimmerImgLayout.Visibility = ViewStates.Gone;
				if (shimmerWhatsNewImageLayout.IsShimmerStarted)
				{
                    shimmerWhatsNewImageLayout.StopShimmer();
				}

                whatsNewMainImg.Visibility = ViewStates.Visible;
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		private void SetupImageParam()
		{
			try
			{
				LinearLayout.LayoutParams currentShimmerImg = whatsNewMainShimmerImgLayout.LayoutParameters as LinearLayout.LayoutParams;
				LinearLayout.LayoutParams currentMainImgLayout = whatsNewMainImg.LayoutParameters as LinearLayout.LayoutParams;
				int currentImgWidth = this.Resources.DisplayMetrics.WidthPixels;
				float currentImgRatio = 180f / 320f;
				int currentImgHeight = (int)(currentImgWidth * currentImgRatio);
				currentShimmerImg.Height = currentImgHeight;
				currentMainImgLayout.Height = currentImgHeight;

				if (shimmerWhatsNewImageLayout.IsShimmerStarted)
				{
                    shimmerWhatsNewImageLayout.StopShimmer();
				}
				var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
				if (shimmerBuilder != null)
				{
                    shimmerWhatsNewImageLayout.SetShimmer(shimmerBuilder?.Build());
				}
                shimmerWhatsNewImageLayout.StartShimmer();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void SetupFullScreenShimmer()
        {
			try
			{
				whatsNewNormalDetailLayout.Visibility = ViewStates.Gone;
				whatsNewFullScreenShimmerLayout.Visibility = ViewStates.Visible;

				if (shimmerFullScreenLayout.IsShimmerStarted)
				{
					shimmerFullScreenLayout.StopShimmer();
				}
				var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
				if (shimmerBuilder != null)
				{
					shimmerFullScreenLayout.SetShimmer(shimmerBuilder?.Build());
				}
				shimmerFullScreenLayout.StartShimmer();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void StopFullScreenShimmer()
		{
			try
			{
				whatsNewFullScreenShimmerLayout.Visibility = ViewStates.Gone;
				if (shimmerFullScreenLayout.IsShimmerStarted)
				{
					shimmerFullScreenLayout.StopShimmer();
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void ShowProgressDialog()
		{
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

		public void HideProgressDialog()
		{
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

		void Android.Gms.Tasks.IOnSuccessListener.OnSuccess(Java.Lang.Object result)
		{
			linkGenerationSuccessful = true;
		}

		void Android.Gms.Tasks.IOnCompleteListener.OnComplete(Android.Gms.Tasks.Task task)
		{
			HideProgressDialog();
			this.SetIsClicked(false);
			if (linkGenerationSuccessful)
			{
				linkGenerationSuccessful = false;

				var pendingResult = task.Result.JavaCast<IShortDynamicLink>();

				Android.Net.Uri deepLink = null;
				if (pendingResult != null)
				{
					deepLink = pendingResult.ShortLink;
					string deepLinkUrl = deepLink.ToString();
					if (!string.IsNullOrEmpty(deepLinkUrl))
					{
						generatedLink = deepLinkUrl;
						Intent shareIntent = new Intent(Intent.ActionSend);
						shareIntent.SetType("text/plain");
						shareIntent.PutExtra(Intent.ExtraSubject, LocalItem.Title);
						shareIntent.PutExtra(Intent.ExtraText, deepLinkUrl);
						StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
					}
				}
			}
			else
			{
				string ID = LocalItem.ID;
				ID = ID.Replace("{", "");
				ID = ID.Replace("}", "");
				string deepLinkUrl = Constants.SERVER_URL.END_POINT + "/whatsnew/redirect.aspx/wnid=" + ID;

				generatedLink = deepLinkUrl;
				Intent shareIntent = new Intent(Intent.ActionSend);
				shareIntent.SetType("text/plain");
				shareIntent.PutExtra(Intent.ExtraSubject, LocalItem.Title);
				shareIntent.PutExtra(Intent.ExtraText, deepLinkUrl);
				StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
			}
		}

		void Android.Gms.Tasks.IOnFailureListener.OnFailure(Java.Lang.Exception e)
		{
			linkGenerationSuccessful = false;
			Utility.LoggingNonFatalError(e);
		}

		public void ShowNoInternetSnackbar()
		{
			try
			{
				HideNoInternetSnackbar();

				mNoInternetSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedLabel("Error", "noDataConnectionMessage"), Snackbar.LengthIndefinite)
				.SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate
				{

					mNoInternetSnackbar.Dismiss();
				}
				);
				mNoInternetSnackbar.Show();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void HideNoInternetSnackbar()
		{
			try
			{
				if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
				{
					mNoInternetSnackbar.Dismiss();
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public string GetLocalItemID()
        {
			return LocalItem.ID;
		}

		public void OnUpdateFullScreenPdf(string path)
		{
			try
			{
				RunOnUiThread(() =>
				{
					try
					{
						fullScreenFirstLoaded = true;
						StopFullScreenShimmer();
						whatsNewFullPDFDetailLayout.Visibility = ViewStates.Visible;

						Java.IO.File file = new Java.IO.File(path);

						pdfFullView
							.FromFile(file)
							.Show();
					}
					catch (Exception ex)
					{
						Utility.LoggingNonFatalError(ex);
					}
				});
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void OnUpdateFullScreenImage(Bitmap fullBitmap)
        {
			try
			{
				RunOnUiThread(() =>
				{
					try
					{
						fullScreenFirstLoaded = true;
						StopFullScreenShimmer();
						whatsNewFullImageDetailLayout.Visibility = ViewStates.Visible;

						imgFullView
							.FromBitmap(fullBitmap)
							.Show();
					}
					catch (Exception ex)
					{
						Utility.LoggingNonFatalError(ex);
					}
				});
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public string GenerateTmpFilePath()
        {
			string path = "";
			try
            {
				string rootPath = this.FilesDir.AbsolutePath;

				if (Utils.FileUtils.IsExternalStorageReadable() && Utils.FileUtils.IsExternalStorageWritable())
				{
					rootPath = this.GetExternalFilesDir(null).AbsolutePath;
				}

				var directory = System.IO.Path.Combine(rootPath, "pdf");
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				string filename = "tmpWhatNew.pdf";
				path = System.IO.Path.Combine(directory, filename);
			}
			catch (Exception e)
            {
				Utility.LoggingNonFatalError(e);
            }
			return path;
        }

		public class LinkBuilder
		{
			private String domain;
			private String link;
			private String apn;
			private String amv;

			private String ibi;
			private String imv;
			private String isi;


			private String st;
			private String sd;
			private String si;

			public String getURLEncode(String input)
			{

				try
				{
					return Java.Net.URLEncoder.Encode(input, "UTF-8");
				}
				catch (Exception ex)
				{
					if (String.IsNullOrEmpty(input))
					{
						input = "";
					}
					Utility.LoggingNonFatalError(ex);
				}

				return input;
			}


			// https://mytnbdev.page.link
			// https://mytnbsit.page.link
			// https://mytnb.page.link
			public LinkBuilder setDomain(String domain)
			{
				this.domain = domain;
				return this;
			}

            // Constants.SERVER_URL.END_POINT + "/whatsnew/redirect.aspx/wnid=" + ID
            public LinkBuilder setLink(String link)
			{
				this.link = getURLEncode(link);
				return this;
			}

			// Android Package Name, for our case com.mytnb.mytnb
			public LinkBuilder setApn(String apn)
			{
				this.apn = apn;
				return this;
			}

			// Android Min Version Code Suport,
			// for our testing now let's put 169,
			// but before creating build need to set to the build before the current available SIT build
			public LinkBuilder setAmv(String amv)
			{
				this.amv = amv;
				return this;
			}

			// iOS Package Name, for our case com.mytnb.mytnb
			public LinkBuilder setIbi(String ibi)
			{
				this.ibi = ibi;
				return this;
			}

			// iOS Min Version Code Suport
			// 2.1.0
			public LinkBuilder setImv(String imv)
			{
				this.imv = imv;
				return this;
			}

			// iOS App Store ID, now is unclear
			// Need it to do the redirection to App Store
			public LinkBuilder setIsi(String isi)
			{
				this.isi = isi;
				return this;
			}

			// Social Post Title when been shared
			public LinkBuilder setSt(String st)
			{
				this.st = getURLEncode(st);
				return this;
			}

			// Social Post Descriptiom when been shared
			public LinkBuilder setSd(String sd)
			{
				this.sd = getURLEncode(sd);
				return this;
			}

			// Social Post Image when been shared
			public LinkBuilder setSi(String si)
			{
				this.si = getURLEncode(si);
				return this;
			}

			public String build()
			{
				return String.Format("{0}/?link={1}&apn={2}&amv={3}&ibi={4}&imv={5}&isi={6}&st={7}&sd={8}&si={9}"
						, domain, link, apn, amv, ibi, imv, isi, st, sd, si);
			}
		}
	}
}
