using System;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Base.Activity
{
    public abstract class BaseActivityCustom : BaseToolbarAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string title = GetLabelByLanguage("title");
            SetToolBarTitle(title);

            Android.Content.Res.Configuration configuration = Resources.Configuration;
            configuration.FontScale = (float)1; //0.85 small size, 1 normal size, 1,15 big etc
            var metrics = this.ApplicationContext.Resources.DisplayMetrics;
            metrics.ScaledDensity = configuration.FontScale * metrics.Density;
            this.Resources.UpdateConfiguration(configuration, metrics);
        }
        protected override void AttachBaseContext(Context baseContext)
        {

            Context newContext;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                DisplayMetrics displayMetrics = baseContext.Resources.DisplayMetrics;
                Configuration configuration = baseContext.Resources.Configuration;
                configuration.DensityDpi = DisplayMetrics.DensityDeviceStable;
                newContext = baseContext.CreateConfigurationContext(configuration);
            }
            else
            {
                // Old API. Screen zoom not supported
                newContext = baseContext;
            }
            base.AttachBaseContext(newContext);
        }
        /// <summary>
        /// Gets the Page Id. To be implemented by child activity.
        /// </summary>
        /// <returns></returns>
        public abstract string GetPageId();

        /// <summary>
        /// Gets the label based on selected language.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLabelByLanguage(string key)
        {
            return Utility.GetLocalizedLabel(GetPageId(), key);
        }

        /// <summary>
        /// Gets the common labels
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLabelCommonByLanguage(string key)
        {
            return Utility.GetLocalizedLabel("Common", key);
        }
        /// <summary>
        /// Gets the common labels
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLabelSelectFontSize(string key)
        {
            return Utility.GetLocalizedLabel("SelectFontSize", key);
        }
    }
}
