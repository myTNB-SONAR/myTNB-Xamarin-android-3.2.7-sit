using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class AccountToCustomerRequest : BaseRequest
    {
        public string tnbBillAccountNum, tnbAccountHolderICNum, tnbAccountContractNum, description, isOwner, suppliedMotherName;
        public int type;

        public AccountToCustomerRequest(string tnbBillAccountNum, string tnbAccountHolderICNum, string tnbAccountContractNum, string description, string isOwner, string suppliedMotherName, int type)
        {
            this.tnbBillAccountNum = tnbBillAccountNum;
            this.tnbAccountHolderICNum = tnbAccountHolderICNum;
            this.tnbAccountContractNum = tnbAccountContractNum;
            this.description = description;
            this.isOwner = isOwner;
            this.suppliedMotherName = suppliedMotherName;
            this.type = type;
        }
    }
}
