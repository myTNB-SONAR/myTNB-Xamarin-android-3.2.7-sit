using System;
using Android.Content;
using Android.OS;
using Android.Preferences;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;

namespace myTNB.AndroidApp.Src.myTNBMenu.Async
{
    public class SyncSRApplicationAPI : AsyncTask
    {
        private DashboardHomeActivity _activity;
        private const string Key = "SyncSRAPIKey";
        public SyncSRApplicationAPI(DashboardHomeActivity activity)
        {
            _activity = activity;
        }

        protected override void OnPreExecute() { }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            try
            {
                if (MyTNBAccountManagement.GetInstance().IsApplicationSyncAPIEnable
                    && ShouldCallSRSyncAPI(MyTNBAccountManagement.GetInstance().ApplicationSyncAPIInterval))
                {
                    _ = ApplicationStatusManager.Instance.SyncSRApplication();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Sync SR API Error: " + e.Message);
            }
            return null;
        }

        private bool ShouldCallSRSyncAPI(double interval)
        {
            try
            {
                DateTime now = DateTime.Now;
#pragma warning disable CS0618 // Type or member is obsolete
                ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(_activity);
#pragma warning restore CS0618 // Type or member is obsolete
                string timeStamp = preferences.GetString(Key, string.Empty);
                if (string.IsNullOrEmpty(timeStamp)
                    || string.IsNullOrWhiteSpace(timeStamp))
                {
                    string dateString = now.ToString();
                    ISharedPreferencesEditor editor = preferences.Edit();
                    editor.PutString(Key, dateString);
                    editor.Apply();
                    return true;
                }
                else
                {
                    DateTime storeDateTime = DateTime.Parse(timeStamp);
                    TimeSpan timeSpan = now - storeDateTime;
                    double minutes = timeSpan.TotalMinutes;
                    if (interval > minutes)
                    {
                        return false;
                    }
                    else
                    {
                        string dateString = now.ToString();
                        ISharedPreferencesEditor editor = preferences.Edit();
                        editor.PutString(Key, dateString);
                        editor.Apply();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] ShouldCallSRSyncAPI Error: " + e.Message);
            }
            return false;
        }
    }
}