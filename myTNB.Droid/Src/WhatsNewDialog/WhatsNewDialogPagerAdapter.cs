using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using Java.Util.Regex;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.WhatsNewDialog
{
    public class WhatsNewDialogPagerAdapter : PagerAdapter
    {
        private Context mContext;
        private List<WhatsNewModel> whatsnew = new List<WhatsNewModel>();
        public event EventHandler<int> DetailsClicked;
        public event EventHandler<int> CloseClicked;
        public event EventHandler<int> RefreshIndicator;
        private bool isTextOnly = false;
        private bool isPhotoOnly = true;
        private RewardServiceImpl mApi;

        public WhatsNewDialogPagerAdapter(Context ctx, List<WhatsNewModel> items)
        {
            this.mContext = ctx;
            this.whatsnew = items;
            this.mApi = new RewardServiceImpl();
        }

        public WhatsNewDialogPagerAdapter()
        {

        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            WhatsNewModel model = whatsnew[position];

            ViewGroup rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.WhatsNewPagerItemLayout, container, false);

            if (!string.IsNullOrEmpty(model.PortraitImage_PopUp))
            {
                FrameLayout whatsNewDialogCardView = (FrameLayout)rootView.FindViewById(Resource.Id.layout_image_holder);
                Button btnGotIt = (Button)rootView.FindViewById(Resource.Id.btnWhatsNewGotIt);
                ImageView imgWhatsNew = (ImageView)rootView.FindViewById(Resource.Id.image_whatsnew);
                LinearLayout whatsNewCheckBoxLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewCheckBoxLayout);
                CheckBox chkDontShow = (CheckBox)rootView.FindViewById(Resource.Id.chk_remember_me);

                LinearLayout whatsNewMainImgLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewMainShimmerImgLayout);
                ShimmerFrameLayout shimmerWhatsNewImageLayout = (ShimmerFrameLayout)rootView.FindViewById(Resource.Id.shimmerWhatsNewImageLayout);

                int maxHeight = GetDeviceVerticalScaleInPixel(0.732f);

                if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                {
                    maxHeight = GetDeviceVerticalScaleInPixel(0.632f);
                }

                whatsNewDialogCardView.LayoutParameters.Height = maxHeight;
                whatsNewDialogCardView.RequestLayout();
                btnGotIt.RequestLayout();
                rootView.RequestLayout();

                TextViewUtils.SetMuseoSans500Typeface(btnGotIt, chkDontShow);
                chkDontShow.Text = Utility.GetLocalizedCommonLabel("dontShowThisAgain");

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

                if (!string.IsNullOrEmpty(model.PortraitImage_PopUpB64))
                {
                    Bitmap localBitmap = Base64ToBitmap(model.PortraitImage_PopUpB64);
                    if (localBitmap != null)
                    {
                        model.PortraitImage_PopUpBitmap = localBitmap;
                        SetWhatsNewDialogImage(localBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                    }
                    else
                    {
                        // WhatsNew TODO: set default img
                    }
                }
                else if (!string.IsNullOrEmpty(model.PortraitImage_PopUp))
                {
                    _ = GetImageAsync(model, position, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
                else
                {
                    // WhatsNew TODO: set default img
                }

                btnGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");

                btnGotIt.Click += delegate
                {
                    OnCloseClick(position);
                };

                if (!model.Donot_Show_In_WhatsNew)
                {
                    imgWhatsNew.Click += delegate
                    {
                        OnDetailsClick(position);
                        OnCloseClick(position);
                    };
                }

                if (model.Disable_DoNotShow_Checkbox)
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Visible;
                    chkDontShow.Checked = model.SkipShowOnAppLaunch;
                    chkDontShow.Click += delegate
                    {
                        SkipWhatsNew(position, chkDontShow.Checked);
                    };
                }
            }
            else
            {
                rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.WhatsNewPagerTextItemLayout, container, false);

                CardView whatsNewCardView = (CardView)rootView.FindViewById(Resource.Id.whatsNewDialogCardView);

                
                LinearLayout whatsNewDialogMainView = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewDialogMainView);
                LinearLayout whatsNewMainImgLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewMainShimmerImgLayout);
                ShimmerFrameLayout shimmerWhatsNewImageLayout = (ShimmerFrameLayout)rootView.FindViewById(Resource.Id.shimmerWhatsNewImageLayout);

                ImageView imgWhatsNew = (ImageView)rootView.FindViewById(Resource.Id.image_whatsnew);

                TextView txtWhatsNewTitle = (TextView)rootView.FindViewById(Resource.Id.txtWhatsNewTitle);
                TextView txtWhatsNewMessage = (TextView)rootView.FindViewById(Resource.Id.txtWhatsNewMessage);

                LinearLayout whatsNewCheckBoxLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewCheckBoxLayout);
                CheckBox chkDontShow = (CheckBox)rootView.FindViewById(Resource.Id.chk_remember_me);

                Button btnGotIt = (Button)rootView.FindViewById(Resource.Id.btnWhatsNewGotIt);

                if (model.PopUp_Text_Only)
                {
                    whatsNewMainImgLayout.Visibility = ViewStates.Gone;
                    imgWhatsNew.Visibility = ViewStates.Visible;
                    imgWhatsNew.SetScaleType(ImageView.ScaleType.CenterCrop);

                    int photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - GetDeviceHorizontalScaleInPixel(0.096f);
                    if (mContext.Resources.DisplayMetrics.HeightPixels >= 2200)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 6 * GetDeviceHorizontalScaleInPixel(0.016f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 4 * GetDeviceHorizontalScaleInPixel(0.036f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1080)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 2 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    float photoRatio = 0.4929f;
                    int photoHeight = (int)(photoWidth * photoRatio);

                    imgWhatsNew.SetImageResource(Resource.Drawable.ic_banner_whatsnewdialog);
                    imgWhatsNew.LayoutParameters.Height = photoHeight;

                    imgWhatsNew.RequestLayout();
                }
                else
                {
                    whatsNewMainImgLayout.Visibility = ViewStates.Visible;
                    imgWhatsNew.Visibility = ViewStates.Gone;

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

                    // WhatsNew TODO: To handle header image
                    if (!string.IsNullOrEmpty(model.PopUp_HeaderImageB64))
                    {
                        Bitmap localBitmap = Base64ToBitmap(model.PopUp_HeaderImageB64);
                        if (localBitmap != null)
                        {
                            model.PopUp_HeaderImageBitmap = localBitmap;
                            SetWhatsNewDialogTextImage(localBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                        }
                        else
                        {
                            SetWhatsNewDialogTextImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.PopUp_HeaderImage))
                    {
                        _ = GetTextImageAsync(model, position, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                    }
                    else
                    {
                        SetWhatsNewDialogTextImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                    }
                }

                txtWhatsNewTitle.Visibility = ViewStates.Gone;

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtWhatsNewMessage.TextFormatted = Html.FromHtml(model.PopUp_Text_Content, FromHtmlOptions.ModeCompact);
                }
                else
                {
                    txtWhatsNewMessage.TextFormatted = Html.FromHtml(model.PopUp_Text_Content);
                }

                txtWhatsNewMessage = ProcessClickableSpan(txtWhatsNewMessage, model.PopUp_Text_Content);

                btnGotIt.RequestLayout();
                rootView.RequestLayout();
                whatsNewCardView.RequestLayout();

                TextViewUtils.SetMuseoSans300Typeface(txtWhatsNewMessage);
                TextViewUtils.SetMuseoSans500Typeface(txtWhatsNewTitle);
                TextViewUtils.SetMuseoSans500Typeface(btnGotIt, chkDontShow);
                chkDontShow.Text = Utility.GetLocalizedCommonLabel("dontShowThisAgain");

                btnGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");

                if (!model.Donot_Show_In_WhatsNew)
                {
                    whatsNewDialogMainView.Click += delegate
                    {
                        OnDetailsClick(position);
                        OnCloseClick(position);
                    };
                }

                btnGotIt.Click += delegate
                {
                    OnCloseClick(position);
                };

                if (model.Disable_DoNotShow_Checkbox)
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Visible;
                    chkDontShow.Checked = model.SkipShowOnAppLaunch;
                    chkDontShow.Click += delegate
                    {
                        SkipWhatsNew(position, chkDontShow.Checked);
                    };
                }
            }

            container.AddView(rootView);
            return rootView;
        }

        private async Task GetTextImageAsync(WhatsNewModel item, int position, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Bitmap imageBitmap = null;
                await Task.Run(() =>
                {
                    imageBitmap = GetImageBitmapFromUrl(item.PopUp_HeaderImage);
                }, cts.Token);

                if (imageBitmap != null)
                {
                    item.PopUp_HeaderImageBitmap = imageBitmap;
                    item.PopUp_HeaderImageB64 = BitmapToBase64(imageBitmap);
                    this.whatsnew[position].PopUp_HeaderImageBitmap = item.PopUp_HeaderImageBitmap;
                    this.whatsnew[position].PopUp_HeaderImageB64 = item.PopUp_HeaderImageB64;
                    WhatsNewEntity wtManager = new WhatsNewEntity();
                    wtManager.UpdateCachePopupHeaderImage(item.ID, item.PopUp_HeaderImageB64);
                    SetWhatsNewDialogTextImage(imageBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
                else
                {
                    SetWhatsNewDialogTextImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetWhatsNewDialogTextImage(Bitmap imgSrc, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                if (imgSrc == null)
                {
                    int photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - GetDeviceHorizontalScaleInPixel(0.096f);
                    if (mContext.Resources.DisplayMetrics.HeightPixels >= 2200)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 6 * GetDeviceHorizontalScaleInPixel(0.016f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 4 * GetDeviceHorizontalScaleInPixel(0.036f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1080)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 2 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    float photoRatio = 0.4929f;
                    int photoHeight = (int)(photoWidth * photoRatio);

                    imgWhatsNew.SetScaleType(ImageView.ScaleType.CenterCrop);
                    imgWhatsNew.SetImageResource(Resource.Drawable.ic_banner_whatsnewdialog);
                    imgWhatsNew.LayoutParameters.Height = photoHeight;

                    imgWhatsNew.RequestLayout();
                }
                else if (imgSrc != null)
                {
                    float currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - GetDeviceHorizontalScaleInPixel(0.096f);
                    if (mContext.Resources.DisplayMetrics.HeightPixels >= 2200)
                    {
                        currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - 6 * GetDeviceHorizontalScaleInPixel(0.016f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                    {
                        currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - 4 * GetDeviceHorizontalScaleInPixel(0.036f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1080)
                    {
                        currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - 2 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    float calImgRatio = currentImgWidth / imgSrc.Width;
                    int currentImgHeight = (int)(imgSrc.Height * calImgRatio);

                    imgWhatsNew.SetScaleType(ImageView.ScaleType.CenterCrop);
                    imgWhatsNew.SetImageBitmap(imgSrc);
                    imgWhatsNew.LayoutParameters.Height = currentImgHeight;
                    imgWhatsNew.RequestLayout();
                }

                whatsNewMainImgLayout.Visibility = ViewStates.Gone;
                if (shimmerWhatsNewImageLayout.IsShimmerStarted)
                {
                    shimmerWhatsNewImageLayout.StopShimmer();
                }

                imgWhatsNew.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = mContext.Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        public int GetDeviceVerticalScaleInPixel(float percentageValue)
        {
            var deviceHeight = mContext.Resources.DisplayMetrics.HeightPixels;
            return GetScaleInPixel(deviceHeight, percentageValue);
        }

        public int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
        }

        private TextView ProcessClickableSpan(TextView mTextView, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                int whileCount = 0;
                bool isContained = false;
                for (int i = 0; i < MyTNBAppToolTipBuilder.RedirectTypeList.Count; i++)
                {
                    if (message.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[i]))
                    {
                        whileCount = i;
                        isContained = true;
                        break;
                    }
                }

                if (isContained)
                {
                    SpannableString s = new SpannableString(mTextView.TextFormatted);
                    var clickableSpan = new ClickSpan()
                    {
                        textColor = new Android.Graphics.Color(ContextCompat.GetColor(mContext, Resource.Color.powerBlue)),
                        typeFace = Typeface.CreateFromAsset(mContext.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                    };

                    clickableSpan.Click += v =>
                    {
                        if (MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[0]
                            || MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[1]
                            || MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[6])
                        {
                            List<string> extractedUrls = ExtractUrls(message);
                            if (extractedUrls.Count > 0)
                            {
                                if (!extractedUrls[0].Contains("http"))
                                {
                                    extractedUrls[0] = "http://" + extractedUrls[0];
                                }

                                if (MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[0] || MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[6])
                                {
                                    if (extractedUrls[0].Contains(".pdf") && !extractedUrls[0].Contains("docs.google"))
                                    {
                                        Intent webIntent = new Intent(this.mContext, typeof(BasePDFViewerActivity));
                                        webIntent.PutExtra(Constants.IN_APP_LINK, extractedUrls[0]);
                                        webIntent.PutExtra(Constants.IN_APP_TITLE, "");
                                        this.mContext.StartActivity(webIntent);
                                    }
                                    else
                                    {
                                        Intent webIntent = new Intent(this.mContext, typeof(BaseWebviewActivity));
                                        webIntent.PutExtra(Constants.IN_APP_LINK, extractedUrls[0]);
                                        webIntent.PutExtra(Constants.IN_APP_TITLE, "");
                                        this.mContext.StartActivity(webIntent);
                                    }
                                }
                                else
                                {
                                    Intent intent = new Intent(Intent.ActionView);
                                    intent.SetData(Android.Net.Uri.Parse(extractedUrls[0]));
                                    this.mContext.StartActivity(intent);
                                }
                            }
                        }
                        else if (MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[2])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("\">") - 1;
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string phonenum = message.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(phonenum))
                                {
                                    if (!phonenum.Contains("tel:"))
                                    {
                                        phonenum = "tel:" + phonenum;
                                    }

                                    var call = Android.Net.Uri.Parse(phonenum);
                                    var callIntent = new Intent(Intent.ActionView, call);
                                    this.mContext.StartActivity(callIntent);
                                }
                            }
                        }
                        else if (MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[3]
                                    || MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[8])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("}");
                            if (lastIndex < 0)
                            {
                                lastIndex = message.LastIndexOf("\">") - 1;
                            }
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string whatsnewid = message.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(whatsnewid))
                                {
                                    if (!whatsnewid.Contains("{"))
                                    {
                                        whatsnewid = "{" + whatsnewid;
                                    }

                                    if (!whatsnewid.Contains("}"))
                                    {
                                        whatsnewid = whatsnewid + "}";
                                    }

                                    WhatsNewEntity wtManager = new WhatsNewEntity();

                                    WhatsNewEntity item = wtManager.GetItem(whatsnewid);

                                    if (item != null)
                                    {
                                        if (!item.Read)
                                        {
                                            UpdateWhatsNewRead(item.ID, true);
                                        }

                                        Intent activity = new Intent(this.mContext, typeof(WhatsNewDetailActivity));
                                        activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, whatsnewid);
                                        activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                                        this.mContext.StartActivity(activity);
                                    }
                                }
                            }
                        }
                        else if (MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[4]
                                    || MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[9])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("}");
                            if (lastIndex < 0)
                            {
                                lastIndex = message.LastIndexOf("\">") - 1;
                            }
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string faqid = message.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(faqid))
                                {
                                    if (!faqid.Contains("{"))
                                    {
                                        faqid = "{" + faqid;
                                    }

                                    if (!faqid.Contains("}"))
                                    {
                                        faqid = faqid + "}";
                                    }

                                    Intent faqIntent = new Intent(this.mContext, typeof(FAQListActivity));
                                    faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                    this.mContext.StartActivity(faqIntent);
                                }
                            }
                        }
                        else if (MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[5]
                                    || MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[10])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("}");
                            if (lastIndex < 0)
                            {
                                lastIndex = message.LastIndexOf("\">") - 1;
                            }
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string rewardid = message.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(rewardid))
                                {
                                    if (!rewardid.Contains("{"))
                                    {
                                        rewardid = "{" + rewardid;
                                    }

                                    if (!rewardid.Contains("}"))
                                    {
                                        rewardid = rewardid + "}";
                                    }

                                    RewardsEntity wtManager = new RewardsEntity();

                                    RewardsEntity item = wtManager.GetItem(rewardid);

                                    if (item != null)
                                    {
                                        if (!item.Read)
                                        {
                                            UpdateRewardRead(item.ID, true);
                                        }

                                        Intent activity = new Intent(this.mContext, typeof(RewardDetailActivity));
                                        activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, rewardid);
                                        activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "rewards"));
                                        this.mContext.StartActivity(activity);
                                    }
                                }
                            }
                        }
                        else if (MyTNBAppToolTipBuilder.RedirectTypeList[whileCount] == MyTNBAppToolTipBuilder.RedirectTypeList[7])
                        {
                            int startIndex = message.LastIndexOf("\"tel") + 1;
                            int lastIndex = message.LastIndexOf("\">") - 1;
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string phonenum = message.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(phonenum))
                                {
                                    if (!phonenum.Contains("tel:"))
                                    {
                                        phonenum = "tel:" + phonenum;
                                    }

                                    var call = Android.Net.Uri.Parse(phonenum);
                                    var callIntent = new Intent(Intent.ActionView, call);
                                    this.mContext.StartActivity(callIntent);
                                }
                            }
                        }
                    };

                    var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
                    int startLink = s.GetSpanStart(urlSpans[0]);
                    int endLink = s.GetSpanEnd(urlSpans[0]);
                    s.RemoveSpan(urlSpans[0]);
                    s.SetSpan(clickableSpan, startLink, endLink, SpanTypes.ExclusiveExclusive);
                    mTextView.TextFormatted = s;
                    mTextView.MovementMethod = new LinkMovementMethod();
                }
            }

            return mTextView;
        }

        private List<string> ExtractUrls(string text)
        {
            List<string> containedUrls = new List<string>();
            string urlRegex = "\\(?\\b(https://|http://|www[.])[-A-Za-z0-9+&amp;@#/%?=~_()|!:,.;]*[-A-Za-z0-9+&amp;@#/%=~_()|]";
            Java.Util.Regex.Pattern pattern = Java.Util.Regex.Pattern.Compile(urlRegex);
            Matcher urlMatcher = pattern.Matcher(text);

            try
            {
                while (urlMatcher.Find())
                {
                    string urlStr = urlMatcher.Group();
                    if (urlStr.StartsWith("(") && urlStr.EndsWith(")"))
                    {
                        urlStr = urlStr.Substring(1, urlStr.Length - 1);
                    }

                    if (!containedUrls.Contains(urlStr))
                    {
                        containedUrls.Add(urlStr);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return containedUrls;
        }

        private void UpdateWhatsNewRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            WhatsNewEntity wtManager = new WhatsNewEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);
        }

        private void UpdateRewardRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);

            _ = OnUpdateReward(itemID);
        }

        private async Task OnUpdateReward(string itemID)
        {
            try
            {
                // Update api calling
                RewardsEntity wtManager = new RewardsEntity();
                RewardsEntity currentItem = wtManager.GetItem(itemID);

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                string rewardId = currentItem.ID;
                rewardId = rewardId.Replace("{", "");
                rewardId = rewardId.Replace("}", "");

                AddUpdateRewardModel currentReward = new AddUpdateRewardModel()
                {
                    Email = UserEntity.GetActive().Email,
                    RewardId = rewardId,
                    Read = currentItem.Read,
                    ReadDate = !string.IsNullOrEmpty(currentItem.ReadDateTime) ? currentItem.ReadDateTime + " +00:00" : "",
                    Favourite = currentItem.IsSaved,
                    FavUpdatedDate = !string.IsNullOrEmpty(currentItem.IsSavedDateTime) ? currentItem.IsSavedDateTime + " +00:00" : "",
                    Redeemed = currentItem.IsUsed,
                    RedeemedDate = !string.IsNullOrEmpty(currentItem.IsUsedDateTime) ? currentItem.IsUsedDateTime + " +00:00" : ""
                };

                AddUpdateRewardRequest request = new AddUpdateRewardRequest()
                {
                    usrInf = currentUsrInf,
                    reward = currentReward
                };

                AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);

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

        private void SkipWhatsNew(int pos, bool isCheck)
        {
            string id = this.whatsnew[pos].ID;
            this.whatsnew[pos].SkipShowOnAppLaunch = isCheck;
            this.NotifyDataSetChanged();
            WhatsNewEntity wtManager = new WhatsNewEntity();
            wtManager.UpdateDialogSkipItem(id, isCheck);
        }

        private async Task GetImageAsync(WhatsNewModel item, int position, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Bitmap imageBitmap = null;
                await Task.Run(() =>
                {
                    imageBitmap = GetImageBitmapFromUrl(item.PortraitImage_PopUp);
                }, cts.Token);

                if (imageBitmap != null)
                {
                    item.PortraitImage_PopUpBitmap = imageBitmap;
                    item.PortraitImage_PopUpB64 = BitmapToBase64(imageBitmap);
                    this.whatsnew[position].PortraitImage_PopUpBitmap = item.PortraitImage_PopUpBitmap;
                    this.whatsnew[position].PortraitImage_PopUpB64 = item.PortraitImage_PopUpB64;
                    WhatsNewEntity wtManager = new WhatsNewEntity();
                    wtManager.UpdateCachePopupImage(item.ID, item.PortraitImage_PopUpB64);
                    SetWhatsNewDialogImage(imageBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
                else
                {
                    // WhatsNew TODO: set default img
                    // SetWhatsNewDialogImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Android.Util.Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        private Bitmap GetImageBitmapFromUrl(string url)
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

        private void SetWhatsNewDialogImage(Bitmap imgSrc, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                if (imgSrc == null)
                {
                    /*BitmapFactory.Options opt = new BitmapFactory.Options();
                    opt.InMutable = true;

                    Bitmap mDefaultBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.promotions_default_image, opt);

                    whatsNewImg.SetImageBitmap(mDefaultBitmap);*/
                    // WhatsNew TODO: set default img
                }
                else if (imgSrc != null)
                {
                    imgWhatsNew.SetImageBitmap(imgSrc);
                }

                whatsNewMainImgLayout.Visibility = ViewStates.Gone;
                if (shimmerWhatsNewImageLayout.IsShimmerStarted)
                {
                    shimmerWhatsNewImageLayout.StopShimmer();
                }

                imgWhatsNew.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Android.Util.Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        void OnDetailsClick(int position)
        {
            if (DetailsClicked != null)
                DetailsClicked(this, position);
        }

        void OnCloseClick(int position)
        {
            this.whatsnew.RemoveAt(position);
            this.NotifyDataSetChanged();
            OnRefreshIndicator(position);
            if (this.whatsnew.Count == 0)
            {
                if (CloseClicked != null)
                    CloseClicked(this, position);
            }
        }

        void OnRefreshIndicator(int position)
        {
            if (RefreshIndicator != null)
            {
                RefreshIndicator(this, position);
            }
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return PagerAdapter.PositionNone;
        }

        public override int Count => whatsnew.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

    }
}