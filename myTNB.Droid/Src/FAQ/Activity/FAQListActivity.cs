using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.FAQ.Adapter;
using myTNB.AndroidApp.Src.FAQ.Model;
using myTNB.AndroidApp.Src.FAQ.MVP;
using myTNB.AndroidApp.Src.Utils;
using myTNB.SitecoreCMS.Models;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.FAQ.Activity
{
    [Activity(Label = "@string/faq_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class FAQListActivity : BaseActivityCustom, FAQContract.IView
    {

        private FAQPresenter mPresenter;
        private FAQContract.IUserActionsListener userActionsListener;

        List<FAQsModel> faqs = new List<FAQsModel>();
        private FAQListAdapter adapter;
        private LinearLayoutManager layoutManager;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        [BindView(Resource.Id.faq_list_recycler_view)]
        public RecyclerView mFAQRecyclerView;

        private string mSavedTimeStamp = "0000000";

        private string FAQ_ID = null;

        const string PAGE_ID = "FAQ";

        public void HideProgressBar()
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

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FAQListView;
        }

        public void SetPresenter(FAQContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowFAQ(bool success)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        HideProgressBar();
                        if (success)
                        {
                            FAQsEntity wtManager = new FAQsEntity();
                            List<FAQsEntity> items = wtManager.GetAllItems();
                            List<FAQsEntity> newList = new List<FAQsEntity>();
                            if (items != null && items.Count > 0)
                            {
                                items.ForEach(item =>
                                {
                                    string quesiton = item.Question.Trim();
                                    string answer = item.Answer.Trim();
                                    if (!string.IsNullOrEmpty(quesiton) && !string.IsNullOrEmpty(answer))
                                    {
                                        newList.Add(new FAQsEntity()
                                        {
                                            ID = item.ID,
                                            Question = quesiton,
                                            Answer = answer
                                        });
                                    }
                                });
                            }

                            if (newList != null && newList.Count > 0)
                            {

                                faqs.AddRange(newList);
                                adapter = new FAQListAdapter(this, faqs);
                                mFAQRecyclerView.SetAdapter(adapter);
                                adapter.NotifyDataSetChanged();

                                if (!string.IsNullOrEmpty(FAQ_ID))
                                {
                                    int index = 0;
                                    foreach (FAQsEntity entity in newList)
                                    {
                                        if (entity.ID.Equals(FAQ_ID))
                                        {
                                            break;
                                        }
                                        index++;
                                    }
                                    mFAQRecyclerView.GetLayoutManager().ScrollToPosition(index);
                                }
                            }
                            else
                            {
                                ReadFallBackFAQ();
                            }

                        }
                        else
                        {
                            ReadFallBackFAQ();
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ReadFallBackFAQ()
        {
            try
            {
                string FaQ = "";

                if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                {
                    FaQ = FAQManager.Instance.GetFAQ(FAQManager.Language.MS);
                }
                else
                {
                    FaQ = FAQManager.Instance.GetFAQ();
                }


                List<FAQCacheModel> FAQs = JsonConvert.DeserializeObject<List<FAQCacheModel>>(FaQ);

                if (FAQs != null && FAQs != null && FAQs.Count > 0)
                {
                    List<FAQsEntity> items = new List<FAQsEntity>();

                    foreach (FAQCacheModel item in FAQs)
                    {
                        items.Add(new FAQsEntity()
                        {
                            ID = item.ID,
                            Question = item.Question,
                            Answer = item.Answer,

                        });
                    }

                    faqs.AddRange(items);

                    adapter = new FAQListAdapter(this, faqs);
                    mFAQRecyclerView.SetAdapter(adapter);
                    adapter.NotifyDataSetChanged();

                    if (!string.IsNullOrEmpty(FAQ_ID))
                    {
                        int index = 0;
                        foreach (FAQsEntity entity in items)
                        {
                            if (entity.ID.Equals(FAQ_ID))
                            {
                                break;
                            }
                            index++;
                        }
                        mFAQRecyclerView.GetLayoutManager().ScrollToPosition(index);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "FAQs");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowProgressBar()
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new FAQPresenter(this);
            try
            {
                Bundle extras = Intent.Extras;
                if (extras != null && extras.ContainsKey(Constants.FAQ_ID_PARAM))
                {
                    FAQ_ID = extras.GetString(Constants.FAQ_ID_PARAM);
                }
                ShowProgressBar();
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                mFAQRecyclerView.SetLayoutManager(layoutManager);
                adapter = new FAQListAdapter(this, faqs);
                mFAQRecyclerView.SetAdapter(adapter);
                mProgressBar.Visibility = ViewStates.Gone;
                this.userActionsListener.GetSavedFAQTimeStamp();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnSavedTimeStamp(string savedTimeStamp)
        {
            if (savedTimeStamp != null)
            {
                this.mSavedTimeStamp = savedTimeStamp;
            }
            this.userActionsListener.OnGetFAQTimeStamp();
        }

        public void ShowFAQTimestamp(bool success)
        {
            try
            {
                if (success)
                {
                    FAQsParentEntity wtManager = new FAQsParentEntity();
                    List<FAQsParentEntity> items = wtManager.GetAllItems();
                    if (items != null)
                    {
                        FAQsParentEntity entity = items[0];
                        if (entity != null)
                        {
                            if (!entity.Timestamp.Equals(mSavedTimeStamp))
                            {
                                this.userActionsListener.OnGetFAQs();
                            }
                            else
                            {
                                ShowFAQ(true);
                            }
                        }
                        else
                        {
                            this.userActionsListener.OnGetFAQs();
                        }
                    }
                    else
                    {
                        this.userActionsListener.OnGetFAQs();
                    }

                }
                else
                {
                    ShowFAQ(false);
                }
            }
            catch (Exception e)
            {
                ShowFAQ(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}