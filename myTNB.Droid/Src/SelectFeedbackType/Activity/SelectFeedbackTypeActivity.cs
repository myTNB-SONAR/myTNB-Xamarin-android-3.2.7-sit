using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.SelectFeedbackType.Adapter;
using myTNB.Android.Src.SelectFeedbackType.MVP;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB.Android.Src.SelectFeedbackType.Activity
{
    [Activity(Label = "@string/select_feedback_type_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.SelectFeedbackStateStyle")]
    public class SelectFeedbackTypeActivity : BaseToolbarAppCompatActivity, SelectFeedbackTypeContract.IView
    {

        private SelectFeedbackTypeContract.IUserActionsListener userActionsListener;
        SelectFeedbackTypePresenter mPresenter;

        SelectFeedbackTypeAdapter adapter;

        [BindView(Resource.Id.listView)]
        ListView listView;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                mPresenter = new SelectFeedbackTypePresenter(this);

                adapter = new SelectFeedbackTypeAdapter(this, true);
                listView.Adapter = adapter;


                mPresenter = new SelectFeedbackTypePresenter(this);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnItemClick(Resource.Id.listView)]
        void OnItemClick(object sender, AdapterView.ItemClickEventArgs eventArgs)
        {
            try
            {
                FeedbackType newType = adapter.GetItemObject(eventArgs.Position);
                this.userActionsListener.OnSelect(newType);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectFeedbackTypeView;
        }

        public void SetPresenter(SelectFeedbackTypeContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowList(List<FeedbackType> feedbackTypeList)
        {
            adapter.AddAll(feedbackTypeList);
        }

        public void ShowSelectedSuccess(FeedbackType feedbackType)
        {
            Intent successIntent = new Intent();
            successIntent.PutExtra(Constants.SELECTED_FEEDBACK_TYPE, JsonConvert.SerializeObject(feedbackType));
            SetResult(Result.Ok, successIntent);
            Finish();
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