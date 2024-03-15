using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class UpdateLinkedAccountNameRequest : BaseRequest
    {
        public string accountNo, oldAccountNickName, newAccountNickName;

        public UpdateLinkedAccountNameRequest(string accountNo, string oldAccountNickName, string newAccountNickName)
        {
            this.accountNo = accountNo;
            this.oldAccountNickName = oldAccountNickName;
            this.newAccountNickName = newAccountNickName;
        }
    }
}
