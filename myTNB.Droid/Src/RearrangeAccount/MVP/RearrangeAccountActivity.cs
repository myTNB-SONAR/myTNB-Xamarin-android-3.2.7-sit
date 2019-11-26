using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.RearrangeAccount.MVP
{
    [Activity(Label = "Rearrange Accounts"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Rearrange")]
    public class RearrangeAccountActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        private RearrangeAccountListView listView;

        List<CustomerBillingAccount> items = new List<CustomerBillingAccount>();

        private LoadingOverlay loadingOverlay;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);

                listView = FindViewById<RearrangeAccountListView>(Resource.Id.list_view);

                DisableSaveButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                RunOnUiThread(() =>
                {
                    string env = Constants.APP_CONFIG.ENV;

                    items = AccountSortingEntity.GetRearrangeList(UserEntity.GetActive().Email, env);

                    listView.Adapter = new RearrangeAccountListAdapter(this, items);
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.AccountRearrangeLayout;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Rearrange Accounts");
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

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                OnSave();
            }
        }

        private Snackbar mRearrangeSnackbar;
        private void OnSave()
        {
            RunOnUiThread(() =>
            {
                ShowProgressDialog();
                List<CustomerBillingAccount> sortedList = ((RearrangeAccountListAdapter)listView.Adapter).Items;

                int rowChange = AccountSortingEntity.InsertOrReplace(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV, sortedList);
                if (rowChange != 0)
                {
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();
                    SetResult(Result.Ok);
                    Finish();
                    HideProgressDialog();
                }
                else
                {
                    if (mRearrangeSnackbar != null && mRearrangeSnackbar.IsShown)
                    {
                        mRearrangeSnackbar.Dismiss();
                    }

                    mRearrangeSnackbar = Snackbar.Make(rootView,
                        "Uh oh, your changes were not saved. Please try again.",
                        Snackbar.LengthLong);
                    mRearrangeSnackbar.Show();
                    this.SetIsClicked(false);
                    HideProgressDialog();
                }
            });
        }

        public void ShowProgressDialog()
        {
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

        public void HideProgressDialog()
        {
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

        public override void OnBackPressed()
        {
            if (btnSubmit.Enabled)
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);

                    MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle("Leave without saving changes?")
                        .SetMessage("Looks like you’ve rearranged your accounts. Would you like to keep this new arrangement?")
                        .SetCTALabel("No")
                        .SetCTAaction(() => {
                            SetResult(Result.Canceled);
                            this.Finish();
                        })
                        .SetSecondaryCTAaction(() =>
                        {
                            OnSave();
                        })
                        .SetSecondaryCTALabel("Yes")
                        .Build().Show();
                }
            }
            else
            {
                SetResult(Result.Canceled);
                this.Finish();
            }
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

        public void EnableSaveButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableSaveButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }


    }
}