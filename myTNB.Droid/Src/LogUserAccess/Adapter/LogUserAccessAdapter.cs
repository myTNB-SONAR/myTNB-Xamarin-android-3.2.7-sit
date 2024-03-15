using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.LogUserAccess.Models;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB.AndroidApp.Src.LogUserAccess.Adapter
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
                string formateddate = day.ToString() + " " + monthName.Substring(0, 3) + " " + year.ToString();
                bool isWhiteList = UserSessions.GetWhiteList(PreferenceManager.GetDefaultSharedPreferences(this.context));

                viewHolder.infoLabelDate.Text = formateddate;

                if (data.Action.Equals("A"))
                {
                    if (UserEntity.GetActive().Email == data.CreateBy) //owner view
                    {
                        if (data.IsPreRegister)
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_joined));
                            string txtdata = Utility.GetLocalizedLabel("UserAccess", "AddNonTNBUserSuccess");
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

                    }
                    else if (UserEntity.GetActive().Email != data.CreateBy && isWhiteList == true) //admin view
                    {
                        if (data.IsPreRegister)
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_joined));
                            string txtdata = Utility.GetLocalizedLabel("UserAccess", "AddNonTNBUserSuccessAdmin");
                            string temp = string.Format(txtdata, data.CreateByName);
                            string details = string.Format(temp, data.UserName);

                            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details);
                            }
                        }
                        else
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_joined));
                            string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccountUserAccesssLogAdmin");
                            string temp = string.Format(txtdata, data.CreateByName);
                            string details = string.Format(temp, data.UserName);

                            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details);
                            }

                        }

                    }
                    else // Non-owner add ca view
                    {
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_joined));
                        string txtData = Utility.GetLocalizedLabel("UserAccess", "addAccountUserLog");
                        string temp2 = string.Format(txtData, data.UserName);

                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp2, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp2);
                        }
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
                    else if (!data.IsApplyEBilling && data.IsHaveAccess) //give full bill view (tick)
                    {
                        if (UserEntity.GetActive().Email == data.CreateBy) //done by owner
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_confirmed));

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
                        else //done by admin
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_confirmed));

                            string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccesssLogfullBillAdmin");
                            string temp = string.Format(txtdata, data.CreateByName);
                            string details = string.Format(temp, data.UserName);

                            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details);
                            }
                        }
                        
                    }
                    else if (data.IsApplyEBilling && !data.IsHaveAccess)
                    {
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_confirmed));

                        string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccesssLogeBilling");
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
                    else // remove access for view full bill (untick)
                    {
                        if (UserEntity.GetActive().Email == data.CreateBy) //done by owner
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_denied));
                            string txtdata = Utility.GetLocalizedLabel("UserAccess", "removeAccesssLogBoth");
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
                        else //done by admin
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_denied));
                            string txtdata = Utility.GetLocalizedLabel("UserAccess", "removeAccesssLogBothAdmin");
                            string temp = string.Format(txtdata, data.CreateByName);
                            string details = string.Format(temp, data.UserName);

                            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                viewHolder.itemTitle.TextFormatted = Html.FromHtml(details);
                            }
                        }
                       
                    }
                }
                else
                {

                    if (UserEntity.GetActive().Email == data.CreateBy)
                    {
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_left));
                        string txtData = Utility.GetLocalizedLabel("UserAccess", "removeAccountUserAccesssLog");
                        string temp2 = string.Format(txtData, data.UserName);

                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp2, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(temp2);
                        }
                    }
                    else if (UserEntity.GetActive().Email != data.CreateBy && isWhiteList == true)
                    {
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_left));
                        string txtData = Utility.GetLocalizedLabel("UserAccess", "removeAccountUserAccesssLogAdmin");
                        string temp2 = string.Format(txtData, data.CreateByName);
                        string details = string.Format(temp2, data.UserName);

                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(details, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            viewHolder.itemTitle.TextFormatted = Html.FromHtml(details);
                        }
                    }
                    else
                    {
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.icons_activity_log_left));
                        string txtdata = Utility.GetLocalizedLabel("UserAccess", "removeAccountLog");
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
                TextViewUtils.SetTextSize12(infoLabelDate);
                TextViewUtils.SetTextSize14(itemTitle);

                ViewGroup.LayoutParams layoutParams = itemTitle.LayoutParameters;
                //layoutParams.Height = tesst;
                if (TextViewUtils.IsLargeFonts)
                {
                    layoutParams.Height = layoutParams.Height + 100;
                }
                itemTitle.LayoutParameters = (layoutParams);
                itemTitle.RequestLayout();
            }
        }
    }
}