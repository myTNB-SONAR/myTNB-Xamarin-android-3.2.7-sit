using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.PushNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using UIKit;

namespace myTNB
{
    public partial class AccountsCardContentViewController : CustomUIViewController
    {
        public AccountsCardContentViewController(IntPtr handle) : base(handle) { }

        DashboardHomeHeader _dashboardHomeHeader;
        public DashboardHomeViewController _homeViewController;
        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        List<string> _accountNumberList = new List<string>();
        public List<List<DueAmountDataModel>> _groupAccountList;
        List<UIView> _viewList = new List<UIView>();

        nfloat padding = 8f;
        nfloat pageControlHeight = 20f;

        private UIPageControl _pageCont;

        private UIScrollView _accountsCardScrollView;
        private int _currentPageIndex;

        UITapGestureRecognizer _tapGestureAddAccount;
        UITapGestureRecognizer _tapGestureSearch;

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

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.BackgroundColor = UIColor.Clear;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetTapGestureRecognizers();
            _dashboardHomeHeader = new DashboardHomeHeader(View);
            _dashboardHomeHeader.SetGreetingText("Good Morning");
            _dashboardHomeHeader.SetNameText(_dashboardHomeHelper.GetDisplayName());
            UIView headerView = _dashboardHomeHeader.GetUI();
            _dashboardHomeHeader.AddNotificationAction(OnNotificationAction);
            _dashboardHomeHeader.SetAddAccountAction(_tapGestureAddAccount);
            _dashboardHomeHeader.SetSearchAction(_tapGestureSearch);
            View.AddSubview(headerView);

            //_pageCont = CreatePageControll();
            _accountsCardScrollView = new UIScrollView(new CGRect(0, headerView.Frame.GetMaxY(), View.Frame.Width, _dashboardHomeHelper.GetHeightForAccountCards() - pageControlHeight))
            {
                Delegate = new ScViewDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = false,
                BackgroundColor = UIColor.Clear
            };
            //_accountsCardScrollView.AddSubview(_pageCont);

            AdjustFrame(_accountsCardScrollView, padding, 0, -padding * 3, 0);
            View.AddSubview(_accountsCardScrollView);

            SetScrollViewSubViews();
        }

        public override void ViewWillAppear(bool animated)
        {

        }

        private CGRect AdjustFrame(CGRect f, nfloat x, nfloat y, nfloat w, nfloat h)
        {
            f.X = f.X + x;
            f.Y = f.Y + y;
            f.Width = f.Width + w;
            f.Height = f.Height + h;

            return f;
        }

        private void AdjustFrame(UIView view, nfloat x, nfloat y, nfloat w, nfloat h)
        {
            view.Frame = AdjustFrame(view.Frame, x, y, w, h);
        }

        private void ClearScrollViewSubViews()
        {
            for (int i = 0; i < _viewList.Count; i++)
            {
                UIView view = _viewList[i];
                view.RemoveFromSuperview();
            }
            _viewList.Clear();
        }

        private void SetScrollViewSubViews()
        {
            nfloat width = _accountsCardScrollView.Frame.Width;
            for (int i = 0; i < _groupAccountList.Count; i++)
            {
                UIView _viewContainer = new UIView(_accountsCardScrollView.Bounds);
                _viewContainer.BackgroundColor = UIColor.Clear;

                CGRect frame = _viewContainer.Frame;
                frame.X = (i * width) + padding;
                frame.Y = (_accountsCardScrollView.Frame.Height - frame.Height) / 2;
                frame.Width = width - (padding * 1);
                _viewContainer.Frame = frame;
                _accountsCardScrollView.AddSubview(_viewContainer);
                _viewList.Add(_viewContainer);

                var groupAccountList = _groupAccountList[i];
                for (int o = 0; o < groupAccountList.Count; o++)
                {
                    DashboardHomeAccountCard _homeAccountCard = new DashboardHomeAccountCard(this, _viewContainer, 68f * o);
                    string iconName = "Accounts-Smart-Meter-Icon";
                    if (groupAccountList[o].IsReAccount)
                    {
                        iconName = "Accounts-RE-Icon";
                    }
                    else if (groupAccountList[o].IsNormalAccount)
                    {
                        iconName = "Accounts-Normal-Icon";
                    }
                    _homeAccountCard.SetAccountIcon(iconName);
                    _homeAccountCard.SetNickname(groupAccountList[o].accNickName);
                    _homeAccountCard.SetAccountNo(groupAccountList[o].accNum);
                    _viewContainer.AddSubview(_homeAccountCard.GetUI());
                    _homeAccountCard.AdjustLabels(groupAccountList[o]);
                    _homeAccountCard.SetModel(groupAccountList[o]);
                }
            }
            _accountsCardScrollView.ContentSize = new CGSize(_accountsCardScrollView.Frame.Width * _groupAccountList.Count, _accountsCardScrollView.Frame.Height);
        }


        private UIPageControl CreatePageControll()
        {
            UIPageControl pageControll = new UIPageControl(new RectangleF(146,
                348, 38, 20));
            pageControll.BackgroundColor = UIColor.Red;
            pageControll.Alpha = 0.7f;

            return pageControll;
        }

        private void UpdatePageControll(UIPageControl cont, int current,
        int pages, UIView showed)
        {
            cont.CurrentPage = current;
            cont.Pages = pages;
            cont.UpdateCurrentPageDisplay();

            //UIView.AnimationsEnabled = true;
            //UIView.BeginAnimations(string.Empty, this.Handle);
            //cont.Frame = new RectangleF((float)showed.Frame.Location.X
            //                                , (float)cont.Frame.Location.Y,
            //                                pageSize().Width,
            //                                (float)cont.Frame.Height);
            //UIView.CommitAnimations();

        }

        private void currentPageIndexDidChange()
        {

            Debug.WriteLine("_currentPageIndex: " + _currentPageIndex);
            //UpdatePageControll(_pageCont,
            //                 _currentPageIndex, this.numberOfPages(),
            //                ((UIView)this._pageViews[_currentPageIndex]));
            //_accountsCardScrollView.BringSubviewToFront(_pageCont);


            //this.NavigationController.Title =
            //                 string.Format("{0} of {1}",
            //                 _currentPageIndex + 1, this.numberOfPages());
        }

        public void OnAccountCardSelected(DueAmountDataModel model)
        {
            _homeViewController.OnAccountCardSelected(model);
        }

        class ScViewDelegate : UIScrollViewDelegate
        {
            AccountsCardContentViewController _id;
            public ScViewDelegate(AccountsCardContentViewController id)
            {
                _id = id;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_id._accountsCardScrollView.ContentOffset.X / _id._accountsCardScrollView.Frame.Width);
                if (newPageIndex == _id._currentPageIndex)
                    return;

                _id._currentPageIndex = newPageIndex;
                _id.currentPageIndexDidChange();
            }
        }
    }
}
