using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.MyHome;
using myTNB.Mobile;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Rating.Adapter;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.Rating.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.Rating.Fargment
{
    public class SubmitRatingFragment : AndroidX.Fragment.App.Fragment, SubmitRatingContract.IView
    {

        private SubmitRatingPresenter mPresenter;
        private SubmitRatingContract.IUserActionsListener userActionsListener;

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

        private DetailCTAType _ctaType;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.SubmitRatingView, container, false);
            try
            {
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
                if (Arguments.ContainsKey(MyHomeConstants.CTA_TYPE))
                {
                    _ctaType = JsonConvert.DeserializeObject<DetailCTAType>(Arguments.GetString(MyHomeConstants.CTA_TYPE));
                }

                recyclerView = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view);
                rootView = mainView.FindViewById<FrameLayout>(Resource.Id.baseView);
                btnSubmit = mainView.FindViewById<Button>(Resource.Id.btnSubmit);

                TextViewUtils.SetMuseoSans300Typeface(btnSubmit);
                TextViewUtils.SetTextSize16(btnSubmit);

                btnSubmit.Text = Utility.GetLocalizedCommonLabel("submit");

                layoutManager = new GridLayoutManager(Activity.ApplicationContext, 1);
                adapter = new RateUsQuestionsAdapter(Activity.ApplicationContext, activeQuestionList, selectedRating);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);
                adapter.RatingUpdate += OnRatingUpdate;

                this.userActionsListener.GetQuestions(questionCatId);

                btnSubmit.Click += delegate
                {
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
            try
            {
                if (adapter != null)
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
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public void ShowError(string exception)
        {
            Snackbar showErrorSnackbar = Snackbar.Make(rootView, exception, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                (view) =>
                {

                    // EMPTY WILL CLOSE SNACKBAR
                }
            );
            View v = showErrorSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            showErrorSnackbar.Show();
        }

        public void ShowGetQuestionSuccess(GetRateUsQuestionResponse response)
        {
            try
            {
                if (response != null)
                {
                    if (response.GetData().Count > 0)
                    {
                        foreach (RateUsQuestion que in response.GetData())
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
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
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
            if (_ctaType == DetailCTAType.SubmitApplicationRating)
            {
                ratingActivity.OnNavigateToApplicationDetails();
            }
            else
            {
                Bundle bundle = new Bundle();
                ratingActivity.nextFragment(this, bundle);
            }
        }
    }
}
