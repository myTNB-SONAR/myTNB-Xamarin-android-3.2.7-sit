using myTNB.Android.Src.AddAccount.Models;

namespace myTNB.Android.Src.AddAccount.Requests
{
    public class AddAccountToCustomerRequest : AddAccountResquest
    {
        public AddAccountToCustomerRequest(string apiKeyID, string userID, string email, string tnbBillAccountNum, string tnbAcctHolderIC, string tnbAcctContractNo, string type, string des, bool isOwner, string suppliedMotherName) : base(apiKeyID, userID, email, tnbBillAccountNum, tnbAcctHolderIC, tnbAcctContractNo, type, des, isOwner, suppliedMotherName)
        {
            base.apiKeyID = apiKeyID;
            base.userID = userID;
            base.email = email;
            base.tnbBillAccountNum = tnbBillAccountNum;
            base.tnbAccountHolderICNum = tnbAccountHolderICNum;
            base.tnbAccountContractNum = tnbAccountContractNum;
            base.type = type;
            base.description = des;
            base.isOwner = isOwner;
            base.suppliedMotherName = suppliedMotherName;
        }
    }
}