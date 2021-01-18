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
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateNickname.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageUser.MVP;
using AndroidX.Core.Content;
using myTNB_Android.Src.Base;
using AndroidX.CoordinatorLayout.Widget;

namespace myTNB_Android.Src.ManageUser.Activity
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
        TextInputLayout txtInputLayoutEmail;

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

        private IMenu ManageSupplyAccountMenu;

        AccountData accountData;
        UserManageAccessAccount account;

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
                TextViewUtils.SetMuseoSans300Typeface( txtNickName );
                TextViewUtils.SetMuseoSans300Typeface(txtEmail);
                TextViewUtils.SetMuseoSans300Typeface(itemTitleFullBill, itemTitleBilling);
                TextViewUtils.SetMuseoSans500Typeface(btnSave, itemTitle);

                txtNickName.Text = account.name;
                txtEmail.Text = account.email;

                itemTitleFullBill.Text = Utility.GetLocalizedLabel("UserAccess", "fullElectricity");
                itemTitleBilling.Text = Utility.GetLocalizedLabel("UserAccess", "e_billing");
                SetToolBarTitle(GetLabelByLanguage("titleManageUser"));
                txtInputLayoutEmail.Hint = GetLabelCommonByLanguage("email_user_address").ToUpper();
                btnSave.Text = GetLabelCommonByLanguage("saveChanges");

                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));

                itemActionFullBill.CheckedChange += CheckedChange;
                itemActionBilling.CheckedChange += CheckedChanged;

                if (account.IsPreRegister)
                {
                    PopulateCheckBoxPreRegister();
                    bottomLayout_Cancel_Resend.Visibility = ViewStates.Visible;
                    bottomLayoutSave.Visibility = ViewStates.Gone;
                    view2.Visibility = ViewStates.Gone;
                    txtNameLayout.Visibility = ViewStates.Gone;
                    itemActionFullBill.Clickable = false;
                    itemActionBilling.Clickable = false;
                }
                else
                {
                    PopulateDataCheckBox();
                }

                MyTNBAccountManagement.GetInstance().AddNewUserAdded(true);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                mPresenter = new ManageUserPresenter(this, account);
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
                    itemActionBilling.SetButtonDrawable(Resource.Drawable.checkbox_disabled);
                }
                else
                {
                    itemActionBilling.Checked = false;
                    itemActionBilling.SetButtonDrawable(Resource.Drawable.checkbox_active_grey);
                }

                if (account.IsHaveAccess)
                {
                    itemActionFullBill.Checked = true;
                    itemActionFullBill.SetButtonDrawable(Resource.Drawable.checkbox_disabled);
                }
                else
                {
                    itemActionFullBill.Checked = false;
                    itemActionFullBill.SetButtonDrawable(Resource.Drawable.checkbox_active_grey);
                }
                DisableSaveButton();
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void PopulateDataCheckBox()
        {
            try
            {
                if (account.IsApplyEBilling)
                {
                    itemActionBilling.Checked = true;
                }
                else
                {
                    itemActionBilling.Checked = false;
                }

                if (account.IsHaveAccess)
                {
                    itemActionFullBill.Checked = true;

                }
                else
                {
                    itemActionFullBill.Checked = false;
                }
                DisableSaveButton();
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked)
            {
                checkboxfullbill = true;
                if(account.IsHaveAccess == checkboxfullbill)
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
                if (account.IsHaveAccess == checkboxfullbill)
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
            
            if(buttonEnableview || buttonEnableEBilling)
            {
                EnableSaveButton();
            }
        }

        private void CheckedChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                checkboxbilling = true;
                if (account.IsApplyEBilling == checkboxbilling)
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
                if (account.IsApplyEBilling == checkboxbilling)
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

            if (buttonEnableview || buttonEnableEBilling)
            {
                EnableSaveButton();
            }
        }

        [OnClick(Resource.Id.infoManageUser)]
        void OnClickManagerUserInfo(object sender, EventArgs eventArgs)
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                       .SetTitle((string.Format(GetLabelByLanguage("dialogManageUser"))))
                       .SetMessage(string.Format(GetLabelByLanguage("dialogManageUserMessage")))
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
                        this.userActionsListener.UpdateAccountAccessRight(account.UserAccountId, checkboxfullbill, checkboxbilling);
                        ShowSaveSuccess();
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
                        //this.userActionsListener.UpdateAccountAccessRight(account.UserAccountId, checkboxfullbill, checkboxbilling);
                        ShowCancelAddSuccess();
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
                        ShowInviteSuccess();
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
                    this.userActionsListener.UpdateAccountAccessRight(account.UserAccountId, checkboxfullbill, checkboxbilling);
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
                        
                        .SetTitle((string.Format(GetLabelByLanguage("manageUserDialogTitle"), nickname)))
                        .SetMessage(string.Format(GetLabelByLanguage("manageUserDialogMessage"),nickname))
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
                        .SetTitle((string.Format(GetLabelByLanguage("manageUserBackTitle"), nickname)))
                       .SetMessage(string.Format(GetLabelByLanguage("manageUserBackMessage"), nickname))
                        .SetContentGravity(GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "confirm"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();

                        }).Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                tooltipBuilder.DismissDialog();
                OnBackProceed();
            }).Show();
        }

        void ShowResendInviteAccessDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {
            string email = txtEmail.Text.ToString().Trim();
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)

                        .SetTitle((string.Format(GetLabelByLanguage("manageUserInviteAccessDialogTitle"), email)))
                        .SetMessage(string.Format(GetLabelByLanguage("manageUserInviteAccessDialogMessage"), email))
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

                        .SetTitle((string.Format(GetLabelByLanguage("manageUserCancelAddAccessDialogTitle"), email)))
                        .SetMessage(string.Format(GetLabelByLanguage("manageUserCancelAddAccessDialogMessage"), email))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "no"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "yes"))
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

        public void EnableSaveButton()
        {
            btnSave.Enabled = true;
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            isSelectionChange = true;
        }

        public void DisableSaveButton()
        {
            btnSave.Enabled = false;
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            isSelectionChange = false;
        }

        public void ShowSaveSuccess()
        {
            try
            {
               
                Snackbar saveSnackBar = Snackbar.Make(rootView, GetLabelByLanguage("saveSuccess"), Snackbar.LengthIndefinite)
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

        public void ShowInviteSuccess()
        {
            try
            {
                string email = txtEmail.Text.ToString().Trim();
                Snackbar saveSnackBar = Snackbar.Make(rootView, (string.Format(GetLabelByLanguage("inviteSuccess"), email)), Snackbar.LengthIndefinite)
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

        public void ShowCancelAddSuccess()
        {
            try
            {
                string email = txtEmail.Text.ToString().Trim();
                Snackbar saveSnackBar = Snackbar.Make(rootView, (string.Format(GetLabelByLanguage("cancelAdddSuccess"), email)), Snackbar.LengthIndefinite)
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
