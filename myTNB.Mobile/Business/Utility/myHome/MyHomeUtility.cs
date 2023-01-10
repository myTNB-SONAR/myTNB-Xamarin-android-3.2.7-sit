namespace myTNB.Mobile.Business
{
    public static class MyHomeUtility
    {
        public static bool IsMarketingPopupEnabled
        {
            get
            {
                return LanguageManager.Instance.GetConfigToggleValue(LanguageManager.ConfigPropertyEnum.IsMyHomeMarketingPopupEnable);
            }
        }

        public static bool IsBannerHidden
        {
            get
            {
                return LanguageManager.Instance.GetConfigToggleValue(LanguageManager.ConfigPropertyEnum.ForceHidemyHomeBanner);
            }
        }
    }
}