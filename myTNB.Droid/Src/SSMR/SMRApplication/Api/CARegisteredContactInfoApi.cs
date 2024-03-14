using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Android.Src.SSMR.SMRApplication.Api
{
    public interface CARegisteredContactInfoApi
{
        [Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/GetCAContactDetails")]
        [Post("/v7/mytnbws.asmx/GetCARegisteredContactInfo")]
        Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] object getAccountRequest);
    }
}
