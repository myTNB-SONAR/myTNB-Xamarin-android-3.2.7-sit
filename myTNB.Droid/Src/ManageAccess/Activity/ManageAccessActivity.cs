using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
using AndroidSwipeLayout.Util;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.AddAccount.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.ManageSupplyAccount.Activity;
using myTNB.AndroidApp.Src.ManageAccess.Adapter;
using myTNB.AndroidApp.Src.ManageAccess.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.ManageAccess.Adapter.ManageAccessAdapter;
using myTNB.AndroidApp.Src.ManageUser.Activity;
using myTNB.AndroidApp.Src.AddNewUser.Activity;
using AndroidX.CoordinatorLayout.Widget;
using Android.Graphics;
using Android.Preferences;
using static myTNB.AndroidApp.Src.ManageAccess.Adapter.ManageAccessDeleteAdapter;
using AndroidX.Core.Content;
using myTNB.AndroidApp.Src.LogUserAccess.Activity;
using myTNB.AndroidApp.Src.LogUserAccess.Models;
using Java.Util;
using myTNB.AndroidApp.Src.ManageAccess.Models;

namespace myTNB.AndroidApp.Src.ManageAccess.Activity
{
    [Activity(Label = "@string/my_account_activity_title_new"
        //, Icon = "@drawable/Logo"
        //, MainLauncher = true
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class ManageAccessActivity : BaseActivityCustom, ManageAccessContract.IView, customButtonListener, customCheckboxListener
    {
        /*        [BindView(Resource.Id.rootView)]
                LinearLayout rootView;*/

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.ScrollviewLayout)]
        ScrollView scrollview;

        [BindView(Resource.Id.listView)]
        ListView listView;

        [BindView(Resource.Id.listViewRemoveAcc)]
        ListView listViewRemoveAcc;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        [BindView(Resource.Id.layout_btnAddUser)]
        LinearLayout layout_btnAddUser;

        [BindView(Resource.Id.bottomLayoutDeleteMultiple)]
        LinearLayout bottomLayoutDeleteMultiple;

        [BindView(Resource.Id.btnCancelRemoveAccess)]
        Button btnCancelRemoveAccess;

        [BindView(Resource.Id.btnRemoveSelectedAccessUser)]
        Button btnRemoveSelectedAccessUser;

        [BindView(Resource.Id.btnAddUser)]
        Button btnAddUser;

        [BindView(Resource.Id.btnRemoveAccess)]
        Button btnRemoveAccess;

        [BindView(Resource.Id.btnAddAccessUser)]
        Button btnAddAccessUser;

        [BindView(Resource.Id.manage_user_layout)]
        FrameLayout manage_user_layout;

        [BindView(Resource.Id.txtManageAccessTitle)]
        TextView txtManageAccessTitle;

        [BindView(Resource.Id.txtEmptyManageAccess)]
        TextView txtEmptyManageAccess;

        ManageAccessAdapter adapter;

        ManageAccessDeleteAdapter adapterDelete;

        ISharedPreferences mPref;

        AccountData accountData;

        ManageAccessContract.IUserActionsListener userActionsListener;
        ManageAccessPresenter mPresenter;

        UserManageAccessAccount userManageAccessAccount;

        private IMenu ManageAccessMenu;

        List<UserManageAccessAccount> customerAccountList;

        const string PAGE_ID = "UserAccess";

        public override int ResourceId()
        {
            return Resource.Layout.ManageAccessView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ManageSupplyAccountToolbarMenu, menu);
            ManageAccessMenu = menu;
            ManageAccessMenu.FindItem(Resource.Id.icon_log_activity_unread).SetIcon(GetDrawable(Resource.Drawable.icon_log_empty)).SetVisible(true);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.icon_log_activity_unread:
                    SetToolBarTitle(GetLabelByLanguage("title"));
                    this.userActionsListener.OnAddLogUserAccess(accountData);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                // Create your application here
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        accountData = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                }

                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                txtEmptyManageAccess.Text = Utility.GetLocalizedLabel("UserAccess", "LabelEmptyTitle");
                txtManageAccessTitle.Text = Utility.GetLocalizedLabel("UserAccess", "LabelTitle");
                btnAddUser.Text = Utility.GetLocalizedLabel("UserAccess", "addUserBtn");
                btnRemoveAccess.Text = Utility.GetLocalizedLabel("UserAccess", "RemoveTitle");
                btnAddAccessUser.Text = Utility.GetLocalizedLabel("UserAccess", "AddTitle");
                btnCancelRemoveAccess.Text = Utility.GetLocalizedCommonLabel("cancel");
                btnRemoveSelectedAccessUser.Text = Utility.GetLocalizedLabel("UserAccess", "RemoveTitle");
                btnRemoveSelectedAccessUser.Enabled = false;

