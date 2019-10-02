using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.More.MyAccount;
using myTNB.Registration.CustomerAccounts;

namespace myTNB
{
    public partial class MyAccountViewController : UIViewController
    {
        public MyAccountViewController(IntPtr handle) : base(handle)
        {
        }

        UIView _viewNotificationMsg;
        UILabel _lblNotificationDetails;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            myAccountTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64, View.Frame.Width, View.Frame.Height - 64);
            myAccountTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ActivityIndicator.Show();
            myAccountTableView.Source = new MyAccountDataSource(this);
            myAccountTableView.ReloadData();
            SetFooterView();
            InitializeNotificationMessage();
            if (DataManager.DataManager.SharedInstance.IsMobileNumberUpdated)
            {
                _lblNotificationDetails.Text = "MyAccount_UpdatedMobileNumber".Translate();
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsMobileNumberUpdated = false;
            }
            if (DataManager.DataManager.SharedInstance.IsAccountDeleted)
            {
                _lblNotificationDetails.Text = "MyAccount_UpdatedTNBAccount".Translate();
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsAccountDeleted = false;
            }
            if (DataManager.DataManager.SharedInstance.IsPasswordUpdated)
            {
                _lblNotificationDetails.Text = "MyAccount_UpdatedPassword".Translate();
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsPasswordUpdated = false;
            }
            ActivityIndicator.Hide();
        }

        internal void SetFooterView()
        {
            UIButton btnLogout = new UIButton(UIButtonType.Custom);
            btnLogout.Frame = new CGRect(18, 16, View.Frame.Width - 36, 48);
            btnLogout.Layer.CornerRadius = 4;
            btnLogout.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnLogout.BackgroundColor = MyTNBColor.FreshGreen;
            btnLogout.Layer.BorderWidth = 1;
            btnLogout.SetTitle("MyAccount_Logout".Translate(), UIControlState.Normal);
            btnLogout.Font = MyTNBFont.MuseoSans16;
            btnLogout.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnLogout.TouchUpInside += (sender, e) =>
            {
                UIAlertController alert = UIAlertController.Create("MyAccount_Logout".Translate()
                    , "MyAccount_LogoutConfirmation".Translate(), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Common_Ok".Translate(), UIAlertActionStyle.Default, (obj) =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Logout", null);
                    LogoutViewController viewController =
                        storyBoard.InstantiateViewController("LogoutViewController") as LogoutViewController;
                    var navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }));
                alert.AddAction(UIAlertAction.Create("Common_Cancel".Translate(), UIAlertActionStyle.Cancel, null));
                alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(alert, animated: true, completionHandler: null);
            };

            UIView viewLogout = new UIView();
            viewLogout.BackgroundColor = MyTNBColor.SectionGrey;
            viewLogout.AddSubview(btnLogout);

            UIView viewFooter = new UIView();
            UIButton btnAddAccount = new UIButton(UIButtonType.Custom);
            btnAddAccount.Frame = new CGRect(18, 16, myAccountTableView.Frame.Width - 36, 48);
            btnAddAccount.Layer.CornerRadius = 4;
            btnAddAccount.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnAddAccount.BackgroundColor = UIColor.White;
            btnAddAccount.Layer.BorderWidth = 1;
            btnAddAccount.SetTitle("Common_AddAnotherAccount".Translate(), UIControlState.Normal);
            btnAddAccount.Font = MyTNBFont.MuseoSans16;
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
                            AlertHandler.DisplayNoDataAlert(this);
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
                UILabel lblTitle = new UILabel(new CGRect(93, 16, myAccountTableView.Frame.Width - 186, 16));
                lblTitle.TextColor = MyTNBColor.TunaGrey();
                lblTitle.Font = MyTNBFont.MuseoSans12_500;
                lblTitle.Text = "Common_NoAccount".Translate();
                lblTitle.TextAlignment = UITextAlignment.Center;

                UILabel lblDetails = new UILabel(new CGRect(0, 32, myAccountTableView.Frame.Width, 36));
                lblDetails.TextColor = MyTNBColor.TunaGrey();
                lblDetails.Font = MyTNBFont.MuseoSans9_300;
                lblDetails.Text = "MyAccount_AddAccountDetails".Translate();
                lblDetails.Lines = 0;
                lblDetails.LineBreakMode = UILineBreakMode.WordWrap;
                lblDetails.TextAlignment = UITextAlignment.Center;

                btnAddAccount.Frame = new CGRect(90, 76, myAccountTableView.Frame.Width - 180, 48);
                btnAddAccount.SetTitle("Common_AddAccount".Translate(), UIControlState.Normal);
                viewLogout.Frame = new CGRect(0, 140, View.Frame.Width, 88);
                viewFooter.AddSubviews(new UIView[] { lblTitle, lblDetails, btnAddAccount, viewLogout });
            }
            myAccountTableView.TableFooterView = viewFooter;
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("MyAccount_Title".Translate());
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
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        internal void UpdatePassword()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdatePassword", null);
            UpdatePasswordViewController viewController =
                storyBoard.InstantiateViewController("UpdatePasswordViewController") as UpdatePasswordViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
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
                        UINavigationController navController = new UINavigationController(viewController);
                        navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(navController, true, null);
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
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
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
            ActivityIndicator.Hide();
        }

        internal void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64));
                _viewNotificationMsg.BackgroundColor = MyTNBColor.SunGlow;
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;
                _viewNotificationMsg.Hidden = true;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32));
                _lblNotificationDetails.TextAlignment = UITextAlignment.Left;
                _lblNotificationDetails.Font = MyTNBFont.MuseoSans12;
                _lblNotificationDetails.TextColor = MyTNBColor.TunaGrey();
                _lblNotificationDetails.Text = TNBGlobal.EMPTY_ADDRESS;
                _lblNotificationDetails.Lines = 0;
                _lblNotificationDetails.LineBreakMode = UILineBreakMode.WordWrap;

                _viewNotificationMsg.AddSubview(_lblNotificationDetails);

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(_viewNotificationMsg);
            }
        }

        internal void ShowNotificationMessage()
        {
            _viewNotificationMsg.Hidden = false;
            _viewNotificationMsg.Alpha = 1.0f;
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewNotificationMsg.Alpha = 0.0f;
            }, () =>
            {
                _viewNotificationMsg.Hidden = true;
            });
        }
    }
}