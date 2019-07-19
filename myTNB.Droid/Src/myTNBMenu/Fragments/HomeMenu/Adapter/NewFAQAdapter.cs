using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
	public class NewFAQAdapter : RecyclerView.Adapter
	{

		List<NewFAQ> faqList = null;

        public event EventHandler<int> ClickChanged;

        public NewFAQAdapter(List<NewFAQ> data)
		{
			this.faqList = new List<NewFAQ>();
			this.faqList = data;
		}

		public override int ItemCount => faqList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			NewFAQViewHolder vh = holder as NewFAQViewHolder;

			NewFAQ model = faqList[position];

			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
			{
				vh.faqTitle.TextFormatted = Html.FromHtml(model.NewFAQString, FromHtmlOptions.ModeLegacy);
			}
			else
			{
				vh.faqTitle.TextFormatted = Html.FromHtml(model.NewFAQString);
			}

			switch (model.Id)
			{
				default:
					vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_1);
					break;

			}

			TextViewUtils.SetMuseoSans500Typeface(vh.faqTitle);
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

                faqCardView.Click += (s, e) => listener((this), base.LayoutPosition);
            }
		}

	}
}
