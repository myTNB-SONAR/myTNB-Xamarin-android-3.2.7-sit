using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace myTNB
{
    public sealed class FAQManager
    {
        private static readonly Lazy<FAQManager> lazy = new Lazy<FAQManager>(() => new FAQManager());
        public static FAQManager Instance { get { return lazy.Value; } }
        private FAQManager() { }

        public enum Language
        {
            EN,
            MS
        }

        private string FAQString = string.Empty;
        private const string FAQ_RESOURCE_PATH = "myTNB.Mobile.Resources.FAQ.FAQ_{0}.json";

        public string GetFAQ(Language lang = Language.EN)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(string.Format(FAQ_RESOURCE_PATH, lang));
                using (StreamReader reader = new StreamReader(stream))
                {
                    FAQString = reader.ReadToEnd();
                    Debug.WriteLine("DEBUG >> FAQString: " + FAQString);
                }
                Debug.WriteLine("DEBUG >> SUCCESS");
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG >> GetFAQ: " + e.Message);
            }
            return FAQString;
        }
    }
}