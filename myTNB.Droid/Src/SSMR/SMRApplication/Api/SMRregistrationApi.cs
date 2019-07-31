using System;
using System.Threading.Tasks;
using Refit;

namespace myTNB_Android.Src.SSMR.SMRApplication.Api
{
    public interface SMRregistrationApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetCAContactDetails")]
        [Post("/v6/mytnbappws.asmx/GetCARegisteredContactInfo")]
        Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] BaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetCAContactDetails")]
        [Post("/v6/mytnbappws.asmx/SubmitSMRApplication")]
        Task<SMRregistrationSubmitResponse> SubmitSMRApplication([Body] BaseRequest request);
    }
}
