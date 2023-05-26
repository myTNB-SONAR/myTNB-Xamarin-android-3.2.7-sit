using System;
using Android.Content;
using Android.Preferences;
using Android.Util;
using myTNB;
using myTNB.Mobile;

namespace myTNB_Android.Src.DeviceCache
{
    internal sealed class AccessTokenCache
    {
        private static readonly Lazy<AccessTokenCache> lazy
            = new Lazy<AccessTokenCache>(() => new AccessTokenCache());

        internal static AccessTokenCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        internal AccessTokenCache() { }

        private string Token { set; get; } = string.Empty;
        private string UserAccessToken { set; get; } = string.Empty;

        internal void SaveAccessToken(Context activity, string token)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(activity);
#pragma warning restore CS0618 // Type or member is obsolete
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutString(MobileConstants.SharePreferenceKey.AccessToken, token);
            editor.Apply();
            Token = token;
            AppInfoManager.Instance.AccessToken = token;
        }

        internal void SaveUserServiceAccessToken(Context activity, string token)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(activity);
#pragma warning restore CS0618 // Type or member is obsolete
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutString(MobileConstants.SharePreferenceKey.UserServiceAccessToken, token);
            editor.Apply();
            UserAccessToken = token;
        }

        internal string GetAccessToken(Context activity)
        {
            if (string.IsNullOrEmpty(Token)
                || string.IsNullOrWhiteSpace(Token))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(activity);
#pragma warning restore CS0618 // Type or member is obsolete
                Token = preferences.GetString(MobileConstants.SharePreferenceKey.AccessToken, string.Empty) ?? string.Empty;
                AppInfoManager.Instance.AccessToken = Token;
            }
            Log.Debug("Access Token", Token);
            return Token;
        }

        internal string GetUserServiceAccessToken(Context activity)
        {
            if (string.IsNullOrEmpty(UserAccessToken)
                || string.IsNullOrWhiteSpace(UserAccessToken))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(activity);
#pragma warning restore CS0618 // Type or member is obsolete
                UserAccessToken = preferences.GetString(MobileConstants.SharePreferenceKey.UserServiceAccessToken, string.Empty) ?? string.Empty;
            }
            Log.Debug("User Service Access Token", UserAccessToken);
            return UserAccessToken;
        }

        internal bool HasTokenSaved(Context activity)
        {
            string tkn = GetAccessToken(activity);
            return !string.IsNullOrEmpty(tkn)
                && !string.IsNullOrWhiteSpace(tkn);
        }

        internal bool HasUserAccessTokenSaved(Context activity)
        {
            string tkn = GetUserServiceAccessToken(activity);
            return !string.IsNullOrEmpty(tkn)
                && !string.IsNullOrWhiteSpace(tkn);
        }

        internal void Clear()
        {
            Token = string.Empty;
            UserAccessToken = string.Empty;
        }
    }
}