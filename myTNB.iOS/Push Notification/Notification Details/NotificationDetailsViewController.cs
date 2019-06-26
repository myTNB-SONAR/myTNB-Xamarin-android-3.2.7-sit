using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using CoreGraphics;
using System.Drawing;
using System.Threading.Tasks;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.SitecoreCMS.Model;
using System.Collections.Generic;

namespace myTNB
{
    public partial class NotificationDetailsViewController : CustomUIViewController
    {
        public NotificationDetailsViewController(IntPtr handle) : base(handle)
        {
        }
        TitleBarComponent _titleBarComponent;
        UIView _viewCTA;

        DueAmountResponseModel _dueAmount = new DueAmountResponseModel();
        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        DeleteNotificationResponseModel _deleteNotificationResponse = new DeleteNotificationResponseModel();

        public UserNotificationDataModel NotificationInfo = new UserNotificationDataModel();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            if (_titleBarComponent != null)
            {
                _titleBarComponent.SetTitle(NotificationInfo.NotificationTitle);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetSubViews();
            RenderButtons();
            int unreadCount = PushNotificationHelper.GetNotificationCount();
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = unreadCount == 0 ? 0 : unreadCount - 1;
        }
        /// <summary>
        /// Renders the buttons based on BCRMNotificationType
        /// </summary>
        internal void RenderButtons()
        {
            if (NotificationInfo.BCRMNotificationType != Enums.BCRMNotificationEnum.Maintenance
                && NotificationInfo.BCRMNotificationType != Enums.BCRMNotificationEnum.News)
            {
                if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.Promotion)
                {
                    SetSubViewForPromotionNotification();
                }
                else
                {
                    ActivityIndicator.Show();
                    GetAccountDueAmount().ContinueWith(task =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            SetSubViewsForNormalNotification();
                            ActivityIndicator.Hide();
                        });
                    });
                }
            }
        }

        internal void SetNavigationBar()
        {
            NavigationController?.SetNavigationBarHidden(true, false);
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle(NotificationInfo.Title);
            _titleBarComponent.SetPrimaryVisibility(false);
            _titleBarComponent.SetPrimaryImage("Notification-Delete");
            _titleBarComponent.SetPrimaryAction(new UITapGestureRecognizer(() =>
            {
                var alert = UIAlertController.Create("PushNotification_DeleteTitle".Translate()
                    , "PushNotification_DeleteMessage".Translate(), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Common_Ok".Translate(), UIAlertActionStyle.Default, (obj) =>
                {
                    ActivityIndicator.Show();
                    InvokeOnMainThread(async () =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            await DeleteUserNotification(NotificationInfo?.Id);
                            if (_deleteNotificationResponse != null && _deleteNotificationResponse?.d != null
                                && _deleteNotificationResponse?.d?.status?.ToLower() == "success"
                                && _deleteNotificationResponse?.d?.didSucceed == true)
                            {
                                DataManager.DataManager.SharedInstance.IsNotificationDeleted = true;
                                var index = DataManager.DataManager.SharedInstance.UserNotifications.FindIndex(x => x.Id == NotificationInfo?.Id);
                                if (index > -1)
                                {
                                    DataManager.DataManager.SharedInstance.UserNotifications.RemoveAt(index);
                                }
                                NavigationController?.PopViewController(true);
                            }
                            else
                            {
                                DisplayServiceError(_deleteNotificationResponse?.d?.message);
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                        ActivityIndicator.Hide();
                    });
                }));
                alert.AddAction(UIAlertAction.Create("Common_Cancel".Translate(), UIAlertActionStyle.Cancel, (obj) => { }));
                PresentViewController(alert, animated: true, completionHandler: null);
            }));
            _titleBarComponent.SetBackVisibility(false);
            _titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                NavigationController?.PopViewController(true);
            }));
            headerView.AddSubview(titleBarView);

            View.AddSubview(headerView);
        }

        internal void OnCTAClick(string actionString)
        {
            var index = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindIndex(x => x.accNum == NotificationInfo.AccountNum) ?? -1;
            if (index > -1)
            {
                ActivityIndicator.Show();

#if true
                var selected = DataManager.DataManager.SharedInstance.AccountRecordsList.d[(int)index];
                DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
#else

                DataManager.DataManager.SharedInstance.SelectedAccount =
                    DataManager.DataManager.SharedInstance.AccountRecordsList.d[index];
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex = index;
                DataManager.DataManager.SharedInstance.PreviousSelectedAccountIndex = index;
#endif

                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            Task[] taskList = new Task[] { GetBillingAccountDetails() };
                            Task.WaitAll(taskList);
                            if (_billingAccountDetailsList != null && _billingAccountDetailsList?.d != null
                                && _billingAccountDetailsList?.d?.data != null && _billingAccountDetailsList?.d?.didSucceed == true)
                            {
                                DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                                if (!DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount)
                                {
                                    DataManager.DataManager.SharedInstance.SaveToBillingAccounts(DataManager.DataManager.SharedInstance.BillingAccountDetails
                                        , DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                                }
                                string storyboardID = actionString == "BillViewController" ? "Dashboard" : "Payment";
                                DisplayPage(storyboardID, actionString);
                            }
                            else
                            {
                                DataManager.DataManager.SharedInstance.IsSameAccount = true;
                                DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                                DisplayServiceError(_billingAccountDetailsList?.d?.message);
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                        ActivityIndicator.Hide();
                    });
                });
            }
        }

        internal Task GetBillingAccountDetails()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    CANum = NotificationInfo.AccountNum
                };
                _billingAccountDetailsList = serviceManager.GetBillingAccountDetails("GetBillingAccountDetails", requestParameter);
            });
        }

        internal Task GetAccountDueAmount()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = NotificationInfo.AccountNum
                };
                _dueAmount = serviceManager.GetAccountDueAmount("GetAccountDueAmount", requestParameter);
            });
        }
        /// <summary>
        /// Sets the sub views for the default content
        /// </summary>
        internal void SetSubViews()
        {
            UIImageView imgViewHeader = new UIImageView(new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64
                , View.Frame.Width, DeviceHelper.GetScaledSizeByHeight(25.5f)));

            imgViewHeader.Image = UIImage.FromBundle(GetBannerImage());
            //UILabel lblTitle = new UILabel(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 36));
            UILabel lblTitle = new UILabel(new CGRect(18, DeviceHelper.GetScaledSizeByHeight(40.5f)
                , View.Frame.Width - 36, 36));

            lblTitle.Text = NotificationInfo.Title;
            lblTitle.Font = MyTNBFont.MuseoSans16_500;
            lblTitle.Lines = 0;
            lblTitle.LineBreakMode = UILineBreakMode.WordWrap;
            lblTitle.TextColor = MyTNBColor.PowerBlue;

            CGSize newTitleSize = GetLabelSize(lblTitle, lblTitle.Frame.Width, 100f);
            lblTitle.Frame = new CGRect(lblTitle.Frame.X, lblTitle.Frame.Y, lblTitle.Frame.Width, newTitleSize.Height);

            var lblDetailsHeight = 0f;
            if (NotificationInfo.BCRMNotificationType != Enums.BCRMNotificationEnum.Maintenance
                && NotificationInfo.BCRMNotificationType != Enums.BCRMNotificationEnum.News)
            {
                lblDetailsHeight = DeviceHelper.GetScaledHeight(240) - 48;
            }
            else
            {
                lblDetailsHeight = DeviceHelper.GetScaledHeight(270);
            }
            UITextView txtDetails = new UITextView(new CGRect(18
                , DeviceHelper.GetScaledSizeByHeight(4.2f) + lblTitle.Frame.Y + lblTitle.Frame.Height
                , View.Frame.Width - 36, lblDetailsHeight))
            {

                Text = NotificationInfo.Message,
                Font = MyTNBFont.MuseoSans14_300,
                Editable = false,
                ScrollEnabled = true,
                TextColor = MyTNBColor.TunaGrey()
            };

            View.AddSubviews(new UIView[] { imgViewHeader, lblTitle, txtDetails });
        }

        /// <summary>
        /// Sets the sub views for normal notification.
        /// </summary>
        internal void SetSubViewsForNormalNotification()
        {
            _viewCTA = new UIView(new CGRect(0, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                ? 106 : DeviceHelper.GetScaledHeight(82)), View.Frame.Width, DeviceHelper.GetScaledHeight(82)));

            nfloat buttonWidth = (_viewCTA.Frame.Width / 2) - 20;
            //Create CTA
            UIButton btnViewDetails = new UIButton(UIButtonType.Custom);
            btnViewDetails.Frame = new CGRect(18, 17, buttonWidth, 48);
            btnViewDetails.Layer.BorderWidth = 1.0f;
            btnViewDetails.Layer.CornerRadius = 4;
            btnViewDetails.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnViewDetails.SetTitle("PushNotification_ViewDetails".Translate(), UIControlState.Normal);
            btnViewDetails.Font = MyTNBFont.MuseoSans16_500;
            btnViewDetails.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);

            btnViewDetails.TouchUpInside += (sender, e) =>
            {
                UpdateAccountCache();
                OnCTAClick("BillViewController");
            };
            bool isEnabled = false;
            if (_dueAmount != null && _dueAmount?.d != null
                && _dueAmount?.d?.data != null
                && _dueAmount?.d?.didSucceed == true)
            {
                isEnabled = _dueAmount.d.data.amountDue > 0;
            }
            UIButton btnPay = new UIButton(UIButtonType.Custom);
            btnPay.Frame = new CGRect((_viewCTA.Frame.Width / 2 + 2), 17, buttonWidth, 48);
            btnPay.Layer.CornerRadius = 4;
            btnPay.Layer.BackgroundColor = isEnabled ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
            btnPay.Layer.BorderColor = isEnabled ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
            btnPay.Layer.BorderWidth = 1;
            btnPay.SetTitle("Common_Pay".Translate(), UIControlState.Normal);
            btnPay.Font = MyTNBFont.MuseoSans16_500;
            btnPay.Enabled = isEnabled;
            btnPay.TouchUpInside += (sender, e) =>
            {
                OnCTAClick("SelectPaymentMethodViewController");
            };

            _viewCTA.AddSubviews(new UIView[] { btnViewDetails, btnPay });
            View.AddSubview(_viewCTA);
        }
        /// <summary>
        /// Sets the sub view for promotion notification.
        /// </summary>
        internal void SetSubViewForPromotionNotification()
        {
            _viewCTA = new UIView(new CGRect(0, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                ? 106 : DeviceHelper.GetScaledHeight(82)), View.Frame.Width, DeviceHelper.GetScaledHeight(82)));

            nfloat buttonWidth = _viewCTA.Frame.Width - 36;
            //Create CTA
            UIButton btnViewPromotion = new UIButton(UIButtonType.Custom);
            btnViewPromotion.Frame = new CGRect(18, 17, buttonWidth, DeviceHelper.GetScaledHeight(48));
            btnViewPromotion.Layer.BorderWidth = 1.0f;
            btnViewPromotion.Layer.CornerRadius = 4;
            btnViewPromotion.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnViewPromotion.SetTitle("PushNotification_ViewPromotion".Translate(), UIControlState.Normal);
            btnViewPromotion.Font = MyTNBFont.MuseoSans16_500;
            btnViewPromotion.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);

            btnViewPromotion.TouchUpInside += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(NotificationInfo.Target))
                {
                    var entity = PromotionsEntity.GetItem(NotificationInfo.Target);
                    if (entity != null)
                    {
                        NavigateToPromotionDetail(entity);
                    }
                    else
                    {
                        NavigateToPromotionTab();
                    }
                }
                else
                {
                    NavigateToPromotionTab();
                }
            };

            _viewCTA.AddSubview(btnViewPromotion);
            View.AddSubview(_viewCTA);
        }
        /// <summary>
        /// Method to navigates to promotion detail when tapping View Promotion button
        /// </summary>
        /// <param name="promotion">Promotion.</param>
        internal void NavigateToPromotionDetail(PromotionsModelV2 promotion)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("PromotionDetails", null);
            PromotionDetailsViewController viewController =
                storyBoard.InstantiateViewController("PromotionDetailsViewController") as PromotionDetailsViewController;
            if (viewController != null)
            {
                viewController.Promotion = promotion;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }
        }
        /// <summary>
        /// Method to navigate to promotion tab
        /// </summary>
        internal void NavigateToPromotionTab()
        {
            var vc = this.PresentingViewController;
            if (vc is HomeTabBarController tabBar)
            {
                tabBar.SelectedIndex = 2;
                tabBar.DismissViewController(true, null);
            }
        }

        string GetBannerImage()
        {
            if (NotificationInfo.BCRMNotificationTypeId.Equals("01"))
            {
                return "Notification-Banner-New-Bill";
            }
            else if (NotificationInfo.BCRMNotificationTypeId.Equals("02"))
            {
                return "Notification-Banner-Bill-Due";
            }
            else if (NotificationInfo.BCRMNotificationTypeId.Equals("03"))
            {
                return "Notification-Banner-Dunning";
            }
            else if (NotificationInfo.BCRMNotificationTypeId.Equals("04"))
            {
                return "Notification-Banner-Disconnection";
            }
            else if (NotificationInfo.BCRMNotificationTypeId.Equals("05"))
            {
                return "Notification-Banner-Reconnection";
            }
            else if (NotificationInfo.BCRMNotificationTypeId.Equals("97"))
            {
                return "Notification-Banner-Promotion";
            }
            else if (NotificationInfo.BCRMNotificationTypeId.Equals("98"))
            {
                return "Notification-Banner-News";
            }
            else if (NotificationInfo.BCRMNotificationTypeId.Equals("99"))
            {
                return "Notification-Banner-Maintenance";
            }
            else
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Updates the account cache.
        /// </summary>
        private void UpdateAccountCache()
        {
            if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.NewBill)
            {
                DataManager.DataManager.SharedInstance.SetChartRefreshStatus(NotificationInfo.AccountNum, true);
                DataManager.DataManager.SharedInstance.SetBillHistoryRefreshStatus(NotificationInfo.AccountNum, true);
                var index = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindIndex(x => x.accNum == NotificationInfo.AccountNum);
                if (index > -1)
                {
                    var acct = DataManager.DataManager.SharedInstance.AccountRecordsList.d[(int)index];
                    if (!acct.IsREAccount)
                    {
                        DataManager.DataManager.SharedInstance.SetBillingRefreshStatus(NotificationInfo.AccountNum, true);
                    }
                }
            }
        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        internal Task DeleteUserNotification(string id)
        {
            var user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    ApiKeyID = TNBGlobal.API_KEY_ID,
                    UpdatedNotifications = new List<UpdateNotificationModel>(){
                        new UpdateNotificationModel()
                        {
                            NotificationType = NotificationInfo.NotificationType,
                            NotificationId = id
                        }
                    },
                    Email = user?.email,
                    DeviceId = DataManager.DataManager.SharedInstance.UDID,
                    SSPUserId = user?.userID
                };
                _deleteNotificationResponse = serviceManager.DeleteUserNotification("DeleteUserNotification_V3", requestParameter);
            });
        }

        internal void DisplayPage(string storyboardID, string vc)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardID, null);
            if (vc == "BillViewController")
            {
                BillViewController billVC = storyBoard.InstantiateViewController(vc) as BillViewController;
                if (billVC != null)
                {
                    billVC.NotificationInfo = NotificationInfo;
                    billVC.IsFromNavigation = true;
                    NavigationController?.PushViewController(billVC, true);
                }
            }
            else
            {
                SelectBillsViewController selectBillsVC =
                    storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                if (selectBillsVC != null)
                {
                    selectBillsVC.SelectedAccountDueAmount = (double)(_dueAmount != null && _dueAmount?.d != null
                        && _dueAmount?.d?.data != null && _dueAmount?.d?.didSucceed == true ? _dueAmount?.d?.data.amountDue : 0);
                    var navController = new UINavigationController(selectBillsVC);
                    PresentViewController(navController, true, null);
                }
            }
        }
    }
}