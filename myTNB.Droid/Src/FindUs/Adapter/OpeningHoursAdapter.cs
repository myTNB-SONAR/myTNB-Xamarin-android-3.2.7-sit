
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.FindUs.Models;
using myTNB.Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB.Android.Src.FindUs.Adapter
{
    public class OpeningHoursAdapter : RecyclerView.Adapter
    {
        BaseAppCompatActivity mActivity;
        private List<OpeningHours> openingHours = new List<OpeningHours>();

        public OpeningHoursAdapter(BaseAppCompatActivity activity, List<OpeningHours> data)
        {
            this.mActivity = activity;
            this.openingHours.AddRange(data);
        }

        public override int ItemCount => openingHours.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            OpeningHoursListViewHolder h = holder as OpeningHoursListViewHolder;
            TextViewUtils.SetMuseoSans300Typeface(h.Title);
            TextViewUtils.SetMuseoSans300Typeface(h.Description);
            OpeningHours item = openingHours[position];
            h.Title.Text = item.Title;
            h.Description.Text = item.Description;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.OpeningHoursListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new OpeningHoursListViewHolder(itemView);
        }

        public class OpeningHoursListViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; private set; }
            public TextView Description { get; private set; }

            public OpeningHoursListViewHolder(View itemView) : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.text_title);
                Description = itemView.FindViewById<TextView>(Resource.Id.text_description);
                TextViewUtils.SetTextSize16(Title, Description);
            }

        }
    }
}