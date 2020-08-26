using Android.Content;
using Android.Graphics;


using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Adapter
{
    public class ApplicationStatusLandingAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        private List<ApplicationStatusColorCodeModel> mStatusCodeColorList = new List<ApplicationStatusColorCodeModel>();
        private List<ApplicationStatusModel> mApplicationStatusList = new List<ApplicationStatusModel>();
        public event EventHandler<int> ItemClick;


        public void Clear()
        {
            this.mApplicationStatusList.Clear();
            this.NotifyDataSetChanged();
        }

        public List<ApplicationStatusModel> GetApplicationStatusList()
        {
            return mApplicationStatusList;
        }

        public void UpdateAddList(List<ApplicationStatusModel> data)
        {
            this.mApplicationStatusList.AddRange(data);
            this.NotifyDataSetChanged();
        }

        public ApplicationStatusLandingAdapter(BaseActivityCustom activity, List<ApplicationStatusModel> data, List<ApplicationStatusColorCodeModel> color)
        {
            this.mActicity = activity;
            this.mApplicationStatusList.AddRange(data);
            this.mStatusCodeColorList.AddRange(color);
        }

        public override int ItemCount => mApplicationStatusList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationStatusViewHolder vh = holder as ApplicationStatusViewHolder;

            ApplicationStatusModel item = mApplicationStatusList[position];
            ApplicationStatusColorCodeModel color = mStatusCodeColorList.Find(x => x.Code == item.StatusCode);
            vh.PopulateData(item, color);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationStatusListGroupItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ApplicationStatusViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }


    public class ApplicationStatusViewHolder : RecyclerView.ViewHolder
    {
        public TextView ApplicationStatusItemTitle { get; private set; }
        public TextView ApplicationStatusItemSubTitle { get; private set; }
        public LinearLayout AppicationStatusListMainLayout { get; private set; }
        public TextView ApplicationStatusItemStatus { get; private set; }
        public TextView ApplicationStatusItemDate { get; private set; }
        public ImageView ApplicationStatusItemNewIndicator { get; private set; }
        public ImageView ApplicationStatusItemRightArrow { get; private set; }
        private Context context;

        private ApplicationStatusModel item = null;

        public ApplicationStatusViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            context = itemView.Context;
            AppicationStatusListMainLayout = itemView.FindViewById<LinearLayout>(Resource.Id.appicationStatusListMainLayout);
            ApplicationStatusItemTitle = itemView.FindViewById<TextView>(Resource.Id.applicationStatusItemTitle);
            ApplicationStatusItemSubTitle = itemView.FindViewById<TextView>(Resource.Id.applicationStatusItemSubTitle);
            ApplicationStatusItemStatus = itemView.FindViewById<TextView>(Resource.Id.applicationStatusItemStatus);
            ApplicationStatusItemDate = itemView.FindViewById<TextView>(Resource.Id.applicationStatusItemDate);
            ApplicationStatusItemNewIndicator = itemView.FindViewById<ImageView>(Resource.Id.applicationStatusItemNewIndicator);
            ApplicationStatusItemRightArrow = itemView.FindViewById<ImageView>(Resource.Id.applicationStatusItemRightArrow);
            ApplicationStatusItemTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.tunaGrey)));
            ApplicationStatusItemSubTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.receipt_note_text)));
            ApplicationStatusItemStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.silverChalice)));
            ApplicationStatusItemDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.receipt_note_text)));
            TextViewUtils.SetMuseoSans300Typeface(ApplicationStatusItemSubTitle);
            TextViewUtils.SetMuseoSans500Typeface(ApplicationStatusItemTitle, ApplicationStatusItemStatus, ApplicationStatusItemDate);
            AppicationStatusListMainLayout.Click += (sender, e) => listener(base.LayoutPosition);
        }


        public void PopulateData(ApplicationStatusModel item, ApplicationStatusColorCodeModel colorMode)
        {
            this.item = item;
            try
            {
                if (this.item.IsUpdated)
                {

                    ApplicationStatusItemNewIndicator.Visibility = ViewStates.Visible;
                    ApplicationStatusItemDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.tunaGrey)));
                }
                else
                {
                    ApplicationStatusItemNewIndicator.Visibility = ViewStates.Gone;
                    ApplicationStatusItemDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.receipt_note_text)));
                }

                ApplicationStatusItemTitle.Text = this.item.Type;
                ApplicationStatusItemStatus.Text = this.item.Status;
                ApplicationStatusItemSubTitle.Text = this.item.SrNumber;
                ApplicationStatusItemDate.Text = this.item.ApplicationDate.FormattedDate;

                if (colorMode != null)
                {
                    ApplicationStatusItemStatus.SetTextColor(Color.Rgb(colorMode.ColorList.Red, colorMode.ColorList.Green, colorMode.ColorList.Blue));
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}
