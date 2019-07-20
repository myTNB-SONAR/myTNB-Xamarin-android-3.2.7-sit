using CoreGraphics;
using myTNB.Dashboard;
using myTNB.DataManager;
using myTNB.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class AccountsContentViewController : CustomUIViewController
    {
        public int pageIndex = 0;
        List<string> _accountNumberList = new List<string>();
        public List<List<DueAmountDataModel>> _groupAccountList;
        UIView _viewContainer;
        public DashboardHomeViewController _homeViewController;

        public AccountsContentViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (_groupAccountList.Count > 0)
            {
                AddUpdateCards();
                if (pageIndex > -1 && pageIndex < _groupAccountList.Count)
                {
                    var groupAccountList = _groupAccountList[pageIndex];
                    for (int i = 0; i < groupAccountList.Count; i++)
                    {
                        if (i > -1 && i < groupAccountList.Count)
                        {
                            _accountNumberList.Add(groupAccountList[i].accNum);
                        }
                    }
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (_groupAccountList.Count > 0)
            {
                LoadAccountsWithDues();
            }
        }

        private void AddUpdateCards()
        {
            if (_groupAccountList.Count <= 0)
                return;

            if (_viewContainer != null)
            {
                _viewContainer.RemoveFromSuperview();
            }
            _viewContainer = new UIView
            {
                BackgroundColor = UIColor.Clear
            };

            if (pageIndex > -1 && pageIndex < _groupAccountList.Count)
            {
                var groupAccountList = _groupAccountList[pageIndex];
                _viewContainer.Frame = new CGRect(0, 0, View.Frame.Width, 68 * groupAccountList.Count + 16f * 2);
                for (int i = 0; i < groupAccountList.Count; i++)
                {
                    DashboardHomeAccountCard _homeAccountCard = new DashboardHomeAccountCard(this, _viewContainer, 68f * i);
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
                    _viewContainer.AddSubview(_homeAccountCard.GetUI());
                    _homeAccountCard.AdjustLabels(groupAccountList[i]);
                    _homeAccountCard.SetModel(groupAccountList[i]);
                }
                View.AddSubview(_viewContainer);
            }
        }

        private void UpdateDueForDisplayedAccounts(List<DueAmountDataModel> dueDetails)
        {
            if (_groupAccountList.Count <= 0)
                return;

            if (pageIndex > -1 && pageIndex < _groupAccountList.Count)
            {
                var groupAccountList = _groupAccountList[pageIndex];

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
        }

        private async Task<bool> GetAccountsSummary(List<string> accounts, bool willGetNew = false)
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
                            //ActivityIndicator.Show();
                            await GetAccountsSummary(accounts);
                            AddUpdateCards();
                            //ActivityIndicator.Hide();
                        }
                        else if (shouldReload)
                        {
                            AddUpdateCards();
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
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

            if (pageIndex > -1 && pageIndex < _groupAccountList.Count)
            {
                var groupAccountList = _groupAccountList[pageIndex];

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

            if (pageIndex > -1 && pageIndex < _groupAccountList.Count)
            {
                List<string> keysToDelete = new List<string>();
                var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
                var groupAccountList = _groupAccountList[pageIndex];

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
            }

            return removedAccounts;
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
    }
}