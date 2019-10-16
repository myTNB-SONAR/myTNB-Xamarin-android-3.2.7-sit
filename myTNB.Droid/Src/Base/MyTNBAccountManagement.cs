using System;
using System.Collections.Generic;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Base
{
    public class MyTNBAccountManagement
    {
        private static MyTNBAccountManagement Instance = null;
        private List<CustomerBillingAccount> masterCustomerBillingAccountList = new List<CustomerBillingAccount>();
        private bool HasLoaded = false;
        private bool IsNeeUpdate = false;
        private static MasterDataResponse currentMasterDataRes = null;
        private List<string> UpdatedAccountNumberList = new List<string>();
        private MyTNBAccountManagement()
        {
        }

        public static MyTNBAccountManagement GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MyTNBAccountManagement();
            }
            return Instance;
        }

        public bool IsCustomerBillingAccountsLoaded()
        {
            return CustomerBillingAccount.List().Count > 0;
        }

        public List<CustomerBillingAccount> GetEligibleSMRBillingAccounts()
        {
            return CustomerBillingAccount.EligibleSMRAccountList();
        }

        public void SetSMRCustomerBillingAccounts()
        {
            masterCustomerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            List<CustomerBillingAccount> smrEligible = CustomerBillingAccount.EligibleSMRAccountList();
            CustomerBillingAccount.Replace(smrEligible);
        }

        public void SetMasterCustomerBillingAccountList()
        {
            if (masterCustomerBillingAccountList.Count > 0)
            {
                CustomerBillingAccount.Replace(masterCustomerBillingAccountList);
            }
            else
            {
                CustomerBillingAccount.Replace(CustomerBillingAccount.GetSortedCustomerBillingAccounts());
            }
        }

        public void UpdateCustomerBillingDetails(List<SummaryDashBoardAccountEntity> summaryDetails)
        {
            CustomerBillingAccount.UpdateBillingDetails(summaryDetails);
            IsNeeUpdate = false;
            foreach (SummaryDashBoardAccountEntity entity in summaryDetails)
            {
                if (!UpdatedAccountNumberList.Contains(entity.AccountNo))
                {
                    UpdatedAccountNumberList.Add(entity.AccountNo);
                }
            }
        }

        public bool HasUpdatedBillingDetails(string accountNumber)
        {
            //return CustomerBillingAccount.HasUpdatedBillingDetails(accountNumber);
            return UpdatedAccountNumberList.Contains(accountNumber);
        }

        public bool IsNeedUpdatedBillingDetails()
        {
            return IsNeeUpdate;
        }

        public void RemoveCustomerBillingDetails()
        {
            CustomerBillingAccount.RemoveCustomerBillingDetails();
            UpdatedAccountNumberList = new List<string>();
            IsNeeUpdate = true;
        }

        public void UpdateIsSMROnboardingShown()
        {
            CustomerBillingAccount.UpdateIsSMROnboardingShown();
        }

        public void UpdateIsPayBillShown()
        {
            CustomerBillingAccount.UpdateIsPayBillShown();
        }

        public void UpdateIsViewBillShown()
        {
            CustomerBillingAccount.UpdateIsViewBillShown();
        }

        public bool IsSMROnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMROnboardingShown();
        }

        public bool IsPayBillShown()
        {
            return CustomerBillingAccount.GetIsPayBillShown();
        }

        public bool IsViewBillShown()
        {
            return CustomerBillingAccount.GetIsViewBillShown();
        }

        public bool IsPayBillEnabledNeeded()
        {
            return CustomerBillingAccount.GetIsPayBillEnabled();
        }

        public bool IsViewBillEnabledNeeded()
        {
            return CustomerBillingAccount.GetIsViewBillEnabled();
        }

        public int IsHasNonREAccountCount()
        {
            return CustomerBillingAccount.NonREAccountList().Count;
        }

        public int IsHasREAccountCount()
        {
            return CustomerBillingAccount.REAccountList().Count;
        }

        public void UpdateIsSMRMeterReadingOnePhaseOnboardingShown(bool flag)
        {
            if (flag)
            {
                CustomerBillingAccount.SetIsSMRMeterReadingOnePhaseOnBoardShown();
            }
            else
            {
                CustomerBillingAccount.UnSetIsSMRMeterReadingOnePhaseOnBoardShown();
            }
        }

        public bool GetSMRMeterReadingOnePhaseOnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMRMeterReadingOnePhaseOnBoardShown();
        }

        public void UpdateIsSMRMeterReadingThreePhaseOnboardingShown(bool flag)
        {
            if (flag)
            {
                CustomerBillingAccount.SetIsSMRMeterReadingThreePhaseOnBoardShown();
            }
            else
            {
                CustomerBillingAccount.UnSetIsSMRMeterReadingThreePhaseOnBoardShown();
            }
        }

        public bool GetSMRMeterReadingThreePhaseOnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMRMeterReadingThreePhaseOnBoardShown();
        }

        public void SetAccountActivityInfo(SMRAccountActivityInfo smrAccountActivityInfo)
        {
            List<SMRAccountActivityInfo> accountActivityInfoList = new List<SMRAccountActivityInfo>();
            if (UserSessions.GetAccountActivityInfoList().Count > 0)
            {
                accountActivityInfoList = UserSessions.GetAccountActivityInfoList();
                int itemIndex = accountActivityInfoList.FindIndex(x => x.GetAccountNumber() == smrAccountActivityInfo.GetAccountNumber());
                if (itemIndex != -1)
                {
                    accountActivityInfoList[itemIndex] = smrAccountActivityInfo;
                }
                else
                {
                    accountActivityInfoList.Add(smrAccountActivityInfo);
                }
            }
            else
            {
                accountActivityInfoList.Add(smrAccountActivityInfo);
                UserSessions.SetAccountActivityInfoList(accountActivityInfoList);
            }
        }

        public SMRAccountActivityInfo GetAccountActivityInfoByAccountNumber(string accountNumber)
        {
            SMRAccountActivityInfo accountActivityInfo = null;

            List<SMRAccountActivityInfo> accountActivityInfoList = new List<SMRAccountActivityInfo>();
            if (UserSessions.GetAccountActivityInfoList().Count > 0)
            {
                accountActivityInfoList = UserSessions.GetAccountActivityInfoList();
                int itemIndex = accountActivityInfoList.FindIndex(x => x.GetAccountNumber() == accountNumber);
                if (itemIndex != -1)
                {
                    accountActivityInfo = accountActivityInfoList[itemIndex];
                }
            }
            return accountActivityInfo;
        }

        public void SetIsSMRTakePhotoOnBoardShown()
        {
            CustomerBillingAccount.SetIsSMRTakePhotoOnBoardShown();
        }

        public bool GetIsSMRTakePhotoOnBoardShown()
        {
            return CustomerBillingAccount.GetIsSMRTakePhotoOnBoardShown();
        }

        public void SetCurentMasterData(MasterDataResponse data)
        {
            currentMasterDataRes = data;
        }

        public MasterDataResponse GetCurrentMasterData()
        {
            return currentMasterDataRes;
        }
    }
}
