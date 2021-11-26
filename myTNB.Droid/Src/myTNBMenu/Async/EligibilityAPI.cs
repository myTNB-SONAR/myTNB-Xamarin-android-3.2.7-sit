using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Preferences;
using Android.Util;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.SessionCache;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Async
{
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
                        string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
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

                        FeatureInfoManager.Instance.SetData(response);
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

                if (DBRUtility.Instance.IsAccountEligible
                    && !EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                        , EligibilitySessionCache.FeatureProperty.TargetGroup))
                {
                    List<string> dbrCAList = AccountTypeCache.Instance.GetDBRAccountList();
                    List<string> residentialList = new List<string>();
                    if (dbrCAList != null && dbrCAList.Count > 0)
                    {
                        PostMultiInstallationDetailsResponse installationdetailsResponse = await DBRManager.Instance.PostMultiInstallationDetails(dbrCAList
                            , AccessTokenCache.Instance.GetAccessToken(mView));
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
                                , AccessTokenCache.Instance.GetAccessToken(mView));
                            if (multiBillRenderingResponse != null
                                && multiBillRenderingResponse.StatusDetail != null
                                && multiBillRenderingResponse.StatusDetail.IsSuccess
                                && multiBillRenderingResponse.Content != null
                                && multiBillRenderingResponse.Content.Count > 0)
                            {
                                AccountTypeCache.Instance.UpdateCARendering(multiBillRenderingResponse.Content, residentialList);
                                Log.Debug("[DEBUG]", "residentialList: " + JsonConvert.SerializeObject(residentialList));
                                Log.Debug("[DEBUG]", "DBREligibleCAs: " + JsonConvert.SerializeObject(AccountTypeCache.Instance.DBREligibleCAs));
                            }
                        }
                    }
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