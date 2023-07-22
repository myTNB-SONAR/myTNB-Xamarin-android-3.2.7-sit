using myTNB.Mobile.Business;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface ISMRAccountActivityInfoApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSMRAccountActivityInfo")]
        Task<SMRActivityInfoResponse> GetSMRAccountActivityInfo([Body] EncryptedRequest encryptedRequest, CancellationToken cancellationToken);
    }
}