﻿using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DBR.DBRApplication.MVP;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            string GetAppString(int id);
            /// <summary>
			/// Show select account activity
			/// </summary>
			void ShowSelectSupplyAccount();
            /// <summary>
			/// Set the account name
			/// </summary>
			/// <param name="accountName">string</param>tive
			void SetAccountName(CustomerBillingAccount selectedAccount);
            void ShowDBREligibleAccountList(List<DBRAccount> dbrAccountList);
            void ShowProgressDialog();
            void HideProgressDialog();
        }

        public interface IPresenter
        {
            List<ManageBillDeliveryModel> GenerateNewWalkthroughList(string currentAppNavigation);
            List<DBRAccount> GetEligibleDBRAccountList();
            void CheckDBRAccountEligibility(List<DBRAccount> dbrAccountList);
        }
        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            /// <summary>
            /// Action to navigate to show SelectvSupply Account
            /// </summary>
            void SelectSupplyAccount(); 
        }
    }
}