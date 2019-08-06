using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
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
			NewFAQViewHolder vh = holder as NewFAQViewHolder;

			NewFAQ model = faqList[position];

			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
			{
				vh.faqTitle.TextFormatted = Html.FromHtml(model.Title, FromHtmlOptions.ModeLegacy);
			}
			else
			{
				vh.faqTitle.TextFormatted = Html.FromHtml(model.Title);
			}

            int currentCount = position % 7;

			switch (currentCount)
			{
                case 0:
                    vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_1);
                    break;
                case 1:
                    vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_2);
                    break;
                case 2:
                    vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_3);
                    break;
                case 3:
                    vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_4);
                    break;
                case 4:
                    vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_5);
                    break;
                case 5:
                    vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_6);
                    break;
                case 6:
                    vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_7);
                    break;
                default:
					vh.backgroundImg.SetBackgroundResource(Resource.Drawable.faq_color_1);
					break;

			}

			TextViewUtils.SetMuseoSans500Typeface(vh.faqTitle);

            ViewGroup.LayoutParams currentCard = vh.faqCardView.LayoutParameters;

            int cardWidth = (int)((this.mActivity.Resources.DisplayMetrics.WidthPixels / 2.85) - DPUtils.ConvertDPToPx(10f));
            if (DPUtils.ConvertPxToDP(cardWidth) < 96f)
            {
                cardWidth = (int)DPUtils.ConvertDPToPx(96f);
            }

            currentCard.Height = cardWidth;
            currentCard.Width = cardWidth;
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
