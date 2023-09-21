using Android.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

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

            Debug.WriteLine(TAG, $"{msg}========Request Start==========");
            Debug.WriteLine(TAG, $"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
            Debug.WriteLine(TAG, $"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

            foreach (var header in req.Headers)
            {
                Debug.WriteLine(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                {
                    Debug.WriteLine(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                Debug.WriteLine(TAG, $"{msg} Content:");

                if (req.Content is StringContent || IsTextBasedContentType(req.Headers) || IsTextBasedContentType(req.Content.Headers))
                {
                    var result = await req.Content.ReadAsStringAsync();

                    Debug.WriteLine(TAG, $"{msg} {string.Join("", result.Cast<char>())}");
                }
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            Debug.WriteLine(TAG, $"{msg}==========Request End==========");

            msg = $"[{req.RequestUri.PathAndQuery} - Response]";

            Debug.WriteLine(TAG, $"{msg}=========Response Start=========");

            var resp = response;

            Debug.WriteLine(TAG, $"{msg} {req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

            foreach (var header in resp.Headers)
            {
                Debug.WriteLine(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                {
                    Debug.WriteLine(TAG, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                Debug.WriteLine(TAG, $"{msg} Content:");

                if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
                {
                    var result = await resp.Content.ReadAsStringAsync();

                    Debug.WriteLine(TAG, $"{msg} {string.Join("", result.Cast<char>())}");
                }
            }

            Debug.WriteLine(TAG, $"{msg} Duration: {DateTime.Now - start}");
            Debug.WriteLine(TAG, $"{msg}==========Response End==========");
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