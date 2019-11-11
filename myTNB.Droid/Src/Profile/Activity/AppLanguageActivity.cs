
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Database.Model;
using Android.Support.V4.Content;
using myTNB;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.Preferences;

namespace myTNB_Android.Src.Profile.Activity
{
    [Activity(Label = "Set App Language", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class AppLanguageActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.language_list_view)]
        ListView languageListView;

        [BindView(Resource.Id.btnSaveChanges)]
        Button btnSaveChanges;

        [BindView(Resource.Id.appLanguageMessage)]
        TextView appLanguageMessage;

        private SelectItemAdapter selectItemAdapter;
        private List<Item> languageItemList;
        private string savedLanguage;

        public override string GetPageId()
        {
            return "";
        }

        public override int ResourceId()
        {
            return Resource.Layout.AppLanguageLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        private void SetSelectedLanguage(string languageTitle)
        {
            if (languageTitle == null) //If lang param is null, use saved language as selected
            {
                languageTitle = GetLanguageLabelByCode(savedLanguage);
            }
            languageItemList.ForEach(item =>
            {
                item.selected = (languageTitle == item.title) ? true : false;
            });
            selectItemAdapter.NotifyDataSetChanged();
            EnableDisableButton();
        }

        private string GetLanguageLabelByCode(string lang)
        {
            string langLabel;
            switch (lang)
            {
                case "EN":
                    {
                        langLabel = "English";
                        break;
                    }
                case "MS":
                    {
                        langLabel = "Bahasa Malaysia";
                        break;
                    }
                default:
                    {
                        langLabel = "";
                        break;
                    }
            }
            return langLabel;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TextViewUtils.SetMuseoSans500Typeface(appLanguageMessage,btnSaveChanges);
            savedLanguage = LanguageUtil.GetAppLanguage();
            languageItemList = new List<Item>();

            foreach (string languageName in Enum.GetNames(typeof(Constants.SUPPORTED_LANGUAGES)))
            {
                Item item = new Item();
                item.title = GetLanguageLabelByCode(languageName);
                item.type = languageName;
                item.selected = false;
                languageItemList.Add(item);
            }

            languageListView.ItemClick += OnItemClick;

            selectItemAdapter = new SelectItemAdapter(this, languageItemList);
            languageListView.Adapter = selectItemAdapter;

            SetToolBarTitle(GetLabelCommonByLanguage("setAppLanguage"));
            appLanguageMessage.Text = GetLabelCommonByLanguage("setAppLanguageDescription");
            btnSaveChanges.Text = GetLabelCommonByLanguage("saveChanges");
            SetSelectedLanguage(null);
        }

        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Item selectedItem = selectItemAdapter.GetItemObject(e.Position);
            SetSelectedLanguage(selectedItem.title);
        }

        [OnClick(Resource.Id.btnSaveChanges)]
        void OnSaveChanges(object sender, EventArgs eventArgs)
        {
            Item selectedItem = languageItemList.Find(item => { return item.selected;});
            string currentLanguage = LanguageUtil.GetAppLanguage();
            Utility.ShowChangeLanguageDialog(this, currentLanguage, ()=>
            {
                LanguageUtil.SaveAppLanguage(selectedItem.type);
                UpdateLanguage();
            });
        }

        private void UpdateLanguage()
        {
            LanguageUtil.SetIsLanguageChanged(true);
            Recreate();
        }

        public void EnableDisableButton()
        {
            try
            {
                Item selectedItem = languageItemList.Find(item => { return item.selected; });

                if (selectedItem.title != GetLanguageLabelByCode(savedLanguage))
                {
                    btnSaveChanges.Enabled = true;
                    btnSaveChanges.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
                }
                else
                {
                    btnSaveChanges.Enabled = false;
                    btnSaveChanges.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            SetResult(Result.Ok);
            Finish();
            base.OnBackPressed();
        }
    }
}
