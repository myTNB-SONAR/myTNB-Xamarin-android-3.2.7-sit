using System;
using System.Collections.Generic;
using Foundation;

namespace myTNB
{
    public sealed class WhatsNewDetailCache
    {
        private static readonly Lazy<WhatsNewDetailCache> lazy = new Lazy<WhatsNewDetailCache>(() => new WhatsNewDetailCache());
        public static WhatsNewDetailCache Instance { get { return lazy.Value; } }
        private static Dictionary<string, NSData> ImageDictionary = new Dictionary<string, NSData>();

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
