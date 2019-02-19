using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Database.Model;
using Android.Support.V4.Content;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using Android.Support.Design.Widget;
using Android.Support.V7.App;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartNoTNBAccount : BaseFragment , DashboardChartNoTNBAccountContract.IView
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
            //actionBar.SetDisplayHomeAsUpEnabled(true);
            //actionBar.SetDisplayShowHomeEnabled(true);
        }

        [OnClick(Resource.Id.btnAddAccount)]    
        void OnAddAccount(object sender , EventArgs args)
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
            Intent linkAccount = new Intent(this.Activity, typeof(LinkAccountActivity));
            linkAccount.PutExtra("fromDashboard", true);
            Activity.StartActivity(linkAccount);  
            //Activity.StartActivity(typeof(LinkAccountActivity));
        }

        public void ShowNotification()
        {
            StartActivity(new Intent(this.Activity, typeof(NotificationActivity)));
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
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chartview_data_not_available_no_internet), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate {

                mNoInternetSnackbar.Dismiss();
            }
            );
            mNoInternetSnackbar.Show();
        }
    }
}