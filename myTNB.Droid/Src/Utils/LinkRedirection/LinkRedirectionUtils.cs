using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Bills.NewBillRedesign;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.WhatsNewDetail.MVP;

using Constant = myTNB_Android.Src.Utils.LinkRedirection.LinkRedirection.Constants;
using Screen = myTNB_Android.Src.Utils.LinkRedirection.LinkRedirection.ScreenEnum;

namespace myTNB_Android.Src.Utils
{
    public class LinkRedirectionUtils
    {
        private Color mClickSpanColor;
        private Typeface mTypeface;
        private Android.App.Activity mActivity;
        private RewardServiceImpl mApi;
        private string mMessage;
        private TextView mTextView;
        private string mHeaderTitle;
        private Action mAction;
        private Screen TargetScreen = Screen.None;

        /// <summary>
        /// WARNING: Please add new type at the bottom of the list and DO NOT rearrange existing items
        /// </summary>
        public static List<string> RedirectTypeList = new List<string> {
            "inAppBrowser=",
            "externalBrowser=",
            "tel=",
            "whatsnew=",
            "faq=",
            "reward=",
            "http",
            "tel:",
            "whatsnewid=",
            "faqid=",
            "rewardid=",
            "inAppScreen="
        };

        private LinkRedirectionUtils(Android.App.Activity activity, string headerTitle)
        {
            this.mActivity = activity;
            this.mApi = new RewardServiceImpl();
            this.mHeaderTitle = headerTitle;
        }

        public static LinkRedirectionUtils Create(Android.App.Activity activity, string headerTitle)
        {
            LinkRedirectionUtils linkRedirection = new LinkRedirectionUtils(activity, headerTitle);
            return linkRedirection;
        }

        public LinkRedirectionUtils SetTextView(TextView textView)
        {
            this.mTextView = textView;
            return this;
        }

        public LinkRedirectionUtils SetAction(Action action)
        {
            this.mAction = action;
            return this;
        }

        public LinkRedirectionUtils SetMessage(string message, Color? color = null, Typeface? typeface = null)
        {
            this.mMessage = message;

            this.mClickSpanColor = color ?? new Android.Graphics.Color(ContextCompat.GetColor(mActivity, Resource.Color.powerBlue));
            this.mTypeface = typeface ?? Typeface.CreateFromAsset(mActivity.Assets, "fonts/" + TextViewUtils.MuseoSans500);
            return this;
        }

        public LinkRedirectionUtils Build()
        {
            if (!string.IsNullOrEmpty(this.mMessage))
            {
                SpannableString s = new SpannableString(mTextView.TextFormatted);
                var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));

                if (urlSpans != null && urlSpans.Length > 0)
                {
                    for (int i = 0; i < urlSpans.Length; i++)
                    {
                        URLSpan URLItem = urlSpans[i] as URLSpan;
                        int startIndex = s.GetSpanStart(urlSpans[i]);
                        int endIndex = s.GetSpanEnd(urlSpans[i]);
                        s.RemoveSpan(urlSpans[i]);
                        ClickSpan clickableSpan = new ClickSpan()
                        {
                            textColor = this.mClickSpanColor,
                            typeFace = this.mTypeface
                        };
                        clickableSpan.Click += v =>
                        {
                            OnClickSpan(URLItem.URL);
                        };
                        s.SetSpan(clickableSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
                    }
                    mTextView.TextFormatted = s;
                    mTextView.MovementMethod = new LinkMovementMethod();
                }
            }

            return this;
        }

        public TextView GetProcessedTextView()
        {
            return this.mTextView;
        }

