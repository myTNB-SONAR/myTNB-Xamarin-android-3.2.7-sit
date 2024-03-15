using System;
using System.Collections.Generic;
using Android.OS;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.MyTNBService.Model;

namespace myTNB.AndroidApp.Src.MyHome.MVP
{
	public class MyHomePaymentHistoryContract
	{
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Set up Views
            /// </summary>
            void SetUpViews();

            /// <summary>
            /// Updates the shimmer loading state UI
            /// </summary>
            /// <param name="show"></param>
            void UpdateShimmerLoadingState(bool show);

            /// <summary>
            /// Shows UI for empty list
            /// </summary>
            void ShowEmptyListWithMessage(string msg);

            /// <summary>
            /// Shows Refresh state
            /// </summary>
            void ShowRefreshStateWithMessage(bool showRefreshButton, string msg, string refreshBtnMsg);

            /// <summary>
            /// Populates the UI for the Payment history list
            /// </summary>
            /// <param name="billingHistoryModelList"></param>
            void PopulatePaymentHistoryList(List<AccountBillPayHistoryModel> billingHistoryModelList);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initializes variables
            /// </summary>
            void OnInitialize(Bundle bundle);

            /// <summary>
            /// Calls the Function to trigger GetAccountBillPayHistoryV4 API call
            /// </summary>
            void GetAccountBillPayHistory();

            /// <summary>
            /// Gets the Contract Account number
            /// </summary>
            /// <returns></returns>
            string GetContractAccount();
        }
    }
}

