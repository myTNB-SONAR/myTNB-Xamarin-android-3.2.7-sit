using System;
using System.Collections.Generic;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;

namespace myTNB_Android.Src.Base
{
    public class MyTNBAccountManagement
    {
        private static MyTNBAccountManagement Instance = null;
        private List<CustomerBillingAccount> masterCustomerBillingAccountList = new List<CustomerBillingAccount>();
        private bool HasLoaded = false;
        private bool IsNeeUpdate = false;
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

        public bool IsSMROnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMROnboardingShown();
        }

        public void UpdateIsSMRMeterReadingOnboardingShown()
        {
            CustomerBillingAccount.UpdateIsSMRMeterReadingOnBoardShown();
        }

        public bool IsSMRMeterReadingOnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMRMeterReadingOnBoardShown();
        }
    }
}
