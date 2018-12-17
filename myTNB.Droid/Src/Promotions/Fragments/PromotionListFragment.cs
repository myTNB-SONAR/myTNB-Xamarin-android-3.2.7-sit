using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using CheeseBind;
using myTNB_Android.Src.Promotions.MVP;
using Android.Support.V7.Widget;
using myTNB_Android.Src.Utils;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Promotions.Adapter;
using myTNB.SitecoreCM.Models;
using myTNB_Android.Src.Promotions.Activity;
using Newtonsoft.Json;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.Promotions.Fragments
{
    public class PromotionListFragment : BaseFragment, PromotionContract.IView
    {
        private PromotionPresenter mPresenter;
        private PromotionContract.IUserActionsListener userActionsListener;        

        List<PromotionsModelV2> promotions = new List<PromotionsModelV2>();
        private PromotionListAdapter adapter;
        private LinearLayoutManager layoutManager;

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.no_promotion_layout)]
        LinearLayout noPromotionLayout;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        [BindView(Resource.Id.promotion_list_recycler_view)]
        public RecyclerView mPromotionRecyclerView;

        [BindView(Resource.Id.no_promotion_title)]
        public TextView textNoPromotion;

        [BindView(Resource.Id.no_promotion_info)]
        public TextView textNoPromotionInfo;

        private string savedTimeStamp = "0000000";

        private LoadingOverlay loadingOverlay;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new PromotionPresenter(this);

        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            
            
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try {
            layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
            mPromotionRecyclerView.SetLayoutManager(layoutManager);
            adapter = new PromotionListAdapter(Activity, promotions);
            mPromotionRecyclerView.SetAdapter(adapter);

            TextViewUtils.SetMuseoSans300Typeface(textNoPromotion, textNoPromotionInfo);

            ShowProgressBar();
            this.userActionsListener.GetSavedPromotionTimeStamp();
            //ShowPromotion(true);
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(Activity, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.PromotionListView;
        }

        public void ShowPromotion(bool success)
        {
            Activity.RunOnUiThread(() =>
            {
                try {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
                if (success)
                {
                    noPromotionLayout.Visibility = ViewStates.Gone;
                    mPromotionRecyclerView.Visibility = ViewStates.Visible;

                    PromotionsEntityV2 wtManager = new PromotionsEntityV2();
                    List<PromotionsEntityV2> items = wtManager.GetAllItems();
                    if (items != null)
                    {
                        promotions = new List<PromotionsModelV2>();
                        promotions.AddRange(items);
                        adapter = new PromotionListAdapter(Activity, promotions);
                        mPromotionRecyclerView.SetAdapter(adapter);
                        adapter.ItemClick += OnItemClick;
                        adapter.NotifyDataSetChanged();
                    }

                }
                else
                {
                    noPromotionLayout.Visibility = ViewStates.Visible;
                    mPromotionRecyclerView.Visibility = ViewStates.Gone;
                }
                }
                catch (Exception ex)
                {
                    Utility.LoggingNonFatalError(ex);
                }
            });
        }

        void OnItemClick(object sender, int position)
        {
            try {
            PromotionsModelV2 model = promotions[position];
            PromotionsEntityV2 wtManager = new PromotionsEntityV2()
            {
                ID = model.ID,
                GeneralLinkUrl = model.GeneralLinkUrl,
                Text = model.Text,
                Title = model.Title,
                HeaderContent = model.HeaderContent,
                BodyContent = model.BodyContent,
                FooterContent = model.FooterContent,
                PortraitImage = model.PortraitImage.Replace(" ", "%20"),
                LandscapeImage = model.LandscapeImage.Replace(" ", "%20"),
                PromoStartDate = model.PromoStartDate,
                PromoEndDate = model.PromoEndDate,
                PublishedDate = model.PublishedDate,
                IsPromoExpired = model.IsPromoExpired,
                Read = true
            };
            wtManager.UpdateItem(wtManager);
            Intent details_activity = new Intent(Activity, typeof(PromotionsActivity));
            details_activity.PutExtra("Promotion", JsonConvert.SerializeObject(model));
            //Activity.StartActivity(details_activity);
            Activity.StartActivity(details_activity);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

            public void SetPresenter(PromotionContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return true;
        }

        public void ShowProgressBar()
        {
            mProgressBar.Visibility = ViewStates.Gone;
        }

        public void HideProgressBar()
        {
            mProgressBar.Visibility = ViewStates.Gone;
        }

        public void OnSavedTimeStamp(string mSavedTimeStamp)
        {
            try {
            if(mSavedTimeStamp != null)
            {
                this.savedTimeStamp = mSavedTimeStamp;
            }
            this.userActionsListener.OnGetPromotionsTimeStamp();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowPromotionTimestamp(bool success)
        {
            try {
            if (success)
            {
                PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                List<PromotionsParentEntityV2> items = wtManager.GetAllItems();
                if (items != null)
                {
                    PromotionsParentEntityV2 entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedTimeStamp))
                        {
                            this.userActionsListener.OnGetPromotions();
                        }
                        else
                        {
                            ShowPromotion(true);
                        }
                    }
                }

            }
            else
            {
                ShowPromotion(false);
            }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnResume()
        {
            base.OnResume();
            try {
            PromotionsEntityV2 wtManager = new PromotionsEntityV2();
            List<PromotionsEntityV2> items = wtManager.GetAllItems();
            if (items != null)
            {
                promotions = new List<PromotionsModelV2>();
                promotions.AddRange(items);
                adapter = new PromotionListAdapter(Activity, promotions);
                mPromotionRecyclerView.SetAdapter(adapter);
                adapter.ItemClick += OnItemClick;
                adapter.NotifyDataSetChanged();
            }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}