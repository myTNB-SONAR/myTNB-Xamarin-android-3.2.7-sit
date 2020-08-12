using Android.Content;
using Android.OS;



using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.Utils;
using System;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartNoTNBAccount : BaseFragment, DashboardChartNoTNBAccountContract.IView
    {

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.dashboard_chartview_no_account_title)]
        TextView txtTitle;

        [BindView(Resource.Id.dashboard_chartview_no_account_content)]
        TextView txtContent;

        [BindView(Resource.Id.btnAddAccount)]
        Button btnAddAccount;

        [BindView(Resource.Id.btnDisabledPay)]
        Button btnDisabledPay;

        [BindView(Resource.Id.txtTotalPayableTitle)]
        TextView txtTotalPayableTitle;

        [BindView(Resource.Id.txtTotalPayableCurrency)]
        TextView txtTotalPayableCurrency;

        [BindView(Resource.Id.txtTotalPayable)]
        TextView txtTotalPayable;

        DashboardChartNoTNBAccountContract.IUserActionsListener userActionsListener;
        DashboardChartNoTNBAccountPresenter mPresenter;

        public override int ResourceId()
        {
            return Resource.Layout.DashboardNoTNBAccount;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            TextViewUtils.SetMuseoSans300Typeface(txtContent, txtTotalPayable);
            TextViewUtils.SetMuseoSans500Typeface(txtTitle, btnAddAccount, btnDisabledPay, txtTotalPayableTitle, txtTotalPayableCurrency);

            mPresenter = new DashboardChartNoTNBAccountPresenter(this);
            mPresenter.Start();
        }

        private IMenu menu;
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            this.menu = menu;
            if (UserNotificationEntity.HasNotifications())
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_header_notification_unread));
            }
            else
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_header_notification));
            }
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override void OnResume()
        {
            base.OnResume();
            this.Activity.InvalidateOptionsMenu();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
        }

        [OnClick(Resource.Id.btnAddAccount)]
        void OnAddAccount(object sender, EventArgs args)
        {
            this.userActionsListener.OnAddAccount();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_notification:
                    this.userActionsListener.OnNotification();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void ShowAddAccount()
        {
            Intent linkAccount = GetIntentObject(typeof(LinkAccountActivity));
            //Intent linkAccount = new Intent(this.Activity, typeof(LinkAccountActivity));
            if (linkAccount != null && IsAdded)
            {
                linkAccount.PutExtra("fromDashboard", true);
                Activity.StartActivity(linkAccount);
            }
            //Activity.StartActivity(typeof(LinkAccountActivity));
        }

        public void ShowNotification()
        {
            Intent intent = GetIntentObject(typeof(NotificationActivity));
            if (intent != null && IsAdded)
            {
                StartActivity(intent);
            }

        }

        public bool HasInternet()
        {
            return ConnectionUtils.HasInternetConnection(this.Activity);
        }

        public void SetPresenter(DashboardChartNoTNBAccountContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsResumed;
        }

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            try
            {
                if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
                {
                    mNoInternetSnackbar.Dismiss();
                }

                mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chartview_data_not_available_no_internet), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate
                {

                    mNoInternetSnackbar.Dismiss();
                }
                );
                mNoInternetSnackbar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        DashboardHomeActivity activity = null;
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                if (context is DashboardHomeActivity)
                {
                    activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                }
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "No Account Inner Dashboard");
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnAttach(Android.App.Activity activity)
        {
            base.OnAttach(activity);
            try
            {
                activity = activity as DashboardHomeActivity;
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override Android.App.Activity GetActivityObject()
        {
            return activity;
        }
    }
}