using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using Refit;

namespace myTNB_Android.Src.SSMRTerminate.Api
{
    public interface SSMRTerminateApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetCAContactDetails")]
        Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] EncryptedRequest encryptedRequest, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetSMRTerminationReasons")]
        Task<SMRTerminationReasonsResponse> GetSMRTerminationReasons([Body] EncryptedRequest encryptedRequest, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitSMRApplication")]
        Task<SMRregistrationSubmitResponse> SubmitSMRApplication([Body] EncryptedRequest encryptedRequest, CancellationToken token);
    }
}
