using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.SelectFeedbackType.Adapter
{
    public class SelectFeedbackTypeAdapter : BaseCustomAdapter<FeedbackType>
    {
        public SelectFeedbackTypeAdapter(Context context) : base(context)
        {
        }

        public SelectFeedbackTypeAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public SelectFeedbackTypeAdapter(Context context, List<FeedbackType> itemList) : base(context, itemList)
        {
        }

        public SelectFeedbackTypeAdapter(Context context, List<FeedbackType> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            FeedbackTypeViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectFeedbackTypeRow, parent, false);
                vh = new FeedbackTypeViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as FeedbackTypeViewHolder;

            }

            try
            {
                FeedbackType item = GetItemObject(position);
                vh.txtFeedbackType.Text = item.FeedbackTypeName;


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

    public class FeedbackTypeViewHolder : BaseAdapterViewHolder
    {
        [BindView(Resource.Id.txtFeedbackType)]
        public TextView txtFeedbackType;

        [BindView(Resource.Id.imageActionIcon)]
        public ImageView imageActionIcon;



        public FeedbackTypeViewHolder(View itemView) : base(itemView)
        {
            TextViewUtils.SetMuseoSans300Typeface(txtFeedbackType);
            TextViewUtils.SetTextSize16(txtFeedbackType);
        }
    }

}