using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Adapter
{
    public class ApplicationStatusDetailProgressAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        private List<StatusTrackerDisplay> mProgressList = new List<StatusTrackerDisplay>();
        public event EventHandler<int> ItemClick;
        public bool IsPayment;
        public int statusDateCount;
        public void Clear()
        {
            this.mProgressList.Clear();
            this.NotifyDataSetChanged();
        }

        public ApplicationStatusDetailProgressAdapter(BaseActivityCustom activity, List<StatusTrackerDisplay> data, bool IsPayment)
        {
            this.IsPayment = IsPayment;
            this.mActicity = activity;
            this.mProgressList.AddRange(data);
        }

        public override int ItemCount => mProgressList.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationStatusDetailItemStatusLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ApplicationDetailProgressViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationDetailProgressViewHolder vh = holder as ApplicationDetailProgressViewHolder;

            StatusTrackerDisplay item = mProgressList[position];
            statusDateCount += vh.PopulateData(item, mProgressList, position, this.IsPayment);

        }

        public class ApplicationDetailProgressViewHolder : RecyclerView.ViewHolder
        {
            public ImageView ImgApplicationStatusGreen { get; private set; }
            public ImageView ImgApplicationStatusOrange { get; private set; }
            public ImageView ImgApplicationStatusGray { get; private set; }
            public ImageView ImgApplicationStatusnLightGray { get; private set; }
            
            public View ApplicationStatusLine { get; private set; }
            public View ApplicationStatusLineInactive { get; private set; }
            public View applicationStatusLine { get; private set; }

            public TextView TxtApplicationStatusDetailWord { get; private set; }
            public TextView TxtApplicationStatusDetailCTA { get; private set; }

            private Context context;
            public int statusDateCount;

            private StatusTrackerDisplay item = null;

            public ApplicationDetailProgressViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                context = itemView.Context;
                ImgApplicationStatusGreen = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationStatusGreen);
                ImgApplicationStatusOrange = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationStatusOrange);
                ImgApplicationStatusGray = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationStatusGray);
                ImgApplicationStatusnLightGray = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationStatusnLightGray);

                ApplicationStatusLine = itemView.FindViewById<View>(Resource.Id.applicationStatusLine);
                ApplicationStatusLineInactive = itemView.FindViewById<View>(Resource.Id.applicationStatusLineInactive);
                TxtApplicationStatusDetailWord = itemView.FindViewById<TextView>(Resource.Id.txtApplicationStatusDetailWord);
                
                TxtApplicationStatusDetailCTA = itemView.FindViewById<TextView>(Resource.Id.txtApplicationStatusDetailCTA);
                //TxtApplicationStatusDetailCTA.Clickable = true;
                //TxtApplicationStatusDetailCTA.Click += (sender, e) => listener(base.LayoutPosition);
            }


            public int PopulateData(StatusTrackerDisplay item, List<StatusTrackerDisplay> mProgressList, int position, bool IsPayment)
            {
                this.item = item;
                try
                {
                    //  TODO: ApplicationStatus Setup whole view
                    ImgApplicationStatusGreen.Visibility = ViewStates.Gone;
                    ImgApplicationStatusOrange.Visibility = ViewStates.Gone;
                    ImgApplicationStatusGray.Visibility = ViewStates.Gone;
                    ImgApplicationStatusnLightGray.Visibility = ViewStates.Gone;
                    ApplicationStatusLine.Visibility = ViewStates.Gone;
                    TextViewUtils.SetMuseoSans300Typeface(TxtApplicationStatusDetailWord, TxtApplicationStatusDetailCTA);

                    TxtApplicationStatusDetailWord.Text = item.StatusDescription;
                    TxtApplicationStatusDetailWord.TextSize = TextViewUtils.GetFontSize(14);
                 if (string.IsNullOrEmpty(item.CompletedDateDisplay) || string.IsNullOrWhiteSpace(item.CompletedDateDisplay))
                    {
                        TxtApplicationStatusDetailCTA.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        TxtApplicationStatusDetailCTA.Text = item.CompletedDateDisplay;
                        TxtApplicationStatusDetailCTA.Visibility = ViewStates.Visible;
                        statusDateCount++;
                    }

                    TxtApplicationStatusDetailWord.Text = item.StatusDescription;
                    TextViewUtils.SetMuseoSans300Typeface(TxtApplicationStatusDetailWord);

                    if(item.TrackerItemState == State.Active)
                    {
                        if (IsPayment && mProgressList.Count != position + 1)
                        {
                            ImgApplicationStatusOrange.Visibility = ViewStates.Visible;
                            TextViewUtils.SetMuseoSans500Typeface(TxtApplicationStatusDetailWord);
                        }
                        if (!IsPayment)
                        {
                            ImgApplicationStatusGray.Visibility = ViewStates.Visible;
                            TextViewUtils.SetMuseoSans500Typeface(TxtApplicationStatusDetailWord);
                        }
                        if (mProgressList.Count != position + 1)
                        {
                            ApplicationStatusLineInactive.Visibility = ViewStates.Visible;
                            TextViewUtils.SetMuseoSans500Typeface(TxtApplicationStatusDetailWord);
                        }

                    }
                    else if (item.TrackerItemState == State.Completed || item.TrackerItemState == State.Past)
                    {
                        ImgApplicationStatusGreen.Visibility = ViewStates.Visible;
                        if (mProgressList.Count != position + 1)
                        {
                            ApplicationStatusLine.Visibility = ViewStates.Visible;
                        }
                    }
                    else if (item.TrackerItemState == State.Inactive)
                    {
                        TxtApplicationStatusDetailWord.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.silverChalice)));
                        ImgApplicationStatusnLightGray.Visibility = ViewStates.Visible;
                        if (mProgressList.Count != position + 1)
                        {
                            ApplicationStatusLineInactive.Visibility = ViewStates.Visible;
                        }
                    }
                    return statusDateCount;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
                return statusDateCount;
            }

        }
    }
}