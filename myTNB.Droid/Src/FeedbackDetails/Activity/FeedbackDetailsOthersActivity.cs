﻿using System;
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
using myTNB_Android.Src.FeedbackDetails.MVP;
using myTNB_Android.Src.Base.Models;
using CheeseBind;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using myTNB_Android.Src.FeedbackDetails.Adapter;
using Newtonsoft.Json;
using myTNB_Android.Src.Utils;
using Android.Preferences;
using myTNB_Android.Src.FeedbackFullScreenImage.Activity;
using Android.Support.V4.Content;

namespace myTNB_Android.Src.FeedbackDetails.Activity
{
    [Activity(Label = "@string/feedback_others_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Others")]
    public class FeedbackDetailsOthersActivity : BaseToolbarAppCompatActivity , FeedbackDetailsContract.Others.IView
    {
       

        [BindView(Resource.Id.txtInputLayoutFeedbackId)]
        TextInputLayout txtInputLayoutFeedbackId;

        [BindView(Resource.Id.txtInputLayoutDateTime)]
        TextInputLayout txtInputLayoutDateTime;

        [BindView(Resource.Id.txtInputLayoutFeedbackType)]
        TextInputLayout txtInputLayoutFeedbackType;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

        [BindView(Resource.Id.txtInputLayoutStatus)]
        TextInputLayout txtInputLayoutStatus;

        [BindView(Resource.Id.txtFeedbackId)]
        EditText txtFeedbackId;

        [BindView(Resource.Id.txtFeedbackStatus)]
        EditText txtFeedbackStatus;

        [BindView(Resource.Id.txtFeedbackDateTime)]
        EditText txtFeedbackDateTime;

        [BindView(Resource.Id.txtFeedbackType)]
        EditText txtFeedbackType;

        [BindView(Resource.Id.txtFeedback)]
        EditText txtFeedback;

        [BindView(Resource.Id.txtRelatedScreenshotTitle)]
        TextView txtRelatedScreenshotTitle;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;


        FeedbackDetailsContract.Others.IUserActionsListener userActionsListener;
        FeedbackDetailsOthersPresenter mPresenter;

        FeedbackImageRecyclerAdapter adapter;

        GridLayoutManager layoutManager;

        SubmittedFeedbackDetails submittedFeedback;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackDetailsOthersView;
        }

        public void SetPresenter(FeedbackDetailsContract.Others.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowImages(List<AttachedImage> list)
        {
            adapter.AddAll(list);
            if (list.Count <= 0)
            {
                txtRelatedScreenshotTitle.Visibility = ViewStates.Gone;
            }
            else
            {
                txtRelatedScreenshotTitle.Visibility = ViewStates.Visible;
            }
        }

        public void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string feedbackType, string feedback)
        {
            txtFeedbackId.Text = feedbackId;
            if (feedbackCode.Equals("CL01"))
            {
                txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.createdColor)));
            }
            else if (feedbackCode.Equals("CL02"))
            {
                txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.inProgressColor)));
            }
            else if (feedbackCode.Equals("CL03"))
            {
                txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
            }
            else if (feedbackCode.Equals("CL04"))
            {
                txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
            }
            else if (feedbackCode.Equals("CL06"))
            {
                txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.cancelledColor)));
            }
            txtFeedbackStatus.Text = feedbackStatus;
            txtFeedbackDateTime.Text = dateTime;
            txtFeedbackType.Text = feedbackType;
            txtFeedback.Text = feedback;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            string selectedFeedback = UserSessions.GetSelectedFeedback(PreferenceManager.GetDefaultSharedPreferences(this));
            submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutDateTime , txtInputLayoutFeedbackId, txtInputLayoutFeedbackType, txtInputLayoutFeedback , txtInputLayoutStatus);
            TextViewUtils.SetMuseoSans300Typeface(txtFeedbackId , txtFeedbackDateTime,  txtFeedbackType, txtFeedback, txtRelatedScreenshotTitle , txtFeedbackStatus);
    

            adapter = new FeedbackImageRecyclerAdapter(true);
            layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);

            adapter.SelectClickEvent += Adapter_SelectClickEvent;

            mPresenter = new FeedbackDetailsOthersPresenter(this , submittedFeedback);
            this.userActionsListener.Start();

        }

        private void Adapter_SelectClickEvent(object sender, int e)
        {
            AttachedImage selectedImage = adapter.GetItemObject(e);
            var fullImageIntent = new Intent(this, typeof(FeedbackDetailsFullScreenImageActivity));
            fullImageIntent.PutExtra(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE, JsonConvert.SerializeObject(selectedImage));
            StartActivity(fullImageIntent);
        }
    }
}