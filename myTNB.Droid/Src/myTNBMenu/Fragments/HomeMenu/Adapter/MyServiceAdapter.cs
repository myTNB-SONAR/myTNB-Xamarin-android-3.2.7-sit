using Android.Graphics;
using Android.Preferences;


using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
	public class MyServiceAdapter : RecyclerView.Adapter
	{

		List<MyService> myServiceList = new List<MyService>();

        public event EventHandler<int> ClickChanged;

        private Android.App.Activity mActivity;

        private bool isRefreshShown = false;


        public MyServiceAdapter(List<MyService> data, Android.App.Activity Activity, bool currentRefresh)
		{
            if (data == null)
            {
                this.myServiceList.Clear();
            }
            else
            {
                this.myServiceList = data;
            }
            this.mActivity = Activity;

            this.isRefreshShown = currentRefresh;
        }

        public override int ItemCount => myServiceList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
            try
            {

                MyServiceViewHolder vh = holder as MyServiceViewHolder;

                MyService model = myServiceList[position];

                try
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        if (model.serviceCategoryName.Contains("<br/>") || model.serviceCategoryName.Contains("\n"))
                        {
                            string newStringValue = "";
                            if (model.serviceCategoryName.Contains("\n"))
                            {
                                newStringValue = model.serviceCategoryName.Replace("\n", "<br/>");
                            }
                            else if (model.serviceCategoryName.Contains("\r\n"))
                            {
                                newStringValue = model.serviceCategoryName.Replace("\r\n", "<br/>");
                            }
                            else
                            {
                                newStringValue = model.serviceCategoryName;
                            }
                            vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            string[] splittedString = model.serviceCategoryName.Trim().Split(" ");
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
                                newStringName = model.serviceCategoryName;
                            }

                            vh.serviceTitle.TextFormatted = Html.FromHtml(newStringName, FromHtmlOptions.ModeLegacy);
                        }
                    }
                    else
                    {
                        if (model.serviceCategoryName.Contains("<br/>") || model.serviceCategoryName.Contains("\n"))
                        {
                            string newStringValue = "";
                            if (model.serviceCategoryName.Contains("\n"))
                            {
                                newStringValue = model.serviceCategoryName.Replace("\n", "<br/>");
                            }
                            else if (model.serviceCategoryName.Contains("\r\n"))
                            {
                                newStringValue = model.serviceCategoryName.Replace("\r\n", "<br/>");
                            }
                            else
                            {
                                newStringValue = model.serviceCategoryName;
                            }
                            vh.serviceTitle.TextFormatted = Html.FromHtml(newStringValue);
                        }
                        else
                        {
                            string[] splittedString = model.serviceCategoryName.Trim().Split(" ");
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
                                newStringName = model.serviceCategoryName;
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
                    switch (model.ServiceCategoryId)
                    {
                        case "1001":
                            vh.serviceImg.SetImageResource(Resource.Drawable.submit_meter);
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
                        case "1002":
                            vh.serviceImg.SetImageResource(Resource.Drawable.check_status);
                            vh.newLabel.Visibility = ViewStates.Gone;
                            break;
                        case "1003":
                            vh.serviceImg.SetImageResource(Resource.Drawable.feedback);
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
                        case "1004":
                            if (Utility.IsEnablePayment() && !isRefreshShown && MyTNBAccountManagement.GetInstance().IsPayBillEnabledNeeded())
                            {
                                vh.serviceImg.SetImageResource(Resource.Drawable.bills);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.powerBlue)));
                            }
                            else
                            {
                                vh.serviceImg.SetImageResource(Resource.Drawable.bills_disabled);
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
                        case "1005":
                            if (!isRefreshShown && MyTNBAccountManagement.GetInstance().IsViewBillEnabledNeeded())
                            {
                                vh.serviceImg.SetImageResource(Resource.Drawable.pdf_bill);
                                vh.serviceTitle.SetTextColor(new Color(ContextCompat.GetColor(this.mActivity, Resource.Color.powerBlue)));
                            }
                            else
                            {
                                vh.serviceImg.SetImageResource(Resource.Drawable.pdf_bill_disabled);
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
                        case "1006":
                            vh.serviceImg.SetImageResource(Resource.Drawable.check_status);
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                vh.serviceTitle.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title"));
                            }
                            if (UserSessions.HasApplicationStatusShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                            {
                                vh.newLabel.Visibility = ViewStates.Gone;
                            }
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

                    currentCard.Height = cardHeight;
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
                ClickChanged(this, position);
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
