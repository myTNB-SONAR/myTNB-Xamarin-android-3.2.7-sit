using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.ManageCards.Models;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.SSMR.SMRApplication.MVP;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.EnergyBudget.MVP
{
    public class EnergyBudgetContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            /// <summary>
            /// Shows a progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideShowProgressDialog();

            /// <summary>
            /// Clears the account adapter
            /// </summary>
            void ClearAccountsAdapter();

            /// <summary>
            /// Shows account list
            /// </summary>
            /// <param name="accountList">List<paramref name="SMRAccount"/></param>
            void ShowAccountList(List<SMRAccount> accountList);

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


        }
    }
}