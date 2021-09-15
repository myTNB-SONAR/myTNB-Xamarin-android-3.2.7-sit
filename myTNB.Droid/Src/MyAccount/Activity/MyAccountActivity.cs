using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
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
using myTNB_Android.Src.MyAccount.Adapter;
using myTNB_Android.Src.MyAccount.MVP;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using static myTNB_Android.Src.MyAccount.Adapter.MyAccountAdapter;

namespace myTNB_Android.Src.MyAccount.Activity
{
    [Activity(Label = "@string/my_account_activity_title_new"
        //, Icon = "@drawable/Logo"
        //, MainLauncher = true
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class MyAccountActivity : BaseActivityCustom, MyAccountContract.IView, customButtonListener
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.listView)]
        ListView listView;

        [BindView(Resource.Id.btnAddAnotherAccount)]
        Button btnAddAnotherAccount;

        //[BindView(Resource.Id.btnAddAccount)]
        //Button btnAddAccount;

        [BindView(Resource.Id.no_account_layout)]
        FrameLayout NoAccountLayout;

        //[BindView(Resource.Id.txtMyAccountNoAccountTitle)]
        //TextView txtMyAccountNoAccountTitle;

        [BindView(Resource.Id.txtMyAccountNoAccountContent)]
        TextView txtMyAccountNoAccountContent;

        MyAccountAdapter adapter;

        AccountData accountData;

        MyAccountContract.IUserActionsListener userActionsListener;
        MyAccountPresenter mPresenter;

        MaterialDialog accountRetrieverDialog;

        private bool fromDashboard = false;

        const string PAGE_ID = "ManageAccount";

        private static int HORIZONTAL_MIN = 30; 

        public override int ResourceId()
        {
            return Resource.Layout.MyAccountViewNew;
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
                if (Intent.HasExtra("fromDashboard"))
                {
                    fromDashboard = Intent.Extras.GetBoolean("fromDashboard", false);
                }
                // Create your application here
                //TextViewUtils.SetMuseoSans300Typeface(
                //    txtMyAccountNoAccountContent);
                //TextViewUtils.SetMuseoSans500Typeface(btnAddAnotherAccount,
                //    btnAddAccount);

                TextViewUtils.SetTextSize12(txtMyAccountNoAccountContent);
                //TextViewUtils.SetTextSize14(txtMyAccountNoAccountTitle);
                TextViewUtils.SetTextSize16( btnAddAnotherAccount);
                //TextViewUtils.SetTextSize18(txtTnBSupplyAccountTitle);

                //txtTnBSupplyAccountTitle.Text = GetLabelByLanguage("accountSectionTitle");
                //btnAddAnotherAccount.Text = Utility.GetLocalizedLabel("AddAccount", "addAccountCTATitle");
                //txtMyAccountNoAccountTitle.Text = GetLabelByLanguage("noAccounts");
                txtMyAccountNoAccountContent.Text = Utility.GetLocalizedLabel("MyAccount", "emptyListDetails");
                //btnAddAccount.Text = Utility.GetLocalizedLabel("AddAccount", "addAccountCTATitle");
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                adapter = new MyAccountAdapter(this, false);
                listView.Adapter = adapter;
                adapter.setCustomButtonListner(this);
                listView.SetNoScroll();
                listView.ItemClick += ListView_ItemClick;

                mPresenter = new MyAccountPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

/*        public bool OnTouch(View v, MotionEvent e)
        {
            float downX, downY, upX, upY;
            if (v is ListView)
            {
                ListView eListView = v as ListView;
                if (eListView.Id == Resource.Id.listView)
                {
                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            {
                                 downX = e.GetX();
                                 downY = e.GetY();
                                 return false; // allow other events like Click to be processed
                            }
                        case MotionEventActions.Move:
                            upX = e.GetX();
                            upY = e.GetY();
                            *//*float deltaX = downX - upX;
                            float deltaY = downY - upY;*//*
                            if (Math.Abs(upX) > HORIZONTAL_MIN)
                            {
                                // left or right
                                if (upX < 0)
                                {
                                    ((SwipeLayout)(listView.GetChildAt(listView.FirstVisiblePosition))).Open(SwipeLayout.DragEdge.Right);
                                    return true;
                                }
                                if (upX > 0)
                                {
                                    return true;
                                }
                            }
                            break;
                    }
                    return true;
                }
            }
            return false;
        }*/

        [Preserve]

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                CustomerBillingAccount customerBillingAccount = adapter.GetItemObject(e.Position);
                ShowManageSupplyAccount(AccountData.Copy(customerBillingAccount, false), e.Position);
            }
        }

        AlertDialog removeDialog;
        public void onButtonClickListner(int position)
        {
            ShowDeleteAccDialog(this, position, () =>
            {
                CustomerBillingAccount account = adapter.GetItemObject(position);
                this.mPresenter.OnRemoveAccount(account.AccNum);                
            });           
        }

        public void showsuccessDelete()
        {
            adapter.Clear();
            listView.Adapter = null;
            listView.Adapter = adapter;
            adapter.setCustomButtonListner(this);
            this.userActionsListener.Start();
            ShowAccountRemovedSuccess();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (fromDashboard)
                ShowDashboard();
            else
                this.Finish();
        }

        //public void NavigateToDashboard()
        //{
        //    if (fromDashboard)
        //        ShowDashboard();
        //    else
        //        this.Finish();
        //}

        public void ShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
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

        //[OnClick(Resource.Id.btnAddAccount)]
        //void OnClickAddAccount(object sender, EventArgs eventArgs)
        //{
        //    if (!this.GetIsClicked())
        //    {
        //        this.SetIsClicked(true);
        //        ShowAddAccount();
        //    }
        //}

        [OnClick(Resource.Id.btnAddAnotherAccount)]
        void OnClickAddAnotherAccount(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                ShowAddAccount();
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
                        .SetContentGravity(Android.Views.GravityFlags.Left)
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
            //int titleId = Resources.GetIdentifier("alertTitle", "id", "android");
            //TextView txtTitle = removeDialog.FindViewById<TextView>(titleId);
            //txtTitle.SetTextSize(ComplexUnitType.Sp ,17);
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

        public void SetPresenter(MyAccountContract.IUserActionsListener userActionListener)
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
                NoAccountLayout.Visibility = ViewStates.Gone;
                adapter.AddAll(accountList);
                adapter.NotifyDataSetChanged();
                listView.SetNoScroll();
                btnAddAnotherAccount.Text = GetLabelCommonByLanguage("addAnotherAcct");
                //btnAddAnotherAccount.Visibility = ViewStates.Visible;
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
                listView.EmptyView = NoAccountLayout;
                btnAddAnotherAccount.Visibility = ViewStates.Visible;
                btnAddAnotherAccount.Text = Utility.GetLocalizedLabel("AddAccount", "addAccountCTATitle");
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
                TextViewUtils.SetTextSize14(tv);
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
                TextViewUtils.SetTextSize14(tv);
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
