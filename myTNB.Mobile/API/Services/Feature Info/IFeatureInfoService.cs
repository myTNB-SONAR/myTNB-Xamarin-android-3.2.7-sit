﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB.Mobile.API.Services.FeatureInfo
{
    internal interface IFeatureInfoService
    {
        [Post("/{urlPrefix}/SaveFeatureInfo")]
        Task<HttpResponseMessage> SaveFeatureInfo([Body] EncryptedRequest request
           , [Header(MobileConstants.Header_UserInfo)] string userInfo
           , CancellationToken cancellationToken
           , [Header(MobileConstants.Header_Lang)] string lang
           , string urlPrefix = MobileConstants.ApiUrlPath
           , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId);
    }
}