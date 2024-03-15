using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.Mobile.Business;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Api
{
	public interface IAccountsSMRStatusApi
	{
		[Headers("Content-Type:application/json; charset=utf-8")]
		[Post("/v7/mytnbws.asmx/GetAccountsSMRStatus")]
		Task<AccountSMRStatusResponse> AccountsSMRStatusApi([Body] EncryptedRequest request, CancellationToken token);
	}
}