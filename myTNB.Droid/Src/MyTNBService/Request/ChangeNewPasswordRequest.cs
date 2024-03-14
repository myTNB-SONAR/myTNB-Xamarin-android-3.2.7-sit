using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class ChangeNewPasswordRequest : BaseRequest
    {
        public string currentPassword, newPassword, confirmNewPassword;

        public ChangeNewPasswordRequest(string currentPassword, string newPassword, string confirmNewPassword)
        {
            this.currentPassword = currentPassword;
            this.newPassword = newPassword;
            this.confirmNewPassword = confirmNewPassword;
        }
    }
}
