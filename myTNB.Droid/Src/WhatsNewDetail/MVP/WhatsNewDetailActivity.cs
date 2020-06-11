﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Firebase.DynamicLinks;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
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

        WhatsNewDetailContract.IWhatsNewDetailPresenter presenter;

		private WhatsNewModel LocalItem = new WhatsNewModel();

		private string ItemID = "";

		private string Title = "What’s New";

		private IMenu menu;

		private bool linkGenerationSuccessful = false;

		private string generatedLink = "";

		private Snackbar mNoInternetSnackbar;

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
                            SetupImageParam();
                            this.presenter.GetActiveWhatsNew(ItemID);
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

		public void SetWhatsNewDetail(WhatsNewModel item)
		{
			try
			{
				if (item != null)
				{
					LocalItem = item;
					txtTitle.Text = item.Title;
                    txtDescription.TextFormatted = GetFormattedText(item.Description);

					if (item.Description != null && (item.Description.Contains("<img")))
                    {
						this.presenter.ExtractImage(item.Description);
					}

					if (item.Description != null && (item.Description.Contains("http") || item.Description.Contains("www.")))
					{
						SpannableString s = new SpannableString(txtDescription.TextFormatted);

						var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));

						List<string> extractedUrls = this.presenter.ExtractUrls(item.Description);

						if (urlSpans != null && urlSpans.Length > 0 && extractedUrls != null && extractedUrls.Count > 0)
						{
							for (int i = 0; i < urlSpans.Length; i++)
							{
								URLSpan URLItem = urlSpans[i] as URLSpan;
								string searchedString = extractedUrls.Find(x => x == URLItem.URL);
								if (!string.IsNullOrEmpty(searchedString))
								{
									int startIndex = s.GetSpanStart(urlSpans[i]);
									int endIndex = s.GetSpanEnd(urlSpans[i]);
									s.RemoveSpan(urlSpans[i]);
									ClickSpan clickableSpan = new ClickSpan()
									{
										textColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.powerBlue)),
										typeFace = Typeface.CreateFromAsset(this.Assets, "fonts/" + TextViewUtils.MuseoSans500)
									};
									clickableSpan.Click += v =>
									{
										OnClickSpan(searchedString);
									};
									s.SetSpan(clickableSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
								}
							}
						}
                        txtDescription.TextFormatted = s;
                        txtDescription.MovementMethod = new LinkMovementMethod();
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

		class ClickSpan : ClickableSpan
		{
			public Action<View> Click;
			public Color textColor { get; set; }
			public Typeface typeFace { get; set; }

			public override void OnClick(View widget)
			{
				if (Click != null)
				{
					Click(widget);
				}
			}

			public override void UpdateDrawState(TextPaint ds)
			{
				base.UpdateDrawState(ds);
				ds.Color = textColor;
				ds.SetTypeface(typeFace);
				ds.UnderlineText = false;
			}
		}

		public void OnClickSpan(string url)
		{
			try
			{
				HideNoInternetSnackbar();

				if (!string.IsNullOrEmpty(url) && (url.Contains("http") || url.Contains("www.")))
				{
					if (!this.GetIsClicked())
					{
						this.SetIsClicked(true);

						if (!url.Contains("http"))
						{
							url = "http://" + url;
						}

						Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
						webIntent.PutExtra(Constants.IN_APP_LINK, url);
						webIntent.PutExtra(Constants.IN_APP_TITLE, Title);
						StartActivity(webIntent);
					}
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
