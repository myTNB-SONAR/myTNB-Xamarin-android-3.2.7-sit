using System;
using myTNB.Model.Usage;

namespace myTNB
{
    public sealed class AccountStatusCache
    {
        private static readonly Lazy<AccountStatusCache> lazy = new Lazy<AccountStatusCache>(() => new AccountStatusCache());
        public static AccountStatusCache Instance { get { return lazy.Value; } }

        private static AccountStatusDataModel accountStatusData = new AccountStatusDataModel();

        public static void AddAccountStatusData(AccountStatusResponseModel response)
        {
            if (accountStatusData == null)
            {
                accountStatusData = new AccountStatusDataModel();
            }
            if (response != null &&
                response.d != null &&
                response.d.data != null)
            {
                accountStatusData = response.d.data;
            }
        }

        public static void ClearAccountStatusData()
        {
            accountStatusData = null;
            accountStatusData = new AccountStatusDataModel();
        }

        public static AccountStatusDataModel GetAccountStatusData()
        {
            if (accountStatusData != null)
            {
                return accountStatusData;
            }
            return new AccountStatusDataModel();
        }

        public static bool AccountStatusIsAvailable()
        {
            bool res = true;
            if (accountStatusData != null)
            {
                res = accountStatusData.DisconnectionStatus?.ToLower() == "available";
            }
            return res;
        }
    }
}
