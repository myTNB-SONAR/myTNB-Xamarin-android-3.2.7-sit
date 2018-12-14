using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using CoreGraphics;
using System.Drawing;
using System.Threading.Tasks;
using myTNB.PushNotification;

namespace myTNB
{
    public partial class NotificationDetailsViewController : UIViewController
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
            ActivityIndicator.Show();
            GetAccountDueAmount().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    SetSubViews();
                    ActivityIndicator.Hide();
                });
            });
            int unreadCount = PushNotificationHelper.GetNotificationCount();
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = unreadCount == 0 ? 0 : unreadCount - 1;
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle(NotificationInfo.Title);
            _titleBarComponent.SetNotificationVisibility(false);
            _titleBarComponent.SetNotificationImage("Notification-Delete");
            _titleBarComponent.SetNotificationAction(new UITapGestureRecognizer(() =>
            {
                var alert = UIAlertController.Create("Delete Notification", "Are you sure you want to delete this notification?", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) =>
                {
                    ActivityIndicator.Show();
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            Console.WriteLine("Delete Notification");
                            Task[] taskList = new Task[] { DeleteUserNotification(NotificationInfo.Id) };
                            Task.WaitAll(taskList);
                            if (_deleteNotificationResponse != null && _deleteNotificationResponse.d != null
                               && _deleteNotificationResponse.d.isError.ToLower() == "false"
                               && _deleteNotificationResponse.d.status.ToLower() == "success")
                            {
                                DataManager.DataManager.SharedInstance.IsNotificationDeleted = true;
                                PushNotificationHelper.GetNotifications();
                                UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                                PushNotificationViewController viewController =
                                    storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
                                NavigationController.PushViewController(viewController, true);
                            }
                            else
                            {
                                DisplayAlertMessage("Error", _deleteNotificationResponse.d.message);
                            }
                            ActivityIndicator.Hide();
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            DisplayAlertMessage("No Data Connection", "Please check your data connection and try again.");
                            ActivityIndicator.Hide();
                        }
                    });
                }));
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (obj) =>
                {

                }));
                PresentViewController(alert, animated: true, completionHandler: null);
            }));
            _titleBarComponent.SetBackVisibility(false);
            _titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                PushNotificationViewController viewController =
                    storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
                NavigationController.PushViewController(viewController, true);
            }));
            headerView.AddSubview(titleBarView);

            View.AddSubview(headerView);
        }

        internal void OnCTAClick(string actionString)
        {
            int index = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == NotificationInfo.AccountNum);
            if (index > -1)
            {
                ActivityIndicator.Show();
                DataManager.DataManager.SharedInstance.SelectedAccount =
                    DataManager.DataManager.SharedInstance.AccountRecordsList.d[index];
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex = index;
                DataManager.DataManager.SharedInstance.PreviousSelectedAccountIndex = index;

                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            Task[] taskList = new Task[] { GetBillingAccountDetails() };
                            Task.WaitAll(taskList);
                            if (_billingAccountDetailsList != null && _billingAccountDetailsList.d != null
                                && _billingAccountDetailsList.d.data != null)
                            {
                                DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                                string storyboardID = actionString == "BillViewController" ? "Dashboard" : "Payment";
                                DisplayPage(storyboardID, actionString);
                            }
                            else
                            {
                                DataManager.DataManager.SharedInstance.IsSameAccount = true;
                                DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                                var alert = UIAlertController.Create("Error in Response", "There is an error in the server, please try again.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                PresentViewController(alert, animated: true, completionHandler: null);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            DisplayAlertMessage("No Data Connection", "Please check your data connection and try again.");
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

        internal void SetSubViews()
        {
            UIImageView imgViewHeader = new UIImageView(new CGRect(0
                                                                   , DeviceHelper.IsIphoneX() ? 88 : 64
                                                                   , View.Frame.Width
                                                                   , DeviceHelper.GetScaledSizeByHeight(25.5f)));

            imgViewHeader.Image = UIImage.FromBundle(GetBannerImage());
            //UILabel lblTitle = new UILabel(new CGRect(18, DeviceHelper.IsIphoneX() ? 104 : 80, View.Frame.Width - 36, 36));
            UILabel lblTitle = new UILabel(new CGRect(18
                                                      , DeviceHelper.GetScaledSizeByHeight(40.5f)
                                                      , View.Frame.Width - 36
                                                      , 36));

            lblTitle.Text = NotificationInfo.Title;
            lblTitle.Font = myTNBFont.MuseoSans16();
            lblTitle.Lines = 0;
            lblTitle.LineBreakMode = UILineBreakMode.WordWrap;
            lblTitle.TextColor = myTNBColor.PowerBlue();

            CGSize newTitleSize = GetLabelSize(lblTitle, lblTitle.Frame.Width, 100f);
            lblTitle.Frame = new CGRect(lblTitle.Frame.X, lblTitle.Frame.Y, lblTitle.Frame.Width, newTitleSize.Height);

            //UILabel lblDetails = new UILabel(new CGRect(18, (DeviceHelper.IsIphoneX() ? 120 :96) + newTitleSize.Height, View.Frame.Width - 36, 36));
            UILabel lblDetails = new UILabel(new CGRect(18
                                                        , DeviceHelper.GetScaledSizeByHeight(4.2f)
                                                        + lblTitle.Frame.Y
                                                        + lblTitle.Frame.Height
                                                        , View.Frame.Width - 36
                                                        , 36));
            lblDetails.Text = NotificationInfo.Message;
            lblDetails.Font = myTNBFont.MuseoSans14();
            lblDetails.Lines = 0;
            lblDetails.LineBreakMode = UILineBreakMode.WordWrap;
            lblDetails.TextColor = myTNBColor.TunaGrey();

            CGSize newDetailsSize = GetLabelSize(lblDetails, lblDetails.Frame.Width, 100f);
            lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y, lblDetails.Frame.Width, newDetailsSize.Height);

            _viewCTA = new UIView(new CGRect(0, View.Frame.Height - (DeviceHelper.IsIphoneX() ? 106 : 82), View.Frame.Width, 82));

            nfloat buttonWidth = (_viewCTA.Frame.Width / 2) - 20;
            //Create CTA
            UIButton btnViewDetails = new UIButton(UIButtonType.Custom);
            btnViewDetails.Frame = new CGRect(18, 17, buttonWidth, 48);
            btnViewDetails.Layer.BorderWidth = 1.0f;
            btnViewDetails.Layer.CornerRadius = 4;
            btnViewDetails.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            btnViewDetails.SetTitle("View Details", UIControlState.Normal);
            btnViewDetails.Font = myTNBFont.MuseoSans16();
            btnViewDetails.SetTitleColor(myTNBColor.FreshGreen(), UIControlState.Normal);

            btnViewDetails.TouchUpInside += (sender, e) =>
            {
                OnCTAClick("BillViewController");
            };
            bool isEnabled = false;
            if (_dueAmount != null && _dueAmount.d != null
               && _dueAmount.d.data != null)
            {
                isEnabled = _dueAmount.d.data.amountDue > 0;
            }
            UIButton btnPay = new UIButton(UIButtonType.Custom);
            btnPay.Frame = new CGRect((_viewCTA.Frame.Width / 2 + 2), 17, buttonWidth, 48);
            btnPay.Layer.CornerRadius = 4;
            btnPay.Layer.BackgroundColor = isEnabled ? myTNBColor.FreshGreen().CGColor : myTNBColor.SilverChalice().CGColor;
            btnPay.Layer.BorderColor = isEnabled ? myTNBColor.FreshGreen().CGColor : myTNBColor.SilverChalice().CGColor;
            btnPay.Layer.BorderWidth = 1;
            btnPay.SetTitle("Pay", UIControlState.Normal);
            btnPay.Font = myTNBFont.MuseoSans16();
            btnPay.Enabled = isEnabled;
            btnPay.TouchUpInside += (sender, e) =>
            {
                OnCTAClick("SelectPaymentMethodViewController");
            };

            _viewCTA.AddSubviews(new UIView[] { btnViewDetails, btnPay });
            View.AddSubviews(new UIView[] { imgViewHeader, lblTitle, lblDetails, _viewCTA });
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
            else
            {
                return string.Empty;
            }

        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        internal Task DeleteUserNotification(string id)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    id = id
                };
                _deleteNotificationResponse = serviceManager.DeleteUserNotification("DeleteUserNotification", requestParameter);
            });
        }

        internal void DisplayAlertMessage(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        internal void DisplayPage(string storyboardID, string vc)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardID, null);
            if (vc == "BillViewController")
            {
                BillViewController billVC = storyBoard.InstantiateViewController(vc) as BillViewController;
                billVC.NotificationInfo = NotificationInfo;
                billVC.IsFromNavigation = true;
                NavigationController.PushViewController(billVC, true);
            }
            else
            {
                SelectBillsViewController selectBillsVC =
                    storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                selectBillsVC.SelectedAccountDueAmount = _dueAmount != null && _dueAmount.d != null
                    && _dueAmount.d.data != null ? _dueAmount.d.data.amountDue : 0;
                var navController = new UINavigationController(selectBillsVC);
                PresentViewController(navController, true, null);
            }
        }
    }
}