using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Preferences;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.SessionCache;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Async
{
    public class EligibilityAPI : AsyncTask
    {
        private DashboardHomeActivity _activity;
        private Context mView;

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

            if (EligibilityManager.Instance.ShouldCallApi(AWSConstants.Services.GetEligibility
                 , eligibilityTimeStamp))
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

            if (DBRUtility.Instance.IsAccountDBREligible
                    && !EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                        , EligibilitySessionCache.FeatureProperty.TargetGroup))
            {
                List<string> dbrCAList = AccountTypeCache.Instance.GetDBRAccountList();
                List<string> residentialList = new List<string>();
                if (dbrCAList != null && dbrCAList.Count > 0)
                {
                    PostMultiInstallationDetailsResponse installationdetailsResponse = await DBRManager.Instance.PostMultiInstallationDetails(dbrCAList
                        , AccessTokenCache.Instance.GetAccessToken(_activity));
                    if (installationdetailsResponse != null
                        && installationdetailsResponse.StatusDetail != null
                        && installationdetailsResponse.StatusDetail.IsSuccess
                        && installationdetailsResponse.Content != null
                        && installationdetailsResponse.Content.Count > 0)
                    {
                        for (int i = 0; i < dbrCAList.Count; i++)
                        {
                            if (installationdetailsResponse.Content.ContainsKey(dbrCAList[i])
                                && installationdetailsResponse.Content[dbrCAList[i]] is List<PostInstallationDetailsResponseModel> installationDetail
                                && installationDetail != null
                                && installationDetail.Count > 0
                                && installationDetail[0] != null
                                && installationDetail[0].IsResidential)
                            {
                                residentialList.Add(dbrCAList[i]);
                            }
                        }
                        AccountTypeCache.Instance.UpdateCATariffType(residentialList);

                        PostMultiBillRenderingResponse multiBillRenderingResponse = await DBRManager.Instance.PostMultiBillRendering(residentialList
                            , AccessTokenCache.Instance.GetAccessToken(_activity));
                        if (multiBillRenderingResponse != null
                            && multiBillRenderingResponse.StatusDetail != null
                            && multiBillRenderingResponse.StatusDetail.IsSuccess
                            && multiBillRenderingResponse.Content != null
                            && multiBillRenderingResponse.Content.Count > 0)
                        {
                            AccountTypeCache.Instance.UpdateCARendering(multiBillRenderingResponse.Content, residentialList);
                            Console.WriteLine("Debug 0: " + JsonConvert.SerializeObject(residentialList));
                            Console.WriteLine("Debug 1: " + JsonConvert.SerializeObject(AccountTypeCache.Instance.DBREligibleCAs));
                        }
                    }
                }
            }

            bool IsAccountDBREligible = DBRUtility.Instance.ShouldShowHomeDBRCard;
            _activity.ShowHomeDBRCard(IsAccountDBREligible);
        }
    }

    public class CustomEligibility
    {

        private static readonly Lazy<CustomEligibility> lazy =
            new Lazy<CustomEligibility>(() => new CustomEligibility());
        public static CustomEligibility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public CustomEligibility() { }

        public async Task<bool> EvaluateEligibility(Context mView)
        {
            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(mView);
#pragma warning restore CS0618 // Type or member is obsolete
                string eligibilityTimeStamp = preferences.GetString(MobileConstants.SharePreferenceKey.GetEligibilityTimeStamp, string.Empty);

                if (EligibilityManager.Instance.ShouldCallApi(AWSConstants.Services.GetEligibility
                   , eligibilityTimeStamp))
                {
                    if (!AccessTokenCache.Instance.HasTokenSaved(mView))
                    {
                        //string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                        string accessToken;
                        int i = 0;
                        do
                        {
                            accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                            i++;
                        } while (string.IsNullOrEmpty(accessToken) && i <= 4);
                        AccessTokenCache.Instance.SaveAccessToken(mView, accessToken);
                    }

                    GetEligibilityResponse response = await EligibilityManager.Instance.GetEligibility(UserEntity.GetActive().UserID ?? string.Empty
                        , AccessTokenCache.Instance.GetAccessToken(mView));

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
                        FeatureInfoManager.Instance.SetData(data);
                        MyTNBAccountManagement.GetInstance().SetFinishApiEB(true);
                        //Use data or any EligibilitySessionCache functionality
                    }
                    else
                    {
                        MyTNBAccountManagement.GetInstance().SetFinishApiEB(true);
                    }
                }
                else if (EligibilityManager.Instance.IsEnabled(AWSConstants.Services.GetEligibility)
                    && preferences.GetString(MobileConstants.SharePreferenceKey.GetEligibilityData, string.Empty) is string encryptedData
                    && !string.IsNullOrEmpty(encryptedData)
                    && !string.IsNullOrWhiteSpace(encryptedData))
                {
                    GetEligibilityResponse data = SecurityManager.Instance.Decrypt<GetEligibilityResponse>(encryptedData);
                    EligibilitySessionCache.Instance.SetData(data);
                    FeatureInfoManager.Instance.SetData(data);
                    MyTNBAccountManagement.GetInstance().SetFinishApiEB(true);
                    //Use data or any EligibilitySessionCache functionality
                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Eligibility API Error: " + e.Message);
            }
            return true;
        }
    }
}