﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Android.Util;
using System.Net.Http.Headers;

namespace myTNB_Android.Src.Utils
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        static readonly string TAG = "HttpLoggingHandler";
        public HttpLoggingHandler(HttpMessageHandler innerHandler = null) : base(
            innerHandler ?? new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await Task.Delay(1, cancellationToken).ConfigureAwait(false);
            var start = DateTime.Now;
            var req = request;
            var msg = $"[{req.RequestUri.PathAndQuery} -  Request]";

            Log.Debug(TAG , $"{msg}========Request Start==========");
            Log.Debug(TAG, $"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
            Log.Debug(TAG, $"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

            foreach (var header in req.Headers)
            {
                Log.Debug(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                {
                    Log.Debug(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                Log.Debug(TAG, $"{msg} Content:");

                if (req.Content is StringContent || IsTextBasedContentType(req.Headers) || IsTextBasedContentType(req.Content.Headers))
                {
                    var result = await req.Content.ReadAsStringAsync();

                    Log.Debug(TAG, $"{msg} {string.Join("", result.Cast<char>().Take(256))}...");
                }
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            Log.Debug(TAG, $"{msg}==========Request End==========");

            msg = $"[{req.RequestUri.PathAndQuery} - Response]";

            Log.Debug(TAG, $"{msg}=========Response Start=========");

            var resp = response;

            Log.Debug(TAG, $"{msg} {req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

            foreach (var header in resp.Headers)
            {
                Log.Debug(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                {
                    Log.Debug(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                Log.Debug(TAG, $"{msg} Content:");

                if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
                {
                    var result = await resp.Content.ReadAsStringAsync();

                    Log.Debug(TAG, $"{msg} {string.Join("", result.Cast<char>().Take(256))}...");
                }
            }

            Log.Debug(TAG, $"{msg} Duration: {DateTime.Now - start}");
            Log.Debug(TAG, $"{msg}==========Response End==========");
            return response;
        }

        readonly string[] types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            IEnumerable<string> values;
            if (!headers.TryGetValues("Content-Type", out values))
                return false;
            var header = string.Join(" ", values).ToLowerInvariant();

            return types.Any(t => header.Contains(t));
        }
    }
}