using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
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
using Android.Support.V4.Content;
using Android.Text;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.AddNewUser.Activity
{
    [Activity(Label = "@string/manage_supply_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class AddNewUserActivity : BaseActivityCustom, AddNewUserContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

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

        [BindView(Resource.Id.itemTitleFullBill)]
        TextView itemTitleFullBill;

        [BindView(Resource.Id.itemTitleBilling)]
        TextView itemTitleBilling;

        [BindView(Resource.Id.itemActionBilling)]
        CheckBox itemActionBilling;

        [BindView(Resource.Id.itemActionFullBill)]
        CheckBox itemActionFullBill;

        [BindView(Resource.Id.btnAddUser)]
        Button btnAddUser;

        [BindView(Resource.Id.infoAddress)]
        TextView infoAddress;

        AccountData accountData;
        int position;

        private bool checkboxbilling;
        private bool checkboxfullbill;

        private ManageSupplyItemContentComponent manageUser;
        AddNewUserContract.IUserActionsListener userActionsListener;
        AddNewUserPresenter mPresenter;


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
                        accountData = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                    position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                }

                var boxcondition = new CheckBox(this)
                {
                    ScaleX = 0.8f,
                    ScaleY = 0.8f
                };

                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutUserEmail);
                TextViewUtils.SetMuseoSans300Typeface(txtUserEmail);
                TextViewUtils.SetMuseoSans300Typeface(txtValue, itemTitleFullBill, itemTitleBilling, infoAddress);
                TextViewUtils.SetMuseoSans500Typeface(txtAddNewUserTitle, txtNewUserOptionalTitle);
                TextViewUtils.SetMuseoSans500Typeface(btnAddUser);

                itemTitleFullBill.TextSize = TextViewUtils.GetFontSize(12);
                itemTitleBilling.TextSize = TextViewUtils.GetFontSize(12);
                txtUserEmail.TextSize = TextViewUtils.GetFontSize(12);
                txtValue.TextSize = TextViewUtils.GetFontSize(12);
                infoAddress.TextSize = TextViewUtils.GetFontSize(12);
                txtAddNewUserTitle.TextSize = TextViewUtils.GetFontSize(14);
                txtNewUserOptionalTitle.TextSize = TextViewUtils.GetFontSize(14);
                btnAddUser.TextSize = TextViewUtils.GetFontSize(14);

                itemTitleFullBill.Text = Utility.GetLocalizedLabel("AddUserAccess", "viewFullElec");
                itemTitleBilling.Text = Utility.GetLocalizedLabel("AddUserAccess", "applyEBilling");
                txtAddNewUserTitle.Text = Utility.GetLocalizedLabel("AddUserAccess", "titleAddNewUser");
                txtNewUserOptionalTitle.Text = Utility.GetLocalizedLabel("AddUserAccess", "bodyAddNewUser");
                textInputLayoutUserEmail.Hint = Utility.GetLocalizedLabel("manageUser", "usremail");
                btnAddUser.Text = Utility.GetLocalizedLabel("AddUserAccess", "addUserButton");
                txtValue.Text = Utility.GetLocalizedLabel("AddUserAccess", "titleAddEmail");
                infoAddress.Text = Utility.GetLocalizedLabel("AddUserAccess", "whatIsThisMean");

                txtUserEmail.AddTextChangedListener(new InputFilterFormField(txtUserEmail, textInputLayoutUserEmail));
                txtUserEmail.AfterTextChanged += new EventHandler<AfterTextChangedEventArgs>(AddTextChangedListener);
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                SetToolBarTitle(Utility.GetLocalizedLabel("AddUserAccess", "title"));

                itemActionFullBill.CheckedChange += CheckedChange;
                itemActionBilling.CheckedChange += CheckedChanged;

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
                    string email = txtUserEmail.Text.ToString().Trim();
                    this.SetIsClicked(true);
                    this.userActionsListener.OnAddAccount(email, accountData.AccountNum, checkboxfullbill, checkboxbilling);


                    //ShowAddTNBUserSuccess();
                    //ShowAddNonTNBUserSuccess();
                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }
        
        private void CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                checkboxfullbill = true;
            }
            else
            {
                checkboxfullbill = false;
            }
        }

        private void CheckedChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                checkboxbilling = true;
            }
            else
            {
                checkboxbilling = false;
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

        public void ShowSuccessAddNewUser(string email)
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("Invited", email);
            SetResult(Result.Ok, resultIntent);
            Finish();
        }

        public void ShowSuccessAddNewUserPreRegister(string email)
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("Add", email);
            SetResult(Result.Ok, resultIntent);
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
        }

        private void AddTextChangedListener(object sender, AfterTextChangedEventArgs e)
        {
            try
            {
                string email = txtUserEmail.Text.ToString().Trim();
                this.userActionsListener.CheckRequiredFields(email);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
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

        [OnClick(Resource.Id.infoAddress)]
        void OnClickAddressInfo(object sender, EventArgs eventArgs)
        {

            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                       .SetTitle((string.Format(Utility.GetLocalizedLabel("AddUserAccess", "toottipTitle"))))
                       .SetMessage(string.Format(Utility.GetLocalizedLabel("AddUserAccess", "tooltipBody")))
                       .SetContentGravity(GravityFlags.Left)
                       .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                       .Build().Show();
        }

        public void EnableAddUserButton()
        {
            btnAddUser.Enabled = true;
            btnAddUser.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableAddUserButton()
        {
            btnAddUser.Enabled = false;
            btnAddUser.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
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
