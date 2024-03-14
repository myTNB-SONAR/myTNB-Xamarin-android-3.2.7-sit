using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class RegisteredCardsRequest : BaseRequest
    {
        public bool isOwnedAccount;

        public RegisteredCardsRequest(bool isOwnedAccount)
        {
            this.isOwnedAccount = isOwnedAccount;
        }
    }
}
