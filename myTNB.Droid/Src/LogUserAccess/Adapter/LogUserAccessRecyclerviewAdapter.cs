
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.LogUserAccess.Models;
using myTNB.Android.Src.LogUserAccess.MVP;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB.Android.Src.LogUserAccess.Adapter
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
                    bool isWhiteList = UserSessions.GetWhiteList(PreferenceManager.GetDefaultSharedPreferences(mActicity));

                    viewHolder.infoLabelDate.Text = formateddate;

                    if (data.Action.Equals("A"))
                    {
                        if (UserEntity.GetActive().Email == data.CreateBy) //owner view
                        {
                            if (data.IsPreRegister)
                            {
                                viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_joined));

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
                            
                        }
                        else if (UserEntity.GetActive().Email != data.CreateBy && isWhiteList == true && isWhiteList != null) //admin view
                        {
                            if (data.CreateByName == data.UserName)
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
                            else
                            {
                                if (data.IsPreRegister)
                                {
                                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_joined));
                                    string txtdata = Utility.GetLocalizedLabel("UserAccess", "AddNonTNBUserSuccessAdmin");
                                    string temp = string.Format(txtdata, data.CreateByName, data.UserName);
                                    //string details = string.Format(temp, data.UserName);

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
                                    viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_joined));
                                    string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccountUserAccesssLogAdmin");
                                    string temp = string.Format(txtdata, data.CreateByName, data.UserName);
                                    //string details = string.Format(temp, data.UserName);

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
                        else // Non-owner add ca view
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
                        else if (!data.IsApplyEBilling && data.IsHaveAccess) //give full bill view (tick)
                        {
                            if (UserEntity.GetActive().Email == data.CreateBy) //done by owner
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
                            else //done by admin
                            {
                                viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_confirmed));

                                string txtdata = Utility.GetLocalizedLabel("UserAccess", "addAccesssLogfullBillAdmin");
                                string temp = string.Format(txtdata, data.CreateByName, data.UserName);
                                //string details = string.Format(temp, data.UserName);

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
                        else  // remove access for view full bill (untick)
                        {
                            if (UserEntity.GetActive().Email == data.CreateBy) //done by owner
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
                            else //done by admin
                            {
                                viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_denied));
                                string txtdata = Utility.GetLocalizedLabel("UserAccess", "removeAccesssLogBothAdmin");
                                string temp = string.Format(txtdata, data.CreateByName, data.UserName);
                                //string details = string.Format(temp, data.UserName);

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
                        else if (UserEntity.GetActive().Email != data.CreateBy && isWhiteList == true)
                        {
                            if (data.CreateByName == data.UserName)
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
                            else
                            {
                                viewHolder.itemIcon.SetImageDrawable(ContextCompat.GetDrawable(mActicity, Resource.Drawable.icons_activity_log_left));
                                string txtData = Utility.GetLocalizedLabel("UserAccess", "removeAccountUserAccesssLogAdmin");
                                string temp2 = string.Format(txtData, data.CreateByName, data.UserName);

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