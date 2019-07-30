using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB_Android.Src.SSMR.SMRApplication.Api
{
    public interface CARegisteredContactInfoApi
{
        [Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetCAContactDetails")]
        [Post("/v6/mytnbappws.asmx/GetCARegisteredContactInfo")]
        Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] object getAccountRequest);
    }
}
