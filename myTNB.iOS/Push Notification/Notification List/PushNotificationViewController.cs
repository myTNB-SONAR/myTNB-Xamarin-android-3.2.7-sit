using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using UIKit;
using myTNB.SQLite.SQLiteDataManager;
using Foundation;
using System.Diagnostics;

namespace myTNB.PushNotification
{
    public partial class PushNotificationViewController : CustomUIViewController
    {
        public PushNotificationViewController(IntPtr handle) : base(handle)
        {
        }
        public DeleteNotificationResponseModel _deleteNotificationResponse = new DeleteNotificationResponseModel();
        public bool _isSelectionMode;
        internal bool _isDeletionMode;
        bool _isAllSelected;

        AccountSelectionComponent _notificationSelectionComponent;
        NotificationDetailedInfoResponseModel _detailedInfo = new NotificationDetailedInfoResponseModel();
        TitleBarComponent _titleBarComponent;
        List<UserNotificationDataModel> _notifications;
        List<UpdateNotificationModel> _notificationsForUpdate;

        UIImageView _imgNoNotification, _imgCheckbox;
        UILabel _lblNoNotification, _lblTitle;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"OnNotificationFilterDidChange", OnNotificationFilterDidChange);
            SetNavigationBar();
            SetSubViews();
        }

        public void OnNotificationFilterDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> PushNotificationViewController OnNotificationFilterDidChange");
            OnReset();
            SetNavigationBar();
            pushNotificationTableView.TableHeaderView = null;
        }

        void OnReset()
        {
            _isDeletionMode = false;
            _isSelectionMode = false;
            _isAllSelected = false;
            _notificationsForUpdate?.Clear();
        }

        private void UpdateNotificationDisplay()
        {
            string filterID = "all";
            if (DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex
                < DataManager.DataManager.SharedInstance.NotificationGeneralTypes?.Count)
            {
                _notificationSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance
                    .NotificationGeneralTypes[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Title);
                filterID = DataManager.DataManager.SharedInstance.NotificationGeneralTypes[DataManager
                    .DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Id;
            }

            _notifications = new List<UserNotificationDataModel>();
            if (DataManager.DataManager.SharedInstance.UserNotifications.Count > 0)
            {
                var format = @"M/d/yyyy h:mm:ss tt";
                if (filterID == "all")
                {
                    _notifications = DataManager.DataManager.SharedInstance.UserNotifications
                    .Where(x => x.IsDeleted.ToLower() == "false").OrderByDescending(o => DateTime.ParseExact(o.CreatedDate
                    , format, System.Globalization.CultureInfo.InvariantCulture)).ThenBy(t => t.Title).ToList();
                }
                else
                {
                    _notifications = DataManager.DataManager.SharedInstance.UserNotifications
                    .Where(x => x.IsDeleted.ToLower() == "false" && x.NotificationTypeId == filterID).OrderByDescending(o => DateTime.ParseExact(o.CreatedDate
                    , format, System.Globalization.CultureInfo.InvariantCulture)).ThenBy(t => t.Title).ToList();
                }

                if (_notifications != null && _notifications.Count > 0)
                {
                    if (_imgNoNotification != null && _lblNoNotification != null)
                    {
                        _imgNoNotification.Hidden = true;
                        _lblNoNotification.Hidden = true;
                    }
                    pushNotificationTableView.Hidden = false;
                    pushNotificationTableView.ClearsContextBeforeDrawing = true;
                    pushNotificationTableView.Source = new PushNotificationDataSource(this, _notifications);
                    pushNotificationTableView.ReloadData();
                }
                else
                {
                    DisplayNoNotification();
                }
            }
            else
            {
                DisplayNoNotification();
            }
            _titleBarComponent.SetNotificationVisibility(_notifications == null || _notifications.Count == 0);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (DataManager.DataManager.SharedInstance.IsNotificationDeleted)
            {
                DisplayToast("PushNotification_NotificationDeleted".Translate());
                DataManager.DataManager.SharedInstance.IsNotificationDeleted = false;
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
                            AlertHandler.DisplayNoDataAlert(this);
                        }
                    });
                });
            }
            else
            {
                UpdateNotificationDisplay();
            }
        }

        void DisplayNoNotification()
        {
            if (_imgNoNotification == null || _lblNoNotification == null)
            {
                _imgNoNotification = new UIImageView(new CGRect((View.Frame.Width / 2) - 75, 185, 150, 150))
                {
                    Image = UIImage.FromBundle("Notification-Empty")
                };
                _lblNoNotification = new UILabel(new CGRect(44, 352, View.Frame.Width - 88, 16))
                {
                    TextAlignment = UITextAlignment.Center,
                    Text = "PushNotification_NoNotification".Translate(),
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.SilverChalice
                };
                View.AddSubviews(new UIView[] { _imgNoNotification, _lblNoNotification });
            }
            pushNotificationTableView.Hidden = true;
            _imgNoNotification.Hidden = false;
            _lblNoNotification.Hidden = false;
        }

        internal void SetNavigationBar()
        {
            NavigationController?.SetNavigationBarHidden(true, false);
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 89, true);
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);

            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle("PushNotification_Title".Translate());
            _titleBarComponent.SetSecondaryImage("Notification-MarkAsRead");
            _titleBarComponent.SetSecondaryAction(new UITapGestureRecognizer((obj) =>
            {
                int count = _notificationsForUpdate != null ? _notificationsForUpdate.Count : 0;
                if (count > 1)
                {
                    var readAlert = UIAlertController.Create("PushNotification_ReadNotificationsTitle".Translate()
                        , "PushNotification_ReadNotificationsMessage".Translate(), UIAlertControllerStyle.Alert);
                    readAlert.AddAction(UIAlertAction.Create("Common_Yes".Translate(), UIAlertActionStyle.Default, (args) =>
                    {
                        if (_notificationsForUpdate != null)
                        {
                            Debug.WriteLine("notificationsForDeletion count: " + _notificationsForUpdate.Count);
                            foreach (UpdateNotificationModel notif in _notificationsForUpdate)
                            {
                                Debug.WriteLine("Delete NotificationId: " + notif.NotificationId);
                            }
                            //Todo: Read Notification
                        }
                    }));
                    readAlert.AddAction(UIAlertAction.Create("Common_No".Translate(), UIAlertActionStyle.Cancel, null));
                    PresentViewController(readAlert, animated: true, completionHandler: null);
                }
                else
                {
                    if (_notificationsForUpdate != null)
                    {
                        Debug.WriteLine("notificationsForDeletion count: " + _notificationsForUpdate.Count);
                        foreach (UpdateNotificationModel notif in _notificationsForUpdate)
                        {
                            Debug.WriteLine("Delete NotificationId: " + notif.NotificationId);
                        }
                        //Todo: Read Notification
                    }
                }
            }));
            _titleBarComponent.SetNotificationVisibility(false);
            _titleBarComponent.SetNotificationImage("Notification-Select");
            _titleBarComponent.SetNotificationAction(new UITapGestureRecognizer(() =>
            {
                if (_isDeletionMode)
                {
                    Debug.WriteLine("_isDeletionMode");
                    int count = _notificationsForUpdate != null ? _notificationsForUpdate.Count : 0;
                    string alertTitle = count > 1 ? "PushNotification_DeleteTitleMultiple" : "PushNotification_DeleteMessage";
                    string alertMsg = count > 1 ? "PushNotification_DeleteMessageMultiple" : "PushNotification_DeleteMessage";
                    var deleteAlert = UIAlertController.Create(alertTitle.Translate(), alertMsg.Translate(), UIAlertControllerStyle.Alert);
                    deleteAlert.AddAction(UIAlertAction.Create("Common_Yes".Translate(), UIAlertActionStyle.Default, (args) =>
                    {
                        if (_notificationsForUpdate != null)
                        {
                            Debug.WriteLine("notificationsForDeletion count: " + _notificationsForUpdate.Count);
                            foreach (UpdateNotificationModel notif in _notificationsForUpdate)
                            {
                                Debug.WriteLine("Delete NotificationId: " + notif.NotificationId);
                            }
                            DeleteNotification(_notificationsForUpdate, true);
                        }
                    }));
                    deleteAlert.AddAction(UIAlertAction.Create("Common_No".Translate(), UIAlertActionStyle.Cancel, null));
                    PresentViewController(deleteAlert, animated: true, completionHandler: null);
                }
                else
                {
                    if (_isSelectionMode)
                    {
                        _titleBarComponent.SetNotificationImage("Notification-Select");
                        pushNotificationTableView.TableHeaderView = null;
                        _isAllSelected = false;
                    }
                    else
                    {
                        _titleBarComponent.SetNotificationImage("IC-Header-Cancel");
                        UpdateSelectAllFlags(false);
                        pushNotificationTableView.TableHeaderView = GetTableViewHeader();
                    }
                    _isSelectionMode = !_isSelectionMode;
                    UpdateTitleRightIconImage();
                    pushNotificationTableView.ReloadData();
                }
            }));

            _titleBarComponent.SetBackVisibility(false);
            _titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                OnReset();
                DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex = 0;
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);

            _notificationSelectionComponent = new AccountSelectionComponent(headerView);
            UIView accountSelectionView = _notificationSelectionComponent.GetUI();

            if (DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex
                < DataManager.DataManager.SharedInstance.NotificationGeneralTypes?.Count)
            {
                _notificationSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance.NotificationGeneralTypes[DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex].Title);
            }
            _notificationSelectionComponent.SetSelectAccountEvent(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = "PushNotification_SelectNotification".Translate();
                viewController.Items = GetNotificationTypeList();
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }));
            headerView.AddSubview(accountSelectionView);
            View.AddSubview(headerView);
        }

        void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex = index;
            NSNotificationCenter.DefaultCenter.PostNotificationName("OnNotificationFilterDidChange", new NSObject());
        }

        List<string> GetNotificationTypeList()
        {
            if (DataManager.DataManager.SharedInstance.NotificationGeneralTypes != null
                && DataManager.DataManager.SharedInstance.NotificationGeneralTypes.Count > 0)
            {
                List<string> feedbackList = new List<string>();
                foreach (NotificationPreferenceModel item in DataManager.DataManager.SharedInstance.NotificationGeneralTypes)
                {
                    feedbackList.Add(item?.Title ?? string.Empty);
                }
                return feedbackList;
            }
            return new List<string>();
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
                            InvokeOnMainThread(() =>
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
                                    var index = DataManager.DataManager.SharedInstance.UserNotifications.FindIndex(x => x.Id == dataModel.Id);
                                    var notificationTitle = string.Empty;
                                    if (index > -1)
                                    {
                                        DataManager.DataManager.SharedInstance.UserNotifications[index].IsRead = @"true";
                                        notificationTitle = DataManager.DataManager.SharedInstance.UserNotifications[index]?.Title;
                                    }
                                    _detailedInfo.d.data.NotificationTitle = notificationTitle;
                                    viewController.NotificationInfo = _detailedInfo?.d?.data;
                                    NavigationController?.PushViewController(viewController, true);
                                    UpdateNotificationDisplay();
                                }
                                else
                                {
                                    AlertHandler.DisplayServiceError(this, _detailedInfo?.d?.message);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
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

        internal void MarkNotificationAsRead(NSIndexPath indexPath)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await MarkUserNotification(_notifications[indexPath.Row]).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                var markAsReadResponse = true;
                                var deleteNotifResponse = _deleteNotificationResponse;
                                if (markAsReadResponse)
                                {
                                    _notifications[indexPath.Row].IsRead = "true";
                                    pushNotificationTableView.ReloadData();
                                }
                                else
                                {
                                    AlertHandler.DisplayServiceError(this, deleteNotifResponse?.d?.message);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        internal void DeleteNotification(List<UpdateNotificationModel> updateNotificationList, bool isMultiple = false, NSIndexPath indexPath = null)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await DeleteUserNotification(updateNotificationList).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                var deleteNotifResponse = _deleteNotificationResponse;
                                if (deleteNotifResponse != null && deleteNotifResponse?.d != null
                                    && deleteNotifResponse?.d?.status?.ToLower() == "success"
                                    && deleteNotifResponse?.d?.didSucceed == true)
                                {
                                    UpdateNotifications(updateNotificationList, isMultiple, indexPath);
                                    UpdateNotificationDisplay();
                                    pushNotificationTableView.ReloadData();
                                    UpdateTitleRightIconImage();
                                    NSNotificationCenter.DefaultCenter.PostNotificationName("NotificationDidChange", new NSObject());
                                    DisplayToast("PushNotification_NotificationsDeleted".Translate());
                                }
                                else
                                {
                                    DisplayToast(deleteNotifResponse?.d?.message ?? "Error_DefaultMessage".Translate());
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        void test()
        {

        }

        void UpdateNotifications(List<UpdateNotificationModel> updateNotificationList, bool isMultiple = false, NSIndexPath indexPath = null)
        {
            if (isMultiple)
            {
                if (updateNotificationList.Count == DataManager.DataManager.SharedInstance.UserNotifications.Count)
                {
                    updateNotificationList.Clear();
                    _notifications.Clear();
                    DataManager.DataManager.SharedInstance.UserNotifications.Clear();
                    return;
                }
                foreach (UpdateNotificationModel item in updateNotificationList)
                {
                    int notificationIndex = _notifications.FindIndex(x => x?.NotificationType == item.NotificationType
                        && x.Id == item.NotificationId);
                    Debug.WriteLine("notificationIndex: " + notificationIndex);
                    if (notificationIndex > -1)
                    {
                        _notifications.RemoveAt(notificationIndex);
                    }

                    int userNotificationIndex = DataManager.DataManager.SharedInstance.UserNotifications
                        .FindIndex(x => x?.NotificationType == item.NotificationType && x.Id == item.NotificationId);
                    Debug.WriteLine("userNotificationIndex: " + userNotificationIndex);
                    if (userNotificationIndex > -1)
                    {
                        DataManager.DataManager.SharedInstance.UserNotifications.RemoveAt(userNotificationIndex);
                    }
                }
                updateNotificationList.Clear();
            }
            else
            {
                int userNotificationIndex = DataManager.DataManager.SharedInstance.UserNotifications
                       .FindIndex(x => x?.NotificationType == _notifications[indexPath.Row].NotificationType
                       && x.Id == _notifications[indexPath.Row].Id);
                if (userNotificationIndex > -1)
                {
                    DataManager.DataManager.SharedInstance.UserNotifications.RemoveAt(userNotificationIndex);
                }
                _notifications.RemoveAt(indexPath.Row);
            }
        }

        Task DeleteUserNotification(List<UpdateNotificationModel> deleteNotificationList)
        {
            var user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    ApiKeyID = TNBGlobal.API_KEY_ID,
                    UpdatedNotifications = deleteNotificationList,
                    Email = user?.email,
                    DeviceId = DataManager.DataManager.SharedInstance.UDID,
                    SSPUserId = user?.userID
                };
                _deleteNotificationResponse = serviceManager.DeleteUserNotification("DeleteUserNotification_V3", requestParameter);
            });
        }

        Task MarkUserNotification(UserNotificationDataModel dataModel)
        {
            var user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                ? DataManager.DataManager.SharedInstance.UserEntity[0]
                : new UserEntity();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    ApiKeyID = TNBGlobal.API_KEY_ID,
                    //UpdatedNotifications
                    NotificationType = dataModel?.BCRMNotificationType,
                    NotificationId = dataModel?.Id,
                    Email = user?.email,
                    DeviceId = DataManager.DataManager.SharedInstance.UDID,
                    SSPUserId = user?.userID
                };
            });
        }

        /// <summary>
        /// Creates the view header.
        /// </summary>
        UIView GetTableViewHeader()
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 66;

            UIView viewCheckBox = new UIView(new CGRect(cellWidth - 40, 22, 24, 24));
            _imgCheckbox = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Payment-Checkbox-Inactive")
            };
            viewCheckBox.AddSubview(_imgCheckbox);

            _lblTitle = new UILabel(new CGRect(18, 25, cellWidth - 96 - 60, 18))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14,
                Text = "Feedback_SelectAll".Translate()
            };

            viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnCheckboxSelect();
            }));


            UIView viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };

            UIView viewHeader = new UIView
            {
                ClipsToBounds = true,
                Frame = new CGRect(0, 0, cellWidth, cellHeight),
                BackgroundColor = UIColor.White
            };
            viewHeader.AddSubviews(new UIView[] { viewCheckBox, _lblTitle, viewLine });

            return viewHeader;
        }

        void OnCheckboxSelect(bool isCellSelected = false)
        {
            _isAllSelected = !_isAllSelected;
            if (_imgCheckbox != null)
            {
                _imgCheckbox.Image = UIImage.FromBundle(_isAllSelected ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");
            }
            if (_lblTitle != null)
            {
                _lblTitle.Text = _isAllSelected ? "Feedback_UnselectAll".Translate() : "Feedback_SelectAll".Translate();
            }
            if (!isCellSelected)
            {
                UpdateSelectAllFlags(_isAllSelected);
            }
        }

        internal void UpdateSectionHeaderWidget()
        {
            int selectedCount = _notifications.FindAll(x => x.IsSelected == true).Count;
            _isAllSelected = selectedCount != _notifications.Count;
            OnCheckboxSelect(true);
        }

        /// <summary>
        /// Updates the select all flags.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        internal void UpdateSelectAllFlags(bool flag)
        {
            _notificationsForUpdate = null;
            if (flag)
            {
                _notificationsForUpdate = new List<UpdateNotificationModel>();
            }

            foreach (UserNotificationDataModel obj in _notifications)
            {
                obj.IsSelected = flag;
                if (flag)
                {
                    _notificationsForUpdate.Add(new UpdateNotificationModel()
                    {
                        NotificationType = obj?.NotificationType,
                        NotificationId = obj.Id
                    });
                }
            }

            pushNotificationTableView.ReloadData();
            UpdateTitleRightIconImage();
        }

        /// <summary>
        /// Updates the title right icon image.
        /// </summary>
        public void UpdateTitleRightIconImage(UserNotificationDataModel notifModel = null)
        {
            _isDeletionMode = IsAtLeastOneIsSelected();
            string icon = "Notification-Select";
            if (_isSelectionMode)
            {
                icon = _isDeletionMode ? "Notification-Delete" : "IC-Header-Cancel";
                _titleBarComponent.SetSecondaryVisibility(!_isDeletionMode);
            }
            _titleBarComponent.SetNotificationImage(icon);
            UpdateNotificationForDeletionList(notifModel);
        }

        /// <summary>
        /// Checks if at least one notification is selected for deletion
        /// </summary>
        /// <returns><c>true</c>, if at least one is selected was ised, <c>false</c> otherwise.</returns>
        internal bool IsAtLeastOneIsSelected()
        {
            foreach (UserNotificationDataModel obj in _notifications)
            {
                if (obj.IsSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the notification for deletion list.
        /// </summary>
        /// <param name="notifModel">Notif model.</param>
        internal void UpdateNotificationForDeletionList(UserNotificationDataModel notifModel)
        {
            if (notifModel != null)
            {
                if (notifModel.IsSelected)
                {
                    if (_notificationsForUpdate == null)
                    {
                        _notificationsForUpdate = new List<UpdateNotificationModel>();
                    }
                    _notificationsForUpdate.Add(new UpdateNotificationModel()
                    {
                        NotificationType = notifModel?.NotificationType,
                        NotificationId = notifModel?.Id

                    });
                }
                else
                {
                    if (_notificationsForUpdate != null)
                    {
                        int index = _notificationsForUpdate.FindIndex(x => x.NotificationType == notifModel?.NotificationType
                            && x.NotificationId == notifModel.Id);
                        if (index > -1)
                        {
                            _notificationsForUpdate.RemoveAt(index);
                        }
                    }
                }
            }
        }
    }
}