using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.More.PushNotificationSettings;
using System.Collections.Generic;
using myTNB.Model;
using System.Threading.Tasks;
using System.Linq;


namespace myTNB
{
    public partial class NotificationSettingsViewController : UIViewController
    {
        public NotificationSettingsViewController(IntPtr handle) : base(handle)
        {
        }

        internal List<string> NotificationSettingsTitle = new List<string>
        {
            "Select the type of notifications you wish to receive from TNB",
            "Select how you wish to receive your notifications"
        };

        NotificationPreferenceUpdateResponseModel _notificationPreferenceUpdate = new NotificationPreferenceUpdateResponseModel();

        internal List<NotificationPreferenceModel> SelectedNotificationTypeList = new List<NotificationPreferenceModel>();
        internal List<NotificationPreferenceModel> SelectedNotificationChannelList = new List<NotificationPreferenceModel>();

        internal List<NotificationPreferenceUpdateResponseModel> _notificationPreferenceUpdateList = new List<NotificationPreferenceUpdateResponseModel>();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
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

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Notifications Settings");
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void SetSubViews()
        {
            notificationSettingsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64, View.Frame.Width, View.Frame.Height - 64);
            notificationSettingsTableView.RowHeight = 54f;
            notificationSettingsTableView.SectionHeaderHeight = 66f;
            notificationSettingsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            notificationSettingsTableView.BackgroundColor = myTNBColor.SectionGrey();
        }

        internal void DisplayAlertMessage(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
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
                                   || _notificationPreferenceUpdate.d.isError.ToLower() == "true"
                                    || _notificationPreferenceUpdate.d.status.ToLower() != "success")
                                {
                                    DisplayAlertMessage("Error", "Unable to update your preferences. Please try later");
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
                        Console.WriteLine("No Network");
                        DisplayAlertMessage("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate());
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        internal Task SaveUserNotificationPreference(bool isNotificationType, NotificationPreferenceModel preference)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    id = preference.Id,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    deviceId = DataManager.DataManager.SharedInstance.UDID,
                    channelTypeId = preference.MasterId,
                    notificationTypeId = preference.MasterId,
                    isOpted = preference.IsOpted
                };
                string suffix = isNotificationType ? "SaveUserNotificationTypePreference" : "SaveUserNotificationChannelPreference";
                _notificationPreferenceUpdate = serviceManager.SaveUserNotificationPreference(suffix, requestParameter);
            });
        }
    }
}