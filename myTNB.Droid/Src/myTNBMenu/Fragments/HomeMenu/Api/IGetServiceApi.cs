using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.Api
{
	public interface IGetServiceApi
    {
		[Headers("Content-Type:application/json; charset=utf-8")]
		[Post("/v7/mytnbws.asmx/GetServicesV3")]
		Task<GetServicesResponse> GetService([Body] GetServiceRequests request, CancellationToken cancellationToken);
	}
}