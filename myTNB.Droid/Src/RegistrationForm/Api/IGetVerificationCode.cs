using myTNB_Android.Src.RegistrationForm.Activity.Models;
using myTNB_Android.Src.RegistrationForm.Requests;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.RegistrationForm.Api
{
    public interface IGetVerificationCode
    {
        /// <summary>
        /// Depreated in CR - sprint 2
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v5/my_billingssp.asmx/SendRegistrationTokenSMS")]
        //Task<VerificationCodeResponse> GetVerificationCodeThruSMS([Body] VerificationCodeRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SendRegistrationTokenSMS_V2")]
        Task<VerificationCodeResponse> GetVerificationCodeThruSMSV2([Body] VerificationCodeRequest request);
    }
}