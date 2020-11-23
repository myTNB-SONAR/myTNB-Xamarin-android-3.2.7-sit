using Android.Graphics;

using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.SSMRMeterHistory.Adapter
{
	public class SSMRMeterHistoryAdapter : RecyclerView.Adapter
	{
        private List<SSMRMeterHistoryModel> ssmrMeterHistoryList = new List<SSMRMeterHistoryModel>();

		public SSMRMeterHistoryAdapter(List<SSMRMeterHistoryModel> data)
		{
			if (data == null)
			{
				this.ssmrMeterHistoryList.Clear();
                this.NotifyDataSetChanged();
			}
			else
			{
                this.ssmrMeterHistoryList.Clear();
				this.ssmrMeterHistoryList.AddRange(data);
                this.NotifyDataSetChanged();
            }
		}

		public override int ItemCount => ssmrMeterHistoryList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
            SSMRMeterHistoryViewHolder vh = holder as SSMRMeterHistoryViewHolder;

            SSMRMeterHistoryModel model = ssmrMeterHistoryList[position];

			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
			{
				vh.ssmrDate.TextFormatted = Html.FromHtml(model.ReadingDate, FromHtmlOptions.ModeLegacy);
                vh.ssmrKWhValue.TextFormatted = Html.FromHtml(model.ReadingValue, FromHtmlOptions.ModeLegacy);
                vh.ssmrDescription.TextFormatted = Html.FromHtml(model.ReadingType, FromHtmlOptions.ModeLegacy);
                vh.ssmrForDate.TextFormatted = Html.FromHtml(model.ReadingForMonth, FromHtmlOptions.ModeLegacy);
            }
			else
			{
                vh.ssmrDate.TextFormatted = Html.FromHtml(model.ReadingDate);
                vh.ssmrKWhValue.TextFormatted = Html.FromHtml(model.ReadingValue);
                vh.ssmrDescription.TextFormatted = Html.FromHtml(model.ReadingType);
                vh.ssmrForDate.TextFormatted = Html.FromHtml(model.ReadingForMonth);
			}

			TextViewUtils.SetMuseoSans500Typeface(vh.ssmrDate, vh.ssmrKWhValue);
            TextViewUtils.SetMuseoSans300Typeface(vh.ssmrDescription, vh.ssmrForDate);

            if (model.ReadingTypeCode.Trim() == "03" || model.ReadingTypeCode.Trim() == "53" ||
                model.ReadingTypeCode.Trim() == "73" || model.ReadingTypeCode.Trim() == "83" ||
                model.ReadingTypeCode.Trim() == "93")
            {
                vh.ssmrDescription.SetTextColor(Color.ParseColor("#e44b21"));
            }
        }

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var id = Resource.Layout.SSMRMeterReadingCardLayout;
			var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
			return new SSMRMeterHistoryViewHolder(itemView);
		}


		public class SSMRMeterHistoryViewHolder : RecyclerView.ViewHolder
		{

			public TextView ssmrDate { get; private set; }

			public TextView ssmrDescription { get; private set; }

            public TextView ssmrKWhValue { get; private set; }

            public TextView ssmrForDate { get; private set; }

            public SSMRMeterHistoryViewHolder(View itemView) : base(itemView)
			{
                ssmrDate = itemView.FindViewById<TextView>(Resource.Id.ssmrDate);
                ssmrDescription = itemView.FindViewById<TextView>(Resource.Id.ssmrDescription);
                ssmrKWhValue = itemView.FindViewById<TextView>(Resource.Id.ssmrKWhValue);
                ssmrForDate = itemView.FindViewById<TextView>(Resource.Id.ssmrForDate);

				ssmrDate.TextSize = TextViewUtils.GetFontSize(12f);
				ssmrDescription.TextSize = TextViewUtils.GetFontSize(12f);
				ssmrKWhValue.TextSize = TextViewUtils.GetFontSize(12f);
				ssmrForDate.TextSize = TextViewUtils.GetFontSize(12f);
			}
		}

	}
}
