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
using myTNB_Android.Src.DigitalSignature.DSNotificationDetails.Activity;

namespace myTNB_Android.Src.Notifications.Activity
{
    enum EditNotificationStates
    {
        SHOW,
        HIDE
    }

    enum EnableNotificationState
    {
        ENABLE,
        DISABLED
    }

    enum SelectNotificationStates
    {
        SELECTED,
        UNSELECTED
    }

    [Activity(Label = "@string/notification_activity_title"
    , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationActivity : BaseActivityCustom, NotificationContract.IView, IOnCheckedChangeListener
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.layout_api_refresh)]
        LinearLayout refreshLayout;

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

        private IMenu notificationMenu;
        NotificationRecyclerAdapter notificationRecyclerAdapter;
        NotificationContract.IUserActionsListener userActionsListener;
        NotificationPresenter mPresenter;
        MaterialDialog mProgressDialog, mQueryProgressDialog;
        ItemTouchHelper itemTouchHelper;
        private static NotificationSwipeDeleteCallback notificationSwipeDelete;
        private static EditNotificationStates editState = EditNotificationStates.HIDE;
        private static SelectNotificationStates selectNotificationState = SelectNotificationStates.UNSELECTED;
        private MaterialDialog deleteAllDialog;
        private MaterialDialog markReadAllDialog;
        private int selectedNotification;
        private static EnableNotificationState enableNotificationState = EnableNotificationState.DISABLED;
        private bool hasNotification = false;
        const string PAGE_ID = "PushNotificationList";

        //========================================== FORM LIFECYCLE ==================================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                mProgressDialog = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.notification_activity_progress_title))
                    .Content(GetString(Resource.String.notification_activity_progress_content))
                    .Cancelable(false)
                    .Progress(true, 0)
                    .Build();

