
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.LogUserAccess.MVP;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB_Android.Src.LogUserAccess.Adapter
{
    public class LogUserAccessRecyclerviewAdapter : RecyclerView.Adapter
    {
        private Android.App.Activity mActicity;
        private List<LogUserAccessNewData> logList = new List<LogUserAccessNewData>();

        public LogUserAccessRecyclerviewAdapter(Android.App.Activity acticity, List<LogUserAccessNewData> data)
        {
            this.mActicity = acticity;
            this.logList.AddRange(data);
        }

        public event EventHandler<int> ClickEvent;

        public override int ItemCount => logList.Count;

        public void OnClickEvent(int position)
        {
            if (ClickEvent != null)
            {
                ClickEvent(this, position);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                LogUserAccessNewData data = logList[position];

                var viewHolder = holder as LogUserAccessViewHolder;
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

                    viewHolder.infoLabelDate.Text = formateddate;

                    if (data.Action.Equals("A"))
                    {
                        if (UserEntity.GetActive().Email == data.CreateBy)
                        {
                            //viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(viewHolder, Resource.Drawable.icons_activity_log_joined));
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_joined));

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
                        else
                        {
                            //viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(viewHolder, Resource.Drawable.icons_activity_log_joined));
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_joined));

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
                        viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_confirmed));

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
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_confirmed));

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
                        else if (data.IsApplyEBilling && !data.IsHaveAccess)
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_confirmed));

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
                        else
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_denied));
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
                    }
                    else
                    { 
                        if (UserEntity.GetActive().Email == data.CreateBy)
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_left));
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
                        else
                        {
                            viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_left));
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
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ActivityLogUserListview;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new LogUserAccessViewHolder(itemView);
        }

        class LogUserAccessViewHolder : RecyclerView.ViewHolder
        {
            public ImageView itemIcon { get; private set; }
            public TextView itemTitle { get; private set; }
            public TextView infoLabelDate { get; private set; }

            public LogUserAccessViewHolder(View itemView) : base(itemView)
            {
                itemIcon = itemView.FindViewById<ImageView>(Resource.Id.itemIcon);
                infoLabelDate = itemView.FindViewById<TextView>(Resource.Id.infoLabelDate);
                itemTitle = itemView.FindViewById<TextView>(Resource.Id.itemTitle);

                TextViewUtils.SetMuseoSans300Typeface(infoLabelDate);
                TextViewUtils.SetMuseoSans500Typeface(itemTitle);
                TextViewUtils.SetTextSize12(infoLabelDate);
                TextViewUtils.SetTextSize14(itemTitle);

            }
        }
    }
}