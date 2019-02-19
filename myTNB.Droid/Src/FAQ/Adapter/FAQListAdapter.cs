using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using myTNB.SitecoreCMS.Models;
using myTNB_Android.Src.Utils;
using Android.Text;

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

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                {
                    vh.Answer.TextFormatted = Html.FromHtml(model.Answer, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    vh.Answer.TextFormatted = Html.FromHtml(model.Answer);
                }

                TextViewUtils.SetMuseoSans500Typeface(vh.Question);
                TextViewUtils.SetMuseoSans300Typeface(vh.Answer);
            } catch(Exception e) {
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
    }
}