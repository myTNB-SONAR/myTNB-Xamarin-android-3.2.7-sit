using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.NotificationFilter.Activity;
using myTNB_Android.Src.Notifications.Adapter;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Notifications.MVP;
using Refit;
using System;
using System.Collections.Generic;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Android.Runtime;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using static Android.Widget.CompoundButton;

namespace myTNB_Android.Src.Notifications.Activity
{

    enum EditNotificationStates{
        SHOW,
        HIDE
    }

    enum SelectNotificationStates
    {
        SELECTED,
        UNSELECTED
    }


    [Activity(Label = "@string/notification_activity_title"
              //, MainLauncher = true
              ,Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationActivity : BaseToolbarAppCompatActivity, NotificationContract.IView, IOnCheckedChangeListener
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        // [BindView(Resource.Id.notification_listview)]`
        // ListView notificationListView;

        [BindView(Resource.Id.notification_recyclerView)]
        RecyclerView notificationRecyclerView;

        [BindView(Resource.Id.txt_notification_name)]
        TextView txtNotificationName;

        [BindView(Resource.Id.txtNotificationsContent)]
        TextView txtNotificationsContent;

        [BindView(Resource.Id.emptyLayout)]
        LinearLayout emptyLayout;

        [BindView(Resource.Id.notificationSelectAll)]
        LinearLayout notificationSelectAllContainer;

        [BindView(Resource.Id.selectAllCheckBox)]
        CheckBox selectAllCheckboxButton;

        [BindView(Resource.Id.selectAllNotificationLabel)]
        TextView selectAllNotificationLabel;

        private IMenu notificationMenu;
        private MaterialDialog deleteDialog;

        //NotificationAdapter notificationAdapter;
        NotificationRecyclerAdapter notificationRecyclerAdapter;

        NotificationContract.IUserActionsListener userActionsListener;
        NotificationPresenter mPresenter;

        MaterialDialog mProgressDialog, mQueryProgressDialog;
        private LoadingOverlay loadingOverlay;

        ItemTouchHelper itemTouchHelper;
        NotificationSwipeDeleteCallback notificationSwipeDelete;

        private static EditNotificationStates editState = EditNotificationStates.HIDE;
        private static SelectNotificationStates selectNotificationState = SelectNotificationStates.UNSELECTED;
        private bool isReadEnabled = false;

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
            this.userActionsListener.OnShowNotificationFilter();
        }

        public void ShowNotificationFilter()
        {
            Intent notificationFilter = new Intent(this, typeof(NotificationFilterActivity));
            StartActivityForResult(notificationFilter, Constants.NOTIFICATION_FILTER_REQUEST_CODE);
        }

        public void ShowNotificationsList(List<UserNotificationData> userNotificationList)
        {
            notificationRecyclerAdapter.AddAll(userNotificationList);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            notificationMenu = menu;
            menu.FindItem(Resource.Id.action_notification).SetIcon(GetDrawable(Resource.Drawable.ic_action_Select));
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_notification)
            {
                //this.userActionsListener.EditNotification();
                if (editState == EditNotificationStates.HIDE)
                {
                    notificationMenu.FindItem(Resource.Id.action_notification).SetIcon(Resource.Drawable.ic_header_cancel);
                    ShowSelectAllOption(ViewStates.Visible);
                    notificationRecyclerAdapter.ShowSelectButtons(true);
                    editState = EditNotificationStates.SHOW;
                    itemTouchHelper.AttachToRecyclerView(null);
                    SetToolBarTitle("Select");
                }
                else
                {
                    itemTouchHelper.AttachToRecyclerView(notificationRecyclerView);
                    if (selectNotificationState == SelectNotificationStates.SELECTED)
                    {
                        //Remove notifications here
                        deleteDialog.Show();
                    }
                    else
                    {
                        //Cancel Delete
                        notificationMenu.FindItem(Resource.Id.action_notification).SetIcon(Resource.Drawable.ic_action_Select);
                        ShowSelectAllOption(ViewStates.Gone);
                        notificationRecyclerAdapter.ShowSelectButtons(false);
                        editState = EditNotificationStates.HIDE;
                        SetToolBarTitle(GetString(Resource.String.notification_activity_title));
                    }
                }
                return true;
            }
            else
            {
                if (isReadEnabled)
                {
                    ReadAllSelectedNotifications();
                    return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetNotificationRecyclerView()
        {
            //notificationAdapter = new NotificationAdapter(this, true);
            //notificationListView.Adapter = notificationAdapter;
            //notificationListView.EmptyView = emptyLayout;
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            notificationRecyclerView.SetLayoutManager(layoutManager);

            DividerItemDecoration mDividerItemDecoration = new DividerItemDecoration(notificationRecyclerView.Context,
            DividerItemDecoration.Vertical);
            notificationRecyclerView.AddItemDecoration(mDividerItemDecoration);

            notificationRecyclerAdapter = new NotificationRecyclerAdapter(this, this, true);
            //notificationRecyclerAdapter.SetNotificationSelectListener(this.mPresenter);
            notificationRecyclerView.SetAdapter(notificationRecyclerAdapter);
            //NotificationSimpleCallback notificationSimpleCallback = new NotificationSimpleCallback(notificationRecyclerAdapter,0, ItemTouchHelper.Left);

            notificationSwipeDelete = new NotificationSwipeDeleteCallback(this, GetDrawable(Resource.Drawable.ic_header_delete), GetDrawable(Resource.Drawable.ic_header_mark_read));

            itemTouchHelper = new ItemTouchHelper(notificationSwipeDelete);
            itemTouchHelper.AttachToRecyclerView(notificationRecyclerView);
        }

        private void DeleteAllSelectedNotifications()
        {

        }

        private void ReadAllSelectedNotifications()
        {

        }

        private void SetInitialNotificationState()
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

            ShowSelectAllOption(ViewStates.Gone);
            editState = EditNotificationStates.HIDE;
            selectNotificationState = SelectNotificationStates.UNSELECTED;
            isReadEnabled = false;

            deleteDialog = new MaterialDialog.Builder(this)
                    .Title("Delete All Notifications")
                    .Content("Are you sure you want to delete all notifications?")
                    .PositiveText("Yes")
                    .PositiveColor(Resource.Color.blue)
                    .NegativeText("No")
                    .NegativeColor(Resource.Color.blue)
                    .OnPositive((dialog, which) =>
                    {
                        DeleteAllSelectedNotifications();
                        dialog.Dismiss();
                    })
                    .Cancelable(false)
                    .Build();
        }

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

                TextViewUtils.SetMuseoSans500Typeface(txtNotificationName);
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationsContent);

                selectAllCheckboxButton.SetOnCheckedChangeListener(this);
                this.mPresenter = new NotificationPresenter(this);
                SetNotificationRecyclerView();
                SetInitialNotificationState();
                this.userActionsListener.Start();

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
        //[OnItemClick(Resource.Id.notification_listview)]
        void OnItemClick(object sender, AbsListView.ItemClickEventArgs args)
        {
            //try
            //{
            //    UserNotificationData data = notificationAdapter.GetItemObject(args.Position);
            //    this.userActionsListener.OnSelectedNotificationItem(data, args.Position);
            //}
            //catch (Exception e)
            //{
            //    Utility.LoggingNonFatalError(e);
            //}
        }

        public void ShowProgress()
        {
            //if (mProgressDialog != null && !mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Show();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgress()
        {
            //if (mProgressDialog != null && mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Dismiss();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_cancelled_exception_btn_close), delegate {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_api_exception_btn_close), delegate {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_unknown_exception_btn_close), delegate {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

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

        public void UpdateIsReadNotificationItem(int position, bool isRead)
        {
            //try
            //{
            //    UserNotificationData userNotificationData = notificationAdapter.GetItemObject(position);
            //    userNotificationData.IsRead = isRead;
            //    notificationAdapter.Update(position, userNotificationData);
            //}
            //catch (Exception e)
            //{
            //    Utility.LoggingNonFatalError(e);
            //}
        }

        public void UpdateIsDeleteNotificationItem(int position, bool isDelete)
        {
            //try
            //{
            //    UserNotificationData userNotificationData = notificationAdapter.GetItemObject(position);
            //    notificationAdapter.Remove(position);
            //}
            //catch (Exception e)
            //{
            //    Utility.LoggingNonFatalError(e);
            //}

        }


        private Snackbar mNotificationRemoved;
        public void ShowNotificationRemoved()
        {
            if (mNotificationRemoved != null && mNotificationRemoved.IsShown)
            {
                mNotificationRemoved.Dismiss();
            }

            mNotificationRemoved = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_notification_removed), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_notification_removed_btn_close), delegate {

                mNotificationRemoved.Dismiss();

            }
            );
            mNotificationRemoved.Show();
        }

