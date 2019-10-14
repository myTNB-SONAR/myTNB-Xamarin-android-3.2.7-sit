using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace myTNB
{
    public sealed class AccountUsageManager
    {
        private static readonly Lazy<AccountUsageManager> lazy = new Lazy<AccountUsageManager>(() => new AccountUsageManager());
        public static AccountUsageManager Instance { get { return lazy.Value; } }

        private const string ACCOUNT_USAGE_PATH = "myTNB.Mobile.Resources.Usage.AccountUsageSmart.json";

        public static string GetData()
        {
            string info = string.Empty;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(ACCOUNT_USAGE_PATH);
                using (StreamReader reader = new StreamReader(stream))
                {
                    info = reader.ReadToEnd();
                    Debug.WriteLine("DEBUG >> info: " + info);
                }
                Debug.WriteLine("DEBUG >> SUCCESS");
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG >> GetInfo: " + e.Message);
                info = string.Empty;
            }
            return info;
        }
    }
}