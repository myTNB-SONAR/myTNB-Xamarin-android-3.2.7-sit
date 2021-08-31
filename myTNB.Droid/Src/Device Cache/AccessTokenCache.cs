using System;
using Android.Content;
using Android.Preferences;
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

        internal void SaveAccessToken(Context activity, string token)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(activity);
#pragma warning restore CS0618 // Type or member is obsolete
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutString(MobileConstants.SharePreferenceKey.AccessToken, token);
            editor.Apply();
            Token = token;
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
            }
            return Token;
        }

        internal bool HasTokenSaved(Context activity)
        {
            string tkn = GetAccessToken(activity);
            return !string.IsNullOrEmpty(tkn)
                && !string.IsNullOrWhiteSpace(tkn);
        }

        public void ClearToken()
        {
            Token = string.Empty;
        }
    }
}