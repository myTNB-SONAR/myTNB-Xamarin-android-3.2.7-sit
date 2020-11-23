using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.FindUs.Models;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.FindUs.Adapter
{
    public class LocationTypeAdapter : BaseCustomAdapter<LocationType>
    {

        public LocationTypeAdapter(Android.Content.Context context) : base(context)
        {
        }

        public LocationTypeAdapter(Android.Content.Context context, bool notify) : base(context, notify)
        {
        }

        public LocationTypeAdapter(Android.Content.Context context, List<LocationType> itemList) : base(context, itemList)
        {
        }

        public LocationTypeAdapter(Android.Content.Context context, List<LocationType> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LocationTypeViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.LocationTypeRow, parent, false);
                vh = new LocationTypeViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as LocationTypeViewHolder;

            }

            LocationType item = GetItemObject(position);
            vh.txtLocationType.Text = item.Description;

            if (item.IsSelected)
            {
                vh.imageActionIcon.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.imageActionIcon.Visibility = ViewStates.Gone;

            }

            return convertView;
        }

        public class LocationTypeViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtLocationType)]
            public TextView txtLocationType;

            [BindView(Resource.Id.imageActionIcon)]
            public ImageView imageActionIcon;

            public LocationTypeViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtLocationType);
                txtLocationType.TextSize = TextViewUtils.GetFontSize(16f);

            }
        }
    }
}