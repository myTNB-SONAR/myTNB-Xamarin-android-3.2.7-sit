using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.RearrangeAccount.MVP
{
    [Activity(Label = "Rearrange Accounts"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Rearrange")]
    public class RearrangeAccountActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        private RearrangeAccountListView listView;

        List<CustomerBillingAccount> items = new List<CustomerBillingAccount>();

        private string PAGE_ID = "RearrangeAccount";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);
                TextViewUtils.SetTextSize16(btnSubmit);
                btnSubmit.Text = GetLabelByLanguage("btnTitle");

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
                    try
                    {
                        string env = Constants.APP_CONFIG.ENV;

                        items = AccountSortingEntity.GetRearrangeList(UserEntity.GetActive().Email, env);

                        listView.Adapter = new RearrangeAccountListAdapter(this, items);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
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
            try
            {
                RunOnUiThread(() =>
                {
                    try
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
                                GetLabelByLanguage("rearrangeToastFailMsg"),
                                Snackbar.LengthLong);
                            View v = mRearrangeSnackbar.View;
                            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                            tv.SetMaxLines(5);
                            mRearrangeSnackbar.Show();
                            this.SetIsClicked(false);
                            HideProgressDialog();
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
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

        public void HideProgressDialog()
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

        public override void OnBackPressed()
        {
            if (btnSubmit.Enabled)
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);

                    MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(GetLabelByLanguage("rearrangeTitle"))
                        .SetMessage(GetLabelByLanguage("rearrangeMsg"))
                        .SetCTALabel(GetLabelCommonByLanguage("no"))
                        .SetCTAaction(() =>
                        {
                            SetResult(Result.Canceled);
                            this.Finish();
                        })
                        .SetSecondaryCTAaction(() =>
                        {
                            OnSave();
                        })
                        .SetSecondaryCTALabel(GetLabelCommonByLanguage("yes"))
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
