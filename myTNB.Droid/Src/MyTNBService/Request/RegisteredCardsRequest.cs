using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
