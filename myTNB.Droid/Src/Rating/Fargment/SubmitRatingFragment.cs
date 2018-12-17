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
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Rating.MVP;
using myTNB_Android.Src.Rating.Response;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Android.Support.V7.Widget;
using CheeseBind;
using myTNB_Android.Src.Rating.Adapter;
using Android.Support.V4.Content;
using myTNB_Android.Src.Rating.Model;
using Android.Support.Design.Widget;
using myTNB_Android.Src.Rating.Request;
using myTNB_Android.Src.Rating.Activity;

namespace myTNB_Android.Src.Rating.Fargment
{
    public class SubmitRatingFragment : Fragment, SubmitRatingContract.IView
    {

        private SubmitRatingPresenter mPresenter;
        private SubmitRatingContract.IUserActionsListener userActionsListener;

        private LoadingOverlay loadingOverlay;


        private FrameLayout rootView;

        private RecyclerView recyclerView;

        private Button btnSubmit;

        private GridLayoutManager layoutManager;

        private RateUsQuestionsAdapter adapter;

        private List<RateUsQuestion> activeQuestionList = new List<RateUsQuestion>();

        private RatingActivity ratingActivity;
        private string merchantTransId;
        private string deviceID;
        private int selectedRating;
        private string questionCatId;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.SubmitRatingView, container, false);
            try {
            mPresenter = new SubmitRatingPresenter(this);
            ratingActivity = ((RatingActivity)Activity);

            if (Arguments.ContainsKey(Constants.QUESTION_ID_CATEGORY))
            {
                questionCatId = Arguments.GetString(Constants.QUESTION_ID_CATEGORY);
            }
            if (Arguments.ContainsKey(Constants.DEVICE_ID_PARAM))
            {
                deviceID = Arguments.GetString(Constants.DEVICE_ID_PARAM);
            }
            if (Arguments.ContainsKey(Constants.MERCHANT_TRANS_ID))
            {
                merchantTransId = Arguments.GetString(Constants.MERCHANT_TRANS_ID);
            }
            if (Arguments.ContainsKey(Constants.SELECTED_RATING))
            {
                selectedRating = Arguments.GetInt(Constants.SELECTED_RATING);
            }

            recyclerView = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view);
            rootView = mainView.FindViewById<FrameLayout>(Resource.Id.baseView);
            btnSubmit = mainView.FindViewById<Button>(Resource.Id.btnSubmit);

            layoutManager = new GridLayoutManager(Activity.ApplicationContext, 1);
            adapter = new RateUsQuestionsAdapter(Activity.ApplicationContext, activeQuestionList, selectedRating);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);
            adapter.RatingUpdate += OnRatingUpdate;

           

            this.userActionsListener.GetQuestions(questionCatId);

            btnSubmit.Click += delegate {
                if (adapter.GetInputAnswers().Count > 0)
                {
                    this.userActionsListener.PrepareSubmitRateUsRequest(merchantTransId, deviceID, adapter.GetInputAnswers());
                }
            };
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return mainView;
        }

        public void SetPresenter(SubmitRatingContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        void OnRatingUpdate(object sender, int position)
        {
            try {
            if(adapter != null)
            {
                if (adapter.IsAllQuestionAnswered())
                {
                    EnableSubmitButton();
                }
                else
                {
                    DisableSubmitButton();
                }
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableSubmitButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableSubmitButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.green_button_background);
        }

        public void HideProgressDialog()
        {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public void ShowError(string exception)
        {
            Snackbar.Make(rootView, exception, Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.manage_cards_btn_close),
                (view) =>
                {

                    // EMPTY WILL CLOSE SNACKBAR
                }
            ).Show();
        }

        public void ShowGetQuestionSuccess(GetRateUsQuestionsResponse response)
        {
            try {
            if(response != null)
            {
                if(response.feedbackQuestionStatus.rateUsQuestionList.Count > 0)
                {
                    foreach(RateUsQuestion que in response.feedbackQuestionStatus.rateUsQuestionList)
                    {
                        if (que.IsActive)
                        {
                            activeQuestionList.Add(que);
                        }
                    }
                    adapter.NotifyDataSetChanged();
                }
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowProgressDialog()
        {
            try {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(Activity, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            
        }

        public void ShowSumitRateUsSuccess()
        {
            Bundle bundle = new Bundle();
            ratingActivity.nextFragment(this, bundle);
        }
    }
}