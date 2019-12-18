using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace myTNB
{
    public sealed class TnCManager
    {
        private static readonly Lazy<TnCManager> lazy = new Lazy<TnCManager>(() => new TnCManager());
        public static TnCManager Instance { get { return lazy.Value; } }
        private TnCManager() { }

        public enum Language
        {
            EN,
            MS
        }

        private string TNCString = string.Empty;
        private const string TNC_RESOURCE_PATH = "myTNB.Mobile.Resources.TnC.TermsAndConditions_{0}.txt";

        public string GetTnC(Language lang = Language.EN)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(string.Format(TNC_RESOURCE_PATH, lang));
                using (StreamReader reader = new StreamReader(stream))
                {
                    TNCString = reader.ReadToEnd();
                    Debug.WriteLine("DEBUG >> TNCString: " + TNCString);
                }
                Debug.WriteLine("DEBUG >> SUCCESS");
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG >> GetTnC: " + e.Message);
            }

            return TNCString;
        }
    }
}