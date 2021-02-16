using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace myTNB.Mobile.TestData.ApplicationSearch
{
    internal sealed class SearchTestManager
    {
        private static readonly Lazy<SearchTestManager> lazy = new Lazy<SearchTestManager>(() => new SearchTestManager());

        public static SearchTestManager Instance { get { return lazy.Value; } }

        private SearchTestManager()
        {
        }

        public string GetData()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream("myTNB.Mobile.Test_Data.Application_Search.Search01.json");
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG >> GetData: " + e.Message);
            }
            return string.Empty;
        }
    }
}
