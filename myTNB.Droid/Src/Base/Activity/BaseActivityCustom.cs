using System;
using System.Collections.Generic;
using Android.OS;
using myTNB;

namespace myTNB_Android.Src.Base.Activity
{
    public abstract class BaseActivityCustom : BaseToolbarAppCompatActivity
    {
        private Dictionary<string, string> languageKeyValue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            languageKeyValue = LanguageManager.Instance.GetValuesByPage(GetPageId());
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
            return languageKeyValue[key];
        }
    }
}
