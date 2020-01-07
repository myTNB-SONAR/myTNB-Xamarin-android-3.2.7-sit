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
using myTNB.Home.Components;

namespace myTNB.PushNotification
{
    public partial class PushNotificationViewController : CustomUIViewController
    {
        public PushNotificationViewController(IntPtr handle) : base(handle) { }
        private DeleteNotificationResponseModel _deleteNotificationResponse = new DeleteNotificationResponseModel();
        private ReadNotificationResponseModel _readNotificationResponse = new ReadNotificationResponseModel();
        private UserNotificationResponseModel userNotificationResponse = new UserNotificationResponseModel();
        public bool _isSelectionMode;
        internal bool _isDeletionMode;
        private bool _isAllSelected;

        private AccountSelectionComponentV2 _notificationSelectionComponent;
        private NotificationDetailedInfoResponseModel _detailedInfo = new NotificationDetailedInfoResponseModel();
        private TitleBarComponentV2 _titleBarComponent;
        private List<UserNotificationDataModel> _notifications;
        private List<UpdateNotificationModel> _notificationsForUpdate;

        private UIImageView _imgCheckbox;
        private UILabel _lblTitle;

        private RefreshViewComponent _refreshViewComponent;
        private UIView _headerView, _titleBarView;

        private GradientViewComponent _gradientViewComponent;

        public override void ViewDidLoad()
        {
            PageName = PushNotificationConstants.Pagename_PushNotificationList;
            base.ViewDidLoad();
            UpdateNotificationTypes();
            NotifCenterUtility.AddObserver((NSString)"OnNotificationFilterDidChange", OnNotificationFilterDidChange);
            NotifCenterUtility.AddObserver((NSString)"OnReceiveNotificationFromDashboard", OnReceiveNotificationFromDashboard);
            SetNavigationBar();
            SetSubViews();
        }

        private void OnReceiveNotificationFromDashboard(NSNotification notification)
        {
            Debug.WriteLine("OnReceiveNotificationFromDashboard");
            ValidateResponse();
        }

        public void OnNotificationFilterDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> PushNotificationViewController OnNotificationFilterDidChange");
            OnReset();
            SetNavigationBar();
            pushNotificationTableView.TableHeaderView = null;
        }

        private void UpdateNotificationTypes()
        {
            if (DataManager.DataManager.SharedInstance.NotificationGeneralTypes.Count > 0
                && DataManager.DataManager.SharedInstance.NotificationGeneralTypes[0].Id == "all")
            {
                DataManager.DataManager.SharedInstance.NotificationGeneralTypes[0].Title
                    = GetCommonI18NValue(Constants.Common_AllNotifications);
            }
        }

        private void OnReset()
        {
            _isDeletionMode = false;
            _isSelectionMode = false;
            _isAllSelected = false;
            if (_notificationsForUpdate != null)
            {
                _notificationsForUpdate?.Clear();
            }
        }

        private void GetUserNotif()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await PushNotificationHelper.GetNotifications();
                        ValidateResponse();
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void ValidateResponse()
        {
            userNotificationResponse = DataManager.DataManager.SharedInstance.UserNotificationResponse;
            bool isBRCRMAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;

            if (isBRCRMAvailable && userNotificationResponse != null && userNotificationResponse?.d != null && userNotificationResponse.d.IsSuccess
                && userNotificationResponse.d.data != null && userNotificationResponse.d.data.UserNotificationList != null)
            {
                UpdateNotificationDisplay();
            }
            else
            {
                string msg = !string.IsNullOrWhiteSpace(userNotificationResponse?.d?.RefreshMessage)
                    ? userNotificationResponse?.d?.RefreshMessage : GetErrorI18NValue(Constants.Error_RefreshMessage);
                string btnText = !string.IsNullOrWhiteSpace(userNotificationResponse?.d?.RefreshBtnText)
                    ? userNotificationResponse?.d?.RefreshBtnText : GetCommonI18NValue(Constants.Common_RefreshNow);
                DisplayRefreshScreen(msg, btnText);
                _titleBarComponent.SetPrimaryVisibility(true);
                _titleBarComponent.SetSecondaryVisibility(true);
                pushNotificationTableView.Hidden = true;
            }
        }

