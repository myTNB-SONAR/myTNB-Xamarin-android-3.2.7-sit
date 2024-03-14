﻿using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Models;
using Org.BouncyCastle.Asn1.BC;
using System.Collections.Generic;
using System.Threading.Tasks;
using static myTNB.Android.Src.Feedback_Prelogin_NewIC.Activity.FeedbackPreloginNewICActivity;

namespace myTNB.Android.Src.Feedback_Prelogin_NewIC.MVP
{
    public class FeedbackPreloginNewICContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowGeneralEnquiry();
            void ShowAboutBillEnquiry();

            void ShowInvalidAccountNumberError();

            void RemoveNumberErrorMessage();

            void showUpdatePersonalDetail();

            void ShowEnterOrSelectAccNumber();

            void toggleEnableClick();

            void toggleDisableClick();

            void ShowWhereIsMyAcc();

            void OnSubmitError(string message = null);

            void ShowProgressDialog();

            void HideProgressDialog();

            void makeSetClick(bool setclick);

            void onScan();

            void ShowSelectAccount();

            void ShowGSLRebate(bool isOwner);

            void ShowOvervoltageClaim();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void OnGeneralEnquiry();

            void OnSelectAccount();

            bool CheckRequiredFields(string accno);

            void onUpdatePersonalDetail();

            void onShowWhereIsMyAcc();

            void ValidateAccountAsync(string contractAccount, EnquiryTypeEnum type);

            void showScan();
        }
    }
}