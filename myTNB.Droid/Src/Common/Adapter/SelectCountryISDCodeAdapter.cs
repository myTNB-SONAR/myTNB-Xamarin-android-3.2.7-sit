using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.Common.Model;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.Common.Adapter
{
    public class SelectCountryISDCodeAdapter : BaseCustomAdapter<Country>, ISectionIndexer
    {
        private Dictionary<string, int> alphaIndex;
        string[] sections;
        Java.Lang.Object[] sectionsObjects;

        public SelectCountryISDCodeAdapter(Context context) : base(context)
        {
        }

        public SelectCountryISDCodeAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public SelectCountryISDCodeAdapter(Context context, List<Country> itemList) : base(context, itemList)
        {
            alphaIndex = new Dictionary<string, int>();
            for (int i = 0; i < itemList.Count; i++)
            {
                var key = itemList[i].name[0].ToString();
                if (!alphaIndex.ContainsKey(key))
                    alphaIndex.Add(key, i);
            }
            sections = new string[alphaIndex.Keys.Count];
            alphaIndex.Keys.CopyTo(sections, 0);
            sectionsObjects = new Java.Lang.Object[sections.Length];
            for (int i = 0; i < sections.Length; i++)
            {
                sectionsObjects[i] = new Java.Lang.String(sections[i]);
            }
        }

        public SelectCountryISDCodeAdapter(Context context, List<Country> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public int GetPositionForSection(int sectionIndex)
        {
            return alphaIndex[sections[sectionIndex]];
        }

        public int GetSectionForPosition(int position)
        {
            int prevSection = 0;

            for (int i = 0; i < sections.Length; i++)
            {
                if (GetPositionForSection(i) > position)
                {
                    break;
                }

                prevSection = i;
            }

            return prevSection;
        }

        public Java.Lang.Object[] GetSections()
        {
            return sectionsObjects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectCountryISDCodeItemLayout, parent, false);
            CountryISDCodeViewHolder viewHolder = new CountryISDCodeViewHolder(convertView);
            Country itemCountry = GetItemObject(position);
            int imageFlag = CountryUtil.Instance.GetFlagImageResource(context, itemCountry.code);
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
                TextViewUtils.SetTextSize16(countryISDCode, countryCountryName);
            }
        }
    }
}
