using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB.Android.Src.UpdatePersonalDetailStepOne.Model;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.UpdatePersonalDetailStepOne.MVP
{


    public class UpdatePersonalDetailStepOnePresenter : UpdatePersonalDetailStepOneContract.IUserActionsListener
    {
        UpdatePersonalDetailStepOneContract.IView mView;
        public void Start()
        {
            try
            {

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public UpdatePersonalDetailStepOnePresenter(UpdatePersonalDetailStepOneContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnShowUpdatePersonalDetailStepTwoActivity()
        {
            this.mView.ShowUpdatePersonalDetailStepTwoActivity();
        }

        public void OnDisableSubmitButton()
        {
            this.mView.DisableSubmitButton();
        }

        public void OnHideOnCreate()
        {
            this.mView.HideOnCreate();
        }

        public void OninfoLabelWhoIsRegistered()
        {
            this.mView.ShowinfoLabelWhoIsRegistered();
        }

        public void OnShowbtnYes()
        {
            this.mView.ShowbtnYes();
        }



        public void OnShowICLay()
        {

            this.mView.BoolICLayout(true);

        }

        public void OnShowOwnerName()
        {
            this.mView.BoolLayoutCurrentOwnerName(true);
        }

        public void OnShowEmail()
        {
            this.mView.BoolInputLayoutCurrentEmailAddress(true);

        }

        public void OnShowMailing()
        {
            this.mView.BoolInputLayoutCurrentMailingAddress(true);
        }

        public void OnShowPremise()
        {
            this.mView.BoolInputLayoutCurrentPremiseAddress(true);
        }

        public void OnHideICLay()
        {

            this.mView.BoolICLayout(false);

        }

        public void OnHideOwnerName()
        {
            this.mView.BoolLayoutCurrentOwnerName(false);
        }

        public void OnHideEmail()
        {
            this.mView.BoolInputLayoutCurrentEmailAddress(false);

        }

        public void OnHideMailing()
        {
            this.mView.BoolInputLayoutCurrentMailingAddress(false);
        }

        public void OnHidePremise()
        {
            this.mView.BoolInputLayoutCurrentPremiseAddress(false);
        }

        public void OnShowbtnNo()
        {
            this.mView.ShowbtnNo();
        }

        public void OnRelationshipWithOwner()
        {
            this.mView.showRelationshipWithOwner();
        }

        public void OntoggleIc()
        {
            this.mView.toggleIc();
        }

        public void OntoggleAccountOwnerName()
        {
            this.mView.toggleAccountOwnerName();
        }

        public void OntoggleMobileNumber()
        {
            this.mView.toggleMobileNumber();
        }

        public void OntoggleEmailAddress()
        {
            this.mView.toggleEmailAddress();
        }

        public void OntoggleMailingAddress()
        {
            this.mView.toggleMailingAddress();
        }

        public void OntogglePremiseAddress()
        {
            this.mView.togglePremiseAddress();
        }

        public void OninfoLabelDoIneedOwnerConsent()
        {
            this.mView.ShowinfoLabelDoIneedOwnerConsent();
        }

        public void CheckRequiredFields(string iC, bool toggleChkBoxIC, string ownerName, bool toggleChkOwnerName, bool mobileNumber, bool toggleChkMobileNumber, string emailAddress, bool toggleChkEmailAddress, string mailingAddress, bool toggleChkMailingAddress, string premiseAddress, bool toggleChkPremiseAddress, string otherRelationstip, bool isOtherChoosed)
        {
            try
            {
                bool shoudButtonEnable = true;
                // this.mView.ClearErrors();
                if (toggleChkBoxIC)
                {
                    if (!TextUtils.IsEmpty(iC.Trim()))
                    {
                        this.mView.ClearInvalidError(typeOfLayout.ic);
                        this.mView.EnableSubmitButton();
                    }
                    else
                    {
                        this.mView.ShowEmptyError(typeOfLayout.ic);
                        shoudButtonEnable = false;

                    }
                }

                if (toggleChkOwnerName)
                {
                    if (!TextUtils.IsEmpty(ownerName.Trim()))
                    {
                        if (!Utility.IsNotASCII(ownerName.Trim()))
                        {
                            this.mView.ClearInvalidError(typeOfLayout.ownerName);
                            this.mView.EnableSubmitButton();
                        }
                        else
                        {
                            this.mView.ShowInvalidError(typeOfLayout.ownerName);
                            shoudButtonEnable = false;
                        }
                    }
                    else
                    {
                        this.mView.ShowEmptyError(typeOfLayout.ownerName);
                        shoudButtonEnable = false;
                    }
                }

                if (toggleChkMobileNumber)
                { //mobileNumber
                    if (mobileNumber == false)
                    {
                        this.mView.ShowEmptyError(typeOfLayout.mobileNumber);
                        shoudButtonEnable = false;
                    }
                    else
                    {
                        this.mView.EnableSubmitButton();
                        this.mView.ClearInvalidError(typeOfLayout.mobileNumber);
                    }

                }

                if (toggleChkEmailAddress)
                {
                    if (!TextUtils.IsEmpty(emailAddress.Trim()))
                    {
                        if (!Patterns.EmailAddress.Matcher(emailAddress).Matches())
                        {
                            this.mView.ShowInvalidError(typeOfLayout.emailAddress);
                            shoudButtonEnable = false;
                        }
                        else
                        {
                            this.mView.EnableSubmitButton();
                            this.mView.ClearInvalidError(typeOfLayout.emailAddress);
                        }
                    }
                    else
                    {
                        this.mView.ShowEmptyError(typeOfLayout.emailAddress);
                        shoudButtonEnable = false;
                    }
                }


                if (toggleChkMailingAddress)
                {
                    if (!TextUtils.IsEmpty(mailingAddress.Trim()))
                    {
                        this.mView.EnableSubmitButton();
                        this.mView.ClearInvalidError(typeOfLayout.mailingAddress);

                    }
                    else
                    {

                        this.mView.ShowEmptyError(typeOfLayout.mailingAddress);
                        shoudButtonEnable = false;
                    }
                }


                if (toggleChkPremiseAddress)
                {
                    if (!TextUtils.IsEmpty(premiseAddress.Trim()))
                    {
                        this.mView.EnableSubmitButton();
                        this.mView.ClearInvalidError(typeOfLayout.premiseAddress);
                    }
                    else
                    {

                        this.mView.ShowEmptyError(typeOfLayout.premiseAddress);
                        shoudButtonEnable = false;
                    }
                }

                if (isOtherChoosed)
                {
                    if (!TextUtils.IsEmpty(otherRelationstip.Trim()))
                    {
                        // do nothing , enable only if other input is tick
                    }
                    else
                    {
                        shoudButtonEnable = false;
                    }
                }


                if (shoudButtonEnable)
                {
                    if (toggleChkBoxIC || toggleChkOwnerName || toggleChkMobileNumber || toggleChkEmailAddress || toggleChkMailingAddress || toggleChkPremiseAddress)
                    {
                        this.mView.EnableSubmitButton();
                    }

                }
                else
                {
                    this.mView.DisableSubmitButton();
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



    }
}