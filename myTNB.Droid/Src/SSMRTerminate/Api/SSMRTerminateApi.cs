using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using Refit;

namespace myTNB_Android.Src.SSMRTerminate.Api
{
    public interface SSMRTerminateApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetCAContactDetails")]
        Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] GetRegisteredContactInfoRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSMRTerminationReasons")]
        Task<SMRTerminationReasonsResponse> GetSMRTerminationReasons([Body] GetSMRTerminationReasonsRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitSMRApplication")]
        Task<SMRregistrationSubmitResponse> SubmitSMRApplication([Body] SubmitSMRApplicationRequest request, CancellationToken token);
    }
}
