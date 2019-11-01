using System;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.Model.Usage;

namespace myTNB
{
    public static class UsageServiceCall
    {
        /// <summary>
        /// API Call for GetAccountStatus
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<AccountStatusResponseModel> GetAccountStatus(CustomerAccountRecordModel account)
        {
            AccountStatusResponseModel accountStatusResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwner = account.isOwned,
                serviceManager.usrInf
            };

            accountStatusResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<AccountStatusResponseModel>("GetAccountStatus", requestParameter);
            });

            return accountStatusResponse;
        }

        /// <summary>
        /// API Call for GetSMRAccountActivityInfo
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<SMRAccountActivityInfoResponseModel> GetSMRAccountActivityInfo(CustomerAccountRecordModel account)
        {
            SMRAccountActivityInfoResponseModel smrActivityInfoResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwnedAccount = account.isOwned,
                serviceManager.usrInf
            };
            smrActivityInfoResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<SMRAccountActivityInfoResponseModel>("GetSMRAccountActivityInfo", requestParameter);
            });

            return smrActivityInfoResponse;
        }

        /// <summary>
        /// API Call for GetAccountDueAmount
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<DueAmountResponseModel> GetAccountDueAmount(CustomerAccountRecordModel account)
        {
            DueAmountResponseModel dueAmountResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwnedAccount = account.isOwned,
                serviceManager.usrInf
            };
            dueAmountResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<DueAmountResponseModel>("GetAccountDueAmount", requestParameter);
            });

            return dueAmountResponse;
        }

        /// <summary>
        /// API Call for GetAccountUsageSmart
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<AccountUsageSmartResponseModel> GetAccountUsageSmart(CustomerAccountRecordModel account)
        {
            AccountUsageSmartResponseModel accountUsageSmartResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwner = account.isOwned,
                metercode = account.smartMeterCode,
                serviceManager.usrInf
            };

            accountUsageSmartResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<AccountUsageSmartResponseModel>("GetAccountUsageSmart", requestParameter);
            });

            return accountUsageSmartResponse;
        }
    }
}