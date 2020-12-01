﻿using AFollestad.MaterialDialogs;
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
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageSupplyAccount.Activity;
using myTNB_Android.Src.ManageAccess.Adapter;
using myTNB_Android.Src.ManageAccess.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using static myTNB_Android.Src.ManageAccess.Adapter.ManageAccessAdapter;
using myTNB_Android.Src.AddNewUser.Activity;

namespace myTNB_Android.Src.ManageAccess.Activity
{
    [Activity(Label = "@string/my_account_activity_title_new"
        //, Icon = "@drawable/Logo"
        //, MainLauncher = true
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.MyAccount")]
    public class ManageAccessActivity : BaseActivityCustom, ManageAccessContract.IView, customButtonListener
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.listView)]
        ListView listView;

        [BindView(Resource.Id.listViewRemoveAcc)]
        ListView listViewRemoveAcc;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        [BindView(Resource.Id.layout_btnAddUser)]
        LinearLayout layout_btnAddUser;

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

        AccountData accountData;

        ManageAccessContract.IUserActionsListener userActionsListener;
        ManageAccessPresenter mPresenter;

        MaterialDialog accountRetrieverDialog;

        const string PAGE_ID = "UserAccess";

        public override int ResourceId()
        {
            return Resource.Layout.ManageAccessView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                // Create your application here

                txtEmptyManageAccess.Text = GetLabelByLanguage("LabelEmptyTitle");
                txtManageAccessTitle.Text = GetLabelByLanguage("LabelTitle");
                btnAddUser.Text = GetLabelByLanguage("addUserBtn");
                btnRemoveAccess.Text = GetLabelByLanguage("RemoveTitle");
                btnAddAccessUser.Text = GetLabelByLanguage("AddTitle");

                adapter = new ManageAccessAdapter(this, false);
                listView.Adapter = adapter;
                listView.SetNoScroll();
                listView.ItemClick += ListView_ItemClick;

                listView.Touch += (sender, e) =>
                {
                    ((SwipeLayout)(listView.GetChildAt(listView.FirstVisiblePosition))).Open(SwipeLayout.DragEdge.Right);
                    Console.WriteLine("ListView: OnTouch");
                    e.Handled = true;
                };

                mPresenter = new ManageAccessPresenter(this);
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
                /*CustomerBillingAccount customerBillingAccount = adapter.GetItemObject(e.Position);
                ShowManageSupplyAccount(AccountData.Copy(customerBillingAccount, false), e.Position);*/
            }
        }

        AlertDialog removeDialog;
        public void onButtonClickListner(int position)
        {
            ShowDeleteAccDialog(this, position, () =>
            {
                /*CustomerBillingAccount account = adapter.GetItemObject(position);
                CustomerBillingAccount.Remove(account.AccNum);
                this.mPresenter.OnRemoveAccount(account.AccNum);
                adapter.Clear();
                listView.Adapter = null;
                listView.Adapter = adapter;
                adapter.setCustomButtonListner(this);
                this.userActionsListener.Start();*/
            });           
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
                    StartActivity(addAccountIntent);

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
                    txtManageAccessTitle.Text = GetLabelByLanguage("HeaderRemoveTitle");
                    listViewRemoveAcc.Visibility = ViewStates.Visible;
                    bottomLayout.Visibility = ViewStates.Visible;
                    listView.Visibility = ViewStates.Gone;
                    layout_btnAddUser.Visibility = ViewStates.Gone;
                    manage_user_layout.Visibility = ViewStates.Gone;

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
                    StartActivity(addAccountIntent);

                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
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
                mUknownExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowManageSupplyAccount(AccountData accountData, int position)
        {
            try
            {
                Intent manageAccount = new Intent(this, typeof(ManageSupplyAccountActivityEdit));
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

        void ShowDeleteAccDialog(Android.App.Activity context, int position, Action confirmAction, Action cancelAction = null)
        {
            CustomerBillingAccount account = adapter.GetItemObject(position);
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("ManageAccount", "popupremoveAccountTitle"))
                        //.SetMessage(Utility.GetLocalizedLabel("Common", "updateIdMessage"))
                        .SetMessage(string.Format(Utility.GetLocalizedLabel("ManageAccount", "popupremoveAccountMessage"), account.AccDesc, account.AccNum))
                        .SetContentGravity(Android.Views.GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "ok"))
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

        public void ShowAccountList(List<CustomerBillingAccount> accountList)
        {
            try
            {
                adapter.AddAll(accountList);
                adapter.NotifyDataSetChanged();
                listView.SetNoScroll();
                bottomLayout.Visibility = ViewStates.Visible;
                //txtManageAccessTitle.Visibility = ViewStates.Gone;
                layout_btnAddUser.Visibility = ViewStates.Gone;
                manage_user_layout.Visibility = ViewStates.Gone;
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
                listView.EmptyView = manage_user_layout;
                txtManageAccessTitle.Visibility = ViewStates.Gone;
                layout_btnAddUser.Visibility = ViewStates.Visible;
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }

    }
}
