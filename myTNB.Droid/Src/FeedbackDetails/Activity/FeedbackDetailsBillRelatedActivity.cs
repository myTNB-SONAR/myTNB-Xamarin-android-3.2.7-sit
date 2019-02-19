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
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using Android.Support.Design.Widget;
using CheeseBind;
using Android.Support.V7.Widget;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;
using myTNB_Android.Src.FeedbackDetails.Adapter;
using myTNB_Android.Src.FeedbackDetails.MVP;
using Android.Preferences;
using myTNB_Android.Src.FeedbackFullScreenImage.Activity;
using Android.Support.V4.Content;

namespace myTNB_Android.Src.FeedbackDetails.Activity
{
    [Activity(Label = "@string/bill_related_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.BillRelated")]
    public class FeedbackDetailsBillRelatedActivity : BaseToolbarAppCompatActivity , FeedbackDetailsContract.BillRelated.IView
    {

        [BindView(Resource.Id.txtInputLayoutFeedbackId)]
        TextInputLayout txtInputLayoutFeedbackId;

        [BindView(Resource.Id.txtInputLayoutDateTime)]
        TextInputLayout txtInputLayoutDateTime;

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

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

        [BindView(Resource.Id.txtAccountNo)]
        EditText txtAccountNo;

        [BindView(Resource.Id.txtFeedback)]
        EditText txtFeedback;

        [BindView(Resource.Id.txtRelatedScreenshotTitle)]
        TextView txtRelatedScreenshotTitle;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;


        FeedbackImageRecyclerAdapter adapter;

        GridLayoutManager layoutManager;

        SubmittedFeedbackDetails submittedFeedback;

        
        FeedbackDetailsBillRelatedPresenter mPresenter;
        FeedbackDetailsContract.BillRelated.IUserActionsListener userActionsListener;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackDetailsBillRelatedView;
        }

        public void SetPresenter(FeedbackDetailsContract.BillRelated.IUserActionsListener userActionListener)
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


        public void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string accountNoName, string feedback)
        {
            txtFeedbackId.Text = feedbackId;
            txtFeedbackStatus.Text = feedbackStatus;

            if (feedbackCode.Equals("CL01"))
            {
                txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this , Resource.Color.createdColor)));
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

            txtFeedbackDateTime.Text = dateTime;
            txtAccountNo.Text = accountNoName;
            txtFeedback.Text = feedback;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            string selectedFeedback = UserSessions.GetSelectedFeedback(PreferenceManager.GetDefaultSharedPreferences(this));
            submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNo, txtInputLayoutFeedback , txtInputLayoutFeedbackId , txtInputLayoutDateTime , txtInputLayoutStatus);
            TextViewUtils.SetMuseoSans300Typeface(txtFeedbackId, txtFeedbackDateTime,  txtAccountNo, txtFeedback, txtRelatedScreenshotTitle , txtFeedbackStatus);

            adapter = new FeedbackImageRecyclerAdapter(true);
            layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);
            adapter.SelectClickEvent += Adapter_SelectClickEvent;

            mPresenter = new FeedbackDetailsBillRelatedPresenter(this , submittedFeedback);
            this.userActionsListener.Start();

        }

        private void Adapter_SelectClickEvent(object sender, int e)
        {
            AttachedImage selectedImage = adapter.GetItemObject(e);
            var fullImageIntent = new Intent(this , typeof(FeedbackDetailsFullScreenImageActivity));
            fullImageIntent.PutExtra(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE , JsonConvert.SerializeObject(selectedImage));
            StartActivity(fullImageIntent);
        }
    }
}