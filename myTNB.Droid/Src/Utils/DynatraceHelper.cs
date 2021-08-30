using Android.Util;
using DynatraceAndroid;
using myTNB_Android.Src.Database.Model;
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
                    Log.Debug("[DEBUG]", "[Success] Dynatrace Track: " + actionName);
                }
                else
                {
                    Log.Debug("[DEBUG]", "[Warning] Dynatrace Track: Action is Null");
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                Log.Debug("[DEBUG]", "[Error] Dynatrace Track: " + e.Message);
            }
        }

        internal static void IdentifyUser()
        {
            try
            {
                UserEntity loggedUser = UserEntity.GetActive();
                string userEmail = loggedUser.Email;
                if (userEmail.IsValid())
                {
                    DynatraceAndroid.Dynatrace.IdentifyUser(userEmail);
                    Log.Debug("[DEBUG]", "[Success] Dynatrace IdentifyUser: " + userEmail);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                Log.Debug("[DEBUG]", "[Error] Dynatrace IdentifyUser: " + e.Message);
            }
        }
    }
}