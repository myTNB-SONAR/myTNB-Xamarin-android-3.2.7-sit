using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Preferences;
using Android.Util;
using myTNB.Mobile;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.SessionCache;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Async
{
    public class CustomEligibility
    {
        private static readonly Lazy<CustomEligibility> lazy =
            new Lazy<CustomEligibility>(() => new CustomEligibility());
        public static CustomEligibility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public CustomEligibility() { }

        public List<ContractAccountModel> GetContractAccountList(List<string> accNumList = null)
        {
            List<CustomerBillingAccount> accountList;
            List<ContractAccountModel> contractAccountList = new List<ContractAccountModel>();

            if (accNumList != null && accNumList.Count > 0)
            {
                accountList = CustomerBillingAccount.List();
                accNumList.ForEach(accNum =>
                {
                    var account = CustomerBillingAccount.FindByAccNum(accNum);
                    if (account != null)
                    {
                        contractAccountList.Add(GetAccountModel(account));
                    }
                });
            }
            else
            {
                accountList = CustomerBillingAccount.List();
                accountList.ForEach(account =>
                {
                    contractAccountList.Add(GetAccountModel(account));
                });
            }

            return contractAccountList;
        }

        private ContractAccountModel GetAccountModel(CustomerBillingAccount account)
        {
            try
            {
                ContractAccountModel accountModel = new ContractAccountModel
                {
                    accNum = account.AccNum,
                    userAccountID = account.UserAccountId,
                    accDesc = account.AccDesc,
                    icNum = account.ICNum,
                    amCurrentChg = account.AmtCurrentChg.IsValid() ? double.Parse(account.AmtCurrentChg) : 0,
                    isRegistered = account.IsRegistered.ToString(),
                    isOwned = account.isOwned.ToString(),
                    isPaid = account.IsPaid.ToString(),
                    isError = account.IsError.ToString(),
                    message = null,
                    accountTypeId = account.AccountTypeId,
                    accountStAddress = account.AccountStAddress,
                    ownerName = account.OwnerName,
                    accountCategoryId = account.AccountCategoryId,
                    SmartMeterCode = account.SmartMeterCode,
                    isTaggedSMR = account.IsTaggedSMR.ToString(),
                    BudgetAmount = account.BudgetAmount.IsValid() ? Int32.Parse(account.BudgetAmount) : 0,
                    InstallationType = account.InstallationType,
                    IsApplyEBilling = account.IsApplyEBilling,
                    IsHaveAccess = account.IsHaveAccess,
                    BusinessArea = account.BusinessArea,
                    RateCategory = account.RateCategory
                };
                return accountModel;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                return new ContractAccountModel();
            }
        }

        public async Task<bool> EvaluateEligibility(Context mView, bool isForceCall)
        {
            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(mView);
#pragma warning restore CS0618 // Type or member is obsolete
                string eligibilityTimeStamp = preferences.GetString(MobileConstants.SharePreferenceKey.GetEligibilityTimeStamp, string.Empty);

                if (EligibilityManager.Instance.ShouldCallApi(AWSConstants.Services.GetEligibility
                   , eligibilityTimeStamp) || isForceCall)
                {
                    if (!AccessTokenCache.Instance.HasTokenSaved(mView))
                    {
                        string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                        AccessTokenCache.Instance.SaveAccessToken(mView, accessToken);
                    }

                    GetEligibilityResponse response = await EligibilityManager.Instance.PostEligibility(UserEntity.GetActive().UserID ?? string.Empty,
                        GetContractAccountList(), AccessTokenCache.Instance.GetAccessToken(mView));

                    //Nullity Check
                    if (response != null
                       && response.StatusDetail != null
                       && response.StatusDetail.IsSuccess)
                    {
                        EligibilitySessionCache.Instance.SetData(response);
                        DateTime now = DateTime.Now;
                        string encryptedData = SecurityManager.Instance.Encrypt(response);
                        ISharedPreferencesEditor editor = preferences.Edit();
                        editor.PutString(MobileConstants.SharePreferenceKey.GetEligibilityData, encryptedData);
                        editor.PutString(MobileConstants.SharePreferenceKey.GetEligibilityTimeStamp, now.ToString());
                        editor.Apply();

                        FeatureInfoManager.Instance.SetData(response);
                        MyTNBAccountManagement.GetInstance().SetFinishApiEB(true);
                        //Use data or any EligibilitySessionCache functionality
                    }
                    else
                    {
                        MyTNBAccountManagement.GetInstance().SetFinishApiEB(true);
                    }
                }
                else if (EligibilityManager.Instance.IsEnabled(AWSConstants.Services.GetEligibility)
                    && preferences.GetString(MobileConstants.SharePreferenceKey.GetEligibilityData, string.Empty) is string encryptedData
                    && !string.IsNullOrEmpty(encryptedData)
                    && !string.IsNullOrWhiteSpace(encryptedData))
                {
                    GetEligibilityResponse data = SecurityManager.Instance.Decrypt<GetEligibilityResponse>(encryptedData);
                    EligibilitySessionCache.Instance.SetData(data);
                    FeatureInfoManager.Instance.SetData(data);
                    MyTNBAccountManagement.GetInstance().SetFinishApiEB(true);
                    //Use data or any EligibilitySessionCache functionality
                }

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Eligibility API Error: " + e.Message);
            }
            return true;
        }
    }
}