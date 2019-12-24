
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
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System.Threading.Tasks;
using System.Threading;

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
        private bool isSelectionChange;
        private LoadingOverlay loadingOverlay;

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

        private void UpdateLabels()
        {
            SetToolBarTitle(GetLabelCommonByLanguage("setAppLanguage"));
            appLanguageMessage.Text = GetLabelCommonByLanguage("setAppLanguageDescription");
            btnSaveChanges.Text = GetLabelCommonByLanguage("saveChanges");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TextViewUtils.SetMuseoSans500Typeface(appLanguageMessage,btnSaveChanges);
            savedLanguage = LanguageUtil.GetAppLanguage();
            languageItemList = new List<Item>();
            isSelectionChange = false;

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
            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            languageListView.Adapter = selectItemAdapter;
            UpdateLabels();
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
                ShowProgressDialog();
                _ = RunUpdateLanguage(selectedItem);
            });
        }

        private async void UpdateLanguage()
        {
            savedLanguage = LanguageUtil.GetAppLanguage();
            await LanguageUtil.CheckUpdatedLanguage();
            UpdateLabels();
            EnableDisableButton();
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
                    isSelectionChange = true;
                }
                else
                {
                    btnSaveChanges.Enabled = false;
                    btnSaveChanges.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                    isSelectionChange = false;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            if (isSelectionChange)
            {
                ShowTooltipConfirm();
            }
            else
            {
                OnBackProceed();
            }
        }

        private void ShowTooltipConfirm()
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Common", "changeLanguageTitle_" + savedLanguage))
                        .SetMessage(Utility.GetLocalizedLabel("Common", "saveLanguageMessage"))
                        .SetContentGravity(GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageNo_" + savedLanguage))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageYes_" + savedLanguage))
                        .SetSecondaryCTAaction(() =>
                        {
                            ShowProgressDialog();
                            Item selectedItem = languageItemList.Find(item => { return item.selected; });
                            _ = RunUpdateLanguage(selectedItem);
                        }).Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                tooltipBuilder.DismissDialog();
                OnBackProceed();
            }).Show();
        }

        public void OnBackProceed()
        {
            SetResult(Result.Ok);
            Finish();
            base.OnBackPressed();
        }

        private Task RunUpdateLanguage(Item selectedItem)
        {
            return Task.Run(() =>
            {
                LanguageUtil.SaveAppLanguage(selectedItem.type);
                MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                RunOnUiThread(() =>
                {
                    MyTNBAccountManagement.GetInstance().ClearSitecoreItem();
                    MyTNBAccountManagement.GetInstance().ClearAppCacheItem();
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();
                    SMRPopUpUtils.SetSSMRMeterReadingRefreshNeeded(true);
                    SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                    UpdateLanguage();
                    OnBackProceed();
                    HideShowProgressDialog();
                });
            });
        }

        public void ShowProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideShowProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
