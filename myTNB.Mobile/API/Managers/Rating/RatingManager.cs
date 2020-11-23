using System;
using System.Diagnostics;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB.Mobile.API.Services.Rating;
using myTNB.Mobile.Extensions;
using Refit;

namespace myTNB.Mobile.API.Managers.Rating
{
    public sealed class RatingManager
    {
        private static readonly Lazy<RatingManager> lazy =
             new Lazy<RatingManager>(() => new RatingManager());

        public static RatingManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public RatingManager() { }

        #region SearchApplicationType
        public async Task<GetCustomerRatingMasterResponse> GetCustomerRatingMaster(string categoryID)
        {
            try
            {
                IRatingService service = RestService.For<IRatingService>(Constants.ApiDomain);
                try
                {
                    GetCustomerRatingMasterResponse response = await service.GetCustomerRatingMaster(AppInfoManager.Instance.GetUserInfo()
                        , categoryID
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    if (response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_GetCustomerRatingMaster.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetCustomerRatingMaster.GetStatusDetails(Constants.DEFAULT);
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetCustomerRatingMaster]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetCustomerRatingMaster]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            GetCustomerRatingMasterResponse res = new GetCustomerRatingMasterResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = Constants.Service_GetCustomerRatingMaster.GetStatusDetails(Constants.DEFAULT);
            return res;
        }
        #endregion
    }
}