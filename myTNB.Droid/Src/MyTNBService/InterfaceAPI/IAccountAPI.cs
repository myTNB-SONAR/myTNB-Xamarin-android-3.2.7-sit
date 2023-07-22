using System;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB_Android.Src.MyTNBService.InterfaceAPI
{
    public interface IAccountAPI
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLanguagePreference")]
        Task<T> GetLanguagePreference<T>([Body] EncryptedRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveLanguagePreference")]
        Task<T> SaveLanguagePreference<T>([Body] EncryptedRequest request);
    }
}
