using System;
using System.Collections.Generic;
using Force.DeepCloner;
using myTNB.Model;
using myTNB.SQLite.SQLiteDataManager;

namespace myTNB
{
    public sealed class AccountManager
    {
        private static readonly Lazy<AccountManager> lazy = new Lazy<AccountManager>(() => new AccountManager());
        public static AccountManager Instance { get { return lazy.Value; } }

        private List<CustomerAccountRecordModel> CAData = new List<CustomerAccountRecordModel>();

        public void SetAccounts(CustomerAccountRecordListModel accounts, bool isRecursed = false)
        {
            if (accounts != null && accounts.d != null)
            {
                CAData = accounts.d.DeepClone();
            }
            else
            {
                if (isRecursed) { CAData = new List<CustomerAccountRecordModel>(); }
                UserAccountsEntity entity = new UserAccountsEntity();
                CustomerAccountRecordListModel eAccounts = entity.GetCustomerAccountRecordList();
                SetAccounts(eAccounts, true);
            }
        }

        public CustomerAccountRecordModel GetAccountByIndex(int index)
        {
            CustomerAccountRecordModel account = new CustomerAccountRecordModel();
            if (CAData != null && index > -1 && index < CAData.Count)
            {
                account = CAData[index].DeepClone();
            }
            return account;
        }

        public CustomerAccountRecordModel GetAccountByAccountNumber(string accountNumber)
        {
            if (CAData != null && !string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber))
            {
                int index = CAData.FindIndex(x => x.accNum == accountNumber);
                return GetAccountByIndex(index);
            }
            return new CustomerAccountRecordModel();
        }

        public int CurrentAccountIndex { set; get; } = 0;

        public string Nickname
        {
            get { return CurrentAccount.accountNickName ?? string.Empty; }
        }

        public string Address
        {
            get { return CurrentAccount.accountStAddress ?? string.Empty; }
        }

        public CustomerAccountRecordModel CurrentAccount
        {
            get { return GetAccountByIndex(CurrentAccountIndex); }
        }

        public List<CustomerAccountRecordModel> GetREAccount()
        {
            List<CustomerAccountRecordModel> reAccounts = new List<CustomerAccountRecordModel>();
            if (CAData != null)
            {
                reAccounts = CAData.FindAll(x => x.IsREAccount).DeepClone();
            }
            return reAccounts;
        }
    }
}