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
using myTNB.AndroidApp.Src.CompoundView;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.UpdateNickname.Activity;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.ManageUser.MVP;
using AndroidX.Core.Content;
using myTNB.AndroidApp.Src.Base;
using AndroidX.CoordinatorLayout.Widget;
using Android.Preferences;

namespace myTNB.AndroidApp.Src.ManageUser.Activity
{
    [Activity(Label = "@string/manage_supply_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class ManageUserActivity : BaseActivityCustom, ManageUserContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtNickName)]
        EditText txtNickName;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextView txtInputLayoutEmail;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.itemTitle)]
        TextView itemTitle;

        [BindView(Resource.Id.itemTitleFullBill)]
        TextView itemTitleFullBill;

        [BindView(Resource.Id.itemTitleBilling)]
        TextView itemTitleBilling;

        [BindView(Resource.Id.itemActionBilling)]
        CheckBox itemActionBilling;

        [BindView(Resource.Id.itemActionFullBill)]
        CheckBox itemActionFullBill;

        [BindView(Resource.Id.btnSave)]
        Button btnSave;

        [BindView(Resource.Id.infoManageUser)]
        TextView infoManageUser;

        [BindView(Resource.Id.txtNameLayout)]
        FrameLayout txtNameLayout;

        [BindView(Resource.Id.bottomLayout_Cancel_Resend)]
        LinearLayout bottomLayout_Cancel_Resend;

        [BindView(Resource.Id.bottomLayoutSave)]
        LinearLayout bottomLayoutSave;

        [BindView(Resource.Id.view2)]
        View view2;

        [BindView(Resource.Id.btnCancelAddAccess)]
        Button btnCancelAddAccess;

        [BindView(Resource.Id.btnResendInviteAccessUser)]
        Button btnResendInviteAccessUser;

        [BindView(Resource.Id.btnResendInviteDisable)]
        Button btnResendInviteDisable;
        
        private IMenu ManageSupplyAccountMenu;

        AccountData accountData;
        UserManageAccessAccount account;

        UserManageAccessAccount accountnew;

        private bool checkboxbilling;
        private bool checkboxfullbill;
        private bool buttonEnableview = false;
        private bool buttonEnableEBilling = false;


        int position;

        private ManageSupplyItemContentComponent manageUser;
        ManageUserContract.IUserActionsListener userActionsListener;
        ManageUserPresenter mPresenter;

        MaterialDialog progress;
        private bool isSelectionChange;

        const string PAGE_ID = "ManageAccount";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            isSelectionChange = false;
            base.OnCreate(savedInstanceState);

