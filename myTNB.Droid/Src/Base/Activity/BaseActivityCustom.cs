using System;
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
