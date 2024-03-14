using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.Profile.Activity;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.Common
{
    public class SelectItemAdapter : BaseCustomAdapter<Item>
    {
        Context mContext;

        public SelectItemAdapter(Context context) : base(context)
        {
            mContext = context;
        }

        public SelectItemAdapter(Context context, bool notify) : base(context, notify)
        {
            mContext = context;
        }

        public SelectItemAdapter(Context context, List<Item> itemList) : base(context, itemList)
        {
            mContext = context;
        }

        public SelectItemAdapter(Context context, List<Item> itemList, bool notify) : base(context, itemList, notify)
        {
            mContext = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SelectItemViewHolder vh;
            convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectItemListLayout, parent, false);
            vh = new SelectItemViewHolder(convertView, mContext);
            Item item = GetItemObject(position);
            vh.txtItemTitle.Text = item.title;
            //TextViewUtils.SetTextSize16(vh.txtItemTitle);
            vh.imageActionIcon.Visibility = item.selected ? ViewStates.Visible : ViewStates.Gone;
            return convertView;
        }

        public class SelectItemViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtItemTitle)]
            public TextView txtItemTitle;

            [BindView(Resource.Id.imageActionIcon)]
            public ImageView imageActionIcon;

            public SelectItemViewHolder(View itemView, Context mContext) : base(itemView)
            {
                try
                {
                    TextViewUtils.SetTextSize14(txtItemTitle);
                    if (mContext is AppLanguageActivity)
                    {
                        TextViewUtils.SetMuseoSans300Typeface(txtItemTitle);
                    }
                    else
                    {
                        TextViewUtils.SetMuseoSans300Typeface(txtItemTitle);
                    }
                }
                catch (Exception e)
                {
                    TextViewUtils.SetMuseoSans300Typeface(txtItemTitle);
                    Utility.LoggingNonFatalError(e);
                }
            }
        }
    }
}