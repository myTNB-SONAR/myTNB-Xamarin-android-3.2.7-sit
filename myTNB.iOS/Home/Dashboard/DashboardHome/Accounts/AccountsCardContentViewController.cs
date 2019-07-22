using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using myTNB.PushNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class AccountsCardContentViewController : CustomUIViewController
    {
        public AccountsCardContentViewController(IntPtr handle) : base(handle) { }

        public DashboardHomeViewController _homeViewController;
        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        public List<List<DueAmountDataModel>> _groupAccountList;
        List<UIView> _viewList = new List<UIView>();

        nfloat padding = 8f;
        nfloat searchPadding = 16f;
        nfloat labelHeight = 24f;
        nfloat imageWidth = 24f;
        nfloat imageHeight = 24f;

        UIPageControl _pageControl;
        UIScrollView _accountsCardScrollView;
        int _currentPageIndex;
        UIView _parentView, _searchView, _textFieldView;
        UILabel _headerTitle;
        UIImageView _searchIcon, _addAccountIcon;
        UITextField _textFieldSearch;

        UITapGestureRecognizer _tapGestureAddAccount;
        UITapGestureRecognizer _tapGestureSearch;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetTapGestureRecognizers();
            SetParentView();
            SetSearchView();
            SetCardScrollView();
            SetScrollViewSubViews();
            LoadAccountsWithDues();
        }

        public override void ViewWillAppear(bool animated) { }

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

        private void OnAddAccountAction()
        {
            Debug.WriteLine("OnAddAccountAction");
        }

        private void OnSearchAction()
        {
            Debug.WriteLine("OnSearchAction");
        }

        private void SetParentView()
        {
            _parentView = new UIView(new CGRect(0, 0, View.Frame.Width, _dashboardHomeHelper.GetHeightForAccountCards() + DashboardHomeConstants.SearchViewHeight + DashboardHomeConstants.PageControlHeight));
            _parentView.BackgroundColor = UIColor.Clear;
            View.AddSubview(_parentView);
        }

        private void SetSearchView()
        {
            _searchView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, DashboardHomeConstants.SearchViewHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _headerTitle = new UILabel(new CGRect(searchPadding, 0, 186f, labelHeight))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = UIColor.White,
                Text = "Dashboard_MyAccounts".Translate()
            };

            _searchIcon = new UIImageView(new CGRect(_searchView.Frame.Width - imageWidth - searchPadding, 0, imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle("Search-Icon")
            };

            _addAccountIcon = new UIImageView(new CGRect(_searchIcon.Frame.GetMinX() - imageWidth - 8f, 0, imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle("Add-Account-Icon")
            };

            var spacing = searchPadding + imageWidth + 8f;
            _textFieldView = new UIView(new CGRect(spacing, 0, _searchView.Frame.Width - spacing - searchPadding, 24f))
            {
                BackgroundColor = UIColor.White
            };
            _textFieldView.Layer.CornerRadius = 12f;
            _textFieldSearch = new UITextField(new CGRect(12f, 0, _textFieldView.Frame.Width - 24f - imageWidth / 2, 24f))
            {
                AttributedPlaceholder = new NSAttributedString(
                    "Dashboard_SearchPlacehoder".Translate()
                    , font: MyTNBFont.MuseoSans12_500
                    , foregroundColor: MyTNBColor.WhiteTwo
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_500
            };
            _textFieldView.Hidden = true;

            // SEARCH MODE
            //_headerTitle.Hidden = true;
            //_addAccountIcon = new UIImageView(new CGRect(padding, 0, imageWidth, imageHeight))
            //{
            //    Image = UIImage.FromBundle("Add-Account-Icon")
            //};

            _textFieldView.AddSubview(_textFieldSearch);
            _searchView.AddSubviews(new UIView { _headerTitle, _textFieldView, _addAccountIcon, _searchIcon });
            _parentView.AddSubview(_searchView);
        }

        private void SetCardScrollView()
        {
            _accountsCardScrollView = new UIScrollView(new CGRect(0, _searchView.Frame.GetMaxY(), _parentView.Frame.Width, _dashboardHomeHelper.GetHeightForAccountCards()))
            {
                Delegate = new AccountsScrollViewDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = false,
                BackgroundColor = UIColor.Clear
            };

            AdjustFrame(_accountsCardScrollView, padding, 0, -padding * 3, 0);
            _parentView.AddSubview(_accountsCardScrollView);
        }

        private void AddPageControl()
        {
            _pageControl = new UIPageControl(new CGRect(8, _accountsCardScrollView.Frame.GetMaxY(), View.Frame.Width - 16f, DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = MyTNBColor.WaterBlue,
                PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo,
                CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue,
                UserInteractionEnabled = false
            };
            _parentView.AddSubview(_pageControl);
        }

        /// <summary>
        /// Loads the Accounts with Dues
        /// </summary>
        private void LoadAccountsWithDues()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        bool shouldReload = false;
                        var accounts = GetAccountsToUpdate(ref shouldReload);

                        if (accounts?.Count > 0)
                        {
                            await GetAccountsSummary(accounts);
                            UpdateCardsWithTag(_currentPageIndex);
                        }
                        else if (shouldReload)
                        {
                            UpdateCardsWithTag(_currentPageIndex);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void UpdateDueForDisplayedAccounts(List<DueAmountDataModel> dueDetails)
        {
            if (_groupAccountList.Count <= 0)
                return;

            var groupAccountList = _groupAccountList[_currentPageIndex];

            foreach (var due in dueDetails)
            {
                foreach (var account in groupAccountList)
                {
                    if (account.accNum == due.accNum)
                    {
                        var item = account;
                        item.UpdateValues(due);
                        DataManager.DataManager.SharedInstance.SaveDue(item);
                        break;
                    }
                }
            }
        }

        private async Task<bool> GetAccountsSummary(List<string> accounts)
        {
            bool res = false;

            var response = await ServiceCall.GetLinkedAccountsSummaryInfo(accounts);
            res = response.didSucceed;

            if (response.didSucceed && response.AccountDues?.Count > 0)
            {
                UpdateDueForDisplayedAccounts(response.AccountDues);
            }
            else
            {
                //FAIL scenarios here...
            }
            return res;
        }

        /// <summary>
        /// Gets the accounts to update.
        /// </summary>
        /// <returns>The accounts to update.</returns>
        private List<string> GetAccountsToUpdate(ref bool shouldReload)
        {
            var acctsToGetLatestDues = new List<string>();

            if (_groupAccountList.Count <= 0)
                return acctsToGetLatestDues;

            shouldReload = RemoveDeletedAccounts() > 0;

            var groupAccountList = _groupAccountList[_currentPageIndex];

            // cache updates
            for (int i = 0; i < groupAccountList.Count; i++)
            {
                if (i > -1 && i < groupAccountList.Count)
                {
                    var account = groupAccountList[i];
                    var acctCached = DataManager.DataManager.SharedInstance.GetDue(account.accNum);
                    if (acctCached == null)
                    {
                        // get latest if not in cache
                        acctsToGetLatestDues.Add(account.accNum);
                    }
                    else if (account.amountDue != acctCached.amountDue
                           || string.Compare(account.accNickName, acctCached.accNickName) != 0)
                    {
                        // update nickname
                        account.amountDue = acctCached.amountDue;
                        account.accNickName = acctCached.accNickName;
                        groupAccountList[i] = account;
                        shouldReload = true;
                    }
                }
            }

            return acctsToGetLatestDues;
        }

        /// <summary>
        /// Removes the deleted accounts.
        /// </summary>
        /// <returns>The deleted accounts.</returns>
        private int RemoveDeletedAccounts()
        {
            int removedAccounts = 0;

            if (_groupAccountList.Count <= 0)
                return removedAccounts;

            List<string> keysToDelete = new List<string>();
            var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
            var groupAccountList = _groupAccountList[_currentPageIndex];

            // remove deleted accounts
            foreach (var delAccNum in DataManager.DataManager.SharedInstance.AccountsDeleted)
            {
                var deleteIndex = groupAccountList.FindIndex(x => x.accNum == delAccNum);
                if (deleteIndex > -1)
                {
                    groupAccountList.RemoveAt(deleteIndex);
                    removedAccounts++;
                }
            }

            // for accounts deleted in backend or encountered remove error
            var acctsToDelete = new List<string>();
            foreach (var item in groupAccountList)
            {
                // delete later if cannot find in main list
                var index = accountsList?.FindIndex(x => x.accNum == item.accNum);
                if (index < 0)
                {
                    acctsToDelete.Add(item.accNum);
                }
            }

            foreach (var delAccNum in acctsToDelete)
            {
                var deleteIndex = groupAccountList.FindIndex(x => x.accNum == delAccNum);
                if (deleteIndex > -1)
                {
                    groupAccountList.RemoveAt(deleteIndex);
                    removedAccounts++;
                }
            }

            if (removedAccounts > 0)
            {
                DataManager.DataManager.SharedInstance.AccountsDeleted.Clear();
            }

            return removedAccounts;
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
                _viewContainer.Tag = i;

                CGRect frame = _viewContainer.Frame;
                frame.X = (i * width) + padding;
                frame.Y = (_accountsCardScrollView.Frame.Height - frame.Height) / 2;
                frame.Width = width - (padding * 1);
                _viewContainer.Frame = frame;
                _accountsCardScrollView.AddSubview(_viewContainer);
                _viewList.Add(_viewContainer);

                AddAccountsCardInContainerView(_viewContainer, i);
            }
            _accountsCardScrollView.ContentSize = new CGSize(_accountsCardScrollView.Frame.Width * _groupAccountList.Count, _accountsCardScrollView.Frame.Height);
            AddPageControl();
            UpdatePageControl(_pageControl, _currentPageIndex, _groupAccountList.Count, GetContainerViewWithTag(_currentPageIndex));
        }

        private UIView GetContainerViewWithTag(int tag)
        {
            var subviews = _accountsCardScrollView.Subviews;
            UIView containerView = null;
            foreach (var views in subviews)
            {
                var view = views as UIView;
                if (view != null)
                {
                    if (view.Tag == tag)
                    {
                        containerView = view;
                        break;
                    }
                }
            }
            return containerView;
        }

        private void UpdateCardsWithTag(int tag)
        {
            var subviews = _accountsCardScrollView.Subviews;
            foreach (var views in subviews)
            {
                var view = views as UIView;
                if (view != null)
                {
                    if (view.Tag == tag)
                    {
                        RemoveAccountCardsFromView(view);
                        AddAccountsCardInContainerView(view, tag);
                        break;
                    }
                }
            }
        }

        private void RemoveAccountCardsFromView(UIView containerView)
        {
            var subviews = containerView.Subviews;
            foreach (var view in subviews)
            {
                view.RemoveFromSuperview();
            }
        }

        private void AddAccountsCardInContainerView(UIView containerView, int pageIndex)
        {
            var groupAccountList = _groupAccountList[pageIndex];
            for (int i = 0; i < groupAccountList.Count; i++)
            {
                DashboardHomeAccountCard _homeAccountCard = new DashboardHomeAccountCard(this, containerView, 68f * i);
                _homeAccountCard.SetTag(i);
                string iconName = "Accounts-Smart-Meter-Icon";
                if (groupAccountList[i].IsReAccount)
                {
                    iconName = "Accounts-RE-Icon";
                }
                else if (groupAccountList[i].IsNormalAccount)
                {
                    iconName = "Accounts-Normal-Icon";
                }
                _homeAccountCard.SetAccountIcon(iconName);
                _homeAccountCard.SetNickname(groupAccountList[i].accNickName);
                _homeAccountCard.SetAccountNo(groupAccountList[i].accNum);
                containerView.AddSubview(_homeAccountCard.GetUI());
                _homeAccountCard.AdjustLabels(groupAccountList[i]);
                _homeAccountCard.SetModel(groupAccountList[i]);
            }
        }

        private void UpdatePageControl(UIPageControl pageControl, int current, int pages, UIView currentView)
        {
            pageControl.CurrentPage = current;
            pageControl.Pages = pages;
            pageControl.UpdateCurrentPageDisplay();
            //pageControl.Frame = new CGRect(currentView.Frame.X, pageControl.Frame.Location.Y, View.Frame.Width - 16f, pageControl.Frame.Height);
        }

        private void ScrollViewHasPaginated()
        {
            Debug.WriteLine("_currentPageIndex: " + _currentPageIndex);
            UpdatePageControl(_pageControl, _currentPageIndex, _groupAccountList.Count, GetContainerViewWithTag(_currentPageIndex));
            LoadAccountsWithDues();
        }

        public void OnAccountCardSelected(DueAmountDataModel model)
        {
            _homeViewController.OnAccountCardSelected(model);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.BackgroundColor = UIColor.Clear;
        }

        private class AccountsScrollViewDelegate : UIScrollViewDelegate
        {
            AccountsCardContentViewController _controller;
            public AccountsScrollViewDelegate(AccountsCardContentViewController controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._accountsCardScrollView.ContentOffset.X / _controller._accountsCardScrollView.Frame.Width);
                if (newPageIndex == _controller._currentPageIndex)
                    return;

                _controller._currentPageIndex = newPageIndex;
                _controller.ScrollViewHasPaginated();
            }
        }
    }
}
