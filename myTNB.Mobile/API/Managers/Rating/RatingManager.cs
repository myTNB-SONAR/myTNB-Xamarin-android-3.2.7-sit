﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB.Mobile.API.Models.Rating.PostSubmitRating;
using myTNB.Mobile.API.Services.Rating;
using myTNB.Mobile.Business;
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

        #region GetCustomerRatingMaster
        /// <summary>
        /// Get All Rating fields
        /// </summary>
        /// <returns></returns>
        public async Task<GetCustomerRatingMasterResponse> GetCustomerRatingMaster()
        {
            try
            {
                const string categoryID = "8";
                IRatingService service = RestService.For<IRatingService>(MobileConstants.ApiDomain);
                try
                {
                    GetCustomerRatingMasterResponse response = await service.GetCustomerRatingMaster(AppInfoManager.Instance.GetUserInfo()
                        , categoryID
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString()
                        , AppInfoManager.Instance.Language.ToString());
                    if (response != null && response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = MobileConstants.Service_GetCustomerRatingMaster.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetCustomerRatingMasterResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_GetCustomerRatingMaster.GetStatusDetails(MobileConstants.DEFAULT);
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
            res.StatusDetail = MobileConstants.Service_GetCustomerRatingMaster.GetStatusDetails(MobileConstants.DEFAULT);
            return res;
        }
        #endregion

        #region SubmitRating
        /// <summary>
        /// Submits the customer rating
        /// </summary>
        /// <param name="customerName">Display name from Login</param>
        /// <param name="mobileNumber">Mobile Number from Login</param>
        /// <param name="srNumber">Application SR Number</param>
        /// <param name="applicationID">Application ID</param>
        /// <param name="backendAppID">Application backend app ID</param>
        /// <param name="applicationType">Application Type</param>
        /// <param name="questionCategoryValue"></param>
        /// <param name="ratingInput"></param>
        /// <returns></returns>
        public async Task<PostSubmitRatingResponse> SubmitRating(string customerName
            , string mobileNumber
            , string srNumber
            , string applicationID
            , string backendAppID
            , string applicationType
            , string questionCategoryValue
            , List<RatingAnswers> ratingInput)
        {
            try
            {
                IRatingService service = RestService.For<IRatingService>(MobileConstants.ApiDomain);
                try
                {
                    PostSubmitRatingRequest request = new PostSubmitRatingRequest
                    {
                        SubmitRating = new SubmitRating
                        {
                            CustomerName = customerName,
                            PhoneNumber = mobileNumber,
                            SrNo = srNumber,
                            ApplicationId = applicationID,
                            BackendAppId = backendAppID,
                            ModuleName = applicationType,
                            QuestionCategoryValue = questionCategoryValue,
                            RatingResult = ratingInput
                        }
                    };
                    EncryptedRequest encryptedRequest = APISecurityManager.Instance.GetEncryptedRequest(request);
                    HttpResponseMessage rawResponse = await service.SubmitRating(encryptedRequest
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());

                    PostSubmitRatingResponse response = await rawResponse.ParseAsync<PostSubmitRatingResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = MobileConstants.Service_PostSubmitRating.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostSubmitRatingResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_PostSubmitRating.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SubmitRating]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SubmitRating]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            PostSubmitRatingResponse res = new PostSubmitRatingResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = MobileConstants.Service_PostSubmitRating.GetStatusDetails(MobileConstants.DEFAULT);
            return res;
        }
        #endregion
    }
}