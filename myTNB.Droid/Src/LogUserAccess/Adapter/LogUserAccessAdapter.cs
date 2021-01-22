﻿using Android.Content;
using Android.OS;
using Android.Text;
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
                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_joined));
                    string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccountUserAccesssLog");
                    string temp = string.Format(txtdata, data.UserName);

                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp);
                    }
                }
                else if (data.Action.Equals("U"))
                {
                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_confirmed));

                    if (data.IsApplyEBilling && data.IsHaveAccess)
                    {
                        string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccesssLogBoth");
                        string temp = string.Format(txtdata, data.UserName);

                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp);
                        }
                    }
                    else if (!data.IsApplyEBilling && data.IsHaveAccess)
                    {
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_denied));

                        string txtdata = Utility.GetLocalizedLabel("UserAccess", "removeAccesssLogeBilling");
                        string temp = string.Format(txtdata, data.UserName);

                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp);
                        }
                    }
                    else if (data.IsApplyEBilling && !data.IsHaveAccess)
                    {
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_denied));

                        string txtdata = Utility.GetLocalizedLabel("UserAccess", "removeAccesssLogfullBill");
                        string temp = string.Format(txtdata, data.UserName);

                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp);
                        }
                    }
                    else
                    {
                        string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccesssLogfullBill");
                        string temp = string.Format(txtdata, data.UserName);

                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp);
                        }
                    }
                }
                else
                {
                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_left));
                    string txtdata = Utility.GetLocalizedLabel("UserAccess", "removeAccountUserAccesssLog");
                    string temp = string.Format(txtdata, data.UserName);

                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp);
                    }
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