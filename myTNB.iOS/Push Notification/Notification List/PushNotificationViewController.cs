using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using UIKit;
using myTNB.Extensions;

namespace myTNB.PushNotification
{
    public partial class PushNotificationViewController : UIViewController
    {
        public PushNotificationViewController(IntPtr handle) : base(handle)
        {
        }

        AccountSelectionComponent _notificationSelectionComponent;
        NotificationDetailedInfoResponseModel _detailedInfo = new NotificationDetailedInfoResponseModel();

        UIView _viewDelete;
        UIImageView _imgNoNotification;
        UILabel _lblNoNotification;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            SetSubViews();
        }

        private void UpdateNotificationDisplay()
        {
            string filterID = "all";
            if (DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex < DataManager.DataManager.SharedInstance.NotificationGeneralTypes?.Count)
            {
                _notificationSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance.NotificationGeneralTypes[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Title);
                filterID = DataManager.DataManager.SharedInstance.NotificationGeneralTypes[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Id;
            }


            List<UserNotificationDataModel> notifications = new List<UserNotificationDataModel>();
            if (DataManager.DataManager.SharedInstance.UserNotifications.Count > 0)
            {
                var format = @"M/d/yyyy h:mm:ss tt";
                if (filterID == "all")
                {
                    notifications = DataManager.DataManager.SharedInstance.UserNotifications
                    .Where(x => x.IsDeleted.ToLower() == "false").OrderByDescending(o => DateTime.ParseExact(o.CreatedDate, format, System.Globalization.CultureInfo.InvariantCulture)).ThenBy(t => t.Title).ToList();
                }
                else
                {
                    notifications = DataManager.DataManager.SharedInstance.UserNotifications
                    .Where(x => x.IsDeleted.ToLower() == "false" && x.NotificationTypeId == filterID).OrderByDescending(o => DateTime.ParseExact(o.CreatedDate, format, System.Globalization.CultureInfo.InvariantCulture)).ThenBy(t => t.Title).ToList();
                }
                if (_imgNoNotification != null && _lblNoNotification != null)
                {
                    _imgNoNotification.Hidden = true;
                    _lblNoNotification.Hidden = true;
                }
                pushNotificationTableView.Hidden = false;
                pushNotificationTableView.ClearsContextBeforeDrawing = true;
                pushNotificationTableView.Source = new PushNotificationDataSource(this, notifications);
                pushNotificationTableView.ReloadData();
            }
            else
            {
                if (_imgNoNotification == null || _lblNoNotification == null)
                {
                    _imgNoNotification = new UIImageView(new CGRect((View.Frame.Width / 2) - 75, 185, 150, 150));
                    _imgNoNotification.Image = UIImage.FromBundle("Notification-Empty");
                    _lblNoNotification = new UILabel(new CGRect(44, 352, View.Frame.Width - 88, 16));
                    _lblNoNotification.TextAlignment = UITextAlignment.Center;
                    _lblNoNotification.Text = "No notifications yet.";
                    _lblNoNotification.Font = myTNBFont.MuseoSans12();
                    _lblNoNotification.TextColor = myTNBColor.SilverChalice();
                    View.AddSubviews(new UIView[] { _imgNoNotification, _lblNoNotification });
                }
                pushNotificationTableView.Hidden = true;
                _imgNoNotification.Hidden = false;
                _lblNoNotification.Hidden = false;
            }
        }
        /// <summary>
        /// Gets the user notif.
        /// </summary>
        //private async void GetUserNotif()
        //{
        //    ActivityIndicator.Show();
        //    await PushNotificationHelper.GetNotifications();
        //    UpdateNotificationDisplay();
        //    ActivityIndicator.Hide();
        //}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (_viewDelete == null)
            {
                InitializeDeleteSuccessView();
            }
            if (DataManager.DataManager.SharedInstance.IsNotificationDeleted)
            {
                ShowDeleteNotification();
            }
            if (DataManager.DataManager.SharedInstance.NotificationNeedsUpdate)
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(async () =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ActivityIndicator.Show();
                            await PushNotificationHelper.GetNotifications();
                            UpdateNotificationDisplay();
                            ActivityIndicator.Hide();
                        }
                        else
                        {
                            var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            PresentViewController(alert, animated: true, completionHandler: null);
                        }
                    });
                });
            }
            else
            {
                UpdateNotificationDisplay();
            }
        }



        internal void SetNavigationBar()
        {
            NavigationController.SetNavigationBarHidden(true, false);
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 89, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Notifications");
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);

            _notificationSelectionComponent = new AccountSelectionComponent(headerView);
            UIView accountSelectionView = _notificationSelectionComponent.GetUI();

            if (DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex < DataManager.DataManager.SharedInstance.NotificationGeneralTypes?.Count)
            {
                _notificationSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance.NotificationGeneralTypes[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Title);
            }
            _notificationSelectionComponent.SetSelectAccountEvent(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                SelectNotificationViewController viewController =
                    storyBoard.InstantiateViewController("SelectNotificationViewController") as SelectNotificationViewController;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }));
            headerView.AddSubview(accountSelectionView);

            View.AddSubview(headerView);
        }

        internal void SetSubViews()
        {
            pushNotificationTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 113 : 89, View.Frame.Width, View.Frame.Height - 89);
            pushNotificationTableView.RowHeight = 66f;
            pushNotificationTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        internal void ExecuteGetNotificationDetailedInfoCall(UserNotificationDataModel dataModel)
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        GetNotificationDetailedInfo(dataModel).ContinueWith(task =>
                        {
                            InvokeOnMainThread(async () =>
                            {
                                if (_detailedInfo != null && _detailedInfo?.d != null
                                    && _detailedInfo?.d?.didSucceed == true
                                   && _detailedInfo?.d?.status.ToLower() == "success"
                                   && _detailedInfo?.d?.data != null)
                                {
                                    DataManager.DataManager.SharedInstance.NotificationNeedsUpdate = false;
                                    UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                                    NotificationDetailsViewController viewController =
                                        storyBoard.InstantiateViewController("NotificationDetailsViewController") as NotificationDetailsViewController;
                                    var notificationTitle = string.Empty;
                                    var notifResult = DataManager.DataManager.SharedInstance.UserNotifications.Where(x => x.Id == dataModel.Id).ToList();
                                    if (notifResult.Count > 0)
                                    {
                                        notificationTitle = notifResult[0]?.Title;
                                    }
                                    _detailedInfo.d.data.NotificationTitle = notificationTitle;
                                    viewController.NotificationInfo = _detailedInfo?.d?.data;
                                    NavigationController?.PushViewController(viewController, true);
                                }
                                ActivityIndicator.Hide();
                                await PushNotificationHelper.GetNotifications();
                                UpdateNotificationDisplay();
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

        internal Task GetNotificationDetailedInfo(UserNotificationDataModel dataModel)
        {
            var emailAddress = string.Empty;
            var userId = string.Empty;
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email;
                userId = DataManager.DataManager.SharedInstance.UserEntity[0]?.userID;
            }

            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    ApiKeyID = TNBGlobal.API_KEY_ID,
                    NotificationId = dataModel.Id,
                    NotificationType = dataModel.NotificationType,
                    Email = emailAddress,
                    DeviceId = DataManager.DataManager.SharedInstance.UDID,
                    SSPUserId = userId
                };
                _detailedInfo = serviceManager.GetNotificationDetailedInfo("GetNotificationDetailedInfo_V2", requestParameter);
            });
        }

        internal void DisplayAlertMessage(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        internal void InitializeDeleteSuccessView()
        {
            _viewDelete = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 48));
            _viewDelete.BackgroundColor = myTNBColor.SunGlow();
            _viewDelete.Layer.CornerRadius = 2.0f;
            _viewDelete.Hidden = true;

            UILabel lblDeleteDetails = new UILabel(new CGRect(16, 16, _viewDelete.Frame.Width - 32, 16));
            lblDeleteDetails.TextAlignment = UITextAlignment.Left;
            lblDeleteDetails.Font = myTNBFont.MuseoSans12();
            lblDeleteDetails.TextColor = myTNBColor.TunaGrey();
            lblDeleteDetails.Text = "Notification removed.";
            lblDeleteDetails.Lines = 0;
            lblDeleteDetails.LineBreakMode = UILineBreakMode.WordWrap;

            _viewDelete.AddSubview(lblDeleteDetails);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(_viewDelete);
        }

        internal void ShowDeleteNotification()
        {
            _viewDelete.Hidden = false;
            _viewDelete.Alpha = 1.0f;
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewDelete.Alpha = 0.0f;
            }, () =>
            {
                _viewDelete.Hidden = true;
                DataManager.DataManager.SharedInstance.IsNotificationDeleted = false;
            });
        }
    }
}