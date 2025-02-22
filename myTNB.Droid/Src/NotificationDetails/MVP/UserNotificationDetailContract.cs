﻿using System;
using myTNB;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyHome;
using myTNB.AndroidApp.Src.MyHome.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.SSMRMeterHistory.MVP;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;

using MyHomeModel = myTNB.AndroidApp.Src.MyHome.Model.MyHomeModel;

namespace myTNB.AndroidApp.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailContract
    {
        public interface IView
        {
            void PayNow(AccountData mSelectedAccountData);
            void ContactUs(WeblinkEntity entity);
            void ViewUsage(AccountData mSelectedAccountData);
            void ViewDetails(AccountData mSelectedAccountData);
            void SubmitMeterReading(AccountData mSelectedAccountData, SMRActivityInfoResponse SMRAccountActivityInfoResponse);
            void EnableSelfMeterReading(AccountData mSelectedAccountData);
            void ViewBillHistory(AccountData mSelectedAccountData);
            void ViewManageAccess(AccountData mSelectedAccountData);
            void ShowPaymentReceipt(GetPaymentReceiptResponse response);
            void ShowSelectBill(AccountData mSelectedAccountData);
            void ShowPaymentReceiptError();
            void ShowLoadingScreen();
            void HideLoadingScreen();
            void ViewTips();
            void ViewAccountStatement(AccountData mSelectedAccountData, string statementPeriod);
            void ViewManageBillDelivery();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="operationCanceledException"></param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="apiException"></param>
            void ShowRetryOptionsApiException(ApiException apiException);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="exception"></param>
            void ShowRetryOptionsUnknownException(Exception exception);

            /// <summary>
            /// Show notification list as deleted
            /// </summary>
            void ShowNotificationListAsDeleted();

            void RenderUI();

            void ReturnToDashboard();

            void ShowFeedBackSetupPageRating();

            void GetFeedbackTwoQuestionsNo(GetRateUsQuestionResponse questionRespone);

            void GetFeedbackTwoQuestionsYes(GetRateUsQuestionResponse questionRespone);

            void ShowUpateApp();
            
            void ShareFeedback();

            void ShowStopNotiUpdate();

            void ShowResumeNotiUpdate();

            void UpateCheckBox(bool ishaveValue);
            
            void showFeedbackSDStatus(bool showWLTYPage);
            
            void FeedbackQuestionCall();

            void ShowProgressDialog();

            void HideProgressDialog();

            void ShowNoInternetSnackbar();

            void ShowErrorPopUp();

            void ShowApplicationPopupMessage(StatusDetail statusDetail);

            void NavigateToMyHomeMicrosite(MyHomeModel model, string accessToken);

            void NavigateToApplicationDetails(GetApplicationStatusDisplay application);
        }
    }
}
