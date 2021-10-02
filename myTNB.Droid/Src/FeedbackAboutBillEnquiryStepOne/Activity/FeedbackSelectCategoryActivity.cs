using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System.Threading.Tasks;
using myTNB;
using myTNB.Mobile.SessionCache;
using Newtonsoft.Json;

namespace myTNB_Android.Src.FeedbackAboutBillEnquiryStepOne.Activity
{
    [Activity(Label = "Set App Category", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class FeedbackSelectCategoryActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.Category_list_view)]
        ListView CategoryListView;



        private SelectItemAdapter selectItemAdapter;
        private List<Item> CategoryItemList;
        private string CategoryKey;
        private bool isSelectionChange;
        private bool LargeCategoryOnBoard = false;
        private List<SelectorModel> _mappingList;

        public override string GetPageId()
        {
            return "";
        }

        public override int ResourceId()
        {
            return Resource.Layout.AppSetCategoryLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        private void SetSelectedCategory(string CategoryTitle)
        {
            if (CategoryTitle == null) //If lang param is null, use saved Category as selected
            {
                foreach (var item in CategoryItemList)
                {
                    if (item.type == CategoryKey)
                    {
                        CategoryTitle = item.type;
                    }
                }

            }
            CategoryItemList.ForEach(item =>
            {
                item.selected = (CategoryTitle == item.type) ? true : false;
            });
            selectItemAdapter.NotifyDataSetChanged();
           
        }

        private void UpdateLabels()
        {
            SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "selectEnquiryType"));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("SELECT_CATEGORY_REQUEST"))
                {
                    CategoryKey = extras.GetString("SELECT_CATEGORY_REQUEST");
                }
            }

            CategoryItemList = new List<Item>();
            isSelectionChange = false;

            Dictionary<string, List<SelectorModel>> selectors = LanguageManager.Instance.GetSelectorsByPage("SubmitEnquiry");
            _mappingList = new List<SelectorModel>();
            if (selectors != null && selectors.ContainsKey("enquiryType"))
            {
                _mappingList = selectors["enquiryType"];
            }

            foreach (SelectorModel selectorModel in _mappingList)
            {
                Item item = new Item();
                item.title = selectorModel.Description;
                item.type = selectorModel.Key;
                item.selected = false;
                CategoryItemList.Add(item);
            }

            CategoryItemList.ForEach(item =>
            {
                item.selected = (CategoryKey == item.type) ? true : false;
            });

            CategoryListView.ItemClick += OnItemClick;

            selectItemAdapter = new SelectItemAdapter(this, CategoryItemList);
            CategoryListView.Adapter = selectItemAdapter;
            UpdateLabels();
            SetSelectedCategory(null);
            
        }

        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Item selectedItem = selectItemAdapter.GetItemObject(e.Position);
            CategoryKey = selectedItem.type;
            SetSelectedCategory(selectedItem.type);
            Intent finishIntent = new Intent();
            finishIntent.PutExtra("SELECT_CATEGORY_REQUEST", JsonConvert.SerializeObject(CategoryItemList));
            SetResult(Result.Ok, finishIntent);
            Finish();
        }

        public override void OnBackPressed()
        {
            OnBackProceed();
        }



        public void OnBackProceed()
        {
            SetResult(Result.Ok);
            Finish();
            base.OnBackPressed();
        }

        private Task RunUpdateCategory(Item selectedItem)
        {
            SearchApplicationTypeCache.Instance.Clear();
            return Task.Run(() =>
            {
                ShowProgressDialog();
                OnBackProceed();
                selectItemAdapter = new SelectItemAdapter(this, CategoryItemList);
                CategoryListView.Adapter = selectItemAdapter;
                HideShowProgressDialog();
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