using Android.App;
using Android.OS;



using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.ApplicationStatusRating.Activity;
using myTNB_Android.Src.ApplicationStatusRating.Adapter;
using myTNB_Android.Src.ApplicationStatusRating.Model;
using myTNB_Android.Src.ApplicationStatusRating.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB.Mobile.API.Managers.Rating;
using static myTNB_Android.Src.ApplicationStatusRating.Model.RateUsQuestion;
using System.Linq;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatusRating.Fargment
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
        private List<AnswerDetail> answerDetails = new List<AnswerDetail>();
        private RatingActivity ratingActivity;
        private GetCustomerRatingMasterResponse response;
     

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        public async void GetCustomerRatingMaster(View mainView)
        {
          response = await RatingManager.Instance.GetCustomerRatingMaster();
            //var json = JsonConvert.SerializeObject(response.Content);
            recyclerView = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view);
            rootView = mainView.FindViewById<FrameLayout>(Resource.Id.baseView);
            btnSubmit = mainView.FindViewById<Button>(Resource.Id.btnSubmit);

            btnSubmit.Text = Utility.GetLocalizedCommonLabel("submit");

            layoutManager = new GridLayoutManager(Activity.ApplicationContext, 1);
            Dictionary<string, string> answerDetailDictionary = new Dictionary<string, string>();
            foreach (var questionAnswerSet in response.Content.QuestionAnswerSets)
            {
                if (questionAnswerSet.RateType == RateType.Star)
                {
                    answerDetailDictionary = questionAnswerSet.AnswerDetail.AnswerSetValue;
                }
            }
            var items = answerDetailDictionary.ToList();
            foreach (var item in items)
            {
                AnswerDetail answerDetail = new AnswerDetail();
                answerDetail.Key = item.Key;
                answerDetail.answerValue = item.Value;
                answerDetails.Add(answerDetail);
            }

            adapter = new RateUsQuestionsAdapter(Activity.ApplicationContext, response.Content.QuestionAnswerSets[0], 3);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);
            adapter.RatingUpdate += OnRatingUpdate;
            //ShowGetQuestionSuccess();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.ApplicationStatus_SubmitRatingView, container, false);
            try
            {
                mPresenter = new SubmitRatingPresenter(this);
                ratingActivity = ((RatingActivity)Activity);

               



                GetCustomerRatingMaster(mainView);
                



                

                btnSubmit.Click += delegate
                {
                    if (adapter.GetInputAnswers().Count > 0)
                    {
                        //this.userActionsListener.PrepareSubmitRateUsRequest(merchantTransId, deviceID, adapter.GetInputAnswers());
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
                        //foreach (RateUsQuestion que in response.GetData())
                        {
                          //  if (que.IsActive)
                            {
                           //     activeQuestionList.Add(que);
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
            Bundle bundle = new Bundle();
            ratingActivity.nextFragment(this, bundle);
        }
    }
}
