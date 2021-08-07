using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myTNB.Mobile;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;

namespace myTNB_Android.Src.SessionCache
{
    internal sealed class AccountTypeCache
    {
        private static readonly Lazy<AccountTypeCache> lazy
            = new Lazy<AccountTypeCache>(() => new AccountTypeCache());

        internal static AccountTypeCache Instance { get { return lazy.Value; } }

        public AccountTypeCache()
        {
        }

        internal List<string> DBREligibleCAs { private set; get; }
        internal List<GetBillRenderingModel> MultiBillRenderingContent { private set; get; }

        internal List<string> GetDBRAccountList()
        {
            try
            {
                List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
                List<string> caList = allAccountList.Where(x =>
                    x.isOwned
                    && x.SmartMeterCode != "0")
                    .Select(y => y.AccNum).ToList();
                return caList;
            }
            catch (Exception e)
            {
                Console.WriteLine("[Console] GetDBRAccountList Error: " + e.Message);
            }
            return new List<string>();
        }

        internal void UpdateCATariffType(List<string> caList)
        {
            if (caList == null || caList.Count == 0)
            {
                return;
            }
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            for (int i = 0; i < caList.Count; i++)
            {
                int index = allAccountList.FindIndex(x => x.AccNum == caList[i]);
                if (index > -1)
                {
                    //Todo: Add to db?
                }
            }
        }

        internal void UpdateCARendering(List<GetBillRenderingModel> multiBillRenderingContent
            , List<string> caList)
        {
            DBREligibleCAs = new List<string>();
            for (int i = 0; i < caList.Count; i++)
            {
                int index = multiBillRenderingContent.FindIndex(x => x.ContractAccountNumber == caList[i]);
                if (index > -1 && multiBillRenderingContent[index].DBRType != myTNB.Mobile.MobileEnums.DBRTypeEnum.None)
                {
                    DBREligibleCAs.Add(caList[i]);
                }
            }
            SetMultiBillRenderingContent(multiBillRenderingContent);
        }

        internal void SetMultiBillRenderingContent(List<GetBillRenderingModel> multiBillRenderingContent)
        {
            if (multiBillRenderingContent != null && multiBillRenderingContent.Count > 0)
            {
                this.MultiBillRenderingContent = multiBillRenderingContent;
            }
        }

        internal GetBillRenderingModel GetFirstRenderingResponse()
        {
            if (MultiBillRenderingContent != null && MultiBillRenderingContent.Count > 0)
            {
                int index = MultiBillRenderingContent.FindIndex(x => x.ContractAccountNumber == DBREligibleCAs[0]);
                if (index > -1)
                {
                    return MultiBillRenderingContent[index];
                }
            }
            return null;
        }

        internal async Task<ApplicationPaymentDetail> UpdateApplicationPayment(ApplicationPaymentDetail applicationPaymentDetail
            , BaseAppCompatActivity activity)
        {
            if (DBRUtility.Instance.IsAccountDBREligible
                && applicationPaymentDetail != null
                && !string.IsNullOrEmpty(applicationPaymentDetail.caNo)
                && !string.IsNullOrWhiteSpace(applicationPaymentDetail.caNo))
            {
                string caNumber = applicationPaymentDetail.caNo;

                List<string> caList = EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                        , EligibilitySessionCache.FeatureProperty.TargetGroup)
                    ? DBRUtility.Instance.GetDBRCAs()
                    : DBREligibleCAs;
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

        internal void Clear()
        {
            DBREligibleCAs = new List<string>();
            MultiBillRenderingContent = new List<GetBillRenderingModel>();
        }
    }
}