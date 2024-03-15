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
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.UpdatePersonalDetailStepOne.Model;

namespace myTNB.AndroidApp.Src.UpdatePersonalDetailStepOne.MVP
{
    public class UpdatePersonalDetailStepOneContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {



            void DisableSubmitButton();

            void HideOnCreate();

            void ShowinfoLabelWhoIsRegistered();

            void ShowbtnYes();

            void BoolICLayout(Boolean isShown);
            void BoolLayoutCurrentOwnerName(Boolean isShown);
            void BoolInputLayoutCurrentEmailAddress(Boolean isShown);
            void BoolInputLayoutCurrentMailingAddress(Boolean isShown);
            void BoolInputLayoutCurrentPremiseAddress(Boolean isShown);

            void ShowbtnNo();

            void showRelationshipWithOwner();

            void toggleIc();

            void toggleAccountOwnerName();

            void toggleMobileNumber();

            void toggleEmailAddress();

            void toggleMailingAddress();

            void togglePremiseAddress();

            void ShowinfoLabelDoIneedOwnerConsent();

            void EnableSubmitButton();

            void ShowEmptyError(typeOfLayout lay);

            void ClearInvalidError(typeOfLayout lay);

            void ShowInvalidError(typeOfLayout lay);

            void ClearErrors(typeOfLayout lay);

            void ShowUpdatePersonalDetailStepTwoActivity();

            void ShowInvalidMobileNoError();




        }

        public interface IUserActionsListener : IBasePresenter
        {

            void OnDisableSubmitButton();

            void OnHideOnCreate();

            void OninfoLabelWhoIsRegistered();

            void OnShowbtnYes();

            void OnShowICLay();

            void OnShowOwnerName();

            void OnShowEmail();

            void OnShowMailing();

            void OnShowPremise();

            void OnHideICLay();

            void OnHideOwnerName();

            void OnHideEmail();

            void OnHideMailing();

            void OnHidePremise();

            void OnShowbtnNo();

            void OnRelationshipWithOwner();


            void OntoggleIc();

            void OntoggleAccountOwnerName();

            void OntoggleMobileNumber();

            void OntoggleEmailAddress();

            void OntoggleMailingAddress();

            void OntogglePremiseAddress();

            void OninfoLabelDoIneedOwnerConsent();

            void CheckRequiredFields(string iC, bool toggleChkBoxIC, string ownerName, bool toggleChkOwnerName, bool mobileNumber, bool toggleChkMobileNumber, string emailAddress, bool toggleChkEmailAddress, string mailingAddress, bool toggleChkMailingAddress, string premiseAddress, bool toggleChkPremiseAddress, string otherRelationstip , bool isOtherChoosed);

            void OnShowUpdatePersonalDetailStepTwoActivity();

        }
    }
}