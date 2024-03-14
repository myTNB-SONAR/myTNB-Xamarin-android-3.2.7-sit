using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.UpdatePersonalDetailStepOne.Model;

namespace myTNB.Android.Src.AddAcc_UpdateIdentification_StepTwo.MVP
{
    public class AddAccUpdateIdetificationDetailsStepTwoContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {



            void DisableSubmitButton();

            //void HideOnCreate();

            
            void BoolLayoutCurrentOwnerName(Boolean isShown);
            void BoolInputLayoutCurrentEmailAddress(Boolean isShown);
            void BoolInputLayoutCurrentMailingAddress(Boolean isShown);
            void BoolInputLayoutCurrentPremiseAddress(Boolean isShown);

            void toggleSkipStep();

            void toggleAccountOwnerName();

            void toggleMobileNumber();

            void toggleEmailAddress();

            void toggleMailingAddress();

            void togglePremiseAddress();

            void EnableSubmitButton();

            void ShowEmptyError(typeOfLayout lay);

            void ClearInvalidError(typeOfLayout lay);

            void ShowInvalidError(typeOfLayout lay);

            void ClearErrors(typeOfLayout lay);

            void ShowUpdatePersonalDetailStepThreeActivity();

            void ShowInvalidMobileNoError();




        }

        public interface IUserActionsListener : IBasePresenter
        {

            void OnDisableSubmitButton();

            //void OnHideOnCreate();

            void OnShowOwnerName();

            void OnShowEmail();

            void OnShowMailing();

            void OnShowPremise();

            void OnHideOwnerName();

            void OnHideEmail();

            void OnHideMailing();

            void OnHidePremise();

            void OntoggleSelectSkipChkBox();

            void OntoggleAccountOwnerName();

            void OntoggleMobileNumber();

            void OntoggleEmailAddress();

            void OntoggleMailingAddress();

            void OntogglePremiseAddress();

            void CheckRequiredFields(bool toggleChkBoxIC, string ownerName, bool toggleChkOwnerName, bool mobileNumber, bool toggleChkMobileNumber, string emailAddress, bool toggleChkEmailAddress, string mailingAddress, bool toggleChkMailingAddress, string premiseAddress, bool toggleChkPremiseAddress, bool isOtherChoosed);

            void OnShowUpdatePersonalDetailStepThreeActivity();

        }
    }
}