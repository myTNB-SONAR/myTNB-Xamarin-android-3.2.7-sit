using System;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.Model.Usage;

namespace myTNB
{
    public static class UsageServiceCall
    {
        /// <summary>
        /// API Call for GetAccountUsage
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<AccountUsageResponseModel> GetAccountUsage(CustomerAccountRecordModel account)
        {
            AccountUsageResponseModel accountUsageResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwner = account.isOwned,
                serviceManager.usrInf
            };

            accountUsageResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<AccountUsageResponseModel>("GetAccountUsage", requestParameter);
            });

            return accountUsageResponse;
        }

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
            object request = new
            {
                contractAccount = account.accNum,
                isOwnedAccount = account.isOwned,
                serviceManager.usrInf
            };
            smrActivityInfoResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<SMRAccountActivityInfoResponseModel>("GetSMRAccountActivityInfo", request);
            });

            return smrActivityInfoResponse;
        }
    }
}
