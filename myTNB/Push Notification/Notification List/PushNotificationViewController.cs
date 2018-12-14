using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using UIKit;

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
                PushNotificationHelper.GetNotifications();
            }
            _notificationSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance.NotificationGeneralTypes.d.data[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Title);

            string filterID = DataManager.DataManager.SharedInstance.NotificationGeneralTypes.d.data[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Id;

            List<UserNotificationDataModel> notifications = new List<UserNotificationDataModel>();
            if (DataManager.DataManager.SharedInstance.UserNotifications.Count > 0)
            {
                if (filterID == "all")
                {
                    notifications = DataManager.DataManager.SharedInstance.UserNotifications
                                           .Where(x => x.IsDeleted.ToLower() == "false").ToList();
                }
                else
                {
                    notifications = DataManager.DataManager.SharedInstance.UserNotifications
                                               .Where(x => x.IsDeleted.ToLower() == "false"
                                                      && x.NotificationTypeId == filterID).ToList();
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

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
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
            _notificationSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance.NotificationGeneralTypes.d.data[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Title);
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
            pushNotificationTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneX() ? 113 : 89, View.Frame.Width, View.Frame.Height - 89);
            pushNotificationTableView.RowHeight = 66f;
            pushNotificationTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        internal void ExecuteGetNotificationDetailedInfoCall(string id)
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        GetNotificationDetailedInfo(id).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_detailedInfo != null && _detailedInfo.d != null
                                   && _detailedInfo.d.isError.ToLower() == "false"
                                   && _detailedInfo.d.status.ToLower() == "success"
                                   && _detailedInfo.d.data != null)
                                {
                                    DataManager.DataManager.SharedInstance.NotificationNeedsUpdate = true;
                                    UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                                    NotificationDetailsViewController viewController =
                                        storyBoard.InstantiateViewController("NotificationDetailsViewController") as NotificationDetailsViewController;
                                    _detailedInfo.d.data.NotificationTitle = DataManager.DataManager.SharedInstance.UserNotifications.Where(x => x.Id == id).ToList()[0].Title;
                                    viewController.NotificationInfo = _detailedInfo.d.data;
                                    NavigationController.PushViewController(viewController, true);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        DisplayAlertMessage("No Data Connection", "Please check your data connection and try again.");
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        internal Task GetNotificationDetailedInfo(string id)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    notificationId = id
                };
                _detailedInfo = serviceManager.GetNotificationDetailedInfo("GetNotificationDetailedInfo", requestParameter);
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