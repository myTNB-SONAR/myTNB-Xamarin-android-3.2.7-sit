using System;
using System.Collections.Generic;
using Foundation;

namespace myTNB
{
    public sealed class WhatsNewCache
    {
        private static readonly Lazy<WhatsNewCache> lazy = new Lazy<WhatsNewCache>(() => new WhatsNewCache());
        public static WhatsNewCache Instance { get { return lazy.Value; } }
        private static Dictionary<string, NSData> ImageDictionary = new Dictionary<string, NSData>();

        public static bool WhatsNewIsAvailable { set; get; }
        public static bool RefreshWhatsNew { set; get; } = false;
        public static string DeeplinkWhatsNewId { set; get; }
        public static bool IsSitecoreRefresh { set; get; }

        public static void SaveImage(string key, NSData data)
        {
            if (ImageDictionary == null)
            {
                ImageDictionary = new Dictionary<string, NSData>();
            }
            if (ImageDictionary.ContainsKey(key))
            {
                ImageDictionary[key] = data;
            }
            else
            {
                ImageDictionary.Add(key, data);
            }
        }

        public static NSData GetImage(string key)
        {
            if (ImageDictionary.ContainsKey(key))
            {
                return ImageDictionary[key];
            }
            return null;
        }

        public static void Clear()
        {
            if (ImageDictionary != null)
            {
                ImageDictionary.Clear();
            }
            WhatsNewIsAvailable = false;
            DeeplinkWhatsNewId = string.Empty;
        }

        public static void ClearImages()
        {
            if (ImageDictionary != null)
            {
                ImageDictionary.Clear();
            }
        }
    }
}
