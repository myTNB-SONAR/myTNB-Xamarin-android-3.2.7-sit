using Android.Content;
using Android.OS;
using Android.Views;
using myTNB.Android.Src.Utils;

using static myTNB.Android.Src.Utils.Constants;
using myTNB.SitecoreCMS.Model;
using System.Collections.Generic;
using Android.Preferences;
using System;
using myTNB.Android.Src.myTNBMenu.Fragments.WhatsNewMenu.Adapter;
using myTNB.Android.Src.WhatsNewDetail.MVP;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;

namespace myTNB.Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP
{
    public class WhatsNewItemFragment : Fragment, WhatsNewItemContract.IWhatsNewItemView
    {
        RecyclerView mWhatsNewRecyclerView;
        WhatsNewRecyclerAdapter mWhatsNewRecyclerAdapter;
        List<WhatsNewModel> mWhatsNewList = new List<WhatsNewModel>();
        WHATSNEWITEMLISTMODE mListMode;
        WhatsNewItemContract.IWhatsNewItemPresenter presenter;
        string mWhatsNewSearchKey = "";
        bool initializeComplete = false;

        private bool isClicked = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            presenter = new WhatsNewItemPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));
            if (Arguments != null && Arguments.ContainsKey(Constants.WHATSNEW_ITEM_LIST_MODE))
            {
                mListMode = (WHATSNEWITEMLISTMODE)Arguments.GetInt(Constants.WHATSNEW_ITEM_LIST_MODE);
            }
            else
            {
                mListMode = WHATSNEWITEMLISTMODE.INITIATE;
            }

            mWhatsNewList = new List<WhatsNewModel>();

            if (mListMode == WHATSNEWITEMLISTMODE.INITIATE)
            {
                mWhatsNewList = this.presenter.InitializeWhatsNewList();
            }
            else
            {
                if (Arguments != null && Arguments.ContainsKey(Constants.WHATSNEW_ITEM_LIST_SEARCH_STRING_KEY))
                {
                    mWhatsNewSearchKey = Arguments.GetString(Constants.WHATSNEW_ITEM_LIST_SEARCH_STRING_KEY);
                }

                if (mWhatsNewSearchKey != "")
                {
                    mWhatsNewList = this.presenter.GetActiveWhatsNewList(mWhatsNewSearchKey);
                }
                else
                {
                    mWhatsNewList = this.presenter.GetActiveWhatsNewList();
                }
            }
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return IsAdded && IsVisible && !IsDetached && !IsRemoving;
        }

        public override void OnPause()
        {
            base.OnPause();
            this.isClicked = true;
        }

        public override void OnResume()
        {
            base.OnResume();
            this.isClicked = false;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.WhatsNewListItemLayout, container, false);

            mWhatsNewRecyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.whatsNewRecyclerView);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
            mWhatsNewRecyclerView.SetLayoutManager(linearLayoutManager);
            ((SimpleItemAnimator)mWhatsNewRecyclerView.GetItemAnimator()).SupportsChangeAnimations = false;

            mWhatsNewRecyclerAdapter = new WhatsNewRecyclerAdapter(mWhatsNewList, this.Activity, mListMode);
            mWhatsNewRecyclerView.SetAdapter(mWhatsNewRecyclerAdapter);
            mWhatsNewRecyclerView.OverScrollMode = OverScrollMode.Never;
            mWhatsNewRecyclerAdapter.ClickChanged += mWhatsNewRecyclerAdapter_ClickChanged;
            initializeComplete = true;
            return rootView;
        }

        private void mWhatsNewRecyclerAdapter_ClickChanged(object sender, int e)
        {
            if (!isClicked && e != -1)
            {
                isClicked = true;

                if (!mWhatsNewList[e].Read)
                {
                    mWhatsNewList[e].Read = true;
                    mWhatsNewRecyclerAdapter.NotifyItemChanged(e);
                    this.presenter.UpdateWhatsNewRead(mWhatsNewList[e].ID, mWhatsNewList[e].Read);
                }

                WhatsNewMenuUtils.OnSetRefreshAll(true);

                Intent activity = new Intent(this.Activity, typeof(WhatsNewDetailActivity));
                activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, mWhatsNewList[e].ID);
                activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                StartActivity(activity);
            }
        }

        public void Refresh()
        {
            if (initializeComplete)
            {
                if (mListMode == WHATSNEWITEMLISTMODE.LOADED)
                {
                    if (mWhatsNewSearchKey != "")
                    {
                        mWhatsNewList = this.presenter.GetActiveWhatsNewList(mWhatsNewSearchKey);
                    }
                    else
                    {
                        mWhatsNewList = this.presenter.GetActiveWhatsNewList();
                    }

                    mWhatsNewRecyclerAdapter.RefreshList(mWhatsNewList);
                }
            }
        }

        public int GetFirstItemRelativePosition()
        {
            int i = 0;

            try
            {
                View firstView = mWhatsNewRecyclerView.GetChildAt(0);
                int[] location = new int[2];
                firstView.GetLocationOnScreen(location);
                i = location[1];
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public int GetFirstItemHeight()
        {
            int i = 0;

            try
            {
                View firstView = mWhatsNewRecyclerView.GetChildAt(0);
                i = firstView.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public void StopScrolling()
        {
            try
            {
                mWhatsNewRecyclerView.SmoothScrollBy(0, 0);
                mWhatsNewRecyclerView.ScrollTo(0, 0);
                mWhatsNewRecyclerView.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void WhatsNewCustomScrolling(int yPosition)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        mWhatsNewRecyclerView.ScrollTo(0, yPosition);
                        mWhatsNewRecyclerView.RequestLayout();
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
    }
}
