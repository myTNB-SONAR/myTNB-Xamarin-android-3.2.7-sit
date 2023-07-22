using myTNB_Android.Src.AppLaunch.Models;
using myTNB.Mobile.Business;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Api
{
	public interface IAccountsSMRStatusApi
	{
		[Headers("Content-Type:application/json; charset=utf-8")]
		[Post("/v6/mytnbappws.asmx/GetAccountsSMRStatus")]
		Task<AccountSMRStatusResponse> AccountsSMRStatusApi([Body] EncryptedRequest request, CancellationToken token);
	}
}