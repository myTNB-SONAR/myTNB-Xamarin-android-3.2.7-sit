using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MakePayment.Model;
using myTNB_Android.Src.MakePayment.Models;

namespace myTNB_Android.Src.MakePayment.MVP
{
    public class SelectPaymentMethodContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show error message : common for all methods
            /// </summary>
            void ShowErrorMessage(string message);

            /// <summary>
            /// Check payable amount : vaidate payable amount before user click submit
            /// </summary>
            bool IsValidPayableAmount();

            /// <summary>
            /// Start add new card process 
            /// </summary>
            void AddNewCard();
            /// <summary>
            /// Enter CVV number
            /// </summary>
            void EnterCVVNumber(CreditCard card);

            /// <summary>
            /// Show progress dialog for payment request
            /// </summary>
            void ShowPaymentRequestDialog();

            /// <summary>
            /// Hide payment dialog for payment request
            /// </summary>
            void HidePaymentRequestDialog();

            /// <summary>
            /// Save Initiate payment response to start payment flow with payment gateway
            /// </summary>
            void SaveInitiatePaymentResponse(InitiatePaymentResponse response);

            /// <summary>
            /// Initate payment request
            /// </summary>
            void InitiatePaymentRequest();

            /// <summary>
            /// Submit payment request
            /// </summary>
            void InitiateSubmitPayment(InitiatePaymentResponse response, CardDetails card);

            /// <summary>
            /// Start web view for credit card payment
            /// </summary>
            void InitiateWebView(string html);

            /// <summary>
            /// launch external web view to start FPX payment
            /// </summary>
            void InitiateFPXPayment(InitiatePaymentResponse response);

            /// <summary>
            /// Show progress dialog for get registered cards 
            /// </summary>
            void ShowGetRegisteredCardDialog();

            /// <summary>
            /// Hide progress dialog for get registered cards 
            /// </summary>
            void HideGetRegisteredCardDialog();

            /// <summary>
            /// Show get registered card sucess with registered cards response
            /// </summary>

            void GetRegisterCardsResult(GetRegisteredCardsResponse response);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Service call for payment request, this api is called to initiate payment request Api : RequestPayBill
            /// </summary>
            void RequestPayment(string apiKeyID, string custName, string accNum, string payAm, string custEmail, string custPhone, string sspUserID, string platform, string platformMode, string registeredCardId);

            /// <summary>
            /// Submit payment method will call payment gateway Api : SubmitPaymentPG
            /// </summary>
            void SubmitPayment(string apiKeyID, string merchantId, string accNum, string payAm, string custName, string custEmail, string custPhone, string mParam1, string des, string cardNo, string cardName, string expM, string expY, string cvv);

            /// <summary>
            /// Get customer registered cards Api : GetRegisteredCards
            /// </summary>
            void GetRegisterdCards(string apiKeyID, string email);
        }
    }
}

    