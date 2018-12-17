using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using myTNB_Android.Src.FAQ.MVP;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Models;
using myTNB_Android.Src.FAQ.Adapter;
using Android.Support.V7.Widget;
using CheeseBind;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using myTNB_Android.Src.Utils;
using System.Runtime;

namespace myTNB_Android.Src.FAQ.Activity
{
    [Activity(Label = "@string/faq_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.FAQ")]
    public class FAQListActivity : BaseToolbarAppCompatActivity, FAQContract.IView
    {

        private FAQPresenter mPresenter;
        private FAQContract.IUserActionsListener userActionsListener;

        List<FAQsModel> faqs = new List<FAQsModel>();
        private FAQListAdapter adapter;
        private LinearLayoutManager layoutManager;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        private LoadingOverlay loadingOverlay;

        [BindView(Resource.Id.faq_list_recycler_view)]
        public RecyclerView mFAQRecyclerView;

        private string mSavedTimeStamp = "0000000";

        private string FAQ_ID = null;

        public void HideProgressBar()
        {
            try
            {
                //mProgressBar.Visibility = ViewStates.Gone;
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            } catch(Exception e) {
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
            RunOnUiThread(() =>
            {
                HideProgressBar();
                if (success)
                {
                    FAQsEntity wtManager = new FAQsEntity();
                    List<FAQsEntity> items = wtManager.GetAllItems();
                    if (items != null)
                    {

                        faqs.AddRange(items);
                        adapter = new FAQListAdapter(this, faqs);
                        mFAQRecyclerView.SetAdapter(adapter);
                        adapter.NotifyDataSetChanged();

                        if (!string.IsNullOrEmpty(FAQ_ID))
                        {
                            int index = 0;
                            foreach(FAQsEntity entity in items)
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
                else
                {

                }
            });
        }

        public void ShowProgressBar()
        {
            //mProgressBar.Visibility = ViewStates.Visible;
            try {
            if(loadingOverlay != null)
            {
                loadingOverlay.Show();
            }
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

                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                mFAQRecyclerView.SetLayoutManager(layoutManager);
                adapter = new FAQListAdapter(this, faqs);
                mFAQRecyclerView.SetAdapter(adapter);

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                mProgressBar.Visibility = ViewStates.Gone;
                ShowProgressBar();
                this.userActionsListener.GetSavedFAQTimeStamp();
            } catch (Exception e)
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
            try {
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
                }

            }
            else
            {
                ShowFAQ(false);
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}