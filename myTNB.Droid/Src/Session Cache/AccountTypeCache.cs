using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.Mobile;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.AWS.Models;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.DeviceCache;

namespace myTNB.AndroidApp.Src.SessionCache
{
    internal sealed class AccountTypeCache
    {
        private static readonly Lazy<AccountTypeCache> lazy
            = new Lazy<AccountTypeCache>(() => new AccountTypeCache());

        internal static AccountTypeCache Instance { get { return lazy.Value; } }

        public AccountTypeCache()
        {
        }

        internal async Task<ApplicationPaymentDetail> UpdateApplicationPayment(ApplicationPaymentDetail applicationPaymentDetail
            , BaseAppCompatActivity activity)
        {
            if (DBRUtility.Instance.IsAccountEligible
                && applicationPaymentDetail != null
                && !string.IsNullOrEmpty(applicationPaymentDetail.caNo)
                && !string.IsNullOrWhiteSpace(applicationPaymentDetail.caNo))
            {
                string caNumber = applicationPaymentDetail.caNo;

                List<string> caList = DBRUtility.Instance.GetCAList();
                int index = caList.FindIndex(x => x == caNumber);
                if (index > -1)
                {
                    GetBillRenderingResponse billRenderingResponse = await DBRManager.Instance.GetBillRendering(caNumber
                        , AccessTokenCache.Instance.GetAccessToken(activity));
                    if (billRenderingResponse != null
                        && billRenderingResponse.StatusDetail != null
                        && billRenderingResponse.StatusDetail.IsSuccess
                        && billRenderingResponse.Content != null)
                    {
                        applicationPaymentDetail.dbrEnabled = billRenderingResponse.Content.DBRType == MobileEnums.DBRTypeEnum.Paper;
                    }
                }
            }
            return applicationPaymentDetail;
        }
    }
}