        private void OnClickSpan(string url)
        {
            try
            {
                if (this.mAction != null)
                    this.mAction();

                if (!string.IsNullOrEmpty(url))
                {
                    //for:
                    //"inAppBrowser="
                    //"externalBrowser="
                    //"http"
                    if (url.Contains(RedirectTypeList[0])
                        || url.Contains(RedirectTypeList[1])
                        || url.Contains(RedirectTypeList[6]))
                    {
                        string uri = url;
                        if (url.Contains(RedirectTypeList[0]))
                        {
                            uri = url.Split(RedirectTypeList[0])[1];
                        }
                        else if (url.Contains(RedirectTypeList[1]))
                        {
                            uri = url.Split(RedirectTypeList[1])[1];
                        }

                        string compareText = uri.ToLower();

                        if (!compareText.Contains("http"))
                        {
                            uri = "http://" + uri;
                        }

                        //External Browser
                        if (url.Contains(RedirectTypeList[1]))
                        {
                            Intent intent = new Intent(Intent.ActionView);
                            intent.SetData(Android.Net.Uri.Parse(uri));
                            mActivity.StartActivity(intent);
                        }
                        //In App Browser
                        else
                        {
                            if (compareText.Contains(".pdf") && !compareText.Contains("docs.google"))
                            {
                                Intent webIntent = new Intent(mActivity, typeof(BasePDFViewerActivity));
                                webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                webIntent.PutExtra(Constants.IN_APP_TITLE, this.mHeaderTitle);
                                mActivity.StartActivity(webIntent);
                            }
                            else if (compareText.Contains(".jpeg") || compareText.Contains(".jpg") || compareText.Contains(".png"))
                            {
                                Intent webIntent = new Intent(mActivity, typeof(BaseFullScreenImageViewActivity));
                                webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                webIntent.PutExtra(Constants.IN_APP_TITLE, this.mHeaderTitle);
                                mActivity.StartActivity(webIntent);
                            }
                            else
                            {
                                Intent webIntent = new Intent(mActivity, typeof(BaseWebviewActivity));
                                webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                webIntent.PutExtra(Constants.IN_APP_TITLE, this.mHeaderTitle);
                                mActivity.StartActivity(webIntent);
                            }
                        }
                    }
                    //for:
                    //"tel="
                    //"tel:"
                    else if (url.Contains(RedirectTypeList[2])
                                || url.Contains(RedirectTypeList[7]))
                    {
                        string phonenum = url;
                        if (url.Contains(RedirectTypeList[2]))
                        {
                            phonenum = url.Split(RedirectTypeList[2])[1];
                        }
                        if (!string.IsNullOrEmpty(phonenum))
                        {
                            if (!phonenum.Contains("tel:"))
                            {
                                phonenum = "tel:" + phonenum;
                            }

                            var call = Android.Net.Uri.Parse(phonenum);
                            var callIntent = new Intent(Intent.ActionView, call);
                            mActivity.StartActivity(callIntent);
                        }
                    }
                    //for:
                    //"whatsnew="
                    //"whatsnewid="
                    else if (url.Contains(RedirectTypeList[3])
                                || url.Contains(RedirectTypeList[8]))
                    {
                        string whatsnewid = url;
                        if (url.Contains(RedirectTypeList[3]))
                        {
                            whatsnewid = url.Split(RedirectTypeList[3])[1];
                        }
                        else if (url.Contains(RedirectTypeList[8]))
                        {
                            whatsnewid = url.Split(RedirectTypeList[8])[1];
                        }

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

                                Intent activity = new Intent(mActivity, typeof(WhatsNewDetailActivity));
                                activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, whatsnewid);
                                activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                                mActivity.StartActivity(activity);
                            }
                        }
                    }
                    //for:
                    //"faq="
                    //"faqid="
                    else if (url.Contains(RedirectTypeList[4])
                                || url.Contains(RedirectTypeList[9]))
                    {
                        string faqid = url;
                        if (url.Contains(RedirectTypeList[4]))
                        {
                            faqid = url.Split(RedirectTypeList[4])[1];
                        }
                        else if (url.Contains(RedirectTypeList[9]))
                        {
                            faqid = url.Split(RedirectTypeList[9])[1];
                        }

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

                            if (faqid.Contains("\""))
                            {
                                faqid = faqid.Replace("\"", string.Empty);
                            }

                            if (faqid.Contains("\\"))
                            {
                                faqid = faqid.Replace("\\", string.Empty);
                            }

                            Intent faqIntent = new Intent(mActivity, typeof(FAQListActivity));
                            faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                            mActivity.StartActivity(faqIntent);
                        }
                    }
                    //for:
                    //"reward="
                    //"rewardid="
                    else if (url.Contains(RedirectTypeList[5])
                                || url.Contains(RedirectTypeList[10]))
                    {
                        string rewardid = url;
                        if (url.Contains(RedirectTypeList[5]))
                        {
                            rewardid = url.Split(RedirectTypeList[5])[1];
                        }
                        else if (url.Contains(RedirectTypeList[10]))
                        {
                            rewardid = url.Split(RedirectTypeList[10])[1];
                        }

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

                                Intent activity = new Intent(mActivity, typeof(RewardDetailActivity));
                                activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, rewardid);
                                activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "rewards"));
                                mActivity.StartActivity(activity);
                            }
                        }
                    }
                    //for:
                    //"inAppScreen="
                    else if (url.Contains(RedirectTypeList[11]))
                    {
                        var targetScreen = GetTargetInAppScreen(url);
                        if (targetScreen.Contains(Screen.NewBillDesignComms.ToString()))
                        {
                            TargetScreen = Screen.NewBillDesignComms;
                        }
                        NavigateToTargetScreen(TargetScreen);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private string GetTargetInAppScreen(string path)
        {
            string value = string.Empty;
            string pattern = string.Format(Constant.Pattern, Constant.InAppScreenKey);
            Regex regex = new Regex(pattern);
            Match match = regex.Match(path);
            if (match.Success)
            {
                value = match.Value.Replace(string.Format(Constant.ReplaceKey, Constant.InAppScreenKey), string.Empty);
            }

            return value;
        }

        private void NavigateToTargetScreen(Screen targetScreen)
        {
            switch (targetScreen)
            {
                case Screen.NewBillDesignComms:
                    {
                        Intent nbrDiscoverMoreIntent = new Intent(mActivity, typeof(NBRDiscoverMoreActivity));
                        mActivity.StartActivityForResult(nbrDiscoverMoreIntent, Constants.NEW_BILL_REDESIGN_REQUEST_CODE);
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateWhatsNewRead(string itemID, bool flag)
        {
            try
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void UpdateRewardRead(string itemID, bool flag)
        {
            try
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
}