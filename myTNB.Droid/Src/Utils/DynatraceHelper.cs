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
                dynaTrace.LeaveAction();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}