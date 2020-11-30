﻿using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Maintenance.Activity;
using myTNB_Android.Src.NotificationSettings.Adapter;
using myTNB_Android.Src.NotificationSettings.MVP;
using myTNB_Android.Src.SelectNotification.Models;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime;
using System.Threading.Tasks;

namespace myTNB_Android.Src.NotificationSettings.Activity
{
    [Activity(Label = "@string/notification_settings_activity_title"
      //, MainLauncher = true
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationSettingsActivity : BaseActivityCustom, NotificationSettingsContract.IView
    {

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtNotificationTypeTitle)]
        TextView txtNotificationTypeTitle;

        [BindView(Resource.Id.txtNotificationChannelTitle)]
        TextView txtNotificationChannelTitle;

        [BindView(Resource.Id.notificationTypeRecyclerView)]
        RecyclerView notificationTypeRecyclerView;

        NotificationTypeAdapter typeAdapter;

        [BindView(Resource.Id.notificationChannelRecyclerView)]
        RecyclerView notificationChannelRecyclerView;

        [BindView(Resource.Id.language_list_view)]
        ListView languageListView;

        [BindView(Resource.Id.appLanguageMessage)]
        TextView appLanguageMessage;

        private SelectItemAdapter selectItemAdapter;
        private List<Item> languageItemList;
        private string savedLanguage;
        private bool isSelectionChange;

        NotificationChannelAdapter channelAdapter;

        NotificationSettingsContract.IUserActionsListener userActionsListener;
        NotificationSettingsPresenter mPresenter;

        LinearLayoutManager notificationChannelLayoutManager, notificationTypeLayoutManager;


        MaterialDialog progressUpdateType, progressUpdateChannel;

        const string PAGE_ID = "NotificationSettings";

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.NotificationSettingsView;
        }

