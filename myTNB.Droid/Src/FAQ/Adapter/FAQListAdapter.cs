using Android.Graphics;
using Android.Runtime;


using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using myTNB.SitecoreCMS.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.FAQ.Adapter
{
    public class FAQListAdapter : RecyclerView.Adapter
    {

        private Android.App.Activity mActicity;
        private List<FAQsModel> faqList = new List<FAQsModel>();
        private List<FAQsModel> orgFAQList = new List<FAQsModel>();
        public event EventHandler<int> ItemClick;


        public FAQListAdapter(Android.App.Activity acticity, List<FAQsModel> data)
        {
            this.mActicity = acticity;
            this.faqList.AddRange(data);
            this.orgFAQList.AddRange(data);
        }

        public override int ItemCount => faqList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                FAQListViewHolder vh = holder as FAQListViewHolder;

                FAQsModel model = faqList[position];
                vh.Question.Text = model.Question;

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    vh.Answer.TextFormatted = Trim(Html.FromHtml(model.Answer, FromHtmlOptions.ModeLegacy));
                }
                else
                {
                    vh.Answer.TextFormatted = Trim(Html.FromHtml(model.Answer));
                }

                Linkify.AddLinks(vh.Answer, MatchOptions.All);

                TextViewUtils.SetMuseoSans500Typeface(vh.Question);
                TextViewUtils.SetMuseoSans300Typeface(vh.Answer);
                vh.Question.TextSize = TextViewUtils.GetFontSize(16f);
                vh.Answer.TextSize = TextViewUtils.GetFontSize(14f);
                SpannableString s = new SpannableString(vh.Answer.TextFormatted);
                var spans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
                if (spans != null && spans.Length > 0)
                {
                    for (int i = 0; i < spans.Length; i++)
                    {
                        URLSpan URLItem = spans[i] as URLSpan;
                        int start = s.GetSpanStart(URLItem);
                        int end = s.GetSpanEnd(URLItem);
                        s.RemoveSpan(URLItem);
                        var newURLSpan = new CustomUrlSpan(URLItem.URL)
                        {
                            textColor = new Android.Graphics.Color(ContextCompat.GetColor(this.mActicity, Resource.Color.powerBlue)),
                            typeFace = Typeface.CreateFromAsset(this.mActicity.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                        };
                        s.SetSpan(newURLSpan, start, end, SpanTypes.ExclusiveExclusive);
                    }

                    vh.Answer.TextFormatted = s;
                    vh.Answer.MovementMethod = new LinkMovementMethod();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.FAQListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new FAQListViewHolder(itemView);
        }

        public class FAQListViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout RootView { get; private set; }
            public TextView Question { get; private set; }
            public TextView Answer { get; private set; }

            public FAQListViewHolder(View itemView) : base(itemView)
            {
                RootView = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                Question = itemView.FindViewById<TextView>(Resource.Id.faq_question);
                Answer = itemView.FindViewById<TextView>(Resource.Id.faq_answer);

            }

        }

        public ICharSequence Trim(ISpanned s)
        {
            int start = 0;
            int end = s.Length() - 1;

            while (start < end && Character.IsWhitespace(s.CharAt(start)))
            {
                start++;
            }

            while (end > start && Character.IsWhitespace(s.CharAt(end - 1)))
            {
                end--;
            }

            return s.SubSequenceFormatted(start, end);
        }

        public class CustomUrlSpan : URLSpan
        {
            public Color textColor { get; set; }
            public Typeface typeFace { get; set; }

            public CustomUrlSpan(string text) : base(text)
            {

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
}