                TextViewUtils.SetMuseoSans300Typeface(txtEmptyManageAccess);
                TextViewUtils.SetMuseoSans500Typeface(txtManageAccessTitle, btnAddUser, btnRemoveAccess, btnAddAccessUser, btnCancelRemoveAccess, btnRemoveSelectedAccessUser);

                TextViewUtils.SetTextSize14(txtEmptyManageAccess);
                TextViewUtils.SetTextSize16(txtManageAccessTitle, btnAddUser, btnRemoveAccess, btnAddAccessUser, btnCancelRemoveAccess, btnRemoveSelectedAccessUser);
                
                adapter = new ManageAccessAdapter(this, false);
                adapterDelete = new ManageAccessDeleteAdapter(this, false);
                listViewRemoveAcc.Adapter = adapterDelete;
                listView.Adapter = adapter;
                adapter.setCustomButtonListner(this);
                adapterDelete.setCustomCheckboxListner(this);
                listView.SetNoScroll();
                listView.ItemClick += ListView_ItemClick;

                mPresenter = new ManageAccessPresenter(this, accountData, mPref);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                UserManageAccessAccount userManageAccessAccount = adapter.GetItemObject(e.Position);
                ShowManageSupplyAccount(userManageAccessAccount, e.Position);
            }
        }

        AlertDialog removeDialog;
        public void onButtonClickListner(int position)
        {
            ShowDeleteAccDialog(this, () =>
            {
                UserManageAccessAccount account = adapter.GetItemObject(position);
                UserManageAccessAccount.SetSelected(account.AccNum, true, account.UserAccountId, account.email, account.IsPreRegister);
                List<UserManageAccessAccount> DeletedSelectedUser = UserManageAccessAccount.ListIsSelected(accountData?.AccountNum);
                List<DeleteAccessAccount> accountList = new List<DeleteAccessAccount>();
                mPresenter.OnRemoveAccountMultiple(DeletedSelectedUser, false, accountList, accountData.AccountNum);
            });
        }

        public void AdapterClean()
        {
            adapter.Clear();
            listView.Adapter = null;
            listView.Adapter = adapter;
            adapter.setCustomButtonListner(this);
        }

        public void AdapterDeleteClean()
        {
            adapterDelete.Clear();
            listViewRemoveAcc.Adapter = null;
            listViewRemoveAcc.Adapter = adapterDelete;
            adapterDelete.setCustomCheckboxListner(this);
            DisableRemoveButton();
        }

        public void onCheckboxListener(int position)
        {
            int i = 0;
            customerAccountList = UserManageAccessAccount.List(accountData?.AccountNum);
            foreach (UserManageAccessAccount userManageAccessAccount in customerAccountList)
            {
                if (userManageAccessAccount.isSelected)
                {
                    i++;
                }

                if (i > 0)
                {
                    EnableRemoveButton();
                }
                else
                {
                    DisableRemoveButton();
                }
            }
        }

        [OnClick(Resource.Id.btnAddUser)]
        void AddUser(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent addAccountIntent = new Intent(this, typeof(AddNewUserActivity));
                    addAccountIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                    StartActivityForResult(addAccountIntent, Constants.ADD_USER);

                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnRemoveAccess)]
        void DeleteUser(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    txtManageAccessTitle.Text = Utility.GetLocalizedLabel("UserAccess", "HeaderRemoveTitle");
                    listViewRemoveAcc.Visibility = ViewStates.Visible;
                    bottomLayoutDeleteMultiple.Visibility = ViewStates.Visible;
                    bottomLayout.Visibility = ViewStates.Gone;
                    listView.Visibility = ViewStates.Gone;
                    layout_btnAddUser.Visibility = ViewStates.Gone;
                    manage_user_layout.Visibility = ViewStates.Gone;
                    AdapterDeleteClean();

                    List<UserManageAccessAccount> customerAccountLatest = UserManageAccessAccount.List(accountData?.AccountNum);
                    if (customerAccountLatest.Count > 0)
                    {
                        ShowAccountDeleteList(customerAccountLatest);
                    }
                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnAddAccessUser)]
        void AddAccessUser(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent addAccountIntent = new Intent(this, typeof(AddNewUserActivity));
                    addAccountIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                    StartActivityForResult(addAccountIntent, Constants.ADD_USER);
                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnCancelRemoveAccess)]
        void CancelDeleteUser(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    UserManageAccessAccount.UnSelectAll();
                    List<UserManageAccessAccount> customerAccountLatest = UserManageAccessAccount.List(accountData?.AccountNum);

                    if (customerAccountLatest.Count > 0)
                    {
                        AdapterClean();
                        ShowAccountList(customerAccountLatest);
                    }
                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnRemoveSelectedAccessUser)]
        void RemoveSelectedUser(object sender, EventArgs eventArgs)
        {
            ArrayList nameList2 = new ArrayList();
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    ShowDeleteAccDialog(this, () =>
                    {
                        List<UserManageAccessAccount> DeletedSelectedUser = UserManageAccessAccount.ListIsSelected(accountData?.AccountNum);
                        List<DeleteAccessAccount> accountList = new List<DeleteAccessAccount>();
                        mPresenter.OnRemoveAccountMultiple(DeletedSelectedUser, false, accountList, accountData.AccountNum);
                    });
                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public int checkListUserEmpty()
        {
            return UserManageAccessAccount.List(accountData?.AccountNum).Count;
        }

        public void UserAccessRemoveSuccess()
        {
            AdapterDeleteClean();

            List<UserManageAccessAccount> customerAccountLatest = UserManageAccessAccount.List(accountData?.AccountNum);

            if (customerAccountLatest.Count > 0)
            {
                ShowAccountDeleteList(customerAccountLatest);
            }

            listViewRemoveAcc.Visibility = ViewStates.Visible;
            bottomLayoutDeleteMultiple.Visibility = ViewStates.Visible;
            bottomLayout.Visibility = ViewStates.Gone;
            ShowRemoveMessageResponse();
        }

        public void UserAccessNonUserRemoveSuccess(string email)
        {
            AdapterDeleteClean();

            List<UserManageAccessAccount> customerAccountLatest = UserManageAccessAccount.List(accountData?.AccountNum);

            if (customerAccountLatest.Count > 0)
            {
                ShowAccountDeleteList(customerAccountLatest);
            }

            listViewRemoveAcc.Visibility = ViewStates.Visible;
            bottomLayoutDeleteMultiple.Visibility = ViewStates.Visible;
            bottomLayout.Visibility = ViewStates.Gone;
            ShowRemoveNonUserMessageResponse(email);
        }

        public void UserAccessRemoveSuccessSwipe()
        {
            AdapterClean();
            this.userActionsListener.Start();
            ShowRemoveMessageResponse();
        }

        public void UserAccessRemoveNonUserSuccessSwipe(string email)
        {
            AdapterClean();
            this.userActionsListener.Start();
            ShowRemoveNonUserMessageResponse(email);
        }

        public void NavigateLogUserAccess(List<LogUserAccessNewData> loglistdata)
        {
            Intent ManageUser = new Intent(this, typeof(LogUserAccessActivity));
            ManageUser.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(loglistdata));
            StartActivityForResult(ManageUser, Constants.UPDATE_NICKNAME_REQUEST);
        }

        public void ShowAddTNBUserSuccess(string email)
        {
            try
            {
                string nickname = accountData.AccountNickName;
                Snackbar saveSnackBar = Snackbar.Make(rootView, (string.Format(Utility.GetLocalizedLabel("UserAccess", "AddTNBUserSuccess"), email, nickname)), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = saveSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                TextViewUtils.SetTextSize14(tv);
                saveSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        
        public void ShowAddNonTNBUserSuccess(string email)
        {
            try
            {
                string nickname = accountData.AccountNickName;
                Snackbar saveSnackBar = Snackbar.Make(rootView, (string.Format(Utility.GetLocalizedLabel("UserAccess", "AddNonTNBUserSuccessToast"), email, nickname)), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = saveSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                TextViewUtils.SetTextSize14(tv);
                saveSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            try
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
                TextViewUtils.SetTextSize14(tv);
                mCancelledExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            try
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
                TextViewUtils.SetTextSize14(tv);
                mApiExcecptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            try
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
                TextViewUtils.SetTextSize14(tv);
                mUknownExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowManageSupplyAccount(UserManageAccessAccount accountData, int position)
        {
            try
            {
                Intent manageAccount = new Intent(this, typeof(ManageUserActivity));
                manageAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                manageAccount.PutExtra(Constants.SELECTED_ACCOUNT_POSITION, position);
                StartActivityForResult(manageAccount, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAddAccount()
        {
            try
            {
                Intent addAccountIntent = new Intent(this, typeof(LinkAccountActivity));
                StartActivity(addAccountIntent);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void ShowDeleteAccDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("UserAccess", "deleteUserTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("UserAccess", "deleteUserBody"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("UserAccess", "RemoveTitle"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public void ShowRemoveMessageResponse()
        {
            Snackbar errorMessageSnackbar =
            Snackbar.Make(rootView, Utility.GetLocalizedLabel("UserAccess", "deleteSuccessMessage"), Snackbar.LengthIndefinite)
                        .SetAction(Utility.GetLocalizedCommonLabel("close"),
                         (view) =>
                         {
                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        );//.Show();
            View snackbarView = errorMessageSnackbar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            TextViewUtils.SetTextSize14(textView);
            errorMessageSnackbar.Show();
        }

        public void ShowRemoveNonUserMessageResponse(string email)
        {
            Snackbar errorMessageSnackbar =
            Snackbar.Make(rootView, string.Format(Utility.GetLocalizedLabel("UserAccess", "cancelAddSuccess"),email), Snackbar.LengthIndefinite)
                        .SetAction(Utility.GetLocalizedCommonLabel("close"),
                         (view) =>
                         {
                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        );//.Show();
            View snackbarView = errorMessageSnackbar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            TextViewUtils.SetTextSize14(textView);
            errorMessageSnackbar.Show();
        }

        public void ShowDeleteMessageResponse(bool click)
        {
            try
            {
                if (removeDialog != null && removeDialog.IsShowing)
                {
                    removeDialog.Dismiss();
                }

                removeDialog = new AlertDialog.Builder(this)

                    .SetTitle(Utility.GetLocalizedLabel("ManageAccount", "popupremoveAccountTitle"))
                    .SetMessage(GetFormattedText(string.Format(Utility.GetLocalizedLabel("ManageAccount", "popupremoveAccountMessage"), accountData.AccountNickName, accountData.AccountNum)))
                    .SetNegativeButton(GetLabelCommonByLanguage("cancel"),
                    delegate
                    {
                        removeDialog.Dismiss();
                    })
                    .SetPositiveButton(GetLabelCommonByLanguage("ok"),
                    delegate
                    {
                        return;
                    })
                    .Show()
                    ;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowErrorMessageResponse(string error)
        {
            Snackbar errorMessageSnackbar =
            Snackbar.Make(rootView, error, Snackbar.LengthIndefinite)
                        .SetAction(Utility.GetLocalizedCommonLabel("close"),
                         (view) =>
                         {
                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        );//.Show();
            View snackbarView = errorMessageSnackbar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            TextViewUtils.SetTextSize14(textView);
            errorMessageSnackbar.Show();
        }

        public void ShowProgressDialog()
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "More -> My Account");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            //ShowAddAccount();
            base.OnPause();
        }

        public void HideShowProgressDialog()
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

        public void SetPresenter(ManageAccessContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void ShowAccountList(List<UserManageAccessAccount> accountList)
        {
            try
            {
                adapter.AddAll(accountList);
                adapter.NotifyDataSetChanged();
                listView.SetNoScroll();
                listView.Visibility = ViewStates.Visible;
                bottomLayout.Visibility = ViewStates.Visible;
                txtManageAccessTitle.Text = GetLabelByLanguage("LabelTitle");
                txtManageAccessTitle.Visibility = ViewStates.Visible;
                layout_btnAddUser.Visibility = ViewStates.Gone;
                manage_user_layout.Visibility = ViewStates.Gone;
                listViewRemoveAcc.Visibility = ViewStates.Gone;
                bottomLayoutDeleteMultiple.Visibility = ViewStates.Gone;
                Handler h = new Handler();
                Action myAction = () =>
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    if (!UserSessions.HasManageAccessPageTutorialShown(this.mPref))
                    {
                        OnManageAccessPageTutorialDialog();
                    }
                };
                h.PostDelayed(myAction, 50);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAccountDeleteList(List<UserManageAccessAccount> accountList)
        {
            try
            {
                adapterDelete.AddAll(accountList);
                adapterDelete.NotifyDataSetChanged();
                listView.SetNoScroll();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyAccount()
        {
            try
            {
                AdapterClean();
                AdapterDeleteClean();
                listView.EmptyView = manage_user_layout;
                txtManageAccessTitle.Visibility = ViewStates.Gone;
                bottomLayout.Visibility = ViewStates.Gone;
                bottomLayoutDeleteMultiple.Visibility = ViewStates.Gone;
                layout_btnAddUser.Visibility = ViewStates.Visible;
                Handler h = new Handler();
                Action myAction = () =>
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    if (!UserSessions.HasManageAccessPageTutorialShown(this.mPref))
                    {
                        OnManageAccessPageTutorialDialog();
                    }
                };
                h.PostDelayed(myAction, 50);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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

        public bool CheckIsScrollable()
        {
            View child = (View)listView.GetChildAt(0);

            return true;
        }

        public void HomeMenuCustomScrolling(int yPosition)
        {
            try
            {
                this.RunOnUiThread(() =>
                {
                    try
                    {
                        listView.ScrollTo(0, yPosition);
                        listView.RequestLayout();
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public int OnGetEndOfScrollView()
        {
            View child = (View)listView.GetChildAt(0);
            View childtext = (View)txtManageAccessTitle;

            return child.Height + listView.PaddingTop + listView.PaddingBottom + childtext.Height;
        }

        public int OnGetEndOfScrollView2()
        {
            View child = (View)listView.GetChildAt(1);
            View childtext = (View)txtManageAccessTitle;

            return child.Height + child.Height + listView.PaddingTop + listView.PaddingBottom + childtext.Height;
        }

        public void OnManageAccessPageTutorialDialog()
        {
            Handler h = new Handler();
            Action myAction = () =>
            {
                NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.mPresenter.OnGeneraNewAppTutorialList());
            };
            h.PostDelayed(myAction, 100);
        }

        public int GetViewBillButtonHeight()
        {
            int height = ManageAccessMenu.FindItem(Resource.Id.icon_log_activity_unread).Icon.IntrinsicHeight;
            return height;
        }

        public int GetViewBillButtonWidth()
        {
            int width = ManageAccessMenu.FindItem(Resource.Id.icon_log_activity_unread).Icon.IntrinsicWidth;
            return width;
        }

        public int GetTopHeight()
        {
            int i = 0;
            try
            {
                Rect offsetViewBounds = new Rect();
                //returns the visible bounds
                toolbar.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent
                rootView.OffsetDescendantRectToMyCoords(toolbar, offsetViewBounds);
                i = offsetViewBounds.Top + (int)DPUtils.ConvertDPToPx(14f);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return i;
        }

        public void ShowAddNewUserEmailExistSuccess()
        {
            try
            {
                Snackbar updateNameBar = Snackbar.Make(rootView, GetLabelByLanguage("NameUpdateSuccess"), Snackbar.LengthIndefinite)
                            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updateNameBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updateNameBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAddNewUserEmailNotExistSuccess()
        {
            try
            {
                Snackbar updateNameBar = Snackbar.Make(rootView, GetLabelByLanguage("NameUpdateSuccess"), Snackbar.LengthIndefinite)
                            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updateNameBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updateNameBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowCancelAddSuccess(string email)
        {
            Snackbar errorMessageSnackbar =
            Snackbar.Make(rootView, string.Format(Utility.GetLocalizedLabel("UserAccess", "cancelAddSuccess"), email), Snackbar.LengthIndefinite)
                        .SetAction(Utility.GetLocalizedCommonLabel("close"),
                         (view) =>
                         {
                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        );//.Show();
            View snackbarView = errorMessageSnackbar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            TextViewUtils.SetTextSize14(textView);
            errorMessageSnackbar.Show();
        }

        public void ShowGetCardsProgressDialog()
        {
            throw new NotImplementedException();
        }

        public void HideGetCardsProgressDialog()
        {
            //throw new NotImplementedException();
        }

        public void ShowRemovedSupplyAccountSuccess(AccountData accountData, int position)
        {
            try
            {
                adapter.Remove(position);
                Snackbar removeSupplySnackbar = Snackbar.Make(rootView, GetString(Resource.String.my_account_successful_remove_supply_account), Snackbar.LengthIndefinite)
                           .SetAction(Utility.GetLocalizedCommonLabel("close"),
                            (view) =>
                            {

                                // EMPTY WILL CLOSE SNACKBAR
                            }
                           );
                View v = removeSupplySnackbar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                removeSupplySnackbar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearAccountsAdapter()
        {
            adapter.Clear();
            listView.Adapter = null;
            listView.Adapter = adapter;
            adapter.setCustomButtonListner(this);
        }

        public void ShowAccountRemovedSuccess()
        {
            try
            {
                Snackbar updatePassWordBar = Snackbar.Make(rootView, GetLabelByLanguage("accountDeleteSuccess"), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
                             (view) =>
                             {

                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );

                View v = updatePassWordBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePassWordBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void EnableRemoveButton()
        {
            btnRemoveSelectedAccessUser.Enabled = true;
            btnRemoveSelectedAccessUser.SetTextColor(Color.ParseColor("#e44b21"));
            btnRemoveSelectedAccessUser.Background = ContextCompat.GetDrawable(this, Resource.Drawable.red_light_button_background);
        }

        public void DisableRemoveButton()
        {
            btnRemoveSelectedAccessUser.Enabled = false;
            btnRemoveSelectedAccessUser.SetTextColor(Color.ParseColor("#a6a6a6"));
            btnRemoveSelectedAccessUser.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

    }
}
