using myTNB.Android.Src.AddAccount.Models;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Base.MVP;
using Refit;
using System;

namespace myTNB.Android.Src.AddAccount.MVP
{
    public class AddAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show error message that account number is empty 
            /// </summary>            
            void ShowEmptyAccountNumberError();

            /// <summary>
            /// Show error message that account nickname is empty 
            /// </summary>
            void ShowEmptyAccountNickNameError();

            /// <summary>
            /// Show error message that owner ic number is empty 
            /// </summary>
            void ShowEmptyOwnerIcNumberError();

            /// <summary>
            /// Show error message that mothers maiden name is empty 
            /// </summary>
            void ShowEmptyMothersMaidenNameError();

            /// <summary>
            /// Show error message that account name is empty 
            /// </summary>
            void ShowEmptyAccountNameError();

            /// <summary>
            /// Show error message that account name is empty 
            /// </summary>
            void ShowSameAccountNameError();

            /// <summary>
            /// Show error message that account number is empty 
            /// </summary>
            void ShowInvalidAccountNumberError();

            /// <summary>
            /// Show error message that account name is empty 
            /// </summary>
            void ShowInvalidAccountNicknameError();

            /// <summary>
            /// Show add account service response
            /// </summary>
            void ShowAddAccountResponse(ServiceResponse response);

            /// <summary>
            /// Show add account success message
            /// </summary>
            void ShowAddAccountSuccess(string message);

            /// <summary>
            /// Show add account failed message
            /// </summary>
            void ShowAddAccountFail(string errorMessage);

            /// <summary>
            /// Show add account failed message
            /// </summary>
            void ShowAddAccountInvalid(string errorMessage);

            /// <summary>
            /// Show add account number failed message
            /// </summary>
            void ShowAddAccountNumberInvalid(string errorMessage);

            /// <summary>
            /// Show add account failed message
            /// </summary>
            void ShowAddAccountROCFail(string errorMessage);

            /// <summary>
            /// Show gov dialog message
            /// </summary>
            void GovermentDialog();

            /// <summary>
            /// Show progress dialog for adding account
            /// </summary>
            void ShowAddingAccountProgressDialog();

            /// <summary>
            /// Hide progress dialog after adding account
            /// </summary>
            void HideAddingAccountProgressDialog();

            /// <summary>
            /// Clear form text
            /// </summary>
            void ClearText();

            /// <summary>
            /// Clear hint text
            /// </summary>
            void ClearNameHint();

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
            /// Show validate account service success message with account info
            /// </summary>
            void ShowValidateAccountSucess(NewAccount account);

            /// <summary>
            /// Enable add account button on form
            /// </summary>
            void EnableAddAccountButton();

            /// <summary>
            /// Disable add account button on form
            /// </summary>
            void DisableAddAccountButton();


            void ShowEnterValidAccountName();

            void RemoveNameErrorMessage();
            void RemoveNumberErrorMessage();
            void ClearAllErrorFields();
            void ClearErrorFields();
            void ClearROCError();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Add account service call Api : AddTNBAccountToUserReg
            /// </summary>
            void AddAccount(string apiKeyId, string userID, string email, string tnbBillAcctNo, string tnbAcctHolderIC, string tnbAcctContractNo, string type, string des, bool isOwner, string suppliedMotherName);

            /// <summary>
            /// Validate account service call Api : ValidateManualAccountLinking
            /// </summary>
            void ValidateAccount(string apiKeyId, string accountNum, string accountType, string userIdentificationNum, string suppliedMotherName, bool isOwner, string accountLabel);

            /// <summary>
            /// Check add account error for all fileds
            /// </summary>
            bool ValidateEditText(string accountno, string accountNickName);

            /// <summary>
            /// Check add account form all fileds are enterd properly
            /// </summary>
            void CheckRequiredFields(string accountno, string accountNickName, bool isOwner, string ownerIC);
        }
    }
}