                mQueryProgressDialog = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.notification_activity_query_progress_title))
                    .Content(GetString(Resource.String.notification_activity_query_progress_content))
                    .Cancelable(false)
                    .Progress(true, 0)
                    .Build();

                TextViewUtils.SetMuseoSans500Typeface(txtNotificationName, selectAllNotificationLabel, btnNewRefresh);
                TextViewUtils.SetMuseoSans300Typeface(txtNewRefreshMessage, txtNotificationsContent);
                TextViewUtils.SetTextSize12(txtNotificationName);
                TextViewUtils.SetTextSize14(selectAllNotificationLabel);
                TextViewUtils.SetTextSize16(txtNewRefreshMessage, txtNotificationsContent, btnNewRefresh);

                selectAllNotificationLabel.Text = GetLabelCommonByLanguage("selectAll");
                txtNotificationsContent.Text = GetLabelByLanguage("noNotification");

                selectAllCheckboxButton.SetOnCheckedChangeListener(this);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                this.mPresenter = new NotificationPresenter(this);
                ShowNotificationBadge();
                SetNotificationRecyclerView();
                SetInitialNotificationState();
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

                Bundle extras = Intent.Extras;
                if (extras != null && extras.ContainsKey(Constants.HAS_NOTIFICATION) && extras.GetBoolean(Constants.HAS_NOTIFICATION))
                {
                    this.userActionsListener.QueryOnLoad(this.DeviceId());
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.NotificationToolbarMenu, menu);
            notificationMenu = menu;
            notificationMenu.FindItem(Resource.Id.action_notification_read).SetIcon(GetDrawable(Resource.Drawable.ic_header_markread)).SetVisible(false);
            if (MyTNBAccountManagement.GetInstance().IsNotificationServiceFailed())
            {
                notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetVisible(false);
                notificationMenu.FindItem(Resource.Id.action_notification_read).SetVisible(false);
            }
            else
            {
                int count = notificationRecyclerAdapter.GetAllNotifications().Count;
                if (count == 0)
                {
                    notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(GetDrawable(Resource.Drawable.notification_select_all)).SetVisible(false);
                }
                else
                {
                    notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(GetDrawable(Resource.Drawable.notification_select_all)).SetVisible(true);
                }
            }
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_notification_edit_delete:
                    if (editState == EditNotificationStates.HIDE)
                    {
                        notificationMenu.FindItem(Resource.Id.action_notification_read).SetIcon(Resource.Drawable.ic_header_markread_disabled).SetVisible(true).SetEnabled(false);
                        notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_delete_disabled).SetVisible(true).SetEnabled(false);
                        ShowSelectAllOption(ViewStates.Visible);
                        notificationRecyclerAdapter.ShowSelectButtons(true);
                        editState = EditNotificationStates.SHOW;
                        itemTouchHelper.AttachToRecyclerView(null);
                        notificationRecyclerAdapter.SetClickable(false);
                        SetToolBarTitle(GetLabelByLanguage("select"));
                    }
                    else
                    {
                        if (enableNotificationState == EnableNotificationState.ENABLE)
                        {
                            itemTouchHelper.AttachToRecyclerView(notificationRecyclerView);
                            if (GetSelectedNotificationCount() > 0)
                            {
                                ShowDeleteAllNotificationDialog();
                            }
                            else
                            {
                                notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_select_all);
                                ShowSelectAllOption(ViewStates.Gone);
                                notificationRecyclerAdapter.ShowSelectButtons(false);
                                editState = EditNotificationStates.HIDE;
                                SetToolBarTitle(GetLabelByLanguage("title"));
                                notificationRecyclerAdapter.SetClickable(true);
                            }
                        }
                    }
                    break;
                case Resource.Id.action_notification_read:
                    if (enableNotificationState == EnableNotificationState.ENABLE)
                    {
                        if (IsValidReadNotifications())
                        {
                            this.mPresenter.ReadAllSelectedNotifications();
                        }
                    }
                    break;
                default:
                    {
                        if (editState == EditNotificationStates.SHOW)
                        {
                            editState = EditNotificationStates.HIDE;
                            notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_select_all).SetEnabled(true);
                            notificationMenu.FindItem(Resource.Id.action_notification_read).SetVisible(false);
                            ShowSelectAllOption(ViewStates.Gone);
                            notificationRecyclerAdapter.ShowSelectButtons(false);
                            SetToolBarTitle(GetLabelByLanguage("title"));
                            notificationRecyclerAdapter.SetClickable(true);
                            notificationRecyclerAdapter.SelectAllNotifications(false);
                            selectAllCheckboxButton.SetOnCheckedChangeListener(null);
                            selectAllCheckboxButton.Checked = false;
                            selectAllCheckboxButton.SetOnCheckedChangeListener(this);
                            itemTouchHelper.AttachToRecyclerView(notificationRecyclerView);
                            ShowEditMode(false);
                            return true;
                        }
                    }
                    break;
            }
            return base.OnOptionsItemSelected(item);
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

        private bool IsValidReadNotifications()
        {
            foreach (UserNotificationData notification in notificationRecyclerAdapter.GetAllNotifications())
            {
                if (notification.IsSelected && !notification.IsRead)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.NotificationView;
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

        public void ShowNotificationFilter()
        {
            Intent notificationFilter = new Intent(this, typeof(NotificationFilterActivity));
            StartActivityForResult(notificationFilter, Constants.NOTIFICATION_FILTER_REQUEST_CODE);
        }

        public void ShowNotificationsList(List<UserNotificationData> userNotificationList)
        {
            if (userNotificationList.Count == 0)
            {
                FindViewById(Resource.Id.emptyLayout).Visibility = ViewStates.Visible;
                notificationRecyclerView.Visibility = ViewStates.Gone;
                notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetVisible(false);
            }
            else
            {
                notificationRecyclerView.Visibility = ViewStates.Visible;
                FindViewById(Resource.Id.emptyLayout).Visibility = ViewStates.Gone;
                notificationRecyclerAdapter.AddAll(userNotificationList);
            }
        }

        private void ShowDeleteAllNotificationDialog()
        {
            string dialogTitle, dialogContent;
            if (GetSelectedNotificationCount() == 1)
            {
                this.mPresenter.DeleteAllSelectedNotifications();
            }
            else
            {
                if (GetSelectedNotificationCount() == notificationRecyclerAdapter.GetAllNotifications().Count)
                {
                    dialogTitle = GetLabelByLanguage("deleteAllTitle");
                    dialogContent = GetLabelByLanguage("deleteAllMessage");
                }
                else
                {
                    dialogTitle = GetLabelByLanguage("deleteTitleMultiple");
                    dialogContent = GetLabelByLanguage("deleteMessageMultiple");
                }

                if (deleteAllDialog != null)
                {
                    deleteAllDialog.SetTitle(dialogTitle);
                    deleteAllDialog.SetContent(dialogContent);
                    deleteAllDialog.Show();
                }
                else
                {
                    deleteAllDialog = new MaterialDialog.Builder(this)
                        .Title(dialogTitle)
                        .Content(dialogContent)
                        .PositiveText(GetLabelCommonByLanguage("yes"))
                        .PositiveColor(Resource.Color.blue)
                        .NegativeText(GetLabelCommonByLanguage("no"))
                        .NegativeColor(Resource.Color.blue)
                        .OnPositive((dialog, which) =>
                        {
                            dialog.Dismiss();
                            this.mPresenter.DeleteAllSelectedNotifications();
                        })
                        .Cancelable(false)
                        .Show();
                }
            }
        }

        private void ShowReadAllNotificationDialog()
        {
            if (markReadAllDialog != null)
            {
                markReadAllDialog.Show();
            }
            else
            {
                markReadAllDialog = new MaterialDialog.Builder(this)
                .Title(GetString(Resource.String.Notification_Read_All_Dialog_Title))
                .Content(GetString(Resource.String.Notification_Read_All_Dialog_Content))
                .PositiveText(GetString(Resource.String.Common_Dialog_Yes))
                .PositiveColor(Resource.Color.blue)
                .NegativeText(GetString(Resource.String.Common_Dialog_No))
                .NegativeColor(Resource.Color.blue)
                .OnPositive((dialog, which) =>
                {
                    dialog.Dismiss();
                    this.mPresenter.ReadAllSelectedNotifications();
                })
                .Cancelable(false)
                .Show();
            }
        }

        private void ShowNotificationBadge()
        {
            int count = UserNotificationEntity.Count();
            if (count == 0)
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
            }
            else
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.ApplicationContext, count);
            }
        }

        private void SetNotificationRecyclerView()
        {
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            notificationRecyclerView.SetLayoutManager(layoutManager);

            DividerItemDecoration mDividerItemDecoration = new DividerItemDecoration(notificationRecyclerView.Context,
            DividerItemDecoration.Vertical);
            notificationRecyclerView.AddItemDecoration(mDividerItemDecoration);

            notificationRecyclerView.Visibility = ViewStates.Visible;
            refreshLayout.Visibility = ViewStates.Gone;

            notificationRecyclerAdapter = new NotificationRecyclerAdapter(this, this, true);
            notificationRecyclerView.SetAdapter(notificationRecyclerAdapter);
            notificationSwipeDelete = new NotificationSwipeDeleteCallback(this, GetDrawable(Resource.Drawable.notification_delete_active), GetDrawable(Resource.Drawable.ic_header_markread));
            notificationSwipeDelete.SetInitialState();
            itemTouchHelper = new ItemTouchHelper(notificationSwipeDelete);
            itemTouchHelper.AttachToRecyclerView(notificationRecyclerView);
        }

        private void SetInitialNotificationState()
        {
            ShowSelectAllOption(ViewStates.Gone);
            editState = EditNotificationStates.HIDE;
            selectNotificationState = SelectNotificationStates.UNSELECTED;
            enableNotificationState = EnableNotificationState.DISABLED;
            selectAllCheckboxButton.SetOnCheckedChangeListener(null);
            selectAllCheckboxButton.Checked = false;
            selectAllCheckboxButton.SetOnCheckedChangeListener(this);
            ShowEditMode(false);
            notificationRecyclerAdapter.SetClickable(true);
            this.userActionsListener.InitialSetFilterName();
        }

        public void ShowProgress()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowView()
        {
            ShowSelectAllOption(ViewStates.Visible);
            notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_delete_active);
            notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetVisible(false);
            notificationMenu.FindItem(Resource.Id.action_notification_read).SetVisible(true);
            refreshLayout.Visibility = ViewStates.Gone;
        }

        public void ShowRefreshView(bool isRefresh, string contentTxt, string btnTxt)
        {
            try
            {
                FindViewById(Resource.Id.emptyLayout).Visibility = ViewStates.Gone;
                notificationRecyclerView.Visibility = ViewStates.Gone;
                refreshLayout.Visibility = ViewStates.Visible;
                btnNewRefresh.Text = string.IsNullOrEmpty(btnTxt) ? GetLabelCommonByLanguage("refreshNow") : btnTxt;
                ShowSelectAllOption(ViewStates.Gone);
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


        public void HideProgress()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateReadNotifications()
        {
            foreach (UserNotificationData notificationData in notificationRecyclerAdapter.GetAllNotifications())
            {
                if (notificationData.IsSelected)
                {
                    notificationData.IsRead = true;
                }
            }
            notificationRecyclerAdapter.NotifyDataSetChanged();
            editState = EditNotificationStates.HIDE;
            notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_select_all).SetEnabled(true);
            notificationMenu.FindItem(Resource.Id.action_notification_read).SetVisible(false);
            ShowSelectAllOption(ViewStates.Gone);
            notificationRecyclerAdapter.ShowSelectButtons(false);
            SetToolBarTitle(GetLabelByLanguage("title"));
            notificationRecyclerAdapter.SetClickable(true);
            notificationRecyclerAdapter.SelectAllNotifications(false);
            selectAllCheckboxButton.SetOnCheckedChangeListener(null);
            selectAllCheckboxButton.Checked = false;
            selectAllCheckboxButton.SetOnCheckedChangeListener(this);
            if (IsActive())
            {
                HideProgress();
            }
        }

        public void UpdateDeleteNotifications()
        {
            notificationRecyclerAdapter.GetAllNotifications().RemoveAll(notification => notification.IsSelected == true);
            notificationRecyclerAdapter.NotifyDataSetChanged();
            editState = EditNotificationStates.HIDE;
            notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_select_all).SetEnabled(true);
            notificationMenu.FindItem(Resource.Id.action_notification_read).SetVisible(false);
            ShowSelectAllOption(ViewStates.Gone);
            notificationRecyclerAdapter.ShowSelectButtons(false);
            SetToolBarTitle(GetLabelByLanguage("title"));
            notificationRecyclerAdapter.SetClickable(true);
            notificationRecyclerAdapter.SelectAllNotifications(false);
            if (notificationRecyclerAdapter.GetAllNotifications().Count == 0)
            {
                ClearAdapter();
                notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetVisible(false);
                FindViewById(Resource.Id.emptyLayout).Visibility = ViewStates.Visible;
                notificationRecyclerView.Visibility = ViewStates.Gone;
            }
            if (IsActive())
            {
                HideProgress();
            }
        }

        private Snackbar mCancelledErrorSnackBar;
        public void ShowFailedErrorMessage(string errorMessage)
        {
            if (mCancelledErrorSnackBar != null && mCancelledErrorSnackBar.IsShown)
            {
                mCancelledErrorSnackBar.Dismiss();
            }

            mCancelledErrorSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mCancelledErrorSnackBar.Dismiss();
            }
            );
            View v = mCancelledErrorSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledErrorSnackBar.Show();
            this.SetIsClicked(false);
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }



        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
            this.SetIsClicked(false);
        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void UpdateIsReadNotificationItem(int position, bool isRead)
        {
        }

        public void UpdateIsDeleteNotificationItem(int position, bool isDelete)
        {
        }

        private Snackbar mNotificationRemoved;
        public void ShowNotificationRemoved()
        {
            if (mNotificationRemoved != null && mNotificationRemoved.IsShown)
            {
                mNotificationRemoved.Dismiss();
            }

            mNotificationRemoved = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_notification_removed), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mNotificationRemoved.Dismiss();

            }
            );
            View v = mNotificationRemoved.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNotificationRemoved.Show();
            this.SetIsClicked(false);
        }

        public void ClearAdapter()
        {
            SetInitialNotificationState();
            notificationMenu.FindItem(Resource.Id.action_notification_read).SetIcon(GetDrawable(Resource.Drawable.ic_header_markread)).SetVisible(false);
            notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(GetDrawable(Resource.Drawable.notification_select_all)).SetVisible(true).SetEnabled(true);
            notificationRecyclerAdapter.ClearAll();
            SetToolBarTitle(GetLabelByLanguage("title"));
        }

        public void ShowNotificationFilterName(string filterName)
        {
            if (!string.IsNullOrEmpty(filterName))
            {
                txtNotificationName.Text = filterName;
            }
            else
            {
                txtNotificationName.Text = "";
            }

        }

        public void ShowDetails(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position)
        {
            switch (details.BCRMNotificationTypeId)
            {
                case Constants.BCRM_NOTIFICATION_EKYC_FIRST_NOTIFICATION:
                case Constants.BCRM_NOTIFICATION_EKYC_SECOND_NOTIFICATION:
                case Constants.BCRM_NOTIFICATION_EKYC_ID_NOT_MATCHING:
                case Constants.BCRM_NOTIFICATION_EKYC_FAILED:
                case Constants.BCRM_NOTIFICATION_EKYC_THREE_TIMES_FAILURE:
                case Constants.BCRM_NOTIFICATION_EKYC_SUCCESSFUL:
                    {
                        Intent notificationDetails = new Intent(this, typeof(DSNotificationDetailsActivity));
                        notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
                        notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
                        StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
                    }
                    break;
                default:
                    {
                        Intent notificationDetails = new Intent(this, typeof(UserNotificationDetailActivity));
                        notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_LIST_ITEM, JsonConvert.SerializeObject(notificationData));
                        notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
                        notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
                        StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
                    }
                    break;
            }
        }

        public void ShowQueryProgress()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideQueryProgress()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        private void ShowSelectAllOption(ViewStates viewState)
        {
            notificationSelectAllContainer.Visibility = viewState;
        }
        private void ShowEditMode(bool isSelected)
        {
            if (isSelected)
            {
                selectNotificationState = SelectNotificationStates.SELECTED;
                selectAllNotificationLabel.Text = GetLabelCommonByLanguage("unselectAll");
            }
            else
            {
                selectNotificationState = SelectNotificationStates.UNSELECTED;
                selectAllNotificationLabel.Text = GetLabelCommonByLanguage("selectAll");
            }
        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            if (buttonView.Id == Resource.Id.selectAllCheckBox)
            {
                notificationRecyclerAdapter.SelectAllNotifications(isChecked);
                ShowReadAndDeleteOption(isChecked);
                ShowEditMode(isChecked);
                SetToolBarTitle(GetSelectedNotificationTitle());
            }
            else
            {
                UpdatedSelectedNotifications();
            }
        }

        private void ShowReadAndDeleteOption(bool show)
        {
            if (editState == EditNotificationStates.SHOW)
            {
                if (show)
                {
                    notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_delete_active);
                    notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetEnabled(true);
                    if (IsValidReadNotifications())
                    {
                        notificationMenu.FindItem(Resource.Id.action_notification_read).SetIcon(Resource.Drawable.ic_header_mark_read);
                        notificationMenu.FindItem(Resource.Id.action_notification_read).SetEnabled(true);
                    }
                    else
                    {
                        notificationMenu.FindItem(Resource.Id.action_notification_read).SetIcon(Resource.Drawable.ic_header_markread_disabled);
                        notificationMenu.FindItem(Resource.Id.action_notification_read).SetEnabled(false);
                    }
                    enableNotificationState = EnableNotificationState.ENABLE;
                }
                else
                {
                    notificationMenu.FindItem(Resource.Id.action_notification_read).SetIcon(Resource.Drawable.ic_header_markread_disabled);
                    notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_delete_disabled);
                    notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetEnabled(false);
                    notificationMenu.FindItem(Resource.Id.action_notification_read).SetEnabled(false);
                    enableNotificationState = EnableNotificationState.DISABLED;
                }
            }
        }

        private string GetSelectedNotificationTitle()
        {
            int selectedCount = GetSelectedNotificationCount();
            if (selectedCount == 0)
            {
                return GetLabelByLanguage("select");
            }
            else
            {
                return string.Format(GetLabelByLanguage("selected"), selectedCount);
            }
        }

        private int GetSelectedNotificationCount()
        {
            int selectedCount = 0;
            foreach (UserNotificationData notification in notificationRecyclerAdapter.GetAllNotifications())
            {
                if (notification.IsSelected)
                {
                    selectedCount++;
                }
            }

            return selectedCount;
        }

        public void DeleteNotificationByPosition(int notificationPos)
        {
            selectedNotification = notificationPos;
            notificationRecyclerAdapter.GetItemObject(selectedNotification).IsSelected = true;
            this.mPresenter.DeleteAllSelectedNotifications();
        }

        public void ReadNotificationByPosition(int notificationPos)
        {
            selectedNotification = notificationPos;
            notificationRecyclerAdapter.GetItemObject(selectedNotification).IsSelected = true;
            this.mPresenter.ReadAllSelectedNotifications();
        }

        public void UpdatedSelectedNotifications()
        {
            if (editState == EditNotificationStates.SHOW)
            {
                int selectedCount = GetSelectedNotificationCount();
                SetToolBarTitle(GetSelectedNotificationTitle());
                if (selectedCount != notificationRecyclerAdapter.ItemCount)
                {
                    if (selectedCount == 0)
                    {
                        ShowReadAndDeleteOption(false);
                    }
                    else
                    {
                        ShowReadAndDeleteOption(true);
                    }
                    selectAllNotificationLabel.Text = GetLabelCommonByLanguage("selectAll");
                    selectAllCheckboxButton.SetOnCheckedChangeListener(null);
                    selectAllCheckboxButton.Checked = false;
                    selectAllCheckboxButton.SetOnCheckedChangeListener(this);
                }
                else
                {
                    ShowReadAndDeleteOption(true);
                    selectAllNotificationLabel.Text = GetLabelCommonByLanguage("unselectAll");
                    selectAllCheckboxButton.SetOnCheckedChangeListener(null);
                    selectAllCheckboxButton.Checked = true;
                    selectAllCheckboxButton.SetOnCheckedChangeListener(this);
                }
            }
        }

        public List<UserNotificationData> GetNotificationList()
        {
            return notificationRecyclerAdapter.GetAllNotifications();
        }

        public void UpdateSelectedNotification()
        {
            notificationRecyclerAdapter.RemoveItem(selectedNotification);
        }

        public void OnFailedNotificationAction()
        {
            notificationRecyclerAdapter.NotifyDataSetChanged();
        }

        public void ShowNotificationDetails(int itemPosition)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                UserNotificationData userNotificationData = notificationRecyclerAdapter.GetAllNotifications()[itemPosition];
                mPresenter.OnShowNotificationDetails(userNotificationData, itemPosition);
            }
        }

        public void ShowEditMode()
        {
            notificationMenu.FindItem(Resource.Id.action_notification_read).SetIcon(Resource.Drawable.ic_header_markread_disabled).SetVisible(true).SetEnabled(false);
            notificationMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_delete_disabled).SetVisible(true).SetEnabled(false);
            ShowSelectAllOption(ViewStates.Visible);
            notificationRecyclerAdapter.ShowSelectButtons(true);
            editState = EditNotificationStates.SHOW;
            notificationRecyclerAdapter.SetClickable(false);
            SetToolBarTitle(GetLabelByLanguage("select"));
        }

        public void SetNotificationItemClickable(bool isClickable)
        {
            notificationRecyclerAdapter.SetClickable(isClickable);
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