using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.AddNewUser.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateNickname.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;
using System.Collections.Generic;
using myTNB_Android.Src.UpdatePassword.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using Org.BouncyCastle.Crypto.Signers;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.ManageAccess.Activity;

namespace myTNB_Android.Src.AddNewUser.Activity
{
    [Activity(Label = "@string/manage_supply_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.ManageSupplyAccount")]
    public class AddNewUserActivity : BaseActivityCustom, AddNewUserContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtAddNewUserTitle)]
        TextView txtAddNewUserTitle;

        [BindView(Resource.Id.txtValue)]
        TextView txtValue;

        [BindView(Resource.Id.textInputLayoutUserEmail)]
        TextInputLayout textInputLayoutUserEmail;

        [BindView(Resource.Id.txtUserEmail)]
        EditText txtUserEmail;

        [BindView(Resource.Id.txtNewUserOptionalTitle)]
        TextView txtNewUserOptionalTitle;

        [BindView(Resource.Id.profileMenuItemsContent)]
        LinearLayout profileMenuItemsContent;

        [BindView(Resource.Id.btnAddUser)]
        Button btnAddUser;

        [BindView(Resource.Id.infoAddress)]
        TextView infoAddress;

        private IMenu ManageSupplyAccountMenu;
        AccountData accountData;
        int position;

        private ManageSupplyItemContentComponent manageUser;
        AddNewUserContract.IUserActionsListener userActionsListener;
        AddNewUserPresenter mPresenter;

        MaterialDialog progress;

        const string PAGE_ID = "ManageAccount";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        //accountData = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
                        accountData = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));

                    }
                    position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                }


                progress = new MaterialDialog.Builder(this)
                    .Title(Resource.String.manage_supply_account_remove_progress_title)
                    .Content(Resource.String.manage_supply_account_remove_progress_content)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                ManageAccessItemComponent manageAccessUserItem = GetManageUserAccess();
                profileMenuItemsContent.AddView(manageAccessUserItem);

                var boxcondition = new CheckBox(this)
                {
                    ScaleX = 0.8f,
                    ScaleY = 0.8f
                };

                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutUserEmail);
                TextViewUtils.SetMuseoSans300Typeface(txtUserEmail);
                TextViewUtils.SetMuseoSans300Typeface(txtValue);
                TextViewUtils.SetMuseoSans500Typeface(txtAddNewUserTitle, txtNewUserOptionalTitle);
                TextViewUtils.SetMuseoSans500Typeface(btnAddUser);


                txtAddNewUserTitle.Text = Utility.GetLocalizedLabel("UserAccess", "titleAddNewUser");
                txtNewUserOptionalTitle.Text = Utility.GetLocalizedLabel("UserAccess", "bodyAddNewUser");
                textInputLayoutUserEmail.Hint = Utility.GetLocalizedLabel("UserAccess", "usremail");
                btnAddUser.Text = Utility.GetLocalizedLabel("UserAccess", "addUserBtn");

                txtUserEmail.AddTextChangedListener(new InputFilterFormField(txtUserEmail, textInputLayoutUserEmail));

                mPresenter = new AddNewUserPresenter(this, accountData);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
                    ShowSuccessAddNewUser();

                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.AddNewUserView;
        }

        public void ShowSuccessAddNewUser()
        {
            SetResult(Result.Ok);
            Finish();
        }


        public void SetPresenter(AddNewUserContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Manage Electricity Account");
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

        private ManageAccessItemComponent GetManageUserAccess()
        {
            //Context context = Activity.ApplicationContext;

            ManageAccessItemComponent manageItem = new ManageAccessItemComponent(this);

            List<View> manageItems = new List<View>();

            ManageUserMenuItemSingleContentComponent electricityBill = new ManageUserMenuItemSingleContentComponent(this);
            electricityBill.SetTitle(GetLabelCommonByLanguage("manageUserAccess"));
            electricityBill.SetItemActionVisibility(true);
            electricityBill.SetItemActionCall(ClickCheck);
            manageItems.Add(electricityBill);

            ManageUserMenuItemSingleContentComponent eBilling = new ManageUserMenuItemSingleContentComponent(this);
            eBilling.SetTitle(GetLabelCommonByLanguage("manageAutopay"));
            eBilling.SetItemActionVisibility(true);
            eBilling.SetItemActionCall(ClickCheck);
            manageItems.Add(eBilling);

            manageItem.AddComponentView(manageItems);
            return manageItem;
        }

        private void ClickCheck()
        {
        }

        public void ShowUpdateNickname()
        {
            Intent updateNickName = new Intent(this, typeof(UpdateNicknameActivity));
            updateNickName.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            StartActivityForResult(updateNickName, Constants.UPDATE_NICKNAME_REQUEST);
        }

        public void ShowSuccessRemovedAccount()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            result.PutExtra(Constants.SELECTED_ACCOUNT_POSITION, position);
            result.PutExtra(Constants.ACCOUNT_REMOVED_FLAG, true);
            SetResult(Result.Ok, result);
            Finish();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
        }

        public void ShowRemoveProgress()
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

        public void HideRemoveProgress()
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

        MaterialDialog addressInfoDialog;
        //AlertDialog addressInfo;
        [OnClick(Resource.Id.infoAddress)]
        void OnClickAddressInfo(object sender, EventArgs eventArgs)
        {

            try
            {
                if (addressInfoDialog != null && addressInfoDialog.IsShowing)
                {
                    addressInfoDialog.Dismiss();
                }

                addressInfoDialog = new MaterialDialog.Builder(this)
                    .CustomView(Resource.Layout.WhyCantSeeFullAddressView, false)
                    .Cancelable(true)
                    .PositiveText(GetLabelCommonByLanguage("gotIt"))
                    .PositiveColor(Resource.Color.gradientPurple)
                    .ItemsGravity(GravityEnum.Center)
                    .Build();

                View view = addressInfoDialog.View;
                if (view != null)
                {
                    TextView titleText = view.FindViewById<TextView>(Resource.Id.textDialogTitle);
                    TextView infoText = view.FindViewById<TextView>(Resource.Id.textDialogInfo);
                    if (titleText != null && infoText != null)
                    {
                        TextViewUtils.SetMuseoSans500Typeface(titleText);
                        TextViewUtils.SetMuseoSans300Typeface(infoText);

                        titleText.Text = Utility.GetLocalizedLabel("ManageAccount", "dialogAddrress");
                        infoText.Text = Utility.GetLocalizedLabel("ManageAccount", "dialogAddrressMessage");
                    }
                }
                addressInfoDialog.Show();
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

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
            View snackbarView = mCancelledExceptionSnackBar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            mCancelledExceptionSnackBar.Show();

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
            View snackbarView = mApiExcecptionSnackBar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            mApiExcecptionSnackBar.Show();

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
            View snackbarView = mUknownExceptionSnackBar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            mUknownExceptionSnackBar.Show();

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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
