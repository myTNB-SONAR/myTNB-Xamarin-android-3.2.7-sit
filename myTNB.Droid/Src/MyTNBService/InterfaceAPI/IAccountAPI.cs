using System;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.Request;
using Refit;

namespace myTNB_Android.Src.MyTNBService.InterfaceAPI
{
    public interface IAccountAPI
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLanguagePreference")]
        Task<T> GetLanguagePreference<T>([Body] APIBaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveLanguagePreference")]
        Task<T> SaveLanguagePreference<T>([Body] APIBaseRequest request);
    }
}
