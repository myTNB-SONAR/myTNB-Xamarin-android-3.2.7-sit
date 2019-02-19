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
using myTNB_Android.Src.SelectFeedbackType.MVP;
using myTNB_Android.Src.SelectFeedbackType.Adapter;

namespace myTNB_Android.Src.SelectFeedbackType.Activity
{
    [Activity(Label = "@string/select_feedback_type_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.SelectFeedbackStateStyle")]
    public class SelectFeedbackTypeActivity : BaseToolbarAppCompatActivity , SelectFeedbackTypeContract.IView
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

            mPresenter = new SelectFeedbackTypePresenter(this);

            adapter = new SelectFeedbackTypeAdapter(this, true);
            listView.Adapter = adapter;


            mPresenter = new SelectFeedbackTypePresenter(this);
            this.userActionsListener.Start();
        }

        [OnItemClick(Resource.Id.listView)]
        void OnItemClick(object sender, AdapterView.ItemClickEventArgs eventArgs)
        {
            FeedbackType newType = adapter.GetItemObject(eventArgs.Position);
            this.userActionsListener.OnSelect(newType);
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
            SetResult(Result.Ok , successIntent);
            Finish();
        }

    }
}