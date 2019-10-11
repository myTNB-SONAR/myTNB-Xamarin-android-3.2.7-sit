using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class AccountListViewController : CustomUIViewController
    {
        public AccountListViewController(IntPtr handle) : base(handle) { }

        public DashboardHomeViewController _homeViewController;
        private DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private List<DueAmountDataModel> _accountList = new List<DueAmountDataModel>();
        private RefreshScreenInfoModel _refreshScreenInfoModel = new RefreshScreenInfoModel();

        private UIView _parentView, _headerView;
        private CustomUIView _footerView;
        private UILabel _headerTitle;
        private UITableView _accountListTableView;

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.BackgroundColor = UIColor.Clear;
        }

        public override void ViewDidLoad()
        {
            PageName = DashboardHomeConstants.PageName;
            base.ViewDidLoad();
            PrepareAccounts();
            SetParentView();
            SetHeaderView();
            AddTableView();
            SetFooterView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ReloadTableView();
        }

        #region Initialization Methods
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
            _headerView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, DashboardHomeConstants.SearchViewHeight))
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

            CustomUIView searchView = new CustomUIView(new CGRect(0, GetScaledHeight(2F), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear
            };
            searchView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnSearchAction();
            }));
            _headerView.AddSubview(searchView);

            UIImageView searchIcon = new UIImageView(new CGRect(0, 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle("Search-Icon-White")
            };
            searchView.AddSubview(searchIcon);

            UILabel searchLbl = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                Text = GetI18NValue(DashboardHomeConstants.I18N_Search),
                BackgroundColor = UIColor.Clear
            };
            searchView.AddSubview(searchLbl);

            CGSize searchSize = searchLbl.SizeThatFits(new CGSize(1000F, 1000F));
            ViewHelper.AdjustFrameSetWidth(searchLbl, searchSize.Width);

            ViewHelper.AdjustFrameSetWidth(searchView, GetScaledWidth(12F) + searchIcon.Frame.Width + GetScaledWidth(4F) + searchLbl.Frame.Width);
            ViewHelper.AdjustFrameSetX(searchView, _headerView.Frame.Width - searchView.Frame.Width - GetScaledWidth(16F));

            ViewHelper.AdjustFrameSetX(searchLbl, searchView.Frame.Width - searchLbl.Frame.Width);
            ViewHelper.AdjustFrameSetX(searchIcon, searchLbl.Frame.GetMinX() - GetScaledWidth(4F) - searchIcon.Frame.Width);

            UIView pipeView = new UIView(new CGRect(searchView.Frame.GetMinX() - GetScaledWidth(1F), 0, GetScaledWidth(1F), GetScaledHeight(20F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            _headerView.AddSubview(pipeView);

            CustomUIView addView = new CustomUIView(new CGRect(0, GetScaledHeight(2F), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear
            };
            addView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnAddAccountAction();
            }));
            _headerView.AddSubview(addView);

            UIImageView addIcon = new UIImageView(new CGRect(0, 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(DashboardHomeConstants.Img_AddAcctIconWhite)
            };
            addView.AddSubview(addIcon);

            UILabel addLbl = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                Text = GetI18NValue(DashboardHomeConstants.I18N_Add),
                BackgroundColor = UIColor.Clear
            };
            addView.AddSubview(addLbl);

            CGSize addSize = addLbl.SizeThatFits(new CGSize(1000F, 1000F));
            ViewHelper.AdjustFrameSetWidth(addLbl, addSize.Width);

            ViewHelper.AdjustFrameSetWidth(addView, addIcon.Frame.Width + GetScaledWidth(4F) + addLbl.Frame.Width + GetScaledWidth(12F));
            ViewHelper.AdjustFrameSetX(addView, pipeView.Frame.GetMinX() - addView.Frame.Width);

            ViewHelper.AdjustFrameSetX(addIcon, 0);
            ViewHelper.AdjustFrameSetX(addLbl, addIcon.Frame.GetMaxX() + GetScaledWidth(4F));

            UIView lineView = new UIView(new CGRect(0, _headerView.Frame.Height - GetScaledHeight(1F), ViewWidth, GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            _headerView.AddSubview(lineView);
        }

        private void SetFooterView(bool allAcctsAreVisible = false)
        {
            if (_footerView != null)
            {
                _footerView.RemoveFromSuperview();
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

            UILabel moreAcctsLabel = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                Text = allAcctsAreVisible ? "Show Less" : "More Accounts"
            };
            moreLessView.AddSubview(moreAcctsLabel);
            UIImageView arrowUpDown = new UIImageView(new CGRect(moreAcctsLabel.Frame.GetMaxX(), 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(allAcctsAreVisible ? "Arrow-Up-White-Small" : "Arrow-Down-White-Small")
            };
            moreLessView.AddSubview(arrowUpDown);

            CGSize lblSize = moreAcctsLabel.SizeThatFits(new CGSize(1000F, 1000F));
            ViewHelper.AdjustFrameSetWidth(moreAcctsLabel, lblSize.Width);
            ViewHelper.AdjustFrameSetX(arrowUpDown, moreAcctsLabel.Frame.GetMaxX() + GetScaledWidth(4F));

            ViewHelper.AdjustFrameSetWidth(moreLessView, moreAcctsLabel.Frame.Width + GetScaledWidth(4F) + arrowUpDown.Frame.Width);
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
                    Image = UIImage.FromBundle("Rearrange-Icon")
                };
                rearrangeView.AddSubview(iconR);
                UILabel rearrangeLabel = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
                {
                    Font = TNBFont.MuseoSans_12_500,
                    TextColor = UIColor.White,
                    Text = "Rearrange Accounts"
                };
                rearrangeView.AddSubview(rearrangeLabel);

                CGSize lblRSize = rearrangeLabel.SizeThatFits(new CGSize(1000F, 1000F));
                ViewHelper.AdjustFrameSetWidth(rearrangeLabel, lblRSize.Width);
                ViewHelper.AdjustFrameSetX(rearrangeLabel, iconR.Frame.GetMaxX() + GetScaledWidth(8F));

                ViewHelper.AdjustFrameSetWidth(rearrangeView, iconR.Frame.Width + GetScaledWidth(8F) + rearrangeLabel.Frame.Width);
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
            _accountListTableView = new UITableView(new CGRect(0, _headerView.Frame.GetMaxY()
                , ViewWidth, _parentView.Frame.Height - _headerView.Frame.Height))
            { BackgroundColor = UIColor.Clear, ScrollEnabled = false };
            _accountListTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _accountListTableView.RegisterClassForCellReuse(typeof(AccountListCell), DashboardHomeConstants.Cell_AccountList);
            _parentView.AddSubview(_accountListTableView);
        }

        private void ReloadTableView()
        {
            if (_accountListTableView != null)
            {
                _accountListTableView.Source = new AccountListDataSource(DataManager.DataManager.SharedInstance.ActiveAccountList, GetI18NValue);
                _accountListTableView.ReloadData();
            }
        }
        #endregion

        #region API/Logic Methods
        private void PrepareAccounts()
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
                _accountList = _dashboardHomeHelper.GeAccountList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
                DataManager.DataManager.SharedInstance.ActiveAccountList = new List<DueAmountDataModel>();
                DataManager.DataManager.SharedInstance.ActiveAccountList = GetBatchAccountList(_accountList, ref acctNumList, true);
            }
            if (acctNumList.Count > 0)
            {
                GetAccountsBillSummary(acctNumList);
            }
        }

        private List<DueAmountDataModel> GetBatchAccountList(List<DueAmountDataModel> inactiveList, ref List<string> acctNumList, bool isReset = false)
        {
            List<DueAmountDataModel> batchList = new List<DueAmountDataModel>();
            if (inactiveList != null &&
                inactiveList.Count > 0)
            {
                int ctr = 0;
                int maxAcctLimit = isReset ? 3 : 5;
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

        private void GetAccountsBillSummary(List<string> accounts)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        InvokeInBackground(async () =>
                        {
                            AmountDueStatusResponseModel response = await ServiceCall.GetAccountsBillSummary(accounts);
                            InvokeOnMainThread(() =>
                            {
                                if (response != null &&
                                    response.d != null &&
                                    response.d.IsSuccess &&
                                    response.d.data != null &&
                                    response.d.data?.Count > 0)
                                {
                                    _homeViewController.ShowRefreshScreen(false, null);
                                    UpdateDueForDisplayedAccounts(response.d.data);
                                    ReloadViews();
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

        private void ReloadViews()
        {
            ReloadTableView();
            if (_homeViewController != null)
            {
                _homeViewController.OnUpdateCell(0);
            }
            ViewHelper.AdjustFrameSetHeight(_parentView, _dashboardHomeHelper.GetHeightForAccountList() - GetScaledHeight(24F));
            ViewHelper.AdjustFrameSetHeight(_accountListTableView, _parentView.Frame.Height - _headerView.Frame.Height);
            SetFooterView(_dashboardHomeHelper.AllAccountsAreVisible);
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
            Debug.WriteLine("OnSearchAction()");
        }
        private void OnShowMoreAction()
        {
            PrepareAccounts();
        }
        private void OnShowLessAction()
        {
            Debug.WriteLine("OnShowLessAction()");
            DataManager.DataManager.SharedInstance.ActiveAccountList = new List<DueAmountDataModel>();
            PrepareAccounts();
        }
        private void OnRearrangeAction()
        {
            Debug.WriteLine("OnRearrangeAction()");
        }
        #endregion
    }
}
