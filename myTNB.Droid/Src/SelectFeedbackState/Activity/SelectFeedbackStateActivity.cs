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
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.SelectFeedbackState.MVP;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.SelectFeedbackState.Adapter;
using CheeseBind;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SelectFeedbackState.Activity
{
    [Activity(Label = "@string/select_feedback_state_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.SelectFeedbackStateStyle")]
    public class SelectFeedbackStateActivity : BaseToolbarAppCompatActivity , SelectFeedbackStateContract.IView
    {

        private SelectFeedbackStateContract.IUserActionsListener userActionsListener;
        SelectFeedbackStatePresenter mPresenter;

        SelectFeedbackStateAdapter adapter;

        [BindView(Resource.Id.listView)]
        ListView listView;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            mPresenter = new SelectFeedbackStatePresenter(this);

            adapter = new SelectFeedbackStateAdapter(this, true);
            listView.Adapter = adapter;


            mPresenter = new SelectFeedbackStatePresenter(this);
            this.userActionsListener.Start();
        }

        [OnItemClick(Resource.Id.listView)]
        void OnItemClick(object sender, AdapterView.ItemClickEventArgs eventArgs)
        {
            FeedbackState newState = adapter.GetItemObject(eventArgs.Position);
            this.userActionsListener.OnSelect(newState);
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectFeedbackStateView;
        }

        public void SetPresenter(SelectFeedbackStateContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowList(List<FeedbackState> feedbackStateList)
        {
            adapter.AddAll(feedbackStateList);
        }

        public void ShowSelectedSuccess(FeedbackState feedbackState)
        {
            Intent successIntent = new Intent();
            successIntent.PutExtra(Constants.SELECTED_FEEDBACK_STATE, JsonConvert.SerializeObject(feedbackState));
            SetResult(Result.Ok , successIntent);
            Finish();
        }

    }
}