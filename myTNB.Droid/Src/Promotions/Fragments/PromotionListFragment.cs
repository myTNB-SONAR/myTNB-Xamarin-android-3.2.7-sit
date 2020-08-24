using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;


using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB.SitecoreCM.Models;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Promotions.Activity;
using myTNB_Android.Src.Promotions.Adapter;
using myTNB_Android.Src.Promotions.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

        [BindView(Resource.Id.promotionRefreshLayout)]
        LinearLayout promotionRefreshLayout;

        [BindView(Resource.Id.no_promotion_layout)]
        LinearLayout noPromotionLayout;

        [BindView(Resource.Id.promotion_main_layout)]
        LinearLayout promotionMainLayout;

        [BindView(Resource.Id.promotion_list_recycler_view)]
        public RecyclerView mPromotionRecyclerView;

        [BindView(Resource.Id.no_promotion_info)]
        public TextView textNoPromotionInfo;

        [BindView(Resource.Id.txtRefresh)]
        public TextView txtRefresh;

        [BindView(Resource.Id.btnRefresh)]
        public Button btnRefresh;


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
            try
            {
                layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
                mPromotionRecyclerView.SetLayoutManager(layoutManager);
                adapter = new PromotionListAdapter(Activity, promotions);
                mPromotionRecyclerView.SetAdapter(adapter);

                TextViewUtils.SetMuseoSans300Typeface(textNoPromotionInfo, txtRefresh);
                TextViewUtils.SetMuseoSans500Typeface(btnRefresh);

                ShowProgressBar();

                OnGetPromotionTimestamp();

                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.PromotionListView;
        }

        public void OnGetPromotionTimestamp()
        {
            try
            {
                this.userActionsListener.GetSavedPromotionTimeStamp();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowPromotion(bool success)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        if (success)
                        {
                            PromotionsEntityV2 wtManager = new PromotionsEntityV2();
                            List<PromotionsEntityV2> items = wtManager.GetAllItems();
                            if (items != null && items.Count > 0)
                            {
                                noPromotionLayout.Visibility = ViewStates.Gone;
                                promotionRefreshLayout.Visibility = ViewStates.Gone;
                                promotionMainLayout.Visibility = ViewStates.Visible;
                                promotions = new List<PromotionsModelV2>();
                                promotions.AddRange(items);
                                adapter = new PromotionListAdapter(Activity, promotions);
                                mPromotionRecyclerView.SetAdapter(adapter);
                                adapter.ItemClick += OnItemClick;
                                adapter.NotifyDataSetChanged();
                            }
                            else
                            {
                                noPromotionLayout.Visibility = ViewStates.Visible;
                                promotionRefreshLayout.Visibility = ViewStates.Gone;
                                promotionMainLayout.Visibility = ViewStates.Gone;
                                textNoPromotionInfo.Text = Utility.GetLocalizedLabel("Promotions", "noPromotions");
                            }

                        }
                        else
                        {
                            noPromotionLayout.Visibility = ViewStates.Gone;
                            promotionRefreshLayout.Visibility = ViewStates.Visible;
                            promotionMainLayout.Visibility = ViewStates.Gone;
                            btnRefresh.Text = Utility.GetLocalizedCommonLabel("refreshNow");
                            txtRefresh.Text = Utility.GetLocalizedCommonLabel("refreshDescription");
                        }

                        HideProgressBar();

                        try
                        {
                            if (PromotionsEntityV2.HasUnread())
                            {
                                ((DashboardHomeActivity)this.Activity).ShowUnreadPromotions(true);
                            }
                            else
                            {
                                ((DashboardHomeActivity)this.Activity).HideUnreadPromotions(true);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }


                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
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
                    promotions[position].Read = true;
                    adapter.NotifyDataSetChanged();
                    Intent details_activity = new Intent(Activity, typeof(PromotionsActivity));
                    details_activity.PutExtra("Promotion", JsonConvert.SerializeObject(model));
                    //Activity.StartActivity(details_activity);
                    Activity.StartActivity(details_activity);
                }
            }
            catch (System.Exception ex)
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
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this.Activity, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressBar()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedTimeStamp(string mSavedTimeStamp)
        {
            try
            {
                if (mSavedTimeStamp != null)
                {
                    this.savedTimeStamp = mSavedTimeStamp;
                }
                this.userActionsListener.OnGetPromotionsTimeStamp();
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                }
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            base.OnAttach(context);
        }

        public void ShowPromotionTimestamp(bool success)
        {
            try
            {
                if (success)
                {
                    PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                    List<PromotionsParentEntityV2> items = wtManager.GetAllItems();
                    if (items != null && items.Count() > 0)
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
                                PromotionsEntityV2 itemEntity = new PromotionsEntityV2();
                                List<PromotionsEntityV2> subItems = itemEntity.GetAllItems();
                                if (subItems != null && subItems.Count > 0)
                                {
                                    ShowPromotion(true);
                                }
                                else
                                {
                                    this.userActionsListener.OnGetPromotions();
                                }
                            }
                        }
                        else
                        {
                            this.userActionsListener.OnGetPromotions();
                        }
                    }
                    else
                    {
                        this.userActionsListener.OnGetPromotions();
                    }

                }
                else
                {
                    this.userActionsListener.OnGetPromotions();
                }
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        // On Press Refresh button action
        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            try
            {
                promotionRefreshLayout.Visibility = ViewStates.Gone;
                noPromotionLayout.Visibility = ViewStates.Gone;
                promotionMainLayout.Visibility = ViewStates.Visible;
                promotions = new List<PromotionsModelV2>();
                adapter = new PromotionListAdapter(Activity, promotions);
                mPromotionRecyclerView.SetAdapter(adapter);
                adapter.ItemClick += OnItemClick;
                adapter.NotifyDataSetChanged();

                this.userActionsListener.GetSavedPromotionTimeStamp();

                ShowProgressBar();
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            try
            {
                var act = this.Activity as AppCompatActivity;

                var actionBar = act.SupportActionBar;
                actionBar.Show();
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}
