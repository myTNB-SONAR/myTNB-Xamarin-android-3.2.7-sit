using System.Diagnostics;
using DynatraceAndroid;
using myTNB_Android.Src.Utils;

namespace myTNB_Android
{
    internal static class DynatraceHelper
    {
        internal static void OnTrack(string actionName)
        {
            try
            {
                IDTXAction dynaTrace = DynatraceAndroid.Dynatrace.EnterAction(actionName);
                if (dynaTrace != null)
                {
                    dynaTrace.LeaveAction();
                    Debug.WriteLine("[Success] Dynatrace Track: " + actionName);
                    System.Console.WriteLine("[Success] Dynatrace Track: " + actionName);
                }
                else
                {
                    Debug.WriteLine("[Warning] Dynatrace Track: Action is Null");
                    System.Console.WriteLine("[Success] Dynatrace Track: " + actionName);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                Debug.WriteLine("[Error] Dynatrace Track: " + e.Message);
                System.Console.WriteLine("[Success] Dynatrace Track: " + actionName);
            }
        }
    }
}