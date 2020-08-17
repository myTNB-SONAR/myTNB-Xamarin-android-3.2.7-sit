using System;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API;
using Sitecore.MobileSDK.API.Session;
using System.Threading.Tasks;
using Sitecore.MobileSDK.API.Request;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sitecore.MobileSDK.PasswordProvider;
using System.Diagnostics;
using System.Threading;

namespace myTNB.SitecoreCMS.Services
{
    public class SitecoreService
    {
        private TimeSpan timeSpan = TimeSpan.FromMilliseconds(5000);

        public SitecoreService(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public async Task<ScItemsResponse> GetItemByPath(string itemPath
            , PayloadType itemLoadType
            , List<ScopeType> itemScopeTypes
            , string websiteUrl = null
            , string itemLanguage = "en")
        {
            try
            {
                using (var session = await GetSession(websiteUrl))
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(timeSpan);
                    IReadItemsByPathRequest request = ItemWebApiRequestBuilder.ReadItemsRequestWithPath(itemPath)
                        .Payload(itemLoadType)
                        .AddScope(itemScopeTypes)
                        .Language(itemLanguage)
                        .Build();
                    return await session.ReadItemAsync(request, source.Token);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: SitecoreService/GetItemByPath " + e.Message);
            }
            return new ScItemsResponse(0, 0, new List<ISitecoreItem>());
        }

        public async Task<ScItemsResponse> GetItemById(string itemId
            , PayloadType itemLoadType
            , List<ScopeType> itemScopeTypes
            , string websiteUrl = null
            , string itemLanguage = "en")
        {
            try
            {
                using (var session = await GetSession(websiteUrl))
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(timeSpan);
                    IReadItemsByIdRequest request = ItemWebApiRequestBuilder.ReadItemsRequestWithId(itemId)
                        .Payload(itemLoadType)
                        .AddScope(itemScopeTypes)
                        .Language(itemLanguage)
                        .Build();
                    return await session.ReadItemAsync(request, source.Token);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: SitecoreService/GetItemById " + e.Message);
            }
            return new ScItemsResponse(0, 0, new List<ISitecoreItem>());
        }

        public async Task<Byte[]> GetMediaByUrl(string mediaUrl)
        {
            try
            {
                mediaUrl = CleanUpMediaUrlByReplacingWeirdTildeSignWithCorrect(mediaUrl);
                using (var session = await SitecoreSession)
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(timeSpan);
                    IMediaResourceDownloadRequest request = ItemWebApiRequestBuilder.DownloadResourceRequestWithMediaPath(mediaUrl)
                        .Language("en")
                        .Build();

                    byte[] data = null;
                    using (Stream response = await session.DownloadMediaResourceAsync(request, source.Token))
                    using (MemoryStream responseInMemory = new MemoryStream())
                    {
                        await response.CopyToAsync(responseInMemory);
                        data = responseInMemory.ToArray();
                        return data;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: SitecoreService/GetMediaByUrl " + e.Message);
            }
            return null;
        }

        async Task<IList<ScItemsResponse>> GetDataSourceFromFieldName(ISitecoreItem sitecoreItem
            , string fieldName)
        {
            if (sitecoreItem.Fields.All(f => f.Name != fieldName))
            {
                return null;
            }

            string value = sitecoreItem[fieldName].RawValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string[] itemIds = value.Split('|');

            if (!itemIds.Any())
            {
                return null;
            }

            IList<ScItemsResponse> sitecoreItems = new List<ScItemsResponse>();

            foreach (string itemId in itemIds)
            {
                ScItemsResponse response = await GetItemById(itemId
                    , PayloadType.Content
                    , new List<ScopeType> { ScopeType.Self }
                    , sitecoreItem.Source.Language);
                if (response == null)
                {
                    continue;
                }
                sitecoreItems.Add(response);
            }
            return sitecoreItems;
        }

        static string CleanUpMediaUrlByReplacingWeirdTildeSignWithCorrect(string mediaUrl)
        {
            if (mediaUrl.IndexOf('~') > 0)
            {
                return mediaUrl;
            }
            mediaUrl = mediaUrl.Substring(1);
            mediaUrl = "~" + mediaUrl;
            return mediaUrl;
        }

        async Task<ScItemsResponse> GetDatasourceFromChildren(ISitecoreItem sitecoreItem)
        {
            return await GetItemById(sitecoreItem.Id
                , PayloadType.Content
                , new List<ScopeType> { ScopeType.Children }
                , sitecoreItem.Source.Language);
        }

        Task<ISitecoreWebApiReadonlySession> SitecoreSession
        {
            get
            {
                return GetSession();
            }
        }

        async Task<ISitecoreWebApiReadonlySession> GetSession(string websiteUrl = null)
        {
            if (string.IsNullOrEmpty(websiteUrl)
                || (!string.IsNullOrEmpty(websiteUrl)
                && !(Uri.IsWellFormedUriString(websiteUrl, UriKind.Absolute))))
            {
                websiteUrl = TNBGlobal.SITECORE_URL;
            }
            try
            {
                using (var credentials = new SecureStringPasswordProvider(TNBGlobal.SITECORE_USERNAME, TNBGlobal.SITECORE_PASSWORD))
                {
                    {
                        var session = SitecoreWebApiSessionBuilder.AuthenticatedSessionWithHost(websiteUrl)
                            .Credentials(credentials)
                            .Site("/sitecore/shell")
                            .DefaultDatabase("web")
                            .DefaultLanguage("en")
                            .MediaLibraryRoot("/sitecore/media library")
                            .MediaPrefix("~/media/")
                            .DefaultMediaResourceExtension("ashx")
                            .BuildReadonlySession();

                        return session;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: SitecoreService/GetSession " + e.Message);
            }
            return null;
        }
    }
}