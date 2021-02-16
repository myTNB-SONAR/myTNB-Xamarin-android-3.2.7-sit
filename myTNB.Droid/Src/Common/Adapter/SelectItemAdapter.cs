using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Profile.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Common
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
            vh.txtItemTitle.TextSize = TextViewUtils.GetFontSize(16f);
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
                    txtItemTitle.TextSize = TextViewUtils.GetFontSize(16f);
                    if (mContext is AppLanguageActivity)
                    {
                        TextViewUtils.SetMuseoSans500Typeface(txtItemTitle);
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
