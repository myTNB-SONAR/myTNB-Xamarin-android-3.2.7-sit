using System;
using System.Collections.Generic;
using Foundation;

namespace myTNB
{
    public sealed class WhatsNewDetailDescriptionCache
    {
        private static readonly Lazy<WhatsNewDetailDescriptionCache> lazy = new Lazy<WhatsNewDetailDescriptionCache>(() => new WhatsNewDetailDescriptionCache());
        public static WhatsNewDetailDescriptionCache Instance { get { return lazy.Value; } }
        private static Dictionary<string, Dictionary<string, string>> ImageDictionary = new Dictionary<string, Dictionary<string, string>>();

        public static void SaveImages(string key, Dictionary<string, string> data)
        {
            if (ImageDictionary == null)
            {
                ImageDictionary = new Dictionary<string, Dictionary<string, string>>();
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

        public static Dictionary<string, string> GetImages(string key)
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
