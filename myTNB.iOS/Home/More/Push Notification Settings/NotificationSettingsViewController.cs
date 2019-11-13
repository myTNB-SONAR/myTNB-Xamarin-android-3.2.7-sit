using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.More.PushNotificationSettings;
using System.Collections.Generic;
using myTNB.Model;
using System.Threading.Tasks;
using myTNB.Profile;

namespace myTNB
{
    public partial class NotificationSettingsViewController : CustomUIViewController
    {
        public NotificationSettingsViewController(IntPtr handle) : base(handle) { }

        private List<string> NotificationSettingsTitle;

        private NotificationPreferenceUpdateResponseModel _notificationPreferenceUpdate = new NotificationPreferenceUpdateResponseModel();

        internal List<NotificationPreferenceModel> SelectedNotificationTypeList = new List<NotificationPreferenceModel>();
        internal List<NotificationPreferenceModel> SelectedNotificationChannelList = new List<NotificationPreferenceModel>();

        internal List<NotificationPreferenceUpdateResponseModel> _notificationPreferenceUpdateList = new List<NotificationPreferenceUpdateResponseModel>();

        public override void ViewDidLoad()
        {
            PageName = ProfileConstants.Pagename_NotificationSettings;
            base.ViewDidLoad();
            NotificationSettingsTitle = new List<string>
            {
               GetI18NValue(ProfileConstants.I18N_TypeDescription),
               GetI18NValue(ProfileConstants.I18N_ModeDescription)
            };
            SetNavigationBar();
            SetSubViews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var rawItems = DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.data;
            if (rawItems != null && rawItems?.Count > 0)
            {
                SelectedNotificationTypeList = rawItems.FindAll(item => item?.ShowInPreference?.ToLower() == "true");
                SelectedNotificationChannelList = DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.data;
            }

            notificationSettingsTableView.Source = new NotificationSettingsDataSource(this, NotificationSettingsTitle, SelectedNotificationTypeList);
            notificationSettingsTableView.ReloadData();
        }

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(ProfileConstants.I18N_NavTitle));
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        private void SetSubViews()
        {
            notificationSettingsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution()
                ? 88 : 64, View.Frame.Width, View.Frame.Height - 64);
            notificationSettingsTableView.RowHeight = 54f;
            notificationSettingsTableView.SectionHeaderHeight = 66f;
            notificationSettingsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            notificationSettingsTableView.BackgroundColor = MyTNBColor.SectionGrey;
        }

        internal void ExecuteSaveUserNotificationPreferenceCall(bool isNotificationType, NotificationPreferenceModel preference)
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        SaveUserNotificationPreference(isNotificationType, preference).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_notificationPreferenceUpdate == null
                                   || _notificationPreferenceUpdate.d == null
                                   || !_notificationPreferenceUpdate.d.IsSuccess)
                                {
                                    DisplayServiceError(_notificationPreferenceUpdate?.d?.ErrorMessage ?? string.Empty);
                                }
                                else
                                {
                                    PushNotificationHelper.GetUserNotificationPreferences();
                                    SelectedNotificationTypeList = DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.data;
                                    SelectedNotificationChannelList = DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.data;
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private Task SaveUserNotificationPreference(bool isNotificationType, NotificationPreferenceModel preference)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    serviceManager.deviceInf,
                    id = preference.Id,
                    channelTypeId = preference.MasterId,
                    notificationTypeId = preference.MasterId,
                    isOpted = preference.IsOpted
                };
                string suffix = isNotificationType ? ProfileConstants.Service_SaveNotificationType : ProfileConstants.Service_SaveNotificationChannel;
                _notificationPreferenceUpdate = serviceManager.OnExecuteAPIV6<NotificationPreferenceUpdateResponseModel>(suffix, requestParameter);
            });
        }
    }
}