using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Common.Adapter
{
    public class SelectCountryISDCodeAdapter : BaseCustomAdapter<Country>
    {
        public SelectCountryISDCodeAdapter(Context context) : base(context)
        {
        }

        public SelectCountryISDCodeAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public SelectCountryISDCodeAdapter(Context context, List<Country> itemList) : base(context, itemList)
        {
        }

        public SelectCountryISDCodeAdapter(Context context, List<Country> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectCountryISDCodeItemLayout, parent, false);
            CountryISDCodeViewHolder viewHolder = new CountryISDCodeViewHolder(convertView);
            Country itemCountry = GetItemObject(position);
            int imageFlag = CountryUtil.Instance.GetFlagImageResource(context,itemCountry.code);
            viewHolder.flagImage.SetImageResource(imageFlag);
            viewHolder.countryISDCode.Text = itemCountry.isd;
            viewHolder.countryCountryName.Text = itemCountry.name;

            if (position == (Count - 1))
            {
                viewHolder.linearLayoutSeparator.Visibility = ViewStates.Gone;
            }
            else
            {
                viewHolder.linearLayoutSeparator.Visibility = ViewStates.Visible;
            }

            return convertView;
        }

        public class CountryISDCodeViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.imgCountryFlag)]
            public ImageView flagImage;

            [BindView(Resource.Id.txtCountryISDCode)]
            public TextView countryISDCode;

            [BindView(Resource.Id.txtCountryName)]
            public TextView countryCountryName;

            [BindView(Resource.Id.separator)]
            public LinearLayout linearLayoutSeparator;

            public CountryISDCodeViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(countryISDCode, countryCountryName);
            }
        }
    }
}
