using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Model;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.MyTNBService.Request.PaymentTransactionIdRequest;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.AndroidApp.Src.Base.Models;

namespace myTNB.AndroidApp.Src.MultipleAccountPayment.MVP
{
    public class MPSelectPaymentMethodContract
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
            void EnterCVVNumber(CreditCard card); // -- CVV enabled --

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
            //void SaveInitiatePaymentResponse(MPInitiatePaymentResponse response);

            /// <summary>
            /// Initate payment request
            /// </summary>
            void InitiatePaymentRequest();

            /// <summary>
            /// Submit payment request
            /// </summary>
            void InitiateSubmitPayment(PaymentTransactionIdResponse response, MPCardDetails card);

            /// <summary>
            /// Start web view for credit card payment
            /// </summary>
            void InitiateWebView(string html);

            /// <summary>
            /// launch external web view to start FPX payment
            /// </summary>
            void InitiateFPXPayment(PaymentTransactionIdResponse response);

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

            void GetRegisterCardsResult(RegisteredCardsResponse response);

            /// <summary>
            /// NEW API RESPONSE
            /// Save Initiate payment response to start payment flow with payment gateway
            /// </summary>
            void SetInitiatePaymentResponse(PaymentTransactionIdResponse response);

            void ShowErrorMessageWithOK(string message);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Service call for payment request, this api is called to initiate payment request Api : RequestPayBill
            /// </summary>
            void RequestPayment(string apiKeyID, string custName, string custEmail, string custPhone, string sspUserID, string platform, string registeredCardId, string platformMode, string totalAmount, List<PaymentItems> paymentItems);

            /// <summary>
            /// Get customer registered cards Api : GetRegisteredCards
            /// </summary>
            void GetRegisterdCards(string apiKeyID, string email);

            /// <summary>
            /// Service call for initializing payment, this api is called to initiate payment request Api : GetPaymentTransactionId
            /// </summary>
            void InitializePaymentTransaction(DeviceInterface deviceInf, string custName, string custPhone, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItem> paymentItems, string applicationType, string applicationRefNo);

            void InitializeApplicationPaymentTransaction(object userInfo
                , string customerName
                , string phoneNo
                , string osType
                , string registeredCardId
                , string paymentMode
                , string totalAmount
                , string applicationType
                , string searchTerm
                , string system
                , string statusId
                , string statusCode
                , ApplicationPaymentDetail applicationPaymentDetail);
        }
    }
}