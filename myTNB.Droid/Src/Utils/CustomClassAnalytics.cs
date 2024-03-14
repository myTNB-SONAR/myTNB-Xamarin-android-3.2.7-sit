using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Dynatrace.Xamarin;
using Dynatrace.Xamarin.Binding.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace myTNB.Android.Src.Utils
{
    public static class CustomClassAnalytics
    {
        public static void SetScreenNameDynaTrace(string screenName)
        {
            try
            {
                var myAction = Agent.Instance.EnterAction(screenName);
                myAction.LeaveAction();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}