        public void ClearAdapter()
        {
            //notificationAdapter.Clear();
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
            Intent notificationDetails = new Intent(this, typeof(NotificationDetailActivity));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_LIST_ITEM, JsonConvert.SerializeObject(notificationData));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        }

        //public void ShowSelectedNotificationNewBillItem(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position)
        //{
        //    Intent notificationDetails = new Intent(this, typeof(NotificationDetailsNewBillActivity));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
        //    StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        //}

        //public void ShowSelectedNotificationDunningDisconnection(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position)
        //{
        //    Intent notificationDetails = new Intent(this, typeof(NotificationDetailAccountRelatedMattersActivity));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
        //    StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        //}

        public void ShowQueryProgress()
        {
            //if (mQueryProgressDialog != null && !mQueryProgressDialog.IsShowing)
            //{
            //    mQueryProgressDialog.Show();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideQueryProgress()
        {
            //if (mQueryProgressDialog != null && mQueryProgressDialog.IsShowing)
            //{
            //    mQueryProgressDialog.Dismiss();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
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

        private void ShowSelectAllOption(ViewStates viewState)
        {
            notificationSelectAllContainer.Visibility = viewState;
        }
        private static bool isSelectAllTap = false;
        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            isSelectAllTap = true;
            if (buttonView.Id == Resource.Id.selectAllCheckBox)
            {
                notificationRecyclerAdapter.SelectAllNotifications(isChecked);
                ShowReadAndDeleteOption(isChecked);
                if (isChecked)
                {
                    selectNotificationState = SelectNotificationStates.SELECTED;
                    selectAllNotificationLabel.Text = "Unselect All";
                }
                else
                {
                    selectNotificationState = SelectNotificationStates.UNSELECTED;
                    selectAllNotificationLabel.Text = "Select All";
                }
                SetToolBarTitle(GetSelectedNotificationTitle());
            }
            else
            {
                updateNotificationTitle();
            }
        }

        private void ShowReadAndDeleteOption(bool show)
        {
            isReadEnabled = show;
            if (show)
            {
                notificationMenu.FindItem(Resource.Id.action_notification).SetIcon(Resource.Drawable.ic_header_delete);
                SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_header_mark_read);
            }
            else
            {
                notificationMenu.FindItem(Resource.Id.action_notification).SetIcon(Resource.Drawable.ic_header_cancel);
                SupportActionBar.SetHomeAsUpIndicator(0);
            }
        }

        public void updateNotificationTitle()
        {
            if (!isSelectAllTap)
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
                    selectAllNotificationLabel.Text = "Select All";
                    selectAllCheckboxButton.SetOnCheckedChangeListener(null);
                    selectAllCheckboxButton.Checked = false;
                    selectAllCheckboxButton.SetOnCheckedChangeListener(this);
                }
                else
                {
                    ShowReadAndDeleteOption(true);
                    selectAllNotificationLabel.Text = "Unselect All";
                    selectAllCheckboxButton.SetOnCheckedChangeListener(null);
                    selectAllCheckboxButton.Checked = true;
                    selectAllCheckboxButton.SetOnCheckedChangeListener(this);
                }
            }
            else
            {
                isSelectAllTap = false;
            }
        }

        private string GetSelectedNotificationTitle()
        {
            int selectedCount = GetSelectedNotificationCount();
            if (selectedCount == 0)
            {
                return "Select";
            }
            else
            {
                return "Selected(" + selectedCount + ")";
            }
        }

        private int GetSelectedNotificationCount()
        {
            int selectedCount = 0;
            foreach(UserNotificationData notification in notificationRecyclerAdapter.GetAllNotifications()){
                if (notification.IsSelected)
                {
                    selectedCount++;
                }
            }

            return selectedCount;
        }

        public void DeleteNotification(int notificationPos)
        {
            //notificationRecyclerAdapter.RemoveItem(notificationPos);

        }

        public void ReadNotification(int notificationPos)
        {
            //notificationRecyclerAdapter.ReadItem(notificationPos);
        }
    }
}
