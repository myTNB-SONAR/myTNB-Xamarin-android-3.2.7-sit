using System;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB.AndroidApp.Src.MyTNBService.InterfaceAPI
{
    public interface IAccountAPI
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetLanguagePreference")]
        Task<T> GetLanguagePreference<T>([Body] EncryptedRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SaveLanguagePreference")]
        Task<T> SaveLanguagePreference<T>([Body] EncryptedRequest request);
    }
}
