using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class AccountListViewController : CustomUIViewController
    {
        public AccountListViewController(IntPtr handle) : base(handle) { }

        public DashboardHomeViewController _homeViewController;
        private readonly DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private List<DueAmountDataModel> _accountList = new List<DueAmountDataModel>();
        private readonly RefreshScreenInfoModel _refreshScreenInfoModel = new RefreshScreenInfoModel();
        private readonly TextFieldHelper _textFieldHelper = new TextFieldHelper();

        private UIView _parentView, _headerView, _addAccountView, _searchView, _pipeView;
        private CustomUIView _footerView, searchView, _addView;
        private UILabel _headerTitle, _searchLbl, _addLbl, _moreAcctsLabel, _rearrangeLabel;
        private UITableView _accountListTableView;
        private UITextField _textFieldSearch;
        private UIImageView _searchIcon, _addIcon;
        private bool _isOnSearchMode;

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.BackgroundColor = UIColor.Clear;
        }

        public override void ViewDidLoad()
        {
            PageName = DashboardHomeConstants.PageName;
            base.ViewDidLoad();
            SetParentView();
            PrepareAccountList();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            base.LanguageDidChange(notification);

            if (_searchLbl != null && _addLbl != null && _searchIcon != null && _addIcon != null)
            {
                _searchLbl.Text = GetI18NValue(DashboardHomeConstants.I18N_Search);
                CGSize searchSize = _searchLbl.SizeThatFits(new CGSize(1000F, 1000F));
                ViewHelper.AdjustFrameSetWidth(_searchLbl, searchSize.Width);

                ViewHelper.AdjustFrameSetWidth(searchView, GetScaledWidth(12F) + _searchIcon.Frame.Width + GetScaledWidth(4F) + _searchLbl.Frame.Width);
                ViewHelper.AdjustFrameSetX(searchView, _addAccountView.Frame.Width - searchView.Frame.Width - GetScaledWidth(16F));

                ViewHelper.AdjustFrameSetX(_searchLbl, searchView.Frame.Width - _searchLbl.Frame.Width);
                ViewHelper.AdjustFrameSetX(_searchIcon, _searchLbl.Frame.GetMinX() - GetScaledWidth(4F) - _searchIcon.Frame.Width);

                _pipeView.Frame = new CGRect(new CGPoint(searchView.Frame.GetMinX() - GetScaledWidth(1F), _pipeView.Frame.Y)
                    , _pipeView.Frame.Size);
            }

            if (_addLbl != null)
            {
                _addLbl.Text = GetI18NValue(DashboardHomeConstants.I18N_Add);
                CGSize addSizes = _addLbl.SizeThatFits(new CGSize(1000F, 1000F));
                ViewHelper.AdjustFrameSetWidth(_addLbl, addSizes.Width);

                ViewHelper.AdjustFrameSetWidth(_addView, _addIcon.Frame.Width + GetScaledWidth(4F) + _addLbl.Frame.Width + GetScaledWidth(_dashboardHomeHelper.HasMoreThanThreeAccts ? 12F : 0F));
                ViewHelper.AdjustFrameSetX(_addView, _dashboardHomeHelper.HasMoreThanThreeAccts ? _pipeView.Frame.GetMinX() - _addView.Frame.Width : _headerView.Frame.Width - _addView.Frame.Width - BaseMarginWidth16);

                ViewHelper.AdjustFrameSetX(_addIcon, 0);
                ViewHelper.AdjustFrameSetX(_addLbl, _addIcon.Frame.GetMaxX() + GetScaledWidth(4F));
            }

            if (_headerTitle != null)
            {
                _headerTitle.Text = GetI18NValue(DashboardHomeConstants.I18N_MyAccts);
            }
            if (_textFieldSearch != null)
            {
                _textFieldSearch.AttributedPlaceholder = new NSAttributedString(
                   GetI18NValue(DashboardHomeConstants.I18N_SearchPlaceholder)
                   , font: TNBFont.MuseoSans_12_500
                   , foregroundColor: UIColor.FromWhiteAlpha(1, 0.6F)
                   , strokeWidth: 0
               );
            }
            if (_moreAcctsLabel != null)
            {
                _moreAcctsLabel.Text = _dashboardHomeHelper.AllAccountsAreVisible
                    ? GetI18NValue(DashboardHomeConstants.I18N_ShowLess)
                    : GetI18NValue(DashboardHomeConstants.I18N_MoreAccts);
            }
            if (_rearrangeLabel != null)
            {
                _rearrangeLabel.Text = GetI18NValue(DashboardHomeConstants.I18N_RearrangeAccts);
            }
        }

        #region Initialization Methods
        public void PrepareAccountList(List<CustomerAccountRecordModel> linkedCAs = null, bool isFromSearch = false, bool isFromNeedRefresh = false)
        {
            if (isFromNeedRefresh)
            {
                SetViewForActiveSearch(false);
                if (_textFieldSearch != null)
                {
                    if (_textFieldSearch.Text.Length > 0)
                    {
                        _textFieldSearch.Text = string.Empty;
                    }
                }
            }
            if (_parentView == null)
            {
                SetParentView();
            }
            if (linkedCAs == null)
            {
                DataManager.DataManager.SharedInstance.CurrentAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
            }
            else
            {
                DataManager.DataManager.SharedInstance.CurrentAccountList = linkedCAs;
            }
            DataManager.DataManager.SharedInstance.ActiveAccountList = new List<DueAmountDataModel>();
            if (!_isOnSearchMode)
            {
                SetHeaderView();
                SetAddAccountView();
                if (_dashboardHomeHelper.HasMoreThanThreeAccts)
                {
                    SetSearchView();
                }
                AddTableView();
            }
            PrepareAccounts(DataManager.DataManager.SharedInstance.CurrentAccountList, isFromSearch);
        }

        private void SetParentView()
        {
            _parentView = new UIView(new CGRect(0,
                GetScaledHeight(24F), View.Frame.Width,
                _dashboardHomeHelper.GetHeightForAccountList() - GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = true
            };
            View.AddSubview(_parentView);
        }

        private void SetHeaderView()
        {
            if (_headerView != null)
            {
                _headerView.RemoveFromSuperview();
                _headerView = null;
            }
            nfloat width = _parentView != null ? _parentView.Frame.Width : ViewWidth;
            _headerView = new UIView(new CGRect(0, 0, width, DashboardHomeConstants.SearchViewHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            _parentView.AddSubview(_headerView);

            _headerTitle = new UILabel(new CGRect(BaseMarginWidth16, 0, GetScaledWidth(84f), GetScaledHeight(20f)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = UIColor.White,
                Text = GetI18NValue(DashboardHomeConstants.I18N_MyAccts),
                BackgroundColor = UIColor.Clear
            };
            _headerView.AddSubview(_headerTitle);

            UIView lineView = new UIView(new CGRect(0, _headerView.Frame.Height - GetScaledHeight(1F), ViewWidth, GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            _headerView.AddSubview(lineView);
        }

        private void SetAddAccountView()
        {
            _addAccountView = new UIView(_headerView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            _headerView.AddSubview(_addAccountView);

            if (_dashboardHomeHelper.HasAccounts)
            {
                searchView = new CustomUIView(new CGRect(0, GetScaledHeight(2F), 0, GetScaledHeight(16F)))
                {
                    BackgroundColor = UIColor.Clear,
                    Hidden = !_dashboardHomeHelper.HasMoreThanThreeAccts
                };
                searchView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSearchAction();
                }));
                _addAccountView.AddSubview(searchView);

                UIImageView searchIcon = new UIImageView(new CGRect(0, 0, GetScaledWidth(16F), GetScaledHeight(16F)))
                {
                    Image = UIImage.FromBundle(DashboardHomeConstants.Img_SearchIconWhite)
                };
                searchView.AddSubview(searchIcon);

                _searchLbl = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
                {
                    Font = TNBFont.MuseoSans_12_500,
                    TextColor = UIColor.White,
                    Text = GetI18NValue(DashboardHomeConstants.I18N_Search),
                    BackgroundColor = UIColor.Clear
                };
                searchView.AddSubview(_searchLbl);

                CGSize searchSize = _searchLbl.SizeThatFits(new CGSize(1000F, 1000F));
                ViewHelper.AdjustFrameSetWidth(_searchLbl, searchSize.Width);

                ViewHelper.AdjustFrameSetWidth(searchView, GetScaledWidth(12F) + searchIcon.Frame.Width + GetScaledWidth(4F) + _searchLbl.Frame.Width);
                ViewHelper.AdjustFrameSetX(searchView, _addAccountView.Frame.Width - searchView.Frame.Width - GetScaledWidth(16F));

                ViewHelper.AdjustFrameSetX(_searchLbl, searchView.Frame.Width - _searchLbl.Frame.Width);
                ViewHelper.AdjustFrameSetX(searchIcon, _searchLbl.Frame.GetMinX() - GetScaledWidth(4F) - searchIcon.Frame.Width);

                _pipeView = new UIView(new CGRect(searchView.Frame.GetMinX() - GetScaledWidth(1F), 0, GetScaledWidth(1F), GetScaledHeight(20F)))
                {
                    BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F),
                    Hidden = !_dashboardHomeHelper.HasMoreThanThreeAccts
                };
                _addAccountView.AddSubview(_pipeView);

                _addView = new CustomUIView(new CGRect(0, GetScaledHeight(2F), 0, GetScaledHeight(16F)))
                {
                    BackgroundColor = UIColor.Clear
                };
                _addView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnAddAccountAction();
                }));
                _addAccountView.AddSubview(_addView);

                _addIcon = new UIImageView(new CGRect(0, 0, GetScaledWidth(16F), GetScaledHeight(16F)))
                {
                    Image = UIImage.FromBundle(DashboardHomeConstants.Img_AddAcctIconWhite)
                };
                _addView.AddSubview(_addIcon);

                _addLbl = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
                {
                    Font = TNBFont.MuseoSans_12_500,
                    TextColor = UIColor.White,
                    Text = GetI18NValue(DashboardHomeConstants.I18N_Add),
                    BackgroundColor = UIColor.Clear
                };
                _addView.AddSubview(_addLbl);

                CGSize addSize = _addLbl.SizeThatFits(new CGSize(1000F, 1000F));
                ViewHelper.AdjustFrameSetWidth(_addLbl, addSize.Width);

                ViewHelper.AdjustFrameSetWidth(_addView, _addIcon.Frame.Width + GetScaledWidth(4F) + _addLbl.Frame.Width + GetScaledWidth(_dashboardHomeHelper.HasMoreThanThreeAccts ? 12F : 0F));
                ViewHelper.AdjustFrameSetX(_addView, _dashboardHomeHelper.HasMoreThanThreeAccts ? _pipeView.Frame.GetMinX() - _addView.Frame.Width : _headerView.Frame.Width - _addView.Frame.Width - BaseMarginWidth16);

                ViewHelper.AdjustFrameSetX(_addIcon, 0);
                ViewHelper.AdjustFrameSetX(_addLbl, _addIcon.Frame.GetMaxX() + GetScaledWidth(4F));
            }
        }

        private void SetSearchView()
        {
            _searchView = new UIView(_headerView.Bounds)
            {
                BackgroundColor = UIColor.Clear,
                Hidden = true
            };
            _headerView.AddSubview(_searchView);

            _searchIcon = new UIImageView(new CGRect(BaseMarginWidth16, 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(DashboardHomeConstants.Img_SearchActiveIconWhite)
            };
            _searchView.AddSubview(_searchIcon);

            _textFieldSearch = new UITextField(new CGRect(_searchIcon.Frame.GetMaxX() + GetScaledWidth(8F), 0, _searchView.Frame.Width - (BaseMarginWidth16 * 2) - (_searchIcon.Frame.GetMaxX() + GetScaledWidth(16F)), GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                AttributedPlaceholder = new NSAttributedString(
                   GetI18NValue(DashboardHomeConstants.I18N_SearchPlaceholder)
                   , font: TNBFont.MuseoSans_12_500
                   , foregroundColor: UIColor.FromWhiteAlpha(1, 0.6F)
                   , strokeWidth: 0
                ),
                BackgroundColor = UIColor.Clear,
                TintColor = UIColor.White
            };
            _textFieldHelper.SetKeyboard(_textFieldSearch);
            _textFieldSearch.ReturnKeyType = UIReturnKeyType.Search;
            SetTextFieldEvents(_textFieldSearch);
            _searchView.AddSubview(_textFieldSearch);

            UIImageView cancelIcon = new UIImageView(new CGRect(_searchView.Frame.Width - GetScaledWidth(16F) - BaseMarginWidth16, 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(DashboardHomeConstants.Img_SearchCancelIcon),
                UserInteractionEnabled = true
            };
            cancelIcon.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnCancelSearch();
            }));
            _searchView.AddSubview(cancelIcon);

            UIView lineView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_textFieldSearch.Frame, 4F), _searchView.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            _searchView.AddSubview(lineView);
        }

        private void SetFooterView(bool allAcctsAreVisible = false)
        {
            if (!_dashboardHomeHelper.HasMoreThanThreeAccts)
            {
                if (_footerView != null)
                {
                    _footerView.RemoveFromSuperview();
                }
                _accountListTableView.TableFooterView = null;
                return;
            }

            if (_footerView != null)
            {
                _footerView.RemoveFromSuperview();
                _accountListTableView.TableFooterView = null;
            }

            _footerView = new CustomUIView(new CGRect(0, 0, ViewWidth, allAcctsAreVisible ? GetScaledHeight(85F) : GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.Clear
            };

            CustomUIView moreLessContainer = new CustomUIView(_footerView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };

            ViewHelper.AdjustFrameSetY(moreLessContainer, allAcctsAreVisible ? GetScaledHeight(41F) : 0);
            moreLessContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (allAcctsAreVisible)
                {
                    OnShowLessAction();
                }
                else
                {
                    OnShowMoreAction();
                }
            }));

            CustomUIView moreLessView = new CustomUIView(new CGRect(0, GetScaledHeight(12F), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear
            };

            _moreAcctsLabel = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                Text = allAcctsAreVisible ? GetI18NValue(DashboardHomeConstants.I18N_ShowLess) : GetI18NValue(DashboardHomeConstants.I18N_MoreAccts)
            };
            moreLessView.AddSubview(_moreAcctsLabel);
            UIImageView arrowUpDown = new UIImageView(new CGRect(_moreAcctsLabel.Frame.GetMaxX(), 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(allAcctsAreVisible ? DashboardHomeConstants.Img_ArrowUpWhite : DashboardHomeConstants.Img_ArrowDownWhite)
            };
            moreLessView.AddSubview(arrowUpDown);

            CGSize lblSize = _moreAcctsLabel.SizeThatFits(new CGSize(1000F, 1000F));
            ViewHelper.AdjustFrameSetWidth(_moreAcctsLabel, lblSize.Width);
            ViewHelper.AdjustFrameSetX(arrowUpDown, _moreAcctsLabel.Frame.GetMaxX() + GetScaledWidth(4F));

            ViewHelper.AdjustFrameSetWidth(moreLessView, _moreAcctsLabel.Frame.Width + GetScaledWidth(4F) + arrowUpDown.Frame.Width);
            ViewHelper.AdjustFrameSetX(moreLessView, GetXLocationToCenterObject(moreLessView.Frame.Width, moreLessContainer));

            moreLessContainer.AddSubview(moreLessView);
            _footerView.AddSubview(moreLessContainer);

            if (allAcctsAreVisible)
            {
                CustomUIView rearrangeContainer = new CustomUIView(_footerView.Bounds)
                {
                    BackgroundColor = UIColor.Clear
                };

                ViewHelper.AdjustFrameSetY(rearrangeContainer, 0);
                ViewHelper.AdjustFrameSetHeight(rearrangeContainer, GetScaledHeight(41F));
                rearrangeContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnRearrangeAction();
                }));

                CustomUIView rearrangeView = new CustomUIView(new CGRect(0, GetScaledHeight(12F), 0, GetScaledHeight(16F)))
                {
                    BackgroundColor = UIColor.Clear
                };

                UIImageView iconR = new UIImageView(new CGRect(0, 0, GetScaledWidth(16F), GetScaledHeight(16F)))
                {
                    Image = UIImage.FromBundle(DashboardHomeConstants.Img_RearrangeIcon)
                };
                rearrangeView.AddSubview(iconR);
                _rearrangeLabel = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
                {
                    Font = TNBFont.MuseoSans_12_500,
                    TextColor = UIColor.White,
                    Text = GetI18NValue(DashboardHomeConstants.I18N_RearrangeAccts)
                };
                rearrangeView.AddSubview(_rearrangeLabel);

                CGSize lblRSize = _rearrangeLabel.SizeThatFits(new CGSize(1000F, 1000F));
                ViewHelper.AdjustFrameSetWidth(_rearrangeLabel, lblRSize.Width);
                ViewHelper.AdjustFrameSetX(_rearrangeLabel, iconR.Frame.GetMaxX() + GetScaledWidth(8F));

                ViewHelper.AdjustFrameSetWidth(rearrangeView, iconR.Frame.Width + GetScaledWidth(8F) + _rearrangeLabel.Frame.Width);
                ViewHelper.AdjustFrameSetX(rearrangeView, GetXLocationToCenterObject(rearrangeView.Frame.Width, rearrangeContainer));

                rearrangeContainer.AddSubview(rearrangeView);

                UIView lineView = new UIView(new CGRect(BaseMarginWidth16, rearrangeView.Frame.GetMaxY() + GetScaledHeight(12F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(1F)))
                {
                    BackgroundColor = UIColor.FromWhiteAlpha(1, 0.20F)
                };
                rearrangeContainer.AddSubview(lineView);

                _footerView.AddSubview(rearrangeContainer);
            }

            if (_accountListTableView != null)
            {
                _accountListTableView.TableFooterView = _footerView;
            }
        }

        private void AddTableView()
        {
            if (_accountListTableView != null)
            {
                _accountListTableView.RemoveFromSuperview();
                _accountListTableView = null;
            }
            _accountListTableView = new UITableView(new CGRect(0, _headerView.Frame.GetMaxY()
                , ViewWidth, DashboardHomeConstants.ShimmerAcctHeight))
            { BackgroundColor = UIColor.Clear, ScrollEnabled = false };
            _accountListTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _accountListTableView.RegisterClassForCellReuse(typeof(AccountListCell), DashboardHomeConstants.Cell_AccountList);
            _accountListTableView.RegisterClassForCellReuse(typeof(AccountListEmptyCell), DashboardHomeConstants.Cell_AccountListEmpty);
            _parentView.AddSubview(_accountListTableView);
        }

        private void ReloadTableView(bool isLoading, bool hasEmptyAcct = false)
        {
            if (_accountListTableView != null)
            {
                _accountListTableView.Source = new AccountListDataSource(DataManager.DataManager.SharedInstance.ActiveAccountList,
                    GetI18NValue,
                    _homeViewController.OnAccountCardSelected,
                    _homeViewController.OnAddAccountAction,
                    isLoading,
                    hasEmptyAcct);
                _accountListTableView.ReloadData();
            }
        }
        #endregion

        #region Search Methods
        private void SetViewForActiveSearch(bool isSearchMode)
        {
            _isOnSearchMode = isSearchMode;
            DataManager.DataManager.SharedInstance.IsOnSearchMode = isSearchMode;
            if (_textFieldSearch != null)
            {
                if (isSearchMode)
                {
                    _textFieldSearch.BecomeFirstResponder();
                }
                else
                {
                    _textFieldSearch.ResignFirstResponder();
                }
            }

            if (_headerTitle != null)
            {
                _headerTitle.Hidden = isSearchMode;
            }
            if (_addAccountView != null)
            {
                _addAccountView.Hidden = isSearchMode;
            }
            if (_searchView != null)
            {
                _searchView.Hidden = !isSearchMode;
            }
        }

        private void SetTextFieldEvents(UITextField textField)
        {
            textField.EditingChanged += (sender, e) =>
            {
                SearchFromAccountList(textField.Text);
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
        }

        public void DismissActiveKeyboard()
        {
            if (_textFieldSearch != null)
            {
                _textFieldSearch.ResignFirstResponder();
            }
        }

        private void SearchFromAccountList(string searchString)
        {
            DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
            if (_footerView != null)
            {
                //_footerView.RemoveFromSuperview();
            }
            var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
            var searchResults = accountsList.FindAll(x => x.accountNickName.ToLower().Contains(searchString.ToLower()) || x.accNum.Contains(searchString));
            PrepareAccountList(searchResults, true);
        }

        private void OnCancelSearch()
        {
            SetViewForActiveSearch(false);
            if (_textFieldSearch != null)
            {
                if (_textFieldSearch.Text.Length > 0)
                {
                    _textFieldSearch.Text = string.Empty;
                    DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
                    if (_homeViewController != null)
                    {
                        _homeViewController.OnReload();//.OnUpdateCellWithoutReload(DashboardHomeConstants.CellIndex_Services);
                    }
                    PrepareAccountList();
                }
            }
        }
        #endregion

        #region API/Logic Methods
        private void PrepareAccounts(List<CustomerAccountRecordModel> accountList, bool isFromSearch = false)
        {
            if (accountList != null && accountList.Count > 0)
            {
                var activeAccountList = DataManager.DataManager.SharedInstance.ActiveAccountList;
                List<string> acctNumList = new List<string>();
                if (activeAccountList.Count > 0)
                {
                    var newBatchList = GetBatchAccountList(GetInactiveAccountList(activeAccountList), ref acctNumList);
                    if (newBatchList != null &&
                        newBatchList.Count > 0)
                    {
                        DataManager.DataManager.SharedInstance.ActiveAccountList.AddRange(newBatchList);
                    }
                }
                else
                {
                    _accountList = _dashboardHomeHelper.GetAccountListForDashboard(accountList);
                    DataManager.DataManager.SharedInstance.ActiveAccountList = new List<DueAmountDataModel>();
                    DataManager.DataManager.SharedInstance.ActiveAccountList = GetBatchAccountList(_accountList, ref acctNumList, true);
                }
                if (acctNumList.Count > 0)
                {
                    var acctsToGetDues = GetAccountsToUpdate(acctNumList);
                    if (acctsToGetDues.Count > 0)
                    {
                        GetAccountsBillSummary(acctNumList, DataManager.DataManager.SharedInstance.ActiveAccountList.Count <= DashboardHomeConstants.InitialLoadMaxCount, isFromSearch);
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.AccountListIsLoaded = true;
                        _homeViewController.ShowRefreshScreen(false, null);
                        ReloadViews(false, isFromSearch);
                    }
                }
            }
            else
            {
                ReloadViews(false, isFromSearch, DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count == 0);
            }
        }

        private List<DueAmountDataModel> GetBatchAccountList(List<DueAmountDataModel> inactiveList, ref List<string> acctNumList, bool isReset = false)
        {
            List<DueAmountDataModel> batchList = new List<DueAmountDataModel>();
            if (inactiveList != null &&
                inactiveList.Count > 0)
            {
                int ctr = 0;
                int maxAcctLimit = isReset ? DashboardHomeConstants.InitialLoadMaxCount : DashboardHomeConstants.MaxAccountPerLoad;
                foreach (var account in inactiveList)
                {
                    if (ctr < maxAcctLimit)
                    {
                        batchList.Add(account);
                        acctNumList.Add(account.accNum);
                        ctr++;
                    }
                }
            }
            return batchList;
        }

        private List<DueAmountDataModel> GetInactiveAccountList(List<DueAmountDataModel> activeList)
        {
            List<DueAmountDataModel> inactiveList = new List<DueAmountDataModel>(_accountList);
            if (inactiveList != null &&
                inactiveList.Count > 0)
            {
                var acctsToRemove = new List<string>();
                foreach (var item in activeList)
                {
                    var index = inactiveList?.FindIndex(x => x.accNum == item.accNum);
                    if (index > -1)
                    {
                        acctsToRemove.Add(item.accNum);
                    }
                }
                if (acctsToRemove != null &&
                    acctsToRemove.Count > 0)
                {
                    foreach (var delAccNum in acctsToRemove)
                    {
                        var deleteIndex = inactiveList.FindIndex(x => x.accNum == delAccNum);
                        if (deleteIndex > -1)
                        {
                            inactiveList.RemoveAt(deleteIndex);
                        }
                    }
                }
            }
            return inactiveList;
        }

        private void UpdateDueForDisplayedAccounts(List<DueAmountDataModel> dueDetails)
        {
            var activeAccountList = DataManager.DataManager.SharedInstance.ActiveAccountList;
            foreach (var due in dueDetails)
            {
                foreach (var account in activeAccountList)
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

        private List<string> GetAccountsToUpdate(List<string> accNumList)
        {
            var acctsToGetLatestDues = new List<string>();
            var activeAccountList = DataManager.DataManager.SharedInstance.ActiveAccountList;

            // cache updates
            for (int i = 0; i < activeAccountList.Count; i++)
            {
                if (i > -1 && i < activeAccountList.Count)
                {
                    foreach (var acctNum in accNumList)
                    {
                        var account = activeAccountList[i];
                        if (acctNum.Equals(account.accNum))
                        {
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
                                DataManager.DataManager.SharedInstance.ActiveAccountList[i] = account;
                            }
                        }
                    }
                }
            }
            return acctsToGetLatestDues;
        }

        private void GetAccountsBillSummary(List<string> accounts, bool isFirstCall = false, bool isFromSearch = false)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
                        ReloadViews(true, isFromSearch);
                        InvokeInBackground(async () =>
                        {
                            AmountDueStatusResponseModel response = await ServiceCall.GetAccountsBillSummary(accounts);
                            InvokeOnMainThread(() =>
                            {
                                DataManager.DataManager.SharedInstance.AccountListIsLoaded = true;
                                if (response != null &&
                                    response.d != null &&
                                    response.d.IsSuccess)
                                {
                                    if (response.d.data != null && response.d.data.Count > 0)
                                    {
                                        _homeViewController.ShowRefreshScreen(false, null);
                                        UpdateDueForDisplayedAccounts(response.d.data);
                                        ReloadViews(false, isFromSearch);
                                    }
                                    else
                                    {
                                        _refreshScreenInfoModel.RefreshBtnText = response?.d?.RefreshBtnText ?? GetI18NValue(DashboardHomeConstants.I18N_RefreshBtnTxt);
                                        _refreshScreenInfoModel.RefreshMessage = response?.d?.RefreshMessage ?? GetI18NValue(DashboardHomeConstants.I18N_RefreshMsg);
                                        _homeViewController.ShowRefreshScreen(true, _refreshScreenInfoModel);
                                    }
                                }
                                else if (response != null &&
                                    response.d != null &&
                                    response.d.IsPlannedDownTime)
                                {
                                    _refreshScreenInfoModel.DisplayMessage = response.d.DisplayMessage.IsValid() ? response.d.DisplayMessage : GetErrorI18NValue(Constants.Error_BcrmDownMessage);
                                    _homeViewController.ShowRefreshScreen(true, _refreshScreenInfoModel, true);
                                }
                                else
                                {
                                    _refreshScreenInfoModel.RefreshBtnText = response?.d?.RefreshBtnText ?? GetI18NValue(DashboardHomeConstants.I18N_RefreshBtnTxt);
                                    _refreshScreenInfoModel.RefreshMessage = response?.d?.RefreshMessage ?? GetI18NValue(DashboardHomeConstants.I18N_RefreshMsg);
                                    _homeViewController.ShowRefreshScreen(true, _refreshScreenInfoModel);
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

        private void ReloadViews(bool isLoading, bool isFromSearch = false, bool hasEmptyAcct = false)
        {
            if (_addAccountView != null)
            {
                _addAccountView.Hidden = _isOnSearchMode;
            }
            ReloadTableView(isLoading, hasEmptyAcct);
            if (!isFromSearch)
            {
                if (_homeViewController != null)
                {
                    _homeViewController.OnUpdateTable();
                }
            }
            else
            {
                if (_homeViewController != null)
                {
                    _homeViewController.OnUpdateCellWithoutReload(DashboardHomeConstants.CellIndex_Services);
                }
            }

            ViewHelper.AdjustFrameSetHeight(_parentView, _dashboardHomeHelper.GetHeightForAccountList() - GetScaledHeight(24F));
            ViewHelper.AdjustFrameSetHeight(_accountListTableView, _parentView.Frame.Height - _headerView.Frame.Height);

            if (isLoading)
            {
                if (_footerView != null)
                {
                    _footerView.RemoveFromSuperview();
                    _accountListTableView.TableFooterView = null;
                }
            }
            else
            {
                SetFooterView(_dashboardHomeHelper.AllAccountsAreVisible);
            }
            if (_homeViewController != null)
            {
                _homeViewController._accountListIsShimmering = isLoading;
                if (!isLoading)
                {
                    _homeViewController.CheckTutorialOverlay();
                }
            }
        }

        #endregion

        #region Action Methods
        private void OnAddAccountAction()
        {
            if (_homeViewController != null)
            {
                _homeViewController.OnAddAccountAction();
            }
        }
        private void OnSearchAction()
        {
            SetViewForActiveSearch(true);
        }
        private void OnShowMoreAction()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    InvokeOnMainThread(() =>
                    {
                        PrepareAccounts(DataManager.DataManager.SharedInstance.CurrentAccountList);
                    });
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        DisplayNoDataAlert();
                    });
                }
            });
        }
        private void OnShowLessAction()
        {
            DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
            if (_homeViewController != null)
            {
                _homeViewController.ScrollTableToTheTop();
            }
            PrepareAccountList(DataManager.DataManager.SharedInstance.CurrentAccountList);
        }
        private void OnRearrangeAction()
        {
            if (_homeViewController != null)
            {
                _homeViewController.OnRearrangeAccountAction();
            }
        }
        #endregion
    }
}
