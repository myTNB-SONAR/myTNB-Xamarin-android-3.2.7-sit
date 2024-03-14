using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class CustomerAccountsForICNumRequest : BaseRequest
    {
        public string currentLinkedAccounts, identificationNo;

        public CustomerAccountsForICNumRequest(string currentLinkedAccounts, string identificationNo)
        {
            this.currentLinkedAccounts = currentLinkedAccounts;
            this.identificationNo = identificationNo;
        }
    }
}
