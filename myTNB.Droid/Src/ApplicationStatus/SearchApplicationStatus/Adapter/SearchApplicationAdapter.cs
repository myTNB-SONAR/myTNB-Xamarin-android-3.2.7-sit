using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile.API.Models.ApplicationStatus.GetApplicationsByCA;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Adapter
{
    public class SearchApplicationAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        //private List<ApplicationStatusColorCodeModel> mStatusCodeColorList = new List<ApplicationStatusColorCodeModel>();
        private List<GetApplicationsByCAModel> mApplicationStatusList = new List<GetApplicationsByCAModel>();
        public event EventHandler<int> ItemClick;

        public void Clear()
        {
            this.mApplicationStatusList.Clear();
            this.NotifyDataSetChanged();
        }

        public List<GetApplicationsByCAModel> GetApplicationStatusList()
        {
            return mApplicationStatusList;
        }

        public void UpdateAddList(List<GetApplicationsByCAModel> data)
        {
            this.mApplicationStatusList.AddRange(data);
            this.NotifyDataSetChanged();
        }

        public SearchApplicationAdapter(BaseActivityCustom activity, List<GetApplicationsByCAModel> data)
        {
            this.mActicity = activity;
            this.mApplicationStatusList.AddRange(data);
            //this.mStatusCodeColorList.AddRange(color);
        }

        public override int ItemCount => mApplicationStatusList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationStatusViewHolder vh = holder as ApplicationStatusViewHolder;

            GetApplicationsByCAModel item = mApplicationStatusList[position];
            //ApplicationStatusColorCodeModel color = mStatusCodeColorList.Find(x => x.Code == item.StatusCode);
            vh.PopulateData(item);
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
            {
                ItemClick(this, position);
            }
        }
    }

    public class ApplicationStatusViewHolder : RecyclerView.ViewHolder
    {
        public TextView ApplicationStatusItemTitle { get; private set; }
        public TextView ApplicationStatusItemSubTitle { get; private set; }
        public LinearLayout AppicationStatusListMainLayout { get; private set; }
        public TextView ApplicationStatusItemStatus { get; private set; }
        public ImageView ApplicationStatusItemNewIndicator { get; private set; }
        public ImageView ApplicationStatusItemRightArrow { get; private set; }
        private Context context;

        private GetApplicationsByCAModel item = null;

        public ApplicationStatusViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            context = itemView.Context;
            AppicationStatusListMainLayout = itemView.FindViewById<LinearLayout>(Resource.Id.appicationStatusListMainLayout);
            ApplicationStatusItemTitle = itemView.FindViewById<TextView>(Resource.Id.applicationStatusItemTitle);
            ApplicationStatusItemSubTitle = itemView.FindViewById<TextView>(Resource.Id.applicationStatusItemSubTitle);
            ApplicationStatusItemStatus = itemView.FindViewById<TextView>(Resource.Id.applicationStatusItemStatus);
            ApplicationStatusItemNewIndicator = itemView.FindViewById<ImageView>(Resource.Id.applicationStatusItemNewIndicator);
            ApplicationStatusItemRightArrow = itemView.FindViewById<ImageView>(Resource.Id.applicationStatusItemRightArrow);
            ApplicationStatusItemTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.tunaGrey)));
            ApplicationStatusItemSubTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.receipt_note_text)));
            ApplicationStatusItemStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.silverChalice)));
            TextViewUtils.SetMuseoSans300Typeface(ApplicationStatusItemSubTitle);
            TextViewUtils.SetMuseoSans500Typeface(ApplicationStatusItemTitle, ApplicationStatusItemStatus);
            AppicationStatusListMainLayout.Click += (sender, e) => listener(base.LayoutPosition);
        }

        public void PopulateData(GetApplicationsByCAModel item)
        {
            this.item = item;
            try
            {
                ApplicationStatusItemTitle.Text = this.item.ApplicationTypeDisplay;
                ApplicationStatusItemStatus.Text = this.item.StatusDisplay;
                ApplicationStatusItemSubTitle.Text = this.item.ReferenceIDDisplay;
                ApplicationStatusItemStatus.SetTextColor(Android.Graphics.Color.Rgb(this.item.StatusColor[0], this.item.StatusColor[1], this.item.StatusColor[2]));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}