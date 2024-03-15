using myTNB.Mobile.Business;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.myTNBMenu.Api
{
	public interface IPaymentStatusApi
	{
		[Headers("Content-Type:application/json; charset=utf-8")]
		[Post("/v7/mytnbws.asmx/CheckPendingPayments")]
		Task<CheckPendingPaymentsResponse> GetCheckPendingPayments([Body] EncryptedRequest request, CancellationToken cancellationToken);
	}
}