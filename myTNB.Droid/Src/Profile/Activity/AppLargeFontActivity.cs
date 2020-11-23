
using System;
using System.Collections.Generic;
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
using myTNB_Android.Src.Base;
using System.Threading.Tasks;
using myTNB_Android.Src.Maintenance.Activity;
using AndroidX.Core.Content;
using myTNB;
using myTNB.Mobile.SessionCache;

namespace myTNB_Android.Src.Profile.Activity
{
    [Activity(Label = "Set App Font", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class AppLargeFontActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.Font_list_view)]
        ListView FontListView;

        [BindView(Resource.Id.btnSaveChanges)]
        Button btnSaveChanges;

        [BindView(Resource.Id.appFontMessage)]
        TextView appFontMessage;

        private SelectItemAdapter selectItemAdapter;
        private List<Item> FontItemList;
        private string savedFont;
        private string FontName;
        private bool isSelectionChange;
        List<SelectorModel> _mappingList;
        public override string GetPageId()
        {
            return "";
        }

        public override int ResourceId()
        {
            return Resource.Layout.AppSetFontLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        private void SetSelectedFont(string FontTitle)
        {
            if (FontTitle == null) //If lang param is null, use saved Font as selected
            {
                foreach(var item in FontItemList)
                {
                    if(item.type == savedFont)
                    {
                        FontTitle = item.type;
                        
                    }
                }
               
            }
            FontItemList.ForEach(item =>
            {
                item.selected = (FontTitle == item.type) ? true : false;
            });
            selectItemAdapter.NotifyDataSetChanged();
            EnableDisableButton();
        }

       
        private void UpdateLabels()
        {
            SetToolBarTitle(GetLabelSelectFontSize("title"));
            appFontMessage.Text = GetLabelSelectFontSize("sectionTitle");
            btnSaveChanges.Text = GetLabelSelectFontSize("ctaTitle");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            TextViewUtils.SetMuseoSans500Typeface(appFontMessage, btnSaveChanges);
            appFontMessage.TextSize = TextViewUtils.GetFontSize(16);
            btnSaveChanges.TextSize = TextViewUtils.GetFontSize(16);

            savedFont = TextViewUtils.SelectedFontSize();
            FontItemList = new List<Item>();
            isSelectionChange = false;


            Dictionary<string, List<SelectorModel>> selectors = LanguageManager.Instance.GetSelectorsByPage("SelectFontSize");
            _mappingList = new List<SelectorModel>();
            if (selectors != null && selectors.ContainsKey("fonts"))
            {
                _mappingList = selectors["fonts"];
            }

            foreach (SelectorModel selectorModel in _mappingList)
            {
                Item item = new Item();
                item.title = selectorModel.Value;
                item.type = selectorModel.Key;
                item.selected = false;
                FontItemList.Add(item);
            }

            FontListView.ItemClick += OnItemClick;

            selectItemAdapter = new SelectItemAdapter(this, FontItemList);
            FontListView.Adapter = selectItemAdapter;
            UpdateLabels();
            SetSelectedFont(null);
        }

        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Item selectedItem = selectItemAdapter.GetItemObject(e.Position);
            FontName = selectedItem.title;
            SetSelectedFont(selectedItem.type);
        }

        [OnClick(Resource.Id.btnSaveChanges)]
        void OnSaveChanges(object sender, EventArgs eventArgs)
        {
            Item selectedItem = FontItemList.Find(item => { return item.selected; });
            //string currentFont = TextViewUtils.SelectedFontSize();

            ShowProgressDialog();
               
                _ = RunUpdateFont(selectedItem);

            HideShowProgressDialog();
           
        }

        private void UpdateFont()
        {
            savedFont = TextViewUtils.SelectedFontSize();
           
           // UpdateLabels();
            EnableDisableButton();
        }

        public void EnableDisableButton()
        {
            try
            {
                Item selectedItem = FontItemList.Find(item => { return item.selected; });
                string FontTitle = string.Empty;
                foreach (var item in FontItemList)
                {
                    if (item.type == savedFont)
                    {
                        FontTitle = item.type;
                     
                    }
                }

                if (selectedItem.type != FontTitle)
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
                        .SetTitle(GetLabelSelectFontSize("popupTitleFormat"))
                        .SetMessage(string.Format(GetLabelSelectFontSize("popupMessage"), FontName))
                        .SetContentGravity(GravityFlags.Center)
                        .SetCTALabel(GetLabelCommonByLanguage("no"))
                        .SetSecondaryCTALabel(GetLabelCommonByLanguage("yes"))
                        .SetSecondaryCTAaction(() =>
                        {
                            ShowProgressDialog();
                            Item selectedItem = FontItemList.Find(item => { return item.selected; });
                            _ = RunUpdateFont(selectedItem);
                            HideShowProgressDialog();
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

        private Task RunUpdateFont(Item selectedItem)
        {
            SearchApplicationTypeCache.Instance.Clear();
            return Task.Run(() =>
            {
                TextViewUtils.SaveFontSize(selectedItem);
                UpdateFont();
                OnBackProceed();
            });
        }

     

    

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
