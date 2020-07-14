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
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.UpdatePersonalDetailStepOne.MVP
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

        public void CheckRequiredFields(string iC, bool toggleChkBoxIC, string ownerName,bool toggleChkOwnerName,string  mobileNumber, bool toggleChkMobileNumber, string emailAddress, bool toggleChkEmailAddress, string mailingAddress, bool toggleChkMailingAddress, string premiseAddress, bool toggleChkPremiseAddress)
        {
            try
            {
                this.mView.ClearErrors();
                

                // check checkBox 
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
                 
                        this.mView.DisableSubmitButton();
                    }
                }

                // check checkBox 
                if (toggleChkOwnerName)
                {
                    if (!TextUtils.IsEmpty(ownerName.Trim()))
                    {
                        this.mView.ClearInvalidError(typeOfLayout.ownerName);
                
                        this.mView.EnableSubmitButton();
                    }
                    else
                    {
                        // this.mView.ShowEmptyFeedbackError();
                        this.mView.ShowEmptyError(typeOfLayout.ownerName);
                  
                        this.mView.DisableSubmitButton();
                    }
                }

                // check checkBox 
                if (toggleChkMobileNumber)
                {
                    if (!TextUtils.IsEmpty(mobileNumber.Trim()))
                    {
                        this.mView.ClearInvalidError(typeOfLayout.mobileNumber);
                 
                        this.mView.EnableSubmitButton();
                       
                    }
                    else
                    {
                        // this.mView.ShowEmptyFeedbackError();
                        this.mView.ShowEmptyError(typeOfLayout.mobileNumber);
                       
                        this.mView.DisableSubmitButton();
                    }
                }

                // check checkBox 
                if (toggleChkEmailAddress)
                {
                    if (!TextUtils.IsEmpty(emailAddress.Trim()))
                    {
                        if (!Patterns.EmailAddress.Matcher(emailAddress).Matches())
                        {
                            this.mView.ShowInvalidError(typeOfLayout.emailAddress);
                 
                            this.mView.DisableSubmitButton();
                            return;
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
                        
                        this.mView.DisableSubmitButton();
                    }
                }

                // check checkBox 
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
                        this.mView.DisableSubmitButton();
                    }
                }
      
                // check checkBox 
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
                        this.mView.DisableSubmitButton();
                    }
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



    }
}