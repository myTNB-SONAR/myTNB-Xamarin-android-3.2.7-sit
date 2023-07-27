using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB_Android.Src.SSMR.SMRApplication.Api
{
    public interface SMRregistrationApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetCAContactDetails")]
        //[Post("/v7/mytnbws.asmx/GetCARegisteredContactInfo")]
        Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] EncryptedRequest encryptedRequest);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitSMRApplication")]
        Task<SMRregistrationSubmitResponse> SubmitSMRApplication([Body] EncryptedRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountsSMREligibility")]
        Task<GetAccountsSMREligibilityResponse> GetAccountsSMREligibility([Body] EncryptedRequest encryptedRequest);
    }
}
