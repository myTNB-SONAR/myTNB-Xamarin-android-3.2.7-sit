using Android.Graphics;
using Android.Preferences;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyHome.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Android.Graphics.ColorSpace;
using ServiceEnum = myTNB.Mobile.MobileEnums.ServiceEnum;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class MyServiceAdapter : RecyclerView.Adapter
    {
        List<MyServiceModel> myServicesList = new List<MyServiceModel>();

        public event EventHandler<int> ClickChanged;

        private Android.App.Activity mActivity;

        private bool isRefreshShown = false;

        public MyServiceAdapter(List<MyServiceModel> data, Android.App.Activity Activity, bool currentRefresh)
        {
            if (data == null)
            {
                this.myServicesList.Clear();
            }
            else
            {
                this.myServicesList = data;
            }
            this.mActivity = Activity;

            this.isRefreshShown = currentRefresh;
        }

        public override int ItemCount => myServicesList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {

                MyServiceViewHolder vh = holder as MyServiceViewHolder;

                MyServiceModel model = myServicesList[position];

                string timestamp = UserSessions.GetServicesTimeStamp(PreferenceManager.GetDefaultSharedPreferences(this.mActivity));
                MyServiceIconEntity iconEntity = new MyServiceIconEntity()
                {
                    ServiceId = model.ServiceId,
                    ServiceIconUrl = model.ServiceIconUrl,
                    DisabledServiceIconUrl = model.DisabledServiceIconUrl,
                    ServiceBannerUrl = model.ServiceBannerUrl,
                    TimeStamp = timestamp
                };

                try
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        if (model.ServiceName.Contains("<br/>") || model.ServiceName.Contains("\n"))
                        {
                            string newStringValue = "";
                            if (model.ServiceName.Contains("\n"))
                            {
                                newStringValue = model.ServiceName.Replace("\n", "<br/>");
                            }
                            else if (model.ServiceName.Contains("\r\n"))
                            {
                                newStringValue = model.ServiceName.Replace("\r\n", "<br/>");
                            }
                            else
                            {
                                newStringValue = model.ServiceName;
                            }
                            vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            string[] splittedString = model.ServiceName.Trim().Split(" ");
                            string newStringName = "";
                            if (splittedString.Length > 4)
                            {
                                for (int i = 0; i < splittedString.Length; i++)
                                {
                                    if (i == 2)
                                    {
                                        newStringName += splittedString[i] + "<br/>";
                                    }
                                    else if (i == splittedString.Length - 1)
                                    {
                                        newStringName += splittedString[i];
                                    }
                                    else
                                    {
                                        newStringName += splittedString[i] + " ";
                                    }
                                }
                            }
                            else if (splittedString.Length == 3 || splittedString.Length == 4)
                            {
                                for (int i = 0; i < splittedString.Length; i++)
                                {
                                    if (i == 1)
                                    {
                                        newStringName += splittedString[i] + "<br/>";
                                    }
                                    else if (i == splittedString.Length - 1)
                                    {
                                        newStringName += splittedString[i];
                                    }
                                    else
                                    {
                                        newStringName += splittedString[i] + " ";
                                    }
                                }
                            }
                            else if (splittedString.Length == 2)
                            {
                                for (int i = 0; i < splittedString.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        newStringName += splittedString[i] + "<br/>";
                                    }
                                    else if (i == splittedString.Length - 1)
                                    {
                                        newStringName += splittedString[i];
                                    }
                                    else
                                    {
                                        newStringName += splittedString[i] + " ";
                                    }
                                }
                            }
                            else
                            {
                                newStringName = model.ServiceName;
                            }

                            vh.serviceTitle.TextFormatted = Html.FromHtml(newStringName, FromHtmlOptions.ModeLegacy);
                        }
                    }
                    else
                    {
                        if (model.ServiceName.Contains("<br/>") || model.ServiceName.Contains("\n"))
                        {
                            string newStringValue = "";
                            if (model.ServiceName.Contains("\n"))
                            {
                                newStringValue = model.ServiceName.Replace("\n", "<br/>");
                            }
                            else if (model.ServiceName.Contains("\r\n"))
                            {
                                newStringValue = model.ServiceName.Replace("\r\n", "<br/>");
                            }
                            else
                            {
                                newStringValue = model.ServiceName;
                            }
                            vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue);
                        }
                        else
                        {
                            string[] splittedString = model.ServiceName.Trim().Split(" ");
                            string newStringName = "";
                            if (splittedString.Length > 4)
                            {
                                for (int i = 0; i < splittedString.Length; i++)
                                {
                                    if (i == 2)
                                    {
                                        newStringName += splittedString[i] + "<br/>";
                                    }
                                    else if (i == splittedString.Length - 1)
                                    {
                                        newStringName += splittedString[i];
                                    }
                                    else
                                    {
                                        newStringName += splittedString[i] + " ";
                                    }
                                }
                            }
                            else if (splittedString.Length == 3 || splittedString.Length == 4)
                            {
                                for (int i = 0; i < splittedString.Length; i++)
                                {
                                    if (i == 1)
                                    {
                                        newStringName += splittedString[i] + "<br/>";
                                    }
                                    else if (i == splittedString.Length - 1)
                                    {
                                        newStringName += splittedString[i];
                                    }
                                    else
                                    {
                                        newStringName += splittedString[i] + " ";
                                    }
                                }
                            }
                            else if (splittedString.Length == 3)
                            {
                                for (int i = 0; i < splittedString.Length; i++)
                                {
                                    if (i == 1)
                                    {
                                        newStringName += splittedString[i] + "<br/>";
                                    }
                                    else if (i == splittedString.Length - 1)
                                    {
                                        newStringName += splittedString[i];
                                    }
                                    else
                                    {
                                        newStringName += splittedString[i] + " ";
                                    }
                                }
                            }
                            else if (splittedString.Length == 2)
                            {
                                for (int i = 0; i < splittedString.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        newStringName += splittedString[i] + "<br/>";
                                    }
                                    else if (i == splittedString.Length - 1)
                                    {
                                        newStringName += splittedString[i];
                                    }
                                    else
                                    {
                                        newStringName += splittedString[i] + " ";
                                    }
                                }
                            }
                            else
                            {
                                newStringName = model.ServiceName;
                            }

                            vh.serviceTitle.TextFormatted = Html.FromHtml(newStringName);
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                try
                {
                    switch (model.ServiceType)
                    {
                        case ServiceEnum.SELFMETERREADING:
                            DynamicIconHandling(vh, model, Resource.Drawable.submit_meter);
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "selfMeterReading"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "selfMeterReading"));
                            }
                            if (UserSessions.HasSMROnboardingShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                            {
                                vh.newLabel.Visibility = ViewStates.Gone;
                            }
                            break;
                        case ServiceEnum.SUBMITFEEDBACK:
                            DynamicIconHandling(vh, model, Resource.Drawable.feedback);
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "submitEnquiry"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "submitEnquiry"));
                            }
                            vh.newLabel.Visibility = ViewStates.Gone;
                            break;
                        case ServiceEnum.PAYBILL:
                            if (Utility.IsEnablePayment() && !isRefreshShown && MyTNBAccountManagement.GetInstance().IsPayBillEnabledNeeded())
                            {
                                DynamicIconHandling(vh, model, Resource.Drawable.bills);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.powerBlue)));
                            }
                            else
                            {
                                DynamicIconHandling(vh, model, Resource.Drawable.bills_disabled, true);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.grey_two)));
                            }
                            if (MyTNBAccountManagement.GetInstance().IsHasNonREAccountCount() > 1)
                            {
                                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                {
                                    vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "payBills"), FromHtmlOptions.ModeLegacy);
                                }
                                else
                                {
                                    vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "payBills"));
                                }
                            }
                            else
                            {
                                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                {
                                    vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "payBill"), FromHtmlOptions.ModeLegacy);
                                }
                                else
                                {
                                    vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "payBill"));
                                }
                            }
                            if (UserSessions.HasPayBillShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                            {
                                vh.newLabel.Visibility = ViewStates.Gone;
                            }
                            break;
                        case ServiceEnum.VIEWBILL:
                            if (!isRefreshShown && MyTNBAccountManagement.GetInstance().IsViewBillEnabledNeeded())
                            {
                                DynamicIconHandling(vh, model, Resource.Drawable.pdf_bill);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.powerBlue)));
                            }
                            else
                            {
                                DynamicIconHandling(vh, model, Resource.Drawable.pdf_bill_disabled, true);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.grey_two)));
                            }

                            bool isHasNonREAccount = MyTNBAccountManagement.GetInstance().IsHasNonREAccountCount() > 0;
                            bool isHasMoreThanOneNonREAccount = MyTNBAccountManagement.GetInstance().IsHasNonREAccountCount() > 1;
                            bool isHasREAccount = MyTNBAccountManagement.GetInstance().IsHasREAccountCount() > 0;
                            bool isHasMoreThanOneREAccount = MyTNBAccountManagement.GetInstance().IsHasREAccountCount() > 1;

                            string newStringValue = Utility.GetLocalizedLabel("DashboardHome", "viewEBill");

                            if (isHasNonREAccount && isHasREAccount)
                            {
                                newStringValue = Utility.GetLocalizedLabel("DashboardHome", "viewEBillAndAdvice");
                            }
                            else if (isHasREAccount && !isHasNonREAccount)
                            {
                                newStringValue = Utility.GetLocalizedLabel("DashboardHome", "viewAdvice");
                                if (isHasMoreThanOneREAccount)
                                {
                                    newStringValue = Utility.GetLocalizedLabel("DashboardHome", "viewAdvices");
                                }
                            }
                            else if (isHasNonREAccount && isHasMoreThanOneNonREAccount && !isHasREAccount)
                            {
                                newStringValue = Utility.GetLocalizedLabel("DashboardHome", "viewEBills");
                            }


                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue);
                            }

                            if (UserSessions.HasViewBillShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                            {
                                vh.newLabel.Visibility = ViewStates.Gone;
                            }
                            break;
                        case ServiceEnum.APPLICATIONSTATUS:
                            DynamicIconHandling(vh, model, Resource.Drawable.check_status);
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "applicationStatus"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "applicationStatus"));
                            }
                            if (UserSessions.HasApplicationStatusShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                            {
                                vh.newLabel.Visibility = ViewStates.Gone;
                            }
                            break;
                        case ServiceEnum.ENERGYBUDGET:
                            if (!isRefreshShown && Utility.IsMDMSDownEnergyBudget() && UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify())
                            {
                                DynamicIconHandling(vh, model, Resource.Drawable.Check_Status_Icon);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.powerBlue)));
                            }
                            else
                            {
                                DynamicIconHandling(vh, model, Resource.Drawable.Energy_Budget_grey, true);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.grey_two)));
                            }
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "energyBudget"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "energyBudget"));
                            }
                            if (UserSessions.HasSmartMeterShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                            {
                                vh.newLabel.Visibility = ViewStates.Gone;
                            }
                            break;
                        case ServiceEnum.MYHOME:
                            DynamicIconHandling(vh, model, Resource.Drawable.Icon_Quick_Access_MyHome);

                            if (UserSessions.MyHomeQuickLinkHasShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                            {
                                vh.newLabel.Visibility = ViewStates.Gone;
                            }
                            break;
                        case ServiceEnum.VIEWMORE:
                            DynamicIconHandling(vh, model, Resource.Drawable.ic_less_more);

                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "showMore"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "showMore"));
                            }
                            vh.newLabel.Visibility = ViewStates.Gone;
                            break;
                        case ServiceEnum.VIEWLESS:
                            DynamicIconHandling(vh, model, Resource.Drawable.ic_less_more);

                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "showLess"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "showLess"));
                            }
                            vh.newLabel.Visibility = ViewStates.Gone;
                            break;
                    }

                    ViewGroup.LayoutParams currentCard = vh.myServiceCardView.LayoutParameters;
                    ViewGroup.LayoutParams currentImg = vh.serviceImg.LayoutParameters;

                    int cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                    float heightRatio = 84f / 96f;
                    int cardHeight = (int)(cardWidth * (heightRatio));
                    if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                    {
                        cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                        cardHeight = cardWidth;
                    }
                    else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                    {
                        cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                        cardHeight = cardWidth;
                    }

                    //currentCard.Height = cardHeight;
                    currentCard.Width = cardWidth;

                    float imgHeightRatio = 28f / 96f;
                    int imgHeight = (int)(cardWidth * (imgHeightRatio));
                    if (DPUtils.ConvertPxToDP(imgHeight) < 28f)
                    {
                        imgHeight = (int)DPUtils.ConvertDPToPx(28f);
                    }
                    currentImg.Height = imgHeight;
                    currentImg.Width = imgHeight;

                    RelativeLayout.LayoutParams currentNewLabel = vh.newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                    currentNewLabel.LeftMargin = imgHeight;
                    currentNewLabel.Width = ViewGroup.LayoutParams.WrapContent;
                    vh.newLabel.SetPadding((int)DPUtils.ConvertDPToPx(4f), 0, (int)DPUtils.ConvertDPToPx(4f), 0);

                    TextViewUtils.SetMuseoSans500Typeface(vh.serviceTitle, vh.txtNewLabel);
                    vh.txtNewLabel.Text = Utility.GetLocalizedCommonLabel("new");
                    TextViewUtils.SetTextSize8(vh.txtNewLabel);
                    TextViewUtils.SetTextSize10(vh.serviceTitle);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            catch (Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        private void DynamicIconHandling(MyServiceViewHolder vh, MyServiceModel model, int fallbackImgRes, bool isDisabled = false)
        {
            this.mActivity.RunOnUiThread(() =>
            {
                try
                {
                    if (model != null)
                    {
                        string timestamp = UserSessions.GetServicesTimeStamp(PreferenceManager.GetDefaultSharedPreferences(this.mActivity));
                        MyServiceIconEntity iconEntity = new MyServiceIconEntity()
                        {
                            ServiceId = model.ServiceId,
                            ServiceIconUrl = model.ServiceIconUrl,
                            DisabledServiceIconUrl = model.DisabledServiceIconUrl,
                            ServiceBannerUrl = model.ServiceBannerUrl,
                            TimeStamp = timestamp
                        };

                        MyServiceIconEntity iconManager = new MyServiceIconEntity();
                        MyServiceIconEntity myServiceIconEntity = iconManager.GetMyServiceItem(model.ServiceId);

                        if (myServiceIconEntity != null)
                        {
                            string iconURL = isDisabled ? myServiceIconEntity.DisabledServiceIconUrl : myServiceIconEntity.ServiceIconUrl;
                            string iconB64 = isDisabled ? myServiceIconEntity.DisabledServiceIconB64 : myServiceIconEntity.ServiceIconB64;

                            if (iconB64.IsValid())
                            {
                                Bitmap convertedImage = ImageUtils.Base64ToBitmap(iconB64);
                                if (convertedImage != null)
                                {
                                    vh.serviceImg.SetImageBitmap(convertedImage);
                                }
                                else
                                {
                                    vh.serviceImg.SetImageResource(fallbackImgRes);
                                }
                            }
                            else
                            {
                                vh.serviceImg.SetImageResource(fallbackImgRes);
                            }

                            if (timestamp != myServiceIconEntity.TimeStamp)
                            {
                                Task.Run(() =>
                                {
                                    var bitmapImage = ImageUtils.GetImageBitmapFromUrlWithTimeOut(iconURL);
                                    if (bitmapImage != null)
                                    {
                                        vh.serviceImg.SetImageBitmap(bitmapImage);
                                    }
                                    else
                                    {
                                        Bitmap convertedImage = ImageUtils.Base64ToBitmap(iconB64);
                                        if (convertedImage != null)
                                        {
                                            vh.serviceImg.SetImageBitmap(convertedImage);
                                        }
                                        else
                                        {
                                            vh.serviceImg.SetImageResource(fallbackImgRes);
                                        }
                                    }
                                });
                            }
                            //else
                            //{
                            //    Bitmap convertedImage = ImageUtils.Base64ToBitmap(iconB64);
                            //    if (convertedImage != null)
                            //    {
                            //        vh.serviceImg.SetImageBitmap(convertedImage);
                            //    }
                            //    else
                            //    {
                            //        vh.serviceImg.SetImageResource(fallbackImgRes);
                            //    }
                            //}
                        }
                        else
                        {
                            vh.serviceImg.SetImageResource(fallbackImgRes);

                            if (model.ServiceId == "1112")
                            {
                                //do nothing
                            }
                            else if (model.ServiceId == "1111")
                            {
                                //do nothing
                            }
                            else
                            {
                                Task.Run(() =>
                                {
                                    var bitmapImage = ImageUtils.GetImageBitmapFromUrlWithTimeOut(isDisabled ? model.DisabledServiceIconUrl : model.ServiceIconUrl);
                                    if (bitmapImage != null)
                                    {
                                        //vh.serviceImg.SetImageBitmap(bitmapImage);
                                        string base64String = ImageUtils.GetBase64FromBitmapPNG(bitmapImage, 100);
                                        iconEntity.ServiceIconB64 = base64String;

                                        iconManager.InsertItem(iconEntity);
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        vh.serviceImg.SetImageResource(fallbackImgRes);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                    vh.serviceImg.SetImageResource(fallbackImgRes);
                }
            });
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.MyServiceComponentView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new MyServiceViewHolder(itemView, OnClick);
        }

        void OnClick(MyServiceViewHolder sender, int position)
        {
            try
            {
                if (ClickChanged != null)
                {
                    MyServiceModel service = myServicesList[position];
                    if (service != null)
                    {
                        if (service.ServiceType == ServiceEnum.MYHOME)
                        {
                            if (sender != null)
                            {
                                sender.newLabel.Visibility = ViewStates.Gone;
                            }
                        }
                    }

                    ClickChanged(this, position);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public class MyServiceViewHolder : RecyclerView.ViewHolder
        {

            public ImageView serviceImg { get; private set; }

            public TextView serviceTitle { get; private set; }

            public TextView serviceTitle_two { get; private set; }

            public LinearLayout myServiceCardView { get; private set; }

            public LinearLayout newLabel { get; private set; }

            public TextView txtNewLabel { get; private set; }

            public MyServiceViewHolder(View itemView, Action<MyServiceViewHolder, int> listener) : base(itemView)
            {
                serviceImg = itemView.FindViewById<ImageView>(Resource.Id.service_img);
                serviceTitle = itemView.FindViewById<TextView>(Resource.Id.service_title);

                myServiceCardView = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                newLabel = itemView.FindViewById<LinearLayout>(Resource.Id.newLabel);
                txtNewLabel = itemView.FindViewById<TextView>(Resource.Id.txtNewLabel);
                TextViewUtils.SetTextSize12(serviceTitle);
                myServiceCardView.Click += (s, e) => listener((this), base.LayoutPosition);
            }
        }

        public class MyServiceItemDecoration : RecyclerView.ItemDecoration
        {

            private int spanCount;
            private float spacing;
            private bool includeEdge;

            public MyServiceItemDecoration(int spanCount, int dpSpacing, bool includeEdge, Android.App.Activity Activity)
            {
                this.spanCount = spanCount;
                this.spacing = DPUtils.ConvertDPToPx(dpSpacing);
                int cardWidth = (Activity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                {
                    this.spacing = DPUtils.ConvertDPToPx(dpSpacing - 1);
                }
                else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                {
                    this.spacing = DPUtils.ConvertDPToPx(dpSpacing - 2);
                }
                this.includeEdge = includeEdge;
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                int position = parent.GetChildAdapterPosition(view); // item position
                int column = position % spanCount; // item column

                outRect.Left = (int)(column * spacing / spanCount); // column * ((1f / spanCount) * spacing)
                outRect.Right = (int)(spacing - (column + 1) * spacing / spanCount); // spacing - (column + 1) * ((1f /    spanCount) * spacing)
            }
        }
        
    }
}