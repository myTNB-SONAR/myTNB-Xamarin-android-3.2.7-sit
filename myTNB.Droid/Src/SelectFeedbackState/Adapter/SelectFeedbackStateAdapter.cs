using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.SelectFeedbackState.Adapter
{
    public class SelectFeedbackStateAdapter : BaseCustomAdapter<FeedbackState>
    {
        public SelectFeedbackStateAdapter(Context context) : base(context)
        {
        }

        public SelectFeedbackStateAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public SelectFeedbackStateAdapter(Context context, List<FeedbackState> itemList) : base(context, itemList)
        {
        }

        public SelectFeedbackStateAdapter(Context context, List<FeedbackState> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            FeedbackStateViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectFeedbackStateRow, parent, false);
                vh = new FeedbackStateViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as FeedbackStateViewHolder;

            }
            try
            {
                FeedbackState item = GetItemObject(position);
                vh.txtFeedbackState.Text = item.StateName;

                if (item.IsSelected)
                {
                    vh.imageActionIcon.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.imageActionIcon.Visibility = ViewStates.Gone;

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }

    }

    public class FeedbackStateViewHolder : BaseAdapterViewHolder
    {
        [BindView(Resource.Id.txtFeedbackState)]
        public TextView txtFeedbackState;

        [BindView(Resource.Id.imageActionIcon)]
        public ImageView imageActionIcon;

        public FeedbackStateViewHolder(View itemView) : base(itemView)
        {
            TextViewUtils.SetMuseoSans300Typeface(txtFeedbackState);
            txtFeedbackState.TextSize = TextViewUtils.GetFontSize(16f);
        }
    }

}