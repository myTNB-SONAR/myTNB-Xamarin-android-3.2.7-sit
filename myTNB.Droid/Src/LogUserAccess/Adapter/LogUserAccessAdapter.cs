using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB_Android.Src.LogUserAccess.Adapter
{
    public class LogUserAccessAdapter : BaseCustomAdapter<LogUserAccessNewData>
    {
        public LogUserAccessAdapter(Context context) : base(context)
        {
        }

        public LogUserAccessAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public LogUserAccessAdapter(Context context, List<LogUserAccessNewData> itemList) : base(context, itemList)
        {
        }

        public LogUserAccessAdapter(Context context, List<LogUserAccessNewData> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public void DisableAll()
        {
            foreach (LogUserAccessNewData data in itemList)
            {
                //data.IsSelected = false;
            }
            NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LogUserAccessNewData data = GetItemObject(position);
            LogUserAccessDataViewHolder viewHolder = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.ActivityLogUserListview, parent, false);
                viewHolder = new LogUserAccessDataViewHolder(convertView);
                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as LogUserAccessDataViewHolder;
            }
            try
            {
                DateTime referenceDate;
                DateTime referenceDate2 = new DateTime();
                referenceDate = data.CreatedDate;
                int day = referenceDate.Day;
                int month = referenceDate.Month;
                int year = referenceDate.Year;

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
                string formateddate = day.ToString() + " " + monthName.Substring(0,3) + " " + year.ToString();

                viewHolder.infoLabelDate.Text = formateddate;

                if (data.Action.Equals("A"))
                {
                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_action_tick));
                    viewHolder.itemTitle.Text = "Raja Udang added this electricity account to their view.";

                    /*if (data.IsApplyEBilling)
                    {
                        viewHolder.itemTitle.Text = "Raja Udang added this electricity account to their view.";
                    }*/
                }
                else if (data.Action.Equals("U"))
                {
                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.re_meter_dashboard));
                    viewHolder.itemTitle.Text = "You have granted Siti Aishah access to full bill view and apply for e-billing.";

                    /*if (data.IsApplyEBilling)
                    {
                        viewHolder.itemTitle.Text = "You have granted Siti Aishah access to full bill view and apply for e-billing.";
                    }*/
                }
                else
                {
                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.autopay_yellow));
                    viewHolder.itemTitle.Text = "You removed Siti Aishah’s access to apply for e-billing.";
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }

        class LogUserAccessDataViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.itemTitle)]
            public TextView itemTitle;

            [BindView(Resource.Id.infoLabelDate)]
            public TextView infoLabelDate;

            [BindView(Resource.Id.itemIcon)]
            public ImageView itemIcon;

            public LogUserAccessDataViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(infoLabelDate);

                TextViewUtils.SetMuseoSans500Typeface(itemTitle);
            }
        }
    }
}