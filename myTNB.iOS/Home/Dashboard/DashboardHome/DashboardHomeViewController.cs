using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.PushNotification;
using UIKit;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();

        private UITableView _homeTableView;
        UIPageViewController _accountsPageViewController;
        private DashboardHomeHeader _dashboardHomeHeader;
        private nfloat _previousScrollOffset;
        private nfloat _imageGradientHeight;
        UITapGestureRecognizer _tapGestureAddAccount;
        UITapGestureRecognizer _tapGestureSearch;
        UIView _headerView;

        UIView _textFieldView;
        UITextField _textFieldSearch;
        TextFieldHelper _textFieldHelper = new TextFieldHelper();

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }
            PageName = DashboardHomeConstants.PageName;
            IsGradientImageRequired = true;
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"NotificationDidChange", NotificationDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"OnReceiveNotificationFromDashboard", NotificationDidChange);
            _imageGradientHeight = IsGradientImageRequired ? ImageViewGradientImage.Frame.Height : 0;

            SetStatusBarNoOverlap();
            SetTapGestureRecognizers();
            AddTableView();
            AddTableViewHeader();
            _dashboardHomeHelper.GroupAccountsList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
            InitializePageView();
            InitializeTableView();
            OnUpdateNotification();

            _textFieldView = new UIView(new CGRect(16f, DeviceHelper.GetStatusBarHeight(), View.Frame.Width - 32f, 24f))
            {
                BackgroundColor = UIColor.White
            };
            _textFieldView.Layer.CornerRadius = 12f;
            _textFieldSearch = new UITextField(new CGRect(12f, 0, View.Frame.Width - 24f - 24d / 2, 24f))
            {
                AttributedPlaceholder = new NSAttributedString(
                   "Search by account nickname or number"
                   , font: MyTNBFont.MuseoSans12_500
                   , foregroundColor: MyTNBColor.WhiteTwo
                   , strokeWidth: 0
               ),
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_500
            };
            SetTextFieldEvents(_textFieldSearch);
            _textFieldView.AddSubview(_textFieldSearch);
            View.AddSubview(_textFieldView);
        }

        private void SetTextFieldEvents(UITextField textField)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                Debug.WriteLine("textField*** " + textField.Text);
                SearchFromAccountList(textField.Text);
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
        }

        private void SearchFromAccountList(string searchString)
        {
            var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
            var searchResults = accountsList.FindAll(x => x.accountNickName.Contains(searchString) || x.accNum.Contains(searchString));
            ResetAccountCardsView(searchResults);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (DataManager.DataManager.SharedInstance.SummaryNeedsRefresh)
            {
                ResetAccountCardsView(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
                DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = false;
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void SetStatusBarNoOverlap()
        {
            base.SetStatusBarNoOverlap();
            _statusBarView.BackgroundColor = MyTNBColor.ClearBlue;
            _statusBarView.Hidden = true;
        }

        private void NotificationDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD NotificationDidChange");
            if (_dashboardHomeHeader != null)
            {
                _dashboardHomeHeader.SetNotificationImage(PushNotificationHelper.GetNotificationImage());
            }
            PushNotificationHelper.UpdateApplicationBadge();
        }

        private void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD LanguageDidChange");
        }

        private void ResetAccountCardsView(List<CustomerAccountRecordModel> accountsList)
        {
            DataManager.DataManager.SharedInstance.AccountsGroupList.Clear();
            _dashboardHomeHelper.GroupAccountsList(accountsList);
            if (_accountsPageViewController != null)
            {
                _accountsPageViewController.View.RemoveFromSuperview();
                InitializePageView();
            }
            InitializeTableView();
        }

        // <summary>
        // Initializes the table view.
        // </summary>
        private void InitializeTableView()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsPageViewController, _headerView);
            _homeTableView.ReloadData();
        }

        private void InitializePageView()
        {
            _accountsPageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min)
            {
                WeakDelegate = this
            };
            _accountsPageViewController.DataSource = new AccountsPageViewDataSource(this, DataManager.DataManager.SharedInstance.AccountsGroupList);

            var startingViewController = ViewControllerAtIndex(0) as AccountsContentViewController;
            var viewControllers = new UIViewController[] { startingViewController };

            _accountsPageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
            _accountsPageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, _dashboardHomeHelper.GetHeightForAccountCards());
            _accountsPageViewController.View.BackgroundColor = UIColor.Clear;
        }

        private void AddTableView()
        {
            nfloat tabbarHeight = TabBarController.TabBar.Frame.Height + 20.0F;
            _homeTableView = new UITableView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + 30f, View.Frame.Width
                , View.Frame.Height - DeviceHelper.GetStatusBarHeight() - tabbarHeight))
            { BackgroundColor = UIColor.Clear };
            _homeTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _homeTableView.RowHeight = UITableView.AutomaticDimension;
            _homeTableView.EstimatedRowHeight = 600.0F;
            _homeTableView.RegisterClassForCellReuse(typeof(AccountsTableViewCell), DashboardHomeConstants.Cell_Accounts);
            _homeTableView.RegisterClassForCellReuse(typeof(HelpTableViewCell), DashboardHomeConstants.Cell_Help);
            _homeTableView.RegisterClassForCellReuse(typeof(ServicesTableViewCell), DashboardHomeConstants.Cell_Services);
            View.AddSubview(_homeTableView);
        }

        private void AddTableViewHeader()
        {
            _dashboardHomeHeader = new DashboardHomeHeader(View);
            _dashboardHomeHeader.SetGreetingText(GetGreeting());
            _dashboardHomeHeader.SetNameText(_dashboardHomeHelper.GetDisplayName());
            _headerView = _dashboardHomeHeader.GetUI();
            _dashboardHomeHeader.AddNotificationAction(OnNotificationAction);
            _dashboardHomeHeader.SetAddAccountAction(_tapGestureAddAccount);
            _dashboardHomeHeader.SetSearchAction(_tapGestureSearch);
        }

        private void SetTapGestureRecognizers()
        {
            _tapGestureAddAccount = new UITapGestureRecognizer(() =>
            {
                OnAddAccountAction();
            });
            _tapGestureSearch = new UITapGestureRecognizer(() =>
            {
                OnSearchAction();
            });
        }

        private void OnNotificationAction()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
            PushNotificationViewController viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
            UINavigationController navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        private void OnAddAccountAction()
        {
            Debug.WriteLine("OnAddAccountAction");
        }

        private void OnSearchAction()
        {
            Debug.WriteLine("OnSearchAction");
        }

        private string GetGreeting()
        {
            DateTime now = DateTime.Now;
            string key = DashboardHomeConstants.I18N_Evening;
            if (now.Hour < 12)
            {
                key = DashboardHomeConstants.I18N_Morning;
            }
            else if (now.Hour < 18)
            {
                key = DashboardHomeConstants.I18N_Afternoon;
            }
            return I18NDictionary[key];
        }

        public UIViewController ViewControllerAtIndex(int index)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            var vc = storyBoard.InstantiateViewController("AccountsContentViewController") as AccountsContentViewController;
            vc.pageIndex = index;
            vc._groupAccountList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            return vc;
        }

        private void OnUpdateNotification()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.IsLoadingFromDashboard = true;
                        await PushNotificationHelper.GetNotifications();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("OnReceiveNotificationFromDashboard", new NSObject());
                    }
                    else
                    {
                        //Todo: user don't need to see no data connection?
                        Debug.WriteLine("No Data connection");
                    }
                });
            });
        }

        internal void OnTableViewScroll(UIScrollView scrollView)
        {
            if (ImageViewGradientImage == null) { return; }
            nfloat scrollDiff = scrollView.ContentOffset.Y - _previousScrollOffset;
            if (scrollDiff < 0 || (scrollDiff > (scrollView.ContentSize.Height - scrollView.Frame.Size.Height))) { return; }
            _previousScrollOffset = tableViewAccounts.ContentOffset.Y;
            CGRect frame = ImageViewGradientImage.Frame;
            frame.Y = scrollDiff > 0 ? 0 - scrollDiff : frame.Y + scrollDiff;
            ImageViewGradientImage.Frame = frame;
            _statusBarView.Hidden = !(scrollDiff > 0 && scrollDiff > _imageGradientHeight / 2);
        }
    }
}
