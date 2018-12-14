using Foundation;
using System;
using UIKit;
using CoreAnimation;
using myTNB.Registration;
using CoreGraphics;
using myTNB.Model;
using myTNB.SQLite.SQLiteDataManager;
using System.Collections.Generic;

namespace myTNB
{
    public partial class AddAccountSuccessViewController : UIViewController
    {
        public AddAccountSuccessViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupSuperViewBackground();

            AccountsTableView.Source = new AddAccountSuccessDataSource();
            AccountsTableView.Layer.CornerRadius = 4f;
            AccountsTableView.RowHeight = 101f;
            AccountsTableView.BackgroundColor = UIColor.Clear;

            var headerView = new UIView((new CGRect(16, 16, View.Frame.Width - 32, 190)));
            headerView.BackgroundColor = UIColor.White;
            UIView viewClose = new UIView(new CGRect(headerView.Frame.Width - 59, 18, 25, 25));

            var imgViewClose = new UIImageView(UIImage.FromBundle("Delete"))
            {
                Frame = new CGRect(0, 0, 25, 25),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            viewClose.AddSubview(imgViewClose);
            viewClose.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                GetStarted();
            }));

            var imgViewSuccess = new UIImageView(UIImage.FromBundle("Circle-With-Check-Green"))
            {
                Frame = new CGRect(headerView.Frame.Width / 2 - 25, 49, 50, 50),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            var lblPasswordSuccess = new UILabel
            {
                Frame = new CGRect(18, 109, headerView.Frame.Width - 36, 18),
                AttributedText = new NSAttributedString(
                    "Add Accounts Successful"
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.PowerBlue()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Center,
            };

            UIView viewLine = new UIView((new CGRect(16, 176, headerView.Frame.Width - 32, 1)));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();

            headerView.AddSubview(viewClose);
            headerView.AddSubview(imgViewSuccess);
            headerView.AddSubview(lblPasswordSuccess);
            headerView.AddSubview(viewLine);

            AccountsTableView.TableHeaderView = headerView;

            var footerView = new UIView((new CGRect(0, 16, View.Frame.Width - 32, 84)));
            footerView.Layer.CornerRadius = 4.0f;
            footerView.BackgroundColor = UIColor.Clear;

            var btnGetStarted = new UIButton(UIButtonType.Custom);
            btnGetStarted.Frame = new CGRect(0, 18, View.Frame.Width - 36, 48);
            btnGetStarted.SetTitle("Get Started", UIControlState.Normal);
            btnGetStarted.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnGetStarted.BackgroundColor = myTNBColor.FreshGreen();
            btnGetStarted.Layer.CornerRadius = 5.0f;
            footerView.AddSubview(btnGetStarted);

            btnGetStarted.TouchUpInside += (object sender, EventArgs e) =>
            {
                GetStarted();
            };

            AccountsTableView.TableFooterView = footerView;
        }

        internal void GetStarted()
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ExecuteGetCustomerRecordsCall();
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        internal void ExecuteGetCustomerRecordsCall()
        {
            if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d != null)
            {
                if(DataManager.DataManager.SharedInstance.AccountRecordsList == null
                   || DataManager.DataManager.SharedInstance.AccountRecordsList.d == null){
                    DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                    DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
                }
                foreach (var item in DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d)
                {
                    int itemIndex = DataManager.DataManager.SharedInstance.AccountRecordsList
                                               .d.FindIndex(x => x.accNum.Equals(item.accNum));
                    if(itemIndex == -1){
                        DataManager.DataManager.SharedInstance.AccountRecordsList.d.Add(item);
                    }
                }
            }
            UserAccountsEntity uaManager = new UserAccountsEntity();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                uaManager.DeleteTable();
                uaManager.CreateTable();
                uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);
            }

            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex = 0;
                DataManager.DataManager.SharedInstance.PreviousSelectedAccountIndex = 0;
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                PresentViewController(loginVC, true, null);
                ActivityIndicator.Hide();
            }
            else
            {
                var alert = UIAlertController.Create("Error in fetching account list.", "There is an error in the server, please try again.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, animated: true, completionHandler: null);
            }
            ActivityIndicator.Hide();
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = myTNBColor.GradientPurpleDarkElement();
            var endColor = myTNBColor.GradientPurpleLightElement();
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = View.Bounds;
            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        internal void ShowPrelogin()
        {
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preloginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preloginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            PresentViewController(preloginVC, true, null);
        }
    }
}