        private void OnRefreshTap()
        {
            GetUserNotif();
        }

        private void UpdateNotificationDisplay(bool isDelete = false)
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
                string format = @"M/d/yyyy h:mm:ss tt";
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
                    if (_refreshViewComponent != null)
                    {
                        if (_refreshViewComponent.GetView().IsDescendantOfView(View))
                        {
                            _refreshViewComponent.GetView().RemoveFromSuperview();
                        }
                    }
                    pushNotificationTableView.Hidden = false;
                    pushNotificationTableView.ClearsContextBeforeDrawing = true;
                    pushNotificationTableView.RowHeight = UITableView.AutomaticDimension;
                    pushNotificationTableView.EstimatedRowHeight = GetScaledHeight(70);
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
            _titleBarComponent.SetPrimaryVisibility(_notifications == null || _notifications.Count == 0);
            if (isDelete)
            {
                _titleBarComponent.SetSecondaryVisibility(_notifications == null || _notifications.Count == 0);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (DataManager.DataManager.SharedInstance.IsNotificationDeleted)
                        {
                            DisplayToast(GetI18NValue(PushNotificationConstants.I18N_NotificationDeleted));
                            DataManager.DataManager.SharedInstance.IsNotificationDeleted = false;
                        }
                        if (DataManager.DataManager.SharedInstance.NotificationNeedsUpdate)
                        {
                            GetUserNotif();
                        }
                        else
                        {
                            ValidateResponse();
                        }
                    }
                    else
                    {
                        pushNotificationTableView.Hidden = true;
                        _titleBarComponent.SetPrimaryVisibility(true);
                        OnReset();
                        DisplayRefreshScreen(GetErrorI18NValue(Constants.Error_RefreshMessage), GetCommonI18NValue(Constants.Common_RefreshNow));
                    }
                });
            });
        }

        private void DisplayNoNotification()
        {
            pushNotificationTableView.Hidden = true;

            if (_refreshViewComponent != null)
            {
                if (_refreshViewComponent.GetView().IsDescendantOfView(View))
                {
                    _refreshViewComponent.GetView().RemoveFromSuperview();
                }
            }

            _refreshViewComponent = new RefreshViewComponent(View, _headerView);
            _refreshViewComponent.SetIconImage(PushNotificationConstants.IMG_Empty);
            _refreshViewComponent.SetDescription(GetI18NValue(PushNotificationConstants.I18N_NoNotification));
            _refreshViewComponent.SetRefreshButtonHidden(true);

            View.AddSubview(_refreshViewComponent.GetUI());
        }

        private void DisplayRefreshScreen(string msg, string btnText)
        {
            if (_refreshViewComponent != null)
            {
                if (_refreshViewComponent.GetView().IsDescendantOfView(View))
                {
                    _refreshViewComponent.GetView().RemoveFromSuperview();
                }
            }

            _refreshViewComponent = new RefreshViewComponent(View, _headerView);
            _refreshViewComponent.SetIconImage(PushNotificationConstants.IMG_RefreshErrorNormal);
            _refreshViewComponent.SetDescription(msg);
            _refreshViewComponent.SetButtonText(btnText);
            _refreshViewComponent.OnButtonTap = OnRefreshTap;

            View.AddSubview(_refreshViewComponent.GetUI());
        }

        private void SetNavigationBar()
        {
            NavigationController?.SetNavigationBarHidden(true, false);
            _gradientViewComponent = new GradientViewComponent(View, true, (float)GetScaledHeight(88), true);
            _headerView = _gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponentV2(_headerView);

            _titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle(GetI18NValue(PushNotificationConstants.I18N_Title));
            _titleBarComponent.SetSecondaryImage(PushNotificationConstants.IMG_MarkAsRead);
            _titleBarComponent.SetSecondaryAction(new UITapGestureRecognizer((obj) =>
            {
                if (_notificationsForUpdate == null)
                {
                    return;
                }
                bool isNotRead = _notificationsForUpdate.FindIndex(x => !x.IsRead) > -1;
                if (!isNotRead)
                {
                    return;
                }
                ReadNotification(_notificationsForUpdate, true);
            }));
            _titleBarComponent.SetPrimaryVisibility(false);
            _titleBarComponent.SetPrimaryImage(PushNotificationConstants.IMG_Select);
            SetPrimaryActionEvent();

            _titleBarComponent.SetBackVisibility(false);
            _titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                if (_isSelectionMode)
                {
                    OnDismiss();
                }
                else
                {
                    OnBack();
                }
            }));
            _headerView.AddSubview(_titleBarView);
            SetSelectorEvent();
        }

        private void SetPrimaryActionEvent()
        {
            _titleBarComponent.SetPrimaryAction(new UITapGestureRecognizer(() =>
            {
                if (_isDeletionMode)
                {
                    Debug.WriteLine("_isDeletionMode");
                    int count = _notificationsForUpdate != null ? _notificationsForUpdate.Count : 0;
                    if (count == 1)
                    {
                        DeleteNotification(_notificationsForUpdate, true);
                    }
                    else if (count > 1)
                    {
                        bool areAllTobeDeleted = count == _notifications.Count;
                        string alertTitle = GetI18NValue(areAllTobeDeleted ? PushNotificationConstants.I18N_DeleteAllTitle : PushNotificationConstants.I18N_DeleteTitleMultiple);
                        string alertMsg = GetI18NValue(areAllTobeDeleted ? PushNotificationConstants.I18N_DeleteAllMessage : PushNotificationConstants.I18N_DeleteMessageMultiple);
                        UIAlertController deleteAlert = UIAlertController.Create(alertTitle, alertMsg, UIAlertControllerStyle.Alert);
                        deleteAlert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Yes), UIAlertActionStyle.Default, (args) =>
                        {
                            if (_notificationsForUpdate != null)
                            {
                                DeleteNotification(_notificationsForUpdate, true);
                            }
                        }));
                        deleteAlert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_No), UIAlertActionStyle.Cancel, null));
                        deleteAlert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(deleteAlert, animated: true, completionHandler: null);
                    }
                }
                else
                {
                    if (_isSelectionMode)
                    {
                        return;
                    }
                    _titleBarComponent.SetPrimaryImage(PushNotificationConstants.IMG_Cancel);
                    UpdateSelectAllFlags(false);
                    pushNotificationTableView.TableHeaderView = GetTableViewHeader();
                    _titleBarComponent.SetTitle(GetI18NValue(_isSelectionMode ? PushNotificationConstants.I18N_Title : PushNotificationConstants.I18N_Select));
                    _isSelectionMode = !_isSelectionMode;
                    UpdateTitleRightIconImage();
                    pushNotificationTableView.ReloadData();
                }
            }));
        }

        private void SetSelectorEvent()
        {
            _notificationSelectionComponent = new AccountSelectionComponentV2(_headerView, _headerView.Frame.Height - GetScaledHeight(32));
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
                viewController.Title = GetI18NValue(PushNotificationConstants.I18N_SelectNotification);
                viewController.Items = GetNotificationTypeList();
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }));
            _headerView.AddSubview(accountSelectionView);
            View.AddSubview(_headerView);
        }

        private void OnBack()
        {
            OnReset();
            DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex = 0;
            DismissViewController(true, null);
        }

        private void OnDismiss()
        {
            OnReset();
            pushNotificationTableView.TableHeaderView = null;
            UpdateTitleRightIconImage();
            pushNotificationTableView.ReloadData();
            _titleBarComponent.SetSecondaryVisibility(true);
            _titleBarComponent.SetTitle(GetI18NValue(PushNotificationConstants.I18N_Title));
            _titleBarComponent.SetPrimaryImage(PushNotificationConstants.IMG_Select);
            _isDeletionMode = false;
        }

        private void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex = index;
            NotifCenterUtility.PostNotificationName("OnNotificationFilterDidChange", new NSObject());
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
            pushNotificationTableView.Frame = new CGRect(0, _headerView.Frame.GetMaxY(), View.Frame.Width
                , View.Frame.Height - _headerView.Frame.GetMaxY());
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
                                    && _detailedInfo.d.IsSuccess
                                    && _detailedInfo?.d?.data != null
                                    && _detailedInfo?.d?.data.UserNotification != null)
                                {
                                    DataManager.DataManager.SharedInstance.NotificationNeedsUpdate = false;
                                    UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                                    NotificationDetailsViewController viewController =
                                        storyBoard.InstantiateViewController("NotificationDetailsViewController") as NotificationDetailsViewController;
                                    int index = DataManager.DataManager.SharedInstance.UserNotifications.FindIndex(x => x.Id == dataModel.Id);
                                    string notificationTitle = string.Empty;
                                    if (index > -1)
                                    {
                                        DataManager.DataManager.SharedInstance.UserNotifications[index].IsRead = @"true";
                                        notificationTitle = DataManager.DataManager.SharedInstance.UserNotifications[index]?.Title;
                                    }
                                    _detailedInfo.d.data.UserNotification.NotificationTitle = notificationTitle;
                                    if (viewController != null)
                                    {
                                        viewController.NotificationInfo = _detailedInfo?.d?.data?.UserNotification;
                                        NavigationController?.PushViewController(viewController, true);
                                    }
                                    UpdateNotificationDisplay();
                                }
                                else
                                {
                                    DisplayServiceError(_detailedInfo?.d?.DisplayMessage ?? string.Empty);
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

        internal Task GetNotificationDetailedInfo(UserNotificationDataModel dataModel)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    NotificationId = dataModel.Id,
                    dataModel.NotificationType
                };
                //_detailedInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<NotificationDetailedInfoResponseModel>(UserNotificationManager.GetInfo(dataModel.Id));
                _detailedInfo = serviceManager.OnExecuteAPIV6<NotificationDetailedInfoResponseModel>("GetNotificationDetailedInfo", requestParameter);
            });
        }

        internal void ReadNotification(List<UpdateNotificationModel> updateNotificationList, bool isMultiple = false, NSIndexPath indexPath = null)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await ReadUserNotification(updateNotificationList).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                ReadNotificationResponseModel readNotificationResponse = _readNotificationResponse;
                                if (readNotificationResponse != null && readNotificationResponse?.d != null
                                    && readNotificationResponse.d.IsSuccess)
                                {
                                    UpdateNotifications(updateNotificationList, isMultiple, indexPath, true);
                                    UpdateNotificationDisplay();
                                    UpdateTitleRightIconImage();
                                    NotifCenterUtility.PostNotificationName("NotificationDidChange", new NSObject());
                                    pushNotificationTableView.ReloadData();
                                    if (_lblTitle != null)
                                    {
                                        _lblTitle.Text = GetCommonI18NValue(Constants.Common_SelectAll);
                                    }
                                    if (_imgCheckbox != null)
                                    {
                                        _imgCheckbox.Image = UIImage.FromBundle(PushNotificationConstants.IMG_ChkInactive);
                                    }
                                    _isDeletionMode = false;
                                    _isSelectionMode = isMultiple;
                                    _isAllSelected = false;
                                    OnDismiss();
                                }
                                else
                                {
                                    DisplayToast(readNotificationResponse?.d?.DisplayMessage ?? GetErrorI18NValue(Constants.Error_DefaultServiceErrorMessage));
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
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
                                DeleteNotificationResponseModel deleteNotifResponse = _deleteNotificationResponse;
                                if (deleteNotifResponse != null && deleteNotifResponse?.d != null
                                    && deleteNotifResponse.d.IsSuccess)
                                {
                                    UpdateNotifications(updateNotificationList, isMultiple, indexPath);
                                    pushNotificationTableView.ReloadData();
                                    UpdateTitleRightIconImage();
                                    UpdateNotificationDisplay(true);
                                    NotifCenterUtility.PostNotificationName("NotificationDidChange", new NSObject());
                                    DisplayToast(GetI18NValue(isMultiple ? PushNotificationConstants.I18N_NotificationsDeleted : PushNotificationConstants.I18N_NotificationDeleted));
                                    OnDismiss();
                                }
                                else
                                {
                                    DisplayToast(deleteNotifResponse?.d?.DisplayMessage ?? GetErrorI18NValue(Constants.Error_DefaultServiceErrorMessage));
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        void UpdateNotifications(List<UpdateNotificationModel> updateNotificationList, bool isMultiple = false
            , NSIndexPath indexPath = null, bool isRead = false)
        {
            if (isMultiple)
            {
                if (updateNotificationList.Count == DataManager.DataManager.SharedInstance.UserNotifications.Count && !isRead)
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
                        ModifyNotification(notificationIndex, isRead);
                    }

                    int userNotificationIndex = DataManager.DataManager.SharedInstance.UserNotifications
                        .FindIndex(x => x?.NotificationType == item.NotificationType && x.Id == item.NotificationId);
                    Debug.WriteLine("userNotificationIndex: " + userNotificationIndex);
                    if (userNotificationIndex > -1)
                    {
                        ModifyNotification(userNotificationIndex, isRead, true);
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
                    ModifyNotification(userNotificationIndex, isRead, true);
                }
                ModifyNotification(indexPath.Row, isRead);
            }
        }

        private void ModifyNotification(int index, bool isRead = false, bool isGlobalList = false)
        {
            if (isRead)
            {
                if (isGlobalList)
                {
                    DataManager.DataManager.SharedInstance.UserNotifications[index].IsRead = "true";
                    DataManager.DataManager.SharedInstance.UserNotifications[index].IsSelected = false;
                    return;
                }
                _notifications[index].IsRead = "true";
                _notifications[index].IsSelected = false;
            }
            else
            {
                if (isGlobalList)
                {
                    DataManager.DataManager.SharedInstance.UserNotifications.RemoveAt(index);
                    return;
                }
                _notifications.RemoveAt(index);
            }
        }

        Task DeleteUserNotification(List<UpdateNotificationModel> deleteNotificationList)
        {
            UserEntity user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    updatedNotifications = deleteNotificationList,
                };
                _deleteNotificationResponse = serviceManager.OnExecuteAPIV6<DeleteNotificationResponseModel>(PushNotificationConstants.Service_DeleteNotification, requestParameter);
            });
        }

        Task ReadUserNotification(List<UpdateNotificationModel> readNotificationList)
        {
            UserEntity user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                ? DataManager.DataManager.SharedInstance.UserEntity[0]
                : new UserEntity();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    updatedNotifications = readNotificationList
                };
                _readNotificationResponse = serviceManager.OnExecuteAPIV6<ReadNotificationResponseModel>(PushNotificationConstants.Service_ReadUserNotification, requestParameter);
            });
        }

        /// <summary>
        /// Creates the view header.
        /// </summary>
        UIView GetTableViewHeader()
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = ScaleUtility.GetScaledHeight(62);
            nfloat chkWidth = ScaleUtility.GetWidthByScreenSize(20);

            UIView viewCheckBox = new UIView(new CGRect(cellWidth - chkWidth - GetScaledWidth(16), GetScaledHeight(24), chkWidth, chkWidth));
            _imgCheckbox = new UIImageView(new CGRect(0, 0, chkWidth, chkWidth))
            {
                Image = UIImage.FromBundle(PushNotificationConstants.IMG_ChkInactive)
            };
            viewCheckBox.AddSubview(_imgCheckbox);

            _lblTitle = new UILabel(new CGRect(GetScaledWidth(16), GetScaledHeight(24), cellWidth / 2, GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500,
                Text = GetCommonI18NValue(Constants.Common_SelectAll)
            };

            viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnCheckboxSelect();
            }));

            UIView viewLine = new UIView(new CGRect(0, cellHeight - GetScaledHeight(1), cellWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
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

        private void SetTableHeader(bool areAllSelected)
        {
            if (_imgCheckbox != null)
            {
                _imgCheckbox.Image = UIImage.FromBundle(areAllSelected ? PushNotificationConstants.IMG_ChkActive : PushNotificationConstants.IMG_ChkInactive);
            }
            if (_lblTitle != null)
            {
                _lblTitle.Text = GetCommonI18NValue(areAllSelected ? Constants.Common_UnselectAll : Constants.Common_SelectAll);
            }
        }

        private void OnCheckboxSelect(bool isCellSelected = false)
        {
            _isAllSelected = !_isAllSelected;
            SetTableHeader(_isAllSelected);
            if (!isCellSelected)
            {
                UpdateSelectAllFlags(_isAllSelected);
            }
        }

        /// <summary>
        /// Updates the select all flags.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        private void UpdateSelectAllFlags(bool flag)
        {
            _notificationsForUpdate = null;
            if (flag)
            {
                _notificationsForUpdate = new List<UpdateNotificationModel>();
            }

            if (_notifications != null && _notifications?.Count > 0)
            {
                foreach (UserNotificationDataModel obj in _notifications)
                {
                    obj.IsSelected = flag;
                    if (flag)
                    {
                        _notificationsForUpdate.Add(new UpdateNotificationModel()
                        {
                            NotificationType = obj?.NotificationType,
                            NotificationId = obj.Id,
                            IsRead = obj.IsRead.ToUpper() == "TRUE"
                        });
                    }
                }
                pushNotificationTableView.ReloadData();
                UpdateTitleRightIconImage();
            }
        }

        internal void UpdateSectionHeaderWidget()
        {
            int selectedCount = _notifications.FindAll(x => x.IsSelected == true).Count;
            _isAllSelected = selectedCount != _notifications.Count;
            OnCheckboxSelect(true);
        }

        /// <summary>
        /// Updates the title right icon image.
        /// </summary>
        internal void UpdateTitleRightIconImage(UserNotificationDataModel notifModel = null)
        {
            _isDeletionMode = IsAtLeastOneIsSelected();
            if (_isSelectionMode)
            {
                _titleBarComponent.SetSecondaryVisibility(false);
                _titleBarComponent.SetPrimaryImage(_isDeletionMode ? PushNotificationConstants.IMG_Delete : PushNotificationConstants.IMG_DeleteInactive);
            }

            UpdateNotificationForDeletionList(notifModel);
            int count = _notificationsForUpdate != null ? _notificationsForUpdate.Count : 0;

            string title = GetI18NValue(PushNotificationConstants.I18N_Title);
            if (_isSelectionMode)
            {
                title = count > 0 ? string.Format(GetI18NValue(PushNotificationConstants.I18N_Selected), count) : GetI18NValue(PushNotificationConstants.I18N_Select);
                if (notifModel != null)
                {
                    bool areAllSelected = count == _notifications.Count;
                    SetTableHeader(areAllSelected);
                }
            }
            _titleBarComponent.SetTitle(title);
            _titleBarComponent.SetSecondaryImage(IsAtleastOneRead() ? PushNotificationConstants.IMG_MarkAsRead : PushNotificationConstants.IMG_MarkAsReadInactive);
            _titleBarComponent.SetPrimaryImage(IsAtLeastOneIsSelected() ? PushNotificationConstants.IMG_Delete : PushNotificationConstants.IMG_DeleteInactive);
        }

        private bool IsAtleastOneRead()
        {
            if (_notifications != null)
            {
                foreach (UserNotificationDataModel obj in _notifications)
                {
                    if (obj.IsSelected && obj.IsRead.ToLower() == "false")
                    {
                        return true;
                    }
                }
            }
            return false;
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
        private void UpdateNotificationForDeletionList(UserNotificationDataModel notifModel)
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
                        NotificationId = notifModel?.Id,
                        IsRead = notifModel.IsRead.ToUpper() == "TRUE"
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