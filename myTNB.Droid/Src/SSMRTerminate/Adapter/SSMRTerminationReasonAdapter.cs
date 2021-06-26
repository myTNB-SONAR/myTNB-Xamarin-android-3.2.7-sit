using Android.Graphics;

using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.SSMRTerminate.Api;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.SSMRTerminate.Adapter
{
    public class SSMRTerminationReasonAdapter : RecyclerView.Adapter
    {
        private List<TerminationReasonModel> ssmrTerminationReasonList = new List<TerminationReasonModel>();

        public event EventHandler<int> ClickChanged;

        public SSMRTerminationReasonAdapter(List<TerminationReasonModel> data)
        {
            if (data == null)
            {
                this.ssmrTerminationReasonList.Clear();
                this.NotifyDataSetChanged();
            }
            else
            {
                this.ssmrTerminationReasonList.Clear();
                this.ssmrTerminationReasonList.AddRange(data);
                this.NotifyDataSetChanged();
            }
        }

        public override int ItemCount => ssmrTerminationReasonList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SSMRTerminationReasonViewHolder vh = holder as SSMRTerminationReasonViewHolder;

            TerminationReasonModel model = ssmrTerminationReasonList[position];

            vh.txtTerminationReason.Text = model.ReasonName;

            if (model.IsSelected)
            {
                vh.imageActionIcon.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.imageActionIcon.Visibility = ViewStates.Gone;
            }

            TextViewUtils.SetMuseoSans300Typeface(vh.txtTerminationReason);
            TextViewUtils.SetTextSize16(vh.txtTerminationReason);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.SSMRTerminationItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new SSMRTerminationReasonViewHolder(itemView, OnClick);
        }

        void OnClick(SSMRTerminationReasonViewHolder sender, int position)
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


        public class SSMRTerminationReasonViewHolder : RecyclerView.ViewHolder
        {

            public TextView txtTerminationReason { get; private set; }

            public LinearLayout rootView { get; private set; }

            public ImageView imageActionIcon { get; private set; }

            public SSMRTerminationReasonViewHolder(View itemView, Action<SSMRTerminationReasonViewHolder, int> listener) : base(itemView)
            {
                txtTerminationReason = itemView.FindViewById<TextView>(Resource.Id.txtTerminationReason);
                rootView = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                imageActionIcon = itemView.FindViewById<ImageView>(Resource.Id.imageActionIcon);

                rootView.Click += (s, e) => listener((this), base.LayoutPosition);
            }
        }

    }
}