            try
            {

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        //accountData = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
                        account = DeSerialze<UserManageAccessAccount>(extras.GetString(Constants.SELECTED_ACCOUNT));

                    }
                    position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                }

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail);
                TextViewUtils.SetMuseoSans300Typeface(txtNickName);
                TextViewUtils.SetMuseoSans300Typeface(txtEmail);
                TextViewUtils.SetMuseoSans300Typeface(infoManageUser);
                TextViewUtils.SetMuseoSans300Typeface(itemTitleFullBill, itemTitleBilling);
                TextViewUtils.SetMuseoSans500Typeface(btnSave, itemTitle, btnCancelAddAccess, btnResendInviteAccessUser, btnResendInviteDisable);
                //TextViewUtils.SetMuseoSans500Typeface(bottomLayout_Cancel_Resend, bottomLayoutSave);

                TextViewUtils.SetTextSize14(txtInputLayoutEmail, txtNickName, txtEmail, itemTitleFullBill, itemTitleBilling, infoManageUser, btnSave, itemTitle, btnCancelAddAccess, btnResendInviteAccessUser);


                itemTitleFullBill.Text = Utility.GetLocalizedLabel("manageUser", "fullElectricity");
                itemTitleBilling.Text = Utility.GetLocalizedLabel("manageUser", "e_billing");
                itemTitle.Text = Utility.GetLocalizedLabel("manageUser", "ManageUserTitle");
                SetToolBarTitle(GetLabelByLanguage("titleManageUser"));
                txtInputLayoutEmail.Hint = Utility.GetLocalizedLabel("UserAccess", "hint_email").ToUpper();
                btnSave.Text = GetLabelCommonByLanguage("saveChanges");
                btnCancelAddAccess.Text = GetLabelCommonByLanguage("cancel");
                btnResendInviteAccessUser.Text = Utility.GetLocalizedLabel("Tnb_Profile", "resend");
                btnResendInviteDisable.Text = Utility.GetLocalizedLabel("Tnb_Profile", "resend");
                infoManageUser.Text = Utility.GetLocalizedLabel("Usage", "missedReadTitle");


                //txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));
                txtNickName.Text = account.name;
                txtEmail.Text = account.email;
                DisableSaveButton();
                if (account.IsPreRegister)
                {
                    PopulateCheckBoxPreRegister();
                    bottomLayout_Cancel_Resend.Visibility = ViewStates.Visible;
                    btnResendInviteDisable.Visibility = ViewStates.Gone;
                    bottomLayoutSave.Visibility = ViewStates.Gone;
                    view2.Visibility = ViewStates.Gone;
                    txtNameLayout.Visibility = ViewStates.Gone;
                    itemActionFullBill.Clickable = false;
                    itemActionBilling.Clickable = false;
                }
                else
                {
                    itemActionFullBill.CheckedChange += CheckedChange;
                    itemActionBilling.CheckedChange += CheckedChanged;
                    PopulateDataCheckBox(account);
                }

                MyTNBAccountManagement.GetInstance().AddNewUserAdded(false);
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                mPresenter = new ManageUserPresenter(this, account, PreferenceManager.GetDefaultSharedPreferences(this));
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ManageUserView;
        }


        public void SetPresenter(ManageUserContract.IUserActionsListener userActionListener)
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

        public void PopulateCheckBoxPreRegister()
        {
            try
            {
                if (account.IsApplyEBilling)
                {
                    itemActionBilling.Checked = true;
                    itemActionBilling.SetButtonDrawable(Resource.Drawable.checkbox_active_grey);
                }
                else
                {
                    itemActionBilling.Checked = false;
                    itemActionBilling.SetButtonDrawable(Resource.Drawable.checkbox_disabled);
                }

                if (account.IsHaveAccess)
                {
                    itemActionFullBill.Checked = true;
                    itemActionFullBill.SetButtonDrawable(Resource.Drawable.checkbox_active_grey);
                }
                else
                {
                    itemActionFullBill.Checked = false;
                    itemActionFullBill.SetButtonDrawable(Resource.Drawable.checkbox_disabled);
                }
                DisableSaveButton();
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void PopulateDataCheckBox(UserManageAccessAccount accountCheckBox)
        {
            try
            {
                accountnew = accountCheckBox;
                if (accountCheckBox.IsApplyEBilling)
                {
                    itemActionBilling.Checked = true;
                }
                else
                {
                    itemActionBilling.Checked = false;
                }

                if (accountCheckBox.IsHaveAccess)
                {
                    itemActionFullBill.Checked = true;

                }
                else
                {
                    itemActionFullBill.Checked = false;
                }
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
                if (accountnew.IsHaveAccess && checkboxbilling.Equals(accountnew.IsApplyEBilling))
                {
                    DisableSaveButton();
                    buttonEnableview = false;
                }
                else
                {
                    EnableSaveButton();
                    buttonEnableview = true;
                }
            }
            else
            {
                checkboxfullbill = false;
                if (!accountnew.IsHaveAccess && checkboxbilling.Equals(accountnew.IsApplyEBilling))
                {
                    DisableSaveButton();
                    buttonEnableview = false;
                }
                else
                {
                    EnableSaveButton();
                    buttonEnableview = true;
                }
            }

        }

        private void CheckedChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                checkboxbilling = true;
                if (accountnew.IsApplyEBilling && checkboxfullbill.Equals(accountnew.IsHaveAccess))
                {
                    DisableSaveButton();
                    buttonEnableEBilling = false;
                }
                else
                {
                    EnableSaveButton();
                    buttonEnableEBilling = true;
                }
            }
            else
            {
                checkboxbilling = false;
                if (!accountnew.IsApplyEBilling && checkboxfullbill.Equals(accountnew.IsHaveAccess))
                {
                    DisableSaveButton();
                    buttonEnableEBilling = false;
                }
                else
                {
                    EnableSaveButton();
                    buttonEnableEBilling = true;
                }
            }

        }

        [OnClick(Resource.Id.infoManageUser)]
        void OnClickManagerUserInfo(object sender, EventArgs eventArgs)
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                       .SetTitle((string.Format(Utility.GetLocalizedLabel("ManageAccount", "dialogManageUser"))))
                       .SetMessage(string.Format(Utility.GetLocalizedLabel("ManageAccount", "dialogManageUserMessage")))
                       .SetContentGravity(GravityFlags.Left)
                       .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                       .Build().Show();

        }

        [OnClick(Resource.Id.btnSave)]
        void OnRegister(object sender, EventArgs eventArgs)
        {
            try
            {

                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    ShowSaveDialog(this, () =>
                    {
                        this.userActionsListener.UpdateAccountAccessRight(account.UserAccountId, account.userId, checkboxfullbill, checkboxbilling, account.email);
                        //ShowSaveSuccess();
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

        [OnClick(Resource.Id.btnCancelAddAccess)]
        void OnRemoveUser(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    ShowCancelAddAccessDialog(this, () =>
                    {
                        this.userActionsListener.CancelInvitedUser(account.email, account.AccNum, account.UserAccountId);
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

        [OnClick(Resource.Id.btnResendInviteAccessUser)]
        void OnResendVerifyUser(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    ShowResendInviteAccessDialog(this, () =>
                    {
                        //this.userActionsListener.UpdateAccountAccessRight(account.UserAccountId, checkboxfullbill, checkboxbilling);
                        //ShowInviteSuccess(account.email);
                        this.userActionsListener.ResendInvitedUser(account.email, account.AccNum, account.IsHaveAccess, account.IsApplyEBilling);

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
        public override void OnBackPressed()
        {
            if ((buttonEnableview || buttonEnableEBilling) && MyTNBAccountManagement.GetInstance().IsNewUserAdd())
            {
                ShowBackDialog(this, () =>
                {
                    OnBackProceed();
                    //this.userActionsListener.UpdateAccountAccessRight(account.UserAccountId, account.userId, checkboxfullbill, checkboxbilling, account.email);
                });
            }
            else
            {
                OnBackProceed();
            }
        }

        void ShowSaveDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {
            string nickname = txtNickName.Text.ToString().Trim();
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)

                        .SetTitle(string.Format(Utility.GetLocalizedLabel("manageUser", "manageUserDialogTitle"), nickname))
                        .SetMessage(string.Format(Utility.GetLocalizedLabel("manageUser", "manageUserDialogMessage"), nickname))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "confirm"))
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
            this.SetIsClicked(false);
        }

        void ShowBackDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {
            string nickname = txtNickName.Text.ToString().Trim();
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(string.Format(Utility.GetLocalizedLabel("manageUser", "notSaveTitle"), nickname))
                        .SetMessage(string.Format(Utility.GetLocalizedLabel("manageUser", "notSaveBody"), nickname))
                        .SetContentGravity(GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("manageUser", "btnStay"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("manageUser", "btnLeave"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();

                        }).Build();
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

        void ShowResendInviteAccessDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {
            string email = txtEmail.Text.ToString().Trim();
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)

                        .SetTitle((string.Format(Utility.GetLocalizedLabel("manageUser", "manageUserInviteAccessDialogTitle"), email)))
                        .SetMessage(string.Format(Utility.GetLocalizedLabel("manageUser", "manageUserInviteAccessDialogMessage"), email))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "no"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "resend"))
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
            this.SetIsClicked(false);
        }

        void ShowCancelAddAccessDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {
            string email = txtEmail.Text.ToString().Trim();
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)

                        .SetTitle((string.Format(Utility.GetLocalizedLabel("manageUser", "manageUserCancelAddAccessDialogTitle"), email)))
                        .SetMessage(string.Format(Utility.GetLocalizedLabel("manageUser", "manageUserCancelAddAccessDialogMessage"), email))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "confirm"))
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
            this.SetIsClicked(false);
        }

        public void OnBackProceed()
        {
            SetResult(Result.Ok);
            Finish();
            base.OnBackPressed();
        }

        public void ShowSuccessCancelInvite(string email)
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("cancelInvited", email);
            SetResult(Result.Ok, resultIntent);
            Finish();
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
        }

        public void EnableSaveButton()
        {
            btnSave.Enabled = true;
            MyTNBAccountManagement.GetInstance().AddNewUserAdded(true);
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            isSelectionChange = true;
        }

        public void DisableSaveButton()
        {
            btnSave.Enabled = false;
            MyTNBAccountManagement.GetInstance().AddNewUserAdded(false);
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            isSelectionChange = false;
        }

        
        public void DisableResendButton()
        {
            btnResendInviteAccessUser.Visibility = ViewStates.Gone;
            btnResendInviteDisable.Visibility = ViewStates.Visible;
            btnResendInviteDisable.Enabled = false;
            btnResendInviteDisable.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);
        }


        public void ShowSaveSuccess()
        {
            try
            {

                Snackbar saveSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedLabel("manageUser", "saveSuccess"), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = saveSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                saveSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowInviteSuccess(string email)
        {
            try
            {
                Snackbar saveSnackBar = Snackbar.Make(rootView, (string.Format(Utility.GetLocalizedLabel("manageUser", "inviteSuccess"), email)), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = saveSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                saveSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
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

        public void ShowNickname(string nickname)
        {
            txtNickName.Text = nickname;
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
