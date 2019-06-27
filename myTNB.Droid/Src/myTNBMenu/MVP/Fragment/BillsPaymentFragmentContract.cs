using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class BillsPaymentFragmentContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows billing list
            /// </summary>
            /// <param name="billsHistoryResponse">BillHistoryResponseV5</param>
            void ShowBillsList(BillHistoryResponseV5 billsHistoryResponse);

            /// <summary>
            /// Shows payment list
            /// </summary>
            /// <param name="paymentHistoryResponse">PaymentHistoryResponseV5</param>
            void ShowPaymentList(PaymentHistoryResponseV5 paymentHistoryResponse);

            /// <summary>
            /// Shows payment list
            /// </summary>
            /// <param name="paymentHistoryResponse">PaymentHistoryResponseV5</param>
            void ShowREPaymentList(PaymentHistoryREResponse paymentHistoryREResponse);

            /// <summary>
            /// Show empty bill list
            /// </summary>
            void ShowEmptyBillList();

            /// <summary>
            /// Shows empty payment list
            /// </summary>
            void ShowEmptyPaymentList();

            /// <summary>
            /// Shows no internet connection
            /// </summary>
            void ShowNoInternet();
            
            /// <summary>
            /// Enable bill and payment tab
            /// </summary>
            void EnableTabs();

            /// <summary>
            /// Disables bill and payment tab
            /// </summary>
            void DisableTabs();

            /// <summary>
            /// Show payment
            /// </summary>
            void ShowPayment();

            void ShowView();

            void ShowRefreshView(string contentTxt, string btnTxt);

            void ToggleFetch(bool yesno);

            /// <summary>
            /// Returns connectivity
            /// </summary>
            /// <returns>bool</returns>
            bool HasNoInternet();

            /// <summary>
            /// Shows the bill menu as account RE
            /// </summary>
            void ShowAccountRE();

            /// <summary>
            /// Shows the bill menu as normal account
            /// </summary>
            void ShowNormalAccount();


            void SetBillDetails(AccountData selectedAccount);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to show bills
            /// </summary>
            void OnShowBills();

            /// <summary>
            /// Action to show payment
            /// </summary>
            void OnShowPayment();

            /// <summary>
            /// Action to pay
            /// </summary>
            void OnPay();

            /// <summary>
            /// Action on bill tab
            /// </summary>
            void OnBillTab();

            /// <summary>
            /// Action on payment tab
            /// </summary>
            void OnPaymentTab();

            void LoadBills(CustomerBillingAccount accountSelected);

            void RefreshData();
        }
    }
}