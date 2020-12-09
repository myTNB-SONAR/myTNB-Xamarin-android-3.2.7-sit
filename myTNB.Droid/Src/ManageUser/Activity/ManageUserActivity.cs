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
using myTNB_Android.Src.ManageSupplyAccount.MVP;
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
using myTNB_Android.Src.ManageUser.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageUser.MVP;
using AndroidX.Core.Content;

namespace myTNB_Android.Src.ManageUser.Activity
{
    [Activity(Label = "@string/manage_supply_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class ManageUserActivity : BaseActivityCustom, ManageUserContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

       /* [BindView(Resource.Id.txtManageUserTitle)]
        TextView txtManageUserTitle;*/

        /*[BindView(Resource.Id.txtInputLayoutNickName)]
        TextInputLayout txtInputLayoutNickName;*/

        [BindView(Resource.Id.txtNickName)]
        EditText txtNickName;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.profileMenuItemsContent)]
        LinearLayout profileMenuItemsContent;

        [BindView(Resource.Id.btnSave)]
        Button btnSave;

        [BindView(Resource.Id.infoManageUser)]
        TextView infoManageUser;

        private IMenu ManageSupplyAccountMenu;
        AccountData accountData;
       // private CustomerBillingAccount account;
        //UserEntity user;
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

           
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail);
                TextViewUtils.SetMuseoSans300Typeface( txtNickName );
                TextViewUtils.SetMuseoSans300Typeface(txtEmail);
                TextViewUtils.SetMuseoSans500Typeface(btnSave);

                txtNickName.Text = accountData.AccountNickName;
                txtEmail.Text = accountData.AddStreet;

                //txtEmail.Text = accountData.AddStreet;
                //txtNickName.Text = accountData.AccountNickName;
                SetToolBarTitle(GetLabelByLanguage("titleManageUser"));
                txtInputLayoutEmail.Hint = GetLabelCommonByLanguage("email_user_address").ToUpper();
                btnSave.Text = GetLabelCommonByLanguage("saveChanges");

                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));

                ManageSupplyItemComponent ManageUserItem = GetManageSupply();
                ManageUserItem.SetHeaderTitle(GetLabelCommonByLanguage("ManageUserTitle"));
                profileMenuItemsContent.AddView(ManageUserItem);

                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                mPresenter = new ManageUserPresenter(this, accountData);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        
       

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ManageSupplyAccountToolbarMenu, menu);
            ManageSupplyAccountMenu = menu;
            ManageSupplyAccountMenu.FindItem(Resource.Id.icon_log_activity_unread).SetIcon(GetDrawable(Resource.Drawable.icon_activity_log)).SetVisible(true);

/*            ManageSupplyAccountMenu.FindItem(Resource.Id.action_notification_edit_delete).SetVisible(false);
            ManageSupplyAccountMenu.FindItem(Resource.Id.action_notification_read).SetVisible(false);


            ManageSupplyAccountMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(GetDrawable(Resource.Drawable.notification_select_all)).SetVisible(false);

            ManageSupplyAccountMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(GetDrawable(Resource.Drawable.notification_select_all)).SetVisible(true);*/

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.icon_log_activity_unread:
                    //ManageSupplyAccountMenu.FindItem(Resource.Id.action_notification_read).SetIcon(Resource.Drawable.ic_header_markread_disabled).SetVisible(true).SetEnabled(false);
                    //ManageSupplyAccountMenu.FindItem(Resource.Id.action_notification_edit_delete).SetIcon(Resource.Drawable.notification_delete_disabled).SetVisible(true).SetEnabled(false);
                    break;                
            }


            return base.OnOptionsItemSelected(item);
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

        private ManageSupplyItemComponent GetManageSupply()
        {
            //Context context = Activity.ApplicationContext;

            ManageSupplyItemComponent manageItem = new ManageSupplyItemComponent(this);
            
            List<View> manageItems = new List<View>();

            ManageUserMenuItemSingleContentComponent viewBill = new ManageUserMenuItemSingleContentComponent(this);
            viewBill.SetTitle(GetLabelCommonByLanguage("viewFullBill"));
            viewBill.SetItemActionVisibility(true);
            viewBill.SetItemActionCall(ClickCheck);
            manageItems.Add(viewBill);

         

            ManageUserMenuItemSingleContentComponent applyBill = new ManageUserMenuItemSingleContentComponent(this);
            applyBill.SetTitle(GetLabelCommonByLanguage("ApplyforBilling"));
            applyBill.SetItemActionVisibility(true);
            applyBill.SetItemActionCall(ClickCheck);
            manageItems.Add(applyBill);

            manageItem.AddComponentView(manageItems);
            return manageItem;
        }

        private void ClickCheck()
        {
            isSelectionChange = true;
            if (!this.GetIsClicked())
            {
                EnableSaveButton();
            }
            DisableSaveButton();
        }

       
        [OnClick(Resource.Id.infoManageUser)]
        void OnClickManagerUserInfo(object sender, EventArgs eventArgs)
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                       .SetTitle((string.Format(GetLabelByLanguage("dialogManageUser"))))
                       .SetMessage(string.Format(GetLabelByLanguage("dialogManageUserMessage")))
                       .SetContentGravity(GravityFlags.Center)
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
                   /* string idtype = selectedIdentificationType.Id;
                    string ic_no = txtICNumber.Text.ToString().Trim();*/
                    ShowSaveDialog(this, () =>
                    {
                        // _ = RunUpdateID(idtype,ic_no);
                        //ShowProgress();
                        ShowSaveSuccess();


                    });
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            if (isSelectionChange)
            {
                ShowBackDialog(this, () =>
                {
                    
                    ShowProgress();
                    
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
                        
                        .SetTitle((string.Format(GetLabelByLanguage("manageUserTitle"), nickname)))
                        .SetMessage(string.Format(GetLabelByLanguage("manageUserMessage"),nickname))
                        .SetContentGravity(Android.Views.GravityFlags.Center)
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
                        .SetTitle((string.Format(GetLabelByLanguage("dialogManageUser"), nickname)))
                       .SetMessage(string.Format(GetLabelByLanguage("dialogManageUserMessage"), nickname))
                        .SetContentGravity(GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "confirm"))
                        .SetSecondaryCTAaction(() =>
                        {
                            ShowProgress();
                           
                        }).Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                tooltipBuilder.DismissDialog();
                OnBackProceed();
            }).Show();

            /* string nickname = txtNickName.Text.ToString().Trim();
             MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                         //.SetTitle(Utility.GetLocalizedLabel("ManageAccount", "manageUserTitle"))
                         .SetTitle((string.Format(GetLabelByLanguage("manageUserBackTitle"), nickname)))
                         //.SetSubTitle(nickname)
                         .SetMessage(string.Format(GetLabelByLanguage("manageUserBackMessage"), nickname))
                         .SetContentGravity(Android.Views.GravityFlags.Center)
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
             this.SetIsClicked(false);*/
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

        private void ShowSaveSuccess()
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
