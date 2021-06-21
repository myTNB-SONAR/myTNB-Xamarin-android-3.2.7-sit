using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.NotificationDetails.Activity;
using myTNB_Android.Src.NotificationFilter.Activity;
using myTNB_Android.Src.Notifications.Adapter;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Notifications.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;
using static Android.Widget.CompoundButton;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.myTNBMenu.Activity;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Snackbar;
using AndroidX.ViewPager.Widget;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
   

    [Activity(Label = "@string/managebilldelivery_activity_title"
    , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class ManageBillDeliveryActivity : BaseActivityCustom, ViewPager.IOnPageChangeListener, ManageBillDeliveryContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        

        [BindView(Resource.Id.notification_recyclerView)]
        RecyclerView notificationRecyclerView;

        [BindView(Resource.Id.txt_notification_name)]
        TextView txtNotificationName;

        [BindView(Resource.Id.txtNotificationsContent)]
        TextView txtNotificationsContent;

        [BindView(Resource.Id.emptyLayout)]
        LinearLayout emptyLayout;

        [BindView(Resource.Id.notificationSelectAllHeader)]
        LinearLayout notificationSelectAllContainer;

        [BindView(Resource.Id.selectAllCheckBox)]
        CheckBox selectAllCheckboxButton;

        [BindView(Resource.Id.btnRefresh)]
        Button btnNewRefresh;

        [BindView(Resource.Id.refresh_content)]
        TextView txtNewRefreshMessage;

        [BindView(Resource.Id.selectAllNotificationLabel)]
        TextView selectAllNotificationLabel;

        [BindView(Resource.Id.refresh_image)]
        ImageView refresh_image;

        [BindView(Resource.Id.viewPager)]
        ViewPager viewPager;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;
        [BindView(Resource.Id.applicationIndicator)]
        RelativeLayout applicationIndicator;

        private IMenu notificationMenu;
        NotificationRecyclerAdapter notificationRecyclerAdapter;
        NotificationContract.IUserActionsListener userActionsListener;
        NotificationPresenter mPresenter;
        MaterialDialog mProgressDialog, mQueryProgressDialog;
        ItemTouchHelper itemTouchHelper;
        private static NotificationSwipeDeleteCallback notificationSwipeDelete;
        private MaterialDialog deleteAllDialog;
        private MaterialDialog markReadAllDialog;
        private int selectedNotification;
        private bool hasNotification = false;
        const string PAGE_ID = "ManageBillDelivery";

        ManageBillDeliveryPresenter presenter;
        ManageBillDeliveryAdapter ManageBillDeliveryAdapter;
        string currentAppNavigation;

        //========================================== FORM LIFECYCLE ==================================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            presenter = new ManageBillDeliveryPresenter(this);
            viewPager = (ViewPager)FindViewById(Resource.Id.viewPager);
            viewPager.AddOnPageChangeListener(this);
            ManageBillDeliveryAdapter = new ManageBillDeliveryAdapter(SupportFragmentManager);

            ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList("test"));

            viewPager.Adapter = ManageBillDeliveryAdapter;

            UpdateAccountListIndicator();
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APP_NAVIGATION_KEY))
                {
                    currentAppNavigation = extras.GetString(Constants.APP_NAVIGATION_KEY);

                    ManageBillDeliveryAdapter.SetData(this.presenter.GenerateManageBillDeliveryList("test"));

                    viewPager.Adapter = ManageBillDeliveryAdapter;

                    UpdateAccountListIndicator();
                }
            }
            try
            {
                if (MyTNBAccountManagement.GetInstance().IsNotificationServiceFailed())
                {
                    ShowRefreshView(true, null, null);
                }
                else if (MyTNBAccountManagement.GetInstance().IsNotificationServiceMaintenance())
                {
                    ShowRefreshView(false, null, null);
                }
                else
                {
                    if (MyTNBAccountManagement.GetInstance().IsNotificationServiceCompleted())
                    {
                        this.userActionsListener.Start();
                    }
                    else
                    {
                        this.userActionsListener.QueryOnLoad(this.DeviceId());
                    }
                }

                Bundle extras2 = Intent.Extras;
                if (extras2 != null && extras2.ContainsKey(Constants.HAS_NOTIFICATION) && extras2.GetBoolean(Constants.HAS_NOTIFICATION))
                {
                    this.userActionsListener.QueryOnLoad(this.DeviceId());
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //throw new NotImplementedException();
        }
        public string GetAppString(int id)
        {
            return this.GetString(id);
        }
        public void OnPageScrollStateChanged(int state)
        {
            //throw new NotImplementedException();
        }

        public void OnPageSelected(int position)
        {
            if (ManageBillDeliveryAdapter != null && ManageBillDeliveryAdapter.Count > 1)
            {
                for (int i = 0; i < ManageBillDeliveryAdapter.Count; i++)
                {
                    ImageView selectedDot = (ImageView)indicatorContainer.GetChildAt(i);
                    if (position == i)
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                    }
                    else
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                    }
                }

                
            }
        }

        private void UpdateAccountListIndicator()
        {
            if (ManageBillDeliveryAdapter != null && ManageBillDeliveryAdapter.Count > 1)
            {
                indicatorContainer.Visibility = ViewStates.Visible;
                for (int i = 0; i < ManageBillDeliveryAdapter.Count; i++)
                {
                    ImageView image = new ImageView(this);
                    image.Id = i;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.RightMargin = 8;
                    layoutParams.LeftMargin = 8;
                    image.LayoutParameters = layoutParams;
                    if (i == 0)
                    {
                        image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                    }
                    else
                    {
                        image.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                    }
                    indicatorContainer.AddView(image, i);
                }
            }
            else
            {
                applicationIndicator.Visibility = ViewStates.Gone;
                

            }
        }
       

      

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Notifications Listing");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        //===========================================================================================================================

     

        public override int ResourceId()
        {
            return Resource.Layout.ManageBillDelivery;
        }

        public void SetPresenter(NotificationContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }
        [OnClick(Resource.Id.txt_notification_name)]
        void OnNotificationFilter(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnShowNotificationFilter();
            }
        }


       

        

       

      
       

       

        

        public void ShowRefreshView(bool isRefresh, string contentTxt, string btnTxt)
        {
            try
            {
                FindViewById(Resource.Id.emptyLayout).Visibility = ViewStates.Gone;
                notificationRecyclerView.Visibility = ViewStates.Gone;
                btnNewRefresh.Text = string.IsNullOrEmpty(btnTxt) ? GetLabelCommonByLanguage("refreshNow") : btnTxt;
                if (notificationMenu != null)
                {
                    notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetVisible(false);
                    notificationMenu.FindItem(Resource.Id.action_notification_read).SetVisible(false);
                }

                if (isRefresh)
                {
                    refresh_image.SetImageResource(Resource.Drawable.refresh_1);

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(Utility.GetLocalizedErrorLabel("refreshMessage"), FromHtmlOptions.ModeLegacy) : Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(Utility.GetLocalizedErrorLabel("refreshMessage")) : Html.FromHtml(contentTxt);
                    }

                    btnNewRefresh.Visibility = ViewStates.Visible;
                }
                else
                {
                    //  TODO: LinSiong update bcrmdown to notification
                    refresh_image.SetImageResource(Resource.Drawable.maintenance_new);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"), FromHtmlOptions.ModeLegacy) : Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage")) : Html.FromHtml(contentTxt);
                    }

                    btnNewRefresh.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            this.userActionsListener.QueryOnLoad(this.DeviceId());
        }


       

       

       
       

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override void OnBackPressed()
        {
            try
            {
                if (MyTNBAccountManagement.GetInstance().IsUsageFromNotification())
                {
                    MyTNBAccountManagement.GetInstance().SetIsAccessUsageFromNotification(false);
                    if (MyTNBAccountManagement.GetInstance().IsNotificationsFromLaunch())
                    {
                        MyTNBAccountManagement.GetInstance().SetIsNotificationListFromLaunch(false);
                        base.OnBackPressed();
                    }
                    else
                    {
                        Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                        MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                        HomeMenuUtils.ResetAll();
                        DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        StartActivity(DashboardIntent);
                    }
                }
                else
                {
                    base.OnBackPressed();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}