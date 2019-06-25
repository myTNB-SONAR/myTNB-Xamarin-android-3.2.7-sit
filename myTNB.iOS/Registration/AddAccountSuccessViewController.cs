using Foundation;
using System;
using UIKit;
using CoreAnimation;
using myTNB.Registration;
using CoreGraphics;
using myTNB.Model;
using System.Collections.Generic;
using System.Linq;

namespace myTNB
{
    public partial class AddAccountSuccessViewController : UIViewController
    {
        public int AccountsAddedCount = 0;
        public bool IsDashboardFlow = false;
        const float TopPadding = 48f;
        const float RowHeight = 115f;
        const float HeaderViewHeight = 170f;
        CustomerAccountRecordListModel GetStartedList = new CustomerAccountRecordListModel();

        public AddAccountSuccessViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupSuperViewBackground();

            AccountsTableView.Frame = new CGRect(16, TopPadding, View.Frame.Width - 32, DeviceHelper.GetScaledHeight(HeaderViewHeight));
            AccountsTableView.Source = new AddAccountSuccessDataSource(GetStartedList);
            AccountsTableView.Layer.CornerRadius = 4f;
            AccountsTableView.RowHeight = RowHeight;
            AccountsTableView.BackgroundColor = UIColor.White;
            AccountsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            AccountsTableView.Bounces = false;

            var headerView = new UIView((new CGRect(0, 0, View.Frame.Width - 32, DeviceHelper.GetScaledHeight(HeaderViewHeight))));
            headerView.BackgroundColor = UIColor.White;

            var imgViewSuccess = new UIImageView(UIImage.FromBundle("Circle-With-Check-Green"))
            {
                Frame = new CGRect((headerView.Frame.Width / 2) - DeviceHelper.GetScaledWidth(25), 49, DeviceHelper.GetScaledWidth(50), DeviceHelper.GetScaledHeight(50)),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            var lblPasswordSuccess = new UILabel
            {
                Frame = new CGRect(DeviceHelper.GetScaledWidth(18), DeviceHelper.GetScaledHeight(109), headerView.Frame.Width - 36, 18),
                AttributedText = new NSAttributedString("Registration_AddAccountSuccessMessage".Translate()
                    , font: MyTNBFont.MuseoSans18_500
                    , foregroundColor: MyTNBColor.PowerBlue
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Center,
            };

            headerView.AddSubview(imgViewSuccess);
            headerView.AddSubview(lblPasswordSuccess);

            AccountsTableView.TableHeaderView = headerView;

            btnStart.SetTitle("Common_Done".Translate(), UIControlState.Normal);
            btnStart.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnStart.BackgroundColor = MyTNBColor.FreshGreen;
            btnStart.Font = MyTNBFont.MuseoSans16_500;

            btnStart.TouchUpInside += (object sender, EventArgs e) =>
            {
                GetStarted();
            };

            float maxTableHeight = (float)(View.Frame.Height - btnStart.Frame.Height - DeviceHelper.GetScaledHeight(TopPadding * 2));
            float tableHeight = (float)AccountsTableView.Frame.Height;
            if (GetStartedList != null && GetStartedList?.d != null && GetStartedList?.d?.Count > 0)
            {
                tableHeight += (RowHeight * GetStartedList.d.Count);
                if (tableHeight > maxTableHeight)
                {
                    tableHeight = maxTableHeight;
                }
                ViewHelper.AdjustFrameSetHeight(AccountsTableView, tableHeight);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }

        /// <summary>
        /// Creates the newly added list.
        /// </summary>
        public void CreateNewlyAddedList()
        {
            GetStartedList.d = new List<CustomerAccountRecordModel>();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null)
            {
                foreach (var item in DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d)
                {
                    int index = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == item.accNum);
                    if (index == -1)
                    {
                        GetStartedList.d.Add(item);
                    }
                }
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                   && DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d != null)
                {
                    GetStartedList = DataManager.DataManager.SharedInstance.AccountsToBeAddedList;
                }
            }
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
                        AlertHandler.DisplayNoDataAlert(this);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }


        internal void ExecuteGetCustomerRecordsCall()
        {

            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0
                    ? DataManager.DataManager.SharedInstance.AccountRecordsList.d.First()
                    : new CustomerAccountRecordModel();
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex = 0;
                DataManager.DataManager.SharedInstance.PreviousSelectedAccountIndex = 0;
                DataManager.DataManager.SharedInstance.AccountsAddedCount = AccountsAddedCount;

#if true // CREATE_TABBAR
                if (IsDashboardFlow)
                {
                    //var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    //var topVc = AppDelegate.GetTopViewController(baseRootVc);
                    ViewHelper.DismissControllersAndSelectTab(this, 0, true, true);

                    //var newtopVc = AppDelegate.GetTopViewController(baseRootVc);
                    //var newPresenting = newtopVc?.PresentingViewController;
                    //if (!(newPresenting is HomeTabBarController))
                    //{
                    //    Debug.WriteLine("newPresenting = " + newPresenting.GetType().ToString());
                    //}
                }
                else
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                    UIViewController homeVc = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                    PresentViewController(homeVc, true, null);
                }
#else
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                PresentViewController(loginVC, true, null);
#endif
                ActivityIndicator.Hide();
            }
            else
            {
                AlertHandler.DisplayServiceError(this, string.Empty);
            }
            ActivityIndicator.Hide();
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
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