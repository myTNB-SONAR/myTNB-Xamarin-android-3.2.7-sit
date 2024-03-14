using System;
namespace myTNB.Android.Src.SSMR.SMRApplication.Api
{
    public enum SUBMIT_MODE
    {
        REGISTER,
        TERMINATE
    }
    public class SMRregistrationSubmitRequest : BaseRequest
    {
        public string contractAccount, oldPhone, newPhone, oldEmail, newEmail, SMRMode, reason;
        public SMRregistrationSubmitRequest(string contractAccountValue,
            string oldPhoneValue, string newPhoneValue, string oldEmailValue, string newEmailValue,
            SUBMIT_MODE mode, string reasonValue)
        {
            contractAccount = contractAccountValue;
            oldPhone = oldPhoneValue;
            newPhone = newPhoneValue;
            oldEmail = oldEmailValue;
            newEmail = newEmailValue;
            SMRMode = mode == SUBMIT_MODE.REGISTER ? "R" : "T";
            reason = reasonValue;
        }
    }
}
