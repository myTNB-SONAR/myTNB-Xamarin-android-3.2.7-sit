﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using myTNB.AndroidApp.Src.Base.MVP;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Base.Request;
using System.Threading.Tasks;
using myTNB.AndroidApp.Src.FeedbackGeneralEnquiryStepTwo.Model;

namespace myTNB.AndroidApp.Src.AddAcc_UpdateIdentification_StepFour.MVP
{
    public class AddAccUpdateIdentificationStepFourContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowNavigateToTermsAndConditions();

            void ShowFullNameError();

            void DisableRegisterButton();

            void ClearFullNameError();

            void ShowInvalidEmailError();

            void ClearInvalidEmailError();

            void EnableRegisterButton();
            void UpdateMobileNumber(string mobile_no);

            void ClearInvalidMobileError();

            void ShowInvalidMobileNoError();

            string GetImageName(int itemCount);

            Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage);

            public void HideProgressDialog();

            void OnSubmitError(string message = null);

            void ShowSuccess(string date, string feedbackId, int imageCount);

            void ShowProgressDialog();



        }

        public interface IUserActionsListener : IBasePresenter
        {

            void NavigateToTermsAndConditions();

            void CheckRequiredFields(string fullname, bool mobile_no, string email, bool tnc, bool isNeedTNC);

            void OnSubmit(string acc, string feedback, string fullname, string mobile_no, string email, List<AttachedImage> attachedImages, List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsModelList, bool isowner, int ownerRelationship, string relationshipDescription);

        }
    }
}