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
        private bool IsNeeUpdate = false;
        private bool IsMaintenanceShown = false;
        private bool IsNotificationFailed = false;
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

        public int IsHasSMAccountCount()
        {
            return CustomerBillingAccount.SMAccountList().Count;
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

        public bool IsAccountNumberExist(string accountNumber)
        {
            return (CustomerBillingAccount.FindByAccNum(accountNumber) != null);
        }

        public string GetNotificationAccountName(string accountNumber)
        {
            string notificationAccountName = string.Empty;
            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountNumber);
            if (customerBillingAccount != null)
            {
                if (!string.IsNullOrEmpty(customerBillingAccount.AccDesc))
                {
                    notificationAccountName = customerBillingAccount.AccDesc;
                }
                else
                {
                    notificationAccountName = "Customer Account Number " + customerBillingAccount.AccNum;
                }
            }
            return notificationAccountName;
        }

        public bool IsMaintenanceDialogShown()
        {
            return IsMaintenanceShown;
        }

        public void SetIsMaintenanceDialogShown(bool isShown)
        {
            IsMaintenanceShown = isShown;
        }

        public bool IsNotificationServiceFailed()
        {
            return IsNotificationFailed;
        }

        public void SetIsNotificationServiceFailed(bool isShown)
        {
            IsNotificationFailed = isShown;
        }
    }
}
