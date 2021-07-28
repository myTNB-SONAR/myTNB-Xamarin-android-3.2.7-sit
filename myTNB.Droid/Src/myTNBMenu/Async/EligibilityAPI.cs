using System;
using Android.Content;
using Android.OS;
using Android.Preferences;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.myTNBMenu.Activity;

namespace myTNB_Android.Src.myTNBMenu.Async
{
    public class EligibilityAPI : AsyncTask
    {
        private DashboardHomeActivity _activity;

        public EligibilityAPI(DashboardHomeActivity activity)
        {
            _activity = activity;
        }

        protected override void OnPreExecute() { }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            try
            {
                EvaluateEligibility();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Eligibility API Error: " + e.Message);
            }
            return null;
        }

        private async void EvaluateEligibility()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(_activity);
#pragma warning restore CS0618 // Type or member is obsolete
            string eligibilityTimeStamp = preferences.GetString(MobileConstants.SharePreferenceKey.GetEligibilityTimeStamp, string.Empty);

           //if (EligibilityManager.Instance.ShouldCallApi(AWSConstants.Services.GetEligibility
           //   , eligibilityTimeStamp))
           if(true)
            {
                if (!AccessTokenCache.Instance.HasTokenSaved(_activity))
                {
                    string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                    AccessTokenCache.Instance.SaveAccessToken(_activity, accessToken);
                }

                GetEligibilityResponse response = await EligibilityManager.Instance.GetEligibility(UserEntity.GetActive().UserID ?? string.Empty
                    , AccessTokenCache.Instance.GetAccessToken(_activity));

                //Nullity Check
                if (response != null
                   && response.StatusDetail != null
                   && response.StatusDetail.IsSuccess)
                {
                    EligibilitySessionCache.Instance.SetData(response);
                    DateTime now = DateTime.Now;
                    string encryptedData = SecurityManager.Instance.Encrypt(response);
                    ISharedPreferencesEditor editor = preferences.Edit();
                    editor.PutString(MobileConstants.SharePreferenceKey.GetEligibilityData, encryptedData);
                    editor.PutString(MobileConstants.SharePreferenceKey.GetEligibilityTimeStamp, now.ToString());
                    editor.Apply();

                    GetEligibilityResponse data = SecurityManager.Instance.Decrypt<GetEligibilityResponse>(encryptedData);
                    //Use data or any EligibilitySessionCache functionality
                }
            }
            else if (EligibilityManager.Instance.IsEnabled(AWSConstants.Services.GetEligibility)
                && preferences.GetString(MobileConstants.SharePreferenceKey.GetEligibilityData, string.Empty) is string encryptedData
                && !string.IsNullOrEmpty(encryptedData)
                && !string.IsNullOrWhiteSpace(encryptedData))
            {
                GetEligibilityResponse data = SecurityManager.Instance.Decrypt<GetEligibilityResponse>(encryptedData);
                EligibilitySessionCache.Instance.SetData(data);

                //Use data or any EligibilitySessionCache functionality
            }
        }
    }
}