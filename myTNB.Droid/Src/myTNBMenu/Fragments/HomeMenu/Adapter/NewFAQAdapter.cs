using Android.Graphics;
using Android.Graphics.Drawables;

using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class NewFAQAdapter : RecyclerView.Adapter
    {

        List<NewFAQ> faqList = new List<NewFAQ>();

        public event EventHandler<int> ClickChanged;

        private Android.App.Activity mActivity;

        public NewFAQAdapter(List<NewFAQ> data, Android.App.Activity Activity)
        {
            if (data == null)
            {
                this.faqList.Clear();
            }
            else
            {
                this.faqList = data;
            }
            this.mActivity = Activity;
        }

        public override int ItemCount => faqList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                NewFAQViewHolder vh = holder as NewFAQViewHolder;

                NewFAQ model = faqList[position];

                try
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        vh.faqTitle.TextFormatted = Html.FromHtml(model.Title, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        vh.faqTitle.TextFormatted = Html.FromHtml(model.Title);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                try
                {
                    Color startColor = Color.Argb(255, 85, 60, 207);
                    Color endColor = Color.Argb(255, 58, 155, 244);

                    if (!string.IsNullOrEmpty(model.BGStartColor) && !string.IsNullOrEmpty(model.BGEndColor))
                    {
                        string[] startColorArray = model.BGStartColor.Split("|");

                        string[] endColorArray = model.BGEndColor.Split("|");

                        if (startColorArray.Length == 3 && endColorArray.Length == 3)
                        {
                            try
                            {
                                startColor = Color.Argb(255, int.Parse(startColorArray[0]), int.Parse(startColorArray[1]), int.Parse(startColorArray[2]));
                                endColor = Color.Argb(255, int.Parse(endColorArray[0]), int.Parse(endColorArray[1]), int.Parse(endColorArray[2]));
                            }
                            catch (Exception e)
                            {
                                startColor = Color.Argb(255, 85, 60, 207);
                                endColor = Color.Argb(255, 58, 155, 244);
                                Utility.LoggingNonFatalError(e);
                            }
                        }
                    }

                    GradientDrawable gd = new GradientDrawable(
                        GradientDrawable.Orientation.TopBottom,
                        new int[] { startColor, endColor });

                    vh.backgroundImg.Background = gd;

                    TextViewUtils.SetMuseoSans500Typeface(vh.faqTitle);

                    ViewGroup.LayoutParams currentCard = vh.faqCardView.LayoutParameters;

                    float h1 = TextViewUtils.IsLargeFonts ? 65f : 0f;
                    int cardWidth = (int)(((this.mActivity.Resources.DisplayMetrics.WidthPixels / 3.05) + DPUtils.ConvertDPToPx(h1)) - DPUtils.ConvertDPToPx(16f));

                    float heightRatio = 56f / 92f;
                    int cardHeight = (int)((int)(((this.mActivity.Resources.DisplayMetrics.WidthPixels / 3.05)) - DPUtils.ConvertDPToPx(16f)) * (heightRatio));

                    currentCard.Height = cardHeight;
                    currentCard.Width = cardWidth;

                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(cardWidth,
                    cardHeight);
                    if (position == 0)
                    {
                        layoutParams.LeftMargin = (int)DPUtils.ConvertDPToPx(16f);
                        layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(8f);
                    }
                    if ((position + 1) == faqList.Count)
                    {
                        layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(16f);
                    }
                    else
                    {
                        layoutParams.RightMargin = (int)DPUtils.ConvertDPToPx(8f);
                    }
                    vh.faqCardView.LayoutParameters = layoutParams;
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
            var id = Resource.Layout.NewFAQComponentView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new NewFAQViewHolder(itemView, OnClick);
        }

        void OnClick(NewFAQViewHolder sender, int position)
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


        public class NewFAQViewHolder : RecyclerView.ViewHolder
        {

            public LinearLayout backgroundImg { get; private set; }

            public TextView faqTitle { get; private set; }

            public CardView faqCardView { get; private set; }

            public NewFAQViewHolder(View itemView, Action<NewFAQViewHolder, int> listener) : base(itemView)
            {
                backgroundImg = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                faqTitle = itemView.FindViewById<TextView>(Resource.Id.faq_title);
                faqCardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
                if (TextViewUtils.IsLargeFonts)
                {
                    TextViewUtils.SetTextSize10(faqTitle);
                }
                else
                {
                    TextViewUtils.SetTextSize12(faqTitle);
                }
                faqCardView.Click += (s, e) => listener((this), base.LayoutPosition);
            }
        }

    }
}
