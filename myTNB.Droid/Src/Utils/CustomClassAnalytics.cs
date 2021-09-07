using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DynatraceAndroid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace myTNB_Android.Src.Utils
{
    public static class CustomClassAnalytics
    {
        public static void SetScreenNameDynaTrace(string screenName)
        {
            try
            {
                IDTXAction dynaTrace = DynatraceAndroid.Dynatrace.EnterAction(screenName);
                dynaTrace.LeaveAction();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}