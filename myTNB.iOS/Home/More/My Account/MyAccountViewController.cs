using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.More.MyAccount;
using myTNB.Registration.CustomerAccounts;
using myTNB.MyAccount;

namespace myTNB
{
    public partial class MyAccountViewController : CustomUIViewController
    {
        public MyAccountViewController(IntPtr handle) : base(handle) { }

        private UIView _viewNotificationMsg;
        private UILabel _lblNotificationDetails;

        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_MyAccount;
            base.ViewDidLoad();
            SetNavigationBar();
            myAccountTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution()
                ? 88 : 64, View.Frame.Width, View.Frame.Height - 64);
            myAccountTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //ActivityIndicator.Show();
            myAccountTableView.Source = new MyAccountDataSource(this);
            myAccountTableView.ReloadData();
            //SetFooterView();
            InitializeNotificationMessage();
            if (DataManager.DataManager.SharedInstance.IsMobileNumberUpdated)
            {
                _lblNotificationDetails.Text = GetI18NValue(MyAccountConstants.I18N_MobileNumberVerified);
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsMobileNumberUpdated = false;
            }
            if (DataManager.DataManager.SharedInstance.IsAccountDeleted)
            {
                _lblNotificationDetails.Text = GetI18NValue(MyAccountConstants.I18N_AccountDeleteSuccess);
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsAccountDeleted = false;
            }
            if (DataManager.DataManager.SharedInstance.IsPasswordUpdated)
            {
                _lblNotificationDetails.Text = GetI18NValue(MyAccountConstants.I18N_PasswordUpdateSuccess);
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsPasswordUpdated = false;
            }
           // ActivityIndicator.Hide();
        }

        private void SetFooterView()
        {
            UIButton btnLogout = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, 16, View.Frame.Width - 36, 48),
                BackgroundColor = MyTNBColor.FreshGreen,
                Font = MyTNBFont.MuseoSans16
            };

            btnLogout.Layer.CornerRadius = 4;
            btnLogout.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnLogout.Layer.BorderWidth = 1;
            btnLogout.SetTitle(GetCommonI18NValue(Constants.Common_Logout), UIControlState.Normal);
            btnLogout.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnLogout.TouchUpInside += (sender, e) =>
            {
                UIAlertController alert = UIAlertController.Create(GetI18NValue(MyAccountConstants.I18N_Logout)
                    , GetI18NValue(MyAccountConstants.I18N_LogoutMessage), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Ok), UIAlertActionStyle.Default, (obj) =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Logout", null);
                    LogoutViewController viewController =
                        storyBoard.InstantiateViewController("LogoutViewController") as LogoutViewController;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }));
                alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, null));
                alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(alert, animated: true, completionHandler: null);
            };

            UIView viewLogout = new UIView
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };
            viewLogout.AddSubview(btnLogout);

            UIView viewFooter = new UIView();
            UIButton btnAddAccount = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, 16, myAccountTableView.Frame.Width - 36, 48),
                Font = MyTNBFont.MuseoSans16,
                BackgroundColor = UIColor.White
            };

            btnAddAccount.Layer.CornerRadius = 4;
            btnAddAccount.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnAddAccount.Layer.BorderWidth = 1;
            btnAddAccount.SetTitle(GetCommonI18NValue(Constants.Common_AddAnotherAccount), UIControlState.Normal);
            btnAddAccount.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            btnAddAccount.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
                            AccountsViewController viewController = storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                            viewController.isDashboardFlow = true;
                            viewController._needsUpdate = true;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0)
            {
                viewLogout.Frame = new CGRect(0, 80, View.Frame.Width, 88);
                viewFooter.Frame = new CGRect(0, 0, myAccountTableView.Frame.Width, 168);
                viewFooter.AddSubviews(new UIView[] { btnAddAccount, viewLogout });
            }
            else
            {
                viewFooter = new UIView(new CGRect(0, 0, myAccountTableView.Frame.Width, 150));
                viewFooter.Frame = new CGRect(0, 0, myAccountTableView.Frame.Width, 230);
                UILabel lblTitle = new UILabel(new CGRect(93, 16, myAccountTableView.Frame.Width - 186, 16))
                {
                    TextColor = MyTNBColor.TunaGrey(),
                    Font = MyTNBFont.MuseoSans12_500,
                    Text = GetI18NValue(MyAccountConstants.I18N_NoAccounts),
                    TextAlignment = UITextAlignment.Center
                };

                UILabel lblDetails = new UILabel(new CGRect(18, 32, myAccountTableView.Frame.Width - 36, 36))
                {
                    TextColor = MyTNBColor.TunaGrey(),
                    Font = MyTNBFont.MuseoSans9_300,
                    Text = GetI18NValue(MyAccountConstants.I18N_AddAccountMessage),
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    TextAlignment = UITextAlignment.Center
                };

                btnAddAccount.Frame = new CGRect(18, 76, myAccountTableView.Frame.Width - 36, 48);
                btnAddAccount.SetTitle(GetCommonI18NValue(Constants.Common_AddAnotherAccount), UIControlState.Normal);
                viewLogout.Frame = new CGRect(0, 140, View.Frame.Width, 88);
                viewFooter.AddSubviews(new UIView[] { lblTitle, lblDetails, btnAddAccount, viewLogout });
            }
            myAccountTableView.TableFooterView = viewFooter;
        }

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(MyAccountConstants.I18N_Title));
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void UpdateMobileNumber()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdateMobileNumber", null);
            UpdateMobileNumberViewController viewController =
                storyBoard.InstantiateViewController("UpdateMobileNumberViewController") as UpdateMobileNumberViewController;
            UINavigationController navController = new UINavigationController(viewController)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
            };
            PresentViewController(navController, true, null);
        }

        internal void UpdatePassword()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdatePassword", null);
            UpdatePasswordViewController viewController =
                storyBoard.InstantiateViewController("UpdatePasswordViewController") as UpdatePasswordViewController;
            UINavigationController navController = new UINavigationController(viewController)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
            };
            PresentViewController(navController, true, null);
        }

        internal void ManageRegisteredCards()
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("ManageCards", null);
                        ManageCardViewController viewController =
                            storyBoard.InstantiateViewController("ManageCardViewController") as ManageCardViewController;
                        UINavigationController navController = new UINavigationController(viewController)
                        {
                            ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                        };
                        PresentViewController(navController, true, null);
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        internal void ManageSupplyAccount(int accountRecordIndex)
        {
            ActivityIndicator.Show();
            UIStoryboard storyBoard = UIStoryboard.FromName("ManageAccounts", null);
            ManageAccountsViewController viewController =
                storyBoard.InstantiateViewController("ManageAccountsViewController") as ManageAccountsViewController;
            //viewController.AccountRecordIndex = accountRecordIndex;
            DataManager.DataManager.SharedInstance.AccountRecordIndex = accountRecordIndex;
            UINavigationController navController = new UINavigationController(viewController)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
            };
            PresentViewController(navController, true, null);
            ActivityIndicator.Hide();
        }

        private void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64))
                {
                    BackgroundColor = MyTNBColor.SunGlow,
                    Hidden = true
                };
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = TNBGlobal.EMPTY_ADDRESS,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };

                _viewNotificationMsg.AddSubview(_lblNotificationDetails);

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(_viewNotificationMsg);
            }
        }

        private void ShowNotificationMessage()
        {
            _viewNotificationMsg.Hidden = false;
            _viewNotificationMsg.Alpha = 1.0f;
            UIView.Animate(2, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewNotificationMsg.Alpha = 0.0f;
            }, () =>
            {
                _viewNotificationMsg.Hidden = true;
            });
        }
    }
}