        public void SetPresenter(NotificationSettingsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        private void SetSelectedLanguage(string languageTitle)
        {
            if (languageTitle == null) //If lang param is null, use saved language as selected
            {
                languageTitle = GetLanguageLabelByCode(savedLanguage);
            }
            languageItemList.ForEach(item =>
            {
                item.selected = (languageTitle == item.title) ? true : false;
            });
            selectItemAdapter.NotifyDataSetChanged();
            isSelectionCheck();
        }

        private string GetLanguageLabelByCode(string lang)
        {
            string langLabel;
            switch (lang)
            {
                case "EN":
                    {
                        langLabel = "English";
                        break;
                    }
                case "MS":
                    {
                        langLabel = "Bahasa Malaysia";
                        break;
                    }
                default:
                    {
                        langLabel = "";
                        break;
                    }
            }
            return langLabel;
        }

        private void UpdateLabels()
        {
            SetToolBarTitle(GetLabelCommonByLanguage("appSetting"));
            txtNotificationTypeTitle.Text = GetLabelByLanguage("typeDescription");
            appLanguageMessage.Text = GetLabelCommonByLanguage("setAppLanguageDescription");
        }

        private void UpdateTypesList()
        {
            typeAdapter.ClearAll();
            mPresenter.OnNotification(this.DeviceId());
        }

        public void ShowNotificationChannelList(List<NotificationChannelUserPreference> channelPreferenceList)
        {
            channelAdapter.AddAll(channelPreferenceList);
            //notificationChannelListView.SetNoScroll();
        }

        public void ShowNotificationTypesList(List<NotificationTypeUserPreference> typePreferenceList)
        {
            typeAdapter.AddAll(typePreferenceList);
            //notificationTypeListView.SetNoScroll();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                // Create your application here
                Console.WriteLine("NotificationSettingsActivity OnCreate");

                TextViewUtils.SetMuseoSans500Typeface(txtNotificationTypeTitle, txtNotificationChannelTitle, appLanguageMessage);

                txtNotificationTypeTitle.Text = GetLabelByLanguage("typeDescription");
                //txtNotificationChannelTitle.Text = GetLabelByLanguage("modeDescription");

                notificationChannelLayoutManager = new LinearLayoutManager(this);
                notificationTypeLayoutManager = new LinearLayoutManager(this);
                notificationTypeRecyclerView.SetLayoutManager(notificationTypeLayoutManager);
                notificationChannelRecyclerView.SetLayoutManager(notificationChannelLayoutManager);

                savedLanguage = LanguageUtil.GetAppLanguage();
                languageItemList = new List<Item>();
                isSelectionChange = false;

                foreach (string languageName in Enum.GetNames(typeof(Constants.SUPPORTED_LANGUAGES)))
                {
                    Item item = new Item();
                    item.title = GetLanguageLabelByCode(languageName);
                    item.type = languageName;
                    item.selected = false;
                    languageItemList.Add(item);
                }

                languageListView.ItemClick += OnItemClick;

                selectItemAdapter = new SelectItemAdapter(this, languageItemList);
                languageListView.Adapter = selectItemAdapter;

                typeAdapter = new NotificationTypeAdapter(true);
                typeAdapter.ClickEvent += TypeAdapter_ClickEvent;
                notificationTypeRecyclerView.SetAdapter(typeAdapter);
                notificationTypeRecyclerView.NestedScrollingEnabled = (false);

                //notificationTypeListView.SetNoScroll();

                /*channelAdapter = new NotificationChannelAdapter(true);
                channelAdapter.ClickEvent += ChannelAdapter_ClickEvent;
                notificationChannelRecyclerView.SetAdapter(channelAdapter);
                notificationTypeRecyclerView.NestedScrollingEnabled = (false);*/
                //notificationChannelListView.SetNoScroll();

                UpdateLabels();
                SetSelectedLanguage(null);
                mPresenter = new NotificationSettingsPresenter(this);
                this.userActionsListener.Start();
                bool hasUpdateLanguage = MyTNBAccountManagement.GetInstance().IsUpdateLanguage();
                if (hasUpdateLanguage)
                {
                    ShowLanguageUpdateSuccess();
                    MyTNBAccountManagement.GetInstance().SetIsUpdateLanguage(false);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

        }
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            
            try
            {
                Item selectedItem = selectItemAdapter.GetItemObject(e.Position);
                SetSelectedLanguage(selectedItem.title);
                //Item selectedItem = languageItemList.Find(item => { return item.selected; });

                if (selectedItem.title != GetLanguageLabelByCode(savedLanguage))
                {
                    //Item selectedItem = languageItemList.Find(item => { return item.selected; });
                    string currentLanguage = LanguageUtil.GetAppLanguage();
                    Utility.ShowChangeLanguageDialog(this, currentLanguage, () =>
                    {
                        ShowProgressDialog();
                        _ = RunUpdateLanguage(selectedItem);
                    });
                }

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private async void UpdateLanguage()
        {
            savedLanguage = LanguageUtil.GetAppLanguage();
            await LanguageUtil.SaveUpdatedLanguagePreference();
            UpdateLabels();
            isSelectionCheck();
        }

        public void isSelectionCheck()
        {
            try
            {
                Item selectedItem = languageItemList.Find(item => { return item.selected; });

                if (selectedItem.title != GetLanguageLabelByCode(savedLanguage))
                {
                    isSelectionChange = true;
                }
                else
                {
                    isSelectionChange = false;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            if (isSelectionChange)
            {
                ShowTooltipConfirm();
            }
            else
            {
                OnBackProceed();
            }
        }

        private void ShowTooltipConfirm()
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Common", "changeLanguageTitle_" + savedLanguage))
                        .SetMessage(Utility.GetLocalizedLabel("Common", "saveLanguageMessage"))
                        .SetContentGravity(GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageNo_" + savedLanguage))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageYes_" + savedLanguage))
                        .SetSecondaryCTAaction(() =>
                        {
                            ShowProgressDialog();
                            Item selectedItem = languageItemList.Find(item => { return item.selected; });
                            _ = RunUpdateLanguage(selectedItem);
                        }).Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                tooltipBuilder.DismissDialog();
                OnBackProceed();
            }).Show();
        }

        public void OnBackProceed()
        {
            SetResult(Result.Ok);
            Finish();
            base.OnBackPressed();
        }

        private Task RunUpdateLanguage(Item selectedItem)
        {
            return Task.Run(() =>
            {
                LanguageUtil.SaveAppLanguage(selectedItem.type);
                MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                _ = CheckAppMasterDataDone();
            });
        }

        private Task CheckAppMasterDataDone()
        {
            return Task.Delay(Constants.LANGUAGE_MASTER_DATA_CHECK_TIMEOUT).ContinueWith(_ => {
                if (MyTNBAccountManagement.GetInstance().GetIsAppMasterComplete())
                {
                    if (MyTNBAccountManagement.GetInstance().GetIsAppMasterFailed())
                    {
                        MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                        _ = CheckAppMasterDataDone();
                    }
                    else if (MyTNBAccountManagement.GetInstance().GetIsAppMasterMaintenance())
                    {
                        try
                        {
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    MyTNBAccountManagement.GetInstance().ClearSitecoreItem();
                                    MyTNBAccountManagement.GetInstance().ClearAppCacheItem();
                                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                                    HomeMenuUtils.ResetAll();
                                    SMRPopUpUtils.SetSSMRMeterReadingRefreshNeeded(true);
                                    SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                                    UpdateLanguage();
                                    UpdateTypesList();
                                    MyTNBAccountManagement.GetInstance().SetIsUpdateLanguage(false);
                                    OnMaintenanceProceed();
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
                    else
                    {
                        try
                        {
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    MyTNBAccountManagement.GetInstance().ClearSitecoreItem();
                                    MyTNBAccountManagement.GetInstance().ClearAppCacheItem();
                                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                                    HomeMenuUtils.ResetAll();
                                    SMRPopUpUtils.SetSSMRMeterReadingRefreshNeeded(true);
                                    SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                                    MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                                    UpdateLanguage();
                                    MyTNBAccountManagement.GetInstance().SetIsUpdateLanguage(true);
                                    UpdateTypesList();                                    
                                    ShowLanguageUpdateSuccess();
                                    HideShowProgressDialog();
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
                }
                else
                {
                    _ = CheckAppMasterDataDone();
                }
            });
        }

        private void ChannelAdapter_ClickEvent(object sender, int e)
        {
            try
            {
                NotificationChannelUserPreference userPreference = channelAdapter.GetItemObject(e);
                this.userActionsListener.OnChannelItemClick(userPreference, e);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TypeAdapter_ClickEvent(object sender, int e)
        {
            try
            {
                NotificationTypeUserPreference userPreference = typeAdapter.GetItemObject(e);
                this.userActionsListener.OnTypeItemClick(userPreference, e, this.DeviceId());
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private Snackbar mLanguageSnackbar;
        private void ShowLanguageUpdateSuccess()
        {
            try
            {
                if (mLanguageSnackbar != null && mLanguageSnackbar.IsShown)
                {
                    mLanguageSnackbar.Dismiss();
                }

                mLanguageSnackbar = Snackbar.Make(rootView,
                    GetLabelByLanguage("changeLanguageSuccess"),
                    Snackbar.LengthIndefinite)
                            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = mLanguageSnackbar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mLanguageSnackbar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Notification Prefences");
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

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_settings_cancelled_exception_btn_close), delegate {

               mCancelledExceptionSnackBar.Dismiss();
            }
            );
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_settings_api_exception_btn_close), delegate {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            mApiExcecptionSnackBar.Show();
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_settings_unknown_exception_btn_close), delegate {

                mUknownExceptionSnackBar.Dismiss();

           }
            );
            mUknownExceptionSnackBar.Show();
        }

        public void ShowSuccessUpdatedNotificationType(NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
        }

        public void ShowSuccessUpdatedNotificationChannel(NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
        }

        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException, NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();
        }

        public void ShowRetryOptionsApiException(ApiException apiException, NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
        }

        public void ShowRetryOptionsUnknownException(Exception exception, NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
        }

        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException, NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();
        }

        public void ShowRetryOptionsApiException(ApiException apiException, NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
        }

        public void ShowRetryOptionsUnknownException(Exception exception, NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
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

        private void OnMaintenanceProceed()
        {
            HideShowProgressDialog();
            Intent maintenanceScreen = new Intent(this, typeof(MaintenanceActivity));
            maintenanceScreen.PutExtra(Constants.MAINTENANCE_TITLE_KEY, MyTNBAccountManagement.GetInstance().GetMaintenanceTitle());
            maintenanceScreen.PutExtra(Constants.MAINTENANCE_MESSAGE_KEY, MyTNBAccountManagement.GetInstance().GetMaintenanceContent());
            StartActivity(maintenanceScreen);
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
